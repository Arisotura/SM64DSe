#ifndef EXTRALAVA_INCLUDED
#define EXTRALAVA_INCLUDED

#include "../SM64DS_2.h"

struct YBG_ExtraLava : public Platform
{	
	TextureTransformer texSRT;
	TextureSequence texSeq;
	
	void UpdateModelTransform();

	static YBG_ExtraLava* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual ~YBG_ExtraLava();

	static SharedFilePtr modelFile;
	static SharedFilePtr clsnFile;
	static SharedFilePtr texSeqFile;
	
	static SpawnInfo<YBG_ExtraLava> spawnData;
};

#endif
