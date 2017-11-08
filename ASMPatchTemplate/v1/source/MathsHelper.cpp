#include "MathsHelper.h"

short int CalculateSmallestObjectAngleDifference(short int angle1, short int angle2) 
{
	if (angle1 == angle2) 
	{
		return 0;
	}
	else if ((angle1 > 0 && angle2 > 0) || (angle1 < 0 && angle2 < 0)) 
	{
		return (angle2 - angle1);
	}
	else if (angle1 > 0 && angle2 < 0) 
	{
		short int throughMax = (0 - ((angle2 - 0x8000) + (0x8000 - angle1)));
		short int throughZero = (0 - (angle1 + (0 - angle2)));
		return (abs16(throughMax) < abs16(throughZero)) ? throughMax : throughZero;
	}
	else if (angle1 < 0 && angle2 > 0)
	{
		short int throughMax = ((angle1 - 0x8000) + (0x8000 - angle2));
		short int throughZero = ((0 - angle1) + angle2);
		return (abs16(throughMax) < abs16(throughZero)) ? throughMax : throughZero;
	}
	else if (angle1 == 0) 
	{
		return angle2;
	}
	else if (angle2 == 0) 
	{
		return (0 - angle1);
	}
	else 
	{
		return 0;
	}
}

int max32(int a, int b) 
{
	return (a > b) ? a : b;
}

int min32(int a, int b) 
{
	return (a < b) ? a : b;
}

int abs32(int a) 
{
	return (a >= 0) ? (a) : (0 - a);
}

int max16(short int a, short int b) 
{
	return (a > b) ? a : b;
}

int min16(short int a, short int b) 
{
	return (a < b) ? a : b;
}

int abs16(short int a) 
{
	return (a >= 0) ? (a) : (0 - a);
}
