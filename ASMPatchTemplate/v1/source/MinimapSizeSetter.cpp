#include "SM64DS.h"

// One bit for each of the 52 levels: 1 for 256x256, 0 for 128x128
unsigned int LEVEL_MINIMAP_SIZES[2] = { 0xFFFFFFFF, 0xFFFFFFFF }; 

unsigned int Map_c = 0xFFFFFFFF;
unsigned int DMA_Related = 0x0400100E;
void hook_020FB698_ov_02() 
{
	asm
	(
		"ldr r0, =Map_c				\t\n"
		"str r6, [r0]				\t\n"
		"ldr r0, =DMA_Related		\t\n"
		"str r2, [r0]				\t\n"
	);
	
	byte currentLevelMinimapSize = ((byte)(LEVEL_MINIMAP_SIZES[CurrentLevelID >> 5] >> CurrentLevelID) & 0x01);
	
	if (currentLevelMinimapSize == 0x00) 
	{
		// 128x128
		*((volatile unsigned int*)(Map_c + 0x1D8)) = 0x00000080;
		*((volatile unsigned int*)(Map_c + 0x1DC)) = 0x00000040;
		*((volatile unsigned int*)(Map_c + 0x1E0)) = 0x00000000;
		*((volatile unsigned int*)(Map_c + 0x1E4)) = 0x00000000;
		*((volatile unsigned int*)(Map_c + 0x1E8)) = 0x00000000;
		*((volatile byte*)(Map_c + 0x251)) = 0x01;
		*((volatile unsigned short int*)(DMA_Related)) = 0x1F12;
	}
	else if (currentLevelMinimapSize == 0x01) 
	{
		// 256 x 256
		*((volatile unsigned int*)(Map_c + 0x1D8)) = 0x00000100;
		*((volatile unsigned int*)(Map_c + 0x1DC)) = 0x00000080;
		*((volatile unsigned int*)(Map_c + 0x1E0)) = 0xFFD44000;
		*((volatile unsigned int*)(Map_c + 0x1E4)) = 0x00000000;
		*((volatile unsigned int*)(Map_c + 0x1E8)) = 0x00000000;
		*((volatile byte*)(Map_c + 0x251)) = 0x02;
		*((volatile unsigned short int*)(DMA_Related)) = 0x5F12;
	}
}

// Disable all of the level-specific minimap changes eg. the ship in 
// JRB or the fortress in WF.
void repl_020F99B4_ov_02() 
{
	asm
	(
		"mov r0, #0x00		\t\n"
	);
}
