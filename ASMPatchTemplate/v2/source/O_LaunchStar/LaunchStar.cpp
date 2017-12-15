#include "LaunchStar.h"

namespace
{
	enum Animations
	{
		WAIT,
		LAUNCH,
		
		NUM_ANIMS
	};
	
	LaunchStar* ls_ptr = nullptr;
	
	SharedFilePtr modelFile;
	SharedFilePtr animFiles[NUM_ANIMS];
}

SpawnInfo<LaunchStar> LaunchStar::spawnData =
{
	&LaunchStar::Spawn,
	0x0168,
	0x00aa,
	0x00000003,
	0x00000000_f,
	0x00600000_f,
	0x01000000_f,
	0x01000000_f
};

bool Player::LS_Init()
{
	horzSpeed = vertAccel = 0_f;
	lsInitPos = pos;
	lsPtr = ls_ptr;
	lsPos = ls_ptr->pos; //launch star pos
	lsPos.y += 0xa0000_f; //correct y-pos of target launch star
	lsInitAng = ang;
	lsDiffAng.x = ls_ptr->ang.x - ang.x;
	lsDiffAng.y = ls_ptr->ang.y - ang.y;
	lsDiffAng.z = ls_ptr->ang.z - ang.z;
	SetAnim(0x5f, Animation::NO_LOOP, 0x00001000_f, 0);
	Sound::PlayBank3(0x61, camSpacePos);
	return true;
}

bool Player::LS_Behavior()
{
	Fix12s t = Fix12s(lsState0Timer) >> 2;
	speed.y = 0_f;
	Model& lsModel = lsPtr->rigMdl;
	Animation& lsAnim = lsPtr->rigMdl.anim;

	if(launchState == 1 || launchState == 2)
	{
		lsModel.data.UpdateBones(lsAnim.file, (int)lsAnim.currFrame);

		pos = lsModel.data.bones[1].pos.Transform(lsModel.mat4x3) << 3;
	}

	switch(launchState)
	{
		case 0:

			pos = Vector3::Interp(lsInitPos, lsPos, t);

			ang.x = (Fix12s(lsDiffAng.x, true) * t).val + lsInitAng.x;
			ang.y = (Fix12s(lsDiffAng.y, true) * t).val + lsInitAng.y;
			ang.z = (Fix12s(lsDiffAng.z, true) * t).val + lsInitAng.z;

			++lsState0Timer; //increment state timer
			if(lsState0Timer > 4)
			{
				lsPtr->rigMdl.SetAnim(animFiles[LAUNCH].filePtr, Animation::NO_LOOP, 0x00001000_f, 0x00000000);
				launchState = 1;
			}
			break;
		case 1:
			if(lsAnim.currFrame >= 0x14000_f)
			{
				lsAnim.speed = lsPtr->launchSpeed / 8;
				Sound::PlayBank3(0x14f, camSpacePos);
				Sound::PlayBank3(0x0c5, camSpacePos);
				Sound::PlayBank0(0x0b9, camSpacePos);
				launchState = 2;
			}
			break;
		case 2:
			if(lsAnim.currFrame >= 0x2f000_f)
			{
				lsAnim.currFrame = 0x2f000_f;
				lsAnim.speed = 0x00001000_f;
				pos = lsPos;

				BezierPathIter& bzIt = lsPathIt;
				bzIt.pathPtr = lsPtr->pathPtr;
				bzIt.currSplineX3 = 0;
				bzIt.tinyStep = 0x0010_fs;
				bzIt.step = lsPtr->launchSpeed;
				bzIt.currTime = 0_f;
				bzIt.pos = lsPos;

				Vector3_16f zeros {0_fs, 0_fs, 0_fs};
				lsPtr->particleID = Particle::System::New(lsPtr->particleID, 0x114, pos.x, pos.y, pos.z, &zeros, nullptr);
				launchState = 3;
			}
			break;
		case 3:
			bool finished = !lsPathIt.Advance();
			pos = lsPathIt.pos;

			ang.y = motionAng.y = prevPos.HorzAngle(pos);
			ang.x = 0x4000 - pos.VertAngle(prevPos);

			Vector3_16f zeros {0_fs, 0_fs, 0_fs};
			lsPtr->particleID = Particle::System::New(lsPtr->particleID, 0x114, pos.x, pos.y, pos.z, &zeros, nullptr);

			if(finished)
			{
				Vector3 p2;
				lsPathIt.pathPtr.GetPt(p2, lsPathIt.currSplineX3 + 2);

				short ang = pos.VertAngle(p2);
				horzSpeed = Cos(ang) * lsPathIt.step;
				speed.y   = Sin(ang) * lsPathIt.step;
				ChangeState(*(Player::State*)Player::ST_FALL);
			}
			break;
	}

	prevPos = pos;

	/*if((launchState == 1 && lsModel.anim.currFrame > 0) || launchState == 2)
	{
		lsModel.anim.currFrame -= lsModel.anim.speed;
	}*/

	return true;
}

