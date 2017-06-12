#include "GoombaColored.h"

//Goomba: 02129020 to 0212c10c and then 02130174 to 02130290 and then 02130880 to 021309c4
/*
Hierarchy:
Actor (0x000 to 0x0d4)
EnemyBase (0x0d4 to 0x110)
CapEnemy (0x110 to 0x180)
	Model (+0x000)
	CapIcon (+0x050)
Kuribo (0x180 to 0x478)
	CcAcPos (Cylinder collider)
	BgCh_Actr (With mesh collider)
		BgCh (+0x000)
		BgCh_SphCrr (+0x020)
		BgCh_Lin (+0x134)
*/

//REMEMBER TO MANUALLY CHANGE THE OFFSETS TO THE SPAWN INFO FOR THIS ONE!

namespace
{
	using StateFuncPtr = void(Goomba::*)();

	

	bool constructed = false;

	const char materialName[] = "kuribo_all";
	char aliveMatVals[] = {0x1f, 0x10, 0x00, 0x00};
	MaterialProperties aliveMatProps =
	{
		(short)0xffff, 0,
		&materialName[0],
		0x01, 0x00, 0x0000,    0x01, 0x00, 0x0000,    0x01, 0x00, 0x0000, 
		0x01, 0x00, 0x0001,    0x01, 0x00, 0x0001,    0x01, 0x00, 0x0001,  
		0x01, 0x00, 0x0002,    0x01, 0x00, 0x0002,    0x01, 0x00, 0x0002, 
		0x01, 0x00, 0x0002,    0x01, 0x00, 0x0002,    0x01, 0x00, 0x0002, 
		0x01, 0x00, 0x0000, 
	};
	MaterialDef aliveMat = {0x0002, 0x0000, &aliveMatVals[0], 1, &aliveMatProps};

	char regurgMatVals[] = {0x00, 0x03, 0x0c, 0x16, 0x1f, 0x1f, 0x1f, 0x1f,
							0x1f, 0x1f, 0x1f, 0x1f, 0x1f, 0x1f, 0x1f, 0x1f,
							0x1f, 0x10, 0x00, 0x00};
	MaterialProperties regurgMatProps =
	{
		(short)0xffff, 0,
		&materialName[0],
		0x01, 0x01, 0x0000,    0x01, 0x00, 0x0004,    0x01, 0x00, 0x0004,
		0x01, 0x00, 0x0011,    0x01, 0x00, 0x0011,    0x01, 0x00, 0x0011, 
		0x01, 0x00, 0x0000,    0x01, 0x00, 0x0000,    0x01, 0x00, 0x0000,
		0x01, 0x00, 0x0000,    0x01, 0x00, 0x0000,    0x01, 0x00, 0x0000,
		0x01, 0x00, 0x0004, 
	};
	MaterialDef regurgMat = {0x0011, 0x0000, &regurgMatVals[0], 1, &regurgMatProps};

	const StateFuncPtr stateFuncs[] =
	{
		&Goomba::State0,
		&Goomba::State1,
		&Goomba::State2,
		&Goomba::State3,
		&Goomba::State4,
		&Goomba::State5
	};

	const Fix12i SCALES[]          = { 0x00000555_f,  0x00001000_f,  0x00002555_f};
	const Fix12i HORZ_CLSN_SIZES[] = { 0x00028000_f,  0x00064000_f,  0x000e0000_f};
	const Fix12i VERT_ACCELS[]     = {-0x00000aaa_f, -0x00002000_f, -0x00005aaa_f};
	const Fix12i WALK_SPEEDS[]     = { 0x00000aaa_f,  0x00002000_f,  0x00004aaa_f};
	const Fix12i RUN_SPEEDS[]      = { 0x0000a800_f,  0x00015000_f,  0x00015000_f};
	const Fix12i JUMP_SPEEDS[]     = { 0x00008000_f,  0x00010000_f,  0x00020000_f};
	const unsigned DYING_SOUND_IDS[] = {0x00000110, 0x000000d6, 0x00000111};

	const Fix12i JUMP_HORZ_SPEED =  0x00010000_f;
	const Fix12i JUMP_VERT_ACCEL = -0x00002000_f;
	const Fix12i JUMP_INIT_VEL   =  0x0001c48c_f;
	const Fix12i JUMP_DIST       =  0x00190000_f;
	const short SPIN_SPEED	= 0x2000;
	const Fix12i SPIN_TERM_VEL   = -0x00009000_f;
	
