#ifndef YBG_PUSHPIPE_INCLUDED
#define YBG_PUSHPIPE_INCLUDED

#include "../SM64DS_2.h"

struct YBG_PushPipe : public Platform
{	
	ModelAnim rigMdl;
	
	void UpdateModelTransform();
	void UpdateClsnTransform();

	static YBG_PushPipe* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual ~YBG_PushPipe();

	static SpawnInfo<YBG_PushPipe> spawnData;
};

#endif