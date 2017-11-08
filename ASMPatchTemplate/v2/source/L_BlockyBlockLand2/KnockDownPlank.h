#ifndef KNOCK_DOWN_PLANK_INCLUDED
#define KNOCK_DOWN_PLANK_INCLUDED

#include "../SM64DS_2.h"

struct BBL2_Plank : public Platform
{
	
	ShadowVolume shadow;
	Matrix4x3 shadowMat;
	Vector3 backPos;
	Fix12i frontFloorY;
	Fix12i somePosY;
	short wobbleAng;
	short fallAngVel;
	short wobbleTimer;
	uint8_t state;
	
	void UpdateModelPosAndRotXYZ();
	void DropThatShadow();
	void OnAttacked(Actor& attacker);

	static BBL2_Plank* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual ~BBL2_Plank();
	virtual void OnAttacked1(Actor& attacker) override;
	virtual void OnAttacked2(Actor& attacker) override;
	virtual void OnHitByMegaChar(Actor& actor) override;
	
	static SharedFilePtr modelFile;
	static SharedFilePtr clsnFile;
	
	static SpawnInfo<BBL2_Plank> spawnData;
};

#endif