#include "ShyGuy.h"

namespace ShyGuy
{
	SharedFilePtr modelFile{0x0263, 0, nullptr};
	SharedFilePtr animFiles[] =
	{
		SharedFilePtr{0x0269, 0, nullptr}
	};

	unsigned vtable[] =
	{
		(unsigned)&InitResources,
		0x02011268,
		0x02011244,
		(unsigned)&CleanupResources,
		0x02011220,
		0x02011214,
		(unsigned)&Behavior,
		0x02010fd4,
		0x02010fc8,
		(unsigned)&Func24,
		0x02010f78,
		0x02010f6c,
		(unsigned)&OnKill,
		0x0204357c,
		0x0204349c,
		0x02043494,
		(unsigned)&Destructor,
		(unsigned)&DestructAndFree,
		(unsigned)&OnYoshiTryEat,
		(unsigned)&OnTurnIntoEgg,
		0x0201014c,
		0x02010148,
		0x02010144,
		0x02010140,
		0x0201013c,
		0x02010138,
		0x02010134,
		0x02010130,
		0x0201012c,
		(unsigned)&OnAimedAtWithEgg,
		0x020100dc,
		
	};

	unsigned spawnData[] =
	{
		(unsigned)&Spawn,
		0x00180169,
		0x10000006,
		0x00032000,
		0x00046000,
		0x01000000,
		0x01000000,
	};
	
	/*const, but rodata not supported in dynamic libraries yet*/ StateFuncPtr stateFuncs[]
	{
		&State0_Wait,
		&State1_Turn
	};
	
	const int RADIUS = 0x50000;
	const int HEIGHT = 0xa0000;
	const int VERT_ACCEL = -0x2000;
	const int TERM_VEL = -0x32000;
	
	const int DAMAGE = 1;
	const int PLAYER_KNOCKBACK = 0xc000;
	const Vector3_16 KNOCKBACK_ROT = {0x1800, 0, 0};
	const int NUM_COINS = 2;
	const int PLAYER_BOUNCE_INIT_VEL = 0x28000;
	
	const uint16_t WAIT_TIME = 45;
	const short TURN_SPEED = 0x400;

	ShyGuy* Spawn()
	{
		ShyGuy* ptr = (ShyGuy*)AllocateSpace(ShyGuy::SIZE);
		if(ptr)
		{
			Enemy_Construct(ptr);
			ptr->vTable() = (unsigned*)&vtable[0];
			
			CylClsn_Construct(&ptr->cylClsn());
			WMClsn_Construct(&ptr->wmClsn());
			ModelAnim_Construct(&ptr->rigMdl());
			Shadow_Construct(&ptr->shadow());
		}
		
		return ptr;
	}
	
	void UpdateModelTransform(ShyGuy* ptr)
	{
		Matrix4x3& modelMat = ptr->rigMdl().model().mat4x3();
		
		Matrix4x3_FromRotationY(&modelMat, ptr->ang().y);
		modelMat.r0c3 = ptr->pos().x >> 3;
		modelMat.r1c3 = ptr->pos().y >> 3;
		modelMat.r2c3 = ptr->pos().z >> 3;
		
		if(!(ptr->flags() & 0x40000))
			Actor_DropShadowRadHeight(ptr, &ptr->shadow(), &modelMat, RADIUS, WMClsn_IsOnGround(&ptr->wmClsn()) ? 0x1e000 : 0x96000, 0xf);
	}

	bool InitResources(ShyGuy* ptr)
	{
		LoadModel(&modelFile);
		for(int i = 0; i < NUM_ANIMS; ++i)
			LoadAnim(&animFiles[i]);
		
		if(!Model_SetFile(&ptr->rigMdl().model(), modelFile.filePtr, 1, -1))
			return false;
		
		if(!Shadow_InitCylinderVolume(&ptr->shadow()))
			return false;
		
		ModelAnim_ChangeAnim(&ptr->rigMdl(), animFiles[WAIT].filePtr, Animation::LOOP, 0x1000, 0);
			
		CylClsn_Init(&ptr->cylClsn(), ptr, RADIUS, HEIGHT, 0x00200000, 0x00a6efe0);
		WMClsn_Init(&ptr->wmClsn(), ptr, RADIUS, HEIGHT >> 1, 0, 0);
		WMClsn_OrFlag2(&ptr->wmClsn());
		
		UpdateModelTransform(ptr);
		
		ptr->coinType() = Enemy::CoinType::YELLOW;
		ptr->numCoinsMinus1() = NUM_COINS - 1;
		
		ptr->targetAngle() = ptr->ang().y;
		ptr->vAccel() = VERT_ACCEL;
		ptr->termVel() = TERM_VEL;
		
		return true;
	}

	bool CleanupResources(ShyGuy* ptr)
	{
		ReleaseSharedFile(&modelFile);
		return true;
	}
	
	void KillShyGuy(ShyGuy* ptr)
	{
		//The coins have already spawned.
		Actor_KillAndTrackInDeathTable(ptr);
	}
	
