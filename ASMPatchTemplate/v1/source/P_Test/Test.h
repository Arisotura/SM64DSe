#ifndef TEST_INCLUDED
#define TEST_INCLUDED

#include "../SM64DS_2.h"

struct Test : public Platform
{	
	void UpdateModelTransform();

	static Test* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual ~Test();
	
	static SharedFilePtr modelFile;
	static SharedFilePtr clsnFile;

	static SpawnInfo<Test> spawnData;
};

#endif
