#include "ToxBox.h"

namespace
{
	enum States
	{
		ST_HIT_GROUND,
		ST_STOP,
		ST_MOVE,
		ST_JUMP,
		ST_ANTIGRAVITY
	};
	
	struct State
	{
		using FuncPtr = void(ToxBox::*)();
		FuncPtr main;
	};
	const State states[]
	{
		State{ &ToxBox::StHitGround },
		State{ &ToxBox::StStop },
		State{ &ToxBox::StMove },
		State{ &ToxBox::StJump },
		State{ &ToxBox::StAntigravity }
	};
	
	FixedSizeCLPS_Block<1> clpsBlock =
	{
		{'C', 'L', 'P', 'S'},
		8,
		1,
		{
			CLPS(0x04, 0, 0x3f, 0x0, 0x0, 0x00, 1, 0, 0, 0xff)
		}
	};

	SharedFilePtr modelFile;
	SharedFilePtr clsnFile;
	
	//bread, eggs, and breaded eggs
	constexpr Fix12i RADIUS = 0xfa000_f;
	constexpr Fix12i ROOT_2 = 0x16a1_f;
	constexpr Vector3 DIRS[] =
	{
		Vector3{ 0x0000_f, 0x0000_f,  0x1000_f},
		Vector3{ 0x1000_f, 0x0000_f,  0x0000_f},
		Vector3{ 0x0000_f, 0x0000_f, -0x1000_f},
		Vector3{-0x1000_f, 0x0000_f,  0x0000_f}
	};
}


SpawnInfo<ToxBox> ToxBox::spawnData =
{
	&ToxBox::Spawn,
	0x0135,
	0x0132,
	0x02000002,
	RADIUS,
	0x002ee000_f,
	0x01fa0000_f,
	0x01fa0000_f,
};

ToxBox* ToxBox::Spawn()
{
	return new ToxBox;
}

void ToxBox::UpdateClsnTransform()
{
	clsnNextMat = model.mat4x3;
	clsnNextMat.r0c3 = pos.x;
	clsnNextMat.r1c3 = pos.y;
	clsnNextMat.r2c3 = pos.z;
	clsn.Transform(clsnNextMat, ang.y);
}

void ToxBox::UpdateModelTransform()
{
	model.mat4x3 = baseMat * turnMat * orientMat;
	model.mat4x3.r0c3 = pos.x >> 3;
	model.mat4x3.r1c3 = pos.y >> 3;
	model.mat4x3.r2c3 = pos.z >> 3;
}

int ToxBox::InitResources()
{
	char* modelF = Model::LoadFile(modelFile);
	model.SetFile(modelF, 1, -1);
	
	pathPtr.FromID(param1 >> 8 & 0xff);
	numPathPts = pathPtr.NumPts();
	currPathPtID = 0;
	pathPtr.GetPt(currPathPt, currPathPtID);
	
	baseMat.ThisFromRotationXYZExt(ang.x, ang.y, ang.z);
	normal = Vector3{baseMat.r0c1, baseMat.r1c1, baseMat.r2c1};
	pos += normal * RADIUS;
	
	unsigned orientMatID = param1 & 0xff; //24 possible values
	Vector3 matBlob[] =
	{
		Vector3{0x1000_f, 0x0000_f, 0x0000_f},
	    Vector3{0x0000_f, 0x1000_f, 0x0000_f},
	    Vector3{0x0000_f, 0x0000_f, 0x1000_f},
	    Vector3{0x1000_f, 0x0000_f, 0x0000_f},
	    Vector3{0x0000_f, 0x1000_f, 0x0000_f}
	};
	Vector3* matPtr = &matBlob[orientMatID / 8];
	if(orientMatID & 4)
	{
		Vector3 temp = matPtr[1];
		matPtr[1] = matPtr[2];
		matPtr[2] = temp * -0x1000_f;
	}
	if(orientMatID & 2)
	{
		matPtr[1] *= -0x1000_f;
		matPtr[2] *= -0x1000_f;
	}
	if(orientMatID & 1)
	{
		matPtr[0] *= -0x1000_f;
		matPtr[1] *= -0x1000_f;
	}
	MultiCopy_Int((void*)matPtr, (void*)&orientMat, 36);
	turnMat.LoadIdentity();
	turnAng = 0;
	baseMatInv = baseMat.Inv();
	UpdateModelTransform();
	UpdateClsnTransform();
	
	char* clsnF = MovingMeshCollider::LoadFile(clsnFile);
	clsn.SetFile(clsnF, clsnNextMat, 0x1000_f, ang.y, clpsBlock);
	clsn.beforeClsnCallback = (decltype(clsn.beforeClsnCallback))0x02039348;
	
	return 1;
}

int ToxBox::CleanupResources()
{
	clsn.Disable();
	modelFile.Release();
	clsnFile.Release();
	return 1;
}

void ToxBox::AdvancePath()
{
	if(backward)
	{
		if(currPathPtID == 0)
			backward = false;
		else
			--currPathPtID;
	}
	else
	{
		if(currPathPtID == numPathPts - 1)
			backward = true;
		else
			++currPathPtID;
	}
}

