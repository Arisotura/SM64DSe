#include "BBL_Tower.h"
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
}

SharedFilePtr BBL_Tower::modelFile;
SharedFilePtr BBL_Tower::clsnFile;

SpawnInfo<BBL_Tower> BBL_Tower::spawnData =
{
	&BBL_Tower::Spawn,
	0x0031,
	0x00cc,
	0x00000002,
	0x00030000_f,
	0x00180000_f,
	0x08000000_f,
	0x08000000_f,
};

Actor* BBL_Tower::searchByPos(const Vector3& posToFind)
{
	Actor* actor = FIRST_ACTOR_LIST_NODE->actor;
	
	while(actor)
	{
		if(actor->pos == posToFind)
			break;
		actor = actor->Next();
	}
	
	return actor;
}

BBL_Tower* BBL_Tower::Spawn()
{
	return new BBL_Tower;
}

int BBL_Tower::InitResources()
{
	char* modelF = Model::LoadFile(modelFile);
	model.SetFile(modelF, 1, -1);
	
	UpdateModelPosAndRotY();
	UpdateClsnPosAndRot();
	
	char* clsnF = MovingMeshCollider::LoadFile(clsnFile);
	clsn.SetFile(clsnF, clsnNextMat, 0x1000_f, ang.y, clpsBlock);
	
	clsn.beforeClsnCallback = (decltype(clsn.beforeClsnCallback))0x02039348;
	
	maxPosY = pos.y + 0x3c0000_f;
	moveState = 0;
	
	drawDistAsr3 = 0x1000000_f; //remember, Q23.9, also treat this like part of the level's main mesh
	rangeAsr3 = 0x400000_f; //remember, Q23.9
	rangeOffsetY = 0x5a0000_f;
	
	triggerPos.x = Fix12i((short)param1) + pos.x;
	triggerPos.y = Fix12i(ang.x)         + pos.y;
	triggerPos.z = Fix12i(ang.z)         + pos.z;
	
	ang.x = 0;
	ang.z = 0;
	
	return 1;
}

int BBL_Tower::CleanupResources()
{
	if(clsn.IsEnabled())
	{
		clsn.Disable();
	}
	modelFile.Release();
	clsnFile.Release();
	return 1;
}

int BBL_Tower::Behavior()
{
	
	if(moveState == 0)
	{
		if(!trigger)
			trigger = searchByPos(triggerPos);
		
		if(trigger->aliveState == Actor::DEAD)
		{
			moveState = 1;
			speed.y = 0xc000_f;
			towerStartedMoving = true;
		}
	}
	if(moveState == 1)
	{
		UpdatePos(nullptr);
		if(pos.y >= maxPosY)
		{
			pos.y = maxPosY;
			moveState = 2;
			speed.y = 0_f;
		}
		UpdateModelPosAndRotY();
		if(IsClsnInRange(0_f, 0_f))
		{
			UpdateClsnPosAndRot();
		}
		
		//*(int*)((char*)this + 0x38c) = Sound_Play2(*(int*)((char*)this + 0x38c), 3, 0x82, &camSpacePos, 0);
	}
	else if(moveState == 2)
	{
		IsClsnInRange(0_f, 0_f);
		UpdateClsnPosAndRot();
		moveState = 3;
	}
	else
	{
		IsClsnInRange(0_f, 0_f); //remember, this has side effects!
	}
	
	
	clsn.range = 0x2000000_f;
	clsn.rangeOffsetY = 0x5a0000_f;
	
	return 1;
}

int BBL_Tower::Render()
{
	model.Render(nullptr);
	return 1;
}

BBL_Tower::~BBL_Tower() {}