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
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;

namespace SM64DSe
{
    // NitroROM class
    // made especially for SM64DS, but most of it can be used with any NDS ROM

    public partial class NitroROM
    {
        const int ROM_END_MARGIN = 0x88;

        public NitroROM(string path)
        {
            m_Path = path;
            m_CanRW = false;

            BeginRW();

            m_FileStream.Position = 0x00;
            char[] gametitle = m_BinReader.ReadChars(12);
            if (new string(gametitle) != "S.MARIO64DS\0")
            {
                EndRW();
                throw new Exception("This file isn't a Super Mario 64 DS ROM.");
            }

	        m_FileStream.Position = 0x0C;
	        uint gamecode = m_BinReader.ReadUInt32();
	        m_FileStream.Position = 0x1E;
	        byte romversion = m_BinReader.ReadByte();

	        switch (gamecode)
	        {
	        case 0x454D5341: // ASME / USA
		        if (romversion == 0x01)
		        {
			        m_Version = Version.USA_v2;

			        m_LevelOvlIDTableOffset = 0x742B4;
			        m_FileTableOffset = 0x11244;
			        m_FileTableLength = 1824;
		        }
		        else
		        {
			        m_Version = Version.USA_v1;

			        m_LevelOvlIDTableOffset = 0x73594;
			        m_FileTableOffset = 0x1123C;
			        m_FileTableLength = 1824;
		        }
		        break;

            case 0x4A4D5341: // ASMJ / JAP
                m_Version = Version.JAP;

                m_LevelOvlIDTableOffset = 0x73B38;
                m_FileTableOffset = 0x1123C;
                m_FileTableLength = 1824;
                break;

	        case 0x504D5341: // ASMP / EUR
		        m_Version = Version.EUR;

		        m_LevelOvlIDTableOffset = 0x758C8;
		        m_FileTableOffset = 0x13098;
		        m_FileTableLength = 2058;
		        break;

	        default:
		        m_Version = Version.UNK;
                EndRW();
		        throw new Exception("Unknown ROM version. Tell Mega-Mario about it.");
	        }

            m_FileStream.Position = 0x28;
            ARM9RAMAddress = m_BinReader.ReadUInt32();

            m_FileStream.Position = 0x30;
            ARM7Offset = m_BinReader.ReadUInt32();
            m_FileStream.Position += 0x04;
            ARM7RAMAddress = m_BinReader.ReadUInt32();
            ARM7Size = m_BinReader.ReadUInt32();

	        m_FileStream.Position = 0x40;
	        FNTOffset = m_BinReader.ReadUInt32();
	        FNTSize = m_BinReader.ReadUInt32();
	        FATOffset = m_BinReader.ReadUInt32();
	        FATSize = m_BinReader.ReadUInt32();
	        OVTOffset = m_BinReader.ReadUInt32();
	        OVTSize = m_BinReader.ReadUInt32();
	        // no need to bother about ARM7 overlays... there's none in SM64DS

	        m_FileStream.Position = 0x80;
	        m_UsedSize = m_BinReader.ReadUInt32();
            //m_UsedSize += ROM_END_MARGIN;

	        m_FileStream.Position = FNTOffset + 6;
	        ushort numdirs = m_BinReader.ReadUInt16();
	        ushort numfiles = (ushort)(FATSize / 8);

	        m_DirEntries = new DirEntry[numdirs];
	        m_FileEntries = new FileEntry[numfiles];

	        m_FileStream.Position = FATOffset;
	        for (ushort f = 0; f < numfiles; f++)
	        {
		        uint start = m_BinReader.ReadUInt32();
		        uint end = m_BinReader.ReadUInt32();

                FileEntry fe;
                fe.ID = f;
                fe.InternalID = 0xFFFF;
                fe.ParentID = 0;
                fe.Offset = start;
                fe.Size = end - start;
                fe.Name = fe.FullName = "";
		        m_FileEntries[f] = fe;
	        }

            DirEntry root;
            root.ID = 0xF000;
            root.ParentID = 0;
            root.Name = root.FullName = "";
	        m_DirEntries[0] = root;

	        uint tableoffset = FNTOffset;
	        for (ushort d = 0; d < numdirs; d++)
	        {
		        m_FileStream.Position = tableoffset;
		        uint subtableoffset = FNTOffset + m_BinReader.ReadUInt32();
		        ushort first_fileid = m_BinReader.ReadUInt16();
		        ushort cur_fileid = first_fileid;

		        m_FileStream.Position = subtableoffset;
		        for (;;)
		        {
			        byte type_len = m_BinReader.ReadByte();

			        if (type_len == 0x00) break;
			        else if (type_len > 0x80)
			        {
                        DirEntry dir;

                        dir.Name = new string(m_BinReader.ReadChars(type_len & 0x7F));
                        dir.ID = m_BinReader.ReadUInt16();
                        dir.ParentID = (ushort)(d + 0xF000);
                        dir.FullName = "";
                        
				        m_DirEntries[dir.ID - 0xF000] = dir;
			        }
			        else if (type_len < 0x80)
			        {
				        char[] _name = m_BinReader.ReadChars(type_len & 0x7F);

				        m_FileEntries[cur_fileid].ParentID = (ushort)(d + 0xF000);
				        m_FileEntries[cur_fileid].Name = new string(_name);
				        cur_fileid++;
			        }
		        }

		        tableoffset += 8;
	        }

	        for (int i = 0; i < m_DirEntries.Length; i++)
	        {
		        if (m_DirEntries[i].ParentID > 0xF000)
			        m_DirEntries[i].FullName = m_DirEntries[m_DirEntries[i].ParentID-0xF000].FullName + "/" + m_DirEntries[i].Name;
		        else
			        m_DirEntries[i].FullName = m_DirEntries[i].Name;
	        }

	        for (int i = 0; i < m_FileEntries.Length; i++)
	        {
		        if (m_FileEntries[i].ParentID > 0xF000)
			        m_FileEntries[i].FullName = m_DirEntries[m_FileEntries[i].ParentID-0xF000].FullName + "/" + m_FileEntries[i].Name;
		        else
			        m_FileEntries[i].FullName = m_FileEntries[i].Name;
	        }

	        uint numoverlays = OVTSize / 0x20;
	        m_OverlayEntries = new OverlayEntry[numoverlays];

	        for (uint i = 0; i < numoverlays; i++)
	        {
		        m_FileStream.Position = OVTOffset + (i*0x20);
                OverlayEntry oe;

                oe.EntryOffset = (uint)m_FileStream.Position;
		        oe.ID = m_BinReader.ReadUInt32();
		        oe.RAMAddress = m_BinReader.ReadUInt32();
		        oe.RAMSize = m_BinReader.ReadUInt32();
		        oe.BSSSize = m_BinReader.ReadUInt32();
		        m_FileStream.Position += 8;
		        oe.FileID = m_BinReader.ReadUInt16();

		        m_OverlayEntries[oe.ID] = oe;
	        }
        	
	        EndRW();
        }

