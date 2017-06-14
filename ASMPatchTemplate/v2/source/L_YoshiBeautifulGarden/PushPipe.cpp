#include "PushPipe.h"
#include "YBGSpecifics.h"

namespace
{
	FixedSizeCLPS_Block<1> clpsBlock =
	{
		{'C', 'L', 'P', 'S'},
		8,
		1,
		{
			CLPS(0x00, 0, 0x3f, 0x0, 0x0, 0x00, 0, 0, 0, 0xff)
		}
	};
	
	constexpr Fix12i RADIUS = 0x8c000_f;
	constexpr Fix12i TAU_RADIUS = 0x6488_f * RADIUS;
	constexpr Fix12i LENGTH = 0x320000_f;
	constexpr Fix12i CLSN_TOL = 0x07d00000_f;
	constexpr Fix12i PUSH_SPEED = 0x4000_f;
}

SharedFilePtr YBG_PushPipe::modelFile;
SharedFilePtr YBG_PushPipe::clsnFile;

SpawnInfo<YBG_PushPipe> YBG_PushPipe::spawnData =
{
	&YBG_PushPipe::Spawn,
	0x0027,
	0x00cc,
	0x00000002,
	0x0008c000_f,
	0x00320000_f,
	0x01000000_f,
	0x01000000_f,
};

YBG_PushPipe* YBG_PushPipe::Spawn()
{
	return new YBG_PushPipe;
}

void YBG_PushPipe::SetInitialPos()
{
	rotMat = Matrix4x3::FromRotationY(ang.y);
	origPos = pos;
	
	if(fixed)
		return;
	
	distFromStart = Fix12i(param1);
	pos += Vector3{0_f, 0_f, distFromStart}.Transform(rotMat);
	
	rotMat.r0c3 = origPos.x;
	rotMat.r1c3 = origPos.y;
	rotMat.r2c3 = origPos.z;
	
	RaycastLine raycaster;
	raycaster.SetObjAndLine(Vector3{-LENGTH / 2, RADIUS, 0_f}.Transform(rotMat),
							Vector3{-LENGTH / 2, RADIUS, CLSN_TOL}.Transform(rotMat),
							this);
	Fix12i t0 = raycaster.DetectClsn() ? raycaster.GetClsnPos().HorzDist(raycaster.pos0) : 0x07d00000_f;
	raycaster.SetObjAndLine(Vector3{ LENGTH / 2, RADIUS, 0_f}.Transform(rotMat),
							Vector3{ LENGTH / 2, RADIUS, CLSN_TOL}.Transform(rotMat),
							this);
	Fix12i t1 = raycaster.DetectClsn() ? raycaster.GetClsnPos().HorzDist(raycaster.pos0) : 0x07d00000_f;
	maxDistFromStart = std::min(t0, t1) - RADIUS;
	
	rotMat.r0c3 = 0_f;
	rotMat.r1c3 = 0_f;
	rotMat.r2c3 = 0_f;
}

void YBG_PushPipe::UpdateModelTransform()
{
	model.mat4x3.ThisFromRotationZXYExt((distFromStart / (TAU_RADIUS / 0x10)).val, ang.y, 0);
	model.mat4x3.r0c3 = pos.x >> 3;
	model.mat4x3.r1c3 = (pos.y + RADIUS) >> 3;
	model.mat4x3.r2c3 = pos.z >> 3;
}

void YBG_PushPipe::UpdateClsnTransform()
{
	clsnNextMat.ThisFromRotationY(ang.y);
	clsnNextMat.r0c3 = pos.x;
	clsnNextMat.r1c3 = pos.y;
	clsnNextMat.r2c3 = pos.z;
	
	clsn.Transform(clsnNextMat, ang.y);
}

int YBG_PushPipe::InitResources()
{
	char* modelF = Model::LoadFile(modelFile);
	model.SetFile(modelF, 1, -1);
	
	eventID = ang.x;
	ang.x = 0;
	fixed = STAR_ID != 1;
	
	SetInitialPos();
	UpdateModelTransform();
	UpdateClsnTransform();
	
	char* clsnF = MovingMeshCollider::LoadFile(clsnFile);
	clsn.SetFile(clsnF, clsnNextMat, 0x1000_f, ang.y, clpsBlock);
	clsn.beforeClsnCallback = (decltype(clsn.beforeClsnCallback))0x02039348;
	
	return 1;
}

int YBG_PushPipe::CleanupResources()
{
	if(clsn.IsEnabled())
	{
		clsn.Disable();
	}
	modelFile.Release();
	clsnFile.Release();
	return 1;
}

int YBG_PushPipe::Behavior()
{
	if(!fixed)
	{
		distFromStart += pushSide * PUSH_SPEED;
		if(pushSide != 0)
			soundID = Sound::PlayLong(soundID, 0x3, 0x97, camSpacePos, 0);
		
		if(distFromStart < 0_f)
		{
			distFromStart = 0_f;
			if(!fixed)
				Actor::Spawn(0x0167, 0x0000, pos, nullptr, areaID, -1);
			fixed = true;
			if(eventID < 0x20)
				Event::SetBit(eventID);
		}
		else if(distFromStart > maxDistFromStart)
			distFromStart = maxDistFromStart;
		
		pos = Vector3{0_f, 0_f, distFromStart}.Transform(rotMat) + origPos;
	}
	
	UpdateModelTransform();
	if(IsClsnInRange(0_f, 0_f))
	{
		UpdateClsnTransform();
	}
	
	pushSide = 0;
	return 1;
}

int YBG_PushPipe::Render()
{
	model.Render(nullptr);
	return 1;
}

YBG_PushPipe::~YBG_PushPipe() {}

void YBG_PushPipe::OnPushed(Actor& pusher)
{
	short angleDiff = AngleDiff(ang.y, pusher.ang.y);
	if(fixed || (angleDiff >= 0x2000 && angleDiff <= 0x6000))
		return; //no perpendicular pushing!
	
	Fix12i dot = (pusher.pos - pos).Dot(Vector3{0_f, 0_f, 0x1000_f}.Transform(rotMat));
	pushSide = dot >= 0_f ? -1 : 1;
}