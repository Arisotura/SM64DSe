#ifndef MEGA_BLOCK_INCLUDED
#define MEGA_BLOCK_INCLUDED

#include "../SM64DS_2.h"

struct BBL2_MegaBlock : public Platform
{
	ShadowVolume shadow;
	Matrix4x3 shadowMat;
	
	void UpdateShadowMatrix();

	static BBL2_MegaBlock* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual ~BBL2_MegaBlock();
	virtual void OnHitByMegaChar(Actor& actor) override;
	virtual void Kill() override;
	
	static SharedFilePtr modelFile;
	static SharedFilePtr clsnFile;

	static SpawnInfo<BBL2_MegaBlock> spawnData;
};

#endif