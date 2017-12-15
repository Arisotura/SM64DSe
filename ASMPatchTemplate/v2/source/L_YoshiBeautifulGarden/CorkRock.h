#ifndef CORKROCK_INCLUDED
#define CORKROCK_INCLUDED

#include "../SM64DS_2.h"

struct YBG_CorkRock : public Platform
{	
	ShadowVolume shadow;
	Matrix4x3 shadowMat;
	uint8_t eventIDs[2];
	bool startedTrigger;
	bool triggered;

	void UpdateModelTransform();
	void UpdateClsnTransform();

	static YBG_CorkRock* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual ~YBG_CorkRock();

	static SharedFilePtr modelFile;
	static SharedFilePtr clsnFile;
	
	static SpawnInfo<YBG_CorkRock> spawnData;
};

#endif
