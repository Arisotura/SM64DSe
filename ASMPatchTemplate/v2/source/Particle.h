#include "SM64DS_Common.h"


namespace Particle
{
	enum ParticleID
	{
		PT_SPARKLES_1UP = 0x00,
		PT_BUBBLE_CLEAR = 0x01,
		PT_BUBBLES_CLEAR = 0x02, //Not to be confused with 1 bubble, above
		PT_GRAY_TRIANGLES_1 = 0x03,
		PT_GRAY_TRIANGLES_2 = 0x04,
		PT_GRAY_TRIANGLES_3 = 0x05,
		PT_GOLD_TRIANGLES = 0x06,
		PT_BUBBLES_WHITE_BIG = 0x07,
		PT_BUBBLES_WHITE_SMALL = 0x08,
		PT_STARS_PINK_BLUE_GREEN = 0x09,
		PT_FLYING_DUST = 0xd1,
		PT_COIN_SPARKLES = 0xd2,
		PT_DUST = 0xda,
		PT_RIPPLE_CONTINUOUS = 0xe9,
		PT_SNOW = 0x112,
		PT_SPARKLES_1 = 0x114,
		PT_SPARKLES_POWER_STAR = 0x115,
		PT_SMOKE = 0x13a,
		PT_FIRE_ORANGE = 0x13b,
	};
	
	enum Sprite
	{
		SP_LIGHTNING = 0x00,
		SP_LIGHTNING_1 = 0x01,
		SP_BUBBLE = 0x02,
		SP_BROWN_CLAM_THINGY = 0x03,
		SP_FIRE_ORANGE = 0x04,
		SP_FIRE_BLUE = 0x05,
		SP_FIRE_ANIM_BLUE_0 = 0x06,
		SP_FIRE_ANIM_BLUE_1 = 0x07,
		SP_FIRE_ANIM_BLUE_2 = 0x08,
		SP_FIRE_ANIM_BLUE_3 = 0x09,
		SP_FIRE_ANIM_BLUE_4 = 0x0a,
		SP_FIRE_ANIM_BLUE_5 = 0x0b,
		SP_FIRE_ANIM_BLUE_6 = 0x0c,
		SP_FIRE_ANIM_BLUE_7 = 0x0d,
		SP_FIRE_ANIM_ORANGE_0 = 0x0e,
		SP_FIRE_ANIM_ORANGE_1 = 0x0f,
		SP_FIRE_ANIM_ORANGE_2 = 0x10,
		SP_FIRE_ANIM_ORANGE_3 = 0x11,
		SP_FIRE_ANIM_ORANGE_4 = 0x12,
		SP_FIRE_ANIM_ORANGE_5 = 0x13,
		SP_FIRE_ANIM_ORANGE_6 = 0x14,
		SP_FIRE_ANIM_ORANGE_7 = 0x15,
		SP_CIRCLE = 0x16, //must be mirrored both ways
		SP_CIRCLE_ORANGE = 0x17, //must be mirrored both ways
		SP_CIRCLE_GRADIENT_SHARP = 0x18, //must be mirrored both ways
		SP_CIRCLE_GRADIENT_FADE = 0x19, //must be mirrored both ways
		SP_SPARK = 0x1a, //must be mirrored both ways
		SP_SPINDRIFT_PETAL = 0x1b, //must be mirrored horizontally
		SP_SPARKLE = 0x1c, //must be mirrored both ways
		SP_BUBBLE_WEIRD = 0x1d, //must be mirrored horizontally
		SP_WEIRD_SPIKY_THINGY = 0x1e, //must be mirrored horizontally
		SP_DOTS_PINK = 0x1f,
		SP_LEAF = 0x20, //must be mirrored horizontally
		SP_WIND_GUST = 0x21,
		SP_WIND_GUST_1 = 0x22,
		SP_WIND_GUST_2 = 0x23,
		SP_TRIANGLE = 0x24,
		SP_BUBBLE_1 = 0x25, //must be mirrored both ways
		SP_FLOWER_OUTLINE = 0x26, //must be mirrored both ways
		SP_STONE = 0x27,
		SP_CLOUD = 0x28,
		SP_SNOW = 0x29,
		SP_SNOW_WEIRD = 0x2a,
		SP_SNOW_1 = 0x2b, //must be mirrored horizontally
		SP_STAR = 0x2c, //must be mirrored horizontally
		SP_DOTS = 0x2d,
	};
	
