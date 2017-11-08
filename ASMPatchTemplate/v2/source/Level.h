#ifndef SM64DS_LEVEL_INCLUDED
#define SM64DS_LEVEL_INCLUDED

#include "SM64DS_Common.h"

//Order for spawning: 2 3 4 b 1 6 7 8 9 c e a
//In other words:
/*
	Path Nodes
	Paths
	Views
	Minimap Tile File
	Level Entrances
	Teleport Source
	Teleport Dest
	Fog
	Doors
	Minimap Scale
	(Unknown)
	Level Exits
*/
//Not putting these in order in the file can result in drastic consequences
//such as 8-directional cameras suddenly getting angle 0x751c.
namespace LevelFile
{
	struct PathNode
	{
		Vector3_16 pos;
	};
	
	struct Path
	{
		uint16_t firstNodeID;
		uint8_t numNodes;
		uint8_t unk3;
		uint8_t unk4;
		uint8_t unk5;
	};
	
	struct View
	{
		uint8_t param1; //02: normal, 04: rotate-only, 07: pause camera
		uint8_t param2; //ff: normal
		Vector3_16 pos;
		Vector3_16 rot;
		
		static LevelFile::View& Get(unsigned viewID);
	};
	
	struct MapTile
	{
		short ov0FileID;
	};
}

extern "C"
{
	extern LevelFile::MapTile* MAP_TILE_ARR_PTR;
	extern LevelFile::View* VIEW_PTR_ARR;
}

#endif