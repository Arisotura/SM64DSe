#include "SM64DS.h"

const int HOVER_VERTICAL_SPEED = 0x00003B23;
const int VERTICAL_SPEED_ZERO = 0xFFFF8000;
const unsigned short B_AND_L_KEYS_PRESSED = 0x10D;
const unsigned short HOVER_LIMIT = 90;
const unsigned int ACT_LOOP = 0;
const unsigned int ACT_RUN_ONCE = 0x40000000;

unsigned short timer = 0;
unsigned short currentAction = 0xFF;
bool isHovering = false;
bool actionCalled = false;
bool inJumpMethod = false;

// The following address is called every frame. 
void hook_020e50ac()
{
	// If the current character is Mario,
	if (CHARACTER == CHAR_Mario)
	{
		// Check the current key-press state
		
		unsigned short keys = KEYS;
		unsigned short pressedB = keys & 0x00F;
		unsigned short pressedLAndR = keys & 0xF00;
		unsigned short checkPressedBAndL = (pressedB | pressedLAndR);
		
		// If both 'L' and 'B' are being pressed and player is allowed to jump, falling or hovering
		if ( checkPressedBAndL == B_AND_L_KEYS_PRESSED &&
			(inJumpMethod || currentAction == ACT_Fall || 
			currentAction == ACT_HeavyFall || currentAction == ACT_ReachedPeakOfFirstOrSecondJump) )
		{
			// and the timer hasn't run out
			if (timer < HOVER_LIMIT)
			{
				// Set the player's Y speed to have them hover with slight upward movement and increment the timer
				PLAYER->ySpeed = HOVER_VERTICAL_SPEED;
				isHovering = true;
				timer = timer + 1;
				
				// If the "reached top of jump" action hasn't been called, or another action has been called 
				// while we're still hovering, call the Player_PerformAction method with the "reached peak 
				// of jump" action (0x4F) at 1x speed
				if (!actionCalled || currentAction != ACT_ReachedPeakOfFirstOrSecondJump)
				{
					Player_PerformAction(PLAYER, ACT_ReachedPeakOfFirstOrSecondJump, ACT_RUN_ONCE, 0x1000);
					actionCalled = true;
					currentAction = ACT_ReachedPeakOfFirstOrSecondJump;
				}
			}
			else
			{
				isHovering = false;
				actionCalled = false;
			}
		}
		
		if (PLAYER->ySpeed == VERTICAL_SPEED_ZERO)
		{
			timer = 0;
			isHovering = false;
			actionCalled = false;
		}
	}
}

// Following hook is placed just after start of 'OnJump' method, 0x020e22c0
void hook_020e22c8()
{
	inJumpMethod = true;
}

// Following hook is placed just after start of 'OnHitLand' method, 0x020E088C
void hook_020e0894()
{
	inJumpMethod = false;
}

// Following hook is placed just after start of 'OnReachedLandAfterLeavingWater' method, 0x020d4270
void hook_020d4278()
{
	inJumpMethod = false;
}

// This hook is placed just after the start of the Player_PerformAction 0x020BEF2C method and stores the current 
// animation/action into the variable 'currentAction'.
// The "_ov_02" tells NSMBe that the code to be loaded at the specified address is found in Overlay 002
void repl_020BEF38_ov_02()
{
	asm
	(
		"ldr r5, =currentAction		\n\t"
		"str r1, [r5]				\n\t"
		"original:					\n\t"
		"mov r7, r0					\n\t"
	);
}