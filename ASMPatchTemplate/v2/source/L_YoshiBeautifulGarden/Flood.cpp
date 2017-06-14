#include "Flood.h"
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
	
	const char waterMatName[] = "Water-material";
	
	Fix12i waterScales[] = {0x1000_f};
	short waterRots[] = {0};
	Fix12i waterTranss[] = {0x0000_f, 0x0055_f, 0x00ab_f,
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
			&waterMatName[0],
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
		1,
		&waterAnims[0]
	};
	
	constexpr Fix12i FAST_DRAIN_SPEED = -0x20000_f;
	constexpr Fix12i SLOW_DRAIN_SPEED = -0x04000_f;
}

SharedFilePtr YBG_Flood::modelFile;
SharedFilePtr YBG_Flood::clsnFile;

SpawnInfo<YBG_Flood> YBG_Flood::spawnData =
{
	&YBG_Flood::Spawn,
	0x0035,
	0x00cc,
	0x00000002,
	0x00000000_f,
	0x02000000_f,
	0x08000000_f,
	0x08000000_f,
};

YBG_Flood* YBG_Flood::Spawn()
{
	return new YBG_Flood;
}

int YBG_Flood::InitResources()
{
	char* modelF = Model::LoadFile(modelFile);	
	model.SetFile(modelF, 1, -1);
	
	TextureTransformer::Prepare(modelFile.filePtr, waterSRTDef);
	texSRT.SetFile(waterSRTDef, Animation::LOOP, 0x1000_f, 0);
	
	UpdateModelPosAndRotY();
	UpdateClsnPosAndRot();
	
	char* clsnF = MovingMeshCollider::LoadFile(clsnFile);
	clsn.SetFile(clsnF, clsnNextMat, 0x1000_f, ang.y, clpsBlock);
	
	eventID = param1 & 0xff;
	eventToTrigger = param1 >> 8;
	
	return 1;
}

int YBG_Flood::CleanupResources()
{
	if(clsn.IsEnabled())
	{
		clsn.Disable();
	}

	modelFile.Release();
	clsnFile.Release();
	return 1;
}

int YBG_Flood::Behavior()
{
	if(moveState == 0 && Event::GetBit(eventID))
	{
		moveState = 1;
		speed.y = FAST_DRAIN_SPEED; //0x2dcb5c
	}
	else if(moveState == 1 && pos.y <= -0x3f8452_f)
	{
		moveState = 2;
		speed.y = SLOW_DRAIN_SPEED;
	}
	else if(moveState == 2 && pos.y <= -0x5ec452_f)
	{
		moveState = 3;
		speed.y = FAST_DRAIN_SPEED;
	}
	else if(moveState == 3 && pos.y <= -0xb80000_f)
	{
		Event::SetBit(eventToTrigger);
		Destroy();
		return 1;
	}
	UpdatePosWithOnlySpeed(nullptr);
	
	UpdateModelPosAndRotY();
	if(IsClsnInRange(0_f, 0_f))
		UpdateClsnPosAndRot();
	texSRT.Advance();
	return 1;
}

int YBG_Flood::Render()
{
	texSRT.Update(model.data);
	model.Render(nullptr);
	return 1;
}

YBG_Flood::~YBG_Flood() {}