#ifndef POSITION_AND_ROTATION
#define POSITION_AND_ROTATION

const unsigned int UPDATE_MARIO_POS_METHOD = 0x0203923C;
const unsigned int UPDATE_MARIO_POS_ON_ROT_METHOD = 0x0203929C;

// Set the object to update Mario's position when its own position changes
void SetMarioPositionUpdate(unsigned int object_address);

// Set values at position_asr3_offset to the object's X, Y and Z positions 
// shifted right thrice
void SetObjectPositionASR3(unsigned int object_address, 
						   short int position_asr3_offset);

// Update only the object's model's position and rotation 
void UpdateObjectModelPositionAndRotation(unsigned int object_address, 
										  unsigned short model_rotation_offset, 
										  short int position_asr3_offset);

// Update only the object's collision's position and rotation 
void UpdateObjectCollisionPositionAndRotation(unsigned int object_address, 
											  short int position_asr3_offset);

// Update both the object's model's and collision's position and rotation 
void UpdateObjectPositionAndRotation(unsigned int object_address, 
									 unsigned short model_rotation_offset, 
									 short int position_asr3_offset);

// Calculate and set the object's next position based on its X, Y and Z speeds
void UpdatePositionFromSpeed(unsigned int object_address); 
#endif