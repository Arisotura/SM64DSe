#include "FallingRock.h"

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
	
	Particle::MainInfo particleInfo
	{
		(Particle::MainInfo::Flags)0x010040c5, //flags
		0x00000800_f, //rate, fix20.12
		0x00025800_f, //startHorzDist, fix23.9
		Vector3_16f{0x0000_fs, 0x1000_fs, 0x0000_fs}, //dir
		Color5Bit(0x20, 0x20, 0x20), //color
		0x00003000_f, //horzSpeed, fix20.12 (fix23.9???)
		0x00006000_f, //vertSpeed, fix20.12 (fix23.9???)
		0x00008000_f, //scale, fix20.12
		0x1000_fs, //horzScale, fix4.12
		0x0000,
		0x0000,
		0x0000,
		0x0000, //frames
		0x0040, //lifetime
		0xff, //scaleRand
		0x00, //lifetimeRand
		0xff, //speedRand
		0x00,
		0x01, //spawnPeriod
		0x1f, //alpha
		0x80, //speedFalloff
		0x27, //spriteID
		0x01,
		0x00,
		0x00, //velStretchFactor
		0x00 //texMirrorFlags
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
		nullptr,
		nullptr,
		nullptr,
		nullptr,
		nullptr,
		&particleEffects[0],
		1
	};
	
	constexpr Fix12i MIN_SCALE = 0x080000_f;
	constexpr Fix12i MAX_SCALE = 0x100000_f;
	constexpr Fix12i MIN_VERT_INIT_VEL = 0x60000_f;
	constexpr Fix12i MAX_HORZ_SPEED = 0x60000_f;
	constexpr Fix12i MAX_VERT_INIT_VEL = 0x80000_f;
	constexpr Fix12i VERT_ACCEL = -0x2000_f;
	constexpr Fix12i TERM_VEL = -0x64000_f;
	constexpr short ROT_SPEED = -0x200;
}

//START SPAWNER

SpawnInfo<YBG_FallingRock::Spawner> YBG_FallingRock::Spawner::spawnData =
{
	&YBG_FallingRock::Spawner::Spawn,
	0x016f,
	0x016f,
	0x00000000,
	0x00064000_f,
	0x0015e000_f,
	0x08000000_f,
	0x08000000_f
};

YBG_FallingRock::Spawner* YBG_FallingRock::Spawner::Spawn()
{
	return new YBG_FallingRock::Spawner;
}

int YBG_FallingRock::Spawner::InitResources()
{
	Model::LoadFile(modelFile);
	MovingMeshCollider::LoadFile(clsnFile);
	eventID = param1 & 0xff;
	return 1;
}

int YBG_FallingRock::Spawner::CleanupResources()
{
	modelFile.Release();
	clsnFile.Release();
	return 1;
}

int YBG_FallingRock::Spawner::Behavior()
{
	particleSysID = Particle::System::New(particleSysID, (unsigned)&particleSysDef, pos.x, pos.y, pos.z, nullptr, nullptr);
	if(RandomInt() >> 25 == 0)
	{
		Fix12i vertInitVel = Fix12i(RandomInt() / int(0xffffffff / (MAX_VERT_INIT_VEL.val - MIN_VERT_INIT_VEL.val)), true) + MIN_VERT_INIT_VEL;
		Fix12i theHorzSpeed = Fix12i(RandomInt() / int(0xffffffff / MAX_HORZ_SPEED.val), true) * vertInitVel / MAX_VERT_INIT_VEL;
		Fix12i distFromCenter = 0x258000_f * theHorzSpeed / vertInitVel; //will not overflow
		Fix12i scale = Fix12i(RandomInt() / int(0xffffffff / (MAX_SCALE.val - MIN_SCALE.val)), true) + MIN_SCALE;
		short angle = RandomInt() >> 16;
		
		YBG_FallingRock* rock = (YBG_FallingRock*)Actor::Spawn(0x0030, (int)scale, Vector3{pos.x + distFromCenter * Sin(angle),
																					   pos.y - 0x100000_f,
																					   pos.z + distFromCenter * Cos(angle)},
															   nullptr, areaID, -1);
		if(rock)
		{
			Sound::PlayBank3(0x10c, camSpacePos);
			rock->horzSpeed = theHorzSpeed;
			rock->ang.y = rock->motionAng.y = angle;
			rock->speed.y = vertInitVel;
		}
	}
	
	if(eventID < 0x20 && Event::GetBit(eventID))
		Destroy();
	return 1;
}