        public void LoadTables()
        {
            m_FileTable = new ushort[m_FileTableLength];

	        NitroOverlay ovl0 = new NitroOverlay(this, 0);
	        for (uint i = 0; i < m_FileTableLength; i++)
	        {
		        uint str_offset = ovl0.ReadPointer(m_FileTableOffset + (i*4));
		        string fname = ovl0.ReadString(str_offset, 0);
		        m_FileTable[i] = GetFileIDFromName(fname);
                m_FileEntries[GetFileIDFromName(fname)].InternalID = (ushort)i;
	        }

            m_FileStream.Position = m_LevelOvlIDTableOffset;
            m_LevelOvlIDTable = new uint[52];
            for (uint i = 0; i < 52; i++)
                m_LevelOvlIDTable[i] = m_BinReader.ReadUInt32();
        }

        public void BeginRW(bool buffered)
        {
            if (m_CanRW) return;

            m_Buffered = buffered;
            if (m_Buffered)
	        {
                byte[] buf = File.ReadAllBytes(m_Path);
                // indirect way of filling the memorystream
                // initializing it directly would create a non-resizable stream
                m_FileStream = new MemoryStream(buf.Length);
                m_FileStream.Write(buf, 0, buf.Length);
	        }
	        else
                m_FileStream = File.Open(m_Path, FileMode.Open, FileAccess.ReadWrite);

            m_BinReader = new BinaryReader(m_FileStream, Encoding.ASCII);
            m_BinWriter = new BinaryWriter(m_FileStream, Encoding.ASCII);
            m_CanRW = true;
        }
        public void BeginRW() { BeginRW(false); }

        public void EndRW(bool keep)
        {
	        if (!m_CanRW) return;

	        if (m_Buffered)
	        {
                if (keep)
                    File.WriteAllBytes(m_Path, ((MemoryStream)m_FileStream).GetBuffer());
		        m_Buffered = false;
	        }

            m_CanRW = false;
            m_FileStream.Close();
        }
        public void EndRW() { EndRW(true); }

