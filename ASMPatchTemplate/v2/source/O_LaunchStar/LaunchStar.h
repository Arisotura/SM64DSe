#ifndef LAUNCH_STAR_INCLUDED
#define LAUNCH_STAR_INCLUDED

#include "../SM64DS_2.h"

struct LaunchStar : public Actor
{
	ModelAnim rigMdl;
	CylinderClsn cylClsn;
	
	Fix12i launchSpeed;
	PathPtr pathPtr;
	uint8_t eventID;
	unsigned particleID;
	
	void UpdateModelTransform();

	static LaunchStar* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual ~LaunchStar();

	static SpawnInfo<LaunchStar> spawnData;
};

#endif