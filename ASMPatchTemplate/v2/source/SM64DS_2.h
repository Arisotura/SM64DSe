#ifndef SM64DS_2_H_INCLUDED
#define SM64DS_2_H_INCLUDED

#include "SM64DS_Common.h"
#include "Model.h"
#include "Collision.h"
#include "Level.h"
#include "Message.h"
#include "Particle.h"
#include "Sound.h"

#define NOINLINE __attribute__((noinline))
#define NAKED __attribute__((naked))

#define OBJ_TO_ACTOR_ID_TABLE ((volatile uint16_t*)0x02004b00)
#define ACTOR_SPAWN_TABLE     ((volatile unsigned*)0x02006590)

/*
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
		void(*OnYoshiTryEat)(Actor*),
		0x02010154,
		0x0201014c,
		void(*OnGroundPounded)(Actor*, Actor* attacker),
		void(*OnAttacked1)(Actor*, Actor* attacker),
		void(*OnAttacked2)(Actor*, Actor* attacker),
		void(*OnKicked)(Actor*, Actor* attacker),
		0x02010138,
		void(*OnHitByCannonBlastedChar)(Actor*, Player*),
		void(*OnHitByMegaChar)(Actor*, Player*),
		0x0201012c,
		0x02010124,
		0x020100dc,
		(unsigned)&Kill
	};
*/

struct ActorBase
{
	enum AliveState
    {
        ALIVE = 1,
		DEAD = 2
    };
	
	struct SceneNode
	{
		SceneNode* parent;
		SceneNode* firstChild;
		SceneNode* prevSibling;
		SceneNode* nextSibling;
		ActorBase* actor;
	};
	
	struct ProcessingListNode
	{
		ProcessingListNode* prev;
		ProcessingListNode* next;
		ActorBase* actor;
		uint16_t priority;
		uint16_t priorityCopy;
	};
	
	enum VirtualFuncSuccess
	{
		VS_FAIL_BEFORE = 0,
		VS_FAIL = 1,
		VS_SUCCESS = 2,
		VS_RETURN_MINUS_1 = 3
	};
	
	void* operator new(size_t count); //actor bases have their own heap
	inline void operator delete(void* ptr) {FreeHeapAllocation(ptr, *(unsigned**)0x020a0eac);}
	
	virtual int  InitResources();
	virtual bool BeforeInitResources();
	virtual void AfterInitResources(unsigned vfSuccess);
	virtual int  CleanupResources();
	virtual bool BeforeCleanupResources();
	virtual void AfterCleanupResources(unsigned vfSuccess);
	virtual int  Behavior();
	virtual bool BeforeBehavior();
	virtual void AfterBehavior(unsigned vfSuccess);
	virtual int  Render();
	virtual bool BeforeRender();
	virtual void AfterRender(unsigned vfSuccess);
	virtual void Virtual30();
	virtual bool Virtual34(unsigned arg0, unsigned arg1);
	virtual bool Virtual38(unsigned arg0, unsigned arg1);
	virtual bool Virtual3c();
	virtual ~ActorBase();
	
	void Destroy();
	
	//vTable;
	unsigned uniqueID;
	int      param1;
	uint16_t actorID;
	uint8_t aliveState;
	bool shouldBeKilled;
	unsigned unk10;
	SceneNode sceneNode;
	ProcessingListNode behavNode;
	ProcessingListNode renderNode;
	unsigned unk48;
	unsigned unk4c;
};

struct Actor : public ActorBase
{
	
	enum Flags : int
	{
		NO_BEHAVIOR_IF_OFF_SCREEN = 1 << 0,
		NO_RENDER_IF_OFF_SCREEN = 1 << 1, //offscreen can mean too far away.
		OFF_SCREEN = 1 << 3,
		OFF_SHADOW_RANGE = 1 << 4,
		WRONG_AREA = 1 << 5,
		GOING_TO_YOSHI_MOUTH = 1 << 17,
		IN_YOSHI_MOUTH = 1 << 18,
		CAN_SQUISH = 1 << 25,
		AIMABLE_BY_EGG = 1 << 28
	};
	
	struct ListNode
	{
		ListNode* prev;
		ListNode* next;
		Actor* actor;
	};
	
	enum OnYoshiEatReturnVal : int
	{
		YE_DONT_EAT = 0,
		YE_KEEP_IN_MOUTH = 1,
		YE_SPIT_AND_GET_HURT = 2,
		YE_SPIT = 3,
		YE_SWALLOW = 4,
		YE_GAIN_FIRE_BREATH = 5,
		YE_KEEP_AND_CAN_MAKE_EGG = 6,
	};
	
	ListNode listNode;
	Vector3 pos;
	Vector3 prevPos;
	Vector3 camSpacePos;
	Vector3 scale;
	Vector3_16 ang;
	Vector3_16 motionAng;
	Fix12i horzSpeed;
	Fix12i vertAccel;
	Fix12i termVel;
	Vector3 speed;
	unsigned flags;
	Fix12i rangeOffsetY;
	Fix12i rangeAsr3;
	Fix12i drawDistAsr3;
	Fix12i unkc0Asr3;
	unsigned unkc4;
	unsigned unkc8;
	char areaID; //it is signed
	uint8_t unkcd;
	short deathTableID;
	Player* eater;
	
	Actor();
	