	const Vector3 CAP_OFFSET = {0x00000_f, 0x6c000_f, 0x00000_f};
}

SharedFilePtr Goomba::modelFile;
SharedFilePtr Goomba::texSeqFile;
SharedFilePtr Goomba::animFiles[Goomba::NUM_ANIMS];

SpawnInfo<Goomba> Goomba::spawnDataNormal[] =
{
	&Goomba::Spawn,
	0x00c8,
	0x0018,
	0x10000006,
	0x00032000_f,
	0x00046000_f,
	0x01000000_f,
	0x01000000_f
};

SpawnInfo<Goomba> Goomba::spawnDataSmall[] =
{
	&Goomba::Spawn,
	0x00c9,
	0x0019,
	0x10000006,
	0x00032000_f,
	0x00046000_f,
	0x006a4000_f,
	0x00800000_f
};

SpawnInfo<Goomba> Goomba::spawnDataBig[] =
{
	&Goomba::Spawn,
	0x00ca,
	0x001a,
	0x10000006,
	0x00064000_f,
	0x000c8000_f,
	0x01000000_f,
	0x01000000_f
};

//021290d4
void Goomba::UpdateMaxDist()
{
	if(noChargeTimer != 0)
	{
		if(state == 1)
			noChargeTimer = 0;
		else
			--noChargeTimer;
	}
	else if(stuckInSpotTimer > 0xa)
		maxDist = std::max(0x1f4000_f - ((stuckInSpotTimer - 0xa) * 0x14000_f), 0xa000_f);
	else
		maxDist = 0x1f4000_f;
}

//0212bff8
Goomba* Goomba::Spawn()
{
	return new Goomba;
}

int Goomba::InitResources()
{
	if(!constructed)
	{
		constructed = true;
		
		modelFile.Construct(0x0386);
		texSeqFile.Construct(0x0387);
		animFiles[ROLLING  ].Construct(0x0388);
		animFiles[RUN      ].Construct(0x0389);
		animFiles[STRETCH  ].Construct(0x038a);
		animFiles[UNBALANCE].Construct(0x038b);
		animFiles[WAIT     ].Construct(0x038c);
		animFiles[WALK     ].Construct(0x038d);
	}
	
	spawnSilverStar = param1 >> 4 & 0xf;
	silverStarID = 0xff;
	
	spawnCapFlag = param1 >> 8 & 0xf;
	charBehav = param1 >> 12 & 0xf;
	extraDamage = charBehav == 2 ? 1 : 0;
	
	if(spawnSilverStar == 1)
	{
		silverStarID = TrackStar(0xf, 1);
		LoadSilverStarAndNumber();
	}
		
	Model::LoadFile(modelFile);
	TextureSequence::LoadFile(texSeqFile);
	for(int i = 0; i < NUM_ANIMS; ++i)
		BoneAnimation::LoadFile(animFiles[i]);
		
	AddCap((uint8_t)(param1 & 0xf));
	if(capID < 6)
	{
		param1 &= 0xf0ff;
	}
	
	if(!DestroyIfCapNotNeeded())
		return 0;
	
	if(!rigMdl.SetFile(modelFile.filePtr, 1, -1))
		return 0;
	
	if(!shadow.InitCylinder())
		return 0;
	
	MaterialChanger::Prepare(modelFile.filePtr, aliveMat);
	materialChg.SetFile(aliveMat, Animation::NO_LOOP, 0x1000_f, 0);
	TextureSequence::Prepare(modelFile.filePtr, texSeqFile.filePtr);
	texSeq.SetFile(texSeqFile.filePtr, Animation::NO_LOOP, 0x1000_f, charBehav < 3 ? charBehav + 1 : 0);
	coinType = Enemy::CN_YELLOW;

	if(actorID == 0x00c9)
	{
		sizeType = SizeType::SMALL;
	}
	else if(actorID == 0x00ca)
	{
		sizeType = SizeType::BIG;
		LoadBlueCoinModel();
	}
	else
		sizeType = SizeType::NORMAL;
	
	scale.x = SCALES[sizeType];
	scale.y = SCALES[sizeType];
	scale.z = SCALES[sizeType];
	
	cylClsn.Init(this, scale.x * 0x3c000_f, HORZ_CLSN_SIZES[sizeType], 0x00200000, 0x00a6efe0);
	if(sizeType == SizeType::BIG)
		cylClsn.vulnerableFlags &= ~0x00008000;
		
	wmClsn.Init(this, scale.x * 0x3c, scale.x * 0x3c, nullptr, nullptr);
	wmClsn.SetFlag_2();
	
	flags468 = state = 0;
	player = nullptr;
	defeatMethod = Enemy::DF_NOT;
	distToPlayer = 0x7fffffff_f;
	
	targetDir = motionAng.y;
	targetSpeed = 0x00002000_f;
	
	movementTimer = stuckInSpotTimer = 0;
	
	backupPos = pos;
	
	UpdateMaxDist();
	
	originalPos = pos;
	
	vertAccel = VERT_ACCELS[sizeType];
	termVel = -0x00032000_f;
	
	rigMdl.SetAnim(animFiles[WALK].filePtr, Animation::LOOP, 0x1000_f, 0);
	
	regurgBounceCount = 0;
	
	return 1;
}