	void HandleClsn(ShyGuy* ptr)
	{
		Actor* other = FindActorWithID(ptr->cylClsn().otherObjID());
		if(!other)
			return;
		
		CylinderClsn::HitFlags hitFlags = ptr->cylClsn().hitFlags();
		
		if(hitFlags & 0x000667c0)
		{
			if(other->actorID() == 0x00bf)
				Enemy_KillByInvincibleChar(ptr, &KNOCKBACK_ROT, (Player*)other);
			else
			{
				Enemy_SpawnCoin(ptr);
				KillShyGuy(ptr);
			}
			return;
		}
		else if(hitFlags & CylinderClsn::HIT_BY_SPIN_OR_GROUND_POUND)
		{
			ptr->defeatMethod() = Enemy::SQUASHED;
			Enemy_KillByAttack(ptr, other, &ptr->wmClsn());
			return;
		}
		
		if(other->actorID() != 0x00bf)
			return;
		
		Player* player = (Player*)other;
		if(Actor_JumpedOnByPlayer(ptr, &ptr->cylClsn(), player))
		{
			ptr->defeatMethod() = Enemy::SQUASHED;
			Enemy_KillByAttack(ptr, other, &ptr->wmClsn());
			Player_Bounce(player, PLAYER_BOUNCE_INIT_VEL);
		}
		else if(hitFlags & CylinderClsn::HIT_BY_MEGA_CHAR)
		{
			Enemy_SpawnMegaCharParticles(ptr, player, nullptr);
			Sound_PlayBank3(0x001d, &ptr->camSpacePos());
			Enemy_KillByInvincibleChar(ptr, &KNOCKBACK_ROT, player);
		}
		else if(Player_IsOnShell(player) || player->isMetalWario())
		{
			Enemy_KillByInvincibleChar(ptr, &KNOCKBACK_ROT, player);
		}
		else
		{
			Player_Hurt(player, &ptr->pos(), DAMAGE, PLAYER_KNOCKBACK, 1, 0, 1);
		}
		
	}
	
	void State0_Wait(ShyGuy* ptr)
	{
		if(ptr->stateTimer() >= WAIT_TIME)
		{
			ptr->targetAngle() += 0x4000;
			ptr->state() = 1;
		}
	}
	
	void State1_Turn(ShyGuy* ptr)
	{
		if(AdvanceToDest_Short(&ptr->ang().y, ptr->targetAngle(), TURN_SPEED))
			ptr->state() = 0;
	}
	
	bool Behavior(ShyGuy* ptr)
	{
		if(Enemy_UpdateYoshiEat(ptr, &ptr->wmClsn()))
		{
			CylClsn_ClearClsn(&ptr->cylClsn());
		
			if(ptr->isBeingSpit() && ptr->spitTimer() == 0)
				CylClsn_Update(&ptr->cylClsn());
			UpdateModelTransform(ptr);
			return true;
		}
		
		if(ptr->defeatMethod() != Enemy::NOT)
		{
			int res = Enemy_KillByMegaChar(ptr, &ptr->wmClsn(), &ptr->rigMdl(), 3);
			if(res == 2) //finished kill
			{
				KillShyGuy(ptr);
			}
			else if(res == 0) //not yet
			{
				Enemy_UpdateDeath(ptr, &ptr->wmClsn());
				UpdateModelTransform(ptr);
			}
			return true;
		}
		
		int prevState = ptr->state();
		
		//Dun, dun, dun!
		//-----------------------------//
		stateFuncs[ptr->state()](ptr); //
		//-----------------------------//
		
		++ptr->stateTimer();
		if(ptr->state() != prevState)
			ptr->stateTimer() = 0;
		
		
		Actor_UpdatePos(ptr, nullptr);
		UpdateModelTransform(ptr);
		
		Enemy_UpdateWMClsn(ptr, &ptr->wmClsn(), 2);
		HandleClsn(ptr); //must be done before clearing collision, of course
		
		CylClsn_ClearClsn(&ptr->cylClsn());
		if(ptr->defeatMethod() == Enemy::NOT)
			CylClsn_Update(&ptr->cylClsn());
		
		Actor_MakeVanishLuigiWork(ptr, &ptr->cylClsn());
		
		Anim_Advance(&ptr->rigMdl().anim());
		
		return true;
	}

	bool Func24(ShyGuy* ptr)
	{
		if(ptr->flags() & Actor::IN_YOSHI_MOUTH)
			return true;
			
		using ModelFunc14 = void(*)(Model*, Vector3* scale);
		ModelFunc14 modelFunc = (ModelFunc14)ptr->rigMdl().model().vTable()[5];
		modelFunc(&ptr->rigMdl().model(), nullptr);
		
		return true;
	}
	
	void OnKill(ShyGuy* ptr) {}


	ShyGuy* Destructor(ShyGuy* ptr)
	{
		Shadow_Destruct(&ptr->shadow());
		ModelAnim_Destruct(&ptr->rigMdl());
		WMClsn_Destruct(&ptr->wmClsn());
		CylClsn_Destruct(&ptr->cylClsn());
		
		Enemy_Destruct(ptr);
		return ptr;
	}

	ShyGuy* DestructAndFree(ShyGuy* ptr)
	{
		Destructor(ptr);
		FreeHeapAllocation(ptr, *(unsigned**)0x020a0eac);
		return ptr;
	}
	
	int OnYoshiTryEat(ShyGuy* ptr)
	{
		return (int)Actor::YE_KEEP_AND_CAN_MAKE_EGG;
	}
	
	void OnTurnIntoEgg(ShyGuy* ptr, Player* player)
	{
		if(Player_020bea94(player))
			Actor_0201061c(ptr, player, NUM_COINS, 0);
		else
			Player_RegisterEggCoinCount(player, NUM_COINS, false, false);
		
		Actor_Destroy(ptr);
	}
	
	int OnAimedAtWithEgg(ShyGuy* ptr)
	{
		return HEIGHT / 2;
	}
};

void init()
{
	OBJ_TO_ACTOR_ID_TABLE[0x0149] = 0x016b;
	ACTOR_SPAWN_TABLE[0x016b] = (unsigned)&ShyGuy::spawnData[0];
}

void cleanup()
{
	
}