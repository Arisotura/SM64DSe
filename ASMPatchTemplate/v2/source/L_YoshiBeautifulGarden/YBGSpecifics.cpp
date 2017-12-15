#include "Garden.h"
#include "AlcoveGrate.h"
#include "SewerClog.h"
#include "SewerBlock.h"
#include "CorkRock.h"
#include "FallingRock.h"
#include "ExtraLava.h"
#include "Sprinkler.h"
#include "PushPipe.h"
#include "Spill.h"
#include "Flood.h"
#include "YBGSpecifics.h"

namespace
{
	uint8_t START_INTENSITY = 0x40;
	uint16_t BRIGHTEN_TIME = 32;
	
	uint16_t timer;
	
	bool starSpawned = false;
	
	int star2Timer = 30;
	
	Particle::MainInfo rainInfo
	{
		(Particle::MainInfo::Flags)0x00004055, //flags
		0x0000c000_f, //rate, fix20.12
		0x00064000_f, //startHorzDist, fix23.9
		Vector3_16f{0x0000_fs, -0x1000_fs, 0x0000_fs}, //dir
		Color5Bit(0xff, 0xff, 0xff), //color
		0x00000000_f, //horzSpeed, fix20.12 (fix23.9???)
		0x0000c000_f, //vertSpeed, fix20.12 (fix23.9???)
		0x00000700_f, //scale, fix20.12
		0x1000_fs, //horzScale, fix4.12
		0x054f,
		0x0000,
		0x0000,
		0x0000, //frames
		0x000c, //lifetime
		0x00, //scaleRand
		0x00, //lifetimeRand
		0x44, //speedRand
		0x00,
		0x01, //spawnPeriod
		0x18, //alpha
		0x80, //speedFalloff
		0x19, //spriteID
		0x01,
		0x00,
		0x60, //velStretchFactor
		0x05  //texMirrorFlags
	};
	
	Particle::SysDef rainSysDef
	{
		&rainInfo,
		nullptr,
		nullptr,
		nullptr,
		nullptr,
		nullptr,
		nullptr,
		0
	};
	
	unsigned rainID = 0;
	
	struct RaycastingSplashCallback : public Particle::SimpleCallback
	{
		virtual bool OnUpdate(Particle::System&, bool active) override;
	} raySplashCallback;
	
	bool RaycastingSplashCallback::OnUpdate(Particle::System& system, bool active)
	{
		RaycastGround raycast; //for handling dynamic ground
		raycast.flags = 0xa; //water only
		for(Particle::Particle* particle = system.particleList.first; particle != nullptr; particle = particle->node.next)
		{
			Vector3 realPos = (particle->posAsr3 + particle->offsetAsr3) << 3;
			if(particle->age == 0)
				particle->flags &= ~(1 << 31);
			if(!(particle->flags & 1 << 31) && particle->speedAsr3.y < 0_f && realPos.y < WATER_HEIGHT)
			{
				particle->flags |= 1 << 31;
				raycast.SetObjAndPos(Vector3{realPos.x, WATER_HEIGHT + 0x14000_f, realPos.z}, nullptr);
				if(raycast.DetectClsn()) //12 raycasts per frame is less than 1/10 of a frame.
				{
					particle->age = particle->lifetime;
					Particle::System::NewRipple(realPos.x, raycast.clsnPosY + 0x3000_f, realPos.z);
				}
			}
		}
		
		return SimpleCallback::OnUpdate(system, active);
	}
}

SpawnInfo<YBG_Specifics> YBG_Specifics::spawnData =
{
	&YBG_Specifics::Spawn,
	0x0004,
	0x0007,
	0x00000000,
	0x00000000_f,
	0x7fffffff_f,
	0x7fffffff_f,
	0x7fffffff_f
};

YBG_Specifics* YBG_Specifics::Spawn()
{
	return new YBG_Specifics;
}

int YBG_Specifics::InitResources()
{
	if(STAR_ID == 2)
		GXFIFO::SetLightColor(0, 0x40, 0x40, 0x40);
	else if(STAR_ID == 3)
		GXFIFO::SetLightColor(0, 0xc0, 0xc0, 0xc0);
	return 1;
}

int YBG_Specifics::CleanupResources()
{
	GXFIFO::SetLightColor(0, 0xff, 0xff, 0xff);
	return 1;
}

int YBG_Specifics::Behavior()
{
	if(STAR_ID != 1)
	{
		((Stage*)ROOT_ACTOR_BASE)->clsn.clpsBlock->clpses[12].low |= 9; //set texture of grass dirt to flowery
	}
	
	if(STAR_ID == 1 && !starSpawned && Event::GetBit(10) && PLAYER_ARR[0]->areaID == 0)
	{
		starSpawned = true;
		Actor::Spawn(0x00b2, 0x0041, Vector3{0xffcc5000_f, 0x41a000_f, 0xbb8000_f}, nullptr, 0, -1);
	}
	if(STAR_ID == 2 && Event::GetBit(5) && star2Timer > 0)
	{
		--star2Timer;
		if(star2Timer == 0)
			Actor::Spawn(0x00b2, 0x0042, Vector3{0xffcc5000_f, 0x41a000_f, 0xbb8000_f}, nullptr, 0, -1);
	}
	
	if(STAR_ID == 3)
	{
		Vector3 pos = CAM_SPACE_CAM_POS_ASR_3.Transform(INV_VIEW_MATRIX_ASR_3) << 3;
		RaycastGround raycast;
		raycast.SetObjAndPos(Vector3{pos.x, 0x07d00000_f, pos.z}, nullptr);
		if(!raycast.DetectClsn() || raycast.clsnPosY < pos.y)
			raySplashCallback.id = Particle::System::New(raySplashCallback.id,
														 (unsigned)&rainSysDef,
														 pos.x,
														 pos.y,
														 pos.z,
														 nullptr,
														 &raySplashCallback);
														 
		if(!starSpawned && Event::GetBit(7))
		{
			starSpawned = true;
			Actor::Spawn(0x00b2, 0x0043, Vector3{0xffcc5000_f, 0x41a000_f, 0xbb8000_f}, nullptr, 0, -1);
		}
	}
	return 1;
}

