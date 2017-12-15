#ifndef GOOMBA_COLORED_INCLUDED
#define GOOMBA_COLORED_INCLUDED

#include "../SM64DS_2.h"

	
	
struct Goomba : public CapEnemy
{
	enum SizeType
	{
		SMALL = 0,
		NORMAL = 1,
		BIG = 2
	};
	
	CylinderClsn cylClsn;
	WithMeshClsn wmClsn;
	ModelAnim rigMdl;
	ShadowVolume shadow;
	MaterialChanger materialChg;
	TextureSequence texSeq;
	
	Vector3 noCliffPos;
	Vector3 originalPos;
	Vector3 backupPos;
	unsigned state;
	Player* player;
	Fix12i distToPlayer; //capped at 0x001a8000, and actually 0x061a8000 when it hits the cap
	Fix12i targetSpeed;
	Fix12i maxDist;
	short movementTimer;
	short regurgTimer;
	uint16_t stuckInSpotTimer;
	uint16_t noChargeTimer;
	short targetDir;
	short targetDir2;
	unsigned sizeType;
	char spawnSilverStar; //1 is true, any other value is false
	char silverStarID;
	char charBehav; //0 = jump, 1 = jump 'n' spin, 2 = do double damage
	char regurgBounceCount;
	char flags468;
	char extraDamage;
	unsigned hitFlags;
	
	void UpdateMaxDist();
	void Kill();
	void SpawnSilverStarIfNecessary();
	void KillAndSpawnCap();
	void KillAndSpawnSilverStar();
	bool UpdateIfDying();
	void RenderRegurgGoombaHelpless(Player* player);
	void KillIfTouchedBadSurface();
	void UpdateModel();
	bool UpdateIfEaten();
	void PlayMovingSound();
	void GetHurtOrHurtPlayer();
	void KillIfIntoxicated();
	void Jump();
	void UpdateTargetDirAndDist(Fix12i theMaxDist);
	void State0HelperFunc();

	static Goomba* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual void Virtual30() override;
	virtual ~Goomba();
	virtual unsigned OnYoshiTryEat() override;
	virtual void OnTurnIntoEgg(Player& player) override;
	virtual Fix12i OnAimedAtWithEgg() override;
	
	void State0(); //walking, running, all that stuff
	void State1(); //just hit player
	void State2(); //jumping
	void State3(); //bouncing after being spit out
	void State4(); //jumping at player
	void State5(); //spinning at player
	
	enum Animations
	{
		ROLLING,
		RUN,
		STRETCH,
		UNBALANCE,
		WAIT,
		WALK,
		
		NUM_ANIMS
	};
	
	static SharedFilePtr modelFile;
	static SharedFilePtr texSeqFile;
	static SharedFilePtr animFiles[NUM_ANIMS];
	
	static SpawnInfo<Goomba> spawnDataNormal[], spawnDataSmall[], spawnDataBig[];

	
};

#endif