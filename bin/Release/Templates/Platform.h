#ifndef (_NAME)_INCLUDED
#define (_NAME)_INCLUDED

#include "../SM64DS_2.h"

struct (_Name) : public Platform
{	
	void UpdateModelTransform();
#ifdeftmpl _CreateUpdateClsnFunc
	void UpdateClsnTransform();
#endiftmpl

	static (_Name)* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual ~(_Name)();
	
	static SharedFilePtr modelFile;
	static SharedFilePtr clsnFile;

	static SpawnInfo<(_Name)> spawnData;
};

#endif