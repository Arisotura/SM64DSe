#ifndef SM64DS
#define SM64DS

#include <nds.h>

#define KEYS 								*((volatile unsigned int*)		0x04000130)
#define KEY_STATES_ALL						*((volatile unsigned short int*)0x020A0E58)
#define CHARACTER 							*((volatile unsigned char*)		0x0209CAE1)
#define HAT_CHARACTER				   		*((volatile unsigned char*)		0x02092128)
#define LEVEL_ID 							*((volatile unsigned char*)		0x0209F2F8)
#define STAR_ID								*((volatile unsigned byte*)		0x0209F220)
#define GAME_PAUSED 						*((volatile unsigned byte*) 	0x0209F2C4)
#define CURRENT_ENTRANCE_LEVEL_LEVEL_ID 	*((volatile unsigned byte*) 	0x0209F1F0)
#define CURRENT_ENTRANCE_LEVEL_ENTRANCE_ID 	*((volatile unsigned byte*) 	0x0209F200)

struct MarioActor
{
	void* vtable;
	u32 unk004;
	u32 unk008;
	u32 unk00C; // camera related, always 0xBF
	u32 unk010;
	u32 unk014;
	u32 unk018;
	u32 unk01C;
	u32 unk020;
	u32 unk024;
	u32 unk028;
	u32 unk02C;
	u32 unk030;
	u32 unk034;
	u32 unk038;
	u32 unk03C;
	u32 unk040;
	u32 unk044;
	u32 unk048;
	u32 unk04C;
	u32 unk050;
	u32 unk054;
	u32 unk058;
	int32 xPos; //0x5C
	int32 yPos;
	int32 zPos;
	u32 unk068;
	u32 unk06C;
	u32 unk070;
	u32 unk074;
	u32 unk078;
	u32 unk07C;
	u32 unk080;
	u32 unk084;
	u32 unk088;
	s16 xRot; // 0x8C
	s16 yRot; // 0x8E
	s16 zRot; // 0x90
	u16 unk092;
	s16 forwardDirection; // 0x94
	u16 unk096;
	int32 forwardSpeed; // 0x98
	u32 unk09C;
	u32 unk0A0;
	int32 xSpeed; // 0xA4
	int32 ySpeed;	// 0xA8
	int32 zSpeed; // 0xAC
	u32 unk0B0;
	u32 unk0B4;
	u32 unk0B8;
	u32 unk0BC;
	u32 unk0C0;
	u32 unk0C4;
	u32 unk0C8;
	u32 unk0CC;
	u32 unk0D0;
	u32 unk0D4;
	u32 unk0D8;
	u32 unk0DC;
	u32 unk0E0;
	u32 unk0E4;
	u32 unk0E8;
	u32 unk0EC;
	u32 unk0F0;
	u32 unk0F4;
	u32 unk0F8;
	u32 unk0FC;
	u32 unk100;
	u32 unk104;
	u32 unk108;
	u32 unk10C;
	u32 unk110;
	u32 unk114;
	u32 unk118;
	u32 unk11C;
	u32 unk120;
	u32 unk124;
	u32 unk128;
	u32 unk12C;
	u32 unk130;
	u32 unk134;
	u32 unk138;
	u32 unk13C;
	u32 unk140;
	u32 unk144;
	u32 unk148;
	u32 unk14C;
	u32 unk150;
	u32 unk154;
	u32 unk158;
	u32 unk15C;
	u32 unk160;
	u32 unk164;
	u32 unk168;
	u32 unk16C;
	u32 unk170;
	u32 unk174;
	u32 unk178;
	u32 unk17C;
	u32 unk180;
	u32 unk184;
	u32 unk188;
	u32 unk18C;
	u32 unk190;
	u32 unk194;
	u32 unk198;
	u32 unk19C;
	u32 unk1A0;
	u32 unk1A4;
	u32 unk1A8;
	u32 unk1AC;
	u32 unk1B0;
	u32 unk1B4;
	u32 unk1B8;
	u32 unk1BC;
	u32 unk1C0;
	u32 unk1C4;
	u32 unk1C8;
	u32 unk1CC;
	u32 unk1D0;
	u32 unk1D4;
	u32 unk1D8;
	u32 unk1DC;
	u32 unk1E0;
	u32 unk1E4;
	u32 unk1E8;
	u32 unk1EC;
	u32 unk1F0;
	u32 unk1F4;
	u32 unk1F8;
	u32 unk1FC;
	u32 unk200;
	u32 unk204;
	u32 unk208;
	u32 unk20C;
	u32 unk210;
	u32 unk214;
	u32 unk218;
	u32 unk21C;
	u32 unk220;
	u32 unk224;
	u32 unk228;
	u32 unk22C;
	u32 unk230;
	u32 unk234;
	u32 unk238;
	u32 unk23C;
	u32 unk240;
	u32 unk244;
	u32 unk248;
	u32 unk24C;
	u32 unk250;
	u32 unk254;
	u32 unk258;
	u32 unk25C;
	u32 unk260;
	u32 unk264;
	u32 unk268;
	u32 unk26C;
	u32 unk270;
	u32 unk274;
	u32 unk278;
	u32 unk27C;
	u32 unk280;
	u32 unk284;
	u32 unk288;
	u32 unk28C;
	u32 unk290;
	u32 unk294;
	u32 unk298;
	u32 unk29C;
	u32 unk2A0;
	u32 unk2A4;
	u32 unk2A8;
	u32 unk2AC;
	u32 unk2B0;
	u32 unk2B4;
	u32 unk2B8;
	u32 unk2BC;
	u32 unk2C0;
	u32 unk2C4;
	u32 unk2C8;
	u32 unk2CC;
	u32 unk2D0;
	u32 unk2D4;
	u32 unk2D8;
	u32 unk2DC;
	u32 unk2E0;
	u32 unk2E4;
	u32 unk2E8;
	u32 unk2EC;
	u32 unk2F0;
	u32 unk2F4;
	u32 unk2F8;
	u32 unk2FC;
	u32 unk300;
	u32 unk304;
	u32 unk308;
	u32 unk30C;
	u32 unk310;
	u32 unk314;
	u32 unk318;
	u32 unk31C;
	u32 unk320;
	u32 unk324;
	u32 unk328;
	u32 unk32C;
	u32 unk330;
	u32 unk334;
	u32 unk338;
	u32 unk33C;
	u32 unk340;
	u32 unk344;
	u32 unk348;
	u32 unk34C;
	u32 unk350;
	u32 unk354;
	u32 unk358;
	u32 unk35C;
	u32 unk360;
	u32 unk364;
	u32 unk368;
	u32 unk36C;
	u32* currentActionAddress_1; // 0x370
	u32* lastActionAddress; // 0x374
	u32* currentActionAddress_2; // 0x378
	u32 unk37C;
	u32 unk380;
	u32 unk384;
	u32 unk388;
	u32 unk38C;
	u8 isOnGround_Related_1_x390;
	u8 isOnGround;
	u8 unk392;
	u8 unk393;
	u32 unk394;
	u32 unk398;
	u32 unk39C;
	u32 unk3A0;
	u32 unk3A4;
	u32 unk3A8;
	u32 unk3AC;
	u32 unk3B0;
	u32 unk3B4;
	u32 unk3B8;
	u32 unk3BC;
	u8 isInWater; // 0x3C0
	u8 unk3C1;
	u8 unk3C2;
	u8 unk3C3;
	u32 unk3C4;
	u32 unk3C8;
	u32 unk3CC;
	u32 unk3D0;
	u32 unk3D4;
	u32 unk3D8;
	u32 unk3DC;
	u32 unk3E0;
	u32 unk3E4;
	u32 unk3E8;
	u32 unk3EC;
	u32 unk3F0;
	u32 unk3F4;
	u32 unk3F8;
	u32 unk3FC;
	u32 unk400;
	u32 unk404;
	u32 unk408;
	u32 unk40C;
	u32 unk410;
	u32 unk414;
	u32 unk418;
	u32 unk41C;
	u32 unk420;
	u32 unk424;
	u32 unk428;
	u32 unk42C;
	u32 unk430;
	u32 unk434;
	u32 unk438;
	u32 unk43C;
	u32 unk440;
	u32 unk444;
	u32 unk448;
	u32 unk44C;
	u32 unk450;
	u32 unk454;
	u32 unk458;
	u32 unk45C;
	u32 unk460;
	u32 unk464;
	u32 unk468;
	u32 unk46C;
	u32 unk470;
	u32 unk474;
	u32 unk478;
	u32 unk47C;
	u32 unk480;
	u32 unk484;
	u32 unk488;
	u32 unk48C;
	u32 unk490;
	u32 unk494;
	u32 unk498;
	u32 unk49C;
	u32 unk4A0;
	u32 unk4A4;
	u32 unk4A8;
	u32 unk4AC;
	u32 unk4B0;
	u32 unk4B4;
	u32 unk4B8;
	u32 unk4BC;
	u32 unk4C0;
	u32 unk4C4;
	u32 unk4C8;
	u32 unk4CC;
	u32 unk4D0;
	u32 unk4D4;
	u32 unk4D8;
	u32 unk4DC;
	u32 unk4E0;
	u32 unk4E4;
	u32 unk4E8;
	u32 unk4EC;
	u32 unk4F0;
	u32 unk4F4;
	u32 unk4F8;
	u32 unk4FC;
	u32 unk500;
	u32 unk504;
	u32 unk508;
	u32 unk50C;
	u32 unk510;
	u32 unk514;
	u32 unk518;
	u32 unk51C;
	u32 unk520;
	u32 unk524;
	u32 unk528;
	u32 unk52C;
	u32 unk530;
	u32 unk534;
	u32 unk538;
	u32 unk53C;
	u32 unk540;
	u32 unk544;
	u32 unk548;
	u32 unk54C;
	u32 unk550;
	u32 unk554;
	u32 unk558;
	u32 unk55C;
	u32 unk560;
	u32 unk564;
	u32 unk568;
	u32 unk56C;
	u32 unk570;
	u32 unk574;
	u32 unk578;
	u32 unk57C;
	u32 unk580;
	u32 unk584;
	u32 unk588;
	u32 unk58C;
	u32 unk590;
	u32 unk594;
	u32 unk598;
	u32 unk59C;
	u32 unk5A0;
	u32 unk5A4;
	u32 unk5A8;
	u32 unk5AC;
	u32 unk5B0;
	u32 unk5B4;
	u32 unk5B8;
	u32 unk5BC;
	u32 unk5C0;
	u32 unk5C4;
	u32 unk5C8;
	u32 unk5CC;
	u32 unk5D0;
	u32 unk5D4;
	u32 unk5D8;
	u32 unk5DC;
	u32 unk5E0;
	u32 unk5E4;
	u32 unk5E8;
	u32 unk5EC;
	u32 unk5F0;
	u32 unk5F4;
	u32 unk5F8;
	u32 unk5FC;
	u32 unk600;
	u32 unk604;
	u32 unk608;
	u32 unk60C;
	u32 unk610;
	u32 unk614;
	u32 unk618;
	u32 unk61C;
	u32 unk620;
	u32 unk624;
	u32 unk628;
	u32 unk62C;
	u32 unk630;
	u32 unk634;
	u32 unk638;
	u8 unk63C;
	u8 currentMovementState; // 0x63D: 0 - moving, 1 - standing still, 2 - swimming 
	u8 unk63E;
	u8 unk63F;
	u32 unk640;
	u32 unk644;
	u32 unk648;
	u32 unk64C;
	u32 unk650;
	u32 unk654;
	u32 unk658;
	u32 unk65C;
	u32 unk660;
	u32 unk664;
	u32 unk668;
	u8 currentTerrainType; // 0x66C: 03 - water
	u8 unk66D;
	u8 unk66E;
	u8 unk66F;
	u32 unk670;
	u32 unk674;
	u32 unk678;
	u32 unk67C;
	u32 unk680;
	int32 jumpPeakHeight; // 0x684
	u32 unk688;
	u32 unk68C;
	u32 unk690;
	u32 unk694;
	u32 unk698;
	u32 unk69C;
	u32 unk6A0;
	u32 unk6A4;
	u32 unk6A8;
	u16 unk6AC;
	u16 featherCapTimeRemaining; // 0x6AE
	u32 unk6B0;
	u32 unk6B4;
	u32 unk6B8;
	u32 unk6BC;
	u32 unk6C0;
	u32 unk6C4;
	u32 unk6C8;
	u32 unk6CC;
	u32 unk6D0;
	u32 unk6D4;
	u32 unk6D8;
	u8 previousHatCharacter; // 0x6DC
	u8 currentHatCharacter; // 0x6DD
	u8 unk6DE;
	u8 unk6DF;
	u8 unk6E0;
	u8 currentJumpNumber; // 0x6E1: 0 - first, 1 - second, 2 - triple jump, more?
	u8 unk6E2;
	u8 climbingOrPushingRelated; // 0x6E3: 1 - on pole or moving along wall, 2 - moving while grabbing roof or pushing object, 3 - hitting wall under water?
	u32 unk6E4;
	u8 unk6E8;
	u8 currentCollisionState; // 0x06E9: 0 - not colliding, 1 - standing on ground, 2 - colliding with wall in water, 3 - colliding with wall on land 
	u16 unk6EA;
	u32 unk6EC;
	u32 unk6F0;
	u32 unk6F4;
	u32 unk6F8;
	u32 unk6FC;
	u32 unk700;
	u32 unk704;
	u32 unk708;
	u32 unk70C;
	u16 unk710;
	u8 isInAir; // 0x712
	u8 unk713;
	u32 unk714;
	u32 unk718;
	u32 unk71C;
	u32 unk720;
	u32 unk724;
	u32 unk728;
	u32 unk72C;
	u32 unk730;
	u32 unk734;
	u32 unk738;
	u32 unk73C;
	u32 unk740;
	u32 unk744;
	u32 unk748;
	u32 unk74C;
	u32 unk750;
	u32 unk754;
	u32 unk758;
	u32 unk75C;
	u32 unk760;
	u32 unk764;
	u32 unk768;
	u32 unk76C;
	u32 unk770;
	// TODO: This struct's huge, add rest
};

