#include "YoshiRide.h"

namespace
{
	enum Animations
	{
		WAIT,
		RIDE,
		RUN,
		
		NUM_ANIMS
	};
	enum States
	{
		ST_WAIT,
		ST_RIDE,
		ST_RUN
	};
	
	struct State
	{
		using FuncPtr = void(YoshiRide::*)();
		FuncPtr init;
		FuncPtr main;
	};
	const State states[]
	{
		State{ &YoshiRide::StWait_Init, &YoshiRide::StWait_Main },
		State{ &YoshiRide::StRide_Init, &YoshiRide::StRide_Main },
		State{ &YoshiRide::StRun_Init,  &YoshiRide::StRun_Main  }
	};
	
	SharedFilePtr ridingAnim;
	SharedFilePtr* animFiles[] =
	{
		(SharedFilePtr*)0x0210ec30,
		&ridingAnim,
		(SharedFilePtr*)0x0210ea28
	};
	SharedFilePtr* headAnimFile = (SharedFilePtr*)0x0210eb88;
	
	constexpr Fix12i RADIUS = 0x63000_f;
	constexpr Fix12i HEIGHT = 0x70000_f;
	
	constexpr Fix12i HORZ_SPEED = 0x14000_f;
	constexpr Fix12i VERT_ACCEL = -0x2000_f;
	constexpr Fix12i TERM_VEL = -0x32000_f;
	
	constexpr uint8_t COOLDOWN_TIME = 30;
	constexpr uint16_t RUN_TIME = 180;
	
	//-1: End ride; do not change character. 1: End ride; change character. 0: Do not end ride
	int ShouldEndRide(Player& player)
	{
		switch((unsigned)player.currState)
		{
			case 0x02110094:
			case 0x021100ac:
			case 0x021100c4:
			case 0x021100dc:
			case 0x021100f4:
				return 1;
		}
		return player.param1 != Player::CH_YOSHI ? -1 : 0;
	}
	
	void ChangeCharacter(Player& player, unsigned character)
	{
		player.SetNewHatCharacter(character, 0, true);
		player.param1 = character;
		player.toonStateAndFlag = 0;
		player.bodyModels[player.GetBodyModelID(character, false)]->CopyAnim(*player.bodyModels[player.GetBodyModelID(player.prevHatChar, false)],
																			 Player::ANIM_PTRS[player.animID + character]->filePtr,
																			 nullptr);
	}
};

SpawnInfo<YoshiRide> YoshiRide::spawnData =
{
	&YoshiRide::Spawn,
	0x0171,
	0x00aa,
	0x00000006,
	0x00032000_f,
	0x00046000_f,
	0x01000000_f,
	0x01000000_f
};

void YoshiRide::UpdateModelTransform()
{
	if(riding)
	{
		rigMdl.mat4x3 = rider->bodyModels[rider->param1]->mat4x3 * rider->bodyModels[rider->param1]->data.transforms[8];
	}
	else
	{
		rigMdl.mat4x3.ThisFromRotationY(ang.y);
		rigMdl.mat4x3.r0c3 = pos.x >> 3;
		rigMdl.mat4x3.r1c3 = pos.y >> 3;
		rigMdl.mat4x3.r2c3 = pos.z >> 3;
	}
	headMdl->mat4x3 = rigMdl.mat4x3 * rigMdl.data.transforms[15];
}

YoshiRide* YoshiRide::Spawn()
{
	return new YoshiRide;
}

int YoshiRide::InitResources()
{
	if(!(headMdl = new ModelAnim))
		return 0;
	
	//The player should load his stuff first, so the SharedFilePtr's should be there before now.
	BoneAnimation::LoadFile(*headAnimFile);
	for(int i = 0; i < NUM_ANIMS; ++i)
		BoneAnimation::LoadFile(*animFiles[i]);
	
	rider = ClosestPlayer();
	
	rigMdl.SetFile(rider->bodyModels[Player::CH_YOSHI]->data.modelFile, 1, -1);
	rigMdl.SetAnim(animFiles[WAIT]->filePtr, Animation::LOOP, 0x1000_f, 0);
	headMdl->SetFile(rider->headModels[Player::CH_YOSHI]->data.modelFile, 1, -1);
	((ModelAnim*)headMdl)->SetAnim(headAnimFile->filePtr, Animation::NO_LOOP, 0x1000_f, 0);
	
	cylClsn.Init(this, RADIUS, HEIGHT, 0x04200004, 0x00000000);
	wmClsn.Init(this, RADIUS, RADIUS, nullptr, nullptr);
	shadow.InitCylinder();
	
	origPos = pos;
	
	UpdateModelTransform();
	
	horzSpeed = 0_f;
	vertAccel = VERT_ACCEL;
	termVel = TERM_VEL;
	state = ST_WAIT; //don't call ChangeState to avoid setting the cooldown timer
	
	return 1;
}

int YoshiRide::CleanupResources()
{
	delete headMdl; //using the virtual destructor to our advantage
	
	headAnimFile->Release();
	for(int i = 0; i < NUM_ANIMS; ++i)
		animFiles[i]->Release();
	return 1;
}

