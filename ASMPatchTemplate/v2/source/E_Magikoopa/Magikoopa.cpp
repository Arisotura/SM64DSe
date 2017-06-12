#include "Magikoopa.h"

//#define __aeabi_idivmod  Math_IDivMod
//#define __aeabi_uidivmod Math_UDivMod
namespace
{
	using StateFuncPtr = void(Magikoopa::*)();
	
	/*const, but rodata not supported in dynamic libraries yet*/ StateFuncPtr stateFuncs[]
	{
		&Magikoopa::State0_Appear,
		&Magikoopa::State1_Wave,
		&Magikoopa::State2_Shoot,
		&Magikoopa::State3_Poof,
		&Magikoopa::State4_Teleport,
		&Magikoopa::State5_Hurt,
		&Magikoopa::State6_Wait,
		&Magikoopa::State7_Defeat
	};
	
	enum Animations
	{
		APPEAR,
		WAVE,
		SHOOT,
		POOF,
		WAIT,
		HURT,
		DEFEAT,
		
		NUM_ANIMS
	};
	
	SharedFilePtr modelFiles[2];
	SharedFilePtr animFiles[NUM_ANIMS];
	
	constexpr Fix12i SCALES[] = {0x1000_f, 0x4000_f};
	
	constexpr Fix12i RADIUSES[] = {0x68000_f, 4 * 0x58000_f}; //sic
	constexpr Fix12i HEIGHTS [] = {0x90000_f, 4 * 0x90000_f};
	constexpr Fix12i VERT_ACCEL = 0_f;
	constexpr Fix12i TERM_VEL = 0x80000000_f;
	constexpr Fix12i SPIT_DECEL = 0x1000_f;

	constexpr Fix12i TP_PLAYER_DIST_TOL = 0x1e0000_f;
	constexpr Fix12i TP_PLAYER_DIST_MAX = 0xc00000_f;
	constexpr Fix12i BOSS_START_BATTLE_RADIUS = 0xbb8000_f;
	constexpr Fix12i BOSS_STOP_BATTLE_RADIUS = 0x1600000_f;
	
	constexpr uint16_t waitMsgIDs  [] = {0x0096, 0x0097, 0x0098, 0x0099};
	constexpr uint16_t defeatMsgIDs[] = {0x009a, 0x009b, 0x009c, 0x009d};
	constexpr Fix12i TALK_HEIGHT = 0x1d74a6_f;

	constexpr int DAMAGE = 1;
	constexpr Fix12i PLAYER_KNOCKBACK = 0xc000_f;
	constexpr Vector3_16 KNOCKBACK_ROT = {0x1800, 0x0000, 0x0000};
	constexpr int NUM_COINS = 3;
	constexpr Fix12i PLAYER_BOUNCE_INIT_VEL = 0x28000_f;

	constexpr uint16_t APPEAR_TIME = 30;
	constexpr uint16_t WAVE_TIME = 32;
	constexpr uint16_t SHOOT_TIME = 12;
	constexpr uint16_t POOF_TIME = 30;
	constexpr uint16_t TELEPORT_TIME = 45;
	constexpr uint16_t HURT_TIME = 32;

	constexpr Fix12i WAND_LENGTH = 0x55000_f;
	
	constexpr short MOST_HORZ_SHOT_ANGLES[] = {-0x1000, 0x0000};
	
	unsigned aLoadFileInst = 0x00000000;
	Magikoopa::SharedRes* ptrToRes = nullptr;
	
	unsigned onLoadFuncPtr = (unsigned)&Magikoopa::SharedRes::OnLoadFile;
	