	struct MainInfo
	{
		enum Flags
		{
			MODE = 0x7 << 0,
				FROM_POINT = 0,
				FROM_PLANE = 1,
				FROM_PLANE_2 = 2,
				FROM_POINT_3 = 3,
				FROM_TORUS = 4,
				FROM_PLANE_5 = 5,
				
			MODE_3D = 0x3 << 4,
				BILLBOARD   = 0x0 << 4,
				VEL_STRETCH = 0x1 << 4,
				THREE_D     = 0x2 << 4,
				CRASH	    = 0x3 << 4,
			ROTATE_SPAWN_CIRCLE_HORZ = 1 << 6,
			ROTATE_SPAWN_CIRCLE_VERT = 1 << 7,
			
			HAS_SCALE_TRANS = 1 << 8,
			HAS_COLOR_TRANS = 1 << 9,
			HAS_ALPHA_TRANS = 1 << 10,
			HAS_INFO4_TRANS = 1 << 11,
			ROTATE = 1 << 12,
			FACE_CAMERA = 1 << 13, //???
			
			HAS_GLITTER = 1 << 16,
			ROTATE_AROUND_RANDOM_AXIS = 1 << 17, //???
			CRASH_18 = 1 << 18,
			HORZ_IF_3D = 1 << 19,
			
			HAS_EFFECT_DRIFT    = 1 << 24,
			HAS_EFFECT_BROWNIAN = 1 << 25,
			HAS_EFFECT_2        = 1 << 26,
			HAS_EFFECT_3        = 1 << 27,
			HAS_EFFECT_4        = 1 << 28,
			HAS_EFFECT_5        = 1 << 29
		};
		
		unsigned flags; //0x0c004345 for particle 0x00bc
		Fix12i rate; //in particles per frame
		Fix12i  startHorzDist;
		Vector3_16f dir; //0x0c, ???
		uint16_t color;
		Fix12i horzSpeed; //restart system for effect
		Fix12i vertSpeed; //positive means go down, for some reason //restart system for effect
		Fix12i scale; //restart system for effect
		Fix12s horzScale; //milestone: 0x20
		uint16_t unk22; //0x00e7 for particle 0x00bc
		short minAngSpeed; //0x00000000 for particle 0x00bc
		short maxAngSpeed;
		uint16_t frames; //of the system, 0 = infinity
		uint16_t lifetime; //of the individual particles
		uint8_t scaleRand;
		uint8_t lifetimeRand;
		uint8_t speedRand;
		uint8_t unk2f; //0x00 for particle 0x00bc, probably padding
		uint8_t spawnPeriod; //number of frames between spawns (WARNING: DENOMINATOR)
		uint8_t alpha; //on a scale of 0x00 to 0x1f
		uint8_t speedFalloff; //higher means faster and for longer. Below 0x80: slow down, above 0x80: speed up
		uint8_t spriteID;
		uint8_t effectLength; //0x01 for particle 0x00bc (WARNING: DENOMINATOR)
		uint8_t unk35;
		uint8_t velStretchFactor; //respective flag must be enabled
		uint8_t texMirrorFlags; //1, 2, 3: horizontally x 2^n, 4, 8, 0xc: vertically x 2^(n/4)
	};

