#ifndef SHY_GUY_INCLUDED
#define SHY_GUY_INCLUDED

#include "../SM64DS_2.h"

struct ShyGuy : public Enemy
{
	CylinderClsn cylClsn;
	WithMeshClsn wmClsn;
	ModelAnim rigMdl;
	ShadowVolume shadow;
	MaterialChanger matChg;
	
	uint8_t state;
	uint8_t chaseCooldown;
	short targetAngle;
	Player* targetPlayer;
	Vector3 targetPos;
	bool offTrack;
	uint8_t nextPathPt;
	uint8_t numPathPts;
	bool alarmed;
	bool backAndForth;
	bool reverse;
	PathPtr pathPtr;
	
	void UpdateModelTransform();
	static Fix12i FloorY(const Vector3& pos);
	void SetTargetPos();
	void Kill();
	void HandleClsn();
	Player* PlayerVisibleToThis(Player* player);
	bool KillIfTouchedBadSurface();
	int GetClosestPathPtID();
	void AimAtClosestPathPt();
	void PlayMovingSoundEffect();
	
	static ShyGuy* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual void Virtual30() override;
	virtual ~ShyGuy();
	virtual unsigned OnYoshiTryEat() override;
	virtual void OnTurnIntoEgg(Player& player) override;
	virtual Fix12i OnAimedAtWithEgg() override; //returns egg height

	void State0_Wait();
	void State1_Turn();
	void State2_Chase();
	void State3_Stop();
	void State4_Warp();

	static unsigned spawnData[];
};

	

#endif