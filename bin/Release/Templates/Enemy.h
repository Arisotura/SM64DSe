#ifndef (_NAME)_INCLUDED
#define (_NAME)_INCLUDED

#include "../SM64DS_2.h"

struct (_Name) : public Enemy
{
	CylinderClsn cylClsn;
	WithMeshClsn wmClsn;
	ModelAnim rigMdl;
	ShadowVolume shadow;
	
#ifdeftmpl _UseStates
	uint8_t state;
#endiftmpl
	
	void UpdateModelTransform();
	void HandleClsn();
#ifdeftmpl _Defeatable
	void Kill();
#endiftmpl
#ifdeftmpl _DefeatableByBadSurface
	bool KillIfTouchedBadSurface();
#endiftmpl
	
	static (_Name)* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual void Virtual30() override;
	virtual ~(_Name)();
#ifdeftmpl _YoshiCanEat
	virtual unsigned OnYoshiTryEat() override;
	virtual void OnTurnIntoEgg(Player& player) override;
#endiftmpl
#ifdeftmpl _YoshiEggCanAim
	virtual Fix12i OnAimedAtWithEgg() override; //returns egg height
#endiftmpl

#ifdeftmpl _UseStates
	(_StateFuncDecls)
#endiftmpl

	static SpawnInfo<(_Name)> spawnData;
};

	

#endif