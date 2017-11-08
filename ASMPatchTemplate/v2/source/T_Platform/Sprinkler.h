#ifndef YBG_SPRINKLER_INCLUDED
#define YBG_SPRINKLER_INCLUDED

#include "../SM64DS_2.h"

namespace YBG_Sprinkler
{
	class Sprinkler : public Platform
	{
	public:
		static const unsigned SIZE = 0x320;
	};

	Sprinkler* Spawn();
	bool InitResources(Sprinkler* ptr);
	bool CleanupResources(Sprinkler* ptr);
	bool Behavior(Sprinkler* ptr);
	bool Func24(Sprinkler* ptr);
	Sprinkler* Destructor(Sprinkler* ptr);
	Sprinkler* DestructAndFree(Sprinkler* ptr);

	extern CLPS_Static<1> CLPS;
	extern SharedFilePtr modelFile, clsnFile;
	extern unsigned vtable[];
	extern unsigned spawnData[];
}

#endif