extern MarioActor* PLAYER;

extern byte Player_CurrentAction;
extern byte Player_PreviousUniqueAction;

const unsigned int SDAT_MINUS_0x10 = 0x0209B4A4;

const unsigned int LEVEL_OVERLAY_LOAD_ADDRESS = 0x0214EAA0;

const unsigned int ACT_SELECTOR_ID_TABLE_ADDRESS = 0x02075298;

const unsigned int CANNON_ACTIVATION_BITMAP_ADDRESS = 0x0209CAB0;
const unsigned int STAR_COMPLETEION_BITMAP_ADDRESS = 0x0209CAB4;

extern byte PreviousLevelID;
extern byte CurrentLevelID;

// So that when spawning an object, there's a constant reference to the TmpThreeIntArray
extern int TmpThreeIntArray[3]; 

const byte OBJECT_BANK_0_OFFSET = 0x54;
const byte OBJECT_BANK_7_OFFSET = 0x5C;
const byte BMD_FILE_ID_OFFSET = 0x68;
const byte KCL_FILE_ID_OFFSET = 0x6A;
const byte ICG_FILE_ID_OFFSET = 0x6C;
const byte ICL_FILE_ID_OFFSET = 0x6E;
const byte CAMERA_START_ZOOMED_OUT_OFFSET = 0x75;
const byte MINIMAP_SCALE_OFFSET = 0x76;
const byte SKYBOX_OFFSET = 0x78;
const byte MUSIC_SETTING_0_OFFSET = 0x7C;

