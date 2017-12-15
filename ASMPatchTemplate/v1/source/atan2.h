// Taken from http://www.coranac.com/documents/arctangent/
// By Jasper Vijn "cearn"

#ifndef ATAN2
#define ATAN2

static const unsigned int BRAD_PI_SHIFT=14,   BRAD_PI = 1<<BRAD_PI_SHIFT;
static const unsigned int BRAD_HPI= BRAD_PI/2, BRAD_2PI= BRAD_PI*2;

static const unsigned int ATAN_ONE = 0x1000, ATAN_FP= 12;

// Get the octant a coordinate pair is in.
#define OCTANTIFY(_x, _y, _o)   do {                            \
    int _t; _o= 0;                                              \
    if(_y<  0)  {            _x= -_x;   _y= -_y; _o += 4; }     \
    if(_x<= 0)  { _t= _x;    _x=  _y;   _y= -_t; _o += 2; }     \
    if(_x<=_y)  { _t= _y-_x; _x= _x+_y; _y=  _t; _o += 1; }     \
} while(0);

// atan via CORDIC (coordinate rotations).
// Returns [0,2pi), where pi ~ 0x4000.
unsigned int atan2Cordic(int x, int y);

#endif