        public bool CanRW() { return m_CanRW; }

        public ushort GetFileIDFromName(string name)
        {
            foreach (FileEntry fe in m_FileEntries)
            {
                if (fe.FullName == name)
                    return fe.ID;
            }

            return 0xFFFF;
        }
        public ushort GetFileIDFromOverlayID(uint ovlid) { return m_OverlayEntries[ovlid].FileID; }
        public ushort GetFileIDFromInternalID(ushort intid) { return m_FileTable[intid]; }

        public ushort GetDirIDFromName(string name)
        {
            foreach (DirEntry de in m_DirEntries)
            {
                if (de.FullName == name)
                    return de.ID;
            }

            return 0x0000;
        }

        public bool FileExists(string name)
        {
            ushort id = GetFileIDFromName(name);
            if (id < 0xF000)
                return true;

            string[] narcNames = (m_Version == Version.EUR ? new String[] { "ar1", "arc0", "c2d", "cee", "cef", "ceg", "cei", "ces", "en1", "vs1", "vs2", "vs3", "vs4" }
                : new String[] { "ar1", "arc0", "c2d", "en1", "vs1", "vs2", "vs3", "vs4" });
            foreach (string narcName in narcNames)
            {
                NARC narc = new NARC(this, GetFileIDFromName("ARCHIVE/" + narcName + ".narc"));
                id = narc.GetFileIDFromName(name);
                if (id < 0xF000)
                    return true;
            }

            return false;
        }

        public NitroFile GetFileFromName(string name)
        {
            ushort id = GetFileIDFromName(name);
            if (id < 0xF000)
                return new NitroFile(this, id);

            string[] narcs = (m_Version == Version.EUR ? new String[]{ "ar1", "arc0", "c2d", "cee", "cef", "ceg", "cei", "ces", "en1", "vs1", "vs2", "vs3", "vs4" }
                : new String[] { "ar1", "arc0", "c2d", "en1", "vs1", "vs2", "vs3", "vs4" });
            foreach (string narc in narcs)
            {
                NARC thenarc = new NARC(this, GetFileIDFromName("ARCHIVE/" + narc + ".narc"));
                id = thenarc.GetFileIDFromName(name);
                if (id < 0xF000)
                    return new NARCFile(thenarc, id);
            }

            throw new Exception("NitroROM: cannot find file '" + name + "'");
        }

        public NitroFile GetFileFromInternalID(ushort intid)
        {
            if (intid >= 0x8000)
            {
                string[] narcs = { "", "", "vs1", "vs2", "vs3", "vs4" };
                ushort narcid = (ushort)(m_Version == Version.EUR ? ((intid >> 10) & 0x1F) : ((intid >> 12) & 0x7));
                ushort fileid = (ushort)(intid & 0x3FF);

                string narcname = "ARCHIVE/" + narcs[narcid] + ".narc";

                return new NARCFile(new NARC(this, GetFileIDFromName(narcname)), fileid);
            }

            return new NitroFile(this, m_FileTable[intid]);
        }

        public string GetFileNameFromID(ushort id)
        {
            return m_FileEntries[id].FullName;
        }

        public FileEntry[] GetFileEntries()
        {
            return m_FileEntries;
        }

        public DirEntry[] GetDirEntries()
        {
            return m_DirEntries;
        }

        public uint GetOverlayEntryOffset(uint ovlid) { return m_OverlayEntries[ovlid].EntryOffset; }

        public OverlayEntry[] GetOverlayEntries()
        {
            return m_OverlayEntries;
        }

        public byte Read8(uint addr) { m_FileStream.Position = addr; return m_BinReader.ReadByte(); }
        public ushort Read16(uint addr) { m_FileStream.Position = addr; return m_BinReader.ReadUInt16(); }
        public uint Read32(uint addr) { m_FileStream.Position = addr; return m_BinReader.ReadUInt32(); }
        public byte[] ReadBlock(uint addr, int size) { m_FileStream.Position = addr; return m_BinReader.ReadBytes(size); }

        public void Write8(uint addr, byte value) { m_FileStream.Position = addr; m_BinWriter.Write(value); }
        public void Write16(uint addr, ushort value) { m_FileStream.Position = addr; m_BinWriter.Write(value); }
        public void Write32(uint addr, uint value) { m_FileStream.Position = addr; m_BinWriter.Write(value); }
        public void WriteBlock(uint addr, byte[] data) { m_FileStream.Position = addr; m_BinWriter.Write(data); }