const short int OBJ_X_LOC_OFFSET = 0x5C;
const short int OBJ_Y_LOC_OFFSET = 0x60;
const short int OBJ_Z_LOC_OFFSET = 0x64;
const short int OBJ_X_ROT_OFFSET = 0x8C;
const short int OBJ_Y_ROT_OFFSET = 0x8E;
const short int OBJ_Z_ROT_OFFSET = 0x90;
const short int OBJ_X_SPEED_OFFSET = 0xA4;
const short int OBJ_Y_SPEED_OFFSET = 0xA8;
const short int OBJ_Z_SPEED_OFFSET = 0xAC;
const short int OBJ_FORWARD_DIRECTION_OFFSET = 0x94;
const short int OBJ_FORWARD_SPEED_OFFSET = 0x98;
const short int OBJ_DRAW_DISTANCE_OFFSET = 0xBC;
const short int OBJ_PARAMETER_01_OFFSET = 0x08;
const short int OBJ_PARAMETER_02_OFFSET = 0x8C;
const short int OBJ_PARAMETER_03_OFFSET = 0x90;
const short int OBJ_UPDATE_MARIO_POS_OFFSET = 0x13C;

struct OAMSettings
{
	byte unk00;
	byte unk01;
	byte unk02;
	byte unk03;
	byte TileID;
	byte unk05; // bits 5 - 8 seem to be palette row index
	byte unk06;
	byte unk07;
};

