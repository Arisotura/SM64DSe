#include "../SM64DS_2.h"

struct YBG_Specifics : public Actor
{
	static YBG_Specifics* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual ~YBG_Specifics() override;
	
	static SpawnInfo<YBG_Specifics> spawnData;
};