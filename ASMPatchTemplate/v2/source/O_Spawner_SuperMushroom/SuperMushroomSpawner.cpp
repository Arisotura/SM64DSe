#include "SuperMushroomSpawner.h"

SpawnInfo<Spawner_SuperMushroom> Spawner_SuperMushroom::spawnData =
{
	&Spawner_SuperMushroom::Spawn,
	0x0169,
	0x00aa,
	0x00000003,
	0x00000000_f,
	0x00600000_f,
	0x01000000_f,
	0x01000000_f
};

Spawner_SuperMushroom* Spawner_SuperMushroom::Spawn()
{
	return new Spawner_SuperMushroom;
}

int Spawner_SuperMushroom::InitResources()
{
	Model::LoadFile(SUPER_MUSHROOM_MODEL_PTR); //Load Super Mushroom model or else particle color glitches will happen
	
	eventID = param1 & 0xff;
	maxTimer = param1 >> 8;
	superMushroomID = 0xffffffff;
	
	return 1;
}

int Spawner_SuperMushroom::CleanupResources()
{
	SUPER_MUSHROOM_MODEL_PTR.Release();
	return 1;
}

int Spawner_SuperMushroom::Behavior()
{
	if(eventID < 0x20)
	{
		if(Event::GetBit(eventID))
			eventID = 0xff;
		else
			return 1;
	}
		
	if(superMushroomID == 0xffffffff)
	{
		if(timer == 0)
		{
			Vector3 position = pos;
			Actor* sm = Actor::Spawn(0x0115, 0xfff3, position, nullptr, areaID, -1);
			if(sm)
			{
				superMushroomID = sm->uniqueID;
				timer = maxTimer;
			}
		}
		else
			--timer;
	}
	else if(!Actor::FindWithID(superMushroomID))
		superMushroomID = 0xffffffff; //It'll take years before that many objects get spawned. <-- that was inb4 Remote Interaction Glitch!
	
	
	return 1;
}

int Spawner_SuperMushroom::Render()
{
	return 1;
}

Spawner_SuperMushroom::~Spawner_SuperMushroom() {}

void init()
{	
	OBJ_TO_ACTOR_ID_TABLE[0x0147] = 0x0169;
	ACTOR_SPAWN_TABLE[0x0169] = (unsigned)&Spawner_SuperMushroom::spawnData;
}

void cleanup()
{
	
}