	Particle::MainInfo particleInfo
	{
		0x00010104, //flags
		0x00001000_f, //rate, fix20.12
		0x00000000_f, //startHorzDist, fix23.9
		Vector3_16f{0x0000_fs, 0x1000_fs, 0x0000_fs}, //dir
		Color5Bit(0xff, 0xff, 0xff), //color
		0x00000800_f, //horzSpeed, fix20.12 (fix23.9???)
		0x00000000_f, //vertSpeed, fix20.12 (fix23.9???)
		0x00002800_f, //scale, fix20.12
		0x1000_fs, //horzScale, fix4.12
		0x0000,
		0x0079,
		0x0261,
		0x0000, //frames
		0x002d, //lifetime
		0x75, //scaleRand
		0x9d, //lifetimeRand
		0xce, //speedRand
		0x00,
		0x01, //spawnPeriod
		0x1f, //alpha
		0x24, //speedFalloff
		0x2e, //spriteID
		0x03,
		0x00,
		0x00, //velStretchFactor
		0x00 //texMirrorFlags
	};
	
	Particle::MainInfo bossParticleInfo
	{
		0x00010104, //flags
		0x00001000_f, //rate, fix20.12
		0x00000000_f, //startHorzDist, fix23.9
		Vector3_16f{0x0000_fs, 0x1000_fs, 0x0000_fs}, //dir
		Color5Bit(0xff, 0xff, 0xff), //color
		0x00001000_f, //horzSpeed, fix20.12 (fix23.9???)
		0x00000000_f, //vertSpeed, fix20.12 (fix23.9???)
		0x00005000_f, //scale, fix20.12
		0x1000_fs, //horzScale, fix4.12
		0x0000,
		0x0079,
		0x0261,
		0x0000, //frames
		0x002d, //lifetime
		0xff, //scaleRand
		0x9d, //lifetimeRand
		0xce, //speedRand
		0x00,
		0x01, //spawnPeriod
		0x1f, //alpha
		0x24, //speedFalloff
		0x2e, //spriteID
		0x03,
		0x00,
		0x00, //velStretchFactor
		0x00 //texMirrorFlags
	};
	
	Particle::ScaleTransition particleScaleTrans
	{
		0x1308_fs, //scaleStart, fix4.12
		0x1000_fs, //scaleMiddle, fix4.12
		0x06a9_fs, //scaleEnd, fix4.12
		0x00, //scaleTrans1End
		0x5b, //scaleTrans2Start;
		0x0004,
		0x057b
	};
	
	Particle::Glitter particleGlitter
	{
		0x0002,
		0x0000,
		0x1000_f, //scale 1, fix4.12
		0x0002, //lifetime
		0x00,
		0x40, //scale 2
		Color5Bit(0xff, 0xff, 0xff),
		0x01, //rate
		0x04,
		0x0c, //period
		0x1c, //spriteID;
		0x00000005, //texMirrorFlags;
	};
	
	Particle::SysDef particleSysDefs[]
	{
		Particle::SysDef
		{
			&particleInfo,
			&particleScaleTrans,
			nullptr,
			nullptr,
			nullptr,
			&particleGlitter,
			nullptr,
			0
		},
		
		Particle::SysDef
		{
			&bossParticleInfo,
			&particleScaleTrans,
			nullptr,
			nullptr,
			nullptr,
			&particleGlitter,
			nullptr,
			0
		}
	};
	
}
	
SpawnInfo<Magikoopa> Magikoopa::spawnData =
{
	&Magikoopa::Spawn,
	0x00c6,
	0x0018,
	0x10000006,
	0x00032000_f,
	0x00046000_f,
	0x01000000_f,
	0x01000000_f,
};

SpawnInfo<Magikoopa> Magikoopa::bossSpawnData =
{
	&Magikoopa::Spawn,
	0x00c6,
	0x0018,
	0x10000006,
	0x000c8000_f,
	0x00118000_f,
	BOSS_STOP_BATTLE_RADIUS,
	BOSS_STOP_BATTLE_RADIUS,
};

void Magikoopa::Resources::Add(SharedFilePtr& sf)
{
	for(int i = 0; i < Magikoopa::Resources::NUM_PER_CHUNK; ++i)
	{
		if((unsigned)&sf == files[i])
			return;
		else if(!files[i])
		{
			files[i] = (unsigned)&sf | 1;
			//++files[i]->numRefs; (Can't do this here or the Model::SetFile function thinks the resource was already loaded and doesn't offset the offsets)
			return;
		}
	}
	
	next = new Resources; //TEST THIS
	if(next)
		next->Add(sf);
	else
		Crash();
}

