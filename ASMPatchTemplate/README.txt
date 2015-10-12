SM64DS ASM Patch project template
==========================

This is based on the NSMB ASM Patch project template by Dirbaio:

http://nsmbhd.net/

See for more details:
http://nsmbhd.net/thread/1025-asm-hacking-project-template/
http://nsmbhd.net/thread/1281-how-asm-hacks-are-setup/


SM64DS version by Fiachra

http://kuribo64.net/

===========================

Stuff works similar to NDS homebrew projects.
Code goes in source/
Data goes in data/

Place a SM64DS EUR ROM in this folder (Other regions will NOT work)
Open the ROM in SM64DSe > More > Toggle Suitability for NSMBe ASM Patching
Use a recent version of NSMB Editor to compile and insert the code into the ROM.

Included features:

ChangeLevelSettingsExample.cpp - Modify level settings during level initialisation. 
PositionAndRotationUpdateExample.cpp - Example on updating objects' position and rotation. 
HoverWithLimit.cpp - Allows Mario to hover for a set time limit by pressing both B and L.
atan2.cpp - Efficient atan2 implementation suitable for use in NDS games.
Helper.cpp - Currently saves the Player object's address for use in the project, more will be added in future.
SM64DS.h - Defines several useful functions from the game. Not complete at all, I've just added stuff as I needed it.

Enjoy!
