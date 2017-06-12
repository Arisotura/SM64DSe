#include "(_Name).h"

namespace
{
#ifdeftmpl _UseStates
	using StateFuncPtr = void((_Name)::*)();
	
	const StateFuncPtr stateFuncs[]
	{
		(_StateFuncPtrs)
	};
#endiftmpl

	enum Animations
	{
		(_AnimNameList)
		
		NUM_ANIMS
	};
	SharedFilePtr modelFile{(_ModelFileID), 0, nullptr};
	SharedFilePtr animFiles[] =
	{
		(_AnimSharedFilePtrs)
	};

	constexpr Fix12i RADIUS = (_Radius);
	constexpr Fix12i HEIGHT = (_Height);
#ifdeftmpl _UseGravity
	constexpr Fix12i VERT_ACCEL = (_VertAccel);
	constexpr Fix12i TERM_VEL = (_TermVel);
#endiftmpl

	constexpr int DAMAGE = (_Damage);
	constexpr Fix12i PLAYER_KNOCKBACK = (_Knockback);
	constexpr Vector3_16 KNOCKBACK_ROT = (_KnockbackRotSpeedOnHit);
	constexpr int NUM_COINS = (_NumCoins);
	constexpr Fix12i PLAYER_BOUNCE_INIT_VEL = (_PlayerBounceInitVel);
}

SpawnInfo<(_Name)> (_Name)::spawnData =
{
	&(_Name)::Spawn,
	(_ActorID),
	0x0000,
	(_Flags),
	(_Height) / 2,
	(_Radius),
	(_DrawDistance),
	(_DrawDistance),
};

(_Name)* (_Name)::Spawn()
{
	return new (_Name);
}

void (_Name)::UpdateModelTransform()
{
	Matrix4x3& modelMat = rigMdl.mat4x3;
	
	modelMat.ThisFromRotationY(ang.y);
	modelMat.r0c3 = pos.x >> 3;
	modelMat.r1c3 = pos.y >> 3;
	modelMat.r2c3 = pos.z >> 3;
	
#ifdeftmpl _YoshiCanEat
	if(!(flags & 0x40000))
#endiftmpl
	#indentbackifntmpl _YoshiCanEat
		DropShadowRadHeight(shadow, modelMat, RADIUS, wmClsn.IsOnGround() ? 0x1e000_f : 0x96000_f, 0xf);
	#endiftmpl
}

int (_Name)::InitResources()
{
	Model::LoadFile(modelFile);
	for(int i = 0; i < NUM_ANIMS; ++i)
		BoneAnimation::LoadFile(animFiles[i]);
	
	if(!rigMdl.SetFile(modelFile.filePtr, 1, -1))
		return 0;
	
	if(!shadow.InitCylinder())
		return 0;
	
	rigMdl.SetAnim(animFiles[(_FirstAnimName)].filePtr, Animation::LOOP, 0x1000_f, 0);
		
	cylClsn.Init(this, RADIUS, HEIGHT, (_CylinderClsnFlags), (_VulnerabilityFlags));
	wmClsn.Init(this, RADIUS, HEIGHT >> 1, nullptr, nullptr);
#ifdeftmpl _DetectWater
	wmClsn.SetFlag_2();
#endiftmpl
	
	UpdateModelTransform();
	
	scale = Vector3{0x1000_f, 0x1000_f, 0x1000_f};
	
	coinType = Enemy::(_CoinType);
	numCoinsMinus1 = NUM_COINS - 1;
	
#ifdeftmpl _UseGravity
	vertAccel = VERT_ACCEL;
	termVel = TERM_VEL;
#endiftmpl
	
#ifdeftmpl _UseStates
	state = 0;
#endiftmpl
	
	return 1;
}

int (_Name)::CleanupResources()
{
	for(int i = 0; i < NUM_ANIMS; ++i)
		animFiles[i].Release();
	modelFile.Release();
	return 1;
}

#ifdeftmpl _Defeatable
void (_Name)::Kill()
{
	//The coins have already spawned.
	Sound::PlayBank3((_KillSoundID), camSpacePos);
#ifdeftmpl _RegenerateOnReloadLevelPart
	Destroy();
#elsetmpl
	KillAndTrackInDeathTable();
#endiftmpl
}
#endiftmpl

