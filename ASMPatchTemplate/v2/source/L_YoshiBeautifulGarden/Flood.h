#ifndef YBG_FLOOD_INCLUDED
#define YBG_FLOOD_INCLUDED

#include "../SM64DS_2.h"

struct YBG_Flood : public Platform
{	
	TextureTransformer texSRT;
	uint8_t moveState;
	uint8_t eventID;
	uint8_t eventToTrigger;

	static YBG_Flood* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual ~YBG_Flood();

	static SharedFilePtr modelFile;
	static SharedFilePtr clsnFile;
	
	static SpawnInfo<YBG_Flood> spawnData;
};

#endif