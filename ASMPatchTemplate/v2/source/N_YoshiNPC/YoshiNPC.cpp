#include "YoshiNPC.h"

namespace
{
	enum Animations
	{
		WAIT,
		TALK,
		
		NUM_ANIMS
	};
	
	SharedFilePtr modelFile; //can't share with the player because multiple materials
	SharedFilePtr* animFiles[] =
	{
		(SharedFilePtr*)0x0210ec30,
		(SharedFilePtr*)0x0210ec30  //the redundancy though
	};
	
	using StateFuncPtr = void(YoshiNPC::*)();
	
	const StateFuncPtr stateFuncs[]
	{
		&YoshiNPC::State0_Wait,
		&YoshiNPC::State1_Talk
	};
	
	constexpr Fix12i RADIUS = 0x78000_f;
	constexpr Fix12i HEIGHT = 0x96000_f;
	constexpr Fix12i TALK_RADIUS = 0xf0000_f;
	constexpr Fix12i TALK_HEIGHT = 0x70000_f;
	constexpr short TURN_SPEED = 0x0800;
};

SpawnInfo<YoshiNPC> YoshiNPC::spawnData =
{
	&YoshiNPC::Spawn,
	0x003b,
	0x00aa,
	0x00000003,
	0x00032000_f,
	0x00046000_f,
	0x01000000_f,
	0x01000000_f
};

void YoshiNPC::UpdateModelTransform()
{
	rigMdl.mat4x3.ThisFromRotationY(ang.y);
	rigMdl.mat4x3.r0c3 = pos.x >> 3;
	rigMdl.mat4x3.r1c3 = pos.y >> 3;
	rigMdl.mat4x3.r2c3 = pos.z >> 3;
	
	DropShadowRadHeight(shadow, rigMdl.mat4x3, 0x62980_f, 0x37000_f, 0xf); //radius and height are (C) Yoshi the Player.
}

YoshiNPC* YoshiNPC::Spawn()
{
	return new YoshiNPC;
}

int YoshiNPC::InitResources()
{
	//The player should load his stuff first, so the SharedFilePtr's should be there before now.
	Model::LoadFile(modelFile);
	for(int i = 0; i < NUM_ANIMS; ++i)
		BoneAnimation::LoadFile(*animFiles[i]);
	
	rigMdl.SetFile(modelFile.filePtr, 1, -1);
	rigMdl.SetAnim(animFiles[WAIT]->filePtr, Animation::LOOP, 0x1000_f, 0);
	
	cylClsn.Init(this, RADIUS, HEIGHT, 0x04200004, 0x00000000);
	shadow.InitCylinder();
	
	RaycastGround raycaster;
	raycaster.SetObjAndPos(Vector3{pos.x, pos.y + 0x14000_f, pos.z}, this);
	if(raycaster.DetectClsn())
		pos.y = raycaster.clsnPosY;
	
	UpdateModelTransform();
	
	state = 0;
	listener = nullptr;
	
	messages[0] = param1 & 0xfff;
	messages[1] = ang.x  & 0xfff;
	eventID = (param1 >> 12 & 0xf) | (ang.x >> 8 & 0xf0);
	unsigned r = ang.z >>  0 & 0x1f,
			 g = ang.z >>  5 & 0x1f,
			 b = ang.z >> 10 & 0x1f;
	rigMdl.data.materials[1].difAmb =
		rigMdl.data.materials[2].difAmb = (uint16_t)ang.z | 0x8000 | r >> 1 << 16 
																   | g >> 1 << 21
																   | b >> 1 << 26;
														 
	shouldTalk = ang.z & 0x8000;
	ang.x = ang.z = 0;
	
	return 1;
}

int YoshiNPC::CleanupResources()
{
	modelFile.Release();
	for(int i = 0; i < NUM_ANIMS; ++i)
		animFiles[i]->Release();
	return 1;
}

void YoshiNPC::State0_Wait()
{
	rigMdl.SetAnim(animFiles[WAIT]->filePtr, Animation::LOOP, 0x1000_f, 0);
	if(!(cylClsn.hitFlags & CylinderClsn::HIT_BY_PLAYER))
		return;
	
	Actor* actor = Actor::FindWithID(cylClsn.otherObjID);
	if(!actor || actor->actorID != 0x00bf)
		return;
	
	Player& player = *(Player*)actor;
	if(player.StartTalk(*this, false))
	{
		Message::PrepareTalk();
		state = 1;
		listener = &player;
	}
}
void YoshiNPC::State1_Talk()
{
	rigMdl.SetAnim(animFiles[TALK]->filePtr, Animation::LOOP, 0x1000_f, 0);
	if(!AdvanceToDest_Short(ang.y, pos.HorzAngle(listener->pos), TURN_SPEED))
		return;
	
	int talkState = listener->GetTalkState();
	switch(talkState)
	{
		case Player::TK_NOT:
			Message::EndTalk();
			state = 0;
			break;
			
		case Player::TK_START:
			listener->ShowMessage(*this, shouldTalk ? messages[eventID < 0x20 && Event::GetBit(eventID) ? 1 : 0] : 0x0038,
				Vector3{pos.x, pos.y + TALK_HEIGHT, pos.z}, 0, 0);
			Sound::PlayCharVoice(Player::CH_YOSHI, 0x4, camSpacePos);
			break;
			
		default:
			return;
	}
}

int YoshiNPC::Behavior()
{
	rigMdl.anim.Advance();
	
	(this->*stateFuncs[state])();
	
	cylClsn.Clear();
	cylClsn.Update();
	
	UpdateModelTransform();
	
	return 1;
}

int YoshiNPC::Render()
{
	rigMdl.Render(nullptr);
	return 1;
}

YoshiNPC::~YoshiNPC() {}

void init()
{	
	OBJ_TO_ACTOR_ID_TABLE[0x014b] = 0x003b;
	ACTOR_SPAWN_TABLE[0x003b] = (unsigned)&YoshiNPC::spawnData;
	modelFile.Construct(0x0813);
}

void cleanup()
{
	
}
