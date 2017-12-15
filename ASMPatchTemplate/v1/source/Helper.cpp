#include "SM64DS.h"

MarioActor * PLAYER;

byte Player_CurrentAction;
byte Player_PreviousUniqueAction;

byte PreviousLevelID = 0x00;
byte CurrentLevelID = 0x00;

int TmpThreeIntArray[3] = { 0, 0, 0 };

OAMSettings* OAM_ThinNumbers[10] = 
{
	(OAMSettings*)0x020ABA68, 
	(OAMSettings*)0x020ABA60, 
	(OAMSettings*)0x020ABA58, 
	(OAMSettings*)0x020ABA50, 
	(OAMSettings*)0x020ABA48, 
	(OAMSettings*)0x020ABA40, 
	(OAMSettings*)0x020ABA38, 
	(OAMSettings*)0x020ABA30, 
	(OAMSettings*)0x020ABA28, 
	(OAMSettings*)0x020ABA20
};

/* char msg[8];
 * HexPrint(value, msg);
 * nocashPrint(msg);
*/
void HexPrint(unsigned int value, char *result) 
{
	byte i = 0; 
	byte current = 0xFF;
	while (i < 8) 
	{
		current = (byte)((value >> i * 4) & 0x0F);
		result[7 - i] = (char)(current + ((current < 10) ? 48 : 55));
		i++;
	}
}

// Because the address at which the Player object is stored is not fixed when changes are made to the ROM or within 
// every level, we need to store the address whenever the game uses it so that we can retrieve it and modify the values 
// at that address later.

// The following code being hooked gets called when the game is first loaded and when the Player object's address changes.
void hook_020E6994_ov_02()
{
	asm
	(
		"ldr r0, =PLAYER	\n\t"		// r0 points to PLAYER address
		"str r5, [r0]		\n\t"		// store the value in r5 at the address stored in r0
		"original:			\n\t"
		"mov r0, r5			\n\t"
	);
}

// The following hook is placed just after the start of the Player_PerformAction method and stores the currently 
// executing Action ID
void hook_020BEF38_ov_02()
{
	asm
	(
		"ldr r0, =Player_CurrentAction				\t\n"
		"ldrb r2, [r0]								\t\n"
		"cmp r1, r2									\t\n"
		"beq save_new_act							\t\n"
		"ldr r3, =Player_PreviousUniqueAction		\t\n"
		"strb r2, [r3]								\t\n"
		"save_new_act: 								\t\n"
		"strb r1, [r0]								\t\n"
	);
}

// Update current and previous level ID's
void hook_0202CE18()
{
	asm
	(
		"ldr r2, =PreviousLevelID		\t\n"
		"ldr r3, =CurrentLevelID		\t\n"
		"ldrb r4, [r3]					\t\n"
		"strb r4, [r2]					\t\n"
		"strb r1, [r3]					\t\n"
	);
}