	virtual bool BeforeInitResources() override;
	virtual void AfterInitResources(unsigned vfSuccess) override;
	virtual bool BeforeCleanupResources() override;
	virtual void AfterCleanupResources(unsigned vfSuccess) override;
	virtual bool BeforeBehavior() override;
	virtual void AfterBehavior(unsigned vfSuccess) override;
	virtual bool BeforeRender() override;
	virtual void AfterRender(unsigned vfSuccess) override;
	virtual ~Actor();
	virtual unsigned OnYoshiTryEat();
	virtual void OnTurnIntoEgg(Player& player);
	virtual bool Virtual50();
	virtual void OnGroundPounded(Actor& groundPounder);
	virtual void OnAttacked1(Actor& attacker);
	virtual void OnAttacked2(Actor& attacker);
	virtual void OnKicked(Actor& kicker);
	virtual void OnPushed(Actor& pusher);
	virtual void OnHitByCannonBlastedChar(Actor& blaster);
	virtual void OnHitByMegaChar(Player& megaChar);
	virtual void Virtual70(Actor& attacker);
	virtual Fix12i OnAimedAtWithEgg();
	virtual Vector3 OnAimedAtWithEggReturnVec();
	
	bool IsTooFarAwayFromPlayer(Fix12i tooFar);
	void MakeVanishLuigiWork(CylinderClsn& cylClsn);
	void SpawnSoundObj(unsigned soundObjParam);
	
	void KillAndTrackInDeathTable();
	void UntrackInDeathTable();
	bool GetBitInDeathTable();
	
	void BigLandingDust(bool doRaycast);
	void LandingDust(bool doRaycast);
	void DisappearPoofDustAt(const Vector3& vec);
	void SmallPoofDust();
	void PoofDustAt(const Vector3& vec);
	void PoofDust(); //calls the two above function
	
	void UntrackStar();
	Actor* UntrackAndSpawnStar(unsigned trackStarID, unsigned starID, const Vector3& spawnPos, unsigned howToSpawnStar);
	unsigned TrackStar(unsigned starID, unsigned starType); //starType=1: silver star, 2: star //returns star ID or 0xff if starID != STAR_ID
	
	void Earthquake(const Vector3& source, Fix12i magnitude);
	short ReflectAngle(Fix12i normalX, Fix12i normalZ, short angToReflect);
	
	bool BumpedUnderneathByPlayer(Player& player); //assumes there is a collision in the first place
	bool JumpedOnByPlayer(CylinderClsn& cylClsn, Player& player);
	void Unk_0201061c(Player& player, unsigned numCoins, unsigned coinType);
	
	Fix12i DistToCPlayer();
	Player* ClosestPlayer();
	short HorzAngleToCPlayer();
	short HorzAngleToCPlayerOrAng(short ang);
	Player* FarthestPlayer();
	Fix12i DistToFPlayer();
	
	void DropShadowScaleXYZ(ShadowVolume& arg1, const Matrix4x3& arg2, Fix12i scaleX, Fix12i scaleY, Fix12i scaleZ, unsigned transparency);
	void DropShadowRadHeight(ShadowVolume& arg1, const Matrix4x3& arg2, Fix12i radius, Fix12i depth, unsigned transparency); //last argument is on a scale of 1 to 16.
	
	void UpdatePos(CylinderClsn* clsn); //Applies motion direction, vertical acceleration, and terminal velocity.
	void UpdatePosWithOnlySpeed(CylinderClsn* clsn);//IMPORTANT!: When spawning a Super Mushroom, make sure to already have the model loaded before the player goes super!
	//You cannot afford to spawn a Super Mushroom if there are 0 uses of the model's SharedFilePtr and the player already went super.
	//If you do, particle color glitches will happen!
	
	Actor* SpawnNumber(const Vector3& pos, unsigned number, bool isRed, unsigned arg3, unsigned arg4);
	static Actor* Spawn(unsigned actorID, unsigned param1, const Vector3& pos, const Vector3_16* rot, int areaID, int deathTableID);
	Actor* Next(); //next in the linked list. Returns the 1st object if given a nullptr. Returns a nullptr if given the last actor
	static Actor* FindWithID(unsigned id);
	static Actor* FindWithActorID(unsigned actorID, Actor* searchStart); //searchStart is not included.
	
};

struct CapIcon
{
	enum Flags
	{
		ACTIVE = 1 << 1
	};
	
	unsigned* vTable;
	Actor* actor;
	unsigned objID;
	unsigned unk0c;
	unsigned unk10;
	unsigned flags;
	unsigned unk18;
};

struct Enemy : public Actor
{
	enum CoinType
	{
		CN_NONE = 0,
		CN_YELLOW = 1,
		CN_RED = 2,
		CN_BLUE = 3
	};
	
	enum DefeatMethod
	{
		DF_NOT = 0,
		DF_SQUASHED = 1,
		DF_PUNCHED = 2,
		DF_KICKED = 3,
		DF_BURNED = 4,
		DF_DIVED = 5,
		DF_UNK_6 = 6,
		DF_HIT_REGURG = 7,
		DF_GIANT_CHAR = 8 //this is definitely the end of the list.
	};
	
	enum InvCharKillFlags
	{
		IK_SPAWN_COINS = 1 << 0,
		IK_KILL = 1 << 1
	};
	
	enum UpdateYoshiEatReturn
	{
		UY_NOT = 0,
		UY_EATING = 1,
		UY_IN_MOUTH = 2,
		UY_SPITTING = 3
	};
	
	Vector3 floorNormal;
	Vector3 wallNormal;
	Vector3 unkNormal; //ceiling?
	unsigned unk0f8;
	unsigned unk0fc;
	uint16_t stateTimer;
	uint16_t deathTimer;
	uint16_t spitTimer;
	bool isAtCliff;
	bool isBeingSpit;
	uint8_t coinType;
	uint8_t unk109;
	uint8_t numCoinsMinus1;
	uint8_t unk10b;
	unsigned defeatMethod;
	
