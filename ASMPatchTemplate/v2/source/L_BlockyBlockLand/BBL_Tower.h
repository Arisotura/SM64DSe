#ifndef BBL_TOWER_INCLUDED
#define BBL_TOWER_INCLUDED

#include "../SM64DS_2.h"

struct BBL_Tower : public Platform
{
	
	Fix12i maxPosY;
	uint8_t moveState;
	Actor* trigger;
	Vector3 triggerPos;
	
	void UpdateModelTransform();
	void UpdateClsnTransform();
	static Actor* searchByPos(const Vector3& posToFind);

	static BBL_Tower* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual ~BBL_Tower();
	
	static SharedFilePtr modelFile;
	static SharedFilePtr clsnFile;

	static SpawnInfo<BBL_Tower> spawnData;
};

#endif