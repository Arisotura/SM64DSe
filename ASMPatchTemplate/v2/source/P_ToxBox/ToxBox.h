#ifndef TOX_BOX_INCLUDED
#define TOX_BOX_INCLUDED

#include "../SM64DS_2.h"

//@02130f00
struct ToxBox : public Platform
{	
	unsigned state;
	unsigned stateTimer;
	
	unsigned numPathPts;
	uint8_t currPathPtID;
	bool backward;
	short turnAng;
	short destTurnAng;
	uint8_t turnDir;
	Vector3 currPathPt;
	PathPtr pathPtr;
	
	Matrix4x3 baseMat;
	Matrix4x3 orientMat;
	Matrix4x3 turnMat;
	Matrix4x3 baseMatInv;
	Vector3 normal;
	Vector3 radiusNormal;
	Vector3 targetPos;
	Vector3 pivotCenter;
	Vector3 pivotHorzDist;
	int numTurns;
	
	
	void UpdateClsnTransform();
	void UpdateModelTransform();
	void AdvancePath();
	void InitMoveState(); //returns whether to move or jump
	Matrix4x3 GetTurnMat(short ang);
	void SetTurnMat(Matrix4x3& theTurnMat, short ang);
	void SetBaseMat();
	void Move();
	
	void StHitGround();
	void StStop();
	void StMove();
	void StJump();
	void StAntigravity();
	
	static ToxBox* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual ~ToxBox();

	static SpawnInfo<ToxBox> spawnData;
};

#endif