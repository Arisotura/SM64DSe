#include "KnockOverPipe.h"

//100000 ->        0,    10000
//200000 ->    26000     10000
//240000 -> fffe6000,    12000
//280000 ->    39000,     d555
//2ee000 ->    495ac
//300000 ->    4c000,     ffff

//RealHeight = StatedHeight + (StatedHeight - 0x100_f) * 0x26_f / 0x100_f
//StatedHeight = (0x100 * RealHeight + 0x2600) / 0x126
namespace
{
	SharedFilePtr modelFile;
	SharedFilePtr clsnFile;
	SharedFilePtr stubModelFile;
				   
	FixedSizeCLPS_Block<1> clpsBlock =
	{
		{'C','L','P','S'},
		8,
		1,
		{
			CLPS(0x00, 0, 0x3f, 0x0, 0x0, 0x00, 0, 0, 0, 0xff)
		}
	};
}
	
SpawnInfo<PipeVH> PipeVH::spawnData =
{
	&PipeVH::Spawn,
	0x002e,
	0x00c3,
	0x00000002,
	0x000c8000_f,
	0x00320000_f,
	0x01000000_f,
	0x01000000_f
};

char blocksRunOver = 0;

const int log2ClsnHeight = 8; //clsnHeight = 8192 in Q20.12

PipeVH* PipeVH::Spawn()
{
	return new PipeVH;
}

void PipeVH::UpdateClsn()
{
	clsnNextMat = model.mat4x3;
	
	clsnNextMat.r0c3 = pos.x;
	clsnNextMat.r1c3 = pos.y;
	clsnNextMat.r2c3 = pos.z;
	
	clsn.Transform(clsnNextMat, ang.y);
}

void PipeVH::SetClsnHeight()
{
	Fix12i realClsnHeight = height / 0x126 * 0x100 + 0x2600000_f / 0x126;
	
	Fix12i quotient = 0x1000000_f / realClsnHeight;
	invMat4x3_1a8.r1c1 = quotient >> 4;
	scMat4x3_1d8.r1c1 = realClsnHeight >> 12;
	invMat4x3_208.r1c1 = quotient >> 4;
	ledgeMat.r1c1 = realClsnHeight >> 12;
	clsnInvMat.r1c1 = quotient;
	sc2InvMat4x3_2bc.r1c1 = quotient;
}

void PipeVH::UpdateStubModel()
{
	stub.mat4x3.ThisFromRotationY(ang.y);
	stub.mat4x3.r0c3 = pos.x >> 3;
	stub.mat4x3.r1c3 = pos.y >> 3;
	stub.mat4x3.r2c3 = pos.z >> 3;
}

void PipeVH::UpdateShadowMatrix()
{
	shadowMat.ThisFromRotationY(ang.y);
	shadowMat.r0c3 =  pos.x >> 3;
	shadowMat.r1c3 = (pos.y + 0x14000_f) >> 3;
	shadowMat.r2c3 =  pos.z >> 3;
}

int PipeVH::InitResources()
{
	height = Fix12i(param1);
	stubbed = false;
	
	char* modelF = Model::LoadFile(modelFile);
	model.SetFile(modelF, 1, -1);
	model.data.bones[1].pos.y = height >> 3;
	
	modelF = Model::LoadFile(stubModelFile);
	stub.SetFile(modelF, 1, -1);
	shadow.InitCylinder();
	
	UpdateModelPosAndRotY();
	UpdateClsn();
	UpdateShadowMatrix();
	UpdateStubModel();
	
	char* clsnF = MovingMeshCollider::LoadFile(clsnFile);
	clsn.SetFile(clsnF, clsnNextMat, 0x0100_f, ang.y, clpsBlock);
	
	SetClsnHeight();
	
	rangeOffsetY = clsn.rangeOffsetY = height >> 1;
	clsn.range = (height >> 1) + 0x320000_f;
	rangeAsr3 = clsn.range;
	
	return 1;
}

int PipeVH::CleanupResources()
{
	if(clsn.IsEnabled())
		clsn.Disable();
	
	modelFile.Release();
	clsnFile.Release();
	stubModelFile.Release();
	
	return 1;
}

int PipeVH::Behavior()
{
	if(stubbed || UpdateKillByMegaChar(0, 0, -0x1400, 0x100000_f))
		return 1;
	
	DropShadowRadHeight(shadow, shadowMat, 0x96000_f, 0x12c000_f, 0xf);
	IsClsnInRangeOnScreen(rangeAsr3, 0_f);
	/*if(IsClsnInRangeOnScreen(range, 0))
		UpdateClsn();*/
	
	return 1;
}

int PipeVH::Render()
{
	if(!stubbed)
	{
		model.data.UpdateVertsUsingBones();
		model.Render(nullptr);
	}
	
	stub.Render(nullptr);
	return 1;
}

PipeVH::~PipeVH() {}

void PipeVH::OnHitByMegaChar(Actor& actor)
{
	if(actor.actorID != 0x00bf) return;
	
	Player& player = *(Player*)&actor;
	player.IncMegaKillCount();
	Sound::PlayBank3(0x1e, camSpacePos);
	KillByMegaChar(player);
}

void PipeVH::Kill()
{
	Vector3 vec = pos;
	vec.y += height >> 1;
	
	Particle::System::NewSimple(0x122, vec.x, vec.y, vec.z);
	Particle::System::NewSimple(0x123, vec.x, vec.y, vec.z);
	DisappearPoofDustAt(vec);
	
	Sound::PlayBank3(0x41, camSpacePos);
	
	stubbed = true;
}

void init()
{
	OBJ_TO_ACTOR_ID_TABLE[0x0148] = 0x016a;
	ACTOR_SPAWN_TABLE[0x016a] = (unsigned)&PipeVH::spawnData;
	modelFile    .Construct(0x0810);
	clsnFile     .Construct(0x0811);
	stubModelFile.Construct(0x0812);
}

void cleanup()
{
	
}