        public uint GetLevelOverlayID(int levelid) { return m_LevelOvlIDTable[levelid]; }

        public void MakeRoom(uint addr, uint amount)
        {
            uint actualend = m_UsedSize + ROM_END_MARGIN;
            if (addr < actualend)
            {
                m_FileStream.Position = addr;
                byte[] tomove = m_BinReader.ReadBytes((int)(actualend - addr));
                m_FileStream.Position = addr + amount;
                m_BinWriter.Write(tomove);
            }
        }

        private void FixPtrAt(uint addr, uint fixstart, int delta)
        {
            m_FileStream.Position = addr;
	        uint temp = m_BinReader.ReadUInt32();
	        if (temp >= fixstart)
	        {
		        temp += (uint)delta;
		        m_FileStream.Position = addr;
		        m_BinWriter.Write(temp);
	        }
        }

        public void AutoFix(ushort fileid, uint fixstart, int delta)
        {
        	// fix the internal variables
            if (ARM7Offset >= fixstart) ARM7Offset += (uint)delta;
            if (FNTOffset >= fixstart) FNTOffset += (uint)delta;
            if (FATOffset >= fixstart) FATOffset += (uint)delta;
            if (m_UsedSize >= fixstart) m_UsedSize += (uint)delta;
            else m_UsedSize = (uint)(fixstart + delta);

            for (int i = 0; i < m_FileEntries.Length; i++)
            {
	            if (m_FileEntries[i].ID == fileid)
		            continue;

	            if (m_FileEntries[i].Offset >= fixstart)
		            m_FileEntries[i].Offset += (uint)delta;
            }

            if (OVTOffset >= fixstart)
            {
	            OVTOffset += (uint)delta;

	            for (int i = 0; i < m_OverlayEntries.Length; i++)
		            m_OverlayEntries[i].EntryOffset += (uint)delta;
            }

            // fix the actual ROM
            FixPtrAt(0x20, fixstart, delta); // ARM9 bin offset
            FixPtrAt(0x30, fixstart, delta); // ARM7 bin offset
            FixPtrAt(0x40, fixstart, delta); // Filename table offset
            FixPtrAt(0x48, fixstart, delta); // FAT offset
            FixPtrAt(0x50, fixstart, delta); // ARM9 overlay offset
            FixPtrAt(0x58, fixstart, delta); // ARM7 overlay offset
            FixPtrAt(0x68, fixstart, delta); // Icon/Internal title offset
            //FixPtrAt(0x70, fixstart, delta);
            //FixPtrAt(0x74, fixstart, delta);
            //FixPtrAt(0x80, fixstart, delta);
            FixPtrAt(0x160, fixstart, delta);

            m_FileStream.Position = 0x80;
            m_BinWriter.Write(m_UsedSize);

            for (uint i = 0; i < (FATSize / 8); i++)
            {
	            m_FileStream.Position = FATOffset + (i*8);
	            uint start = m_BinReader.ReadUInt32();
	            uint end = m_BinReader.ReadUInt32();

	            if (i != fileid)
	            {
                    FixPtrAt(FATOffset + (i * 8), fixstart, delta);
	            }

	            if ((start < fixstart) && (end == fixstart) && (i != fileid))
		            continue;

                FixPtrAt(FATOffset + (i * 8) + 4, fixstart, delta);
            }
        }

        public byte[] ExtractFile(ushort fileid)
        {
            bool autorw = !m_CanRW;
            if (autorw) BeginRW();

            FileEntry fe = m_FileEntries[fileid];

            m_FileStream.Position = fe.Offset;
            byte[] data = m_BinReader.ReadBytes((int)fe.Size);

            if (autorw) EndRW();
            return data;
        }

        public void ReinsertFile(ushort fileid, byte[] data)
        {
            bool autorw = !m_CanRW;
            if (autorw) BeginRW();

            int datalength = (data.Length + 3) & ~3;

            FileEntry fe = m_FileEntries[fileid];

            UInt32 fileend = fe.Offset + fe.Size;
            int delta = (int)(datalength - fe.Size);

            // move data that comes after the file
            MakeRoom(fileend, (uint)delta);

            // write the new data for the file
            m_FileStream.Position = fe.Offset;
            m_BinWriter.Write(data);
            fe.Size = (uint)datalength;

            AutoFix(fileid, fileend, delta);

            // fix file sizes
            if (delta != 0)
            {
                m_FileEntries[fileid].Size = (uint)datalength;

                for (int o = 0; o < m_OverlayEntries.Length; o++)
                {
                    if (m_OverlayEntries[o].FileID == fileid)
                        m_OverlayEntries[o].RAMSize = (uint)datalength;
                }
            }

            // fix the header CRC16... we never know :P
            // as an example NO$GBA won't load the ROM if this CRC16 is wrong
            ushort hcrc = CalcCRC16(0, 0x15E);
            m_FileStream.Position = 0x15E;
            m_BinWriter.Write(hcrc);

            if (autorw) EndRW();
        }