struct LevelSettings
{
	byte ObjectBankSettings[8];
	unsigned short BMD_FileID;
	unsigned short KCL_FileID;
	unsigned short ICG_FileID;
	unsigned short ICL_FileID;
	bool CameraStartZoomedOut;
	unsigned short MinimapScaleFactor;
	byte SkyBox;
	byte MusicSettings[3];
	byte ActSelectorID;
};

struct MarioTalkDirection
{
	int LookAtX;
	int LookAtY;
	int LookAtZ;
};

extern "C" 
{
    void Player_PerformAction(
		MarioActor *, 
		char action, 
		unsigned int loop_related, 
		unsigned short playspeed_20_12
	);
		
	void ParticleEffect(
		short int effect_id, 
		int x_position, 
		int y_position, 
		int z_position
	);
	
	void ComplexParticleEffectForObject( 
		unsigned int object_address, 
		short int effect_id, 
		unsigned int four_free_bytes_offset
	);
		
	void OBJ_028_PositionUpdateDrawMethod(
		unsigned int obj_028_address
	);
		
	void Player_PlaySoundEffect(
		unsigned int sdat_minus_0x10, 
		byte zero_unknown, 
		byte sound_effect_id
	);
	
	// Spawns an object with specified Actor ID, 
	// Object Bank settings must allows object in current level, 
	// Returns address of the new object
	unsigned int SpawnActor(
		short int actor_id, 
		unsigned short int parameter01, 
		int* xyz_positions, 
		short int* xyz_rotations
	);
	
	// Removes the specified object from the scene 
	void DestroyObject(
		unsigned int object_address
	);
	
	void OBJ_UpdateObjectXYZSpeedBasedOnForwardSpeedAndDirection( 
		unsigned int object_address
	);
	
	// Returns the address of the next object in a level with 
	// the specified Actor ID starting after the object whose 
	// address is specified. Set search_from_address 
	// to zero to find the first object.
	unsigned int FindNextObjectByActorID(
		short int actor_id, 
		unsigned int search_from_address
	);
	
	// Allows an object to talk to Mario with message at specified 
	// index into message data. 
	// object_address: address of object talking to Mario
	// message_index: index of message in message data
	// direction: direction Mario's body and head should face 
	void OBJ_TalkToPlayer(
		MarioActor *, 
		unsigned int object_address, 
		unsigned short int message_index, 
		MarioTalkDirection * direction
	);
	
	unsigned short int ObjectMessageIDToActualMessageID( 
		unsigned short int object_message_id
	);
	
	// Update object's model and collision map to match its 
	// X, Y and Z rotation values. To find the model rotation offset 
	// look for three occurrences of 0x00001000 each 16 bytes apart
	void OBJ_UpdateObjectModelRotation(
		unsigned int object_address_model_rotation_offset, 
		short int x_rot, 
		short int y_rot, 
		short int z_rot
	);
	
	void OBJ_UpdateObjectModelRotationY(
		unsigned int object_address_model_rotation_offset, 
		short int y_rot
	);
	
	// Important: +0x114, +0x118 and +0x11C << CHECK FOR ALL << must be set to 
	// the object's X, Y and Z positions respectively, shifted right by 3
	void OBJ_UpdateObjectCollisionRotation(
		unsigned int object_address
	);
	
	bool AreCannonsActivatedForCurrentLevel();
	void SetCannonsToActivatedForCurrentLevel();
	
	// Loads the specified level
	// level_id: Level ID, zero-based index
	// entrance_id: Entrance ID to be used
	// unknown_minus_one: Unknown, set to 0xFFFFFFFF
	// mode: 0 = no HUD display, 1 = lives and Stars on top screen, 2 = health set to 1
	void LoadLevel( 
		byte level_id, 
		byte entrance_id, 
		unsigned int unknown_minus_one, 
		byte mode
	);
	
	bool Player_HitWall( 
		MarioActor *
	);
	
	bool Player_KnockedBackUnderWater(
		MarioActor * 
	);
	
	byte GetActSelectorID( 
		byte level_id
	); 
	
	//Printing
	void nocashPrint(const char* txt);
	void nocashPrint1(const char* txt, u32 r0);
	void nocashPrint2(const char* txt, u32 r0, u32 r1);
	void nocashPrint3(const char* txt, u32 r0, u32 r1, u32 r2);
}

