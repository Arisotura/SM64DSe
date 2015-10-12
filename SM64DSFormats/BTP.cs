/*
 * BTP files contain data to allow animating textures by replacing them with others in a sequence
 */ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM64DSe.SM64DSFormats
{
    public class BTP
    {
        private NitroFile m_File;
        private ushort m_NumFrames;
        private uint m_NumTextures, m_TextureHeadersOffset;
        private uint m_NumPalettes, m_PaletteHeadersOffset;
        private uint m_FrameChangesOffset;
        private uint m_FrameTextureIDsOffset;
        private uint m_FramePaletteIDsOffset;
        private uint m_NumMaterials;
        private uint m_MaterialHeadersOffset;
        public List<string> m_TextureNames;
        public List<string> m_PaletteNames;
        public List<string> m_MaterialNames;

        public string m_FileName;
        public List<PointerReference> m_PointerList;

        public List<BTPFrameData> m_Frames;
        public Dictionary<string, BTPMaterialData> m_MaterialData;

        public BTP(NitroFile file)
        {
            this.m_File = file;
            this.m_FileName = file.m_Name;

            m_NumFrames = m_File.Read16(0x00);

            m_NumTextures = m_File.Read16(0x02);
            m_TextureHeadersOffset = m_File.Read32(0x04);
            m_TextureNames = new List<string>();

            m_NumPalettes = m_File.Read16(0x08);
            m_PaletteHeadersOffset = m_File.Read32(0x0C);
            m_PaletteNames = new List<string>();

            m_FrameChangesOffset = m_File.Read32(0x10);
            m_FrameTextureIDsOffset = m_File.Read32(0x14);
            m_FramePaletteIDsOffset = m_File.Read32(0x18);

            m_NumMaterials = m_File.Read16(0x1C);
            m_MaterialHeadersOffset = m_File.Read32(0x20);
            m_MaterialNames = new List<string>();

            for (int i = 0; i < m_NumTextures; i++)
            {
                m_TextureNames.Add(m_File.ReadString(m_File.Read32(m_TextureHeadersOffset + 0x04 + (uint)(i * 8)), 0));
            }

            for (int i = 0; i < m_NumPalettes; i++)
            {
                m_PaletteNames.Add(m_File.ReadString(m_File.Read32(m_PaletteHeadersOffset + 0x04 + (uint)(i * 8)), 0));
            }

            // Read in the frames for each material
            m_MaterialData = new Dictionary<string, BTPMaterialData>();
            m_Frames = new List<BTPFrameData>();
            for (int i = 0; i < m_NumMaterials; i++)
            {
                m_MaterialNames.Add(m_File.ReadString(m_File.Read32(m_MaterialHeadersOffset + 0x04 + (uint)(i * 12)), 0));

                ushort matFrameChanges = m_File.Read16(m_MaterialHeadersOffset + 0x08 + (uint)(i * 12));
                ushort startOffsetFrameChanges = m_File.Read16(m_MaterialHeadersOffset + 0x0A + (uint)(i * 12));

                int matNumFrames = 0;

                for (int j = 0; j < matFrameChanges; j++)
                {
                    ushort frameNum = m_File.Read16((uint)(m_FrameChangesOffset + (startOffsetFrameChanges * 2) + (j * 2)));
                    // If not the last frame in the sequence, length = (next frame number - current frame number) else, 
                    // length = (number of frames in sequence - current frame number)
                    int nextFrameNum = (j != (matFrameChanges - 1)) ?
                        (int)m_File.Read16((uint)(m_FrameChangesOffset + (startOffsetFrameChanges * 2) + ((j + 1) * 2))) : -1;
                    int length = (nextFrameNum != -1) ? (nextFrameNum - frameNum) : (m_NumFrames - frameNum);

                    ushort texID = m_File.Read16((uint)(m_FrameTextureIDsOffset + (startOffsetFrameChanges * 2) + (j * 2)));
                    ushort palID = m_File.Read16((uint)(m_FramePaletteIDsOffset + (startOffsetFrameChanges * 2) + (j * 2)));

                    AddFrame(matNumFrames, m_Frames.Count, texID, palID, length);

                    matNumFrames += length;
                }

                BTPMaterialData matData = new BTPMaterialData(m_MaterialNames[i], matFrameChanges, startOffsetFrameChanges, matNumFrames);

                m_MaterialData.Add(m_MaterialNames[i], matData);
            }
        }

        public void AddFrame(int frameNum, int id, uint textureID, uint paletteID, int length, int index = -1)
        {
            BTPFrameData frameData = new BTPFrameData(frameNum, id, textureID, paletteID, length);
            if (index != -1)
            {
                m_Frames.Insert(index, frameData);
            }
            else
            {
                m_Frames.Add(frameData);
            }
            UpdateFrameNumbers();
        }

        public void AddFrame(uint textureID, uint paletteID, int length, int index = -1)
        {
            int frameNum = 0;
            for (int i = 0; i < ((index != -1) ? index : m_Frames.Count); i++)
            {
                frameNum += m_Frames[i].m_Length;
            }
            int id = (index != -1) ? index : m_Frames.Count;

            AddFrame(frameNum, id, textureID, paletteID, length, index);

            UpdateMaterialNumFrames();
        }

        private void UpdateMaterialNumFrames()
        {
            for (int i = 0; i < m_MaterialData.Count; i++)
            {
                m_MaterialData.Values.ElementAt(i).m_NumFrames = 0;
                for (int j = m_MaterialData.Values.ElementAt(i).m_StartOffsetFrameChanges;
                    j < (m_MaterialData.Values.ElementAt(i).m_StartOffsetFrameChanges +
                    m_MaterialData.Values.ElementAt(i).m_NumFrameChanges); j++)
                {
                    if (i < m_MaterialData.Values.Count && j < m_Frames.Count)
                        m_MaterialData.Values.ElementAt(i).m_NumFrames += m_Frames[j].m_Length;
                }
            }
        }

        public void UpdateFrameNumbers()
        {
            foreach (BTPMaterialData matData in m_MaterialData.Values)
            {
                int count = 0;
                for (int i = matData.m_StartOffsetFrameChanges; i < matData.m_StartOffsetFrameChanges + matData.m_NumFrameChanges; i++)
                {
                    if (i >= m_Frames.Count)
                        continue;

                    m_Frames[i].m_FrameNum = count;
                    count += m_Frames[i].m_Length;
                }
            }
            for (int i = 0; i < m_Frames.Count; i++)
            {
                m_Frames[i].m_FrameChangeID = i;
            }
        }

        public void AddTexture(string texName)
        {
            m_TextureNames.Add(texName);
            m_NumTextures++;
        }

        public void AddPalette(string palName)
        {
            m_PaletteNames.Add(palName);
            m_NumPalettes++;
        }

        public void AddMaterial(string matName, ushort numFrameChanges, ushort startOffsetFrameChanges)
        {
            m_MaterialNames.Add(matName);
            m_NumMaterials++;
            int numFrames = 0;
            for (int i = startOffsetFrameChanges; i < (startOffsetFrameChanges + numFrameChanges); i++)
            {
                numFrames += m_Frames[i].m_Length;
            }
            m_MaterialData.Add(matName, new BTPMaterialData(matName, numFrameChanges, startOffsetFrameChanges, numFrames));
            UpdateFrameNumbers();
            UpdateMaterialNumFrames();
        }

        public void RemoveFrame(int index)
        {
            if (index != -1)
            {
                m_Frames.RemoveAt(index);
                UpdateFrameNumbers();
            }
        }

        public void RemoveTexture(string texName)
        {
            int index = m_TextureNames.IndexOf(texName);
            if (index != -1)
                m_TextureNames.RemoveAt(index);
        }

        public void RemovePalette(string palName)
        {
            int index = m_PaletteNames.IndexOf(palName);
            if (index != -1)
                m_PaletteNames.RemoveAt(index);
        }

        public void RemoveMaterial(string matName)
        {
            int index = m_MaterialNames.IndexOf(matName);
            if (index != -1)
            {
                m_MaterialNames.RemoveAt(index);
                m_MaterialData.Remove(matName);
            }
        }

        public void SetMaterialStartOffsetFrameChanges(string matName, ushort startOffsetFrameChanges)
        {
            m_MaterialData[matName].m_StartOffsetFrameChanges = startOffsetFrameChanges;
            UpdateFrameNumbers();
            UpdateMaterialNumFrames();
        }

        public void SetMaterialNumFrameChanges(string matName, ushort numFrameChanges)
        {
            m_MaterialData[matName].m_NumFrameChanges = numFrameChanges;
            UpdateFrameNumbers();
            UpdateMaterialNumFrames();
        }

        public int NumFrames()
        {
            return m_Frames.Count;
        }

        public void SaveChanges()
        {
            // Builds up a new BTP file based on data, easier than managing it as it's edited as the file 
            // format is so simple

            m_File.Clear();

            uint dataOffset = 0;

            int numFrames = -1;
            foreach (BTPMaterialData matData in m_MaterialData.Values)
            {
                int matNumFrames = 0;
                for (int i = matData.m_StartOffsetFrameChanges;
                    i < matData.m_StartOffsetFrameChanges + matData.m_NumFrameChanges; i++)
                {
                    matNumFrames += m_Frames[i].m_Length;
                }
                if (matNumFrames > numFrames)
                    numFrames = matNumFrames;
            }

            m_File.Write16(0x00, (ushort)numFrames);// 0x00 2 Number of frames
            m_File.Write16(0x02, (ushort)m_TextureNames.Count);// 0x02 2 Number of textures
            // 0x04  4 address of texture names headers
            m_File.Write16(0x08, (ushort)m_PaletteNames.Count);// 0x08 2 Number of palettes
            m_File.Write16(0x0A, 0x00000);// 0x0A 2 Zero padding
            // 0x0C 4 Address of palette names headers
            // 0x10 4 Address of "Frame Changes" section
            // 0x14 4 Address of texture ID's section
            // 0x18 4 Address of palette ID's section
            m_File.Write16(0x1C, (ushort)m_MaterialData.Values.Count);// 0x1C 2 Number of materials
            m_File.Write16(0x1E, 0x0000);// 0x1E 2 Zero padding
            // 0x20 4 Address of material headers

            // Write texture headers
            dataOffset = 0x24;
            m_File.Write32(0x04, dataOffset);
            uint texNameAddr = dataOffset + (uint)(8 * m_TextureNames.Count);
            for (int i = 0; i < m_TextureNames.Count; i++)
            {
                m_File.Write16(dataOffset + 0x00, 0xFFFF);
                m_File.Write16(dataOffset + 0x02, 0);
                m_File.Write32(dataOffset + 0x04, texNameAddr);

                texNameAddr += (uint)(((m_TextureNames[i].Length + 1) + 3) & ~3);

                dataOffset += 8;
            }
            // Write texture names
            for (int i = 0; i < m_TextureNames.Count; i++)
            {
                m_File.WriteString(dataOffset, m_TextureNames[i], 0);
                dataOffset += (uint)(((m_TextureNames[i].Length + 1) + 3) & ~3);
            }

            // Write palette headers
            m_File.Write32(0x0C, dataOffset);
            uint palNameAddr = dataOffset + (uint)(8 * m_PaletteNames.Count);
            for (int i = 0; i < m_PaletteNames.Count; i++)
            {
                m_File.Write16(dataOffset + 0x00, 0xFFFF);
                m_File.Write16(dataOffset + 0x02, 0x0000);
                m_File.Write32(dataOffset + 0x04, palNameAddr);

                palNameAddr += (uint)(((m_PaletteNames[i].Length + 1) + 3) & ~3);

                dataOffset += 8;
            }
            // Write palette names
            for (int i = 0; i < m_PaletteNames.Count; i++)
            {
                m_File.WriteString(dataOffset, m_PaletteNames[i], 0);
                dataOffset += (uint)(((m_PaletteNames[i].Length + 1) + 3) & ~3);
            }

            // Write frame changes
            m_File.Write32(0x10, dataOffset);
            foreach (BTPMaterialData matData in m_MaterialData.Values)
            {
                ushort count = 0;
                for (int i = matData.m_StartOffsetFrameChanges;
                    i < matData.m_StartOffsetFrameChanges + matData.m_NumFrameChanges; i++)
                {
                    m_File.Write16(dataOffset, count);
                    count += (ushort)m_Frames[i].m_Length;
                    dataOffset += 0x02;
                }
            }
            dataOffset = (uint)((dataOffset + 3) & ~3);

            // Write texture ID's for frame changes
            m_File.Write32(0x14, dataOffset);
            for (int i = 0; i < m_Frames.Count; i++)
            {
                m_File.Write16(dataOffset, (ushort)m_Frames[i].m_TextureID);
                dataOffset += 0x02;
            }
            dataOffset = (uint)((dataOffset + 3) & ~3);

            // Write palette ID's for frame changes
            m_File.Write32(0x18, dataOffset);
            for (int i = 0; i < m_Frames.Count; i++)
            {
                m_File.Write16(dataOffset, (ushort)m_Frames[i].m_PaletteID);
                dataOffset += 0x02;
            }
            dataOffset = (uint)((dataOffset + 3) & ~3);

            // Write material headers
            m_File.Write32(0x20, dataOffset);
            uint matNameAddr = dataOffset + (uint)(12 * m_MaterialData.Values.Count);
            for (int i = 0; i < m_MaterialData.Values.Count; i++)
            {
                m_File.Write16(dataOffset + 0x00, 0xFFFF);
                m_File.Write16(dataOffset + 0x02, 0x0000);
                m_File.Write32(dataOffset + 0x04, matNameAddr);
                m_File.Write16(dataOffset + 0x08, m_MaterialData.Values.ElementAt(i).m_NumFrameChanges);
                m_File.Write16(dataOffset + 0x0A, m_MaterialData.Values.ElementAt(i).m_StartOffsetFrameChanges);

                matNameAddr += (uint)(((m_MaterialData.Values.ElementAt(i).m_Name.Length + 1) + 3) & ~3);

                dataOffset += 12;
            }
            // Write material names
            for (int i = 0; i < m_MaterialData.Values.Count; i++)
            {
                m_File.WriteString(dataOffset, m_MaterialData.Values.ElementAt(i).m_Name, 0);
                dataOffset += (uint)(((m_MaterialData.Values.ElementAt(i).m_Name.Length + 1) + 3) & ~3);
            }

            m_File.SaveChanges();
        }

        public class BTPMaterialData
        {
            public string m_Name;
            public ushort m_NumFrameChanges;
            public ushort m_StartOffsetFrameChanges;
            public int m_NumFrames;

            public BTPMaterialData(string name, ushort numFrameChanges, ushort startOffsetFrameChanges, int numFrames)
            {
                m_Name = name;
                m_NumFrameChanges = numFrameChanges;
                m_StartOffsetFrameChanges = startOffsetFrameChanges;
                m_NumFrames = numFrames;
            }
        }

        public class BTPFrameData
        {
            public int m_FrameNum;// The actual frame number for the current material, takes previous lengths into account
            public int m_FrameChangeID;// The position of this frame change within global list
            public uint m_TextureID;// Texture ID
            public uint m_PaletteID;// Palette ID
            public int m_Length;// The number of frames this frame change lasts

            public BTPFrameData(int frameNum, int id, uint textureID, uint paletteID, int length)
            {
                m_FrameNum = frameNum;
                m_FrameChangeID = id;
                m_TextureID = textureID;
                m_PaletteID = paletteID;
                m_Length = length;
            }
        }
    }
}
