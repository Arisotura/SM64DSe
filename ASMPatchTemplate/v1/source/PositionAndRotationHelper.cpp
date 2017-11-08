#include "SM64DS.h"
#include "PositionAndRotationHelper.h"
#include "atan2.h"
#include "MathsHelper.h"

void SetMarioPositionUpdate(unsigned int object_address)
{
	*((volatile unsigned int*)(object_address + OBJ_UPDATE_MARIO_POS_OFFSET)) = 
		UPDATE_MARIO_POS_ON_ROT_METHOD;
}

__attribute__((noinline)) void SetObjectPositionASR3(unsigned int object_address, short int position_asr3_offset) 
{
	int xPos_asr3 = (int)(*((volatile int*)(object_address + OBJ_X_LOC_OFFSET)) >> 3);
	int yPos_asr3 = (int)(*((volatile int*)(object_address + OBJ_Y_LOC_OFFSET)) >> 3);
	int zPos_asr3 = (int)(*((volatile int*)(object_address + OBJ_Z_LOC_OFFSET)) >> 3);
	
	*((volatile int*)(object_address + (position_asr3_offset + 0x00))) = xPos_asr3;
	*((volatile int*)(object_address + (position_asr3_offset + 0x04))) = yPos_asr3;
	*((volatile int*)(object_address + (position_asr3_offset + 0x08))) = zPos_asr3;
}

__attribute__((noinline)) void UpdateObjectModelPositionAndRotation(unsigned int object_address, 
																	unsigned short model_rotation_offset, 
																	short int position_asr3_offset) 
{
	short int xRot = *((volatile short int*)(object_address + OBJ_X_ROT_OFFSET));
	short int rot = *((volatile short int*)(object_address + OBJ_Y_ROT_OFFSET));
	short int zRot = *((volatile short int*)(object_address + OBJ_Z_ROT_OFFSET));
	
	OBJ_UpdateObjectModelRotation((unsigned int)(object_address + model_rotation_offset), xRot, rot, zRot);
	SetObjectPositionASR3(object_address, position_asr3_offset);
}

__attribute__((noinline)) void UpdateObjectCollisionPositionAndRotation(unsigned int object_address, 
																		short int position_asr3_offset) 
{
	SetObjectPositionASR3(object_address, position_asr3_offset);
	OBJ_UpdateObjectCollisionRotation(object_address);
}

__attribute__((noinline)) void UpdateObjectPositionAndRotation(unsigned int object_address, 
															   unsigned short model_rotation_offset, 
															   short int position_asr3_offset)
{
	UpdateObjectModelPositionAndRotation(object_address, model_rotation_offset, position_asr3_offset);
	UpdateObjectCollisionPositionAndRotation(object_address, position_asr3_offset);
}

__attribute__((noinline)) void UpdatePositionFromSpeed(unsigned int object_address) 
{
	*((volatile int*)(object_address + OBJ_X_LOC_OFFSET)) += *((volatile int*)(object_address + OBJ_X_SPEED_OFFSET));
	*((volatile int*)(object_address + OBJ_Y_LOC_OFFSET)) += *((volatile int*)(object_address + OBJ_Y_SPEED_OFFSET));
	*((volatile int*)(object_address + OBJ_Z_LOC_OFFSET)) += *((volatile int*)(object_address + OBJ_Z_SPEED_OFFSET));
}