void HexPrint(unsigned int value, char *result);

extern OAMSettings* OAM_ThinNumbers[10];

void hook_020E5098();

enum Characters
{
	CHAR_Mario = 0,
	CHAR_Luigi = 1,
	CHAR_Wario = 2,
	CHAR_Yoshi = 3
};

enum Levels
{
	LEVEL_TestMapA = 0,
	LEVEL_CastleGrounds = 1,
	LEVEL_CastleFirstFloor = 2,
	LEVEL_CastleBackyards = 3,
	LEVEL_CastleBasement = 4,
	LEVEL_CastleSecondFloor = 5,
	LEVEL_BobOmbBattlefield = 6,
	LEVEL_WhompFortress = 7,
	LEVEL_JollyRogerBay = 8,
	LEVEL_JollyRogerBay_Ship = 9,
	LEVEL_CoolCoolMountain = 10,
	LEVEL_CoolCoolMountain_Slider = 11,
	LEVEL_BigBooHaunt = 12,
	LEVEL_HazyMazeCave = 13,
	LEVEL_LethalLavaLand = 14,
	LEVEL_LethalLavaLand_Volcano = 15,
	LEVEL_ShiftingSandLand = 16,
	LEVEL_ShiftingSandLand_Pyramid = 17,
	LEVEL_DireDireDocks = 18,
	LEVEL_SnowmanLand = 19,
	LEVEL_SnowmanLand_Igloo = 20,
	LEVEL_WetDryWorld = 21,
	LEVEL_TallTallMountain = 22,
	LEVEL_TallTallMountain_Slider = 23,
	LEVEL_TinyHugeIsland_Huge = 24,
	LEVEL_TinyHugeIsland_Tiny = 25,
	LEVEL_TinyHugeIsland_Mountain = 26,
	LEVEL_TickTockClock = 27,
	LEVEL_RainbowRide = 28,
	LEVEL_PrincessSecretSlide = 29,
	LEVEL_TheSecretAquarium = 30,
	LEVEL_RedSwitch = 31,
	LEVEL_TheSecretUndertheMoat = 32,
	LEVEL_BehindTheWaterfall = 33,
	LEVEL_OvertheRainbows = 34,
	LEVEL_BowserInTheDarkWorld_Map = 35,
	LEVEL_BowserInTheDarkWorld_Fight = 36,
	LEVEL_BowserInTheFireSea_Map = 37,
	LEVEL_BowserInTheFireSea_Fight = 38,
	LEVEL_BowserInTheSky_Map = 39,
	LEVEL_BowserInTheSky_Fight = 40,
	LEVEL_TestMapB = 41,
	LEVEL_TheSecretOfBattleFort = 42,
	LEVEL_SunshineIsles = 43,
	LEVEL_GoombossBattle_Map = 44,
	LEVEL_GoombossBattle_Fight = 45,
	LEVEL_BigBooBattle_Map = 46,
	LEVEL_BigBooBattle_Fight = 47,
	LEVEL_ChiefChillyChallenge_Map = 48,
	LEVEL_ChiefChillyChallenge_Fight = 49,
	LEVEL_PlayRoom = 50,
	LEVEL_CastleGrounds_Multiplayer = 51
};