int YBG_FallingRock::Spawner::Render()
{
	return 1;
}

YBG_FallingRock::Spawner::~Spawner() {}

//END SPAWNER

SharedFilePtr YBG_FallingRock::modelFile;
SharedFilePtr YBG_FallingRock::clsnFile;

SpawnInfo<YBG_FallingRock> YBG_FallingRock::spawnData =
{
	&YBG_FallingRock::Spawn,
	0x0030,
	0x0011,
	0x00000002,
	0x00064000_f,
	0x0015e000_f,
	0x02000000_f,
	0x02000000_f
};

YBG_FallingRock* YBG_FallingRock::Spawn()
{
	return new YBG_FallingRock;
}

void YBG_FallingRock::UpdateModelTransform()
{
	model.mat4x3 = Matrix4x3::IDENTITY;
	model.mat4x3.r0c0 = model.mat4x3.r1c1 = model.mat4x3.r2c2 = scale.x >> 8;
	model.mat4x3 = Matrix4x3::FromRotationXYZExt(ang.x, ang.y, ang.z) * model.mat4x3;
	model.mat4x3.r0c3 = pos.x >> 3;
	model.mat4x3.r1c3 = pos.y >> 3;
	model.mat4x3.r2c3 = pos.z >> 3;
	
	shadowMat = Matrix4x3::IDENTITY;
	*(Vector3*)&shadowMat.r0c3 = *(Vector3*)&model.mat4x3.r0c3;
	DropShadowRadHeight(shadow, shadowMat, scale.x, 0x1f40000_f, 0xf);
}

int YBG_FallingRock::InitResources()
{
	scale.x = Fix12i(param1);
	
	char* modelF = Model::LoadFile(modelFile);
	model.SetFile(modelF, 1, -1);
	
	UpdateModelTransform();
	UpdateClsnPosAndRot();
	
	char* clsnF = MovingMeshCollider::LoadFile(clsnFile);
	clsn.SetFile(clsnF, clsnNextMat, 0x1000_f, ang.y, clpsBlock);
	
	wmClsn.Init(this, scale.x, 0_f, nullptr, nullptr);
	shadow.InitCylinder();
	
	vertAccel = VERT_ACCEL;
	termVel = TERM_VEL;
	
	return 1;
}

int YBG_FallingRock::CleanupResources()
{
	clsn.Disable();
	modelFile.Release();
	clsnFile.Release();
	return 1;
}

int YBG_FallingRock::Behavior()
{
	ang.x += ROT_SPEED;
	UpdatePos(nullptr);
	
	if(speed.y >= 0_f)
		wmClsn.UpdateDiscreteNoLava();
	else
	{
		wmClsn.UpdateContinuous();
		if((wmClsn.sphere.resultFlags & SphereClsn::COLLISION_EXISTS))
			Kill();
	}
		
	
	UpdateModelTransform();
	if(IsClsnInRange(0_f, 0_f))
	{
		UpdateClsnPosAndRot();
	}
	
	return 1;
}

int YBG_FallingRock::Render()
{
	model.Render(nullptr);
	return 1;
}

YBG_FallingRock::~YBG_FallingRock() {}

void YBG_FallingRock::Kill()
{
	Particle::System::NewSimple(0x48, pos.x, pos.y, pos.z);
	PoofDustAt(pos);
	Sound::PlayBank3(0x41, camSpacePos);
	Destroy();
}
