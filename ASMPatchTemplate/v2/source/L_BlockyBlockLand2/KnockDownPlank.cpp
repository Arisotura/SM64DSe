#include "KnockDownPlank.h"
#include "BlockyBlock2Specifics.h"

namespace
{				   
	FixedSizeCLPS_Block<1> clpsBlock =
	{
		{'C', 'L', 'P', 'S'},
		8,
		1,
		{
			CLPS(0x05, 0, 0x3f, 0x0, 0x0, 0x00, 0, 0, 0, 0xff)
		}
	};
}

SharedFilePtr BBL2_Plank::modelFile;
SharedFilePtr BBL2_Plank::clsnFile;
	
SpawnInfo<BBL2_Plank> BBL2_Plank::spawnData =
{
	&BBL2_Plank::Spawn,
	0x002c,
	0x00c5,
	0x00000002,
	0x00000000_f,
	0x01800000_f,
	0x02000000_f,
	0x02000000_f
};

BBL2_Plank* BBL2_Plank::Spawn()
{
	return new BBL2_Plank;
}

void BBL2_Plank::UpdateModelPosAndRotXYZ()
{
	model.mat4x3.ThisFromRotationXYZExt(ang.x, ang.y, ang.z);
	model.mat4x3.r0c3 = pos.x >> 3;
	model.mat4x3.r1c3 = pos.y >> 3;
	model.mat4x3.r2c3 = pos.z >> 3;
}

int BBL2_Plank::InitResources()
{
	char* modelF = Model::LoadFile(modelFile);
	model.SetFile(modelF, 1, -1);
	shadow.InitCuboid();
	
	UpdateModelPosAndRotXYZ();
	UpdateClsnPosAndRot();
	
	char* clsnF = MovingMeshCollider::LoadFile(clsnFile);
	clsn.SetFile(clsnF, clsnNextMat, 0x1000_f, ang.y, clpsBlock);
	
	//rayStart = {0, 0, -0xfa000}.RotateY(ang.y) + pos + {0, 0x14000, 0}
	MATRIX_SCRATCH_PAPER.ThisFromRotationY(ang.y);
	Vector3 rayStart = Vector3{0_f, 0_f, -0x12c000_f}.Transform(MATRIX_SCRATCH_PAPER) + pos;
	rayStart.y += 0x14000_f;
	
	RaycastGround raycast; //r13+0x38
	raycast.SetObjAndPos(rayStart, nullptr);
	frontFloorY = raycast.DetectClsn() ? raycast.clsnPosY : rayStart.y;
	
	MATRIX_SCRATCH_PAPER.ThisFromRotationY(ang.y); //I don't know if raycast.DetectClsn() uses this, so reset it
	backPos = Vector3{0_f, 0_f, 0x96000_f}.Transform(MATRIX_SCRATCH_PAPER);
	
	somePosY = pos.y;
	
	return 1;
}

int BBL2_Plank::CleanupResources()
{
	if(clsn.IsEnabled())
		clsn.Disable();
	
	modelFile.Release();
	clsnFile.Release();
	
	return 1;
}

void BBL2_Plank::DropThatShadow()
{
	const Fix12i poleLen = 0x141e000_f;
	
	Fix12i multSinX = Sin(ang.x) * poleLen;
	Fix12i shadowLen = multSinX.Abs();
	short shadowDir = multSinX <= 0_f ? 0x0000 : 0x8000;
	shadowMat.ThisFromRotationY(ang.y + shadowDir);
	
	//Divide shadowLen by 2 to center the shadow volume
	shadowMat.r0c3 = (-Sin(ang.y + shadowDir) * (shadowLen >> 1) + backPos.x) >> 3;
	shadowMat.r1c3 = backPos.y >> 3;
	shadowMat.r2c3 = (-Cos(ang.y + shadowDir) * (shadowLen >> 1) + backPos.z) >> 3;
	
	if(state >= 2)
		shadowMat.r1c3 = frontFloorY >> 3;
	
	DropShadowScaleXYZ(shadow, shadowMat, 0x2ee000_f, 0x12c000_f, shadowLen, 0xf);
}

int BBL2_Plank::Behavior()
{
	if(state == 1) //Wobbling
	{
		ang.x = (short)int(-Sin(wobbleAng) * Fix12i(wobbleTimer, true));
		if(wobbleTimer <= 0)
		{
			ang.x = 0;
			wobbleAng = 0;
			state = 0;
		}
		else
			wobbleTimer -= 8;
		
		wobbleAng += 0x400;
	}
	else if(state == 2) //Falling
	{
		fallAngVel -= 0x80;
		ang.x += fallAngVel;

		if(ang.x < -0x4000)
		{
			ang.x = -0x4000;
			fallAngVel = 0;
			state = 3;
			
			Earthquake(pos, 0x06000000_f);
			Sound::PlayBank3(0x44, camSpacePos);
		}
	}
	
	UpdateModelPosAndRotXYZ();
	if(IsClsnInRange(0_f, 0_f))
		UpdateClsnPosAndRot();
	
	DropThatShadow();
	
	return 1;
}

int BBL2_Plank::Render()
{
	model.Render(nullptr);
	return 1;
}

BBL2_Plank::~BBL2_Plank() {}

void BBL2_Plank::OnAttacked(Actor& attacker)
{
	//r5 = this, r4 = attacker
	if(state >= 2)
		return;
	
	if(state == 0)
		state = 1;
	
	//int to handle the 0x8000 case
	int attackDir = AngleDiff(pos.HorzAngle(attacker.pos), ang.y);
	if(attackDir >= 0x2000)
		return;
	
	/*if(state == 1 && wobbleTimer >= 0x320 && attacker->pos.y > pos.y + 0x64000)
		state = 2;*/
	
	wobbleTimer = 0x320;
}

void BBL2_Plank::OnAttacked1(Actor& attacker)
{
	OnAttacked(attacker);
}
void BBL2_Plank::OnAttacked2(Actor& attacker)
{
	OnAttacked(attacker);
}

void BBL2_Plank::OnHitByMegaChar(Actor& actor)
{
	if(state >= 2 || actor.actorID != 0x00bf)
		return;
	Player& player = *(Player*)&actor;
	
	player.IncMegaKillCount();
	state = 2;
	wobbleTimer = 0x640;
}
