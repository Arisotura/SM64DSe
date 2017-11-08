#ifndef SM64DS_MODEL_INCLUDED
#define SM64DS_MODEL_INCLUDED

#include "SM64DS_Common.h"

struct Model;
struct ModelAnim;
struct MaterialChanger;
struct TextureSequence;
struct ShadowVolume;
extern "C"
{
	//Remember to have these loaded before spawning an object with them
	//They are as disorganized here as they are in memory.
	extern SharedFilePtr LUIGI_CAP_MODEL_PTR;
	extern SharedFilePtr NUMBER_MODEL_PTR;
	extern SharedFilePtr POWER_FLOWER_OPEN_MODEL_PTR;
	extern SharedFilePtr COIN_YELLOW_POLY32_MODEL_PTR;
	extern SharedFilePtr WARIO_CAP_MODEL_PTR;
	extern SharedFilePtr COIN_BLUE_POLY32_MODEL_PTR;
	extern SharedFilePtr POWER_FLOWER_CLOSED_MODEL_PTR;
	extern SharedFilePtr ONE_UP_MUSHROOM_MODEL_PTR;
	extern SharedFilePtr BOB_OMB_MODEL_PTR;
	extern SharedFilePtr NUMBER_TEXSEQ_PTR;
	extern SharedFilePtr SNUFIT_BULLET_MODEL_PTR;
	extern SharedFilePtr COIN_RED_POLY32_MODEL_PTR;
	extern SharedFilePtr COIN_BLUE_POLY4_MODEL_PTR;
	extern SharedFilePtr SILVER_NUMBER_TEXSEQ_PTR;
	extern SharedFilePtr WATER_RING_MODEL_PTR;
	extern SharedFilePtr SHELL_GREEN_MODEL_PTR;
	extern SharedFilePtr SHELL_RED_MODEL_PTR;
	extern SharedFilePtr SILVER_NUMBER_MODEL_PTR;
	extern SharedFilePtr SUPER_MUSHROOM_MODEL_PTR;
	extern SharedFilePtr BUBBLE_MODEL_PTR;
	extern SharedFilePtr MARIO_CAP_MODEL_PTR;
	extern SharedFilePtr COIN_YELLOW_POLY4_MODEL_PTR;
	extern SharedFilePtr COIN_RED_POLY4_MODEL_PTR;
	extern SharedFilePtr FEATHER_MODEL_PTR;
	
	//Graphics ports
	//Do NOT read from the ports!
	extern volatile unsigned GXPORT_MATRIX_MODE;
	extern volatile unsigned GXPORT_MTX_LOAD_4x4;
	extern volatile unsigned GXPORT_MTX_LOAD_4x3;
	extern volatile unsigned GXPORT_LIGHT_VECTOR;
	extern volatile unsigned GXPORT_LIGHT_COLOR;
	
	extern uint16_t CHANGE_CAP_TOON_COLORS[0x20];
	
	MaterialChanger* MatChg_Construct(MaterialChanger* unk); //constructor
	TextureSequence* TexSeq_Construct(TextureSequence* where);
	ShadowVolume* Shadow_Construct(ShadowVolume* shadow); //constructor
	ModelAnim* ModelAnim_Construct(ModelAnim* mdlAnim); //constructor
	Model* Model_Construct(Model* where);
}

namespace Vram
{
	void StartTexWrite();
	void LoadTex(uint8_t* texelArr, unsigned texVramOffset, unsigned texelArrSize);
	void EndTexWrite();
	void StartPalWrite();
	void LoadPal(uint16_t* palColArr, unsigned palVramOffset, unsigned palleteSize);
	void EndPalWrite();
};

