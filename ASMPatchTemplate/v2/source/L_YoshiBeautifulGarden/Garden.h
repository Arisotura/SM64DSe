#ifndef GARDEN_INCLUDED
#define GARDEN_INCLUDED

#include "../SM64DS_2.h"

struct YBG_Garden : public Actor
{	
	Model model;
	
	void UpdateModelTransform();

	static YBG_Garden* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual ~YBG_Garden();
	
	static SharedFilePtr modelFile;

	static SpawnInfo<YBG_Garden> spawnData;
};

#endif
