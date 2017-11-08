#ifndef MATHS
#define MATHS

/* Calculate the smallest angle which must be added to angle1 to get angle2.
 * Examples: 
 * angle1 = 90, angle2 = 90: return 0
 * angle1 = 170, angle2 = 90: return -80
 * angle1 = 90, angle2 = 170: return 80
 * angle1 = -170, angle2 = -90: return 80
 * angle1 = -90, angle2 = -170: return -80
 * angle1 = 45, angle2 = -45: return -90
 * angle1 = -45, angle2 = 45: return 90
 * angle1 = 170, angle2 = -170: return -20
 * angle1 = -170, angle2 = 170: return 20
 */ 	
short int CalculateSmallestObjectAngleDifference(short int angle1, short int angle2);

int max32(int a, int b);
int min32(int a, int b);
int abs32(int a);

int max16(short int a, short int b);
int min16(short int a, short int b);
int abs16(short int a);

#endif