namespace GXFIFO
{
	inline void LoadMatrix4x3(const Matrix4x3* matrix)
	{
		GXPORT_MTX_LOAD_4x3 = matrix->r0c0.val;		GXPORT_MTX_LOAD_4x3 = matrix->r1c0.val;		GXPORT_MTX_LOAD_4x3 = matrix->r2c0.val;
		GXPORT_MTX_LOAD_4x3 = matrix->r0c1.val;		GXPORT_MTX_LOAD_4x3 = matrix->r1c1.val;		GXPORT_MTX_LOAD_4x3 = matrix->r2c1.val;
		GXPORT_MTX_LOAD_4x3 = matrix->r0c2.val;		GXPORT_MTX_LOAD_4x3 = matrix->r1c2.val;		GXPORT_MTX_LOAD_4x3 = matrix->r2c2.val;
		GXPORT_MTX_LOAD_4x3 = matrix->r0c3.val;		GXPORT_MTX_LOAD_4x3 = matrix->r1c3.val;		GXPORT_MTX_LOAD_4x3 = matrix->r2c3.val;
	}

	//Do NOT set the light vector to <1, 0, 0>, <0, 1, 0>, or <0, 0, 1>. Instead, do <0x0.ff8, 0, 0>, for example.
	inline void SetLightVector(int lightID, Fix12i x, Fix12i y, Fix12i z) //Fixed Point 20.12
	{
		GXPORT_LIGHT_VECTOR = (((z.val >> 3 & 0x1ff) | (z.val >> 22 & 0x200)) << 10 |
								(y.val >> 3 & 0x1ff) | (y.val >> 22 & 0x200)) << 10 |
								(x.val >> 3 & 0x1ff) | (x.val >> 22 & 0x200) | lightID << 30;
	}

	inline void SetLightColor(int lightID, uint8_t r, uint8_t g, uint8_t b) //0x00 to 0xff
	{
		GXPORT_LIGHT_COLOR = (unsigned)b >> 3 << 10 |
							 (unsigned)g >> 3 <<  5 |
							 (unsigned)r >> 3 | lightID << 30;
	}
}

struct MaterialProperties
{
	short materialID;
	short unk02; //probably just padding
	const char* materialName;
	
	char unk08; bool difRedAdv;     uint16_t difRedOffset;
	char unk0c; bool difGreenAdv;   uint16_t difGreenOffset;
	char unk10; bool difBlueAdv;    uint16_t difBlueOffset;
	char unk14; bool ambRedAdv;     uint16_t ambRedOffset;
	char unk18; bool ambGreenAdv;   uint16_t ambGreenOffset;
	char unk1c; bool ambBlueAdv;    uint16_t ambBlueOffset;
	char unk20; bool specRedAdv;    uint16_t specRedOffset;
	char unk24; bool specGreenAdv;  uint16_t specGreenOffset;
	char unk28; bool specBlueAdv;   uint16_t specBlueOffset;
	char unk2c; bool emitRedAdv;    uint16_t emitRedOffset;
	char unk30; bool emitGreenAdv;  uint16_t emitGreenOffset;
	char unk34; bool emitBlueAdv;   uint16_t emiBlueOffset;
	char unk38; bool alphaAdv;      uint16_t alphaOffset;
};

struct MaterialDef
{
	uint16_t numFrames;
	uint16_t unk02;
	char* values;
	int matPropCount;
	MaterialProperties* matProp;
};

struct TexSRTAnim
{
	uint16_t materialID;
	uint16_t unk02; //probably just padding
	const char* materialName;
	uint16_t numScaleXs;
	uint16_t scaleXOffset;
	uint16_t numScaleYs;
	uint16_t scaleYOffset;
	uint16_t numRots;
	uint16_t rotOffset;
	uint16_t numTransXs;
	uint16_t transXOffset;
	uint16_t numTransYs;
	uint16_t transYOffset;
	
};

struct TexSRTDef
{
	unsigned numFrames;
	Fix12i* scales;
	short* rots;
	Fix12i* transs;
	int texAnimCount;
	TexSRTAnim* texAnims;
};

struct Bone
{
	unsigned unk00;
	unsigned unk04;
	unsigned unk08;
	Vector3 scale;
	uint16_t unk18;
	Vector3_16 rot;
	Vector3 pos;
	unsigned unk2c;
	unsigned unk30;
};

struct Animation
{
    enum Flags : int
    {
        LOOP = 0x00000000,
        NO_LOOP = 0x40000000
    };
	
	unsigned* vTable; //take off once a virtual function is added
	Fix12i numFramesAndFlags;
	Fix12i currFrame;
	Fix12i speed;
	char* file;
	