//0212b510
int Goomba::CleanupResources()
{
	if(sizeType == SizeType::BIG)
		UnloadBlueCoinModel();
	
	modelFile.Release();
	texSeqFile.Release();
	for(int i = 0; i < NUM_ANIMS; ++i)
		animFiles[i].Release();
	
	if(spawnSilverStar == 1)
		UnloadSilverStarAndNumber();
	
	UnloadCapModel();
	return 1;
}

//02129498
void Goomba::Kill()
{
	if(capID >= 6)
		KillAndTrackInDeathTable();
	else
		Destroy();
}

//021296cc
void Goomba::SpawnSilverStarIfNecessary()
{
	if(spawnSilverStar == 1)
	{
		UntrackStar();
		Actor* starMarker = Actor::Spawn(0xb4, 0x50, originalPos, nullptr, areaID, -1); //that was the star marker.
		Actor* silverStar = Actor::Spawn(0xb3, 0x10, pos,         nullptr, areaID, -1);
		
		if(starMarker && silverStar)
		{
			*(int*)((char*)silverStar + 0x434) = starMarker->uniqueID;
			LinkSilverStarAndStarMarker(starMarker, silverStar);
			SpawnSoundObj(1);
		}
		
		param1 &= 0xff0f; //rid the silver star part
	}
}

void Goomba::KillAndSpawnCap()
{
	SpawnCoin();
	Kill();
	
	ReleaseCap(CAP_OFFSET);
	pos = originalPos;
	RespawnIfHasCap();
}
void Goomba::KillAndSpawnSilverStar()
{
	SpawnSilverStarIfNecessary();
	KillAndSpawnCap();
}

//021298d0
bool Goomba::UpdateIfDying()
{
	bool dying = UpdateDeath(wmClsn); //r4
	if(defeatMethod - 2 <= 4)
	{
		rigMdl.anim.speed = 0x1000_f;
		rigMdl.anim.Advance();
	}
	
	if(dying)
	{
		Sound::PlayBank3(DYING_SOUND_IDS[sizeType], camSpacePos);
		SpawnSilverStarIfNecessary();
		if(capID < 6)
		{
			pos = originalPos;
			RespawnIfHasCap();
			UntrackInDeathTable();
		}
			
	}
	
	return dying;
}

//02129168
void Goomba::RenderRegurgGoombaHelpless(Player* player)
{
	regurgTimer = 0x3c;
	speed.y = (0xd000_f - vertAccel) / floorNormal.y;
	horzSpeed = -speed.HorzLen();
	
	if(player)
		motionAng.y = pos.HorzAngle(player->pos);
	
	rigMdl.SetAnim(animFiles[UNBALANCE].filePtr, Animation::LOOP, 0x1000_f, 0);
	state = 3;
	isBeingSpit = false;
	
	wmClsn.SetLimMovFlag();
	wmClsn.Unk_0203589c();
	wmClsn.ClearJustHitGroundFlag();
	wmClsn.ClearGroundFlag();
	
	regurgBounceCount = 0;
	
	Sound::PlayBank3(0x13a, camSpacePos);
}

//021294d0
void Goomba::KillIfTouchedBadSurface()
{
	if(wmClsn.IsOnGround())
		return;
	
	if(wmClsn.sphere.floorResult.clps.IsWater())
	{
		KillAndSpawnSilverStar();
		return;
	}
	
	ClsnResult& clsnRes = wmClsn.sphere.floorResult; //r13
	CLPS& clps = clsnRes.clps;
	unsigned behav = clps.BehaviorID();
	
	if((clps.TextureID() == CLPS::TX_SAND &&
		(behav == CLPS::BH_LOW_JUMPS || behav == CLPS::BH_SLOW_QUICKSAND || behav == CLPS::BH_SLOW_QUICKSAND_2 || behav == CLPS::BH_INSTANT_QUICKSAND)) ||
	   behav == CLPS::BH_WIND_GUST || behav == CLPS::BH_LAVA || behav == CLPS::BH_DEATH || behav == CLPS::BH_DEATH_2) //698 else
	{
		KillAndSpawnCap();
	}
	
	clsnRes.Reset();
}

