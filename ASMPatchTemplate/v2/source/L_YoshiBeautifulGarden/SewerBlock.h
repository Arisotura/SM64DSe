#ifndef SEWERBLOCK_INCLUDED
#define SEWERBLOCK_INCLUDED

#include "../SM64DS_2.h"

struct YBG_SewerBlock : public Platform
{	
	void UpdateModelTransform();

	static YBG_SewerBlock* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual ~YBG_SewerBlock();
	
	static SharedFilePtr modelFile;
	static SharedFilePtr clsnFile;

	static SpawnInfo<YBG_SewerBlock> spawnData;
};

#endif