	inline Fix12i GetNumFrames() {return Fix12i(numFramesAndFlags.val & 0x0fffffff, true);}
	void Advance();
	bool Finished();
};

struct FrameCtrl //internal name; ???
{
    enum Flags : int
    {
        LOOP = 0x00000000,
        NO_LOOP = 0x40000000
    };
	
	unsigned* vTable;
	Fix12i numFramesAndFlags;
	Fix12i currFrame;
	Fix12i speed;
};

struct BoneAnimation : Animation
{
    static char* LoadFile(SharedFilePtr& filePtr);
};

struct Material
{
	unsigned unk00;
	unsigned unk04;
	Fix12i texScaleX;
	Fix12i texScaleY;
	short texRot; //then alignment
	Fix12i texTransX;
	Fix12i texTransY;
	unsigned teximageParam; //gx command 0x2a
	unsigned paletteInfo;
	unsigned polygonAttr; //gx command 0x29
	unsigned difAmb; //gx command 0x30
	unsigned speEmi; //gx command 0x31
};

struct ModelComponents
{
	char* modelFile;
	Material* materials;
	Bone* bones;
	Matrix4x3* transforms;
	char* unk10;
	
	void UpdateBones(char* animFile, int frame);
	void UpdateVertsUsingBones();
};

//same structure as Animation
struct MaterialChanger : Animation
{
	MaterialChanger();
	~MaterialChanger();
    static void Prepare(char* modelFile, MaterialDef& matDef);
	void SetFile(MaterialDef& matDef, int flags, Fix12i speed, unsigned startFrame);
	void Update(ModelComponents& modelData);
};

//same structure as Animation
struct TextureTransformer : Animation
{
	TextureTransformer();
	~TextureTransformer();
    static void Prepare(char* modelFile, TexSRTDef& texDef);
	void SetFile(TexSRTDef& texDef, int flags, Fix12i speed, unsigned startFrame);
	void Update(ModelComponents& modelData);
};

//same structure as Animation
struct TextureSequence : Animation
{
	TextureSequence();
	~TextureSequence();
    static void Prepare(char* modelFile, char* texSeqFile);
	void SetFile(char* texSeqFile, int flags, Fix12i speed, unsigned startFrame);
	void Update(ModelComponents& modelData);	
	static char* LoadFile(SharedFilePtr& filePtr);
};

struct Model
{
	//vtable
	unsigned unk04;
	ModelComponents data;
	Matrix4x3 mat4x3;
	Matrix4x3* unkMatPtr;
	
	Model();
	virtual ~Model();
	virtual unsigned Virtual08(unsigned arg0, unsigned arg1, unsigned arg2);
	virtual void UpdateVerts();
	virtual void Virtual10(Matrix4x3& arg0);
	virtual void Render(const Vector3* scale);
	
	bool SetFile(char* file, int arg1, int arg2);
	static char* LoadFile(SharedFilePtr& filePtr);
};

struct ModelAnim : public Model
{
	BoneAnimation anim;
	
	ModelAnim();
	virtual ~ModelAnim();
	virtual void UpdateVerts() override;
	virtual void Virtual10(Matrix4x3& arg0) override;
	virtual void Render(const Vector3* scale) override;
	
	void SetAnim(char* animFile, int flags, Fix12i speed, unsigned startFrame);
};

struct ModelAnim2 : public ModelAnim
{
	BoneAnimation otherAnim; //???
	FrameCtrl frameCtrl; //???
	
	//the last two arguments can be nullptr; this just means they'll be obtained from the other ModelAnim2.
	void CopyAnim(ModelAnim2& other, char* newAnimFile, char* newOtherAnimFile);
};

struct ShadowVolume
{
	unsigned* vTable;
	unsigned unk04;
	ModelComponents* modelDataPtr;
	Matrix4x3* matPtr;
	Vector3 scale;
	unsigned unk1c;
	ShadowVolume* prev;
	ShadowVolume* next;
	
	ShadowVolume();
	~ShadowVolume();
	bool InitCylinder();
	bool InitCuboid();
};



#endif