#include "MegaBlock.h"
#include "BlockyBlock2Specifics.h"

namespace
{
				   
	FixedSizeCLPS_Block<1> clpsBlock =
	{
		{'C', 'L', 'P', 'S'},
		8,
		1,
		{
			CLPS(0x00, 0, 0x3f, 0x0, 0x0, 0x00, 0, 0, 0, 0xff)
		}
	};
	
	char blocksRunOver = 0;
}

SharedFilePtr BBL2_MegaBlock::modelFile;
SharedFilePtr BBL2_MegaBlock::clsnFile;
	
SpawnInfo<BBL2_MegaBlock> BBL2_MegaBlock::spawnData =
{
	&BBL2_MegaBlock::Spawn,
	0x002e,
	0x00c3,
	0x00000002,
	0x000c8000_f,
	0x00320000_f,
	0x01000000_f,
	0x01000000_f
};

BBL2_MegaBlock* BBL2_MegaBlock::Spawn()
{
	return new BBL2_MegaBlock;
}

void BBL2_MegaBlock::UpdateShadowMatrix()
{
	shadowMat.ThisFromRotationY(ang.y);
	shadowMat.r0c3 = pos.x >> 3;
	shadowMat.r1c3 = pos.y >> 3;
	shadowMat.r2c3 = pos.z >> 3;
}

int BBL2_MegaBlock::InitResources()
{
	char* modelF = Model::LoadFile(modelFile);
	model.SetFile(modelF, 1, -1);
	shadow.InitCuboid();
	
	UpdateModelPosAndRotY();
	UpdateClsnPosAndRot();
	UpdateShadowMatrix();
	
	char* clsnF = MovingMeshCollider::LoadFile(clsnFile);
	clsn.SetFile(clsnF, clsnNextMat, 0x1000_f, ang.y, clpsBlock);
	
	Model::LoadFile(SILVER_NUMBER_MODEL_PTR);
	TextureSequence::LoadFile(SILVER_NUMBER_TEXSEQ_PTR);
	
	return 1;
}

int BBL2_MegaBlock::CleanupResources()
{
	if(clsn.IsEnabled())
		clsn.Disable();
	
	modelFile.Release();
	clsnFile.Release();
	
	SILVER_NUMBER_MODEL_PTR.Release();
	SILVER_NUMBER_TEXSEQ_PTR.Release();
	
	return 1;
}

int BBL2_MegaBlock::Behavior()
{
	if(UpdateKillByMegaChar(0, 0, 0, 0x100000_f))
		return 1;
	
	clsn.rangeOffsetY = 0xc8000_f;
	clsn.range = 0x320000_f;
	
	DropShadowScaleXYZ(shadow, shadowMat, 0x190000_f, 0x190000_f, 0x190000_f, 0xf);
	IsClsnInRangeOnScreen(0x600000_f, 0_f);
	
	return 1;
}

int BBL2_MegaBlock::Render()
{
	model.Render(nullptr);
	return 1;
}

BBL2_MegaBlock::~BBL2_MegaBlock() {}

void BBL2_MegaBlock::OnHitByMegaChar(Actor& actor)
{
	if(actor.actorID != 0x00bf) return;
	
	Player& player = *(Player*)&actor;
	player.IncMegaKillCount();
	Sound::PlayBank3(0x1e, camSpacePos);
	KillByMegaChar(player);
}

void BBL2_MegaBlock::Kill()
{
	Vector3 vec = pos;
	vec.y += 0xc8000_f;
	
	Particle::System::NewSimple(0x122, vec.x, vec.y, vec.z);
	Particle::System::NewSimple(0x123, vec.x, vec.y, vec.z);
	DisappearPoofDustAt(vec);
	
	Sound::PlayBank3(0x41, camSpacePos);
	Destroy();
	
	++blocksRunOver;
	Actor::Spawn(0x014a, 0x10 | blocksRunOver, vec, nullptr, areaID, -1);
	
	if(blocksRunOver == 5)
		Actor::Spawn(0x00b2, 0x0042, vec, nullptr, 0, -1);
}