	struct ScaleTransition
	{
		Fix12s scaleStart;
		Fix12s scaleMiddle;
		Fix12s scaleEnd;
		uint8_t scaleTrans1End; //from 0x00 to 0xff
		uint8_t scaleTrans2Start;
		uint16_t unk08; //least significant byte is 1 means that there is no transition. It will start at scaleEnd.
		uint16_t unk0a;
	};
	
	struct ColorTransition
	{
		uint16_t colorStart;
		uint16_t colorEnd; //colorMiddle is in the main info
		uint8_t colorTrans1Start;
		uint8_t colorTrans2Start;
		uint8_t colorTrans2End;
		uint8_t unk07;
		uint8_t interpFlags;
		uint8_t unk09;
		uint16_t unk0a;
	};
	
	struct AlphaTransition
	{
		uint16_t alpha; //3 alphas 5 bits each
		uint8_t flicker;
		bool useLastAlpha; //???
		uint8_t alphaTrans1End;
		uint8_t alphaTrans2Start;
		uint16_t unk06;
	};
	
	struct Glitter
	{
		uint16_t unk00;
		uint16_t distRand;
		Fix12s scale;
		uint16_t lifetime;
		uint8_t unk08;
		uint8_t scale2;
		uint16_t unkColor;
		uint8_t rate;
		uint8_t unk0d;
		uint8_t period;
		uint8_t spriteID;
		unsigned texMirrorFlags;
	};
	
	struct EffectData {};
	using EffectFuncPtr = void(*)(EffectData& data, char*, Vector3& velAsr4);
	struct Drift : public EffectData
	{
		static void Func(EffectData& data, char*, Vector3& velAsr4);
		Vector3_16f sysVelAsr4;
		constexpr Drift(const Vector3_16f& velAsr4) : sysVelAsr4(velAsr4) {}
	};
	struct Brownian : public EffectData
	{
		static void Func(EffectData& data, char*, Vector3& velAsr4);
		Vector3_16f mag;
		uint16_t period; //number of frames between velocity changes (WARNING: DENOMINATOR)
	};
	struct Effect2 : public EffectData
	{
		static void Func(EffectData& data, char*, Vector3& velAsr4);
	};
	struct Effect3 : public EffectData
	{
		static void Func(EffectData& data, char*, Vector3& velAsr4);
	};
	struct Effect4 : public EffectData
	{
		static void Func(EffectData& data, char*, Vector3& velAsr4);
	};
	struct Effect5 : public EffectData
	{
		static void Func(EffectData& data, char*, Vector3& velAsr4);
	};
	
	struct Effect
	{
		EffectFuncPtr func;
		EffectData* data;
	};

	struct SysDef
	{
		MainInfo* info;
		ScaleTransition* scaleTrans;
		ColorTransition* colorTrans;
		AlphaTransition* alphaTrans;
		void* info4; //SysInfo4* info4;
		Glitter* glitter;
		Effect* effects;
		uint16_t numEffects;
	};
	
	enum TexFlags
	{
		FORMAT = 0x7 << 0,
			A3I5 = 0x1 << 0,
			COLOR_4 = 0x2 << 0,
			COLOR_16 = 0x3 << 0,
			COLOR_256 = 0x4 << 0,
			TEXEL_4x4 = 0x5 << 0,
			A5I3 = 0x6 << 0,
			DIRECT = 0x7 << 0,
		LOG_2_WIDTH_MINUS_3 = 0xf << 4, //right shift 4 before using this
		LOG_2_HEIGHT_MINUS_3 = 0xf << 8, //right shift 8 before using this
		REPEAT_S = 0x1 << 12,
		REPEAT_T = 0x1 << 13,
		FLIP_S = 0x1 << 14,
		FLIP_T = 0x1 << 15,
		FIRST_COLOR_TRANSP = 0x1 << 16
	};
	
	struct Texture
	{
		unsigned magic;
		TexFlags flags;
		unsigned texelArrSize;
		unsigned palleteOffset;
		unsigned palleteSize;
		unsigned unk14;
		unsigned unk18;
		unsigned totalSize;
		
