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
    public class NitroOverlay : INitroROMBlock
    {
        public NitroOverlay(NitroROM rom, uint id)
        {
            m_ROM = rom;
            m_ID = id;

            bool autorw = !m_ROM.CanRW();
            if (autorw) m_ROM.BeginRW();

            m_OVTEntryAddr = m_ROM.GetOverlayEntryOffset(m_ID);
            m_FileID = m_ROM.GetFileIDFromOverlayID(m_ID);

            m_RAMAddr = m_ROM.Read32(m_OVTEntryAddr + 0x04);
            Byte flags = m_ROM.Read8(m_OVTEntryAddr + 0x1F);

            m_Data = m_ROM.ExtractFile(m_FileID);
            if ((flags & 0x01) == 0x01)
                Jap77.Decompress(ref m_Data);

            if (autorw) m_ROM.EndRW();
        }

        public uint GetRAMAddr() { return m_RAMAddr; }

        public uint ReadPointer(uint addr)
        {
            uint ptr = Read32(addr);
            if (ptr < m_RAMAddr) return 0xFFFFFFFF;
            return ptr - m_RAMAddr;
        }

        public void WritePointer(uint addr, uint ptr)
        {
            if (ptr == 0xFFFFFFFF) ptr = 0;
            else ptr += m_RAMAddr;
            Write32(addr, ptr);
        }

        public override void SaveChanges()
        {
            bool autorw = !m_ROM.CanRW();
            if (autorw) m_ROM.BeginRW();

            // first, ensure that the size is aligned to 4 byte boundary
            if (m_Data.Length % 4 != 0)
            {
                SetSize((uint)((m_Data.Length + 3) & ~3));
            }

            // reinsert file data
            m_ROM.ReinsertFile(m_FileID, m_Data);
            Update();

            // fix overlay size
            m_ROM.Write32(m_OVTEntryAddr + 0x08, (uint)((m_Data.Length + 3) & ~3));

            // tweak the overlay table entry
            byte flags = m_ROM.Read8(m_OVTEntryAddr + 0x1F);
            flags &= 0xFE; // [Treeki] disable compression :)
            m_ROM.Write8(m_OVTEntryAddr + 0x1F, flags);

            if (autorw) m_ROM.EndRW();
        }

        public uint GetSize()
        {
            return (uint)m_Data.Length;
        }

        public void SetSize(uint newsize)
        {
            Array.Resize(ref m_Data, (int)newsize);
        }

        public void SetInitializer(uint address, uint size)
        {
            bool autorw = !m_ROM.CanRW();
            if (autorw) m_ROM.BeginRW();

            m_ROM.Write32(m_OVTEntryAddr + 0x10, address);
            m_ROM.Write32(m_OVTEntryAddr + 0x14, address + size);

            if (autorw) m_ROM.EndRW();
        }

        public void Update() { m_OVTEntryAddr = m_ROM.GetOverlayEntryOffset(m_ID); }


        private uint m_ID;
        private ushort m_FileID;
        private uint m_OVTEntryAddr;
        private uint m_RAMAddr;
    }
}
