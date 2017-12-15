#ifndef YBG_SPILL_INCLUDED
#define YBG_SPILL_INCLUDED

#include "../SM64DS_2.h"

struct YBG_Spill : public Platform
{	
	TextureTransformer texSRT;
	ModelAnim rigMdl;
	unsigned soundID;
	uint8_t eventID;
	bool corked;
	
	void UpdateModelTransform();

	static YBG_Spill* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual ~YBG_Spill();
	
	enum Animations
	{
		STOP_LEAK,
		
		NUM_ANIMS
	};

	static SharedFilePtr modelFile;
	static SharedFilePtr clsnFile;
	static SharedFilePtr animFiles[NUM_ANIMS];
	
	static SpawnInfo<YBG_Spill> spawnData;
};

#endif