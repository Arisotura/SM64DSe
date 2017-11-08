#include "Test.h"

namespace
{
	FixedSizeCLPS_Block<1> clpsBlock =
	{
		{'C', 'L', 'P', 'S'},
		8,
		1,
		{
        	CLPS(0x00, 0, 0x3f, 0x0, 0x0, 0x00, 0, 1, 0, 0xff)
        }
	};
}

SharedFilePtr Test::modelFile;
SharedFilePtr Test::clsnFile;

SpawnInfo<Test> Test::spawnData =
{
	&Test::Spawn,
	0x0000,
	0x0100,
	0x00000002,
	0x003e8000_f,
	0x007d0000_f,
	0xe8000000_f,
	0xe8000000_f
};

Test* Test::Spawn()
{
	return new Test;
}

void Test::UpdateModelTransform()
{
	model.mat4x3.ThisFromRotationY(ang.y);
	model.mat4x3.r0c3 = pos.x >> 3;
	model.mat4x3.r1c3 = pos.y >> 3;
	model.mat4x3.r2c3 = pos.z >> 3;
}


int Test::InitResources()
{
	char* modelF = Model::LoadFile(modelFile);
	model.SetFile(modelF, 1, -1);
	
	UpdateModelTransform();
	UpdateClsnPosAndRot();
	
	char* clsnF = MovingMeshCollider::LoadFile(clsnFile);
	clsn.SetFile(clsnF, clsnNextMat, 0xa000_f, ang.y, clpsBlock);
	
	clsn.beforeClsnCallback = (decltype(clsn.beforeClsnCallback))0x02039348;
	
	return 1;
}

int Test::CleanupResources()
{
	clsn.Disable();
	modelFile.Release();
	clsnFile.Release();
	return 1;
}

int Test::Behavior()
{
	IsClsnInRange(0_f, 0_f);
	
	return 1;
}

int Test::Render()
{
	model.Render(nullptr);
	return 1;
}

Test::~Test() {}

void init()
{
	OBJ_TO_ACTOR_ID_TABLE[0x0000] = 0x0000;
	ACTOR_SPAWN_TABLE[0x0000] = (unsigned)&Test::spawnData;
	Test::modelFile.Construct(0x0354);
    Test::clsnFile .Construct(0x0355);
}

void cleanup()
{
	
	
}