void Magikoopa::Resources::ProcessAdditions()
{
	for(int i = 0; i < Magikoopa::Resources::NUM_PER_CHUNK; ++i) if(files[i] & 1)
	{
		SharedFilePtr& file = *(SharedFilePtr*)(files[i] & ~1);
		if(file.numRefs > 0)
		{
			files[i] &= ~1;
			++file.numRefs;
		}
		else //The resources may have been deallocated because the object failed to spawn. Don't reload them!
			files[i] = 0;
	}
	if(next)
		next->ProcessAdditions();
}

Magikoopa::Resources::~Resources()
{
	for(int i = 0; i < Magikoopa::Resources::NUM_PER_CHUNK; ++i) if (files[i])
		((SharedFilePtr*)files[i])->Release();
	if(next)
		delete next;
}

/*static*/ void Magikoopa::SharedRes::OnLoadFile(SharedFilePtr& file)
{
	ptrToRes->res.Add(file);
}
void Magikoopa::SharedRes::TrackRes()
{
	ptrToRes = this;
	aLoadFileInst = *(unsigned*)0x02017bfc;
	*(unsigned*)0x02017bfc = (onLoadFuncPtr - 0x02017bfc) / 4 - 2 & 0x00ffffff | 0xeb000000;
}
void Magikoopa::SharedRes::StopTracking()
{
	*(unsigned*)0x02017bfc = aLoadFileInst;
}

//START MAGIKOOPA SHOT
namespace
{
	constexpr Fix12i SHOT_SPEEDS[] = {0x8000_f, 0x20000_f};
	constexpr uint16_t SHOT_MAX_OFFSCREEN_TIME = 45;
	
	constexpr Fix12i SHOT_RADIUS = 0x30000_f;
}
SpawnInfo<Magikoopa::Shot> Magikoopa::Shot::spawnData =
{
	&Magikoopa::Shot::Spawn,
	0x016c,
	0x0018,
	0x00000006,
	0x00032000_f,
	0x00046000_f,
	0x01000000_f,
	0x01000000_f,
};

void Magikoopa::Shot::SetMagikoopa(Magikoopa& magik)
{
	res = magik.res;
	shotState = magik.shotState;
	numFireToSpawn = 4 - magik.health;
	++res->numRefs;
	
	speed = Vector3{(Fix12i)direction.x * SHOT_SPEEDS[res->isBoss],
					(Fix12i)direction.y * SHOT_SPEEDS[res->isBoss],
					(Fix12i)direction.z * SHOT_SPEEDS[res->isBoss]};
}