        public uint AddOverlay(uint ramaddr)
        {
            // find an usable overlay ID
	        uint id = 0;
	        foreach (OverlayEntry _oe in m_OverlayEntries)
	        {
		        if (_oe.ID > id)
			        id = _oe.ID;
	        }
	        id++;

	        // add a file for the overlay
	        ushort fileid = (ushort)(FATSize / 8);

	        MakeRoom(FATOffset + FATSize, 8);
	        AutoFix(0xFFFF, FATOffset + FATSize, 8);

	        FATSize += 8;
	        m_FileStream.Position = 0x4C;
	        m_BinWriter.Write(FATSize);

	        m_FileStream.Position = FATOffset + (fileid * 8);
	        uint fileaddr = m_UsedSize;
	        m_BinWriter.Write(fileaddr);
	        m_BinWriter.Write(fileaddr);

	        Array.Resize(ref m_FileEntries, m_FileEntries.Length+1);
            FileEntry fe;
            fe.ID = fileid;
            fe.InternalID = 0xFFFF;
            fe.ParentID = 0;
            fe.Offset = fileaddr;
            fe.Size = 0;
            fe.Name = fe.FullName = "";
            m_FileEntries[fileid] = fe;

	        // and add an overlay entry
	        uint entryaddr = OVTOffset + OVTSize;

	        MakeRoom(entryaddr, 0x20);
	        AutoFix(0xFFFF, entryaddr, 0x20);

	        OVTSize += 0x20;
	        m_FileStream.Position = 0x54;
	        m_BinWriter.Write(OVTSize);

	        m_FileStream.Position = entryaddr;
	        m_BinWriter.Write(id);
	        m_BinWriter.Write(ramaddr);
	        m_BinWriter.Write((uint)0);
	        m_BinWriter.Write((uint)0);
	        m_BinWriter.Write(ramaddr);
	        m_BinWriter.Write(ramaddr);
	        m_BinWriter.Write((uint)fileid);
	        m_BinWriter.Write((uint)0);

	        Array.Resize(ref m_OverlayEntries, m_OverlayEntries.Length+1);
            OverlayEntry oe;
            oe.EntryOffset = entryaddr;
            oe.ID = id;
            oe.FileID = fileid;
            oe.RAMAddress = ramaddr;
            oe.RAMSize = 0;
            oe.BSSSize = 0;
	        m_OverlayEntries[id] = oe;

	        return id;
        }


        public enum Version
	    {
		    UNK = -1,
		    USA_v1,
		    USA_v2,
            JAP,
		    EUR
	    };
        public Version m_Version;


        private string m_Path;

        private bool m_CanRW;
        private bool m_Buffered;
        private Stream m_FileStream;
        private BinaryReader m_BinReader;
        private BinaryWriter m_BinWriter;

        private uint m_LevelOvlIDTableOffset;
        private uint m_FileTableOffset, m_FileTableLength;
        private ushort[] m_FileTable;
        private uint[] m_LevelOvlIDTable;

        private uint m_UsedSize;

        private uint ARM9RAMAddress;

        private uint ARM7Offset, ARM7RAMAddress, ARM7Size;

        private uint FNTOffset, FNTSize;
        private uint FATOffset, FATSize;
        private uint OVTOffset, OVTSize;

        public struct DirEntry
        {
            public ushort ID;
            public ushort ParentID;
            public string Name;
            public string FullName;
        }

        public struct FileEntry
        {
            public ushort ID;
            public ushort InternalID;
            public ushort ParentID;
            public string Name;
            public string FullName;
            public uint Offset;
            public uint Size;
        }

        public struct OverlayEntry
        {
            public uint EntryOffset;
            public uint ID;
            public ushort FileID;
            public uint RAMAddress;
            public uint RAMSize, BSSSize;
        }

        private DirEntry[] m_DirEntries;
        private FileEntry[] m_FileEntries;
        private OverlayEntry[] m_OverlayEntries;
    }


    public class INitroROMBlock
    {
        public INitroROMBlock() { }

