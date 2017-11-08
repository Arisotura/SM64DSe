#include "SewerBlock.h"

namespace
{
	FixedSizeCLPS_Block<1> clpsBlock =
	{
		{'C', 'L', 'P', 'S'},
		8,
		1,
		{
        	CLPS(0x0c, 0, 0x3f, 0x0, 0x0, 0x00, 0, 0, 0, 0xff)
        }
	};
}

SharedFilePtr YBG_SewerBlock::modelFile;
SharedFilePtr YBG_SewerBlock::clsnFile;

SpawnInfo<YBG_SewerBlock> YBG_SewerBlock::spawnData =
{
	&YBG_SewerBlock::Spawn,
	0x0034,
	0x0011,
	0x00000002,
	0x00000000_f,
	0x00400000_f,
	0x01000000_f,
	0x01000000_f
};

YBG_SewerBlock* YBG_SewerBlock::Spawn()
{
	return new YBG_SewerBlock;
}

void YBG_SewerBlock::UpdateModelTransform()
{
	model.mat4x3.ThisFromRotationY(ang.y);
	model.mat4x3.r0c3 = pos.x >> 3;
	model.mat4x3.r1c3 = pos.y >> 3;
	model.mat4x3.r2c3 = pos.z >> 3;
}


int YBG_SewerBlock::InitResources()
{
	char* modelF = Model::LoadFile(modelFile);
	model.SetFile(modelF, 1, -1);
	
	UpdateModelTransform();
	UpdateClsnPosAndRot();
	
	char* clsnF = MovingMeshCollider::LoadFile(clsnFile);
	clsn.SetFile(clsnF, clsnNextMat, 0x1000_f, ang.y, clpsBlock);
	
	return 1;
}

int YBG_SewerBlock::CleanupResources()
{
	clsn.Disable();
	modelFile.Release();
	clsnFile.Release();
	return 1;
}

int YBG_SewerBlock::Behavior()
{
	IsClsnInRange(0_f, 0_f);
	
	return 1;
}

int YBG_SewerBlock::Render()
{
	model.Render(nullptr);
	return 1;
}

YBG_SewerBlock::~YBG_SewerBlock() {}