	Enemy();
	virtual ~Enemy();
	
	unsigned UpdateKillByInvincibleChar(WithMeshClsn& wmClsn, ModelAnim& rigMdl, unsigned flags); //returns 0 with 0 side effects if N/A.
	void KillByInvincibleChar(const Vector3_16& rotation, Player& player);
	void SpawnMegaCharParticles(Actor& attacker, char* arg2);
	bool SpawnParticlesIfHitOtherObj(CylinderClsn& cylClsn); //returns whether there was a collision with an object that isn't a coin
	unsigned UpdateYoshiEat(WithMeshClsn& wmClsn);
	//returns whether the angle was redirected
	//if hitting wall, reflect angle; if at cliff, set angle to the opposite one.
	bool AngleAwayFromWallOrCliff(WithMeshClsn& wmClsn, short& ang);
	bool UpdateDeath(WithMeshClsn& wmClsn); //returns whether the object is actually dying and the death function returned true
	bool IsGoingOffCliff(WithMeshClsn& wmClsn, Fix12i arg2, int arg3, unsigned arg4, unsigned arg5, Fix12i stepToleranceY);
	void KillByAttack(Actor& other, WithMeshClsn& wmc);
	void SpawnCoin();
	void UpdateWMClsn(WithMeshClsn& wmClsn, unsigned arg2);
	
	
};

struct CapEnemy : public Enemy
{
	uint8_t capParam;
	bool hasNotSpawned;
	char spawnCapFlag;
	char capID;
	Model capModel;
	CapIcon capIcon;
	
	CapEnemy();
	virtual ~CapEnemy();
	
	void Unk_02005d94();
	bool DestroyIfCapNotNeeded(); //returns true if the cap is needed (player is different character than cap or 0x0209f2d8 has a 0)
	int GetCapState(); //returns 2 if obj+0x111 = 0, else 0 if dead or capID == player character, else 1
	CapEnemy* RespawnIfHasCap(); //nullptr if failed
	bool GetCapEatenOffIt(const Vector3& offset); //returns whether there was a cap and the cap is not the original object
	Actor* ReleaseCap(const Vector3& offset); //returns the ptr to the cap if cap was released, ptr to original obj else.
	void RenderCapModel(const Vector3* scale);
	void UpdateCapPos(const Vector3& offsetPos, const Vector3_16& rot);
	Actor* AddCap(unsigned capID);
	void UnloadCapModel();
	
};

struct Platform : public Actor
{
	Model model;
	MovingMeshCollider clsn;
	Matrix4x3 clsnNextMat;
	unsigned unk31c;
	
	Platform();
	virtual ~Platform();
	virtual void Kill();
	
	void KillByMegaChar(Player& player);
	bool UpdateKillByMegaChar(short rotSpeedX, short rotSpeedY, short rotSpeedZ, Fix12i speed); //true if killed by mega char
	void UpdateClsnPosAndRot(); //make sure the mesh collider is at 0x124 first! Also, call this after updating the model's matrix
	void UpdateModelPosAndRotY(); //make sure the model is at 0x0d4 first!
	bool IsClsnInRange(Fix12i clsnRange, Fix12i clsnRangeOffsetY); //both in fixed-point 20.12 //side effect: enables collision if and only if in range
	bool IsClsnInRangeOnScreen(Fix12i clsnRange, Fix12i clsnRangeOffsetY); //if offsetY is 0, it is loaded from +0xb4.
	
};

struct PathPtr
{
    LevelFile::Path* path;
	unsigned unk04;
	
	PathPtr();
	
	void FromID(unsigned pathID);
	void GetPt(Vector3& vF, unsigned index);
	unsigned NumPts();
};

struct BezierPathIter
{
	PathPtr pathPtr;
	uint16_t currSplineX3;
	Fix12s tinyStep;
	Fix12i step;
	Fix12i currTime;
	Vector3 pos;
	
	bool Advance();
};

struct CameraDef
{
	static const unsigned SIZE = 0x28;
	
	int unk00; //something to do with going behind the player...
	int unk04;
	int camVertAngIsh;
	unsigned unk0c;
	int vertOffset;
	int offsetIsh; //???
	unsigned jumpWithPlayerIsh;
	int dist0; //???
	int dist1;
	uint16_t fovOver2;
	uint16_t unk26;
};

class Camera : public ActorBase //ActorID = 0x14c
{
public:
	static const unsigned SIZE = 0x1a8;
	
	enum Flags
	{
		ZOOMED_OUT = 1 << 2,
		BOSS_TALK = 1 << 3,
		ROTATING_LEFT = 1 << 5,
		ROTATING_RIGHT = 1 << 6,
		ARROWS_ALLOWED = 1 << 12,
		TALK = 1 << 14,
		ZOOM_OUT_FROM_TALK = 1 << 15,
		ZOOMED_IN = 1 << 19
	};
	
