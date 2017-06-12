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
    public class NitroFile : INitroROMBlock
    {
        public NitroFile() { }

        public NitroFile(NitroROM rom, ushort id)
        {
            if (id >= 0xF000)
                throw new Exception("NitroFile: invalid file ID");

            m_ROM = rom;
            m_ID = id;
            m_Name = m_ROM.GetFileNameFromID(id);
            m_Data = m_ROM.ExtractFile(m_ID);

            if (m_Data.Length >= 4 && Read32(0x0) == 0x37375A4C)
                LZ77.Decompress(ref m_Data, true);
        }

        public void ForceDecompression()
        {
            LZ77.Decompress(ref m_Data, false);
        }

        public void Decompress()
        {
            LZ77.Decompress(ref m_Data, true);
        }

        public void Compress()
        {
            LZ77.LZ77_Compress(ref m_Data, true);
        }

        public void ForceCompression()
        {
            LZ77.LZ77_Compress(ref m_Data, false);
        }

        public override void SaveChanges()
        {
            // TODO: LZ77 recompression!

            m_ROM.ReinsertFile(m_ID, m_Data);
        }


        public ushort m_ID;
        public string m_Name;
    }
}
