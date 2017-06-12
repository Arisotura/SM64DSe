#include "../SM64DS_2.h"

struct BBL2_Specifics : public Actor
{
	void SearchForGoombas();
	Minimap* GetMinimap();
	
	static BBL2_Specifics* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual ~BBL2_Specifics() override;
	
	static SpawnInfo<BBL2_Specifics> spawnData;
};