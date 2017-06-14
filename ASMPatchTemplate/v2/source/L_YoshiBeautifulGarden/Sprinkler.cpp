#include "Sprinkler.h"

namespace
{
	FixedSizeCLPS_Block<1> clpsBlock =
	{
		{'C', 'L', 'P', 'S'},
		8,
		1,
		{
			CLPS(0x00, 0, 0x3f, 0x0, 0x0, 0x00, 0, 0, 0, 0xff)
		}
	};
	
	const Fix12i CLSN_SCALE = 0x200_f;
	
	const unsigned ACTOR_FLAGS = 0x00000002;
	const Fix12i CLSN_RANGE_OFFSET_Y = 0x00032000_f;
	const Fix12i CLSN_RANGE = 0x00100000_f;
	const Fix12i DRAW_DIST = 0x01000000_f;
	
	const short ROT_SPEED = 0x100;
	
	Particle::MainInfo particleInfo
	{
		(Particle::MainInfo::Flags)0x010043d5, //flags
		0x00003000_f, //rate, fix20.12
		0x00002000_f, //startHorzDist, fix23.9
		Vector3_16f{0x0000_fs, 0x0000_fs, 0x1000_fs}, //dir
		Color5Bit(0xd8, 0xff, 0xff), //color
		0x00001800_f, //horzSpeed, fix20.12 (fix23.9???)
		0x00008000_f, //vertSpeed, fix20.12 (fix23.9???)
		0x00001000_f, //scale, fix20.12
		0x1000_fs, //horzScale, fix4.12
		0x0000,
		0x0000,
		0x0000,
		0x0000, //frames
		0x0040, //lifetime
		0x33, //scaleRand
		0x0f, //lifetimeRand
		0x80, //speedRand
		0x00,
		0x01, //spawnPeriod
		0x18, //alpha
		0x80, //speedFalloff
		0x19, //spriteID
		0x01,
		0x00,
		0x48, //velStretchFactor
		0x05 //texMirrorFlags
	};
	
	Particle::ScaleTransition particleScaleTrans
	{
		0x1000_fs, //scaleStart, fix4.12
		0x1000_fs, //scaleMiddle, fix4.12
		0x0800_fs, //scaleEnd, fix4.12
		0x00, //scaleTrans1End
		0x80, //scaleTrans2Start;
		0x0000,
		0x0000
	};
	
	Particle::ColorTransition particleColorTrans
	{
		Color5Bit(0xff, 0xff, 0xff), //colorStart
		Color5Bit(0xc0, 0xd8, 0xff), //colorEnd (colorMiddle is in the main info)
		0x00, //colorTrans1Start
		0x54, //colorTrans2Start
		0xc1, //colorTrans2End
		0x00,
		0x04, //interpFlags
		0x00,
		0x0000
	};
	
	Particle::Drift particleDriftEffect
	{
		Vector3_16f{0x0000_fs, -0x0333_fs, 0x0000_fs}
	};
	Particle::Effect particleEffects[]
	{
		Particle::Effect{
							&Particle::Drift::Func,
							&particleDriftEffect
						}
	};
	
	Particle::SysDef particleSysDef
	{
		&particleInfo,
		&particleScaleTrans,
		&particleColorTrans,
		nullptr,
		nullptr,
		nullptr,
		&particleEffects[0],
		1
	};
}

SharedFilePtr YBG_Sprinkler::modelFile;
SharedFilePtr YBG_Sprinkler::clsnFile;

SpawnInfo<YBG_Sprinkler> YBG_Sprinkler::spawnData =
{
	&YBG_Sprinkler::Spawn,
	0x0034,
	0x00cd,
	ACTOR_FLAGS,
	CLSN_RANGE_OFFSET_Y,
	CLSN_RANGE,
	DRAW_DIST,
	0x01000000_f, //???
};

YBG_Sprinkler* YBG_Sprinkler::Spawn()
{
	return new YBG_Sprinkler;
}

int YBG_Sprinkler::InitResources()
{
	char* modelF = Model::LoadFile(modelFile);
	model.SetFile(modelF, 1, -1);
	
	UpdateModelPosAndRotY();
	UpdateClsnPosAndRot();
	
	char* clsnF = MovingMeshCollider::LoadFile(clsnFile);
	clsn.SetFile(clsnF, clsnNextMat, CLSN_SCALE, ang.y, clpsBlock);
	clsn.beforeClsnCallback = (decltype(clsn.beforeClsnCallback))0x02039348;
	eventID = param1 & 0xff;
	
	return 1;
}

int YBG_Sprinkler::CleanupResources()
{
	if(clsn.IsEnabled())
	{
		clsn.Disable();
	}
	modelFile.Release();
	clsnFile.Release();
	return 1;
}

int YBG_Sprinkler::Behavior()
{
	bool triggered = eventID >= 0x20 || Event::GetBit(eventID);
	if(triggered)
	{
		ang.y += ROT_SPEED;
		
		constexpr Fix12s cosVertAng = 0x0b50_fs;
		constexpr Fix12s sinVertAng = 0x0b50_fs; //the angle is 0x2000, or 1/8 tau.
		Vector3_16f particleDir = {(Fix12s)Sin(ang.y) * cosVertAng,
								   sinVertAng,
								   (Fix12s)Cos(ang.y) * cosVertAng};
		particleSysID = Particle::System::New(particleSysID,
											  (unsigned)&particleSysDef,
											  pos.x,
											  pos.y + 0x50000_f,
											  pos.z,
											  &particleDir,
											  nullptr);
												
		UpdateModelPosAndRotY();
	}
	if(IsClsnInRange(0_f, 0_f) /*<-- has side effects*/ && triggered)
		UpdateClsnPosAndRot();
	
	return 1;
}

int YBG_Sprinkler::Render()
{
	model.Render(nullptr);
	return 1;
}

YBG_Sprinkler::~YBG_Sprinkler() {}