#include "SM64DS.h"
#include "PositionAndRotationHelper.h"

///////////////////////////////////////////////////////////////////////////////////////////////////
// POSITION AND ROTATION UPDATE EXAMPLE	
// 
// Some objects in the game were not meant to have their postions and/or rotations updated after 
// they've been loaded by the game, eg. Bowser's Submarine (object 106). In such cases there are 
// two additional steps we need to take: 
// 		1) Tell the game to update the object's model and collision map to our new position 
// 		   and/or rotation.
// 		2) Tell the game that if Mario is standing on/hanging from etc. the object whilst 
// 		   the object is being moved to update Mario's position accordingly.
// 
// Step 1 is achieved by calling methods UpdateObjectModelRotation(...) and 
// UpdateObjectCollisionRotation after updating the object's postion/rotation. Below shows an 
// exmaple of how to achieve this.
// 
// Step 2 is achieved by setting the function address pointed to at +0x13C within the object to 
// 0x0203929C.
///////////////////////////////////////////////////////////////////////////////////////////////////

unsigned int OBJ_106_ObjectAddress = 0xFFFFFFFF;

//////////////////////////// WL_SUBMARINE ////////////////////////////

// OBJ 106: 
// - Ensure that object always loads and isn't dependent on Stars acquired
void repl_02111A30_ov_1A()
{
	asm
	(
		"cmp r0, r0		\t\n"
	);
}

// OBJ 106: 
// - Change method value at +0x13C to ensure that Mario's position is updated when 
//   object's position and rotation are updated
void hook_02111A4C_ov_1A()
{
	asm
	(
		"ldr r2, =OBJ_106_ObjectAddress		\t\n"
		"str r4, [r2]						\t\n"
	);
	
	SetMarioPositionUpdate(OBJ_106_ObjectAddress);
}

// OBJ 106: 
// - Rotate and move object
// - Update model and collision maps based on rotation and position
void hook_021119B0_ov_1A()
{
	asm
	(
		"ldr r1, =OBJ_106_ObjectAddress		\t\n"
		"str r5, [r1]						\t\n"
	);
	
	if (GAME_PAUSED)
	{
		return;
	}
	
	*((volatile short int*)(OBJ_106_ObjectAddress + OBJ_X_ROT_OFFSET)) += 0x80;
	*((volatile short int*)(OBJ_106_ObjectAddress + OBJ_Y_ROT_OFFSET)) += 0x80;
	*((volatile short int*)(OBJ_106_ObjectAddress + OBJ_Z_ROT_OFFSET)) += 0x80;
	
	*((volatile int*)(OBJ_106_ObjectAddress + OBJ_X_LOC_OFFSET)) += 0x800;
	*((volatile int*)(OBJ_106_ObjectAddress + OBJ_Y_LOC_OFFSET)) += 0x800;
	*((volatile int*)(OBJ_106_ObjectAddress + OBJ_Z_LOC_OFFSET)) += 0x800;
	
	UpdateObjectPositionAndRotation(OBJ_106_ObjectAddress, 0xF0, 0x114);
}
