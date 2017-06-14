#include "PushPipe.h"
#include "YBGSpecifics.h"

namespace
{
	CLPS_Block clpsBlock =
	{
		{'C', 'L', 'P', 'S'},
		8,
		1,
		CLPS(0x00, 0, 0x3f, 0x0, 0x0, 0x00, 0, 0, 0, 0xff)
	};

	SharedFilePtr modelFile = {0x03db, 0, nullptr};
	SharedFilePtr clsnFile  = {0x03dc, 0, nullptr};
}

SpawnInfo<YBG_PushPipe> YBG_PushPipe::spawnData =
{
	&YBG_PushPipe::Spawn,
	0x0027,
	0x00cc,
	0x00000002,
	0x00118000_f,
	0x00320000_f,
	0x01000000_f,
	0x01000000_f,
};

YBG_PushPipe* YBG_PushPipe::Spawn()
{
	return new YBG_PushPipe;
}

void YBG_PushPipe::UpdateModelTransform()
{
	model.mat4x3.ThisFromRotationY(ang.y);
	model.mat4x3.r0c3 = pos.x >> 3;
	model.mat4x3.r1c3 = pos.y >> 3;
	model.mat4x3.r2c3 = pos.z >> 3;
}

void YBG_PushPipe::UpdateClsnTransform()
{
	clsnNextMat.ThisFromRotationY(ang.y);
	clsnNextMat.r0c3 = pos.x;
	clsnNextMat.r1c3 = pos.y;
	clsnNextMat.r2c3 = pos.z;
	
	clsn.Transform(clsnNextMat, ang.y);
}

int YBG_PushPipe::InitResources()
{
	char* modelF = Model::LoadFile(modelFile);
	model.SetFile(modelF, 1, -1);
	
	UpdateModelTransform();
	UpdateClsnTransform();
	
	char* clsnF = MovingMeshCollider::LoadFile(clsnFile);
	clsn.SetFile(clsnF, clsnNextMat, 0x1000_f, ang.y, clpsBlock);
	
	clsn.unkFunc18 = (decltype(clsn.unkFunc18))0x02039348;
	
	return 1;
}

int YBG_PushPipe::CleanupResources()
{
	if(clsn.IsEnabled())
	{
		clsn.Disable();
	}
	modelFile.Release();
	clsnFile.Release();
	return 1;
}

int YBG_PushPipe::Behavior()
{
	UpdateModelTransform();
	if(IsClsnInRange(0_f, 0_f))
	{
		UpdateClsnTransform();
	}
	
	return 1;
}

int YBG_PushPipe::Render()
{
	model.Render(nullptr);
	return 1;
}

YBG_PushPipe::~YBG_PushPipe() {}