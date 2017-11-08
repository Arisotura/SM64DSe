#include "CorkRock.h"

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
	
	constexpr Fix12i FINAL_POS_Y = 0x1644000_f;
	constexpr Fix12i VERT_ACCEL = -0x2000_f;
}

SharedFilePtr YBG_CorkRock::modelFile;
SharedFilePtr YBG_CorkRock::clsnFile;

SpawnInfo<YBG_CorkRock> YBG_CorkRock::spawnData =
{
	&YBG_CorkRock::Spawn,
	0x002f,
	0x0011,
	0x00000000,
	0x00384000_f,
	0x00dac000_f,
	0x08000000_f,
	0x08000000_f
};

YBG_CorkRock* YBG_CorkRock::Spawn()
{
	return new YBG_CorkRock;
}

void YBG_CorkRock::UpdateModelTransform()
{
	model.mat4x3.ThisFromRotationY(ang.y);
	model.mat4x3.r0c3 = pos.x >> 3;
	model.mat4x3.r1c3 = pos.y >> 3;
	model.mat4x3.r2c3 = pos.z >> 3;
	shadowMat = model.mat4x3;
	shadowMat.r1c3 += 0x514000_f >> 3;
	DropShadowRadHeight(shadow, shadowMat, 0xb1c1eb_f, 0x1f40000_f, 0xf);
}

void YBG_CorkRock::UpdateClsnTransform()
{
	clsnNextMat.ThisFromRotationY(ang.y);
	clsnNextMat.r0c3 = pos.x;
	clsnNextMat.r1c3 = pos.y;
	clsnNextMat.r2c3 = pos.z;
	
	clsn.Transform(clsnNextMat, ang.y);
}

int YBG_CorkRock::InitResources()
{
	if(STAR_ID == 1)
		return 0;
	if(STAR_ID > 2)
		pos.y = FINAL_POS_Y;
	
	char* modelF = Model::LoadFile(modelFile);
	model.SetFile(modelF, 1, -1);
	
	UpdateModelTransform();
	UpdateClsnPosAndRot();
	
	char* clsnF = MovingMeshCollider::LoadFile(clsnFile);
	clsn.SetFile(clsnF, clsnNextMat, 0x1000_f, ang.y, clpsBlock);
	clsn.beforeClsnCallback = (decltype(clsn.beforeClsnCallback))0x02039348;
	
	shadow.InitCylinder();
	
	vertAccel = VERT_ACCEL;
	termVel = 0x80000000_f;
	
	eventIDs[0] = param1 >> 0 & 0xff;
	eventIDs[1] = param1 >> 8 & 0xff;
	triggered = STAR_ID > 2;
	startedTrigger = false;
	
	return 1;
}

int YBG_CorkRock::CleanupResources()
{
	clsn.Disable();
	modelFile.Release();
	clsnFile.Release();
	return 1;
}

int YBG_CorkRock::Behavior()
{
	if(STAR_ID == 2)
	{
		if(pos.y <= FINAL_POS_Y)
		{
			pos.y = FINAL_POS_Y;
			if(eventIDs[1] < 0x20 && !triggered)
			{
				Event::SetBit(eventIDs[1]);
				Earthquake(pos, 0x0c000000_f);
				triggered = true;
			}
		}
		else if(eventIDs[0] < 0x20 && Event::GetBit(eventIDs[0]))
		{
			UpdatePos(nullptr);
			if(!startedTrigger)
			{
				Actor::Spawn(0x0167, 0x0000, pos, nullptr, areaID, -1);
				startedTrigger = true;
			}
		}
	}
	
	UpdateModelTransform();
	if(!clsn.IsEnabled())
		clsn.Enable();
	UpdateClsnPosAndRot();
	
	return 1;
}

int YBG_CorkRock::Render()
{
	model.Render(nullptr);
	return 1;
}

YBG_CorkRock::~YBG_CorkRock() {}
