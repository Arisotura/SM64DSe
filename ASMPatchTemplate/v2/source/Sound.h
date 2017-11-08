#ifndef SM64DS_SOUND_INCLUDED
#define SM64DS_SOUND_INCLUDED

#include "SM64DS_Common.h"

struct ROM_Info;
namespace Sound
{
	enum RecordType
	{
		RC_SEQUENCE,
		RC_SEQUENCE_ARCHIVE,
		RC_INSTRUMENT_BANK,
		RC_WAVE_ARCHIVE,
		RC_PLAYER, //???
		RC_GROUP,
		RC_PLAYER2, //???
		RC_STREAM //D.N.E. (Does Not Exist) for SM64DS
	};
	
	struct SDAT_Header
	{
		struct Block
		{
			unsigned offset;
			unsigned size;
		};
		char magic[4]; //"SDAT"
		unsigned x0100feff;
		unsigned fileSize;
		uint16_t headerSize;
		uint16_t numBlocks;
		Block symbolBlock;
		Block infoBlock;
		Block FAT;
		Block fileBlock;		
	};
		
	struct SymbolBlock
	{
		
	};
	
	struct InfoSequenceEntry
	{
		unsigned fileID;
		uint16_t bank;
		uint8_t volume;
		uint8_t channelPressure; //???
		uint8_t polyphonicPressure; //???
		uint8_t play; //???
		uint16_t padding;
		
		static InfoSequenceEntry* GetWithID(unsigned seqID);
	};
	
	struct InfoInstrumentBankEntry
	{
		unsigned fileID;
		uint16_t waveArchiveIDs[4];
		
		static InfoInstrumentBankEntry* GetWithID(unsigned bankID);
	};
	
	struct InfoBlockRecord
	{
		unsigned count;
		unsigned entryOffsets[]; //size = count
	};
	
	struct InfoBlock
	{
		char magic[4]; //"INFO"
		unsigned size;
		unsigned recordOffsets[8];
		char padding[0x18];
	};
	
	struct FAT
	{
		
	};
	
	struct FileBlock
	{
		
	};
	
	struct Sequence
	{
		
	};
	
	struct InstrumentBank
	{
		
	};
	
	struct WaveArchive
	{
		char magic[4]; //"SWAR"
		unsigned x0100feff;
		unsigned fileSize;
		uint16_t headerSize;
		uint16_t one;
	};
	
	struct SDAT_RAM
	{
		SDAT_Header header;
		unsigned unk30;
		unsigned unk34;
		ROM_Info* rom;
		unsigned unk3c;
		unsigned unk40;
		unsigned unk44;
		unsigned unk48;
		unsigned unk4c;
		unsigned unk50;
		unsigned unk54;
		unsigned unk58;
		WaveArchive* waveArchive;
		unsigned waveArchiveSize;
		unsigned unk64; //waveArchiveSize copy?
		unsigned unk68;
		unsigned unk6c;
		unsigned unk70;
		unsigned unk74; //ROM?
		unsigned unk78;
		FAT* fileAllocTable;
		unsigned unk80;
		InfoBlock* infoBlock;
		
		static SDAT_RAM* PTR;
		
	};

	
	struct SequenceArchive
	{
		char magic[4]; //"SSAR"
		uint16_t endianCode; //0xfeff
		uint16_t unk6;
		unsigned size; //includes header
		unsigned dataOffset; //0x10
		unsigned unke;
		
		struct Data
		{
			char magic[4]; //"DATA"
			unsigned size;
			unsigned sampleDefSize;
			unsigned numSamples;
			
			struct SampleDef
			{
				unsigned unk0;
				uint16_t bankID;
				uint8_t volume;
				uint8_t unk7;
				uint8_t unk8;
				uint8_t unk9;
				uint8_t unka;
				uint8_t unkb;
			} sampleDefs[];
			
		} data;
	};
	
	struct FileRef //guess
	{
		FileRef** unk0;
		Player* soundPlayer;
		char* file;
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
		uint8_t unk3c;
		uint8_t unk3d;
		uint8_t unk3e;
		uint8_t unk3f;
		
		static FileRef* PTR_0; //there's one after it, but it's, as far as I know, not referred to exactly.
	};
	
	struct Player //not to be confused with ::Player
	{
		FileRef* fileRef;
		FileRef* fileRefCopy;
		uint16_t unk08;
		uint16_t unk0a;
		char* unk0c;
		char* unk10;
		unsigned unk14;
		unsigned unk18;
	};
	
	enum MusicID
	{
		MU_STAR_SELECT = 0x16,
		MU_SILVER_STAR_GET_1 = 0x19,
		MU_SILVER_STAR_GET_2 = 0x1a,
		MU_SILVER_STAR_GET_3 = 0x1b,
		MU_SILVER_STAR_GET_4 = 0x1c,
		MU_SILVER_STAR_GET_5 = 0x1d,
		MU_SILVER_STAR_LOSE = 0x1e,
		MU_CORRECT_SOLUTION = 0x20,
		MU_STAR_APPEAR = 0x21,
		MU_STAR_GET = 0x22,
		MU_TOAD_TALK = 0x25,
		MU_KEY_GET = 0x28,
		MU_KEY_GET_2 = 0x29,
		MU_CLASSIC_MARIO_JINGLE = 0x2a,
		MU_BOSS = 0x2d,
		MU_ENDLESS_STAIRS = 0x2e,
		MU_MEGA = 0x31,
		MU_FIRE_YOSHI = 0x32,
		MU_POWERFUL = 0x33,
		MU_LULLABY = 0x36,
		MU_BOB_OMB_BATTLEFIELD = 0x3a,
	};
	
	extern Player PLAYERS[]; //size not known, but greater than 9.
	
	unsigned PlayLong(unsigned uniqueID, unsigned soundArchiveID, unsigned soundID, const Vector3& camSpacePos, unsigned arg4); //first arg = guess
	
	void PlayCharVoice(unsigned charID, unsigned soundID, const Vector3& camSpacePos);
	
	void Play(unsigned archiveID, unsigned soundID, const Vector3& camSpacePos);
	void PlayBank0(unsigned soundID, const Vector3& camSpacePos);
	void PlayBank3(unsigned soundID, const Vector3& camSpacePos);
	void PlayBank2_2D(unsigned soundID);
	void PlayBank3_2D(unsigned soundID);
	
	//volume goes up to 0x7f
	bool PlayMsgSound(unsigned soundID, unsigned arg1, unsigned volume, Fix12i timeInv, bool starting); //return value: did it finish?
	
	void LoadAndSetMusic(unsigned musicID);
	void StopLoadedMusic();
	void SetMusic(unsigned arg0, unsigned musicID);
	void EndMusic(unsigned arg0, unsigned musicID);
	
	void ChangeMusicVolume(unsigned newVolume, Fix12i changeSpeed);
}

#endif