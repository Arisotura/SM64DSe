#include "Spill.h"
#include "YBGSpecifics.h"

namespace
{
	
	FixedSizeCLPS_Block<1> clpsBlock =
	{
		{'C', 'L', 'P', 'S'},
		8,
		1,
		{
			CLPS(0x00, 1, 0x3f, 0x0, 0x0, 0x00, 0, 0, 0, 0xff)
		}
	};
	
	const char water0MatName[] = "Water-material";
	const char water1MatName[] = "Water_001-material";
	
	Fix12i waterScales[] = {0x1000_f};
	short waterRots[] = {0};
	Fix12i waterTranss[] = {0x0000_f, 0x0d55_f, 0x0aab_f,
							0x0800_f, 0x0555_f, 0x02ab_f,
							0x0000_f, 0x0d55_f, 0x0aab_f,
							0x0800_f, 0x0555_f, 0x02ab_f,
							0x0000_f, 0x0d55_f, 0x0aab_f,
							0x0800_f, 0x0555_f, 0x02ab_f,
							0x0000_f, 0x0d55_f, 0x0aab_f,
							0x0800_f, 0x0555_f, 0x02ab_f,
							0x0000_f, 0x0d55_f, 0x0aab_f,
							0x0800_f, 0x0555_f, 0x02ab_f,
							0x0000_f, 0x0d55_f, 0x0aab_f,
							0x0800_f, 0x0555_f, 0x02ab_f,
							0x0000_f, 0x0d55_f, 0x0aab_f,
							0x0800_f, 0x0555_f, 0x02ab_f,
							0x0000_f, 0x0d55_f, 0x0aab_f,
							0x0800_f, 0x0555_f, 0x02ab_f,
							
							0x0000_f, 0x0055_f, 0x00ab_f,
							0x0100_f, 0x0155_f, 0x01ab_f,
							0x0200_f, 0x0255_f, 0x02ab_f,
							0x0300_f, 0x0355_f, 0x03ab_f,
							0x0400_f, 0x0455_f, 0x04ab_f,
							0x0500_f, 0x0555_f, 0x05ab_f,
							0x0600_f, 0x0655_f, 0x06ab_f,
							0x0700_f, 0x0755_f, 0x07ab_f,
							0x0800_f, 0x0855_f, 0x08ab_f,
							0x0900_f, 0x0955_f, 0x09ab_f,
							0x0a00_f, 0x0a55_f, 0x0aab_f,
							0x0b00_f, 0x0b55_f, 0x0bab_f,
							0x0c00_f, 0x0c55_f, 0x0cab_f,
							0x0d00_f, 0x0d55_f, 0x0dab_f,
							0x0e00_f, 0x0e55_f, 0x0eab_f,
							0x0f00_f, 0x0f55_f, 0x0fab_f};
	
	
	TexSRTAnim waterAnims[] =
	{
		TexSRTAnim
		{
			0xffff,
			0,    //probably just padding
			&water0MatName[0],
			1, //numScaleXs
			0, //scaleXOffset
			1, //numScaleYs
			0, //scaleYOffset
			1, //numRots
			0, //rotOffset
			1, //numTransXs
			0, //transXOffset
			48, //numTransYs
			48  //transYOffset
		},
		
		TexSRTAnim
		{
			0xffff,
			0,    //probably just padding
			&water1MatName[0],
			1, //numScaleXs
			0, //scaleXOffset
			1, //numScaleYs
			0, //scaleYOffset
			1, //numRots
			0, //rotOffset
			1, //numTransXs
			0, //transXOffset
			48, //numTransYs
			0  //transYOffset
		}
	};

	TexSRTDef waterSRTDef
	{
		48,
		&waterScales[0],
		&waterRots[0],
		&waterTranss[0],
		2,
		&waterAnims[0]
	};
}

SharedFilePtr YBG_Spill::modelFile;
SharedFilePtr YBG_Spill::clsnFile;
SharedFilePtr YBG_Spill::animFiles[YBG_Spill::NUM_ANIMS];

SpawnInfo<YBG_Spill> YBG_Spill::spawnData =
{
	&YBG_Spill::Spawn,
	0x0029,
	0x00cc,
	0x00000002,
	0x00000000_f,
	0x009c4000_f,
	0x01000000_f,
	0x01000000_f,
};

YBG_Spill* YBG_Spill::Spawn()
{
	return new YBG_Spill;
}

void YBG_Spill::UpdateModelTransform()
{
	rigMdl.mat4x3.ThisFromRotationY(ang.y);
	rigMdl.mat4x3.r0c3 = pos.x >> 3;
	rigMdl.mat4x3.r1c3 = pos.y >> 3;
	rigMdl.mat4x3.r2c3 = pos.z >> 3;
}

int YBG_Spill::InitResources()
{
	char* modelF = Model::LoadFile(modelFile);
	for(int i = 0; i < NUM_ANIMS; ++i)
		BoneAnimation::LoadFile(animFiles[i]);
	
	rigMdl.SetFile(modelF, 1, -1);
	rigMdl.SetAnim(animFiles[STOP_LEAK].filePtr, Animation::NO_LOOP, 0x1000_f, 0);
	
	TextureTransformer::Prepare(modelFile.filePtr, waterSRTDef);
	texSRT.SetFile(waterSRTDef, Animation::LOOP, 0x1000_f, 0);
	
	UpdateModelTransform();
	UpdateClsnPosAndRot();
	
	char* clsnF = MovingMeshCollider::LoadFile(clsnFile);
	clsn.SetFile(clsnF, clsnNextMat, 0x1000_f, ang.y, clpsBlock);
	
	eventID = param1 & 0x1f;
	corked = false;
	
	return 1;
}

int YBG_Spill::CleanupResources()
{
	if(clsn.IsEnabled())
	{
		clsn.Disable();
	}
	
	for(int i = 0; i < NUM_ANIMS; ++i)
		animFiles[i].Release();
	modelFile.Release();
	clsnFile.Release();
	return 1;
}

int YBG_Spill::Behavior()
{
	IsClsnInRange(0_f, 0_f);
	if(Event::GetBit(eventID))
		corked = true;
	else
		soundID = Sound::PlayLong(soundID, 0x3, 0x81, camSpacePos, 0);
	if(corked)
		rigMdl.anim.Advance();
	texSRT.Advance();
	return 1;
}

int YBG_Spill::Render()
{
	texSRT.Update(rigMdl.data);
	rigMdl.Render(nullptr);
	return 1;
}

YBG_Spill::~YBG_Spill() {}