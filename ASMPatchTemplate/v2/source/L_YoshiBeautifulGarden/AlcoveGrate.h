#ifndef ALCOVEGRATE_INCLUDED
#define ALCOVEGRATE_INCLUDED

#include "../SM64DS_2.h"

struct YBG_AlcoveGrate : public Platform
{	
	Fix12i origPosY;
	uint8_t eventID;
	
	void UpdateModelTransform();

	static YBG_AlcoveGrate* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual ~YBG_AlcoveGrate();
	
	static SharedFilePtr modelFile;
	static SharedFilePtr clsnFile;

	static SpawnInfo<YBG_AlcoveGrate> spawnData;
};

#endif