//0212a580
void Goomba::UpdateModel()
{
	Matrix4x3& modelMat = rigMdl.mat4x3;
	
	modelMat.ThisFromRotationY(ang.y);
	modelMat.r0c3 = pos.x >> 3;
	modelMat.r1c3 = pos.y >> 3;
	modelMat.r2c3 = pos.z >> 3;
	
	if(!(flags & 0x40000))
		DropShadowRadHeight(shadow, modelMat, scale.x * 0x50, wmClsn.IsOnGround() ? 0x1e000_f : 0x96000_f, 0xf);
	
	UpdateCapPos(Vector3{Sin(ang.y) * 0xa, CAP_OFFSET.y, Cos(ang.y) * 0xa}, ang);
}

//02129a00
bool Goomba::UpdateIfEaten()
{
	unsigned eatState = UpdateYoshiEat(wmClsn); //r4
	if(eatState != 0)
	{
		if(eatState == 1)
		{
			if(GetCapEatenOffIt(CAP_OFFSET))
			{
				RenderRegurgGoombaHelpless(eater);
				horzSpeed = -0xf000_f;
				speed.y = 0x14000_f;
				MaterialChanger::Prepare(modelFile.filePtr, regurgMat);
				materialChg.SetFile(regurgMat, Animation::NO_LOOP, 0x1000_f, 0);
				materialChg.currFrame = 0_f;
				cylClsn.Clear();
				return false;
			}
		}
		else if(eatState == 3)
		{
			if(wmClsn.IsOnGround())
				horzSpeed >>= 1;
		}
		
		if(SpawnParticlesIfHitOtherObj(cylClsn))
		{
			Actor* other = Actor::FindWithID(cylClsn.otherObjID); //guaranteed to exist by condition
			defeatMethod = Enemy::DF_HIT_REGURG;
			KillByAttack(*other, wmClsn);
			rigMdl.SetAnim(animFiles[ROLLING].filePtr, Animation::NO_LOOP, 0x1000_f, 0);
			cylClsn.flags1 |= CylinderClsn::F1_DISABLED;
			return true;
		}
		
		UpdateModel();
		cylClsn.Clear();
		if(isBeingSpit)
		{
			KillIfTouchedBadSurface();
			if(spitTimer == 0)
				cylClsn.Update();
			else if(spitTimer == 5)
			{
				rigMdl.SetAnim(animFiles[ROLLING].filePtr, Animation::NO_LOOP, 0x1000_f, 0);
				motionAng.y += 0x8000;
				horzSpeed = -horzSpeed;
				
				MaterialChanger::Prepare(modelFile.filePtr, regurgMat);
				materialChg.SetFile(regurgMat, Animation::NO_LOOP, 0x1000_f, 0);
				materialChg.currFrame = 0_f;
			}
			
			rigMdl.anim.Advance();
			if(wmClsn.JustHitGround())
				RenderRegurgGoombaHelpless(nullptr);
		}
		
		return true;
	}
	return false;
}

void Goomba::PlayMovingSound()
{
	if(state != 0)
		return;
		
	if(!wmClsn.IsOnGround())
		return;
		
	unsigned currFrameInt = (int)rigMdl.anim.currFrame;
	if ((rigMdl.anim.file == animFiles[WALK].filePtr && (currFrameInt <= 4 || (currFrameInt >= 12 && currFrameInt <= 16))) ||
		(rigMdl.anim.file == animFiles[RUN ].filePtr && (currFrameInt <= 3 || (currFrameInt >= 10 && currFrameInt <= 13))))
	{
		if(flags468 & 2)
			return;
			
		Sound::PlayBank3(0xd0, camSpacePos);
		flags468 |= 2;	
	}
	else
		flags468 = flags468 & ~2;
}


