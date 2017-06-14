#include "ShyGuy.h"

namespace
{
	using StateFuncPtr = void(ShyGuy::*)();

	enum Animations
	{
		WAIT,
		WALK,
		RUN,
		FREEZE,
		
		NUM_ANIMS
	};
	SharedFilePtr modelFile;
	SharedFilePtr animFiles[NUM_ANIMS];

	const StateFuncPtr stateFuncs[]
	{
		&ShyGuy::State0_Wait,
		&ShyGuy::State1_Turn,
		&ShyGuy::State2_Chase,
		&ShyGuy::State3_Stop,
		&ShyGuy::State4_Warp
	};

	const char material0Name[] = "mat_body-material";
	const char material1Name[] = "color-material"; //silly DAE adding "-material" to the end of the name (didn't fall for it)
	char matVals[] = {0x1f, 0x1d, 0x1a, 0x18, 0x15, 0x13, 0x10, 0x00};
	MaterialProperties matProps[] =
	{
		MaterialProperties  {
								(short)0xffff, 0,
								&material0Name[0],
								0x01, false, 0x0000,   0x01, false, 0x0000,   0x01, false, 0x0000,
								0x01, false, 0x0006,   0x01, false, 0x0006,   0x01, false, 0x0006, 
								0x01, false, 0x0007,   0x01, false, 0x0007,   0x01, false, 0x0007,
								0x01, false, 0x0007,   0x01, false, 0x0007,   0x01, false, 0x0007,
								0x01, true , 0x0000
							},
							
		MaterialProperties  {
								(short)0xffff, 0,
								&material1Name[0],
								0x01, false, 0x0000,   0x01, false, 0x0000,   0x01, false, 0x0000,
								0x01, false, 0x0006,   0x01, false, 0x0006,   0x01, false, 0x0006, 
								0x01, false, 0x0007,   0x01, false, 0x0007,   0x01, false, 0x0007,
								0x01, false, 0x0007,   0x01, false, 0x0007,   0x01, false, 0x0007,
								0x01, true , 0x0000
							}
	};

	MaterialDef matDef = {7, 0x0000, &matVals[0], 2, &matProps[0]};

	constexpr Fix12i RADIUS = 0x50000_f;
	constexpr Fix12i HEIGHT = 0xa0000_f;
	constexpr Fix12i VERT_ACCEL = -0x2000_f;
	constexpr Fix12i TERM_VEL = -0x32000_f;

	constexpr int DAMAGE = 1;
	constexpr Fix12i PLAYER_KNOCKBACK = 0xc000_f;
	constexpr Vector3_16 KNOCKBACK_ROT = {0x1800, 0, 0};
	constexpr int NUM_COINS = 2;
	constexpr Fix12i PLAYER_BOUNCE_INIT_VEL = 0x28000_f;

	constexpr uint16_t WAIT_TIME = 45;
	constexpr short TURN_SPEED = 0x400;
	constexpr Fix12i SIGHT_COS_RADIUS = 0xddb_f; //cos(30 degrees)
	constexpr Fix12i SIGHT_DIST = 0x4b0000_f;

	constexpr Fix12i WALK_SPEED = 0x8000_f;
	constexpr Fix12i WARP_SPEED = 0x20000_f;
	constexpr Fix12i TARGET_POS_TOLERANCE = 0x8000_f;
	constexpr Fix12i TARGET_POS_WARP_TOL = 0x20000_f;
	constexpr Fix12i CLIFF_TOLERANCE = 0x32000_f;
	constexpr Fix12i HEIGHT_TOLERANCE = HEIGHT >> 1;

	constexpr Fix12i CHASE_SPEED = 0x10000_f;
	constexpr Fix12i CHASE_ACCEL = 0x800_f;
	constexpr short CHASE_TURN_SPEED = 0xa00;
	constexpr Fix12i QUIT_CHASE_DIST = 0x4b0000_f;
	constexpr uint8_t CHASE_COOLDOWN = 30;

	constexpr uint8_t GIVE_UP_TIMER = 150;
}

unsigned ShyGuy::spawnData[] =
{
	(unsigned)&ShyGuy::Spawn,
	0x0018016b,
	0x10000006,
	0x00032000,
	0x00046000,
	0x01000000,
	0x01000000,
};