enum PlayerActions
{
	ACT_StandStillLookInAwe = 0x00,
	ACT_SpinCelebrateAfterLookInAwe = 0x01,
	ACT_FallOverDead = 0x02,
	ACT_TooWeakToGetUpFromFront = 0x03,
	ACT_TooWeakToGetUpFromBack = 0x04,
	ACT_Cough = 0x05,
	ACT_SuffocateAndDie = 0x06,
	ACT_SwaySideToSideAndDropDown = 0x07,
	ACT_GetUpFromSittingOnGround = 0x08,
	ACT_SleepOnSide = 0x09,
	ACT_SettleToSleepOnSide = 0x0A,
	ACT_QuicklyGetUpFromSleeping = 0x0B,
	ACT_SittingNoddingOff = 0x0C,
	ACT_JerkHeadUpWhileSleeping = 0x0D,
	ACT_YawnStretchThenSitDownToSleep = 0x0E,
	ACT_LeaningForwardPantingTired = 0x0F,
	ACT_HitAgainstWall = 0x12,
	ACT_LongJump = 0x1A,
	ACT_CrouchAfterLongJump = 0x1B,
	ACT_DismountFromHandstand = 0x1C,
	ACT_MaintainHandstand = 0x1D,
	ACT_StartHandstand = 0x1E,
	ACT_PullSelfUpFromHangingOffEdgeBothLegsTogether = 0x20,
	ACT_HangOffEdge = 0x21,
	ACT_StartToHangOffEdge = 0x22,
	ACT_PullSelfUpFromHangingOffEdgeRightLegFirst = 0x23,
	ACT_ClimbPole = 0x24,
	ACT_SlideDownPole = 0x25,
	ACT_HoldingOnToPole = 0x26,
	ACT_GrabPole = 0x27,
	ACT_WallJump = 0x28,
	ACT_SlideDownWall = 0x29,
	ACT_Backflip = 0x2A,
	ACT_LandAfterBackflip = 0x2B,
	ACT_Crouch = 0x2C,
	ACT_StartToCrouch = 0x2D,
	ACT_GetUpFromCrouch = 0x2E,
	ACT_Punch = 0x38,
	ACT_DoublePunch = 0x39,
	ACT_Kick = 0x3A,
	ACT_GroundPoundStart = 0x3B,
	ACT_GroundPound = 0x3C, 
	ACT_GroundPoundEnd = 0x3D, 
	ACT_Run = 0x3F,
	ACT_SlideBackOnStomach = 0x43,
	ACT_QuicklyTurn = 0x45,
	ACT_SkidToHalt = 0x46,
	ACT_Wait = 0x47,
	ACT_Walk = 0x48,
	ACT_Fly = 0x49,
	ACT_TumbbleThenStartToFly = 0x4A,
	ACT_Tumble = 0x4B,
	ACT_LandAfterTripleJump = 0x4C,
	ACT_SideJump = 0x4D,
	ACT_LandAfterSideJump = 0x4E, 
	ACT_ReachedPeakOfFirstOrSecondJump = 0x4F,
	ACT_StartToFallAfterJump = 0x50,
	ACT_LandOnGroundAfterJumpReachingPeak = 0x51,
	ACT_LandOnGroundAfterJump = 0x52,
	ACT_JumpStart = 0x53,
	ACT_Fall = 0x54,
	ACT_HeavyFall = 0x55,
	ACT_Tumble2 = 0x56,
	ACT_HangByLeftArm = 0x57,
	ACT_HangByRightArm = 0x58,
	ACT_MoveWhileHangingLeftToRight = 0x59,
	ACT_MoveWhileHangingRightToLeft = 0x5A, 
	ACT_HangByRightArmStart = 0x5B, 
	ACT_HugWallMoveLeft = 0x5C, 
	ACT_HugWallMoveRight = 0x5D, 
	ACT_HugWallStandStill = 0x5E, 
	ACT_SpinJumpTPosition = 0x5F, 
	ACT_Sneak = 0x61,
	ACT_EndCrawl = 0x62,
	ACT_Crawl = 0x63,
	ACT_StartToCrawl = 0x64,
	ACT_PushAgainstSolidObject = 0x65,
	ACT_SlideBeforeSlideFlip = 0x67,
	ACT_SlideFlip = 0x68,
	ACT_SmallKnockBackFromHit = 0x69,
	ACT_MediumKnockBackFromHit = 0x6A,
	ACT_SpinAndPunchAirCelebrate = 0x6B,
	ACT_HitBackOfHead = 0x6D,
	ACT_PickedUpAndTrapped = 0x71,
	ACT_ShotFromCannon = 0x73,
	ACT_StuckInGroundStartPullingOut = 0x74,
	ACT_StuckInGroundLookAround = 0x75,
	ACT_StuckInGroundPullOut = 0x76,
	ACT_BumStuckInGroundPullOut = 0x77,
	ACT_BumStuckInGroundStartPullingOut = 0x78,
	ACT_BumStuckInGroundLookAround = 0x79,
	ACT_HeadStuckInGroundPullOut = 0x7A,
	ACT_HeadStuckInGroundIdle = 0x7B,
	ACT_HeadStuckInGroundEnter = 0x7C,
	ACT_DrowningInSand = 0x7D,
	ACT_PushingThroughDeepSand = 0x7E,
	ACT_KnockedBackOnBum = 0x7F,
	ACT_HoldOnToObjectInGround = 0x80,
	ACT_StartGrabbingObjectInGround = 0x81,
	ACT_StartToDrown = 0x82,
	ACT_DieInWater = 0x83,
	ACT_Drown = 0x84,
	ACT_CelebrateAfterExitingLevelWithStar = 0x85,
	ACT_StartToGrabObject = 0x86,
	ACT_HoldObject = 0x87,
	ACT_CarryObject = 0x88,
	ACT_HoldObject2 = 0x89,
	ACT_SetDownObject = 0x8A,
	ACT_StartToGrabCaughtRabbitAfterDive = 0x8B,
	ACT_RunAwayScared = 0x8C,
	ACT_OpenDoor = 0x8D,
	ACT_PickUpHugeObject = 0x8E,
	ACT_CarryHugeObject = 0x8F,
	ACT_HoldHugeObject = 0x90,
	ACT_ThrowHugeObject = 0x91,
	ACT_LaunchObjectIntoAir = 0x92,
	ACT_UnlockDoubleDoorWithKey = 0x93,
	ACT_FlipAndPutAwayKeyAfterReceiving = 0x94,
	ACT_HuddleInCold = 0x95,
	ACT_ShiverWhileHuddledInCold = 0x96,
	ACT_SwitchCaps = 0x98,
	ACT_SpinAndPutOnCap = 0x99,
	ACT_GrowFromSuperMushroom = 0x9A,
	ACT_ShrinkAfterSuperMushroom = 0x9B,
	ACT_LookDownAtHands = 0xA0,
	ACT_CheckForDamageAndShrug = 0xA5,
	ACT_KickingInWater = 0xA6,
	ACT_EnterWater = 0xA7,
	ACT_FloatInWater = 0xA8,
	ACT_StartBreastStroke = 0xA9,
	ACT_StartToEndSwimMotion = 0xAA,
	ACT_EndSwimMotion = 0xAB,
	ACT_GetStarUnderWater = 0xAC,
	ACT_KnockedBackUnderWater = 0xAD,
	ACT_TryToPunchUnderWater = 0xAE,
	ACT_GrabObjectUnderWater = 0xAF,
	ACT_SwimWhileHoldingObject = 0xB0,
	ACT_LoseObjectUnderWater = 0xB1,
	ACT_PointForwardStartIntro = 0xB2,
	ACT_StandStillStartIntro = 0xB3,
	ACT_SitAttentivelyStartIntro = 0xB4,
	ACT_StandStillSwaySlightly = 0xB5,
	ACT_PointToRightStartIntro = 0xB6,
	ACT_StandStillUnknown1 = 0xB7,
	ACT_StartToTurnToLookLeft = 0xB8,
	ACT_TurnedToLookLeft = 0xB9,
	ACT_TurnQuicklyToLookLeft = 0xBA,
	ACT_RevealStarAsWhenOpeningDoor = 0xBB,
	ACT_ArmsOutstretchedAfterRevealingStar = 0xBC,
	ACT_RemoveCapEndScene = 0xBD,
	ACT_WaitInFrontOfPeachEndScene = 0xBE,
	ACT_WaveEndScene = 0xBF,
	ACT_CollectFinalStarCelebrateAndTakeOff = 0xC0,
	ACT_FlyingFinalDefeat = 0xC1					// Final Valid Action
};

#endif