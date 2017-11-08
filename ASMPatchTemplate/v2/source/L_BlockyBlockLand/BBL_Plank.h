#ifndef BBL_PLANK_INCLUDED
#define BBL_PLANK_INCLUDED

#include "../SM64DS_2.h"

struct BBL_Plank : public Platform
{
	Fix12i offsetY;
	Fix12i maxOffsetY;
	uint8_t moveState;
	
	void UpdateModelTransform();
	void UpdateClsnTransform();
	
	static BBL_Plank* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual ~BBL_Plank();
	
	static SharedFilePtr modelFile;
	static SharedFilePtr clsnFile;

	static SpawnInfo<BBL_Plank> spawnData;
};


#endif