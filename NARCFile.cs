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
using System.Text;

namespace SM64DSe
{
    public class NARCFile : NitroFile
    {
        public NARCFile(NARC arc, ushort id) : base()
        {
            if (id >= 0xF000)
                throw new Exception("NARCFile: invalid file ID");

            m_Narc = arc;
            m_ID = id;
            m_Name = m_Narc.GetFileNameFromID(id);
            m_Data = m_Narc.ExtractFile(m_ID);

            if (Read32(0x0) == 0x37375A4C)
                LZ77.Decompress(ref m_Data, true);
        }

        public override void SaveChanges()
        {
            // TODO: LZ77 recompression!

            m_Narc.ReinsertFile(m_ID, m_Data);
        }


        public NARC m_Narc;
    }
}
