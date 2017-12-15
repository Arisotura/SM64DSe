#ifndef FALLINGROCK_INCLUDED
#define FALLINGROCK_INCLUDED

#include "../SM64DS_2.h"

struct YBG_FallingRock : public Platform
{	
	struct Spawner : public Actor
	{	
		unsigned particleSysID;
		uint8_t eventID;
		
		void UpdateModelTransform();

		static Spawner* Spawn();
		virtual int InitResources() override;
		virtual int CleanupResources() override;
		virtual int Behavior() override;
		virtual int Render() override;
		virtual ~Spawner();

		static SpawnInfo<Spawner> spawnData;
	};
	
	WithMeshClsn wmClsn;
	ShadowVolume shadow;
	Matrix4x3 shadowMat;
	
	void UpdateModelTransform();

	static YBG_FallingRock* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual ~YBG_FallingRock();
	virtual void Kill();

	static SharedFilePtr modelFile;
	static SharedFilePtr clsnFile;
	
	static SpawnInfo<YBG_FallingRock> spawnData;
};

#endif