Magikoopa::Shot* Magikoopa::Shot::Spawn()
{
	static_assert(sizeof(Particle::Particle) == 0x44, "Particle size is WRONG!");
	return new Magikoopa::Shot;
}
int Magikoopa::Shot::InitResources()
{
	resourceRefCount = 0;
	
	direction = Vector3_16f{Fix12s(ang.x, true), Fix12s(ang.y, true), Fix12s(ang.z, true)};
	ang.x = ang.z = 0;
	ang.y = Atan2(direction.x, direction.z);
	
	cylClsn.Init(this, SHOT_RADIUS, SHOT_RADIUS * 2, 0x00200000, 0x00000000);
	wmClsn.Init(this, SHOT_RADIUS, SHOT_RADIUS, nullptr, nullptr);
	wmClsn.SetFlag_2();
	
	stateTimer = 0;
	
	return 1;
}
int Magikoopa::Shot::CleanupResources()
{
	if(res->numRefs == 1)
		delete res;
	else
		--res->numRefs;
	return 1;
}
int Magikoopa::Shot::Behavior()
{
	UpdatePosWithOnlySpeed(nullptr);
	shapesID = Particle::System::New(shapesID, (unsigned)&particleSysDefs[res->isBoss], pos.x, pos.y + SHOT_RADIUS, pos.z, &direction, nullptr);
	UpdateWMClsn(wmClsn, 0);
	
	Actor* actor = Actor::FindWithID(cylClsn.otherObjID);
	if(wmClsn.IsOnGround() || wmClsn.IsOnWall() || /*ADD CHECK FOR CEILING*/ (actor && actor->actorID != 0xc6 && actor->actorID != 0x170))
	{
		res->TrackRes();
		Actor* newActor;
		Vector3_16 fireAngle = {0x1000, ang.y, 0x0000};
		if(!res->isBoss)
			newActor = Actor::Spawn(res->spawnActorID, res->spawnActorParams, pos, &ang, areaID, -1);
		else if(shotState == 0)
		{
			for(int i = 0; i < numFireToSpawn; ++i)
			{
				newActor = Actor::Spawn(0x00fe, 0x0003, pos, &fireAngle, areaID, -1);
				if(newActor)
				{
					newActor->horzSpeed = 0x1e000_f;
					*(Fix12i*)((char*)newActor + 0x35c) = 0xa000_f; //timer
				}
				fireAngle.y += 0x10000 / numFireToSpawn;
			}
		}
		else
			newActor = Actor::Spawn(0x00c8, 0xffff, pos, &ang, areaID, -1);
		
		res->StopTracking();
		res->res.ProcessAdditions();
		if(newActor && param1 >= 0)
			res->shotUniqueIDs[param1] = newActor->uniqueID;
		PoofDustAt(Vector3{pos.x, pos.y + SHOT_RADIUS, pos.z});
		Destroy();
	}
	if((flags & Actor::OFF_SCREEN))
	{
		++stateTimer;
		if(stateTimer >= SHOT_MAX_OFFSCREEN_TIME)
			Destroy();
	}
	else
		stateTimer = 0;
	
	cylClsn.Clear();
	cylClsn.Update();
	MakeVanishLuigiWork(cylClsn);
	return 1;
}
int Magikoopa::Shot::Render() {return 1;}
void Magikoopa::Shot::Virtual30() {}
Magikoopa::Shot::~Shot() {}

//END MAGIKOOPA SHOT

Magikoopa* Magikoopa::Spawn()
{
	return new Magikoopa;
}

void Magikoopa::UpdateModelTransform()
{
	Matrix4x3& modelMat = rigMdl.mat4x3;
	
	modelMat = Matrix4x3::FromRotationY(ang.y) * Matrix4x3::FromScale(SCALES[res->isBoss], SCALES[res->isBoss], SCALES[res->isBoss]);
	modelMat.r0c3 = pos.x >> 3;
	modelMat.r1c3 = pos.y >> 3;
	modelMat.r2c3 = pos.z >> 3;
	
	if(!(flags & 0x40000))
		DropShadowRadHeight(shadow, modelMat, RADIUSES[0], 0x320000_f / SCALES[res->isBoss], 0xf);
}