void ToxBox::InitMoveState()
{
	state = ST_MOVE;
	if(numTurns == 0)
	{
		Vector3 prevPathPt = currPathPt;
		AdvancePath();
		pathPtr.GetPt(currPathPt, currPathPtID);
		
		Vector3 diff = currPathPt - prevPathPt;
		diff.TransformThis(baseMatInv);
		if(diff.y.Abs() < 0x1000_f)
			diff.y = 0_f;
		else
			state = ST_ANTIGRAVITY;
		
		if(diff.x.Abs() > diff.z.Abs())
		{
			diff.z = 0_f;
			turnDir = diff.x >= 0_f ? 1 : 3;
		}
		else
		{
			diff.x = 0_f;
			turnDir = diff.z >= 0_f ? 0 : 2;
		}
		if(state != ST_ANTIGRAVITY)
		{
			numTurns = (int)(diff.HorzLen() / (2 * RADIUS) + 0x800_f);
			if(numTurns == 0)
			{
				state = ST_JUMP;
				return;
			}
		}
		
		targetPos = currPathPt;
		pivotHorzDist = (DIRS[turnDir] * RADIUS).Transform(baseMat);
		radiusNormal = Vector3{0_f, RADIUS, 0_f}.Transform(baseMat); //for precision
		if(state == ST_ANTIGRAVITY)
		{
			pivotCenter = (DIRS[turnDir] * RADIUS).Transform(baseMat) + prevPathPt;
			Fix12i forwardDist;
			switch(turnDir)
			{
				case 0: forwardDist =  diff.z; break;
				case 1: forwardDist =  diff.x; break;
				case 2: forwardDist = -diff.z; break;
				case 3: forwardDist = -diff.x; break;
			}
			destTurnAng = Atan2(forwardDist - RADIUS, diff.y);
			if(destTurnAng > -0x4000 && destTurnAng <= 0)
			{
				SetBaseMat();
				InitMoveState();
			}
			return;
		}
		else
			destTurnAng = 0x4000;
	}
	--numTurns;
	pivotCenter = (DIRS[turnDir] * RADIUS * Fix12i(-2 * numTurns - 1)).Transform(baseMat) + targetPos;
}

Matrix4x3 ToxBox::GetTurnMat(short ang)
{
	switch(turnDir)
	{
		case 0: return Matrix4x3::FromRotationX( ang);
		case 1: return Matrix4x3::FromRotationZ(-ang);
		case 2: return Matrix4x3::FromRotationX(-ang);
		case 3: return Matrix4x3::FromRotationZ( ang);
	}
}
void ToxBox::SetTurnMat(Matrix4x3& theTurnMat, short ang)
{
	switch(turnDir)
	{
		case 0: theTurnMat.ThisFromRotationX( ang); break;
		case 1: theTurnMat.ThisFromRotationZ(-ang); break;
		case 2: theTurnMat.ThisFromRotationX(-ang); break;
		case 3: theTurnMat.ThisFromRotationZ( ang); break;
	}
}

void ToxBox::SetBaseMat()
{
	baseMat = baseMat * GetTurnMat(destTurnAng - 0x4000);
	baseMatInv = GetTurnMat(-destTurnAng + 0x4000) * baseMatInv;
	orientMat = GetTurnMat(0x4000) * orientMat;
	normal = Vector3{baseMat.r0c1, baseMat.r1c1, baseMat.r2c1};
	pos = targetPos + normal * RADIUS;
}

void ToxBox::StHitGround()
{
	if(stateTimer == 0)
	{
		Earthquake(pos, 0x860000_f);
		Sound::PlayBank3(0x46, camSpacePos);
	}
	
	StStop();
}
void ToxBox::StStop()
{
	if(stateTimer == 20)
		InitMoveState();
}
//8 frames long

/* Turn directions
           2
           ^
           |
           |
   3 <-----+-----> 1
           |
		   |
		   v
		   0
*/
void ToxBox::Move()
{
	turnAng = (unsigned)(uint16_t)destTurnAng * (stateTimer + 1) / 8;
	SetTurnMat(turnMat, turnAng);
	pos = pivotCenter + radiusNormal * (Sin(turnAng + 0x2000) * ROOT_2) - pivotHorzDist * (Cos(turnAng + 0x2000) * ROOT_2);
	
	if(stateTimer == 7)
	{
		turnAng = 0;
		if(state == ST_ANTIGRAVITY)
		{
			SetBaseMat();
		}
		else
		{
			orientMat = turnMat * orientMat;
			if(numTurns == 0)
				pos = targetPos + normal * RADIUS;
		}
		state = ST_HIT_GROUND;
		turnMat.LoadIdentity();
	}
}

void ToxBox::StMove()
{
	Move();
}

void ToxBox::StJump()
{
	pos = currPathPt + radiusNormal * (Sin((stateTimer + 1) * 0x800 + 0x2000) * ROOT_2);
	if(stateTimer == 7)
	{
		state = ST_HIT_GROUND;
	}
}

void ToxBox::StAntigravity()
{
	Move();
}

int ToxBox::Behavior()
{
	unsigned prevMoveState = state;
	(this->*states[state].main)();
	++stateTimer;
	if(prevMoveState != state)
		stateTimer = 0;
	
	UpdateModelTransform();
	if(IsClsnInRange(0_f, 0_f))
		UpdateClsnTransform();
	
	return 1;
}

int ToxBox::Render()
{
	model.Render(nullptr);
	return 1;
}

ToxBox::~ToxBox() {}

void init()
{
	OBJ_TO_ACTOR_ID_TABLE[0x00f2] = 0x0135;
	ACTOR_SPAWN_TABLE[0x0135] = (unsigned)&ToxBox::spawnData;
	modelFile.Construct(0x03c9);
	clsnFile .Construct(0x03ca);
}