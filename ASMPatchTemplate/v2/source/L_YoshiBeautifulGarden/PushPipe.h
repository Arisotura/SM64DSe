#ifndef YBG_PUSHPIPE_INCLUDED
#define YBG_PUSHPIPE_INCLUDED

#include "../SM64DS_2.h"

struct YBG_PushPipe : public Platform
{	
	Matrix4x3 rotMat;
	Vector3 origPos;
	Fix12i distFromStart;
	Fix12i maxDistFromStart;
	int pushSide; //1 or -1 if pushed, 0 if not pushed
	unsigned soundID;
	bool fixed;
	uint8_t eventID;
	
	void SetInitialPos();
	void UpdateModelTransform();
	void UpdateClsnTransform();

	static YBG_PushPipe* Spawn();
	virtual int InitResources() override;
	virtual int CleanupResources() override;
	virtual int Behavior() override;
	virtual int Render() override;
	virtual ~YBG_PushPipe();
	virtual void OnPushed(Actor& pusher);

	static SharedFilePtr modelFile;
	static SharedFilePtr clsnFile;
	
	static SpawnInfo<YBG_PushPipe> spawnData;
};

#endif