int Magikoopa::InitResources()
{
	if(!(res = new SharedRes)) //the use of 1 equal sign was intentional
		return 0;
		
	res->isBoss = actorID == 0x170 ? 1 : 0;
	shotState = 0;
	
	char* modelF = Model::LoadFile(modelFiles[res->isBoss]);
	for(int i = 0; i < NUM_ANIMS; ++i)
		BoneAnimation::LoadFile(animFiles[i]);
	
	if(!rigMdl.SetFile(modelF, 1, -1))
		return 0;
	
	if(!shadow.InitCylinder())
		return 0;
	
	rigMdl.SetAnim(animFiles[res->isBoss ? WAIT : APPEAR].filePtr, Animation::NO_LOOP, 0x1000_f, 0);
		
	cylClsn.Init(this, RADIUSES[res->isBoss], HEIGHTS[res->isBoss], 0x00200000, 0x00a6efe0);
	wmClsn.Init(this, RADIUSES[res->isBoss], HEIGHTS[res->isBoss] >> 1, nullptr, nullptr);
	wmClsn.SetFlag_2();
	
	scale = Vector3{0x1000_f, 0x1000_f, 0x1000_f};
	
	UpdateModelTransform();
	
	coinType = Enemy::CN_YELLOW;
	numCoinsMinus1 = NUM_COINS - 1;
	
	vertAccel = VERT_ACCEL;
	termVel = TERM_VEL;
	
	eventToTrigger = param1 >> 8 & 0xff;
	pathPtr.FromID(param1 & 0xff);
	numPathPts = pathPtr.NumPts();
	currPathPt = numPathPts; //hax
	
	res->spawnActorID = ang.x;
	res->spawnActorParams = ang.z;
	ang.x = ang.z = 0;
	originalPos = pos;
	
	res->TrackRes();
	Actor* newActor;
	//avoid the glitch where particles mess up if all the whatevers are killed before the Magikoopa spawns a whatever.
	//Fire uses only particles, so it doesn't count.
	if(res->isBoss)
		if((newActor = Actor::Spawn(0x00c8, 0xffff, pos, nullptr, 0, -1)))
			newActor->Destroy();
	if(!res->isBoss)
		if((newActor = Actor::Spawn(res->spawnActorID, res->spawnActorParams, pos, nullptr, 0, -1)))
			newActor->Destroy();
	res->StopTracking();
	res->res.ProcessAdditions();
	
	state = 4;
	
	health = 3;
	battleStarted = false;
	invincible = true;
	
	return 1;
}

int Magikoopa::CleanupResources()
{
	for(int i = 0; i < NUM_ANIMS; ++i)
		animFiles[i].Release();
	modelFiles[res->isBoss].Release();
	
	if(res->numRefs == 1)
		delete res;
	else
		--res->numRefs;
	
	return 1;
}

void Magikoopa::KillMagikoopa()
{
	//The coins have already spawned.
	KillAndTrackInDeathTable();
}

void Magikoopa::HandleClsn()
{
	Actor* other = Actor::FindWithID(cylClsn.otherObjID);
	if(!other)
		return;
	
	unsigned hitFlags = cylClsn.hitFlags;
	
	if(res->isBoss)
	{
		if((hitFlags & CylinderClsn::HIT_BY_EGG) && !invincible)
		{
			--health;
			state = 5;
			stateTimer = 0;
			invincible = true;
			return;
		}
		
		if(other->actorID != 0x00bf)
			return;
	
		Player* player = (Player*)other;
		
		if((hitFlags & 0x400000) && state != 5)
			player->Hurt(pos, DAMAGE, PLAYER_KNOCKBACK, 1, 0, 1);
		
		return;
	}
	
	if(hitFlags & 0x000667c0)
	{
		if(other->actorID == 0x00bf)
			KillByInvincibleChar(KNOCKBACK_ROT, *(Player*)other);
		else
		{
			SpawnCoin();
			KillMagikoopa();
		}
		return;
	}
	else if(hitFlags & CylinderClsn::HIT_BY_SPIN_OR_GROUND_POUND)
	{
		defeatMethod = Enemy::DF_SQUASHED;
		KillByAttack(*other, wmClsn);
		scale.y = 0x0555_f;
		return;
	}
	
	if(other->actorID != 0x00bf)
		return;
	
	Player* player = (Player*)other;
	if(JumpedOnByPlayer(cylClsn, *player))
	{
		defeatMethod = Enemy::DF_SQUASHED;
		KillByAttack(*other, wmClsn);
		scale.y = 0x0555_f;
		player->Bounce(PLAYER_BOUNCE_INIT_VEL);
	}
	else if(hitFlags & CylinderClsn::HIT_BY_MEGA_CHAR)
	{
		SpawnMegaCharParticles(*player, nullptr);
		Sound::PlayBank3(0x001d, camSpacePos);
		KillByInvincibleChar(KNOCKBACK_ROT, *player);
	}
	else if(player->IsOnShell() || player->isMetalWario)
	{
		KillByInvincibleChar(KNOCKBACK_ROT, *player);
	}
	else if(hitFlags & 0x400000)
	{
		player->Hurt(pos, DAMAGE, PLAYER_KNOCKBACK, 1, 0, 1);
	}
	
}