bool Player::LS_Cleanup()
{
	switch((unsigned)nextState)
	{
		case Player::ST_SWIM:
		case Player::ST_HURT:
		case Player::ST_HURT_WATER:
			return false;
		default:
			return true;
	}
}

void LaunchStar::UpdateModelTransform()
{
	Model& model = rigMdl;
	model.mat4x3.ThisFromRotationZXYExt(ang.x, ang.y, ang.z);
	model.mat4x3.r0c3 = pos.x >> 3;
	model.mat4x3.r1c3 = pos.y >> 3;
	model.mat4x3.r2c3 = pos.z >> 3;
}

LaunchStar* LaunchStar::Spawn()
{
	return new LaunchStar;
}

int LaunchStar::InitResources()
{
	Model::LoadFile(modelFile);
	rigMdl.SetFile(modelFile.filePtr, 1, -1);
	
	BoneAnimation::LoadFile(animFiles[WAIT]);
	BoneAnimation::LoadFile(animFiles[LAUNCH]);
	rigMdl.SetAnim(animFiles[WAIT].filePtr, Animation::LOOP, 0x1000_f, 0);
	
	cylClsn.Init(this, 0x000a0000_f, 0x00140000_f, 0x00100002, 0x00008000);
	
	launchSpeed = Fix12i(ang.x);
	eventID = param1 >> 8 & 0xff;
	particleID = 0;
	
	Vector3 p0, p1;
	pathPtr.FromID(param1 & 0xff);
	pathPtr.GetPt(p0, 0);
	pathPtr.GetPt(p1, 1);
	
	ang.x = 0x4000 - p1.VertAngle(p0);
	ang.y = p0.HorzAngle(p1);
	ang.z = 0;
	
	UpdateModelTransform();
	
	pos.y -= 0x000a0000_f; //silly cylinder colliders that can't be offset
	
	return 1;
}

int LaunchStar::CleanupResources()
{
	animFiles[WAIT].Release();
	animFiles[LAUNCH].Release();
	modelFile.Release();
	return 1;
}

int LaunchStar::Behavior()
{
	if(eventID < 0x20)
	{
		if(Event::GetBit(eventID))
			eventID = 0xff;
		else
			return 1;
	}
		
	Player* isThereAPlayer = ClosestPlayer();
	
	if(isThereAPlayer)
	{
		Player& player = *isThereAPlayer;
		
		if(player.uniqueID == cylClsn.otherObjID && (INPUT_1_FRAME & Input::A) &&
		   ((unsigned)player.currState != Player::ST_LAUNCH_STAR || player.lsPtr != this))
		{
			ls_ptr = this;
			player.ChangeState(*(Player::State*)Player::ST_LAUNCH_STAR);
		}
	}
	
	if(rigMdl.anim.file == animFiles[LAUNCH].filePtr && rigMdl.anim.currFrame + 1_f >= rigMdl.anim.GetNumFrames())
		rigMdl.SetAnim(animFiles[WAIT].filePtr, Animation::LOOP, 0x1000_f, 0);
	
	rigMdl.anim.Advance();
	cylClsn.Clear();
	cylClsn.Update();
	return 1;
}

int LaunchStar::Render()
{
	if(eventID < 0x20)
		return 1;
	
	rigMdl.Render(nullptr);
	return 1;
}

LaunchStar::~LaunchStar() {}

void init()
{
	((Player::State*)Player::ST_LAUNCH_STAR)->init    = &Player::LS_Init;
	((Player::State*)Player::ST_LAUNCH_STAR)->main    = &Player::LS_Behavior;
	((Player::State*)Player::ST_LAUNCH_STAR)->cleanup = &Player::LS_Cleanup;
	
	OBJ_TO_ACTOR_ID_TABLE[0x0146] = 0x0168;
	ACTOR_SPAWN_TABLE[0x0168] = (unsigned)&LaunchStar::spawnData;
	modelFile.Construct(0x080b);
	animFiles[WAIT  ].Construct(0x080c);
	animFiles[LAUNCH].Construct(0x080d);
}

void cleanup()
{
	
}