//02129ed4 (How does this handle getting hit by a shell?)
void Goomba::GetHurtOrHurtPlayer()
{
	if(cylClsn.otherObjID == 0)
		return;
	
	Player* player = ClosestPlayer();
	
	if(!player)
		return;
	
	hitFlags = cylClsn.hitFlags;
	
	bool rotate = false;
	
	if(sizeType != SizeType::SMALL && (hitFlags & CylinderClsn::HIT_BY_MEGA_CHAR))
	{
		ReleaseCap(CAP_OFFSET);
		KillByInvincibleChar(Vector3_16{short(0 - (sizeType == SizeType::BIG ? 0x2000 : 0x1800)), 0, 0}, *player);
		
		return;
	}
	else if(hitFlags & CylinderClsn::HIT_BY_SPIN_OR_GROUND_POUND)
	{
		defeatMethod = Enemy::DF_SQUASHED;
		if(sizeType == SizeType::BIG)
			coinType = Enemy::CN_BLUE;
		
		scale.x = scale.y = scale.z = 0x1000_f;
		Sound::PlayBank3(0xe0, camSpacePos);
	}
	else if(hitFlags & CylinderClsn::HIT_BY_FIRE)
	{
		defeatMethod = Enemy::DF_BURNED;
		rigMdl.SetAnim(animFiles[STRETCH].filePtr, Animation::NO_LOOP, 0x1000_f, 0);
		rotate = true;
	}
	else
	{
		bool killedByOtherMeans = false;
		if(sizeType != SizeType::BIG)
		{
			killedByOtherMeans = true;
			if(hitFlags & CylinderClsn::HIT_BY_REGURG_GOOMBA)
			{
				defeatMethod = Enemy::DF_HIT_REGURG;
				rigMdl.SetAnim(animFiles[ROLLING].filePtr, Animation::NO_LOOP, 0x1000_f, 0);
				cylClsn.flags1 |= CylinderClsn::F1_DISABLED;
			}
			else if(hitFlags & (0x2000 | CylinderClsn::HIT_BY_DIVE))
			{
				defeatMethod = Enemy::DF_DIVED;
				rigMdl.SetAnim(animFiles[ROLLING].filePtr, Animation::NO_LOOP, 0x1000_f, 0);
			}
			else if(hitFlags & 0x4000)
			{
				defeatMethod = Enemy::DF_UNK_6;
				rigMdl.SetAnim(animFiles[STRETCH].filePtr, Animation::NO_LOOP, 0x1000_f, 0);
			}
			else if(hitFlags & (CylinderClsn::HIT_BY_KICK | CylinderClsn::HIT_BY_BREAKDANCE | CylinderClsn::HIT_BY_SLIDE_KICK))
			{
				defeatMethod = Enemy::DF_KICKED;
				rigMdl.SetAnim(animFiles[STRETCH].filePtr, Animation::NO_LOOP, 0x1000_f, 0);
				rotate = true;
			}
			else if(hitFlags & CylinderClsn::HIT_BY_PUNCH)
			{
				defeatMethod = Enemy::DF_PUNCHED;
				rigMdl.SetAnim(animFiles[ROLLING].filePtr, Animation::NO_LOOP, 0x1000_f, 0);
				rotate = true;
			}
			else killedByOtherMeans = false;
		}
		if(!(hitFlags & 0x8000) && player->actorID == 0x00bf && !killedByOtherMeans)
		{
			Vector3 playerPos = player->pos;
			if(player->isMetalWario && sizeType != SizeType::BIG)
			{
				ReleaseCap(CAP_OFFSET);
				KillByInvincibleChar(Vector3_16{0x2000, 0, 0}, *player);
				return;
			}
			else if(player->IsOnShell() && sizeType != SizeType::BIG)
			{
				defeatMethod = Enemy::DF_DIVED;
				rigMdl.SetAnim(animFiles[ROLLING].filePtr, Animation::NO_LOOP, 0x1000_f, 0);
				rotate = true;
			}
			else if(JumpedOnByPlayer(cylClsn, *player))
			{
				player->Bounce(0x28000_f);
				Sound::PlayBank3(0xe0, camSpacePos);
				defeatMethod = Enemy::DF_SQUASHED;
				scale.x = scale.y = scale.z = 0x1000_f;
			}
			else if(player->isVanishLuigi)
				return;
			else if(state == 0 || state == 4 || state == 5)
			{
				if(sizeType == SizeType::SMALL)
				{
					SmallPoofDust();
					
					player->Hurt(pos, 0 + extraDamage, 0xc000_f + 0x6000_f * extraDamage, 1, 0, 1);
					Kill();
					Sound::PlayBank3(0x110, camSpacePos);
				}
				else if(cylClsn.hitFlags & 0x400000)
				{
					Vector3 objPos = pos;
					player->Hurt(pos, sizeType + extraDamage, 0xc000_f + 0x6000_f * extraDamage, 1, 0, 1);
					state = 1; //a.k.a. Haha, plumber!
				}
				
				return;
			}
		}
	}
	
	if(defeatMethod != Enemy::DF_NOT)
	{
		ReleaseCap(CAP_OFFSET);
		KillByAttack(*player, wmClsn);
		
		if(rotate)
			ang.y = motionAng.y + 0x8000;
	}
	
	//Whew! What a long function!
}