Vector3 Magikoopa::GetWandTipPos()
{
	return Vector3{0_f >> 3, WAND_LENGTH >> 3, 0_f >> 3}.Transform(rigMdl.mat4x3 * rigMdl.data.transforms[5]) << 3;
}

void Magikoopa::State0_Appear()
{
	rigMdl.SetAnim(animFiles[APPEAR].filePtr, Animation::NO_LOOP, 0x1000_f, 0);
	
	if(!res->isBoss) //The boss should not be hit before talking.
		battleStarted = true;
	if(battleStarted)
		invincible = false;
	else if(res->isBoss)
		AttemptTalkStartIfNotStarted();
	
	if(stateTimer == 0)
	{
		Player* player = ClosestPlayer();
		if(player)
			ang.y = pos.HorzAngle(player->pos);
	}
	if(stateTimer >= APPEAR_TIME - 1)
	{
		state = (res->isBoss && !battleStarted) ? 6 : 1;
	}
}

void Magikoopa::State1_Wave()
{
	rigMdl.SetAnim(animFiles[WAVE].filePtr, Animation::LOOP, 0x1000_f, 0);
	battleStarted = true;
	invincible = false;
	
	if(stateTimer >= WAVE_TIME - 1)
	{
		state = 2;
	}
}

void Magikoopa::State2_Shoot()
{
	rigMdl.SetAnim(animFiles[SHOOT].filePtr, Animation::NO_LOOP, 0x1000_f, 0);
	Player* player = ClosestPlayer();
	if(stateTimer == 2 && !(flags & Actor::OFF_SCREEN))
	{
		int shotID = -1;
		
		if(!res->isBoss || shotState != 0)
		{
			for(int i = 0; i < 3; ++i) if(!Actor::FindWithID(res->shotUniqueIDs[i]))
			{
				shotID = i;
				break;
			}
		}
		
		if(shotID >= 0 || (res->isBoss && shotState == 0))
		{	
			Vector3 shotPos = GetWandTipPos() - Vector3{0_f, SHOT_RADIUS, 0_f};
			
			// & 0xc000 because of that silly 0x8000 case
			short vertAng = player ? (AngleDiff(pos.HorzAngle(player->pos), ang.y) & 0xc000) ? -0x4000 : -abs(player->pos.VertAngle(shotPos)) : -0x1555;
			if(vertAng < -0x4000)
				vertAng = -0x4000;
			if(vertAng > MOST_HORZ_SHOT_ANGLES[res->isBoss])
				vertAng = MOST_HORZ_SHOT_ANGLES[res->isBoss];
			Vector3_16 dir = {(Sin(ang.y) * Cos(vertAng)).val,
							  (Sin(vertAng)).val,
							  (Cos(ang.y) * Cos(vertAng)).val};
			Shot* shot = (Magikoopa::Shot*)Actor::Spawn(0x16c, shotID, shotPos, &dir, areaID, -1);
			if(shot)
			{
				shot->SetMagikoopa(*this);
				if(shotID >= 0)
					res->shotUniqueIDs[shotID] = shot->uniqueID;
				shotState ^= 1;
			}
		}
	}
	if(stateTimer >= SHOOT_TIME - 1)
	{
		state = 3;
	}
}

void Magikoopa::State3_Poof()
{
	rigMdl.SetAnim(animFiles[POOF].filePtr, Animation::NO_LOOP, 0x1000_f, 0);
	if(stateTimer >= POOF_TIME - 1)
	{
		state = 4;
	}
}

