#ifndef SUPER_MUSHROOM_SPAWNER_INCLUDED
#define SUPER_MUSHROOM_SPAWNER_INCLUDED

#include "../SM64DS_2.h"

struct Spawner_SuperMushroom : public Actor
{
	uint16_t timer;
	uint16_t maxTimer;
	unsigned superMushroomID;
	uint8_t eventID;

	static Spawner_SuperMushroom* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual ~Spawner_SuperMushroom() override;
	
	static SpawnInfo<Spawner_SuperMushroom> spawnData;
};

#endif