        public INitroROMBlock(byte[] data)
        {
            m_Data = data;
        }

        public byte Read8(uint addr) { return m_Data[addr]; }
	    public ushort Read16(uint addr) { return (ushort)(m_Data[addr] | (m_Data[addr+1]<<8)); }
	    public uint Read32(uint addr) { return (uint)(m_Data[addr] | (m_Data[addr+1]<<8) | (m_Data[addr+2]<<16) | (m_Data[addr+3]<<24)); }
        public uint ReadVar(uint addr, uint size)
        {
            switch(size)
            {
                case 1: return Read8(addr);
                case 2: return Read16(addr);
                case 4: return Read32(addr);
                default: throw new InvalidDataException("Size must be 1, 2, or 4, not " + size + "!");
            }
        }

	    public byte[] ReadBlock(uint addr, uint len)
	    {
		    byte[] ret = new byte[len];
		    Array.Copy(m_Data, (int)addr, ret, 0, (int)len);
		    return ret;
	    }

	    // reads a string until the specified length or until a null byte
	    // if length is zero, no length limit is applied
	    public string ReadString(uint addr, int len)
	    {
		    string result = "";

		    for (int i = 0; ; i++)
		    {
			    if ((len > 0) && (i >= len)) break;

			    char ch = (char)m_Data[addr + i];
			    if (ch == 0) break;

			    result += ch;
		    }

		    return result;
	    }

        public void Write8(uint addr, byte value) { AutoResize(addr, 1); m_Data[addr] = value; }
        public void Write16(uint addr, ushort value) { AutoResize(addr, 2); m_Data[addr] = (byte)(value & 0xFF); m_Data[addr + 1] = (byte)(value >> 8); }
        public void Write32(uint addr, uint value) { AutoResize(addr, 4); m_Data[addr] = (byte)(value & 0xFF); m_Data[addr + 1] = (byte)((value >> 8) & 0xFF); m_Data[addr + 2] = (byte)((value >> 16) & 0xFF); m_Data[addr + 3] = (byte)(value >> 24); }
        public void WriteVar(uint addr, uint size, uint value)
        {
            switch (size)
            {
                case 1: Write8(addr, (byte)value); break;
                case 2: Write16(addr, (ushort)value); break;
                case 4: Write32(addr, value); break;
                default: throw new InvalidDataException("Size must be 1, 2, or 4, not " + size + "!");
            }
        }

        public void WriteBlock(uint addr, byte[] data)
	    {
		    AutoResize(addr, (uint)data.Length);
		    Array.Copy(data, 0, m_Data, addr, data.Length);
	    }

        public void WriteString(uint addr, string str, int len)
        {
            AutoResize(addr, (uint)((len > 0) ? len : (str.Length + 1)));

            int i = 0;
            for (; ; i++)
            {
                if ((len > 0) && (i >= len)) break;
                if (i >= str.Length) break;

                m_Data[addr + i] = (byte)str[i];
            }

            if ((len == 0) || (i < len))
                m_Data[addr + i] = 0;
        }

        public void RemoveSpace(uint addr, uint size)
        {
            WriteBlock(addr, ReadBlock(addr + size, (uint)m_Data.Length - addr - size));
            Array.Resize(ref m_Data, m_Data.Length - (int)size);
        }
        public void AddSpace(uint addr, uint size)
        {
            //will get auto-resized
            WriteBlock(addr + size, ReadBlock(addr, (uint)m_Data.Length - addr));
            if (size >= 0x80000000u)
                Array.Resize(ref m_Data, m_Data.Length + (int)size);
        }
        public void ResizeSpace(uint addr, uint oldLen, uint newLen)
        {
            if (oldLen == newLen)
                return;
            if (oldLen < newLen)
                AddSpace(addr + oldLen, newLen - oldLen);
            else
                RemoveSpace(addr + newLen, oldLen - newLen);
        }

        public void Clear()
        {
            Array.Resize(ref m_Data, 0);
        }

        private void AutoResize(uint addr, uint size)
	    {
		    if ((addr + size) > m_Data.Length)
			    Array.Resize(ref m_Data, (int)(addr + size));
	    }

        public void FixPtrAt(uint addr, uint fixstart, int delta)
        {
            uint temp = Read32(addr);
            if (temp >= fixstart)
                Write32(addr, temp + (uint)delta);
        }

        // To be implemented by subclasses
        public virtual void SaveChanges() { }

	    public NitroROM m_ROM;
	    public byte[] m_Data;
    }
}
