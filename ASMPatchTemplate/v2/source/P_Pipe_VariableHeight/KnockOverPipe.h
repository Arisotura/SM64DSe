#ifndef KNOCK_OVER_PIPE_INCLUDED
#define KNOCK_OVER_PIPE_INCLUDED

#include "../SM64DS_2.h"

struct PipeVH : public Platform
{
	
	ShadowVolume shadow;
	Matrix4x3 shadowMat;
	Model stub;
	Fix12i height;
	bool stubbed;
	
	void UpdateClsn();
	void SetClsnHeight();
	void UpdateStubModel();
	void UpdateShadowMatrix();

	static PipeVH* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual ~PipeVH();
	virtual void OnHitByMegaChar(Actor& actor) override;
	virtual void Kill() override;
	
	static SpawnInfo<PipeVH> spawnData;
};

#endif