#include "ExtraLava.h"

namespace
{
	FixedSizeCLPS_Block<1> clpsBlock =
	{
		{'C', 'L', 'P', 'S'},
		8,
		1,
		{
        	CLPS(0x00, 0, 0x3f, 0x0, 0x0, 0x01, 0, 0, 0, 0xff)
        }
	};
	
	const char lavaMatName[] = "Lava_mat";
	
	Fix12i lavaScales[] = {0x1000_f};
	short lavaRots[] = {0};
	Fix12i lavaTranss[] = {0x0000_f, 0x0100_f, 0x0200_f, 0x0300_f,
						   0x0400_f, 0x0500_f, 0x0600_f, 0x0700_f,
						   0x0800_f, 0x0900_f, 0x0a00_f, 0x0b00_f,
						   0x0c00_f, 0x0d00_f, 0x0e00_f, 0x0f00_f};
	
	
	TexSRTAnim lavaAnims[] =
	{
		TexSRTAnim
		{
			0xffff,
			0,    //probably just padding
			&lavaMatName[0],
			1, //numScaleXs
			0, //scaleXOffset
			1, //numScaleYs
			0, //scaleYOffset
			1, //numRots
			0, //rotOffset
			1, //numTransXs
			0, //transXOffset
			16, //numTransYs
			0  //transYOffset
		}
	};

	TexSRTDef lavaSRTDef
	{
		16,
		&lavaScales[0],
		&lavaRots[0],
		&lavaTranss[0],
		1,
		&lavaAnims[0]
	};
}

SharedFilePtr YBG_ExtraLava::modelFile;
SharedFilePtr YBG_ExtraLava::clsnFile;
SharedFilePtr YBG_ExtraLava::texSeqFile;

SpawnInfo<YBG_ExtraLava> YBG_ExtraLava::spawnData =
{
	&YBG_ExtraLava::Spawn,
	0x0036,
	0x0011,
	0x00000002,
	0x00000000_f,
	0x01388000_f,
	0x08000000_f,
	0x08000000_f
};

YBG_ExtraLava* YBG_ExtraLava::Spawn()
{
	return new YBG_ExtraLava;
}

void YBG_ExtraLava::UpdateModelTransform()
{
	model.mat4x3.ThisFromRotationY(ang.y);
	model.mat4x3.r0c3 = pos.x >> 3;
	model.mat4x3.r1c3 = pos.y >> 3;
	model.mat4x3.r2c3 = pos.z >> 3;
}


int YBG_ExtraLava::InitResources()
{
	if(STAR_ID == 1)
		return 0;
	if(STAR_ID > 2)
		clpsBlock.clpses[0].low &= ~(1 << 19); //It hardens after the eruption.
	
	char* modelF = Model::LoadFile(modelFile);
	model.SetFile(modelF, 1, -1);
	
	TextureTransformer::Prepare(modelFile.filePtr, lavaSRTDef);
	texSRT.SetFile(lavaSRTDef, Animation::LOOP, 0x1000_f, 0);
	
	TextureSequence::LoadFile(texSeqFile);
	TextureSequence::Prepare(modelFile.filePtr, texSeqFile.filePtr);
	texSeq.SetFile(texSeqFile.filePtr, Animation::NO_LOOP, 0x1000_f, STAR_ID > 2);
	
	UpdateModelTransform();
	UpdateClsnPosAndRot();
	
	char* clsnF = MovingMeshCollider::LoadFile(clsnFile);
	clsn.SetFile(clsnF, clsnNextMat, 0x1000_f, ang.y, clpsBlock);
	
	return 1;
}

int YBG_ExtraLava::CleanupResources()
{
	clsn.Disable();
	texSeqFile.Release();
	modelFile.Release();
	clsnFile.Release();
	return 1;
}

int YBG_ExtraLava::Behavior()
{
	IsClsnInRange(0_f, 0_f);
	if(STAR_ID == 2)
		texSRT.Advance();
	
	return 1;
}

int YBG_ExtraLava::Render()
{	
	texSeq.Update(model.data);
	texSRT.Update(model.data);
	model.Render(nullptr);
	return 1;
}

YBG_ExtraLava::~YBG_ExtraLava() {}