ShyGuy* ShyGuy::Spawn()
{
	static_assert(sizeof(Bone) == 0x34, "Bone is wrong size");
	static_assert(sizeof(Animation) == 0x14, "Anim is wrong size");
	static_assert(sizeof(BoneAnimation) == 0x14, "BoneAnim is wrong size");
	static_assert(sizeof(MaterialChanger) == 0x14, "MatChg is wrong size");
	static_assert(sizeof(TextureSequence) == 0x14, "TexSeq is wrong size");
	static_assert(sizeof(Model) == 0x50, "Model is wrong size");
	static_assert(sizeof(ModelAnim) == 0x64, "ModelAnim is wrong size");
	static_assert(sizeof(ShadowVolume) == 0x28, "Shadow is wrong size");
	static_assert(sizeof(MovingMeshCollider) == 0x50, "MovingMeshCollider is wrong size");
	static_assert(sizeof(CylinderClsn) == 0x34, "CylinderClsn is wrong size");
	static_assert(sizeof(WithMeshClsn) == 0x1bc, "WithMeshClsn is wrong size");
	static_assert(sizeof(RaycastLine) == 0x78, "RaycastLine is wrong size");
	static_assert(sizeof(RaycastGround) == 0x50, "RaycastGround is wrong size");
	static_assert(sizeof(Particle::System) == 0x78, "ParticleSystem is wrong size");
	static_assert(sizeof(ActorBase) == 0x50, "ActorBase is wrong size");
	static_assert(sizeof(Actor) == 0xd4, "Actor is wrong size");
	static_assert(sizeof(Platform) == 0x320, "Platform is wrong size");
	static_assert(sizeof(Enemy) == 0x110, "Enemy is wrong size");
	static_assert(sizeof(CapEnemy) == 0x180, "CapEnemy is wrong size");
	static_assert(sizeof(Camera) == 0x1a8, "Camera is wrong size");
	static_assert(sizeof(Stage) == 0x9c8, "Stage is wrong size");
	static_assert(sizeof(Player) == 0x788, "Player is wrong size");
	static_assert(sizeof(Minimap) == 0x258, "Minimap is wrong size");
	
	return new ShyGuy;
}

void ShyGuy::UpdateModelTransform()
{
	Matrix4x3& modelMat = rigMdl.mat4x3;
	
	modelMat.ThisFromRotationY(ang.y);
	modelMat.r0c3 = pos.x >> 3;
	modelMat.r1c3 = pos.y >> 3;
	modelMat.r2c3 = pos.z >> 3;
	
	if(!(flags & 0x40000))
		DropShadowRadHeight(shadow, modelMat, RADIUS, wmClsn.IsOnGround() ? 0x1e000_f : 0x96000_f, 0xf);
}

Fix12i ShyGuy::FloorY(const Vector3& pos)
{
	Vector3 raycasterPos = {pos.x, pos.y + 0x14000_f, pos.z};
	RaycastGround raycaster;
	raycaster.SetObjAndPos(raycasterPos, nullptr);
	Fix12i res;
	if(raycaster.DetectClsn())
		res = raycaster.clsnPosY;
	else
		res = pos.y;
	return res;
}

void ShyGuy::SetTargetPos()
{
	pathPtr.GetPt(targetPos, nextPathPt);
	targetPos.y = FloorY(targetPos);
	targetAngle = pos.HorzAngle(targetPos);
}

int ShyGuy::InitResources()
{
	Model::LoadFile(modelFile);
	for(int i = 0; i < NUM_ANIMS; ++i)
		BoneAnimation::LoadFile(animFiles[i]);
	
	if(!rigMdl.SetFile(modelFile.filePtr, 1, -1))
		return 0;
	
	if(!shadow.InitCylinder())
		return 0;
	
	rigMdl.SetAnim(animFiles[WAIT].filePtr, Animation::LOOP, 0x1000_f, 0);
	MaterialChanger::Prepare(modelFile.filePtr, matDef);
	matChg.SetFile(matDef, Animation::NO_LOOP, 0x1000_f, 0);
		
	cylClsn.Init(this, RADIUS, HEIGHT, 0x00200000, 0x00a6efe0);
	wmClsn.Init(this, RADIUS, HEIGHT >> 1, nullptr, nullptr);
	
	UpdateModelTransform();
	
	scale = Vector3{0x1000_f, 0x1000_f, 0x1000_f};
	
	coinType = Enemy::CN_YELLOW;
	numCoinsMinus1 = NUM_COINS - 1;
	
	targetAngle = ang.y;
	vertAccel = VERT_ACCEL;
	termVel = TERM_VEL;
	
	state = chaseCooldown = 0;
	targetPlayer = nullptr;
	
	offTrack = false;
	
	if(param1 != 0xffff)
	{
		backAndForth = param1 & 0x8000;
		reverse = false;
		
		pathPtr.FromID(param1 & 0x7fff);
		numPathPts = pathPtr.NumPts();
		nextPathPt = 1;
		pathPtr.GetPt(pos, 0);
		SetTargetPos();
	}
	else
	{
		targetPos = Vector3{pos.x, FloorY(pos), pos.z};
	}
	
	return 1;
}

