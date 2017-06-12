#include "MegaBlock.h"
#include "KnockDownPlank.h"
#include "BlockyBlock2Specifics.h"

namespace
{
	bool searched = false;

	struct GoombaInfo
	{
		uint16_t killID;
		Vector3 pos;
		Actor* goomba;
	};

	GoombaInfo goombaInfos[4] = {
		GoombaInfo{0xffff, Vector3{0_f, 0_f, 0_f}, nullptr},
		GoombaInfo{0xffff, Vector3{0_f, 0_f, 0_f}, nullptr},
		GoombaInfo{0xffff, Vector3{0_f, 0_f, 0_f}, nullptr},
		GoombaInfo{0xffff, Vector3{0_f, 0_f, 0_f}, nullptr},
	};
	int count;
	
	unsigned prevMinimapSizeInst;
}

SpawnInfo<BBL2_Specifics> BBL2_Specifics::spawnData =
{
	&BBL2_Specifics::Spawn,
	0x0004,
	0x0007,
	0x00000000,
	0x00000000_f,
	0x7fffffff_f,
	0x7fffffff_f,
	0x7fffffff_f
};

void BBL2_Specifics::SearchForGoombas()
{
	Actor* actor = Actor::FindWithActorID(0x00ca, FIRST_ACTOR_LIST_NODE->actor);
	Vector3 battleCenter = {-0x5c94000_f, 0x00000000_f, -0x2a30000_f};
	
	while(actor)
	{
		if(actor->pos.Dist(battleCenter) < 0x8fc000_f)
		{
			goombaInfos[count].killID = actor->deathTableID;
			goombaInfos[count].pos = actor->pos;
			goombaInfos[count].goomba = actor;
			++count;
			if(count == 4)
				break;
		}
		
		actor = Actor::FindWithActorID(0x00ca, actor);
	}
}

Minimap* BBL2_Specifics::GetMinimap()
{
	ActorBase::SceneNode* node = ROOT_ACTOR_BASE->sceneNode.firstChild;
	while(node)
	{
		ActorBase* actor = node->actor;
		if(actor->actorID == 0x014f)
			return (Minimap*)actor;
			
		node = node->nextSibling;
	}
	
	return nullptr;
}

BBL2_Specifics* BBL2_Specifics::Spawn()
{
	return new BBL2_Specifics;
}

int BBL2_Specifics::InitResources()
{
	return 1;
}

int BBL2_Specifics::Behavior()
{
	if(!searched)
	{
		SearchForGoombas();
		Minimap* minimap = GetMinimap();
		minimap->targetInvScale = 0x500_f;
		minimap->arrowType = Minimap::AR_ROTATE_WITH_MINIMAP;
		searched = true;
	}
		
	
	for(int i = 0; i < 4; ++i) if(goombaInfos[i].killID != 0xffff)
	{
		if(DeathTable_GetBit(goombaInfos[i].killID))
		{
			goombaInfos[i].killID = 0xffff;
			--count;
			Actor::Spawn(0x014a, 4 - count, goombaInfos[i].pos + Vector3{0_f, 0x60000_f, 0_f}, nullptr, 0, -1);
			if(count == 0)
				Event::SetBit(3);
		}
		else
			goombaInfos[i].pos = goombaInfos[i].goomba->pos;
	}
	
	return 1;
}

int BBL2_Specifics::Render()
{
	return 1;
}

int BBL2_Specifics::CleanupResources()
{
	*(unsigned*)0x020fb4e4 = prevMinimapSizeInst; //Go back to default minimap
	return 1;
}

BBL2_Specifics::~BBL2_Specifics() {}

void init()
{
	OBJ_TO_ACTOR_ID_TABLE[0x0150] = 0x0172;
	ACTOR_SPAWN_TABLE[0x0172] = (unsigned)&BBL2_Specifics::spawnData;
	
	OBJ_TO_ACTOR_ID_TABLE[0x00c8] = 0x002e;
	ACTOR_SPAWN_TABLE[0x002e] = (unsigned)&BBL2_MegaBlock::spawnData;
	BBL2_MegaBlock::modelFile.Construct(0x0591);
	BBL2_MegaBlock::clsnFile .Construct(0x0592);
	
	OBJ_TO_ACTOR_ID_TABLE[0x00c6] = 0x002c;
	ACTOR_SPAWN_TABLE[0x002c] = (unsigned)&BBL2_Plank::spawnData;
	BBL2_Plank::modelFile.Construct(0x0589);
	BBL2_Plank::clsnFile .Construct(0x058a);
	
	prevMinimapSizeInst  = *(unsigned*)0x020fb4e4;	
	*(unsigned*)0x020fb4e4 = (0x020fb568 - 0x020fb4e4) / 4 - 2 & 0x00ffffff | 0xea000000; //Force 256x256 minimap
}