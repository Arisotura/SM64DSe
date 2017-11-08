#include "Berry.h"

namespace
{
	SharedFilePtr modelFile;
	SharedFilePtr stemFile;
	
	constexpr Fix12i RADIUS = 0x32000_f;
	constexpr Fix12i COIN_SPEED = 0xb000_f;
	
	const uint8_t coinPrizeTypes[] = {0, 0, 0, 0,
									  0, 0, 0, 2,
									  0, 2, 2, 2};
	
	unsigned berryCount = 0;
	unsigned berryMaxCount = 0;
};

SpawnInfo<Berry> Berry::spawnData =
{
	&Berry::Spawn,
	0x016d,
	0x00aa,
	0x00000003,
	0x00032000_f,
	0x00060000_f,
	0x01000000_f,
	0x01000000_f
};

Berry* Berry::Spawn()
{
	return new Berry;
}

void Berry::UpdateModelTransform()
{
	//model.mat4x3.ThisFromRotationY(ang.y);
	model.mat4x3.r0c3 = pos.x >> 3;
	model.mat4x3.r1c3 = pos.y >> 3;
	model.mat4x3.r2c3 = pos.z >> 3;
	
	DropShadowRadHeight(shadow, model.mat4x3, RADIUS * 2, 0x32000_f, 0xf);
}

void Berry::Kill()
{
	++berryCount;
	Actor::Spawn(0x014a, berryCount, Vector3{pos.x, pos.y + 0xa0000_f, pos.z}, nullptr, areaID, -1);
	berryCount &= 7;
	if(berryCount == 0)
	{
		++berryMaxCount;
		switch(berryMaxCount)
		{
			case 1:
			case 2:
			case 3:
				for(int i = 0; i < 4; ++i)
				{
					Actor* coin = Actor::Spawn(0x120 + coinPrizeTypes[4 * (berryMaxCount - 1) + i], 0xf2, pos, nullptr, areaID, -1);
					if(coin)
					{
						coin->motionAng = Vector3_16{0, i * 0x4000, 0};
						coin->horzSpeed = COIN_SPEED;
					}
				}
				break;
			default:
				Actor::Spawn(0x114, 0, Vector3{pos.x, pos.y + 0x5000_f, pos.z}, nullptr, areaID, -1); break;
		}
	}
	pos = origPos;
	killed = true;
}

int Berry::InitResources()
{
	//hack to make sure berries in the garden don't appear until after star 2
	if(LEVEL_ID == 8 && STAR_ID < 3 && (pos.y - 0x3fe800_f).Abs() < 0xa0000_f && pos.HorzLen() < 0x600000_f)
		return 0;
	
	Model::LoadFile(modelFile);
	model.SetFile(modelFile.filePtr, 1, -1);
	Model::LoadFile(stemFile);
	stem.SetFile(stemFile.filePtr, 1, -1);
	model.data.materials[0].difAmb = (param1 & 0x7fff) << 16 | 0x8000;
	
	cylClsn.Init(this, RADIUS, RADIUS * 2, 0x00200000, 0x00008000);
	shadow.InitCylinder();
	
	UpdateModelTransform();
	origPos = pos;
	
	return 1;
}

int Berry::CleanupResources()
{
	stemFile.Release();
	modelFile.Release();
	return 1;
}

int Berry::Behavior()
{
	if(killed)
		return 1;
	
	if(!groundFound) //not called during InitResources because not all the platforms may have initialized their resources yet
	{
		groundFound = true; 
		SphereClsn sphere;
		sphere.SetObjAndSphere(Vector3{pos.x, pos.y + RADIUS, pos.z}, RADIUS * 3 / 2, nullptr);
		
		Vector3 stemPos, stemNormal;
		if(sphere.DetectClsn())
		{
			Vector3 newSpherePos = sphere.pos + sphere.pushback;
			stemPos = newSpherePos - sphere.result.normal * sphere.radius;
			stemNormal = sphere.result.normal;
		}
		else
		{
			stemPos = pos;
			stemNormal = Vector3{0_f, 0x1000_f, 0_f};
		}
		
		Vector3 zeros {0_f, 0_f, 0_f};
		stem.mat4x3 = Matrix4x3::FromRotationZXYExt(zeros.VertAngle(stemNormal) + 0x4000, zeros.HorzAngle(stemNormal), 0) * Matrix4x3::FromRotationY(ang.y);
		stem.mat4x3.r0c3 = stemPos.x >> 3;
		stem.mat4x3.r1c3 = stemPos.y >> 3;
		stem.mat4x3.r2c3 = stemPos.z >> 3;
	}
	
	cylClsn.Clear();
	cylClsn.Update();	
	UpdateModelTransform();
	
	return 1;
}

int Berry::Render()
{
	stem.Render(nullptr);
	if(!killed && !(flags & Actor::IN_YOSHI_MOUTH))
		model.Render(nullptr);
	return 1;
}

Berry::~Berry() {}

unsigned Berry::OnYoshiTryEat()
{
	return Actor::YE_SWALLOW;
}

void Berry::OnTurnIntoEgg(Player& player)
{
	player.Heal(0x100);
	Kill();
}

void init()
{	
	OBJ_TO_ACTOR_ID_TABLE[0x014c] = 0x016d;
	ACTOR_SPAWN_TABLE[0x016d] = (unsigned)&Berry::spawnData;
	modelFile.Construct(0x0815);
	stemFile.Construct(0x0820);
}

void cleanup()
{
	
}
