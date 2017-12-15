#include "SM64DS.h"

///////////////////////////////////////////////////////////////////////////////////////////////////
// CHANGE LEVEL SETTINGS EXAMPLE
// 
// Sometimes it is desirable to be able to change a level's settings based upon some condition 
// being met. For example, if there were two objects that you wanted to include in different Star 
// missions but due to conflicts in their object bank requirements were unable to, then it is 
// possible to get around this limitation by changing the object bank settings depending on the 
// current Star mission. 
// 
// The below code is executed at 0214EAAC - within the level overlay's initialisation code. The 
// method name "hook_0214EAAC_ov_90" specifies that this method is to be executed at RAM address 
// 0214EAAC whose code is found in overlay 0x90 (144 - Tox Box Test Map). The overlay index may 
// be the index of any of the levels' new overlays (103 - 154) in hexadecimal. 
// 
// The below example shows how to update the level's settings.
///////////////////////////////////////////////////////////////////////////////////////////////////

LevelSettings RequiredLevelSettings = 
{
	{ 0, 0, 0, 0, 4, 0, 0, 0 }, 
	2011, 
	2010, 
	0, 
	0, 
	false, 
	1600, 
	2, 
	{ 3, 32, 255 }, 
	255
};
byte RequiredObjectBank07 = 0x00;

// Change level settings 
void hook_0214EAAC_ov_90()
{
	// Object banks
	for (int i = 0; i < 7; i++)
	{
		*((volatile byte*)(LEVEL_OVERLAY_LOAD_ADDRESS + OBJECT_BANK_0_OFFSET + i)) = 
			RequiredLevelSettings.ObjectBankSettings[i];
	}
	RequiredObjectBank07 = RequiredLevelSettings.ObjectBankSettings[7];
	*((volatile byte*)(LEVEL_OVERLAY_LOAD_ADDRESS + OBJECT_BANK_7_OFFSET)) = 
		RequiredObjectBank07;
	// File ID's
	*((volatile unsigned short*)(LEVEL_OVERLAY_LOAD_ADDRESS + BMD_FILE_ID_OFFSET)) = 
		RequiredLevelSettings.BMD_FileID;
	*((volatile unsigned short*)(LEVEL_OVERLAY_LOAD_ADDRESS + KCL_FILE_ID_OFFSET)) = 
		RequiredLevelSettings.KCL_FileID;
	*((volatile unsigned short*)(LEVEL_OVERLAY_LOAD_ADDRESS + ICG_FILE_ID_OFFSET)) = 
		RequiredLevelSettings.ICG_FileID;
	*((volatile unsigned short*)(LEVEL_OVERLAY_LOAD_ADDRESS + ICL_FILE_ID_OFFSET)) = 
		RequiredLevelSettings.ICL_FileID;
	// Start camera zoomed out
	*((volatile byte*)(LEVEL_OVERLAY_LOAD_ADDRESS + CAMERA_START_ZOOMED_OUT_OFFSET)) = 
		RequiredLevelSettings.CameraStartZoomedOut;
	// Minimap scale factor
	*((volatile unsigned short*)(LEVEL_OVERLAY_LOAD_ADDRESS + MINIMAP_SCALE_OFFSET)) = 
		RequiredLevelSettings.MinimapScaleFactor;
	// Skybox
	byte sbValue = 0x0F | (RequiredLevelSettings.SkyBox << 4);
	*((volatile byte*)(LEVEL_OVERLAY_LOAD_ADDRESS + SKYBOX_OFFSET)) = sbValue;
	// Music settings
	for (int i = 0; i < 3; i++)
	{
		*((volatile byte*)(LEVEL_OVERLAY_LOAD_ADDRESS + MUSIC_SETTING_0_OFFSET + i)) = 
			RequiredLevelSettings.MusicSettings[i];
	}
	
	asm
	(
		"pop {r0}							\t\n"
		"ldr r0, =RequiredObjectBank07		\t\n"
		"ldrb r0, [r0]						\t\n"
		"push {r0}							\t\n"
		
		"cmp r0, #0x00						\t\n" // Original instruction
	);
}