	Matrix4x3 camMat;
	Vector3 lookAt;
	Vector3 pos;
	Vector3 ownerPos;
	unsigned unk0a4;
	unsigned unk0a8;
	unsigned unk0ac;
	unsigned unk0b0;
	unsigned unk0b4;
	unsigned unk0b8;
	unsigned unk0bc;
	unsigned unk0c0;
	unsigned unk0c4;
	unsigned unk0c8;
	unsigned unk0cc;
	unsigned unk0d0;
	unsigned unk0d4;
	unsigned unk0d8;
	unsigned unk0dc;
	unsigned unk0e0;
	unsigned unk0e4;
	unsigned unk0e8;
	unsigned unk0ec;
	unsigned unk0f0;
	unsigned unk0f4;
	unsigned unk0f8;
	unsigned unk0fc;
	unsigned unk100;
	unsigned unk104;
	unsigned unk108;
	unsigned unk10c;
	Actor* owner;
	Actor* owner2;
	Actor* unk118;
	unsigned unk11c;
	Vector3 otherPos;
	Fix12i ownerDist;
	unsigned unk130;
	unsigned unk134;
	unsigned unk138;
	CameraDef* defaultCamDef;
	CameraDef* currCamDef;
	LevelFile::View* currView;
	Vector3* pausePos;
	unsigned unk14c;
	unsigned unk150;
	unsigned flags;
	unsigned unk158;
	unsigned unk15c;
	unsigned unk160;
	unsigned unk164;
	unsigned unk168;
	unsigned unk16c;
	unsigned unk170;
	unsigned unk174;
	uint16_t unk178;
	Vector3_16 angle;
	short eightDirAngleY;
	short eightDirStartAngle;
	short eightDirDeltaAngle;
	short unk186;
	unsigned unk188;
	unsigned unk18c;
	unsigned unk190;
	unsigned unk194;
	unsigned unk198;
	unsigned unk19c;
	unsigned unk1a0;
	unsigned unk1a4;
};

struct Area
{
	TextureTransformer* texSRT;
	bool showing;
	uint8_t unk5;
	uint16_t unk6;
	unsigned unk8;
};

struct HUD : public ActorBase //ActorID = 0x14e
{
	unsigned unk50;
	unsigned unk54;
	unsigned unk58;
	unsigned unk5c;
	unsigned unk60;
	unsigned unk64;
	unsigned unk68;
	unsigned unk6c;
	unsigned unk70;
	char currNumInDecimal[3];
	uint8_t unk77;
	unsigned unk78;
};

//vtable at 020921c0, constructor at 0202e088
struct Stage : public ActorBase //ActorID = 0x003
{
	
	Particle::SysTracker particleSysTracker;
	Model model;
	Area areas[0x08];
	MeshCollider clsn;
	uint8_t fogTable[0x20];
	bool enableFog;
	uint8_t fogInfo;
	uint16_t fogOffset;
	uint16_t fogColor;
	uint16_t unk992;
	uint8_t unk994[0x20];
	unsigned unk9b4;
	unsigned unk9b8;
	Model* skyBox;
	unsigned unk9c0;
	unsigned unk9c4;
};

//vtable at 0210c1c0, constructor at 020fb8bc
//Code address to init 256x256 map: 020fb568
//Code address to init 128x128 map: 020fb694
/*
Conditions for a 256x256 map:
In Peach's Castle but not the castle graunds or the backyard OR
In Big Boo's Haunt OR
In Big Boo Battle (the map, not the fight) OR
In a test stage
*/
struct Minimap : public ActorBase //ActorID = 0x14f
{
	enum ArrowType
	{
		AR_NONE = 0,
		AR_DONT_ROTATE_WITH_MINIMAP = 1,
		AR_ROTATE_WITH_MINIMAP = 2
	};
	
	unsigned unk050;
	unsigned unk054;
	unsigned unk058;
	unsigned unk05c;
	unsigned unk060;
	unsigned unk064;
	unsigned unk068;
	unsigned unk06c;
	unsigned unk070;
	unsigned unk074;
	unsigned unk078;
	unsigned unk07c;
	unsigned unk080;
	unsigned unk084;
	unsigned unk088;
	unsigned unk08c;
	unsigned unk090;
	unsigned unk094;
	unsigned unk098;
	unsigned unk09c;
	unsigned unk0a0;
	unsigned unk0a4;
	unsigned unk0a8;
	unsigned unk0ac;
	unsigned unk0b0;
	unsigned unk0b4;
	unsigned unk0b8;
	unsigned unk0bc;
	unsigned unk0c0;
	unsigned unk0c4;
	unsigned unk0c8;
	unsigned unk0cc;
	unsigned unk0d0;
	unsigned unk0d4;
	unsigned unk0d8;
	unsigned unk0dc;
	unsigned unk0e0;
	unsigned unk0e4;
	unsigned unk0e8;
	unsigned unk0ec;
	unsigned unk0f0;
	unsigned unk0f4;
	unsigned unk0f8;
	unsigned unk0fc;
	unsigned unk100;
	unsigned unk104;
	unsigned unk108;
	unsigned unk10c;
	unsigned unk110;
	unsigned unk114;
	unsigned unk118;
	unsigned unk11c;
	unsigned unk120;
	unsigned unk124;
	unsigned unk128;
	unsigned unk12c;
	unsigned unk130;
	unsigned unk134;
	unsigned unk138;
	unsigned unk13c;
	unsigned unk140;
	unsigned unk144;
	unsigned unk148;
	unsigned unk14c;
	unsigned unk150;
	unsigned unk154;
	unsigned unk158;
	unsigned unk15c;
	unsigned unk160;
	unsigned unk164;
	unsigned unk168;
	unsigned unk16c;
	unsigned unk170;
	unsigned unk174;
	unsigned unk178;
	unsigned unk17c;
	unsigned unk180;
	unsigned unk184;
	unsigned unk188;
	unsigned unk18c;
	unsigned unk190;
	unsigned unk194;
	unsigned unk198;
	unsigned unk19c;
	unsigned unk1a0;
	unsigned unk1a4;
	unsigned unk1a8;
	unsigned unk1ac;
	unsigned unk1b0;
	unsigned unk1b4;
	unsigned unk1b8;
	unsigned unk1bc;
	unsigned unk1c0;
	unsigned unk1c4;
	unsigned unk1c8;
	unsigned unk1cc;
	unsigned unk1d0;
	unsigned unk1d4;
	unsigned unk1d8;
	unsigned unk1dc;
	unsigned unk1e0;
	unsigned unk1e4;
	unsigned unk1e8;
	unsigned unk1ec;
	unsigned unk1f0;
	Vector3 center;
	Matrix2x2 arrowMat;
	unsigned unk210;
	Fix12i targetInvScale;
	Fix12i invScale;
	short angle;
	short unk21e;
	unsigned unk220;
	unsigned unk224;
	unsigned unk228;
	unsigned unk22c;
	unsigned unk230;
	unsigned unk234;
	unsigned unk238;
	unsigned unk23c;
	unsigned unk240;
	unsigned unk244;
	unsigned unk248;
	unsigned unk24c;
	uint8_t unk250;
	uint8_t arrowType;
	uint8_t unk252;
	uint8_t unk253;
	unsigned unk254;
};

