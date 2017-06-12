/*
    Copyright 2012 Kuribo64

    This file is part of SM64DSe.

    SM64DSe is free software: you can redistribute it and/or modify it under
    the terms of the GNU General Public License as published by the Free
    Software Foundation, either version 3 of the License, or (at your option)
    any later version.

    SM64DSe is distributed in the hope that it will be useful, but WITHOUT ANY 
    WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS 
    FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along 
    with SM64DSe. If not, see http://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace SM64DSe
{
    public partial class MainForm
    {
        public void DumpObjectInfo()
        {
            uint actorptrtable = 0x8000 + 0x88864;

            Program.m_ROM.BeginRW();
          
            Stream fs = File.Create("objinfo.php");
            StreamWriter wr = new StreamWriter(fs);
           
            wr.Write("<?php\n$objinfo = array\n(\n");
            
            for (int i = 0; i < LevelObject.NUM_OBJ_TYPES; i++)
            {
                wr.Write("\tarray\n\t(\n");

                wr.Write(String.Format("\t\t'id' => 0x{0:X4},\n", i));
                wr.Write("\t\t'internalname' => '" + ObjectDatabase.m_ObjectInfo[i].m_InternalName + "',\n");

                uint actorid = ObjectDatabase.m_ObjectInfo[i].m_ActorID;
                uint actoraddr = Program.m_ROM.Read32(actorptrtable + (uint)(actorid * 4));

                wr.Write(String.Format("\t\t'actorid' => 0x{0:X4},\n", actorid));

                if (actoraddr < 0x021111A0)
                    wr.Write("\t\t'requirement' => 'none',\n");
                else
                {
                    uint minovl = 0, maxovl = 0, bank = 0, bankbase = 0;
                    if ((actoraddr >= 0x021111A0) && (actoraddr < 0x02115EE0))
                    { minovl = 8; maxovl = 59; bank = 7; bankbase = 1;  }
                    else if ((actoraddr >= 0x02115EE0) && (actoraddr < 0x0211F000))
                    { minovl = 0x3E; maxovl = 0x42; bank = 0; bankbase = 1;  }
                    else if ((actoraddr >= 0x0211F000) && (actoraddr < 0x02123740))
                    { minovl = 0x46; maxovl = 0x4A; bank = 1; bankbase = 2; }
                    else if ((actoraddr >= 0x02123740) && (actoraddr < 0x02129020))
                    { minovl = 0x4D; maxovl = 0x51; bank = 2; bankbase = 1; }
                    else if ((actoraddr >= 0x02129020) && (actoraddr < 0x02130F00))
                    { minovl = 0x54; maxovl = 0x55; bank = 3; bankbase = 1; }
                    else if ((actoraddr >= 0x02130F00) && (actoraddr < 0x02135700))
                    { minovl = 0x59; maxovl = 0x5C; bank = 4; bankbase = 1; }
                    else if ((actoraddr >= 0x02135700) && (actoraddr < 0x02140D80))
                    { minovl = 0x5E; maxovl = 0x61; bank = 5; bankbase = 1; }
                    else if ((actoraddr >= 0x02140D80) && (actoraddr < 0x0214EAA0))
                    { minovl = 0x64; maxovl = 0x64; bank = 6; bankbase = 1; }

                    uint curovl = minovl;
                    do
                    {
                        NitroOverlay theovl = new NitroOverlay(Program.m_ROM, curovl);

                        uint testval = 0xFFFFFFFF;
                        try { testval = (uint)theovl.Read16(actoraddr - theovl.GetRAMAddr() + 4); }
                        catch {}

                        if (testval == actorid)
                        {
                            wr.Write(String.Format("\t\t'requirement' => 'bank{0}={1}',\n", bank, curovl - minovl + bankbase));

                            break;
                        }

                        curovl++;
                    }
                    while (curovl <= maxovl);
                }

                wr.Write("\t\t0\n\t), \n");
                if ((i % 64) == 0) wr.Flush();
            }

            wr.Write("\t0\n);\n?>\n");
            wr.Flush();

            fs.Close();
            Program.m_ROM.EndRW();
        }
    }
}