		inline uint8_t* TexelArr() {return (uint8_t*)((char*)this + 0x20);}
		inline uint16_t* PalleteColArr() {return (uint16_t*)((char*)this + palleteOffset);}
		
		inline unsigned Format() {return flags & FORMAT;}
		inline uint16_t Width() {return 1 << (((flags & LOG_2_WIDTH_MINUS_3) >> 4) + 3);}
		inline uint16_t Height() {return 1 << (((flags & LOG_2_HEIGHT_MINUS_3) >> 8) + 3);}
		
		static unsigned AllocTexVram(unsigned size, bool isTexel4x4);
		static unsigned AllocPalVram(unsigned size, bool is4Color);
	};
	
	struct TexDef
	{
		Texture* texture;
		unsigned texVramOffset;
		unsigned palVramOffset;
		TexFlags flags;
		uint16_t width;
		uint16_t height;
	};
	
	struct Particle
	{
		enum Flags
		{
			ALPHA = 0x1f << 0
		};
		
		struct ListNode
		{
			Particle* next;
			Particle* prev;
		};
		ListNode node;
		Vector3 posAsr3;
		Vector3 offsetAsr3;
		Vector3 speedAsr3;
		uint16_t lifetime;
		uint16_t age;
		Fix12i scale;
		Fix12s unk34; //starts at 0x1000
		short ang;
		short angSpeed;
		uint16_t color;
		uint16_t unk3c;
		uint16_t lifetimeInv;
		unsigned flags;
	};
	struct List
	{
		Particle* first;
		unsigned num;
	};
	
	struct System;
	struct Callback
	{
		unsigned unk04;
		unsigned id;
		
		virtual void SpawnParticles(System&);
		virtual bool OnUpdate(System&, bool active);
	};
	struct SimpleCallback : public Callback
	{
		SimpleCallback();
		virtual void SpawnParticles(System&);
		virtual bool OnUpdate(System&, bool active) override;
	};
	struct SplashCallback : public SimpleCallback
	{
		virtual bool OnUpdate(System&, bool active) override;
	};
	
	struct System
	{
		struct ListNode
		{
			System* prev;
			System* next;
		};
		ListNode node;
		List particleList;
		List particleList2;
		SysDef* sysDefPtr;
		unsigned unk1c;
		Vector3 posAsr3;
		unsigned unk2c;
		uint16_t unk30;
		uint8_t unk32;
		uint8_t unk33;
		unsigned unk34;
		uint16_t unk38;
		int16_t rateTracker;
		Vector3_16f dir;
		uint16_t unk42;
		Fix12i startHorzDist;
		Fix12i horzSpeed;
		Fix12i vertSpeed;
		Fix12i scale;
		uint16_t lifetime;
		uint16_t unk56;
		uint8_t spawnPeriod;
		uint8_t alpha;
		uint16_t unk5a;
		unsigned unk5c;
		unsigned unk60;
		unsigned unk64;
		unsigned unk68;
		unsigned unk6c;
		unsigned unk70;
		unsigned unk74;
		
		static unsigned NewWeather(unsigned uniqueID, unsigned effectID, Fix12i x, Fix12i y, Fix12i z, const Vector3_16f* dir, unsigned numWeatherEffectsNow);
		static unsigned NewRipple(Fix12i x, Fix12i y, Fix12i z);
		static unsigned New(unsigned uniqueID, unsigned effectID, Fix12i x, Fix12i y, Fix12i z, const Vector3_16f* dir, Callback* callback);
		static void NewSimple(unsigned particleID, Fix12i x, Fix12i y, Fix12i z);
	};
	
	struct ROMEmbeddedFile
	{
		uint64_t magic;
		uint16_t numSysDefs;
		uint8_t numTexs;
		uint8_t numBuiltInTexs;
		MainInfo firstSysDef;
		