struct LaunchStar;

//constructor: 020e6c0c, vtable: 0210a83c
struct Player : public Actor
{
	enum Characters
	{
		CH_MARIO,
		CH_LUIGI,
		CH_WARIO,
		CH_YOSHI
	};
	
    struct State
    {
        bool(Player::* init)();
		bool(Player::* main)();
		bool(Player::* cleanup)();
    };
    enum States
    {
		ST_LEDGE_GRAB         = 0x02110004,
		ST_CEILING_GRATE      = 0x0211001c,
		ST_YOSHI_POWER        = 0x02110034, //tongue, spitting, throwing egg, breathing fire
        ST_SWALLOW            = 0x0211004c,
		
		
		ST_HURT               = 0x02110094,
        ST_HURT_WATER         = 0x021100ac,
		ST_ELECTROCUTE        = 0x021100c4,
		ST_BURN_FIRE          = 0x021100dc,
		ST_BURN_LAVA          = 0x021100f4,
		ST_DEAD_HIT           = 0x0211010c,
		ST_DEAD_PIT           = 0x02110124,
        ST_WALK               = 0x0211013c,
        ST_WAIT               = 0x02110154,
		ST_GRABBED            = 0x0211016c,
		ST_TURN_AROUND        = 0x02110184,
		ST_JUMP               = 0x0211019c,
        ST_FALL               = 0x021101b4,
		ST_THROWN             = 0x021101cc,
		ST_SIDE_FLIP          = 0x021101e4,
		ST_SLIDE_KICK_RECOVER = 0x021101fc,
		ST_FLY                = 0x02110214,
		ST_NO_CONTROL         = 0x0211022c, //includes caps
		ST_OWL                = 0x02110244,
		
		ST_WIND_CARRY         = 0x02110274,
		ST_BALLOON            = 0x0211028c,
		ST_TELEPORT           = 0x021102a4,
		
		ST_CANNON             = 0x021102d4,
		ST_SQUISH             = 0x021102ec,
		ST_SHELL              = 0x02110304,
		ST_STOMACH_SLIDE      = 0x0211031c,
		ST_BUTT_SLIDE         = 0x02110334,
		ST_DIZZY_STARS        = 0x0211034c,
		ST_HOLD_LIGHT         = 0x02110364,
		ST_BONK               = 0x0211037c,
		ST_HOLD_HEAVY         = 0x02110394,
		ST_WALL_SLIDE         = 0x021103ac,
		
		ST_WALL_JUMP          = 0x021103dc,
		ST_SLOPE_JUMP         = 0x021103f4,
		ST_STUCK_IN_GROUND    = 0x0211040c,
        ST_LAND               = 0x02110424,
		ST_ON_WALL            = 0x0211043c,
		ST_SPIN               = 0x02110454,
		ST_TALK		          = 0x0211046c,
		ST_CRAZED_CRATE       = 0x02110484,
		
		ST_LEVEL_ENTER        = 0x021104b4,
		
		ST_CROUCH             = 0x021104e4,
		
		ST_CRAWL              = 0x02110514,
		ST_BACK_FLIP          = 0x0211052c,
		
		ST_LONG_JUMP          = 0x0211055c,
		ST_PUNCH_KICK         = 0x02110574,
		
		ST_GROUND_POUND       = 0x021105a4,
		ST_DIVE               = 0x021105bc,
		ST_THROW              = 0x021105d4,
		
		
		
		ST_SLIDE_KICK         = 0x02110634,
		
		
        ST_SWIM               = 0x0211067c,
		ST_WATER_JUMP         = 0x02110694,
		ST_METAL_WATER_GROUND = 0x021106ac,
		ST_METAL_WATER_WATER  = 0x021106c4,
		ST_CLIMB              = 0x021106dc,
		ST_HEADSTAND          = 0x021106f4,
		ST_POLE_JUMP          = 0x0211070c,
		ST_HEADSTAND_JUMP     = 0x02110724,
		
		
		
		
		ST_LAUNCH_STAR        = 0x0211079c
    };
	
	enum TalkStates
	{
		TK_NOT = -1,
		TK_START = 0,
		TK_TALKING = 1, //+0x6e3 == anything but 3, 5, or 7
		TK_UNK2 = 2, //+0x6e3 == 3
		TK_UNK3 = 3  //+0x6e3 == 5 or 7
	};
	
	enum Flags2
	{
		
		
		F2_CAMERA_ZOOM_IN = 1 << 2,
		F2_TELEPORT = 1 << 3,
		
		
		
		F2_RESET_POSITION = 1 << 7,
		
		
		F2_EXIT_LEVEL_IF_DEAD = 1 << 10,
		F2_NO_CONTROL = 1 << 11,
		F2_START_FLOWER_POWER = 1 << 12
	};
	
