#include "BBL_Tower.h"
#include "BBL_Plank.h"
#include "BlockyBlockSpecifics.h"

bool towerStartedMoving = false;
namespace
{
	bool searched = false;
	bool blackBricksGone = false;
	
	unsigned lastBehavInst;
	unsigned lastCleanupInst;

	struct EnemyInfo
	{
		uint16_t killID;
		Vector3 pos;
		Actor* enemy;
	};

	EnemyInfo goombaInfos[5] = {
		EnemyInfo{0xffff, Vector3{0_f, 0_f, 0_f}, nullptr},
		EnemyInfo{0xffff, Vector3{0_f, 0_f, 0_f}, nullptr},
		EnemyInfo{0xffff, Vector3{0_f, 0_f, 0_f}, nullptr},
		EnemyInfo{0xffff, Vector3{0_f, 0_f, 0_f}, nullptr},
		EnemyInfo{0xffff, Vector3{0_f, 0_f, 0_f}, nullptr},
	};
	int goombaCount = 0;
}

SpawnInfo<BBL_Specifics> BBL_Specifics::spawnData =
{
	&BBL_Specifics::Spawn,
	0x0004,
	0x0007,
	0x00000000,
	0x00000000_f,
	0x7fffffff_f,
	0x7fffffff_f,
	0x7fffffff_f
};

void BBL_Specifics::SearchForStarGoombas()
{
	Actor* actor = Actor::FindWithActorID(0x00ca, FIRST_ACTOR_LIST_NODE->actor);
	
	while(actor)
	{
		if(actor->pos.y >= 0x009c4000_f)
		{
			goombaInfos[goombaCount].killID = actor->deathTableID;
			goombaInfos[goombaCount].pos = actor->pos;
			goombaInfos[goombaCount].enemy = actor;
			++goombaCount;
			if(goombaCount == 5)
				break;
		}
		
		actor = Actor::FindWithActorID(0x00ca, actor);
	}
}

Actor* BBL_Specifics::SearchForBlackBrick()
{
	return Actor::FindWithActorID(0x0011, FIRST_ACTOR_LIST_NODE->actor);
}

BBL_Specifics* BBL_Specifics::Spawn()
{
	return new BBL_Specifics;
}

int BBL_Specifics::InitResources()
{
	return 1;
}

int BBL_Specifics::Behavior()
{
	if(!searched)
	{
		SearchForStarGoombas();
		searched = true;
	}
		
	
	for(int i = 0; i < 5; ++i) if(goombaInfos[i].killID != 0xffff)
	{
		if(DeathTable_GetBit(goombaInfos[i].killID))
		{
			goombaInfos[i].killID = 0xffff;
			--goombaCount;
			Actor::Spawn(0x014a, 5 - goombaCount, goombaInfos[i].pos + Vector3{0_f, 0x60000_f, 0_f}, nullptr, 0, -1);
			if(goombaCount == 0)
				Actor::Spawn(0x00b2, 0x0041, goombaInfos[i].pos, nullptr, 0, -1);
		}
		else
			goombaInfos[i].pos = goombaInfos[i].enemy->pos;
	}
	
	if(!blackBricksGone && !SearchForBlackBrick())
	{
		blackBricksGone = true;
		Event::SetBit(2);
	}
	
	return 1;
}

int BBL_Specifics::Render()
{
	return 1;
}

int BBL_Specifics::CleanupResources()
{
	return 1;
}

BBL_Specifics::~BBL_Specifics() {}

void init()
{
	OBJ_TO_ACTOR_ID_TABLE[0x0150] = 0x0172;
	ACTOR_SPAWN_TABLE[0x0172] = (unsigned)&BBL_Specifics::spawnData;
	
	OBJ_TO_ACTOR_ID_TABLE[0x00cb] = 0x0031;
	ACTOR_SPAWN_TABLE[0x0031] = (unsigned)&BBL_Tower::spawnData;
	BBL_Tower::modelFile.Construct(0x059d);
	BBL_Tower::clsnFile .Construct(0x059e);
	
	OBJ_TO_ACTOR_ID_TABLE[0x00cc] = 0x0032;
	ACTOR_SPAWN_TABLE[0x0032] = (unsigned)&BBL_Plank::spawnData;
	BBL_Plank::modelFile.Construct(0x05a1);
	BBL_Plank::clsnFile .Construct(0x05a2);
}