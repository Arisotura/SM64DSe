#include "SewerClog.h"

namespace
{
	FixedSizeCLPS_Block<1> clpsBlock =
	{
		{'C', 'L', 'P', 'S'},
		8,
		1,
		{
        	CLPS(0x04, 0, 0x3f, 0x0, 0x0, 0x00, 0, 0, 0, 0xff)
        }
	};
	
	Particle::MainInfo particleInfo
	{
		(Particle::MainInfo::Flags)0x010a7325, //flags
		0x00004000_f, //rate, fix20.12
		0x0003e800_f, //startHorzDist, fix23.9
		Vector3_16f{0x0000_fs, -0x1000_fs, 0x0000_fs}, //dir
		Color5Bit(0x20, 0x20, 0x28), //color
		0x00000000_f, //horzSpeed, fix20.12 (fix23.9???)
		0x000049c1_f, //vertSpeed, fix20.12 (fix23.9???)
		0x00008000_f, //scale, fix20.12
		0x1000_fs, //horzScale, fix4.12
		0x6574,
		(short)0xec7d, //minAngSpeed
		0x1569, //maxAngSpeed
		0x0006, //frames
		0x0020, //lifetime
		0x6d, //scaleRand
		0x66, //lifetimeRand
		0x64, //speedRand
		0x00,
		0x01, //spawnPeriod
		0x1f, //alpha
		0x80, //speedFalloff
		0x24, //spriteID
		0x0c, //effectLength ???
		0x00,
		0x00, //velStretchFactor
		0x00  //texMirrorFlags
	};
	
	Particle::ScaleTransition particleScaleTrans
	{
		0x1000_fs, //scaleStart, fix4.12
		0x1000_fs, //scaleMiddle, fix4.12
		0x0199_fs, //scaleEnd, fix4.12
		0x00, //scaleTrans1End
		0xbf, //scaleTrans2Start;
		0x7364,
		0x725f
	};
	
	Particle::ColorTransition particleColorTrans
	{
		Color5Bit(0x60, 0x60, 0x60), //colorStart
		Color5Bit(0x40, 0x40, 0x40), //colorEnd (colorMiddle is in the main info)
		0x37, //colorTrans1Start
		0x80, //colorTrans2Start
		0xca, //colorTrans2End
		0x61,
		0x06, //interpFlags
		0x00,
		0x0000
	};
	
	Particle::SysDef particleSysDef
	{
		&particleInfo,
		&particleScaleTrans,
		&particleColorTrans,
		nullptr,
		nullptr,
		nullptr,
		nullptr,
		0
	};
}

SharedFilePtr YBG_SewerClog::modelFile;
SharedFilePtr YBG_SewerClog::clsnFile;

SpawnInfo<YBG_SewerClog> YBG_SewerClog::spawnData =
{
	&YBG_SewerClog::Spawn,
	0x0033,
	0x0011,
	0x00000002,
	0x00000000_f,
	0x002ee000_f,
	0x01000000_f,
	0x01000000_f
};

YBG_SewerClog* YBG_SewerClog::Spawn()
{
	return new YBG_SewerClog;
}

void YBG_SewerClog::UpdateModelTransform()
{
	model.mat4x3.ThisFromRotationY(ang.y);
	model.mat4x3.r0c3 = pos.x >> 3;
	model.mat4x3.r1c3 = pos.y >> 3;
	model.mat4x3.r2c3 = pos.z >> 3;
}


int YBG_SewerClog::InitResources()
{
	char* modelF = Model::LoadFile(modelFile);
	model.SetFile(modelF, 1, -1);
	
	UpdateModelTransform();
	UpdateClsnPosAndRot();
	
	char* clsnF = MovingMeshCollider::LoadFile(clsnFile);
	clsn.SetFile(clsnF, clsnNextMat, 0x1000_f, ang.y, clpsBlock);
	
	eventID = param1 & 0xff;
	return 1;
}

int YBG_SewerClog::CleanupResources()
{
	clsn.Disable();
	modelFile.Release();
	clsnFile.Release();
	return 1;
}

int YBG_SewerClog::Behavior()
{
	IsClsnInRange(0_f, 0_f);
	if(Event::GetBit(eventID))
	{
		DisappearPoofDustAt(pos);
		particleInfo.dir = {(Fix12s)Sin(ang.y),
							0_fs,
							(Fix12s)Cos(ang.y)};
		Particle::System::NewSimple((unsigned)&particleSysDef,
								    pos.x,
								    pos.y,
								    pos.z);
		Sound::PlayBank3(0x41, camSpacePos);
		Destroy();
	}
	
	return 1;
}

int YBG_SewerClog::Render()
{
	model.Render(nullptr);
	return 1;
}

YBG_SewerClog::~YBG_SewerClog() {}
