using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM64DSe.SM64DSFormats
{
    class SDAT //Sound Data
    {
        private NitroFile m_File;
        public string m_Filename;

        private byte[] GetFile(NitroFile sdat, uint fileID)
        {
            uint fatOffset = sdat.Read32(0x20);
            return sdat.ReadBlock(sdat.Read32(fatOffset + 12 + 16 * fileID),
                                  sdat.Read32(fatOffset + 16 + 16 * fileID));
        }

        public struct SDAT_File
        {
            public string m_Name;
            public INitroROMBlock m_Data;

            public SDAT_File(string name)
            {
                m_Name = name;
                m_Data = null;
            }

            public SDAT_File(string name, NitroFile file, uint fileID)
            {
                m_Name = name;
                uint fatOffset = file.Read32(0x20);
                m_Data = new INitroROMBlock();
                m_Data.m_Data = file.ReadBlock(file.Read32(fatOffset + 12 + 16 * fileID),
                                               file.Read32(fatOffset + 16 + 16 * fileID));
            }

            public SDAT_File(string name, INitroROMBlock data)
            {
                m_Name = name;
                m_Data = data;
            }
        }

        public class Record { }
        public class Sequence : Record
        {
            public SDAT_File m_File;
            public Bank m_Bank;
            public int m_Volume;
            public int m_ChannelPriority;
            public int m_PlayerPriority;
            public Player m_Player;
        }
        public class SequenceArchive : Record
        {
            public class Sequence
            {
                public string m_Filename;
                public uint m_SeqOffset;
                public Bank m_Bank;
                public int m_Volume;
                public int m_ChannelPriority;
                public int m_PlayerPriority;
                public Player m_Player;
            }
            public string m_Filename;
            public INitroROMBlock m_Data;
            public Sequence[] m_Sequences;
        }
        public class Bank : Record
        {
            public SDAT_File m_File;
            public SWAR[] m_WaveArcs; //do NOT assume that null comes after non-null here.
        }
        public class Player : Record
        {
            public string m_Filename;
            public uint m_MaxNumSeqs;
            public uint m_HeapSize;
        }
        public class Group : Record
        {
            public class Entry
            {
                public int m_LoadFlag;
                public Record m_Record;
            }

            public string m_Filename;
            public Entry[] m_Entries;
        }
        public Sequence[] m_Sequences { get; private set; }
        public SequenceArchive[] m_SeqArcs { get; private set; }
        public Bank[] m_Banks { get; private set; }
        public SWAR[] m_WaveArcs { get; private set; }
        public Player[] m_Players { get; private set; }
        public Group[] m_Groups { get; private set; }

        //the last 2 record types don't exist in SM64DS.

        public SDAT(NitroFile file)
        {
            m_File = file;
            m_Filename = file.m_Name;

            uint symbolOffset = file.Read32(0x10);
            uint infoOffset = file.Read32(0x18);

            INitroROMBlock symbolBlock = new INitroROMBlock(file.ReadBlock(file.Read32(0x10), file.Read32(0x14)));
            INitroROMBlock infoBlock   = new INitroROMBlock(file.ReadBlock(file.Read32(0x18), file.Read32(0x1c)));
            INitroROMBlock fatBlock    = new INitroROMBlock(file.ReadBlock(file.Read32(0x20), file.Read32(0x24)));
            fatBlock.m_Data = fatBlock.ReadBlock(0x0c, fatBlock.Read32(0x08) * 0x10); //strip the header

            m_Sequences = new Sequence       [symbolBlock.Read32(symbolBlock.Read32(0x08))];
            m_SeqArcs   = new SequenceArchive[symbolBlock.Read32(symbolBlock.Read32(0x0c))];
            m_Banks     = new Bank           [symbolBlock.Read32(symbolBlock.Read32(0x10))];
            m_WaveArcs  = new SWAR           [symbolBlock.Read32(symbolBlock.Read32(0x14))];
            m_Players   = new Player         [symbolBlock.Read32(symbolBlock.Read32(0x18))];
            m_Groups    = new Group          [symbolBlock.Read32(symbolBlock.Read32(0x1c))];

            INitroROMBlock symbSeqBlock = new INitroROMBlock(
                symbolBlock.ReadBlock(symbolBlock.Read32(0x08) + 4, (uint)m_Sequences.Length * 4));
            INitroROMBlock symbSeqArcBlock = new INitroROMBlock(
                symbolBlock.ReadBlock(symbolBlock.Read32(0x0c) + 4, (uint)m_SeqArcs.Length * 8));
            INitroROMBlock symbBankBlock = new INitroROMBlock(
                symbolBlock.ReadBlock(symbolBlock.Read32(0x10) + 4, (uint)m_Banks.Length * 4));
            INitroROMBlock symbWaveBlock = new INitroROMBlock(
                symbolBlock.ReadBlock(symbolBlock.Read32(0x14) + 4, (uint)m_WaveArcs.Length * 4));
            INitroROMBlock symbPlayerBlock = new INitroROMBlock(
                symbolBlock.ReadBlock(symbolBlock.Read32(0x18) + 4, (uint)m_Players.Length * 4));
            INitroROMBlock symbGroupBlock = new INitroROMBlock(
                symbolBlock.ReadBlock(symbolBlock.Read32(0x1c) + 4, (uint)m_Groups.Length * 4));

            INitroROMBlock infoSeqBlock = new INitroROMBlock(
                infoBlock.ReadBlock(infoBlock.Read32(0x08) + 4, (uint)m_Sequences.Length * 4));
            INitroROMBlock infoSeqArcBlock = new INitroROMBlock(
                infoBlock.ReadBlock(infoBlock.Read32(0x0c) + 4, (uint)m_SeqArcs.Length * 4));
            INitroROMBlock infoBankBlock = new INitroROMBlock(
                infoBlock.ReadBlock(infoBlock.Read32(0x10) + 4, (uint)m_Banks.Length * 4));
            INitroROMBlock infoWaveBlock = new INitroROMBlock(
                infoBlock.ReadBlock(infoBlock.Read32(0x14) + 4, (uint)m_WaveArcs.Length * 4));
            INitroROMBlock infoPlayerBlock = new INitroROMBlock(
                infoBlock.ReadBlock(infoBlock.Read32(0x18) + 4, (uint)m_Players.Length * 4));
            INitroROMBlock infoGroupBlock = new INitroROMBlock(
                infoBlock.ReadBlock(infoBlock.Read32(0x1c) + 4, (uint)m_Groups.Length * 4));

            INitroROMBlock buffer = new INitroROMBlock();
            for (uint i = 0; i < m_WaveArcs.Length; ++i)
            {
                if (infoWaveBlock.Read32(4 * i) == 0) //An offset of 0 means "Does Not Exist".
                    continue;

                uint fileID = infoBlock.Read32(infoWaveBlock.Read32(4 * i));
                SWAR waveArc = new SWAR(symbolBlock.ReadString(symbWaveBlock.Read32(4 * i), -1), 
                    new INitroROMBlock(GetFile(file, fileID)));
                m_WaveArcs[i] = waveArc;
            }

            for (uint i = 0; i < m_Banks.Length; ++i)
            {
                if (infoBankBlock.Read32(4 * i) == 0)
                    continue;

                buffer.m_Data = infoBlock.ReadBlock(infoBankBlock.Read32(4 * i), 12);
                Bank bank = new Bank();
                bank.m_File = new SDAT_File(symbolBlock.ReadString(symbBankBlock.Read32(4 * i), -1),
                    file, buffer.Read32(0));
                bank.m_WaveArcs = new SWAR[4];
                for (uint j = 0; j < 4; ++j)
                {
                    uint waveArcID = buffer.Read16(4 + 2 * j);
                    bank.m_WaveArcs[j] = waveArcID != 0xffff ? m_WaveArcs[waveArcID] : null;
                }
                m_Banks[i] = bank;
            }

            for (uint i = 0; i < m_Players.Length; ++i)
            {
                buffer.m_Data = infoBlock.ReadBlock(infoPlayerBlock.Read32(4 * i), 8);
                Player player = new Player();
                player.m_Filename = symbolBlock.ReadString(symbPlayerBlock.Read32(4 * i), -1);
                player.m_MaxNumSeqs = buffer.Read8(0);
                player.m_HeapSize = buffer.Read32(4);
                m_Players[i] = player;
            }

            for (uint i = 0; i < m_Sequences.Length; ++i)
            {
                if (infoSeqBlock.Read32(4 * i) == 0)
                    continue;

                buffer.m_Data = infoBlock.ReadBlock(infoSeqBlock.Read32(4 * i), 12);
                Sequence sequence = new Sequence();
                sequence.m_File = new SDAT_File(symbolBlock.ReadString(symbSeqBlock.Read32(4 * i), -1),
                    file, buffer.Read32(0));
                sequence.m_Bank = m_Banks[buffer.Read16(4)];
                sequence.m_Volume = buffer.Read8(6);
                sequence.m_ChannelPriority = buffer.Read8(7);
                sequence.m_PlayerPriority = buffer.Read8(8);
                sequence.m_Player = m_Players[buffer.Read8(9)];
                m_Sequences[i] = sequence;
            }

            for(uint i = 0; i < m_SeqArcs.Length; ++i)
            {
                if (infoSeqArcBlock.Read32(4 * i) == 0)
                    continue;

                uint fileID = infoBlock.Read32(infoSeqArcBlock.Read32(4 * i));
                SequenceArchive seqArc = new SequenceArchive();
                seqArc.m_Filename = symbolBlock.ReadString(symbSeqArcBlock.Read32(8 * i), -1);
                seqArc.m_Sequences = new SequenceArchive.Sequence[
                    symbolBlock.Read32(symbSeqArcBlock.Read32(8 * i + 4))];

                INitroROMBlock symbSeqArcSeqBlock = new INitroROMBlock(symbolBlock.ReadBlock(
                    symbSeqArcBlock.Read32(8 * i + 4) + 4, (uint)seqArc.m_Sequences.Length * 4));
                INitroROMBlock ssar = new INitroROMBlock(GetFile(file, fileID));

                INitroROMBlock ssarInfo = new INitroROMBlock(
                    ssar.ReadBlock(0x20, (uint)seqArc.m_Sequences.Length * 12));
                seqArc.m_Data = new INitroROMBlock(
                    ssar.ReadBlock(ssar.Read32(0x18), (uint)ssar.m_Data.Length - ssar.Read32(0x18)));
                for(uint j = 0; j < seqArc.m_Sequences.Length; ++j)
                {
                    buffer.m_Data = ssarInfo.ReadBlock(12 * j, 12);
                    SequenceArchive.Sequence sequence = new SequenceArchive.Sequence();
                    sequence.m_Filename = symbolBlock.ReadString(symbSeqArcSeqBlock.Read32(4 * j), -1);
                    sequence.m_SeqOffset = buffer.Read32(0);
                    sequence.m_Bank = m_Banks[buffer.Read16(4)];
                    sequence.m_Volume = buffer.Read8(6);
                    sequence.m_ChannelPriority = buffer.Read8(7);
                    sequence.m_PlayerPriority = buffer.Read8(8);
                    sequence.m_Player = m_Players[buffer.Read8(9)];
                    seqArc.m_Sequences[j] = sequence;
                }
                m_SeqArcs[i] = seqArc;
            }

            for(uint i = 0; i < m_Groups.Length; ++i)
            {
                if (infoGroupBlock.Read32(4 * i) == 0)
                    continue;

                Group group = new Group();
                group.m_Entries = new Group.Entry[
                    infoBlock.Read32(infoGroupBlock.Read32(4 * i))];
                buffer.m_Data = infoBlock.ReadBlock(infoGroupBlock.Read32(4 * i) + 4,
                    (uint)group.m_Entries.Length * 8);
                group.m_Filename = symbolBlock.ReadString(symbGroupBlock.Read32(4 * i), -1);

                for(uint j = 0; j < group.m_Entries.Length; ++j)
                {
                    uint index = buffer.Read32(8 * j + 4);
                    Group.Entry entry = new Group.Entry();
                    entry.m_LoadFlag = buffer.Read8(8 * j + 1);
                    switch (buffer.Read8(8 * j))
                    {
                        case 0:
                            entry.m_Record = m_Sequences[index]; break;
                        case 3:
                            entry.m_Record = m_SeqArcs[index]; break;
                        case 1:
                            entry.m_Record = m_Banks[index]; break;
                        case 2:
                            entry.m_Record = m_WaveArcs[index]; break;
                        default:
                            throw new Exception("Group " + i + ", Record " + j +
                                " has an unexpected record type: " + buffer.Read8(8 * j));
                    }
                    group.m_Entries[j] = entry;
                }
                m_Groups[i] = group;
            }
        }
    }
}