		inline static MainInfo& nextSysDef(const MainInfo& sysDef)
		{
			const char* ptr = (const char*)&sysDef;
			return *(MainInfo*)(ptr + 0x38 +
							    (sysDef.flags & MainInfo::HAS_SCALE_TRANS ? 0x0c : 0x00) +
								(sysDef.flags & MainInfo::HAS_COLOR_TRANS ? 0x0c : 0x00) +
								(sysDef.flags & MainInfo::HAS_ALPHA_TRANS ? 0x08 : 0x00) +
								(sysDef.flags & MainInfo::HAS_INFO4_TRANS ? 0x0c : 0x00) +
								(sysDef.flags & MainInfo::HAS_GLITTER     ? 0x14 : 0x00) +
								
								(sysDef.flags & MainInfo::HAS_EFFECT_DRIFT    ? 0x08 : 0x00) +
								(sysDef.flags & MainInfo::HAS_EFFECT_BROWNIAN ? 0x08 : 0x00) +
								(sysDef.flags & MainInfo::HAS_EFFECT_2        ? 0x10 : 0x00) +
								(sysDef.flags & MainInfo::HAS_EFFECT_3        ? 0x04 : 0x00) +
								(sysDef.flags & MainInfo::HAS_EFFECT_4        ? 0x08 : 0x00) +
								(sysDef.flags & MainInfo::HAS_EFFECT_5        ? 0x10 : 0x00)
							   );
		}
	};
	
	class TexROMEmbeddedFile
	{
		
	};
	
	struct Manager
	{
		using AdvancePtrFunc = void*(*)(unsigned amount); //returns the pointer before the advancing, advances 0x0209ee78
		
		struct SysList
		{
			System* last;
			unsigned num;
		};		
		
		void*(*advancePtr)(unsigned amount);
		SysList usedSysList;
		SysList freeSysList;
		List freeParticleList; //???
		SysDef* sysDefArr;
		TexDef* texDefArr;
		uint16_t numSysDefs;
		uint8_t numTextures;
		uint8_t numBuiltInTexs;
		uint16_t unk2c;
		uint16_t unk2e;
		
		System* AddSystem(int particleID, Vector3& posAsr3);
		static bool LoadTex(unsigned fileID, unsigned newTexID);
		static void UnloadNewTexs();
	};
	
	struct SysTracker
	{
		struct Data
		{
			unsigned uniqueID; //1-indexed, not 0-indexed
			unsigned effectID;
			uint16_t frames;
			uint8_t unk0a;
			System* system;
			unsigned** unk10; // a vtable?
			Data* prevInLink;
			Data* nextInLink;
		};
		
		struct UnkStr2
		{
			unsigned* vTable;
			unsigned unk04;
			unsigned* vTable2;
			unsigned* unk0c;
		};
		
		struct Contents
		{
			unsigned numComplexSysSpawned;
			unsigned lastUsedSysSlotIndex;
			Data data[0x40];
			Data* usedSystems[0x10];
			unsigned unk750;
			Callback callbacks[0x08];
			UnkStr2 unkStr2s[0x04];
			unsigned* unk7f4;
			unsigned* unk7f8;
			Fix12i unk7fc;
			unsigned* unk800;
			Fix12i unk804;
			unsigned* unk808;
			unsigned unk80c;
			unsigned* unk810;
			unsigned unk814;
			unsigned* unk818;
		};
		
		ROMEmbeddedFile* romFile;
		Manager* manager;
		Contents contents;
	};
}

extern "C"
{
	extern Particle::ROMEmbeddedFile PARTICLE_ROM_EMBEDDED_FILE;
	extern Particle::TexROMEmbeddedFile TEX_ROM_EMBEDDED_FILE;
	extern Particle::SysTracker* PARTICLE_SYS_TRACKER;
	extern unsigned PARTICLE_RNG_STATE; //x => x * 0x5eedf715 + 0x1b0cb173
}
