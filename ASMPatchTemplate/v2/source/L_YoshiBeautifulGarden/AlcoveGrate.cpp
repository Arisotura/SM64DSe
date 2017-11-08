#include "AlcoveGrate.h"

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
	
	constexpr Fix12i DROP_SPEED =  0x8000_f;
	constexpr Fix12i DROP_DIST = 0x1f4000_f;
}

SharedFilePtr YBG_AlcoveGrate::modelFile;
SharedFilePtr YBG_AlcoveGrate::clsnFile;

SpawnInfo<YBG_AlcoveGrate> YBG_AlcoveGrate::spawnData =
{
	&YBG_AlcoveGrate::Spawn,
	0x0035,
	0x0011,
	0x00000002,
	0x000c8000_f,
	0x003e8000_f,
	0x01000000_f,
	0x01000000_f
};

YBG_AlcoveGrate* YBG_AlcoveGrate::Spawn()
{
	return new YBG_AlcoveGrate;
}

void YBG_AlcoveGrate::UpdateModelTransform()
{
	model.mat4x3.ThisFromRotationY(ang.y);
	model.mat4x3.r0c3 = pos.x >> 3;
	model.mat4x3.r1c3 = pos.y >> 3;
	model.mat4x3.r2c3 = pos.z >> 3;
}


int YBG_AlcoveGrate::InitResources()
{
	char* modelF = Model::LoadFile(modelFile);
	model.SetFile(modelF, 1, -1);
	
	UpdateModelTransform();
	UpdateClsnPosAndRot();
	
	char* clsnF = MovingMeshCollider::LoadFile(clsnFile);
	clsn.SetFile(clsnF, clsnNextMat, 0x1000_f, ang.y, clpsBlock);
	clsn.beforeClsnCallback = (decltype(clsn.beforeClsnCallback))0x02039348;
	eventID = param1 & 0xff;
	
	return 1;
}

int YBG_AlcoveGrate::CleanupResources()
{
	clsn.Disable();
	modelFile.Release();
	clsnFile.Release();
	return 1;
}

int YBG_AlcoveGrate::Behavior()
{
	if(eventID < 0x20 && Event::GetBit(eventID))
		speed.y = -DROP_SPEED;
	UpdatePosWithOnlySpeed(nullptr);
	if(origPosY - pos.y >= DROP_DIST)
	{
		Destroy();
		return 1;
	}
	UpdateModelTransform();
	if(IsClsnInRange(0_f, 0_f))
	{
		UpdateClsnPosAndRot();
	}
	
	return 1;
}

int YBG_AlcoveGrate::Render()
{
	model.Render(nullptr);
	return 1;
}

YBG_AlcoveGrate::~YBG_AlcoveGrate() {}