void YoshiRide::ChangeState(uint8_t newState)
{
	state = newState;
	(this->*states[state].init)();
}

void YoshiRide::StartRide(int charID)
{
	//Remember to take care of the player-not-wearing-a-cap case
	riderChar = rider->param1;
	rigMdl.SetFile(rider->bodyModels[charID]->data.modelFile, 1, -1);
	rigMdl.SetAnim(animFiles[RIDE]->filePtr, Animation::LOOP, 0x1000_f, 0);
	((ModelAnim*)headMdl)->~ModelAnim();
	new(headMdl) Model();
	headMdl->SetFile(rider->headModels[charID]->data.modelFile, 1, -1);
	rider->pos = pos;
	
	rider->TurnOffToonShading(rider->prevHatChar);
	rider->TurnOffToonShading(rider->currHatChar);
	rider->TurnOffToonShading(Player::CH_YOSHI);
	ChangeCharacter(*rider, Player::CH_YOSHI);
	
	riding = true;
}

void YoshiRide::EndRide(bool changeChar)
{
	rigMdl.SetFile(rider->bodyModels[Player::CH_YOSHI]->data.modelFile, 1, -1);
	rigMdl.SetAnim(animFiles[WAIT]->filePtr, Animation::LOOP, 0x1000_f, 0);
	headMdl->~Model();
	new(headMdl) ModelAnim();
	headMdl->SetFile(rider->headModels[Player::CH_YOSHI]->data.modelFile, 1, -1);
	((ModelAnim*)headMdl)->SetAnim(headAnimFile->filePtr, Animation::NO_LOOP, 0x1000_f, 0);
	pos = rider->pos;
	rider->pos = Vector3{0_f, 0_f, 0_f}.Transform(rigMdl.mat4x3) << 3;
	
	if(changeChar)
		ChangeCharacter(*rider, riderChar);
	
	riding = false;
}

void YoshiRide::StWait_Init()
{
	rigMdl.SetAnim(animFiles[WAIT]->filePtr, Animation::LOOP, 0x1000_f, 0);
	cooldown = COOLDOWN_TIME;
	horzSpeed = 0_f;
}
void YoshiRide::StWait_Main()
{
	UpdatePos(nullptr);
	Actor* actor = Actor::FindWithID(cylClsn.otherObjID);
	if(!DecIfAbove0_Byte(cooldown) && actor && actor->actorID == 0x00bf)
	{
		Player& player = *(Player*)actor;
		if(player.param1 != 3 && JumpedOnByPlayer(cylClsn, player))
		{
			rider = &player;
			ChangeState(ST_RIDE);
		}
	}
	
	cylClsn.Clear();
	if(state != ST_RIDE)
	{
		cylClsn.Update();
		wmClsn.UpdateDiscreteNoLava();
	}
}

void YoshiRide::StRide_Init()
{
	StartRide(rider->param1);
}
void YoshiRide::StRide_Main()
{
	pos = rider->pos;
	int newArea = 8;
	for(int i = 0; i < 8; ++i)
		if(AREAS[i].showing)
		{
			if(newArea == 8)
				newArea = i;
			else
				newArea = -1;
		}
	if(newArea != 8)
		areaID = newArea;
	
	int endRideState = ShouldEndRide(*rider);
	if(endRideState)
	{
		EndRide(endRideState == 1);
		ChangeState(endRideState == 1 ? ST_RUN : ST_WAIT);
	}
}

void YoshiRide::StRun_Init()
{
	rigMdl.SetAnim(animFiles[RUN]->filePtr, Animation::LOOP, 0x1900_f, 0);
	cooldown = COOLDOWN_TIME;
	runTimer = RUN_TIME;
	motionAng.y = ang.y = rider->ang.y;
}
void YoshiRide::StRun_Main()
{
	horzSpeed = HORZ_SPEED;
	StWait_Main();
	
	if(state == ST_RUN && !DecIfAbove0_Short(runTimer))
	{
		DisappearPoofDustAt(Vector3{pos.x, pos.y + 0x50000_f, pos.z});
		pos = origPos;
		PoofDustAt(Vector3{pos.x, pos.y + 0x50000_f, pos.z});
		ChangeState(ST_WAIT);
	}
}

int YoshiRide::Behavior()
{
	rigMdl.anim.Advance();
	
	(this->*states[state].main)();
	shadowMat = rigMdl.mat4x3;
	shadowMat.r1c3 += 0x14000_f >> 3;
	DropShadowRadHeight(shadow, shadowMat, RADIUS, 0x37000_f, 0xf); //radius and height are (C) Yoshi the Player.
	
	return 1;
}

int YoshiRide::Render()
{
	UpdateModelTransform();
	
	rigMdl.Render(nullptr);
	headMdl->Render(nullptr);
	return 1;
}

YoshiRide::~YoshiRide() {}

void init()
{	
	OBJ_TO_ACTOR_ID_TABLE[0x014f] = 0x0171;
	ACTOR_SPAWN_TABLE[0x0171] = (unsigned)&YoshiRide::spawnData;
	ridingAnim.Construct(0x081f);
}

void cleanup()
{
	
}
