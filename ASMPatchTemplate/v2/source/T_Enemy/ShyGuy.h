#ifndef SHY_GUY_INCLUDED
#define SHY_GUY_INCLUDED

#include "../SM64DS_2.h"

namespace ShyGuy
{
	class ShyGuy : public Enemy
	{
	public:
		static const unsigned SIZE = 0x400;
		
		CylinderClsn& cylClsn() {return *(CylinderClsn*)(this + 0x180);}
		WithMeshClsn& wmClsn() {return *(WithMeshClsn*)(this + 0x1b4);}
		ModelAnim& rigMdl() {return *(ModelAnim*)(this + 0x370);}
		ShadowVolume& shadow() {return *(ShadowVolume*)(this + 0x3d4);}
		uint8_t& state() {return *(uint8_t*)(this + 0x3fc);}
		short& targetAngle() {return *(short*)(this + 0x3fe);}
	};

	ShyGuy* Spawn();
	bool InitResources(ShyGuy* ptr);
	bool CleanupResources(ShyGuy* ptr);
	bool Behavior(ShyGuy* ptr);
	bool Func24(ShyGuy* ptr);
	void OnKill(ShyGuy* ptr);
	ShyGuy* Destructor(ShyGuy* ptr);
	ShyGuy* DestructAndFree(ShyGuy* ptr);
	int OnYoshiTryEat(ShyGuy* ptr);
	void OnTurnIntoEgg(ShyGuy* ptr, Player* player);
	int OnAimedAtWithEgg(ShyGuy* ptr); //returns egg height
	
	void State0_Wait(ShyGuy* ptr);
	void State1_Turn(ShyGuy* ptr);

	extern SharedFilePtr modelFile;
	enum Animations
	{
		WAIT,
		
		NUM_ANIMS
	};
	extern SharedFilePtr animFiles[];
	extern unsigned vtable[];
	extern unsigned spawnData[];

	using StateFuncPtr = void(*)(ShyGuy*);

};
#endif