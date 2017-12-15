#ifndef THWOMP_INCLUDED
#define THWOMP_INCLUDED

#include "../SM64DS_2.h"

struct Thwomp : public Platform
{	
	//Resources* resPtr; 
	TextureSequence texSeq;
	ShadowVolume shadow;
	Matrix4x3 shadowMat;
	Fix12i maxPosY;
	Fix12i minPosY;
	unsigned state;
	uint8_t waitTime;
	uint16_t actionWaitTime;
	
	void GoUp();
	void WaitUp();
	void HitGround();
	void WaitGround();
	void WaitGround2();
	
	void DropShadow();
	
	static Thwomp* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual ~Thwomp();
	virtual void OnHitByMegaChar(Player& megaChar);

	static SpawnInfo<Thwomp> spawnData;
};

#endif