	unsigned unk0d4;
	unsigned unk0d8;
	ModelAnim2* bodyModels[5]; //the fifth one is the Metal Wario model
	ModelAnim balloonModel;
	Model* headModels[4]; //Yoshi's is a ModelAnim
	Model* headNoCapModels[4]; //Yoshi's is the mouth-is-full model
	ModelAnim wings;
	unsigned unk1d8;
	TextureSequence texSeq1dc;
	TextureSequence texSeq1f0;
	TextureSequence texSeq204;
	TextureSequence texSeq218;
	MaterialChanger matChg22c;
	MaterialChanger matChg240;
	TextureSequence texSeq254;
	TextureSequence texSeq268;
	char* unk27c[4];
	char* unk28c[4];
	char* unk29c[4];
	ShadowVolume shadow;
	CylinderClsnWithPos cylClsn;
	CylinderClsnWithPos cylClsn2;
	Actor* shellPtr;
	Actor* actorInHands;
	unsigned unk35c;
	Actor* actorInMouth;
	unsigned unk364;
	ActorBase* speaker;
	unsigned unk36c;
	State* currState;
	State* prevState;
	State* nextState;
	unsigned unk37c;
	WithMeshClsn wmClsn;
	Vector3 unk53c;
	Vector3 unk540; //mirrors the player's position?
	unsigned unk554;
	unsigned unk558;
	unsigned unk55c;
	unsigned unk560;
	unsigned unk564;
	unsigned unk568;
	unsigned unk56c;
	unsigned unk570;
	unsigned unk574;
	char* unk578;
	char* unk57c;
	unsigned unk580;
	unsigned unk584;
	char* unk588;
	unsigned unk58c;
	unsigned unk590;
	unsigned unk594;
	unsigned unk598;
	unsigned unk59c;
	unsigned unk5a0;
	unsigned unk5a4;
	unsigned unk5a8;
	unsigned unk5ac;
	unsigned unk5b0;
	unsigned unk5b4;
	unsigned unk5b8;
	Matrix4x3 unkMat5bc;
	Matrix4x3 unkMat5ec;
	unsigned unk61c;
	unsigned unk620;
	unsigned unk624;
	unsigned unk628;
	unsigned unk62c;
	unsigned unk630;
	unsigned unk634;
	unsigned unk638;
	unsigned animID;
	unsigned unk640;
	Fix12i floorY;
	unsigned unk648;
	unsigned unk64c;
	unsigned unk650;
	unsigned unk654;
	unsigned floorTracID;
	unsigned floorCamBehavID;
	unsigned floorViewID;
	unsigned floorBehavID;
	unsigned unk668;
	unsigned floorTexID;
	unsigned floorWindID;
	unsigned unk674;
	unsigned unk678;
	unsigned unk67c;
	unsigned unk680;
	Fix12i jumpPeakHeight; // 0x684
	unsigned msgID;
	unsigned unk68c;
	unsigned unk690;
	unsigned unk694;
	unsigned unk698;
	unsigned unk69c;
	unsigned unk6a0;
	union
	{
		unsigned sleepTimer;
		unsigned runChargeTimer;
	};
	unsigned unk6a8;
	uint16_t unk6ac;
	uint16_t featherCapTimeRemaining; // 0x6AE
	unsigned unk6b0;
	unsigned unk6b4;
	unsigned unk6b8;
	uint16_t unk6bc;
	uint16_t powerupTimer;
	unsigned unk6c0;
	unsigned unk6c4;
	unsigned unk6c8;
	uint16_t unk6cc;
	uint16_t flags2;
	unsigned unk6d0;
	unsigned unk6d4;
	uint8_t playerID; //always 0 in single player mode
	uint8_t unk6d9;
	uint8_t unk6da;
	uint8_t unk6db;
	uint8_t prevHatChar; // 0x6DC
	uint8_t currHatChar; // 0x6DD
	bool isInAir;
	uint8_t unk6df;
	uint8_t unk6e0;
	uint8_t currJumpNumber; // 0x6E1: 0 - first, 1 - second, 2 - triple jump, more?
	uint8_t unk6e2;
	uint8_t stateState; // 0x6E3: the current state of the current state. How meta.
	uint8_t unk6e4;
	uint8_t canFlutterJump;
	uint8_t unk6e6;
	uint8_t unk6e7;
	uint8_t unk6e8;
	uint8_t currClsnState; // 0x06E9: 0 - not colliding, 1 - standing on ground, 2 - colliding with wall in water, 3 - colliding with wall on land 
	uint16_t unk6ea;
	unsigned unk6ec;
	unsigned unk6f0;
	unsigned unk6f4;
	bool isFireYoshi;
	bool isMetalWario;
	bool hasMetalModel;
	bool isVanishLuigi;
	unsigned unk6fc;
	uint16_t unk700;
	uint8_t unk702;
	bool isMega;
	uint8_t unk704;
	uint8_t unk705;
	bool isUnderwater;
	uint8_t unk707;
	uint8_t unk708;
	uint8_t unk709;
	uint16_t unk70a;
	unsigned unk70c;
	uint16_t unk710;
	uint8_t isInAirIsh; // 0x712
	uint8_t unk713;
	uint8_t unk714;
	uint8_t unk715;
	uint8_t unk716;
	uint8_t unk717;
	unsigned unk718;
	unsigned unk71c;
	unsigned unk720;
	unsigned unk724;
	unsigned unk728;
	unsigned unk72c;
	unsigned unk730;
	unsigned unk734;
	unsigned unk738;
	uint16_t toonStateAndFlag; //8 possible states, 0x8000 is the invincible-and-can't-collect-caps flag
	uint16_t unk73e;
	Fix12i toonIntensity;
	unsigned unk744;
	Vector3 lsPos; //0x748
	Vector3 lsInitPos; //0x754
	uint16_t unk760; 
	uint8_t lsState0Timer; //0x762
	uint8_t launchState; //0x763
	LaunchStar* lsPtr; //0x764
	union
	{
		BezierPathIter lsPathIt;
		struct
		{
			Vector3_16 lsDiffAng; //0x768
			Vector3_16 lsInitAng; //0x76e
		};
	};
	