//02129238
void Goomba::KillIfIntoxicated()
{
	if(wmClsn.IsOnGround())
		return;
		
	RaycastGround raycaster; //r13+0x18
	raycaster.SetFlag_2();
	raycaster.SetFlag_8();
	raycaster.ClearFlag_1();
	
	raycaster.SetObjAndPos(Vector3{pos.x, pos.y + 0x190000_f, pos.z}, this);
	if(raycaster.DetectClsn() && raycaster.result.clps.IsToxic() &&
	   raycaster.clsnPosY != 0x80000000_f && pos.y < raycaster.clsnPosY) //338 else
	{
		PoofDust();
		KillAndSpawnCap();
	}
}

//02129c9c
void Goomba::Jump()
{
	Sound::PlayBank3(0x118, camSpacePos);
	state = 2;
	horzSpeed = 0_f;
	speed.y = JUMP_SPEEDS[sizeType];
	wmClsn.ClearGroundFlag();
	cylClsn.flags1 |= 4;
}

void Goomba::UpdateTargetDirAndDist(Fix12i theMaxDist)
{
	player = ClosestPlayer();
	
	if(pos.Dist(originalPos) > theMaxDist || !player)
	{
		targetDir = Vec3_HorzAngle(&pos, &originalPos);
		distToPlayer = 0x061a8000_f;
		return;
	}
	
	Vector3 playerPos = player->pos;
	
	if(Vec3_Dist(&originalPos, &playerPos) > theMaxDist)
	{
		distToPlayer = 0x061a8000_f;
		return;
	}
	
	distToPlayer = pos.Dist(playerPos);
	targetDir = pos.HorzAngle(playerPos);
}

