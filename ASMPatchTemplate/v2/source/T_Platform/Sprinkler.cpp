#include "Sprinkler.h"

namespace YBG_Sprinkler
{
	CLPS_Static<1> CLPS =
	{
		{'C', 'L', 'P', 'S'},
		8,
		1,
		CLPS_Entry_Static<0x00, 0, 0x3f, 0x0, 0x0, 0x00, 0, 0, 0, 0xff>::val
	};

	SharedFilePtr modelFile = {0x03d9, 0, nullptr};
	SharedFilePtr clsnFile  = {0x03da, 0, nullptr};

	unsigned vtable[] =
	{
		(unsigned)&InitResources,
		0x02011268,
		0x02011244,
		(unsigned)&CleanupResources,
		0x02011220,
		0x02011214,
		(unsigned)&Behavior,
		0x02010fd4,
		0x02010fc8,
		(unsigned)&Func24,
		0x02010f78,
		0x02010f6c,
		0x02043ac0,
		0x0204357c,
		0x0204349c,
		0x02043494,
		(unsigned)&Destructor,
		(unsigned)&DestructAndFree,
		0x02010160,
		0x02010154,
		0x0201014c,
		0x02010148,
		0x02010144,
		0x02010140,
		0x0201013c,
		0x02010138,
		0x02010134,
		0x02010130,
		0x0201012c,
		0x02010124,
		0x020100dc,
		0x020ee55c
		
	};
	
	const int CLSN_SCALE = 0x200;
	
	const unsigned ACTOR_FLAGS = 0x00000002;
	const int CLSN_RANGE_OFFSET_Y = 0x00032000;
	const int CLSN_RANGE = 0x00100000;
	const int DRAW_DIST = 0x01000000;

	unsigned spawnData[] =
	{
		(unsigned)&Spawn,
		0x00cd0034,
		ACTOR_FLAGS,
		CLSN_RANGE_OFFSET_Y,
		CLSN_RANGE,
		DRAW_DIST,
		0x01000000, //???
		0x00000001
	};

	Sprinkler* Spawn()
	{
		Sprinkler* ptr = (Sprinkler*)AllocateSpace(Sprinkler::SIZE);
		if(ptr)
		{
			Platform_Construct(ptr);
			ptr->vTable() = (unsigned*)&vtable[0];
		}
		
		return ptr;
	}

	bool InitResources(Sprinkler* ptr)
	{
		char* modelF = LoadModel(&modelFile);
		Model_SetFile(&ptr->model(), modelF, 1, -1);
		
		Platform_UpdateModelPosAndRotY(ptr);
		Platform_UpdateClsnPosAndRot(ptr);
		
		char* clsnF = LoadClsn(&clsnFile);
		Clsn_SetFile(&ptr->clsn(), clsnF, &ptr->clsnNextMat(), CLSN_SCALE, ptr->ang().y, (char*)&CLPS);
		
		ptr->clsn().unkFunc18() = 0x02039348;
		
		return true;
	}

	bool CleanupResources(Sprinkler* ptr)
	{
		if(Clsn_IsEnabled(&ptr->clsn()))
		{
			Clsn_Disable(&ptr->clsn());
		}
		ReleaseSharedFile(&modelFile);
		ReleaseSharedFile(&clsnFile);
		return true;
	}

	bool Behavior(Sprinkler* ptr)
	{
		Platform_UpdateModelPosAndRotY(ptr);
		if(Platform_IsClsnInRange(ptr, 0, 0))
		{
			Platform_UpdateClsnPosAndRot(ptr);
		}
		
		return true;
	}

	bool Func24(Sprinkler* ptr)
	{
		using ModelFunc14 = void(*)(Model*, Vector3* scale);
		ModelFunc14 modelFunc = (ModelFunc14)ptr->model().vTable()[5];
		modelFunc(&ptr->model(), 0);
		return true;
	}

	Sprinkler* Destructor(Sprinkler* ptr)
	{
		Clsn_Destruct(&ptr->clsn());
		Model_Destruct(&ptr->model());
		Actor_Destruct(ptr);
		
		return ptr;
	}

	Sprinkler* DestructAndFree(Sprinkler* ptr)
	{
		Destructor(ptr);
		FreeHeapAllocation(ptr, *(unsigned**)0x020a0eac);
		
		return ptr;
	}
}