	static SharedFilePtr* ANIM_PTRS[0x308];
	
	//implemented in LaunchStar.cpp
	bool LS_Init();
	bool LS_Behavior();
	bool LS_Cleanup();
	
	void IncMegaKillCount();
	void SetNewHatCharacter(unsigned character, unsigned arg1, bool makeSfx);
	void SetRealCharacter(unsigned character);
	void TurnOffToonShading(unsigned character);
	
	bool Unk_020bea94();
	unsigned GetBodyModelID(unsigned character, bool checkMetalStateInsteadOfWhetherUsingModel);
	void SetAnim(unsigned animID, int flags, Fix12i animSpeed, unsigned startFrame);
	void ShowMessage(ActorBase& speaker, unsigned msgIndex, const Vector3& lookAt, unsigned arg3, unsigned arg4);
	bool StartTalk(ActorBase& speaker, bool noButtonNeeded); //true iff the talk actually started.
	int GetTalkState();
	bool IsOnShell(); //if not on shell, reset shell ptr
	void Burn();
	void Shock(unsigned damage);
	void RegisterEggCoinCount(unsigned numCoins, bool includeSilverStar, bool includeBlueCoin);
	//speed is multiplied by constant at 0x020ff128+charID*2 and divided by 50 (? could be 25, could be 100).
	void Hurt(const Vector3& source, unsigned damage, Fix12i speed, unsigned arg4, unsigned presetHurt, unsigned spawnOuchParticles);
	void Heal(int health);
	void Bounce(Fix12i bounceInitVel);
	bool ChangeState(Player::State& state);
};

namespace Event
{
	void ClearBit(unsigned bit);
	void SetBit(unsigned bit);
	int  GetBit(unsigned bit);
}



enum class ttcClock : char
{
	SLOW = 0,
	FAST = 1,
	RANDOM = 2,
	STOP = 3
};

//used for keeping track of dead objects across level parts (e.g. THI big and small mountains)
struct ActorDeathTable
{
	byte deadObjs[64]; //technically 512 booleans
};

const int NUM_LEVELS = 52;
const int NUM_ACT_SELECTORS = 0x1e;
const int NUM_MAIN_LEVELS = 0xf;
struct Save0
{
	unsigned magic;
	uint8_t keys;
	uint8_t unk05;
	uint8_t unk06;
	uint8_t unk07;
	unsigned unk08;
	unsigned unk0c;
	unsigned unk10;
	uint8_t starsCollected[NUM_ACT_SELECTORS];
	uint8_t coinRecords[NUM_MAIN_LEVELS];
	uint8_t realCharacter;
};

struct ArchiveInfo
{
	char* archive;
	char* heap;
	uint16_t firstFileID;
	uint16_t firstNotFileID; //1 + lastFileID
	char* name;
	char* fileName;
};

struct Archive
{
	char magic[4];
	Archive* next;
	Archive* prev; //if this is first, it points to the ROM.
	unsigned unk0c;
	unsigned unk10;
	unsigned unk14;
	unsigned unk18;
	unsigned unk1c;
	unsigned unk20;
	unsigned unk24;
	unsigned unk28;
	unsigned unk2c;
	unsigned unk30;
	unsigned unk34;
	unsigned unk38;
	unsigned unk3c; //a function
	unsigned unk40; //a function
	unsigned unk44; //a function
	unsigned unk48;
	unsigned unk4c;
	char* header;
	char* FAT;
	char* fileBlock;
	unsigned unk5c;
	char data[];
};

struct ROM_Info
{
	char magic[4]; //"rom\0"
	Archive* firstArchive;
};

using EnemyDeathFunc = bool(Enemy::*)(WithMeshClsn& wmClsn);