//0212b6ec
int Goomba::Behavior()
{
	if(state != 4)
		vertAccel = VERT_ACCELS[sizeType];
	if(state != 5)
		termVel = -0x00032000_f;
	
	UpdateMaxDist();
	int capState = GetCapState();
	if(capState == 0)
		return 1;
		
	if(capState == 1)
	{
		flags |= Actor::AIMABLE_BY_EGG;
		PoofDust();
	}
	
	if(state != 3 && !isBeingSpit && defeatMethod == Enemy::DF_NOT && IsTooFarAwayFromPlayer(0x5dc000_f))
	{
		Unk_02005d94();
		return 1;
	}
	
	if(defeatMethod != Enemy::DF_NOT)
	{
		int res = UpdateKillByInvincibleChar(wmClsn, rigMdl, 3);
		if(res == 2) //killed by mega char
		{
			KillAndSpawnSilverStar();
		}
		else if(res == 0) //not by mega char
		{
			if(!UpdateIfDying())
				UpdateModel();
		}
		return 1;
	}
	
	if(UpdateIfEaten())
		return 1;
	
	if(state >= 3)
		rigMdl.anim.speed = 0x1000_f;
	else
		rigMdl.anim.speed = std::min(horzSpeed / (2 * scale.x), 0x3000_f);
	
	if(state != 2 && state != 4 && state != 5)
	{
		PlayMovingSound();
		rigMdl.anim.Advance();
	}
	
	unsigned prevState = state;
	
	//Dun, dun, dun!
	//----------------------------//
	(this->*stateFuncs[state])(); //yay! State-based code!
	//----------------------------//
	
	++stateTimer;
	if(state != prevState)
		stateTimer = 0;
	
	GetHurtOrHurtPlayer();
	UpdatePos(&cylClsn);
	
	if(defeatMethod == Enemy::DF_NOT && state != 2 && state != 3)
	{
		if(IsGoingOffCliff(wmClsn, 0x32000_f, 0x1f49, 0, 1, 0x32000_f))
			pos = noCliffPos;
		else
			noCliffPos = pos;
	}
	
	UpdateWMClsn(wmClsn, sizeType == SizeType::SMALL ? 0 : 2);
	KillIfTouchedBadSurface();
	cylClsn.Clear();
	
	if(defeatMethod == Enemy::DF_NOT)
		cylClsn.Update();
	UpdateModel();
	KillIfIntoxicated();
	
	if(state == 0 && !(flags & Actor::OFF_SCREEN))
	{
		if(pos.Dist(backupPos) < 0xa000_f)
		{
			++stuckInSpotTimer;
		}
		else
		{
			stuckInSpotTimer = 0;
			backupPos = pos;
		}
	}
	else
	{
		if(noChargeTimer == 0)
			stuckInSpotTimer = 0;
	}
	
	return 1;
}
//0212abd4
void Goomba::State0HelperFunc()
{
	UpdateTargetDirAndDist(0x3e8000_f);
	horzSpeed.AdvanceToDest(targetSpeed, 0x500_f);
	
	short angAccel = 0x400;
	
	if(flags468 & 1)
	{
		if(AdvanceToDest_Short(motionAng.y, targetDir, 0x400))
			flags468 = flags468 & ~1;

		return;
	}
	else if(noChargeTimer != 0)
	{
		targetSpeed = WALK_SPEEDS[sizeType];
		rigMdl.SetAnim(animFiles[WALK].filePtr, Animation::Flags::LOOP, 0x1000_f, 0);
		targetDir2 = pos.HorzAngle(originalPos);
		angAccel = 0x400;
		
		AdvanceToDest_Short(motionAng.y, targetDir2, angAccel);
		return;
	}

	if(distToPlayer >= 0x061a8000_f)
	{
		targetDir2 = targetDir;
		movementTimer = 0x19;
	}
	bool redirected = AngleAwayFromWallOrCliff(wmClsn, targetDir2);
	flags468 = (flags468 & ~1) | ((char)redirected & 1);
	
	if(!redirected) //0212af14 else
	{
		if(distToPlayer < maxDist)
		{
			if(targetSpeed <= WALK_SPEEDS[sizeType])
				Jump();
			else
				rigMdl.SetAnim(animFiles[RUN].filePtr, Animation::Flags::LOOP, 0x1000_f, 0);
			
			targetDir2 = targetDir;				
			targetSpeed = RUN_SPEEDS[sizeType];
			
			if(charBehav < 2 && distToPlayer <= JUMP_DIST && rigMdl.anim.file == animFiles[RUN].filePtr)
			{
				state = 4;
				horzSpeed = JUMP_HORZ_SPEED;
				speed.y = JUMP_INIT_VEL;
				vertAccel = JUMP_VERT_ACCEL;
				rigMdl.SetAnim(animFiles[WAIT].filePtr, Animation::Flags::LOOP, 0x1000_f, 0);
				Sound::PlayBank3(0x118, camSpacePos);
			}
			//goes to 0212af14
		}
		else
		{
			targetSpeed = WALK_SPEEDS[sizeType];
			rigMdl.SetAnim(animFiles[WALK].filePtr, Animation::Flags::LOOP, 0x1000_f, 0);
			
			if(movementTimer != 0)
				--movementTimer;
			else
			{
				if(RandomInt() & 0x30000)
				{
					targetDir2 = (short)(RandomInt() >> 16) + motionAng.y;
					movementTimer = 0x64;
				}
				else
				{
					targetDir2 = (short)(RandomInt() >> 16);
					Jump();
				}
			}
		}
	}
	
	if(stuckInSpotTimer > 0x1e)
		noChargeTimer = stuckInSpotTimer;
	
	AdvanceToDest_Short(motionAng.y, targetDir2, angAccel);
}
//0212b2dc
void Goomba::State0()
{
	State0HelperFunc();
	ang.y = motionAng.y;
}
//0212ab48
void Goomba::State1()
{
	Jump();
	if(actorID == 0x00ca)
		speed.y *= 0x1800_f;
	
	targetDir2 = targetDir;
	flags468 = flags468 & ~1;
	ang.y = motionAng.y;
}
//0212aab0
void Goomba::State2()
{
	if(wmClsn.JustHitGround())
	{
		if(sizeType == SizeType::BIG)
			BigLandingDust(true);
		else if(sizeType == SizeType::NORMAL)
			LandingDust(true);
	}
	
	if(wmClsn.IsOnGround())
	{
		state = 0;
		cylClsn.flags1 = cylClsn.flags1 & ~4;
	}
	else
		AdvanceToDest_Short(motionAng.y, targetDir2, 0x800);
	
	ang.y = motionAng.y;
}
//0212a774
void Goomba::State3()
{
	if(regurgTimer == 0)
	{
		//the code here never gets executed, so not translating
		//Anim_Advance(&materialChg); //Yes, the material changer is secretly an animation struct.
	}
	
	if(regurgTimer <= 0x3c)
	{
		--regurgTimer;
		if(regurgTimer == 0)
			KillAndSpawnSilverStar();
	}
	
	if(wmClsn.JustHitGround())
	{
		if(regurgTimer > 0x3c)
			regurgTimer = 0x1e;

		if(speed.y.Abs() <= 0x500_f * regurgTimer)
			speed.y = regurgTimer * 0x400_f - vertAccel;
		else
			speed.y = -0x50 * speed.y / (0x64 * floorNormal.y);
			
		horzSpeed >>= 1;
		
		if(horzSpeed.Abs() < 0x5000_f)
		{
			if(horzSpeed < 0_f)
				horzSpeed = -0x5000_f;
			else
				horzSpeed = 0x5000_f;
		}
		
		if(regurgBounceCount < 2)
			Sound::PlayBank3(0x13a + regurgBounceCount, camSpacePos);
		
		++regurgBounceCount;
	}
	else if(wmClsn.IsOnGround())
	{
		wmClsn.ClearLimMovFlag();
		horzSpeed = speed.y = 0_f;
		
		if(regurgTimer > 0x3c)
			regurgTimer = 0x1e;
	}
	
	isBeingSpit = true;
	if(SpawnParticlesIfHitOtherObj(cylClsn)) //a74 else
	{
		Actor* other = Actor::FindWithID(cylClsn.otherObjID);
		defeatMethod = Enemy::DF_HIT_REGURG;
		KillByAttack(*other, wmClsn);
		rigMdl.SetAnim(animFiles[ROLLING].filePtr, Animation::NO_LOOP, 0x1000_f, 0);
		cylClsn.flags1 |= 1;
	}
	
	isBeingSpit = false;
}
//0212a6f8
void Goomba::State4()
{
	if(charBehav == 1 && speed.y < 0_f)
	{
		state = 5;
		termVel = SPIN_TERM_VEL;
	}
	
	if(wmClsn.IsOnGround())
		state = 0;
}

