#ifndef YOSHI_NPC_INCLUDED
#define YOSHI_NPC_INCLUDED

#include "../SM64DS_2.h"

struct YoshiNPC : public Actor
{
	ModelAnim rigMdl;
	CylinderClsn cylClsn;
	ShadowVolume shadow;
	uint8_t state;
	uint8_t eventID;
	bool shouldTalk;
	Player* listener;
	uint16_t messages[2];
	
	void UpdateModelTransform();
	
	void State0_Wait();
	void State1_Talk();
	
	static YoshiNPC* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual ~YoshiNPC() override;
	
	static SpawnInfo<YoshiNPC> spawnData;
};

#endif