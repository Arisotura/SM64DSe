#include "Garden.h"

namespace
{
	
}

SharedFilePtr YBG_Garden::modelFile;

SpawnInfo<YBG_Garden> YBG_Garden::spawnData =
{
	&YBG_Garden::Spawn,
	0x003c,
	0x0011,
	0x00000002,
	0x00000000_f,
	0x005c0000_f,
	0x08000000_f,
	0x08000000_f
};

YBG_Garden* YBG_Garden::Spawn()
{
	return new YBG_Garden;
}

void YBG_Garden::UpdateModelTransform()
{
	model.mat4x3.ThisFromRotationY(ang.y);
	model.mat4x3.r0c3 = pos.x >> 3;
	model.mat4x3.r1c3 = pos.y >> 3;
	model.mat4x3.r2c3 = pos.z >> 3;
}


int YBG_Garden::InitResources()
{
	if(STAR_ID == 1)
		return 0;
	
	char* modelF = Model::LoadFile(modelFile);
	model.SetFile(modelF, 1, -1);
	UpdateModelTransform();
	return 1;
}

int YBG_Garden::CleanupResources()
{
	modelFile.Release();
	return 1;
}

int YBG_Garden::Behavior()
{
	return 1;
}

int YBG_Garden::Render()
{
	model.Render(nullptr);
	return 1;
}

YBG_Garden::~YBG_Garden() {}
