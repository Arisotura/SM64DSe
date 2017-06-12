#include "../SM64DS_2.h"

extern bool towerStartedMoving;

struct BBL_Specifics : public Actor
{
	void SearchForStarGoombas();
	Actor* SearchForBlackBrick();
	
	static BBL_Specifics* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual ~BBL_Specifics() override;
	
	static SpawnInfo<BBL_Specifics> spawnData;
};