//File ID 0x8zzz is file from archive 0 with id zzz
//020189f0: overlay 0 file ID to file ID and store
//020189f8: 00 10 b0 e1: movs r0, r1 (checks if file address is nullptr)
//02018a00: 0d 01 00 1b: blne 0x02018e3c
extern "C"
{
	extern char** DL_PTR_ARR_PTR;
	
	extern char LEVEL_PART_TABLE[NUM_LEVELS];
	extern char SUBLEVEL_LEVEL_TABLE[NUM_LEVELS];
	
	extern int ACTOR_BANK_OVL_MAP[7][7];
	extern int LEVEL_OVL_MAP[NUM_LEVELS];
	
	extern MsgGenTextFunc MSG_GEN_TEXT_FUNCS[3];
	
	extern char ACTOR_BANK_SETTINGS[7];
	
	extern Vector3 CAM_SPACE_CAM_POS_ASR_3; //constant <0.0, 64.0, -112.0>
	
	extern ArchiveInfo ARCHIVE_INFOS[13];
	
	extern int NEXT_UNIQUE_ID;
	
	extern Matrix4x3 VIEW_MATRIX_ASR_3;
	extern Matrix4x3 INV_VIEW_MATRIX_ASR_3;
	extern Vector3_16* ROT_AT_SPAWN;
	extern Actor::ListNode* FIRST_ACTOR_LIST_NODE;
	extern Save0 SAVE_DATA_0;
	
	extern bool IMMUNE_TO_DAMAGE;
	
	extern ttcClock TTC_CLOCK_SETTING;
	extern char LEVEL_ID;
	extern char STAR_ID;
	extern uint8_t MAP_TILE_ARR_SIZE;
	extern char NUM_LIVES;
	extern Area* AREAS;
	extern Camera* CAMERA;
	extern Fix12i WATER_HEIGHT;
	extern int EVENT_FIELD;
	extern short NUM_COINS[2];
	extern Player* PLAYER_ARR[4];
	
	extern Actor* SILVER_STARS[12];	
	extern ActorDeathTable ACTOR_DEATH_TABLE_ARR[3]; //maximum three parts per level.
	
	extern ActorBase* ROOT_ACTOR_BASE;
	
	extern uint16_t* DEATH_BY_GIANT_SPAWN_TABLE;
	
	extern ActorBase::ProcessingListNode* FIRST_BEHAVIOR_LIST_NODE;
	extern ActorBase::ProcessingListNode* FIRST_RENDER_LIST_NODE;
	extern EnemyDeathFunc ENEMY_DEATH_FUNCS[8];
	
	bool LoadArchive(int archiveID);
	
	char SublevelToLevel(char levelID);
	int NumStars();
	int IsStarCollected(int actID, int starID);
	
	int DeathTable_GetBit(char bit);
	
	char StarCollectedInCurrLevel(int starID);
	
	void UnloadBlueCoinModel();
	void LoadBlueCoinModel();
	void UnloadSilverStarAndNumber();
	void LoadSilverStarAndNumber();
	void LinkSilverStarAndStarMarker(Actor* starMarker, Actor* silverStar);
}

//Obj to Model Scale: Divide integer units by 8. (So 1.000 (Q20.12) becomes 1000 / 8 = 125.)

//0202df70 is the start of the load obj bank overlays function (0202df84 is where the cleanup branch is)
//0202e034 and 0202e06c are the ends of the load obj bank overlays function.
//Overlay 0x3c ( 60) gets loaded in a Bowser fight level. It is loaded at 0x02111900 and takes up 0x92e0 bytes of space.
//Overlay 0x62 ( 98) is loaded if the boolean at 0x0209f2d8 is false. It contains the small wooden box, the cannon, the water bomb, and arrow signs.
//Overlay 0x66 (102) is loaded, period. It contains the ? block, the bob-omb, and the koopa shell.

//Super mushroom tag vtable: 02108cf4

/*void Vec3_InterpCubic(Vector3* vF, Vector3* v0, Vector3* v1, Vector3* v2, Vector3* v3, int t) NAKED; //0208f670, 70f60822
bool BezPathIter_Advance(BezierPathIter* it) NAKED; //0208f840, 40f80822

void Vec3_Interp(Vector3* vF, Vector3* v1, Vector3* v2, int t) NAKED; //02090dd0, d00d0922
short Vec3_VertAngle(Vector3* v1, Vector3* v2) NAKED; //0203b770, 70b70322
short Vec3_HorzAngle(Vector3* v1, Vector3* v2) NAKED; //0203b7ac, acb70322
void Vec3_LslInPl(Vector3* vF, int amount) NAKED; //0203d0a0, a0d00322
int  Vec3_HorzDist(Vector3* v1, Vector3* v2) NAKED; //0203cf40, 40cf0322
int  Vec3_Dist(Vector3* v1, Vector3* v2) NAKED; //0203cfdc, dccf0322
void Vec3_MulInPl(Vector3* v, int scalar) NAKED; //0203d224, 24d20322
void Vec3_Sub(Vector3* vF, Vector3* v1, Vector3* v2) NAKED; //0203d2fc, fcd20322
void Vec3_Add(Vector3* vF, Vector3* v1, Vector3* v2) NAKED; //0203d340, 40d30322

void Vec3_MulMat3x3(Vector3* v, Matrix3x3* m, Vector3* vF) NAKED; //020525a0, a0250522
void Mat3x3_Mul(Matrix3x3* m2, Matrix3x3* m1, Matrix3x3* mF) NAKED; //02052624, 25260522
void Mat4x3_Identity(Matrix4x3* mF) NAKED; //020527c0, c0270522
void Vec3_MulMat4x3(Vector3* v, Matrix4x3* m, Vector3* vF) NAKED; //02052858, 58280522
void Mat4x3_Mul(Matrix4x3* m2, Matrix4x3* m1, Matrix4x3* mF) NAKED; //02052914, 14290522

void Model_Update(Model* mdl) NAKED; //0201686c, 6c680122
void Model_UpdateBones(char** mdlFilePtr, char* animFile, int currFrame) NAKED; //02045394, 94530422
void Anim_Change(Model* mdl, char* newAnimFile, Animation::Flags flags, int animSpeed, int startFrame) NAKED; //02016748, 48670122

bool Player_ChangeState(Player* player, Player::State* state) NAKED; //020e30a0, a0300e22
void Player_ChangeAnim(Player* player, int animID, Animation::Flags flags, int animSpeed, int startFrame) NAKED;//020bef2c, 2cef0b22
void Sound_Play0(int soundID, Vector3* camSpacePos) NAKED; //0201264c, 4c260122
void Sound_Play3(int soundID, Vector3* camSpacePos) NAKED; //02012664, 64260122
void Sound_Play(int arg0, int soundID, Vector3* camSpacePos); //02012590, 90250122*/
#endif // SM64DS_2_H_INCLUDED