int ShyGuy::CleanupResources()
{
	for(int i = 0; i < NUM_ANIMS; ++i)
		animFiles[i].Release();
	modelFile.Release();
	return 1;
}

void ShyGuy::Kill()
{
	//The coins have already spawned.
	Sound::PlayBank3(0xd6, camSpacePos);
	KillAndTrackInDeathTable();
}

void ShyGuy::HandleClsn()
{
	Actor* other = Actor::FindWithID(cylClsn.otherObjID);
	if(!other)
		return;
	
	unsigned hitFlags = cylClsn.hitFlags;
	
	if(hitFlags & 0x000667c0)
	{
		if(other->actorID == 0x00bf)
			KillByInvincibleChar(KNOCKBACK_ROT, *(Player*)other);
		else
		{
			SpawnCoin();
			Kill();
		}
		return;
	}
	else if(hitFlags & CylinderClsn::HIT_BY_SPIN_OR_GROUND_POUND)
	{
		defeatMethod = Enemy::DF_SQUASHED;
		KillByAttack(*other, wmClsn);
		scale.y = 0x0555_f;
		Sound::PlayBank3(0xe0, camSpacePos);
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
		Sound::PlayBank3(0xe0, camSpacePos);
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
	else
	{
		player->Hurt(pos, DAMAGE, PLAYER_KNOCKBACK, 1, 0, 1);
		if(state == 2)
		{
			state = 0;
			chaseCooldown = CHASE_COOLDOWN;
		}
		else if((state == 0 || state == 1) && chaseCooldown == 0)
		{
			state = 1;
			alarmed = true;
			targetAngle = pos.HorzAngle(player->pos);
		}
	}
	
}

Player* ShyGuy::PlayerVisibleToThis(Player* player)
{
	if(!player)
		player = ClosestPlayer();
	if(!player)
		return nullptr;
	
	Vector3 eyePos = {pos.x, pos.y + (HEIGHT >> 1), pos.z};
	Vector3 playerPos = {player->pos.x, player->pos.y + 0x48000_f, player->pos.z};
	
	Vector3 forward = {0_f, 0_f, 0x1000_f};
	Vector3 toPlayer = playerPos - eyePos;
	Fix12i dist = toPlayer.Len();
	if(dist > SIGHT_DIST)
		return nullptr;
	
	MATRIX_SCRATCH_PAPER.ThisFromRotationY(ang.y);
	forward.TransformThis(MATRIX_SCRATCH_PAPER);
	if(toPlayer.Dot(forward) < dist * SIGHT_COS_RADIUS)
		return nullptr;
	
	RaycastLine raycaster;
	raycaster.SetObjAndLine(eyePos, playerPos, nullptr);
	return !raycaster.DetectClsn() ? player : nullptr;
}

bool ShyGuy::KillIfTouchedBadSurface()
{
	if(wmClsn.IsOnGround())
		return false;
	
	CLPS& clps = wmClsn.sphere.floorResult.clps;
	int behav = clps.BehaviorID();
	
	
	if((clps.TextureID() == CLPS::TX_SAND &&
		(behav == CLPS::BH_LOW_JUMPS || behav == CLPS::BH_SLOW_QUICKSAND || behav == CLPS::BH_SLOW_QUICKSAND_2 || behav == CLPS::BH_INSTANT_QUICKSAND)) ||
		clps.IsWater() ||
	   behav == CLPS::BH_WIND_GUST || behav == CLPS::BH_LAVA || behav == CLPS::BH_DEATH || behav == CLPS::BH_DEATH_2)
	{
		SpawnCoin();
		Kill();
		return true;
	}
	return false;
}

int ShyGuy::GetClosestPathPtID()
{
	Fix12i closestDist = 0x7fffffff_f;
	int closestPt = 0;
	Vector3 pathPt;
	for(int i = 0; i < numPathPts; ++i)
	{
		pathPtr.GetPt(pathPt, i);
		Fix12i dist = pos.Dist(pathPt);
		if(dist < closestDist)
		{
			closestDist = dist;
			closestPt = i;
		}
	}
	return closestPt;
}

void ShyGuy::AimAtClosestPathPt()
{
	if(pathPtr.path)
	{
		nextPathPt = GetClosestPathPtID();
		SetTargetPos();
	}
}

void ShyGuy::PlayMovingSoundEffect()
{
	if(rigMdl.anim.file != animFiles[WALK].filePtr && rigMdl.anim.file != animFiles[RUN].filePtr)
		return;
	
	int animLen = (int)rigMdl.anim.GetNumFrames();
	int currFrame = (int)rigMdl.anim.currFrame;
	if(currFrame == 0 || currFrame == animLen / 2)
		Sound::PlayBank3(0xd0, camSpacePos);
}

void ShyGuy::State0_Wait()
{
	if(offTrack)
	{
		Vector3 targetDir = targetPos - pos;
		
		if((wmClsn.IsOnWall() && targetDir.Dot(wallNormal) < 0_f) ||
			IsGoingOffCliff(wmClsn, 0x32000_f, 0x1f49, 0, 1, CLIFF_TOLERANCE) ||
			(pos.HorzDist(targetPos) <= TARGET_POS_TOLERANCE && (targetPos.y - pos.y).Abs() > HEIGHT_TOLERANCE))
		{
			state = 4;
			return;
		}
	}
		
	if(pos.HorzDist(targetPos) > TARGET_POS_TOLERANCE)
	{
		rigMdl.SetAnim(animFiles[WALK].filePtr, Animation::LOOP, 0x1000_f, 0);
		horzSpeed = WALK_SPEED;
		motionAng.y = ang.y = pos.HorzAngle(targetPos);
	}
	else if(pathPtr.path)
	{
		nextPathPt += reverse ? -1 : 1;
		if(backAndForth)
		{
			if(nextPathPt == numPathPts - 1)
				reverse = true;
			else if(nextPathPt == 0)
				reverse = false;
		}
		else if(nextPathPt >= numPathPts)
			nextPathPt = 0;
		
		SetTargetPos();
		state = 1;
	}
	else
	{
		rigMdl.SetAnim(animFiles[WAIT].filePtr, Animation::LOOP, 0x1000_f, 0);
		horzSpeed = 0_f;
		if(stateTimer >= WAIT_TIME)
		{
			targetAngle += 0x4000;
			state = 1;
		}
	}
}

void ShyGuy::State1_Turn()
{
	horzSpeed = 0_f;
	rigMdl.SetAnim(animFiles[WAIT].filePtr, Animation::LOOP, 0x1000_f, 0);
	if(AdvanceToDest_Short(ang.y, targetAngle, TURN_SPEED))
		state = alarmed ? 2 : 0;
}

void ShyGuy::State2_Chase()
{
	//if(rigMdl.anim.currFrame >= 0x1000)
	//	rigMdl.anim.currFrame -= 0x1000;
	rigMdl.SetAnim(animFiles[RUN].filePtr, Animation::LOOP, 0x1000_f, 0);
	
	offTrack = true;
	if(flags & Actor::OFF_SCREEN)
	{
		state = 0;
		chaseCooldown = CHASE_COOLDOWN;
		AimAtClosestPathPt();
	}
	if(pos.Dist(targetPlayer->pos) >= QUIT_CHASE_DIST)
		state = 3;
	
	targetAngle = pos.HorzAngle(targetPlayer->pos);
	
	horzSpeed.AdvanceToDest(CHASE_SPEED, CHASE_ACCEL);
	AdvanceToDest_Short(ang.y, targetAngle, CHASE_TURN_SPEED);
	motionAng.y = ang.y;
}

void ShyGuy::State3_Stop()
{
	AimAtClosestPathPt();	
	rigMdl.SetAnim(animFiles[FREEZE].filePtr, Animation::LOOP, 0x1000_f, 0);
	offTrack = true;
	if(horzSpeed.AdvanceToDest(0_f, CHASE_ACCEL))
	{
		state = 0;
		chaseCooldown = CHASE_COOLDOWN;
	}
}

void ShyGuy::State4_Warp()
{
	if(stateTimer == 0)
	{
		vertAccel = 0_f;
		termVel = 0x80000000_f;
		flags &= ~Actor::AIMABLE_BY_EGG;
	}
	
	if(pos.HorzDist(targetPos) > TARGET_POS_WARP_TOL)
	{
		int vertAngle = targetPos.VertAngle(pos);
		horzSpeed  = WARP_SPEED * Cos(vertAngle);
		speed.y = WARP_SPEED * Sin(vertAngle);
		motionAng.y = ang.y = pos.HorzAngle(targetPos);
	}
	else
	{
		state = 0;
		chaseCooldown = CHASE_COOLDOWN;
		
		speed.y = 0_f;
		vertAccel = VERT_ACCEL;
		termVel = TERM_VEL;
		flags |= Actor::AIMABLE_BY_EGG;
		
		offTrack = false;
	}
	
	matChg.currFrame.AdvanceToDest(0x6000_f, 0x1000_f);
}

int ShyGuy::Behavior()
{
	if(defeatMethod != Enemy::DF_NOT)
	{
		int res = UpdateKillByInvincibleChar(wmClsn, rigMdl, 3);
		if(res == 2) //finished kill
		{
			Kill();
		}
		else if(res == 0) //not yet
		{
			UpdateDeath(wmClsn);
			UpdateModelTransform();
		}
		return 1;
	}
	
	int eatState = UpdateYoshiEat(wmClsn);
	if(eatState == Enemy::UY_NOT || isBeingSpit)
	{
		if(KillIfTouchedBadSurface())
			return 1;
	}
	
	if(eatState != Enemy::UY_NOT)
	{
		cylClsn.Clear();
	
		if(isBeingSpit && spitTimer == 0)
			cylClsn.Update();
		UpdateModelTransform();
		
		if(isBeingSpit && wmClsn.JustHitGround())
		{
			isBeingSpit = false;
			state = 3;
		}
		return 1;
	}
	
	int prevState = state;
	
	//Dun, dun, dun!
	//----------------------------//
	(this->*stateFuncs[state])(); //dat syntax tho
	//----------------------------//
	
	++stateTimer;
	if(state != prevState)
		stateTimer = 0;
	
	bool cooledDown = DecIfAbove0_Byte(chaseCooldown) == 0;
	if((state == 0 || state == 1) && cooledDown && !(flags & Actor::OFF_SCREEN))
	{
		targetPlayer = PlayerVisibleToThis(nullptr);
		if(targetPlayer)
			state = 2;
	}
	if(state != 1)
		alarmed = false;
	
	
	UpdatePos(nullptr);
	UpdateModelTransform();
	
	if(state == 4)
	{
		cylClsn.Clear();
		return 1;
	}
	
	matChg.currFrame.AdvanceToDest(0_f, -0x1000_f);
	
	UpdateWMClsn(wmClsn, 2);
	HandleClsn(); //must be done before clearing collision, of course
	
	cylClsn.Clear();
	if(defeatMethod == Enemy::DF_NOT)
		cylClsn.Update();
	
	MakeVanishLuigiWork(cylClsn);
	
	PlayMovingSoundEffect();
	rigMdl.anim.Advance();
	
	return 1;
}

int ShyGuy::Render()
{
	if(flags & Actor::IN_YOSHI_MOUTH)
		return 1;
		
	rigMdl.Render(&scale);
	
	matChg.Update(rigMdl.data);
	if(pathPtr.path)
		rigMdl.data.materials[1].difAmb = 0x0010801f; //red
	else
		rigMdl.data.materials[1].difAmb = 0x4100fe00; //blue
	
	return 1;
}

void ShyGuy::Virtual30() {}

ShyGuy::~ShyGuy() {}

unsigned ShyGuy::OnYoshiTryEat()
{
	return state == 4 ? Actor::YE_DONT_EAT : Actor::YE_KEEP_AND_CAN_MAKE_EGG;
}

void ShyGuy::OnTurnIntoEgg(Player& player)
{
	if(player.Unk_020bea94())
		Unk_0201061c(player, NUM_COINS, 0);
	else
		player.RegisterEggCoinCount(NUM_COINS, false, false);
	
	Destroy();
}

Fix12i ShyGuy::OnAimedAtWithEgg()
{
	return HEIGHT >> 1;
}




void init()
{
	OBJ_TO_ACTOR_ID_TABLE[0x0149] = 0x016b;
	ACTOR_SPAWN_TABLE[0x016b] = (unsigned)&ShyGuy::spawnData[0];
	modelFile.Construct(0x0390);
	animFiles[WAIT  ].Construct(0x0391);
	animFiles[WALK  ].Construct(0x0392);
	animFiles[RUN   ].Construct(0x0393);
	animFiles[FREEZE].Construct(0x0394);
}

void cleanup()
{
	
	
}