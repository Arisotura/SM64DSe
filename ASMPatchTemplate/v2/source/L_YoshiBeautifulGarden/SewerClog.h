#ifndef SEWERCLOG_INCLUDED
#define SEWERCLOG_INCLUDED

#include "../SM64DS_2.h"

struct YBG_SewerClog : public Platform
{	
	void UpdateModelTransform();
	uint8_t eventID;

	static YBG_SewerClog* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual ~YBG_SewerClog();
	
	static SharedFilePtr modelFile;
	static SharedFilePtr clsnFile;

	static SpawnInfo<YBG_SewerClog> spawnData;
};

#endif