void Magikoopa::State4_Teleport()
{
	if(stateTimer == 0)
	{
		if(battleStarted)
			DisappearPoofDustAt(Vector3{pos.x, pos.y + (HEIGHTS[res->isBoss] >> 1), pos.z});
		flags &= ~Actor::AIMABLE_BY_EGG;
	}
	
	if(stateTimer >= TELEPORT_TIME || !battleStarted)
	{
		stateTimer = TELEPORT_TIME; //prevent overflow
		
		Player* player = ClosestPlayer();
		if(!res->isBoss || battleStarted)
		{
			if((flags & Actor::OFF_SCREEN) || (player && (pos.Dist(player->pos) <= TP_PLAYER_DIST_TOL || (!res->isBoss && pos.Dist(player->pos) > TP_PLAYER_DIST_MAX))))
				return;
		}
		else if(!player || originalPos.Dist(player->pos) > BOSS_START_BATTLE_RADIUS || player->currClsnState != 1)
			return;
		
		currPathPt = nextPathPt;
		PoofDustAt(Vector3{pos.x, pos.y + (HEIGHTS[res->isBoss] >> 1), pos.z});
		flags |= Actor::AIMABLE_BY_EGG;
		state = 0;
	}
}

void Magikoopa::State5_Hurt()
{
	rigMdl.SetAnim(animFiles[HURT].filePtr, Animation::NO_LOOP, 0x1000_f, 0);
	if(health == 0)
		AttemptTalkStartIfNotStarted();
	if(stateTimer >= HURT_TIME)
		state = health == 0 ? 7 : 3;
}

void Magikoopa::State6_Wait()
{
	rigMdl.SetAnim(animFiles[WAIT].filePtr, Animation::LOOP, 0x1000_f, 0);
	Talk();
	if(state == 6)
		AttemptTalkStartIfNotStarted();
}

void Magikoopa::State7_Defeat()
{
	rigMdl.SetAnim(animFiles[DEFEAT].filePtr, Animation::NO_LOOP, 0x1000_f, 0);
	Talk();
	if(!shouldBeKilled)
		AttemptTalkStartIfNotStarted();
}

void Magikoopa::AttemptTalkStartIfNotStarted()
{
	Player* player = ClosestPlayer();
	if(player->StartTalk(*this, true))
	{
		Message::PrepareTalk();
		listener = player;
	}
}

void Magikoopa::Talk()
{
	if(!listener)
		return;
	
	int talkState = listener->GetTalkState();
	switch(talkState)
	{
		case Player::TK_NOT:
			Message::EndTalk();
			listener = nullptr;
			if(state == 7)
			{
				KillMagikoopa();
				Sound::StopLoadedMusic();
			}
			else
			{
				state = 1;
				Sound::LoadAndSetMusic(Sound::MU_BOSS);
			}
			break;
			
		case Player::TK_START:
			listener->ShowMessage(*this, state == 7 ? defeatMsgIDs[listener->param1] : waitMsgIDs[listener->param1], Vector3{pos.x, pos.y + TALK_HEIGHT, pos.z}, 0, 0);
			break;
			
		default:
			return;
	}
}