int YBG_Specifics::Render()
{
	if(STAR_ID != 2 && STAR_ID != 3)
		return 1;
	
	uint8_t intensity;
	if(STAR_ID == 2)
	{
		if(Event::GetBit(5))
		{
			if(timer < BRIGHTEN_TIME)
			++timer;
			intensity = (int)START_INTENSITY + (int)(0xff - START_INTENSITY) * timer / BRIGHTEN_TIME;
		}
		else
			intensity = START_INTENSITY;
	}
	else
		intensity = 0xc0;
	
	GXFIFO::SetLightColor(0, intensity, intensity, intensity);
	*(unsigned*)(((Stage*)ROOT_ACTOR_BASE)->skyBox->data.modelFile + 0x78) = Color5Bit(intensity, intensity, intensity); //hax
	return 1;
}

YBG_Specifics::~YBG_Specifics() {}

void init()
{
	OBJ_TO_ACTOR_ID_TABLE[0x0150] = 0x0172;
	ACTOR_SPAWN_TABLE[0x0172] = (unsigned)&YBG_Specifics::spawnData;
	
	OBJ_TO_ACTOR_ID_TABLE[0x00b7] = 0x003c;
	ACTOR_SPAWN_TABLE[0x003c] = (unsigned)&YBG_Garden::spawnData;
	YBG_Garden::modelFile.Construct(0x0641);
	
	OBJ_TO_ACTOR_ID_TABLE[0x00d1] = 0x0037;
	ACTOR_SPAWN_TABLE[0x0037] = (unsigned)&YBG_AlcoveGrate::spawnData;
	YBG_AlcoveGrate::modelFile.Construct(0x059f);
	YBG_AlcoveGrate::clsnFile .Construct(0x05a0);
	
	OBJ_TO_ACTOR_ID_TABLE[0x00cd] = 0x0033;
	ACTOR_SPAWN_TABLE[0x0033] = (unsigned)&YBG_SewerClog::spawnData;
	YBG_SewerClog::modelFile.Construct(0x059b);
	YBG_SewerClog::clsnFile .Construct(0x059c);
	
	OBJ_TO_ACTOR_ID_TABLE[0x00ce] = 0x0034;
	ACTOR_SPAWN_TABLE[0x0034] = (unsigned)&YBG_SewerBlock::spawnData;
	YBG_SewerBlock::modelFile.Construct(0x0599);
	YBG_SewerBlock::clsnFile .Construct(0x059a);
	
	OBJ_TO_ACTOR_ID_TABLE[0x00c9] = 0x002f;
	ACTOR_SPAWN_TABLE[0x002f] = (unsigned)&YBG_CorkRock::spawnData;
	YBG_CorkRock::modelFile.Construct(0x0595);
	YBG_CorkRock::clsnFile.Construct(0x0596);
	
	OBJ_TO_ACTOR_ID_TABLE[0x00ca] = 0x016f;
	ACTOR_SPAWN_TABLE[0x016f] = (unsigned)&YBG_FallingRock::Spawner::spawnData;
	ACTOR_SPAWN_TABLE[0x0030] = (unsigned)&YBG_FallingRock::spawnData;
	YBG_FallingRock::modelFile.Construct(0x0593);
	YBG_FallingRock::clsnFile.Construct(0x0594);
	
	OBJ_TO_ACTOR_ID_TABLE[0x00cf] = 0x0036;
	ACTOR_SPAWN_TABLE[0x0036] = (unsigned)&YBG_ExtraLava::spawnData;
	YBG_ExtraLava::modelFile.Construct(0x058e);
	YBG_ExtraLava::clsnFile.Construct(0x058f);
	YBG_ExtraLava::texSeqFile.Construct(0x058d);
	
	OBJ_TO_ACTOR_ID_TABLE[0x002d] = 0x0028;
	ACTOR_SPAWN_TABLE[0x0028] = (unsigned)&YBG_Sprinkler::spawnData;
	YBG_Sprinkler::modelFile.Construct(0x05a3);
	YBG_Sprinkler::clsnFile.Construct(0x05a4);
	
	OBJ_TO_ACTOR_ID_TABLE[0x00ad] = 0x0027;
	ACTOR_SPAWN_TABLE[0x0027] = (unsigned)&YBG_PushPipe::spawnData;
	YBG_PushPipe::modelFile.Construct(0x05a5);
	YBG_PushPipe::clsnFile.Construct(0x05a6);
	
	OBJ_TO_ACTOR_ID_TABLE[0x0033] = 0x0029;
	ACTOR_SPAWN_TABLE[0x0029] = (unsigned)&YBG_Spill::spawnData;
	YBG_Spill::modelFile.Construct(0x05a7);
	YBG_Spill::clsnFile.Construct(0x039c);
	YBG_Spill::animFiles[YBG_Spill::STOP_LEAK].Construct(0x05a8);
	
	OBJ_TO_ACTOR_ID_TABLE[0x00d0] = 0x0035;
	ACTOR_SPAWN_TABLE[0x0035] = (unsigned)&YBG_Flood::spawnData;
	YBG_Flood::modelFile.Construct(0x058b);
	YBG_Flood::clsnFile.Construct(0x058c);
	
	new(&raySplashCallback) RaycastingSplashCallback; //call constructor because static initializers don't work
}
