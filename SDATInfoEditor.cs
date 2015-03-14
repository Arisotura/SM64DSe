using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SM64DSe
{
    public partial class SDATInfoEditor : Form
    {
        SDAT m_SDAT;
        SDAT.SDATInfo.SDATInfoRecordBase m_SelectedNode;

        public SDATInfoEditor()
        {
            InitializeComponent();

            m_SDAT = new SDAT(Program.m_ROM.GetFileFromName("data/sound_data.sdat"));

            InitTreeView();
        }

        private void InitTreeView()
        {
            tvSDAT.Nodes.Add("root", "SDAT");
            tvSDAT.Nodes["root"].Nodes.Add("info", "INFO Block");

            TreeNode info = tvSDAT.Nodes["root"].Nodes["info"];
            info.Nodes.Add("records0", "Record 0 \"SEQ\"");
            info.Nodes.Add("records1", "Record 1 \"SEQARC\"");
            info.Nodes.Add("records2", "Record 2 \"BANK\"");
            info.Nodes.Add("records3", "Record 3 \"WAVEARC\"");
            info.Nodes.Add("records4", "Record 4 \"PLAYER\"");
            info.Nodes.Add("records5", "Record 5 \"GROUP\"");
            info.Nodes.Add("records6", "Record 6 \"PLAYER2\"");
            info.Nodes.Add("records7", "Record 7 \"STRM\"");

            TreeNode infoRecords0 = info.Nodes["records0"];
            SDAT.SDATInfo.SDATInfoSEQ[] records0SEQ = m_SDAT.m_Info.GetRecords0SEQ();
            for (int i = 0; i < records0SEQ.Length; i++)
            {
                string id = String.Format("{0:D3}", i);
                infoRecords0.Nodes.Add(id, "Entry " + id);
                infoRecords0.Nodes[id].Tag = "root.info.records0." + id;
            }
            TreeNode infoRecords1 = info.Nodes["records1"];
            SDAT.SDATInfo.SDATInfoSEQARC[] records1SEQARC = m_SDAT.m_Info.GetRecords1SEQARC();
            for (int i = 0; i < records1SEQARC.Length; i++)
            {
                string id = String.Format("{0:D3}", i);
                infoRecords1.Nodes.Add(id, "Entry " + id);
                infoRecords1.Nodes[id].Tag = "root.info.records1." + id;
            }
            TreeNode infoRecords2 = info.Nodes["records2"];
            SDAT.SDATInfo.SDATInfoBANK[] records2BANK = m_SDAT.m_Info.GetRecords2BANK();
            for (int i = 0; i < records2BANK.Length; i++)
            {
                string id = String.Format("{0:D3}", i);
                infoRecords2.Nodes.Add(id, "Entry " + id);
                infoRecords2.Nodes[id].Tag = "root.info.records2." + id;
            }
            TreeNode infoRecords3 = info.Nodes["records3"];
            SDAT.SDATInfo.SDATInfoWAVEARC[] records3WAVEARC = m_SDAT.m_Info.GetRecords3WAVEARC();
            for (int i = 0; i < records3WAVEARC.Length; i++)
            {
                string id = String.Format("{0:D3}", i);
                infoRecords3.Nodes.Add(id, "Entry " + id);
                infoRecords3.Nodes[id].Tag = "root.info.records3." + id;
            }
            TreeNode infoRecords4 = info.Nodes["records4"];
            SDAT.SDATInfo.SDATInfoPLAYER[] records4PLAYER = m_SDAT.m_Info.GetRecords4PLAYER();
            for (int i = 0; i < records4PLAYER.Length; i++)
            {
                string id = String.Format("{0:D3}", i);
                infoRecords4.Nodes.Add(id, "Entry " + id);
                infoRecords4.Nodes[id].Tag = "root.info.records4." + id;
            }
            TreeNode infoRecords5 = info.Nodes["records5"];
            SDAT.SDATInfo.SDATInfoGROUP[] records5GROUP = m_SDAT.m_Info.GetRecords5GROUP();
            for (int i = 0; i < records5GROUP.Length; i++)
            {
                string id = String.Format("{0:D3}", i);
                infoRecords5.Nodes.Add(id, "Entry " + id);
                infoRecords5.Nodes[id].Tag = "root.info.records5." + id;

                TreeNode currentEntry = infoRecords5.Nodes[id];
                for (int j = 0; j < records5GROUP[i].GetGroupEntries().Length; j++)
                {
                    string groupid = String.Format("{0:D3}", j);
                    currentEntry.Nodes.Add(groupid, "Group " + groupid);
                    currentEntry.Nodes[groupid].Tag = "root.info.records5." + id + "." + groupid;
                }
            }
            TreeNode infoRecords6 = info.Nodes["records6"];
            SDAT.SDATInfo.SDATInfoPLAYER2[] records6PLAYER2 = m_SDAT.m_Info.GetRecords6PLAYER2();
            for (int i = 0; i < records6PLAYER2.Length; i++)
            {
                string id = String.Format("{0:D3}", i);
                infoRecords6.Nodes.Add(id, "Entry " + id);
                infoRecords6.Nodes[id].Tag = "root.info.records6." + id;
            }
            TreeNode infoRecords7 = info.Nodes["records7"];
            SDAT.SDATInfo.SDATInfoSTREAM[] records7STREAM = m_SDAT.m_Info.GetRecords7STREAM();
            for (int i = 0; i < records7STREAM.Length; i++)
            {
                string id = String.Format("{0:D3}", i);
                infoRecords7.Nodes.Add(id, "Entry " + id);
                infoRecords7.Nodes[id].Tag = "root.info.records7." + id;
            }
        }

        private void tvSDAT_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null || e.Node.Tag == null)
                return;
            else
            {
                string tag = e.Node.Tag.ToString();
                string[] location = tag.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                switch (location[1].ToLowerInvariant())
                {
                    case "info":
                        {
                            switch (location[2].ToLowerInvariant())
                            {
                                case "records0":
                                    {
                                        int entryIndex = int.Parse(location[3]);
                                        m_SelectedNode = m_SDAT.m_Info.GetRecords0SEQ()[entryIndex];
                                        pgData.SelectedObject = m_SelectedNode.GenerateProperties();
                                    }
                                    break;
                                case "records1":
                                    {
                                        int entryIndex = int.Parse(location[3]);
                                        m_SelectedNode = m_SDAT.m_Info.GetRecords1SEQARC()[entryIndex];
                                        pgData.SelectedObject = m_SelectedNode.GenerateProperties();
                                    }
                                    break;
                                case "records2":
                                    {
                                        int entryIndex = int.Parse(location[3]);
                                        m_SelectedNode = m_SDAT.m_Info.GetRecords2BANK()[entryIndex];
                                        pgData.SelectedObject = m_SelectedNode.GenerateProperties();
                                    }
                                    break;
                                case "records3":
                                    {
                                        int entryIndex = int.Parse(location[3]);
                                        m_SelectedNode = m_SDAT.m_Info.GetRecords3WAVEARC()[entryIndex];
                                        pgData.SelectedObject = m_SelectedNode.GenerateProperties();
                                    }
                                    break;
                                case "records4":
                                    {
                                        int entryIndex = int.Parse(location[3]);
                                        m_SelectedNode = m_SDAT.m_Info.GetRecords4PLAYER()[entryIndex];
                                        pgData.SelectedObject = m_SelectedNode.GenerateProperties();
                                    }
                                    break;
                                case "records5":
                                    {
                                        int entryIndex = int.Parse(location[3]);
                                        if (location.Length <= 4)
                                            break;
                                        int groupIndex = int.Parse(location[4]);
                                        m_SelectedNode = m_SDAT.m_Info.GetRecords5GROUP()[entryIndex].GetGroupEntries()[groupIndex];
                                        pgData.SelectedObject = m_SelectedNode.GenerateProperties();
                                    }
                                    break;
                                case "records6":
                                    {
                                        int entryIndex = int.Parse(location[3]);
                                        m_SelectedNode = m_SDAT.m_Info.GetRecords6PLAYER2()[entryIndex];
                                        pgData.SelectedObject = m_SelectedNode.GenerateProperties();
                                    }
                                    break;
                                case "records7":
                                    {
                                        int entryIndex = int.Parse(location[3]);
                                        m_SelectedNode = m_SDAT.m_Info.GetRecords7STREAM()[entryIndex];
                                        pgData.SelectedObject = m_SelectedNode.GenerateProperties();
                                    }
                                    break;
                            }
                        }
                        break;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            m_SDAT.m_File.SaveChanges();
        }

        private void pgData_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (m_SelectedNode != null)
            {
                m_SelectedNode.SetProperty(e.ChangedItem.Label, e.ChangedItem.Value);
                pgData.SelectedObject = m_SelectedNode.GenerateProperties();
            }
        }
    }

    public class SDAT
    {
        public NitroFile m_File;
        public string m_FileName;

        public SDATHeader m_Header;
        public SDATInfo m_Info;

        public SDAT(NitroFile sdat)
        {
            m_File = sdat;
            m_FileName = m_File.m_Name;

            m_Header = new SDATHeader(m_File);
            m_Info = new SDATInfo(m_File, m_Header.m_InfoOffset);
        }

        public class SDATHeader
        {
            public byte[] m_Type; // 'SDAT' (4)
            public uint m_Magic; // 0x0100feff
            public uint m_FileSize;
            public ushort m_Size;
            public ushort m_Block; // usually 4, but some have 3 only ( Symbol Block omitted )
            public uint m_SymbolOffset; // offset of Symbol Block = 0x40
            public uint m_SymbolSize; // size of Symbol Block
            public uint m_InfoOffset; // offset of Info Block
            public uint m_InfoSize; // size of Info Block
            public uint m_FatOffset; // offset of FAT
            public uint m_FatSize; // size of FAT
            public uint m_FileBlockOffset; // offset of File Block
            public uint m_FileBlockSize; // size of File Block
            public byte[] m_Reserved;// unused, 0s	(16)

            public SDATHeader(NitroFile sdat)
            {
                m_Type = sdat.ReadBlock(0x00, 4);
                m_Magic = sdat.Read32(0x04);
                m_FileSize = sdat.Read32(0x08);
                m_Size = sdat.Read16(0x0C);
                m_Block = sdat.Read16(0x0E);
                m_SymbolOffset = sdat.Read32(0x10);
                m_SymbolSize = sdat.Read32(0x14);
                m_InfoOffset = sdat.Read32(0x18);
                m_InfoSize = sdat.Read32(0x1C);
                m_FatOffset = sdat.Read32(0x20);
                m_FatSize = sdat.Read32(0x24);
                m_FileBlockOffset = sdat.Read32(0x28);
                m_FileBlockSize = sdat.Read32(0x2C);
                m_Reserved = sdat.ReadBlock(0x30, 16);
            }
        }

        public class SDATInfo
        {
            // All offsets are relative to this block's starting address. 

            char[] m_Type; // 'INFO'
            uint m_Size; // size of this Info Block
            uint[] m_RecordOffset; // offset of a Record (8)
            byte[] m_Reserved; // unused, 0s (24)

            private uint m_Offset;

            SDATInfoRecord[] m_Records;
            SDATInfoSEQ[] m_Records0SEQ;
            SDATInfoSEQARC[] m_Records1SEQARC;
            SDATInfoBANK[] m_Records2BANK;
            SDATInfoWAVEARC[] m_Records3WAVEARC;
            SDATInfoPLAYER[] m_Records4PLAYER;
            SDATInfoGROUP[] m_Records5GROUP;
            SDATInfoPLAYER2[] m_Records6PLAYER2;
            SDATInfoSTREAM[] m_Records7STREAM;

            public SDATInfo(NitroFile sdat, uint offset)
            {
                m_Offset = offset;
                m_Type = sdat.ReadString(m_Offset + 0x00, 4).ToCharArray();
                m_Size = sdat.Read32(m_Offset + 0x04);
                m_RecordOffset = new uint[8];
                for (int i = 0; i < 8; i++)
                    m_RecordOffset[i] = sdat.Read32(m_Offset + 0x08 + (uint)(i * 4));
                m_Reserved = sdat.ReadBlock(m_Offset + 0x28, 24);

                m_Records = new SDATInfoRecord[8];
                for (int i = 0; i < 8; i++)
                {
                    m_Records[i] = new SDATInfoRecord(sdat, m_Offset + m_RecordOffset[i]);
                }
                m_Records0SEQ = new SDATInfoSEQ[m_Records[0].m_Count];
                for (int i = 0; i < m_Records[0].m_Count; i++)
                {
                    m_Records0SEQ[i] = new SDATInfoSEQ(sdat, m_Offset + m_Records[0].m_EntryOffset[i]);
                }
                m_Records1SEQARC = new SDATInfoSEQARC[m_Records[1].m_Count];
                for (int i = 0; i < m_Records[1].m_Count; i++)
                {
                    m_Records1SEQARC[i] = new SDATInfoSEQARC(sdat, m_Offset + m_Records[1].m_EntryOffset[i]);
                }
                m_Records2BANK = new SDATInfoBANK[m_Records[2].m_Count];
                for (int i = 0; i < m_Records[2].m_Count; i++)
                {
                    m_Records2BANK[i] = new SDATInfoBANK(sdat, m_Offset + m_Records[2].m_EntryOffset[i]);
                }
                m_Records3WAVEARC = new SDATInfoWAVEARC[m_Records[3].m_Count];
                for (int i = 0; i < m_Records[3].m_Count; i++)
                {
                    m_Records3WAVEARC[i] = new SDATInfoWAVEARC(sdat, m_Offset + m_Records[3].m_EntryOffset[i]);
                }
                m_Records4PLAYER = new SDATInfoPLAYER[m_Records[4].m_Count];
                for (int i = 0; i < m_Records[4].m_Count; i++)
                {
                    m_Records4PLAYER[i] = new SDATInfoPLAYER(sdat, m_Offset + m_Records[4].m_EntryOffset[i]);
                }
                m_Records5GROUP = new SDATInfoGROUP[m_Records[5].m_Count];
                for (int i = 0; i < m_Records[5].m_Count; i++)
                {
                    m_Records5GROUP[i] = new SDATInfoGROUP(sdat, m_Offset + m_Records[5].m_EntryOffset[i]);
                }
                m_Records6PLAYER2 = new SDATInfoPLAYER2[m_Records[6].m_Count];
                for (int i = 0; i < m_Records[6].m_Count; i++)
                {
                    m_Records6PLAYER2[i] = new SDATInfoPLAYER2(sdat, m_Offset + m_Records[6].m_EntryOffset[i]);
                }
                m_Records7STREAM = new SDATInfoSTREAM[m_Records[7].m_Count];
                for (int i = 0; i < m_Records[7].m_Count; i++)
                {
                    m_Records7STREAM[i] = new SDATInfoSTREAM(sdat, m_Offset + m_Records[7].m_EntryOffset[i]);
                }
            }

            public SDATInfoSEQ[] GetRecords0SEQ()
            {
                return m_Records0SEQ;
            }
            public SDATInfoSEQARC[] GetRecords1SEQARC()
            {
                return m_Records1SEQARC;
            }
            public SDATInfoBANK[] GetRecords2BANK()
            {
                return m_Records2BANK;
            }
            public SDATInfoWAVEARC[] GetRecords3WAVEARC()
            {
                return m_Records3WAVEARC;
            }
            public SDATInfoPLAYER[] GetRecords4PLAYER()
            {
                return m_Records4PLAYER;
            }
            public SDATInfoGROUP[] GetRecords5GROUP()
            {
                return m_Records5GROUP;
            }
            public SDATInfoPLAYER2[] GetRecords6PLAYER2()
            {
                return m_Records6PLAYER2;
            }
            public SDATInfoSTREAM[] GetRecords7STREAM()
            {
                return m_Records7STREAM;
            }

            public class SDATInfoRecord
            {
                public uint m_Count; // No of entries in this record
                public uint[] m_EntryOffset; // array of offsets of each entry

                private uint m_Offset;

                public SDATInfoRecord(NitroFile sdat, uint offset)
                {
                    m_Offset = offset;
                    m_Count = sdat.Read32(m_Offset);
                    m_EntryOffset = new uint[m_Count];
                    for (int i = 0; i < m_Count; i++)
                        m_EntryOffset[i] = sdat.Read32(m_Offset + 0x04 + (uint)(i * 4));
                }
            }

            public abstract class SDATInfoRecordBase
            {
                public abstract PropertyTable GenerateProperties();
                public abstract void SetProperty(string field, object newval);
                //public abstract void Write();
            }

            public class SDATInfoSEQ : SDATInfoRecordBase
            {
                public uint m_FileID;	// for accessing this file
                public ushort m_Bank;	// Associated BANK
                public byte m_Volume;	// Volume
                public byte m_ChannelPriority;
                public byte m_PlayerPriority;
                public byte m_PlayerNumber;
                public byte[] m_Unknown2; // (2)

                private NitroFile m_File;
                private uint m_Offset;

                public SDATInfoSEQ(NitroFile sdat, uint offset)
                {
                    m_File = sdat;
                    m_Offset = offset;

                    m_FileID = sdat.Read32(m_Offset + 0x00);
                    m_Bank = sdat.Read16(m_Offset + 0x04);
                    m_Volume = sdat.Read8(m_Offset + 0x06);
                    m_ChannelPriority = sdat.Read8(m_Offset + 0x07);
                    m_PlayerPriority = sdat.Read8(m_Offset + 0x08);
                    m_PlayerNumber = sdat.Read8(m_Offset + 0x09);
                    m_Unknown2 = sdat.ReadBlock(m_Offset + 0x0A, 2);
                }

                public override PropertyTable GenerateProperties()
                {
                    PropertyTable properties = new PropertyTable();

                    properties.Properties.Add(
                        new PropertySpec("File ID", typeof(uint), "", "File ID for accessing this file", m_FileID, "", typeof(UInt32Converter)));
                    properties.Properties.Add(
                        new PropertySpec("Bank", typeof(ushort), "", "Associated Bank", m_Bank, "", typeof(UInt16Converter)));
                    properties.Properties.Add(
                        new PropertySpec("Volume", typeof(byte), "", "Volume", m_Volume, "", typeof(ByteConverter)));
                    properties.Properties.Add(
                        new PropertySpec("Channel Priority", typeof(byte), "", "Channel Priority", m_ChannelPriority, "", typeof(ByteConverter)));
                    properties.Properties.Add(
                        new PropertySpec("Player Priority", typeof(byte), "", "Player Priority", m_PlayerPriority, "", typeof(ByteConverter)));
                    properties.Properties.Add(
                        new PropertySpec("Player Number", typeof(byte), "", "Player Number", m_PlayerNumber, "", typeof(ByteConverter)));
                    properties.Properties.Add(
                        new PropertySpec("Unknown 2 [0]", typeof(byte), "", "Unknown 2 [0]", m_Unknown2[0], "", typeof(ByteConverter)));
                    properties.Properties.Add(
                        new PropertySpec("Unknown 2 [1]", typeof(byte), "", "Unknown 2 [1]", m_Unknown2[1], "", typeof(ByteConverter)));

                    properties["File ID"] = m_FileID;
                    properties["Bank"] = m_Bank;
                    properties["Volume"] = m_Volume;
                    properties["Channel Priority"] = m_ChannelPriority;
                    properties["Player Priority"] = m_PlayerPriority;
                    properties["Player Number"] = m_PlayerNumber;
                    properties["Unknown 2 [0]"] = m_Unknown2[0];
                    properties["Unknown 2 [1]"] = m_Unknown2[1];

                    return properties;
                }

                public override void SetProperty(string field, object newval)
                {
                    switch (field)
                    {
                        case "File ID":
                            m_FileID = (uint)newval; break;
                        case "Bank":
                            m_Bank = (ushort)newval; break;
                        case "Volume":
                            m_Volume = (byte)newval; break;
                        case "Channel Priority":
                            m_ChannelPriority = (byte)newval; break;
                        case "Player Priority":
                            m_PlayerPriority = (byte)newval; break;
                        case "Player Number":
                            m_PlayerNumber = (byte)newval; break;
                        case "Unknown 2 [0]":
                            m_Unknown2[0] = (byte)newval; break;
                        case "Unknown 2 [1]":
                            m_Unknown2[1] = (byte)newval; break;
                    }
                }

                public /*override*/ void Write()
                {
                    m_File.Write32(m_Offset + 0x00, m_FileID);
                    m_File.Write16(m_Offset + 0x04, m_Bank);
                    m_File.Write8(m_Offset + 0x06, m_Volume);
                    m_File.Write8(m_Offset + 0x07, m_ChannelPriority);
                    m_File.Write8(m_Offset + 0x08, m_PlayerPriority);
                    m_File.Write8(m_Offset + 0x09, m_PlayerNumber);
                    m_File.WriteBlock(m_Offset + 0x0A, m_Unknown2);
                }
            }

            public class SDATInfoSEQARC : SDATInfoRecordBase
            {
                public uint m_FileID;

                private uint m_Offset;

                public SDATInfoSEQARC(NitroFile sdat, uint offset)
                {
                    m_Offset = offset;

                    m_FileID = sdat.Read32(m_Offset + 0x00);
                }

                public override PropertyTable GenerateProperties()
                {
                    PropertyTable properties = new PropertyTable();

                    properties.Properties.Add(
                        new PropertySpec("File ID", typeof(uint), "", "File ID for accessing this file", m_FileID, "", typeof(UInt32Converter)));

                    properties["File ID"] = m_FileID;

                    return properties;
                }

                public override void SetProperty(string field, object newval)
                {

                }
            }

            public class SDATInfoBANK : SDATInfoRecordBase
            {
                public uint m_FileID;
                public ushort[] m_WaveArc; // Associated WAVEARC. 0xffff if not in use (4)

                private uint m_Offset;

                public SDATInfoBANK(NitroFile sdat, uint offset)
                {
                    m_Offset = offset;

                    m_FileID = sdat.Read32(m_Offset + 0x00);
                    m_WaveArc = new ushort[4];
                    for (int i = 0; i < 4; i++)
                    {
                        m_WaveArc[i] = sdat.Read16(m_Offset + 0x04 + (uint)(i * 2));
                    }
                }

                public override PropertyTable GenerateProperties()
                {
                    PropertyTable properties = new PropertyTable();

                    properties.Properties.Add(
                        new PropertySpec("File ID", typeof(uint), "", "File ID for accessing this file", m_FileID, "", typeof(UInt32Converter)));
                    properties.Properties.Add(
                        new PropertySpec("WAVEARC [0]", typeof(ushort), "", "Associated WAVEARC. 0xffff if not in use (4) [0]", m_WaveArc[0], "", typeof(UInt16Converter)));
                    properties.Properties.Add(
                        new PropertySpec("WAVEARC [1]", typeof(ushort), "", "Associated WAVEARC. 0xffff if not in use (4) [1]", m_WaveArc[1], "", typeof(UInt16Converter)));
                    properties.Properties.Add(
                        new PropertySpec("WAVEARC [2]", typeof(ushort), "", "Associated WAVEARC. 0xffff if not in use (4) [2]", m_WaveArc[2], "", typeof(UInt16Converter)));
                    properties.Properties.Add(
                        new PropertySpec("WAVEARC [3]", typeof(ushort), "", "Associated WAVEARC. 0xffff if not in use (4) [3]", m_WaveArc[3], "", typeof(UInt16Converter)));

                    properties["File ID"] = m_FileID;
                    properties["WAVEARC [0]"] = m_WaveArc[0];
                    properties["WAVEARC [1]"] = m_WaveArc[1];
                    properties["WAVEARC [2]"] = m_WaveArc[2];
                    properties["WAVEARC [3]"] = m_WaveArc[3];

                    return properties;
                }

                public override void SetProperty(string field, object newval)
                {

                }
            }

            public class SDATInfoWAVEARC : SDATInfoRecordBase
            {
                public uint m_FileID;
                public byte m_Flag;

                private uint m_Offset;

                public SDATInfoWAVEARC(NitroFile sdat, uint offset)
                {
                    m_Offset = offset;

                    uint num = sdat.Read32(m_Offset + 0x00);
                    m_FileID = (num & 16777215u);
                    m_Flag = (byte)(num >> 24);

                    //er.Write((uint)((long)((long)this.flags << 24) | (long)((ulong)(this.fileID & 16777215u))));
                }

                public override PropertyTable GenerateProperties()
                {
                    PropertyTable properties = new PropertyTable();

                    properties.Properties.Add(
                        new PropertySpec("File ID", typeof(uint), "", "File ID for accessing this file", m_FileID, "", typeof(UInt32Converter)));
                    properties.Properties.Add(
                        new PropertySpec("Flags", typeof(byte), "", "Flags", m_Flag, "", typeof(ByteConverter)));

                    properties["File ID"] = m_FileID;
                    properties["Flags"] = m_Flag;

                    return properties;
                }

                public override void SetProperty(string field, object newval)
                {

                }
            }

            public class SDATInfoPLAYER : SDATInfoRecordBase
            {
                public byte m_SequenceMax;
                public byte m_Padding;
                public ushort m_AllocChannelBitFlag;
                public uint m_HeapSize;

                private uint m_Offset;

                public SDATInfoPLAYER(NitroFile sdat, uint offset)
                {
                    m_Offset = offset;

                    m_SequenceMax = sdat.Read8(m_Offset + 0x00);
                    m_Padding = sdat.Read8(m_Offset + 0x01);
                    m_AllocChannelBitFlag = sdat.Read16(m_Offset + 0x02);
                    m_HeapSize = sdat.Read32(m_Offset + 0x04);
                }

                public override PropertyTable GenerateProperties()
                {
                    PropertyTable properties = new PropertyTable();

                    properties.Properties.Add(
                        new PropertySpec("Sequence Max", typeof(byte), "", "Sequence Max", m_SequenceMax, "", typeof(ByteConverter)));
                    properties.Properties.Add(
                        new PropertySpec("Padding", typeof(byte), "", "Padding", m_Padding, "", typeof(ByteConverter)));
                    properties.Properties.Add(
                        new PropertySpec("Alloc. Channel Bit Flag", typeof(ushort), "", "Alloc. Channel Bit Flag", m_AllocChannelBitFlag, "", typeof(UInt16Converter)));
                    properties.Properties.Add(
                        new PropertySpec("Heap Size", typeof(uint), "", "Heap Size", m_HeapSize, "", typeof(UInt32Converter)));

                    properties["Sequence Max"] = m_SequenceMax;
                    properties["Padding"] = m_Padding;
                    properties["Alloc. Channel Bit Flag"] = m_AllocChannelBitFlag;
                    properties["Heap Size"] = m_HeapSize;

                    return properties;
                }

                public override void SetProperty(string field, object newval)
                {

                }
            }

            public class SDATInfoGROUP : SDATInfoRecordBase
            {
                public uint m_Count;
                public GroupEntry[] m_GroupEntries;

                private uint m_Offset;

                public SDATInfoGROUP(NitroFile sdat, uint offset)
                {
                    m_Offset = offset;

                    m_Count = sdat.Read32(m_Offset + 0x00);
                    m_GroupEntries = new GroupEntry[m_Count];
                    for (int i = 0; i < m_Count; i++)
                    {
                        m_GroupEntries[i] = new GroupEntry(sdat, m_Offset + 0x04 + (uint)(i * 8));
                    }
                }

                public GroupEntry[] GetGroupEntries()
                {
                    return m_GroupEntries;
                }

                public override PropertyTable GenerateProperties() { return null; }
                public override void SetProperty(string field, object newval) { }

                public class GroupEntry : SDATInfoRecordBase
                {
                    public byte m_Type;
                    public byte m_LoadFlag;
                    public ushort m_Padding;
                    public uint m_Index;

                    private uint m_Offset;

                    /* Value	Type
                     * 0x0700	SEQ
                     * 0x0803	SEQARC
                     * 0x0601	BANK
                     * 0x0402	WAVEARC
                     * 
                     * m_Index is the entry number in the relevant Record (SEQ/SEQARC/BANK/WAVEARC).
                     */

                    public GroupEntry(NitroFile sdat, uint offset)
                    {
                        m_Offset = offset;

                        m_Type = sdat.Read8(m_Offset + 0x00);
                        m_LoadFlag = sdat.Read8(m_Offset + 0x01);
                        m_Padding = sdat.Read16(m_Offset + 0x02);
                        m_Index = sdat.Read32(m_Offset + 0x04);
                    }

                    public override PropertyTable GenerateProperties()
                    {
                        PropertyTable properties = new PropertyTable();

                        properties.Properties.Add(
                            new PropertySpec("Type", typeof(byte), "", "Type"/*"0x0700: SEQ, 0x0803: SEQARC, 0x0601: BANK, 0x0402: WAVEARC"*/, m_Type, "", typeof(ByteConverter)));
                        properties.Properties.Add(
                            new PropertySpec("Load Flag", typeof(byte), "", "Load Flag", m_LoadFlag, "", typeof(ByteConverter)));
                        properties.Properties.Add(
                            new PropertySpec("Padding", typeof(ushort), "", "Padding", m_Padding, "", typeof(UInt16Converter)));
                        properties.Properties.Add(
                            new PropertySpec("Index", typeof(uint), "", "The entry number in the relevant Record (SEQ/SEQARC/BANK/WAVEARC)", m_Index, "", typeof(UInt32Converter)));

                        properties["Type"] = m_Type;
                        properties["Load Flag"] = m_LoadFlag;
                        properties["Padding"] = m_Padding;
                        properties["Index"] = m_Index;

                        return properties;
                    }

                    public override void SetProperty(string field, object newval)
                    {

                    }
                }
            }

            public class SDATInfoPLAYER2 : SDATInfoRecordBase
            {
                public byte m_Count; // The first byte states how many of the m_ChannelNumber[16] is used (non 0xff)
                public byte[] m_ChannelNumber; // 0xff if not in use (16)
                public byte[] m_Padding; // padding, 0s

                private uint m_Offset;

                public SDATInfoPLAYER2(NitroFile sdat, uint offset)
                {
                    m_Offset = offset;

                    m_Count = sdat.Read8(m_Offset + 0x00);
                    m_ChannelNumber = new byte[16];
                    for (int i = 0; i < 16; i++)
                        m_ChannelNumber[i] = sdat.Read8(m_Offset + 0x01 + (uint)(i));
                    m_Padding = new byte[7];
                    for (int i = 0; i < 7; i++)
                        m_Padding[i] = sdat.Read8(m_Offset + 0x11 + (uint)(i));
                }

                public override PropertyTable GenerateProperties()
                {
                    PropertyTable properties = new PropertyTable();

                    properties.Properties.Add(
                        new PropertySpec("Count", typeof(byte), "", "The first byte states how many of the ChannelNumber[16] is used (non 0xff)", m_Count, "", typeof(ByteConverter)));
                    for (int i = 0; i < 16; i++)
                    {
                        properties.Properties.Add(
                            new PropertySpec("Channel Number [" + i + "]", typeof(byte), "", "ChannelNumber [" + i + "] 0xff if not in use", m_ChannelNumber[i], "", typeof(ByteConverter)));
                    }
                    for (int i = 0; i < 7; i++)
                    {
                        properties.Properties.Add(
                            new PropertySpec("Padding [" + i + "]", typeof(byte), "", "Padding [" + i + "]", m_Padding[i], "", typeof(ByteConverter)));
                    }

                    properties["Count"] = m_Count;
                    for (int i = 0; i < 16; i++)
                        properties["Channel Number [" + i + "]"] = m_ChannelNumber[i];
                    for (int i = 0; i < 7; i++)
                        properties["Padding [" + i + "]"] = m_Padding[i];

                    return properties;
                }

                public override void SetProperty(string field, object newval)
                {

                }
            }

            public class SDATInfoSTREAM : SDATInfoRecordBase
            {
                public uint m_FileID;
                public byte m_Volume;
                public byte m_PlayerPriority;
                public byte m_PlayerNumber;
                public byte[] m_Reserved; // (5)

                private uint m_Offset;

                public SDATInfoSTREAM(NitroFile sdat, uint offset)
                {
                    m_Offset = offset;

                    m_FileID = sdat.Read32(m_Offset + 0x00);
                    m_Volume = sdat.Read8(m_Offset + 0x04);
                    m_PlayerPriority = sdat.Read8(m_Offset + 0x05);
                    m_PlayerNumber = sdat.Read8(m_Offset + 0x06);
                    m_Reserved = new byte[5];
                    for (int i = 0; i < 5; i++)
                        m_Reserved[i] = sdat.Read8(m_Offset + 0x07 + (uint)(i));
                }

                public override PropertyTable GenerateProperties()
                {
                    PropertyTable properties = new PropertyTable();

                    properties.Properties.Add(
                        new PropertySpec("File ID", typeof(uint), "", "File ID for accessing this file", m_FileID, "", typeof(UInt32Converter)));
                    properties.Properties.Add(
                        new PropertySpec("Volume", typeof(byte), "", "Volume", m_Volume, "", typeof(ByteConverter)));
                    properties.Properties.Add(
                        new PropertySpec("Player Priority", typeof(byte), "", "Player Priority", m_PlayerPriority, "", typeof(ByteConverter)));
                    properties.Properties.Add(
                        new PropertySpec("Player Number", typeof(byte), "", "Player Number", m_PlayerNumber, "", typeof(ByteConverter)));
                    for (int i = 0; i < 5; i++)
                    {
                        properties.Properties.Add(
                            new PropertySpec("Reserved [" + i + "]", typeof(byte), "", "Reserved [" + i + "]", m_Reserved[i], "", typeof(ByteConverter)));
                    }

                    properties["File ID"] = m_FileID;
                    properties["Volume"] = m_Volume;
                    properties["Player Priority"] = m_PlayerPriority;
                    properties["Player Number"] = m_PlayerNumber;
                    for (int i = 0; i < 5; i++)
                        properties["Reserved [" + i + "]"] = m_Reserved[i];

                    return properties;
                }

                public override void SetProperty(string field, object newval)
                {

                }
            }

        }// End SDATInfo
    }// End SDAT
}