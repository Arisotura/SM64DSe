#ifndef YBG_SPRINKLER_INCLUDED
#define YBG_SPRINKLER_INCLUDED

#include "../SM64DS_2.h"

struct YBG_Sprinkler : public Platform
{
	unsigned particleSysID;
	uint8_t eventID;

	static YBG_Sprinkler* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual ~YBG_Sprinkler();

	static SharedFilePtr modelFile;
	static SharedFilePtr clsnFile;
	
	static SpawnInfo<YBG_Sprinkler> spawnData;
};

#endif