int Magikoopa::Behavior()
{
	if(!Particle::Manager::LoadTex(0x0397, 0x2e)) //loads it the first time this is called
		return 0;
	
	if(UpdateYoshiEat(wmClsn)) //will set isBeingSpit to false if the magikoopa runs into ground
	{
		cylClsn.Clear();
		
		rigMdl.SetAnim(animFiles[POOF].filePtr, Animation::NO_LOOP, 0x1000_f, 0);
		rigMdl.anim.currFrame = 0_f;
		speed.y = 1_f;
	
		if(isBeingSpit && spitTimer == 0)
		{
			horzSpeed.AdvanceToDest(0_f, SPIT_DECEL);
			cylClsn.Update();
		}
		if(horzSpeed == 0_f || !isBeingSpit)
		{
			horzSpeed = 0_f;
			isBeingSpit = false;
			state = 3;
			stateTimer = 0;
		}
		UpdateModelTransform();
		return 1;
	}
	
	if(defeatMethod != Enemy::DF_NOT)
	{
		int res = UpdateKillByInvincibleChar(wmClsn, rigMdl, 3);
		if(res == 2) //finished kill
		{
			KillMagikoopa();
		}
		else if(res == 0) //not yet
		{
			UpdateDeath(wmClsn);
			UpdateModelTransform();
		}
		return 1;
	}
	
	int prevState = state;
	
	//Dun, dun, dun!
	//----------------------------//
	(this->*stateFuncs[state])(); // dat syntax tho
	//----------------------------//
	
	++stateTimer;
	if(state != prevState)
		stateTimer = 0;
	
	Player* player = ClosestPlayer();
	if(res->isBoss && battleStarted && (!player || originalPos.Dist(player->pos) >= BOSS_STOP_BATTLE_RADIUS) && health != 0)
	{
		state = 4;
		stateTimer = 0;
		battleStarted = false;
		invincible = true;
		health = 3;
		shotState = 0;
		currPathPt = numPathPts;
		Sound::StopLoadedMusic();
	}
	if(state == 4)
	{
		if(stateTimer >= TELEPORT_TIME)
		{
			if(currPathPt < numPathPts)
			{
				nextPathPt = (unsigned)RandomInt() % (numPathPts - 1);
				if(nextPathPt >= currPathPt)
					++nextPathPt;
			}
			else
				nextPathPt = 0;
			pathPtr.GetPt(pos, nextPathPt);
		}
		return 1;
	}
	
	UpdatePos(nullptr);
	UpdateModelTransform();
	
	Vector3 wandTip = GetWandTipPos();
	shapesID = Particle::System::New(shapesID, (unsigned)&particleSysDefs[res->isBoss], wandTip.x, wandTip.y, wandTip.z, nullptr, nullptr);
	
	//UpdateWMClsn(&wmClsn, 2);
	HandleClsn(); //must be done before clearing collision, of course
	
	cylClsn.Clear();
	if(defeatMethod == Enemy::DF_NOT)
		cylClsn.Update();
	
	MakeVanishLuigiWork(cylClsn);
	
	rigMdl.anim.Advance();
	
	return 1;
}

int Magikoopa::Render()
{
	if(state == 4 || (flags & Actor::IN_YOSHI_MOUTH))
		return 1;
		
	rigMdl.Render(&scale);	
	return 1;
}

void Magikoopa::Virtual30()
{
	if(eventToTrigger < 0x20)
		Event::SetBit(eventToTrigger);
}


Magikoopa::~Magikoopa() {}

unsigned Magikoopa::OnYoshiTryEat()
{
	return state == 4 || res->isBoss ? Actor::YE_DONT_EAT : Actor::YE_KEEP_AND_CAN_MAKE_EGG;
}

void Magikoopa::OnTurnIntoEgg(Player& player)
{
	if(player.Unk_020bea94())
		Unk_0201061c(player, NUM_COINS, 0);
	else
		player.RegisterEggCoinCount(NUM_COINS, false, false);
	
	KillMagikoopa();
}

Fix12i Magikoopa::OnAimedAtWithEgg()
{
	return HEIGHTS[res->isBoss] >> 1;
}

void init()
{
	OBJ_TO_ACTOR_ID_TABLE[0x0012] = 0x00c6;
	ACTOR_SPAWN_TABLE[0x00c6] = (unsigned)&Magikoopa::spawnData;
	modelFiles[0].Construct(0x0398);
	modelFiles[1].Construct(0x081a);
	animFiles[APPEAR].Construct(0x0396);
	animFiles[WAVE  ].Construct(0x039b);
	animFiles[SHOOT ].Construct(0x039a);
	animFiles[POOF  ].Construct(0x0399);
	animFiles[WAIT  ].Construct(0x081b);
	animFiles[HURT  ].Construct(0x081c);
	animFiles[DEFEAT].Construct(0x081d);
	
	OBJ_TO_ACTOR_ID_TABLE[0x014e] = 0x0170;
	ACTOR_SPAWN_TABLE[0x0170] = (unsigned)&Magikoopa::bossSpawnData;
	
	OBJ_TO_ACTOR_ID_TABLE[0x014a] = 0x016c;
	ACTOR_SPAWN_TABLE[0x016c] = (unsigned)&Magikoopa::Shot::spawnData;
}

void cleanup()
{
	
}