#include "Thwomp.h"

namespace
{
	enum States
	{
		ST_GO_UP,
		ST_WAIT_UP,
		ST_HIT_GROUND,
		ST_WAIT_GROUND,
		ST_WAIT_GROUND_2
	};
	
	struct State
	{
		using FuncPtr = void(Thwomp::*)();
		FuncPtr main;
	};
	const State states[]
	{
		//State{ &Thwomp::StHitGround },
	};
	
	FixedSizeCLPS_Block<2> clpsBlock =
	{
		{'C', 'L', 'P', 'S'},
		8,
		2,
		{
			CLPS(0x04, 0, 0x3f, 0x0, 0x0, 0x00, 1, 0, 0, 0xff),
			CLPS(0x04, 0, 0x3f, 0x1, 0x0, 0x00, 1, 0, 0, 0xff)
		}
	};

	SharedFilePtr modelFile;
	SharedFilePtr texSeqFile;
	SharedFilePtr clsnFile;
	
	constexpr Fix12i WIDTH  = 0x140000_f;
	constexpr Fix12i DEPTH  =  0xdc000_f;
	constexpr Fix12i HEIGHT = 0x19a000_f;
}


SpawnInfo<Thwomp> Thwomp::spawnData =
{
	&Thwomp::Spawn,
	0x00a1,
	0x0047,
	0x02000002,
	0x00100000_f,
	0x00200000_f,
	0x01000000_f,
	0x01000000_f,
};

Thwomp* Thwomp::Spawn()
{
	return new Thwomp;
}

//02133254
/*void Thwomp::InitResourcesHelper()
{
	
}*/

int Thwomp::InitResources()
{
	//modelFilePtr = &resources; //???
	
	char* modelF = Model::LoadFile(modelFile);
	model.SetFile(modelF, 1, -1);
	
	UpdateModelPosAndRotY();
	UpdateClsnPosAndRot();
	
	char* clsnF = MovingMeshCollider::LoadFile(clsnFile);
	clsn.SetFile(clsnF, clsnNextMat, 0x199_f, ang.y, clpsBlock);
	clsn.beforeClsnCallback = (decltype(clsn.beforeClsnCallback))0x02039348;
	clsn.Enable(this);
	
	TextureSequence::LoadFile(texSeqFile);
	TextureSequence::Prepare(modelFile.filePtr, texSeqFile.filePtr);
	texSeq.SetFile(texSeqFile.filePtr, Animation::NO_LOOP, 0x1000_f, 0);
	
	if(!shadow.InitCuboid())
		return 0;
	
	minPosY = -0x7d00000_f;
	
	RaycastGround raycaster;
	raycaster.SetObjAndPos(Vector3{pos.x, pos.y + 0x32000_f, pos.z}, nullptr);
	if(raycaster.DetectClsn())
		minPosY = raycaster.clsnPosY;
	
	maxPosY = pos.y;
	pos.y = minPosY;
	waitTime = 40;
	
	vertAccel = -0x4000_f;
	termVel = -0x3c000_f;
	
	state = ST_GO_UP;
	actionWaitTime = 0;
	
	return 1;
}

int Thwomp::CleanupResources()
{
	clsn.Disable();
	texSeqFile.Release();
	modelFile.Release();
	clsnFile.Release();
	return 1;
}

//02133020
void Thwomp::GoUp()
{
	pos.y += 0xa000_f;
	if(pos.y < maxPosY)
		return;
	
	pos.y = maxPosY;
	state = ST_WAIT_UP;
	waitTime = 25;
}

//02132ff4
void Thwomp::WaitUp()
{
	if(DecIfAbove0_Byte(waitTime) == 0)
		state = ST_HIT_GROUND;
}

//02132f04
void Thwomp::HitGround()
{
	speed.y += vertAccel;
	if(speed.y < termVel)
		speed.y = termVel;
	
	pos.y += speed.y;
	if(pos.y > minPosY)
		return;
	
	pos.y = minPosY;
	speed.y = 0_f;
	state = ST_WAIT_GROUND;
	waitTime = 10;
	
	BigLandingDust(true);
	Earthquake(pos, 0x7d0000_f);
	Sound::PlayBank3(0xc7, camSpacePos);
	
}

//02132e98
void Thwomp::WaitGround()
{
	if(DecIfAbove0_Byte(waitTime) != 0)
		return;
	
	state = ST_WAIT_GROUND_2;
	waitTime = 25;
}

//02132e64
void Thwomp::WaitGround2()
{
	if(DecIfAbove0_Byte(waitTime) == 0)
	{
		state = ST_GO_UP;
		waitTime = 40;
	}
}

//02133098
void Thwomp::DropShadow()
{
	Fix12i shadowHeight = pos.y - minPosY + 0x14000_f;
	shadowMat = model.mat4x3;
	DropShadowScaleXYZ(shadow, shadowMat, WIDTH, shadowHeight, DEPTH, 0xf);
}

int Thwomp::Behavior()
{
	switch(state)
	{
		case ST_GO_UP:
			if((int)texSeq.currFrame != 0)
			{
				texSeq.currFrame -= 0x1000_f;
				if((int)texSeq.currFrame == 0)
					actionWaitTime = 10; //decimal
			}
			else if(actionWaitTime == 0)
				GoUp();
			else
				--actionWaitTime;
			break;
			
		case ST_WAIT_UP:
			WaitUp();
			break;
			
		case ST_HIT_GROUND:
			texSeq.Advance();
			if(texSeq.Finished())
			{
				if(actionWaitTime == 0)
					HitGround();
				else
					--actionWaitTime;
			}
			else
				actionWaitTime = 5;
			break;
			
		case ST_WAIT_GROUND:
			WaitGround();
			break;
			
		case ST_WAIT_GROUND_2:
			WaitGround2();
			break;
			
	}
	
	UpdateModelPosAndRotY();
	DropShadow();
	if(IsClsnInRange(0_f, 0_f))
		UpdateClsnPosAndRot();
	
	return 1;
}

int Thwomp::Render()
{
	texSeq.Update(model.data);
	model.Render(nullptr);
	return 1;
}

Thwomp::~Thwomp() {}

void Thwomp::OnHitByMegaChar(Player& megaChar)
{
	megaChar.IncMegaKillCount();
	Particle::System::NewSimple(0x48, pos.x, pos.y + HEIGHT / 2, pos.z);
	PoofDustAt(Vector3{pos.x, pos.y + HEIGHT / 2, pos.z});
	Destroy();
	Sound::PlayBank3(0x1e, camSpacePos);
}

void init()
{
	OBJ_TO_ACTOR_ID_TABLE[0x0042] = 0x00a1;
	ACTOR_SPAWN_TABLE[0x00a1] = (unsigned)&Thwomp::spawnData;
	modelFile .Construct(0x02f4);
	texSeqFile.Construct(0x02f5);
	clsnFile  .Construct(0x02f6);
}

void cleanup()
{
	
}