void Goomba::State5()
{
	ang.y += SPIN_SPEED;
	
	UpdateTargetDirAndDist(0x3e8000_f);
	AdvanceToDest_Short(motionAng.y, targetDir, 0x800);
	
	if(wmClsn.IsOnGround())
		state = 0;
}

//0212b5bc
int Goomba::Render()
{
	if((flags & Actor::IN_YOSHI_MOUTH) || hasNotSpawned)
		return 1;
	
	Vector3 oldScale = scale;
	
	if(defeatMethod == Enemy::DF_SQUASHED) //694 else
	{
		scale.x = SCALES[sizeType] * scale.x;
		scale.y = SCALES[sizeType] * scale.y;
		scale.z = SCALES[sizeType] * scale.z;
	}
	
	rigMdl.Render(&scale);
	
	scale = oldScale;
	
	materialChg.Update(rigMdl.data);
	texSeq.Update(rigMdl.data);
	RenderCapModel(nullptr);
	
	return 1;
}

void Goomba::Virtual30() {}

//02130948 = vtable
//0219001c = experimental goomba 1
//0218fb94 = experimental goomba 2
//02129020
Goomba::~Goomba() {}

//0212bfc0. Return 0 if Yoshi should not eat it
unsigned Goomba::OnYoshiTryEat()
{
	switch(sizeType)
	{
		case SizeType::SMALL:
			return Actor::YE_SWALLOW;
		case SizeType::NORMAL:
			return Actor::YE_KEEP_AND_CAN_MAKE_EGG;
		default:
			return Actor::YE_DONT_EAT;
	}
}

void Goomba::OnTurnIntoEgg(Player& player)
{
	if(player.Unk_020bea94())
	{
		Unk_0201061c(player, 1, 0);
		SpawnSilverStarIfNecessary();
	}
	else
		player.RegisterEggCoinCount(1, spawnSilverStar == 1, false);
	
	Destroy();
}

Fix12i Goomba::OnAimedAtWithEgg()
{
	switch(sizeType)
	{
		case SizeType::BIG:
			return 0x96000_f;
		case SizeType::NORMAL:
			return 0x41000_f;
		default:
			return 0x14000_f;
	}
}

//0218e958 belongs to 0218e584