void (_Name)::HandleClsn()
{
	Actor* other = Actor::FindWithID(cylClsn.otherObjID);
	if(!other)
		return;
	
	unsigned hitFlags = cylClsn.hitFlags;

#ifdeftmpl _DefeatableByAttacks
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
#endiftmpl
#ifdeftmpl _GroundPoundable
	if(hitFlags & CylinderClsn::HIT_BY_SPIN_OR_GROUND_POUND)
	{
		defeatMethod = Enemy::DF_SQUASHED;
		KillByAttack(*other, wmClsn);
		scale.y = 0x0555_f;
		Sound::PlayBank3((_GroundPoundSoundID), camSpacePos);
		return;
	}
#endiftmpl
	
	if(other->actorID != 0x00bf)
		return;
	
	Player* player = (Player*)other;
#ifdeftmpl _DefeatableByJump
	if(JumpedOnByPlayer(cylClsn, *player))
	{
		defeatMethod = Enemy::DF_SQUASHED;
		KillByAttack(*other, wmClsn);
		scale.y = 0x0555_f;
		Sound::PlayBank3((_JumpedOnSoundID), camSpacePos);
		player->Bounce(PLAYER_BOUNCE_INIT_VEL);
		return;
	}
#endiftmpl
#ifdeftmpl _DefeatableByMegaChar
	if(hitFlags & CylinderClsn::HIT_BY_MEGA_CHAR)
	{
		SpawnMegaCharParticles(*player, nullptr);
		Sound::PlayBank3((_MegaCharSoundID), camSpacePos);
		KillByInvincibleChar(KNOCKBACK_ROT, *player);
		return;
	}
#endiftmpl
#ifdeftmpl _CanBeShelled
	if(player->IsOnShell())
	{
		KillByInvincibleChar(KNOCKBACK_ROT, *player);
		return;
	}
#endiftmpl
#ifdeftmpl _DefeatableByMetalChar
	if(player->isMetalWario)
	{
		KillByInvincibleChar(KNOCKBACK_ROT, *player);
		return;
	}
#endiftmpl
#ifdeftmpl _HurtPlayer
	player->Hurt(pos, DAMAGE, PLAYER_KNOCKBACK, 1, 0, 1);
#endiftmpl
	
}

#ifdeftmpl _DefeatableByBadSurface
bool (_Name)::KillIfTouchedBadSurface()
{
	if(wmClsn.IsOnGround())
		return false;
	
	CLPS& clps = wmClsn.sphere.floorResult.clps;
	int behav = clps.BehaviorID();
	
	if((clps.TextureID() == CLPS::TX_SAND &&
		(behav == CLPS::BH_LOW_JUMPS || behav == CLPS::BH_SLOW_QUICKSAND || behav == CLPS::BH_SLOW_QUICKSAND_2 || behav == CLPS::BH_INSTANT_QUICKSAND)) ||
#ifndeftmpl _WalkOnWater
		clps.IsWater() ||
#endiftmpl
	   behav == CLPS::BH_WIND_GUST || behav == CLPS::BH_LAVA || behav == CLPS::BH_DEATH || behav == CLPS::BH_DEATH_2)
	{
		SpawnCoin();
		Kill();
		return true;
	}
	return false;
}
#endiftmpl

#ifdeftmpl _UseStates
(_StateFuncDefs)
#endiftmpl

int (_Name)::Behavior()
{
#ifdeftmpl _Defeatable
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
#endiftmpl
	
#ifdeftmpl _YoshiCanEat
	int eatState = UpdateYoshiEat(wmClsn);
#ifdeftmpl _DefeatableByBadSurface
	if(eatState == Enemy::UY_NOT || isBeingSpit)
	{
		if(KillIfTouchedBadSurface())
			return 1;
	}
#endiftmpl
	
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
#elifdeftmpl _DefeatableByBadSurface
	if(KillIfTouchedBadSurface())
		return 1;
#endiftmpl
	
#ifdeftmpl _UseStates
	int prevState = state;
	
	//Dun, dun, dun!
	//----------------------------//
	(this->*stateFuncs[state])(); //dat syntax tho
	//----------------------------//
	
	++stateTimer;
	if(state != prevState)
		stateTimer = 0;
#endiftmpl
	
	(_UpdatePosFunc)(nullptr);
	UpdateModelTransform();
	
	UpdateWMClsn(wmClsn, 2);
	HandleClsn(); //must be done before clearing collision, of course
	
	cylClsn.Clear();
	if(defeatMethod == Enemy::DF_NOT)
		cylClsn.Update();
	
	MakeVanishLuigiWork(cylClsn);
	
	rigMdl.anim.Advance();
	
	return 1;
}

int (_Name)::Render()
{
#ifdeftmpl _YoshiCanEat
	if(flags & Actor::IN_YOSHI_MOUTH)
		return 1;
#endiftmpl
		
	rigMdl.Render(&scale);
	
	return 1;
}

void (_Name)::Virtual30() {}

(_Name)::~(_Name)() {}

#ifdeftmpl _YoshiCanEat
unsigned (_Name)::OnYoshiTryEat()
{
	return Actor::(_YoshiEatReturnVal)
}

void (_Name)::OnTurnIntoEgg(Player& player)
{
	if(player.Unk_020bea94())
		Unk_0201061c(player, NUM_COINS, 0);
	else
		player.RegisterEggCoinCount(NUM_COINS, false, false);
	
	Kill();
}
#endiftmpl

#ifdeftmpl _YoshiEggCanAim
Fix12i (_Name)::OnAimedAtWithEgg()
{
	return HEIGHT >> 1;
}
#endiftmpl

#ifndeftmpl _LevelSpecific
void init()
{
	OBJ_TO_ACTOR_ID_TABLE[(_ObjectID)] = (_ActorID);
	ACTOR_SPAWN_TABLE[(_ActorID)] = (unsigned)&(_Name)::spawnData;
}

void cleanup()
{
	
}
#endiftmpl