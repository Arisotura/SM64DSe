#include "(_FileName).h"

namespace
{
	FixedSizeCLPS_Block<(_NumCLPSes)> clpsBlock =
	{
		{'C', 'L', 'P', 'S'},
		8,
		(_NumCLPSes),
		(_CLPS)
	};
}

SharedFilePtr (_Name)::modelFile;
SharedFilePtr (_Name)::clsnFile;

SpawnInfo<(_Name)> (_Name)::spawnData =
{
	&(_Name)::Spawn,
	(_BehaviorPriority),
	(_RenderPriority),
	(_Flags),
	(_ClsnRangeOffsetY),
	(_ClsnRange),
	(_DrawDistance),
	(_ShadowDrawDistance)
};

(_Name)* (_Name)::Spawn()
{
	return new (_Name);
}

void (_Name)::UpdateModelTransform()
{
	model.mat4x3.ThisFromRotationY(ang.y);
	model.mat4x3.r0c3 = pos.x >> 3;
	model.mat4x3.r1c3 = pos.y >> 3;
	model.mat4x3.r2c3 = pos.z >> 3;
}

#ifdeftmpl _CreateUpdateClsnFunc
void (_Name)::UpdateClsnTransform()
{
	clsnNextMat.ThisFromRotationY(ang.y);
	clsnNextMat.r0c3 = pos.x;
	clsnNextMat.r1c3 = pos.y;
	clsnNextMat.r2c3 = pos.z;
	
	clsn.Transform(clsnNextMat, ang.y);
}
#definetmpl _ClsnUpdateFunc UpdateClsnTransform
#elsetmpl
#definetmpl _ClsnUpdateFunc UpdateClsnPosAndRot
#endiftmpl

int (_Name)::InitResources()
{
	char* modelF = Model::LoadFile(modelFile);
	model.SetFile(modelF, 1, -1);
	
	UpdateModelTransform();
	(_ClsnUpdateFunc)();
	
	char* clsnF = MovingMeshCollider::LoadFile(clsnFile);
	clsn.SetFile(clsnF, clsnNextMat, (_ClsnScale), ang.y, clpsBlock);
	
	clsn.beforeClsnCallback = (decltype(clsn.beforeClsnCallback))0x02039348;
	
	return 1;
}

int (_Name)::CleanupResources()
{
	clsn.Disable();
	modelFile.Release();
	clsnFile.Release();
	return 1;
}

int (_Name)::Behavior()
{
#ifdeftmpl _Moving
	UpdateModelTransform();
	if(IsClsnInRange(0_f, 0_f))
	{
		(_ClsnUpdateFunc)();
	}
#elsetmpl
	IsClsnInRange(0_f, 0_f);
#endiftmpl
	
	return 1;
}

int (_Name)::Render()
{
	model.Render(nullptr);
	return 1;
}

(_Name)::~(_Name)() {}
#ifndeftmpl _LevelSpecific

void init()
{
	OBJ_TO_ACTOR_ID_TABLE[(_ObjectID)] = (_ActorID);
	ACTOR_SPAWN_TABLE[(_ActorID)] = (unsigned)&(_Name)::spawnData;
	(_Name)::modelFile.Construct((_ModelOv0ID));
    (_Name)::clsnFile .Construct((_ClsnOv0ID));
}

void cleanup()
{
	
	
}
#endiftmpl