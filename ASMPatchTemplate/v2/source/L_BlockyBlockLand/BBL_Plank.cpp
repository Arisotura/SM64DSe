#include "BBL_Plank.h"
#include "BlockyBlockSpecifics.h"

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
	
	constexpr Fix12i PIVOT_DIST = 0x00c37000_f;
}

SharedFilePtr BBL_Plank::modelFile;
SharedFilePtr BBL_Plank::clsnFile;

SpawnInfo<BBL_Plank> BBL_Plank::spawnData =
{
	&BBL_Plank::Spawn,
	0x0032,
	0x00cd,
	0x00000002,
	0x00030000_f,
	0x00180000_f,
	0x08000000_f,
	0x08000000_f,
};

void BBL_Plank::UpdateModelTransform()
{
	model.mat4x3.ThisFromRotationXYZExt(0, ang.y, ang.z);
	model.mat4x3.r0c3 = pos.x >> 3;
	model.mat4x3.r1c3 = pos.y >> 3;
	model.mat4x3.r2c3 = pos.z >> 3;
}

void BBL_Plank::UpdateClsnTransform()
{
	clsnNextMat = model.mat4x3;
	clsnNextMat.r0c3 = pos.x;
	clsnNextMat.r1c3 = pos.y;
	clsnNextMat.r2c3 = pos.z;
	
	clsn.Transform(clsnNextMat, ang.y);
}

BBL_Plank* BBL_Plank::Spawn()
{
	return new BBL_Plank;
}

int BBL_Plank::InitResources()
{
	char* modelF = Model::LoadFile(modelFile);
	model.SetFile(modelF, 1, -1);
	
	UpdateModelTransform();
	UpdateClsnTransform();
	
	char* clsnF = MovingMeshCollider::LoadFile(clsnFile);
	clsn.SetFile(clsnF, clsnNextMat, 0x199_f, ang.y, clpsBlock);
	
	clsn.beforeClsnCallback = (decltype(clsn.beforeClsnCallback))0x02039348;
	
	moveState = 0;
	offsetY = 0_f;
	maxOffsetY = 0x3c0000_f;
	
	drawDistAsr3 = 0x1000000_f; //remember, Q23.9, also treat this like part of the level's main mesh
	rangeAsr3 = 0x400000_f; //remember, Q23.9
	rangeOffsetY = 0x5a0000_f;
	
	return 1;
}

int BBL_Plank::CleanupResources()
{
	if(clsn.IsEnabled())
		clsn.Disable();
	
	clsnFile.Release();
	modelFile.Release();
	
	return 1;
}

//0219007c is the experimental one
//0211d028 is the vtable
int BBL_Plank::Behavior()
{
	if(moveState == 0 && towerStartedMoving)
	{
		moveState = 1;
	}
	if(moveState == 1)
	{
		offsetY += 0xc000_f;
		if(offsetY >= maxOffsetY)
		{
			offsetY = maxOffsetY;
			moveState = 2;
		}
		ang.z = -Atan2(offsetY, PIVOT_DIST);
		
		UpdateModelTransform();
		if(IsClsnInRange(0_f, 0_f))
		{
			UpdateClsnTransform();
		}
		
		//*(int*)((char*)this + 0x38c) = Sound_Play2(*(int*)((char*)this + 0x38c), 3, 0x82, &camSpacePos, 0);
	}
	else if(moveState == 2)
	{
		IsClsnInRange(0_f, 0_f);
		UpdateClsnTransform();
		moveState = 3;
	}
	else
	{
		IsClsnInRange(0_f, 0_f); //remember, this has side effects!
	}
	
	return 1;
}

int BBL_Plank::Render()
{
	model.Render(nullptr);
	return 1;
}

BBL_Plank::~BBL_Plank() {}