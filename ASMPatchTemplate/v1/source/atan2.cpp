#include "atan2.h"
#include "SM64DS.h"

unsigned int atan2Cordic(int x, int y)
{
    if(y==0)    return (x>=0 ? 0 : BRAD_PI);

    int phi;

    OCTANTIFY(x, y, phi);
    phi *= BRAD_PI/4;

    // Scale up a bit for greater accuracy.
    if(x < 0x10000)
    {
        x *= 0x1000;
        y *= 0x1000;
    }

    // atan(2^-i) terms using PI=0x10000 for accuracy
    const u16 list[]=
    {
        0x4000, 0x25C8, 0x13F6, 0x0A22, 0x0516, 0x028C, 0x0146, 0x00A3,
        0x0051, 0x0029, 0x0014, 0x000A, 0x0005, 0x0003, 0x0001, 0x0001
    };

    int i, tmp, dphi=0;
    for(i=1; i<12; i++)
    {
        if(y>=0)
        {
            tmp= x + (y>>i);
            y  = y - (x>>i);
            x  = tmp;
            dphi += list[i];
        }
        else
        {
            tmp= x - (y>>i);
            y  = y + (x>>i);
            x  = tmp;
            dphi -= list[i];
        }
    }
    return phi + (dphi>>2);
}
