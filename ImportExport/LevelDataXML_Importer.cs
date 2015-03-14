using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Forms;
using System.Xml;
using OpenTK;
using System.IO;

namespace SM64DSe.ImportExport
{
    public static class LevelDataXML_Importer
    {
        private static NitroOverlay m_Overlay;
        private static int m_LevelID;
        private static int m_NumAreas;
        private static string m_FileName;
        private static string m_Path;

        private static List<INitroROMBlock> m_FilesToSave;

        private static ushort m_OldBMDFileID;
        private static ushort m_OldKCLFileID;
        private static ushort m_OldNCGFileID;
        private static ushort m_OldNCLFileID;
        private static ushort[] m_OldMinimapTileIDs;

        private static uint m_DataOffset = 0;
        private static List<LevelObject>[] m_ObjectsToImport;
        private static TextureAnimationArea[] m_TextureAnimationAreasToImport;

        private static int[] OBJECT_SIZES = { 16, 16, 6, 6, 14, 8, 8, 8, 8, 12, 14, 2, 2, 0, 4 };
        private static int[] MISC_TYPES = { 1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 14 };
        private static int[] STANDARD_SIMPLE_TYPES = { 0, 5 };
        // The below is needed because unless objects are written with entrance objects last, the view 
        // will not be correct upon entering the level and cannot be fixed. Split by standard and simple objects 
        // and miscelaneous objects
        private static int[] OBJECT_WRITE_TYPE_ORDER = { 0, 5, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 1 };

        private static int pathCount = 0;
        private static int tileCount = 0;

        private static CultureInfo usa = new CultureInfo("en-US");

        public static int ImportLevelDataFromXML(string filename, NitroOverlay overlay, int levelID, 
            ushort[] minimapTileIDs, bool saveChanges = true)
        {
            m_FileName = filename;
            m_Path = Path.GetDirectoryName(m_FileName);

            m_Overlay = overlay;
            m_LevelID = levelID;

            m_FilesToSave = new List<INitroROMBlock>();
            m_FilesToSave.Add(m_Overlay);

            m_OldBMDFileID = m_Overlay.Read16(0x68);// BMD model
            m_OldKCLFileID = m_Overlay.Read16(0x6A);// KCL collision map
            m_OldNCGFileID = m_Overlay.Read16(0x6C);// minimap tileset
            m_OldNCLFileID = m_Overlay.Read16(0x6E);// minimap palette
            m_OldMinimapTileIDs = minimapTileIDs;

            m_DataOffset = 0;
            pathCount = 0;
            tileCount = 0;

            try
            {
                ClearOverlayAndAddLoaderCode();
                ImportXML();

                if (saveChanges)
                    SaveChangesToAllFiles();
            }
            catch (Exception e) { MessageBox.Show(e.Message + "\n\n" + e.StackTrace); return -1; }

            return 0;
        }

        private static void ClearOverlayAndAddLoaderCode()
        {
            byte[] loadercode = null;

            m_Overlay.Clear();

            switch (Program.m_ROM.m_Version)
            {
                case NitroROM.Version.EUR:
                    loadercode = Properties.Resources.level_ovl_init_EUR;
                    break;
                case NitroROM.Version.JAP:
                    loadercode = Properties.Resources.level_ovl_init_JAP;
                    break;
                case NitroROM.Version.USA_v1:
                    loadercode = Properties.Resources.level_ovl_init_USAv1;
                    break;
                case NitroROM.Version.USA_v2:
                    loadercode = Properties.Resources.level_ovl_init_USAv2;
                    break;
            }
            m_Overlay.WriteBlock(0, loadercode);

            m_DataOffset = 0x54;
        }

        public static void SaveChangesToAllFiles()
        {
            foreach (INitroROMBlock file in m_FilesToSave)
            {
                file.SaveChanges();
            }
        }

        private static void ImportXML()
        {
            using (XmlReader reader = XmlReader.Create(m_FileName, new XmlReaderSettings() { CloseInput = true }))
            {
                reader.MoveToContent();

                while (reader.Read())
                {
                    if (reader.NodeType.Equals(XmlNodeType.Element))
                    {
                        switch (reader.LocalName)
                        {
                            case "LevelSettings":
                                ReadLevelSettingsFromXML(reader);
                                break;
                            case "CLPS":
                                ReadCLPSDataFromXML(reader);
                                break;
                            case "LevelData":
                                ReadLevelDataFromXML(reader);
                                break;
                        }
                    }
                    else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("Level"))
                    {
                        break;
                    }
                }

                reader.Close();
            }

            ProcessLevelData();

        }

        private static void ReadLevelSettingsFromXML(XmlReader reader)
        {
            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element))
                {
                    if (reader.LocalName.Equals("ObjectBankSettings"))
                    {
                        ReadObjectBankSettings(reader.ReadSubtree());
                    }
                    else if (reader.LocalName.Equals("LevelModel"))
                    {
                        ReadFileFromXML(reader, "LevelModel", m_OldBMDFileID);
                    }
                    else if (reader.LocalName.Equals("CollisionMap"))
                    {
                        ReadFileFromXML(reader, "CollisionMap", m_OldKCLFileID);
                    }
                    else if (reader.LocalName.Equals("MinimapSettings"))
                    {
                        ReadMinimapSettings(reader.ReadSubtree());
                    }
                    else if (reader.LocalName.Equals("NumberOfAreas"))
                    {
                        byte numAreas = byte.Parse(reader.ReadElementContentAsString());
                        m_Overlay.Write8(0x74, numAreas);
                        m_NumAreas = numAreas;
                    }
                    else if (reader.LocalName.Equals("CameraStartZoomedOut"))
                    {
                        m_Overlay.Write8(0x75, byte.Parse(reader.ReadElementContentAsString()));
                    }
                    else if (reader.LocalName.Equals("Background"))
                    {
                        m_Overlay.Write8(0x78, (byte)(0xF | (byte.Parse(reader.ReadElementContentAsString()) << 4)));
                    }
                    else if (reader.LocalName.Equals("Music"))
                    {
                        ReadMusicBytes(reader.ReadSubtree());
                    }
                    else if (reader.LocalName.Equals("ActSelectorID"))
                    {
                        byte actSelectorID = byte.Parse(reader.ReadElementContentAsString());
                        Program.m_ROM.BeginRW();
                        Program.m_ROM.Write8((uint)(Helper.GetActSelectorIDTableAddress() + m_LevelID), actSelectorID);
                        Program.m_ROM.EndRW();
                    }

                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement))
                {
                    if (reader.LocalName.Equals("LevelSettings"))
                    {
                        m_Overlay.Write16(0x68, m_OldBMDFileID);
                        m_Overlay.Write16(0x6A, m_OldKCLFileID);
                        m_Overlay.Write16(0x6C, m_OldNCGFileID);
                        m_Overlay.Write16(0x6E, m_OldNCLFileID);
                        break;
                    }
                }

            }
            m_DataOffset = 0x94;

            return;
        }

        private static void ReadObjectBankSettings(XmlReader reader)
        {
            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Bank"))
                {
                    int bankID = int.Parse(reader.GetAttribute("id"));
                    uint bankSetting = uint.Parse(reader.ReadElementContentAsString());

                    if (bankID < 7)
                        m_Overlay.Write8(0x54 + (uint)bankID, (byte)bankSetting);
                    else if (bankID == 7)
                        m_Overlay.Write32(0x5C, bankSetting);

                    m_Overlay.Write8(0x5B, 0);// Padding
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("ObjectBankSettings"))
                {
                    return;
                }
            }

            return;
        }

        // File specified in <Location> will overwrite file with specified file ID
        private static void ReadFileFromXML(XmlReader reader, string element, ushort fileID)
        {
            if (reader.IsEmptyElement)
                return;

            string location = null;

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element))
                {
                    if (reader.LocalName.Equals("Location"))
                    {
                        location = reader.ReadElementContentAsString();
                    }
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement))
                {
                    if (reader.LocalName.Equals(element))
                        break;
                }
            }

            NitroFile currentFile = Program.m_ROM.GetFileFromInternalID(fileID);
            if (location != null)
            {
                byte[] fileToImport = System.IO.File.ReadAllBytes(m_Path + "/" + location);
                currentFile.m_Data = fileToImport;
                m_FilesToSave.Add(currentFile);
            }
        }

        private static void ReadMinimapSettings(XmlReader reader)
        {
            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Tileset"))
                {
                    ReadFileFromXML(reader, "Tileset", m_OldNCGFileID);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Palette"))
                {
                    ReadFileFromXML(reader, "Palette", m_OldNCLFileID);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("CoordinateScale"))
                {
                    m_Overlay.Write16(0x76, ushort.Parse(reader.ReadElementContentAsString()));
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("MinimapSettings"))
                {
                    return;
                }
            }

            return;
        }

        private static void ReadMusicBytes(XmlReader reader)
        {
            int byteID = 0;

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Byte"))
                {
                    byte value = byte.Parse(reader.ReadElementContentAsString());

                    if (byteID < 3)
                        m_Overlay.Write8(0x7C + (uint)byteID, value);

                    byteID++;
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("Music"))
                {
                    return;
                }
            }

            return;
        }

        private static void ReadCLPSDataFromXML(XmlReader reader)
        {
            uint CLPS_header = m_DataOffset;
            m_Overlay.WritePointer(0x60, CLPS_header);
            m_DataOffset += 0x08;
            int entryCount = 0;

            byte[] clpsEntry = new byte[8];
            int valueCount = 0;

            while (reader.Read())
            {
                reader.MoveToContent();

                if (reader.NodeType.Equals(XmlNodeType.Element))
                {
                    switch (reader.LocalName)
                    {
                        case "Value":
                            clpsEntry = new byte[8];
                            valueCount = 0;
                            break;
                        case "Byte":
                            if (valueCount >= 8)
                                break;
                            clpsEntry[valueCount++] = Byte.Parse(reader.ReadElementContentAsString());
                            break;
                    }
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement))
                {
                    if (reader.LocalName.Equals("Entry"))
                    {
                        m_Overlay.WriteBlock(m_DataOffset, clpsEntry);
                        m_DataOffset += 8;
                        entryCount++;
                    }
                    else if (reader.LocalName.Equals("CLPS"))
                    {
                        break;
                    }
                }
            }

            m_Overlay.WriteString(CLPS_header, "CLPS", 4);
            m_Overlay.Write8(CLPS_header + 0x04, 0x08);
            m_Overlay.Write8(CLPS_header + 0x05, 0);
            m_Overlay.Write16(CLPS_header + 0x06, (ushort)entryCount);
        }

        private static void ReadLevelDataFromXML(XmlReader reader)
        {
            if (m_NumAreas == 0)
                m_NumAreas = 1;
            m_ObjectsToImport = new List<LevelObject>[m_NumAreas];
            m_TextureAnimationAreasToImport = new TextureAnimationArea[m_NumAreas];

            for (int i = 0; i < m_NumAreas; i++)
            {
                m_ObjectsToImport[i] = new List<LevelObject>();
            }

            // First read in all the objects and texture animations, then worry about writing them
            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element))
                {
                    if (reader.LocalName.Equals("Objects"))
                    {
                        ReadObjects(reader);
                    }
                    if (reader.LocalName.Equals("TextureAnimationData"))
                    {
                        ReadTextureAnimationData(reader);
                    }
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement))
                {
                    if (reader.LocalName.Equals("LevelData"))
                    {
                        break;
                    }
                }
            }
            // End reading LevelData
        }

        private static void ProcessLevelData()
        {
            /* From the documentation:
             * ---===[ LEVEL AREA DATA ]===---
                Each entry is 12 bytes. Number determined by header[0x74].
                
                Offset Size Desc
                00 4 Address of the objects table (NULL = no table)
                04 4 Address of the texture animation data (NULL = no data)
                08 1 Minimap tilemap index
                09 3 ???

                The objects table uses the same format as the one header[0x64] points to.
                Except it typically only contains objects of type 0 and 5. The other object
                types are contained by the header[0x64] table.
                
                ---===[ OBJECT TABLES ]===---
                First part: 8 bytes
                
                Offset Size Desc
                00 4 Number of entries in table
                04 4 Address of the object table
                
                Object table: 8 bytes per entry
                
                Offset Size Desc
                00 1 Bit0-4: object type; Bit5-7: layer (0=all stars, 1-7=star 1..7)
                01 1 Number of objects
                02 2 Zero (?)
                04 4 Address of the object list
             */

            // Split the objects by area, then star, then type
            AreaObjectList[] areaObjectList = SplitObjectsByAreaStarType();

            // 'Misc' objects table
            // All objects are in area 0
            uint misc_objects_table = m_DataOffset;
            m_Overlay.WritePointer(0x64, m_DataOffset);

            for (int i = 0; i < 1; i++)
            {
                int num_entries = GetNumObjectListsInArea(areaObjectList[i], MISC_TYPES);

                m_Overlay.Write32(misc_objects_table, (uint)num_entries);

                uint object_tables_entries = misc_objects_table + 0x08;// adddress of the list of object tables
                m_Overlay.WritePointer(misc_objects_table + 0x04, misc_objects_table + 0x08);
                m_DataOffset += 8;

                WriteObjectTableHeadersAndDataForArea(areaObjectList[i], object_tables_entries, num_entries, MISC_TYPES);
            }
            // End 'Misc' objects table

            m_DataOffset = (uint)((m_DataOffset + 3) & ~3);
 
            // The object tables in the level area data section only contain objects of type 0 (Standard) or 
            // type 5 (Simple). The rest will get written into the 'misc' objects table afterwards
            uint level_area_data = m_DataOffset;
            m_Overlay.WritePointer(0x70, m_DataOffset);
            uint object_tables = level_area_data + (uint)(m_NumAreas * 12);
            m_DataOffset = object_tables;

            for (int i = 0; i < m_NumAreas; i++)
            {
                uint level_area_data_header = level_area_data + (uint)(i * 12);

                int num_entries = GetNumObjectListsInArea(areaObjectList[i], STANDARD_SIMPLE_TYPES);

                uint object_tables_header = m_DataOffset;

                if (num_entries > 0)
                {
                    m_Overlay.WritePointer(level_area_data_header, object_tables_header); // Address of object tables header
                    m_Overlay.Write32(level_area_data_header + 0x04, 0x00000000); // Texture animation, will be overwritten later
                    m_Overlay.Write16(level_area_data_header + 0x08, (ushort)(i << 4)); // Minimap tilemap index (1 byte) + 1 byte unknown
                    m_Overlay.Write16(level_area_data_header + 0x0A, 0x0000); // last 2 bytes of unkown 3 bytes

                    m_Overlay.Write32(object_tables_header, (uint)num_entries);

                    uint object_tables_entries = object_tables_header + 0x08;// adddress of the list of object tables
                    m_Overlay.WritePointer(object_tables_header + 0x04, object_tables_header + 0x08);
                    m_DataOffset += 8;

                    WriteObjectTableHeadersAndDataForArea(areaObjectList[i], object_tables_entries, num_entries, STANDARD_SIMPLE_TYPES);
                }
                else
                {
                    m_Overlay.Write32(level_area_data_header + 0x00, 0x00000000); // Address null if no object tables
                    m_Overlay.Write32(level_area_data_header + 0x04, 0x00000000); // Texture animation, will be overwritten later
                    m_Overlay.Write16(level_area_data_header + 0x08, (ushort)(i << 4)); // Minimap tilemap index (1 byte) + 1 byte unknown
                    m_Overlay.Write16(level_area_data_header + 0x0A, 0x0000); // last 2 bytes of unkown 3 bytes
                }
            }
            // End level area object tables

            m_DataOffset = (uint)((m_DataOffset + 3) & ~3);

            // Now write the texture animations
            WriteTextureAnimations(level_area_data);

            // Done at last
        }

        private static AreaObjectList[] SplitObjectsByAreaStarType()
        {
            AreaObjectList[] areaObjectList = new AreaObjectList[m_NumAreas];

            for (int i = 0; i < m_NumAreas; i++)
            {
                areaObjectList[i] = new AreaObjectList(i);

                for (int j = 0; j <= 7; j++)
                {
                    areaObjectList[i].m_StarObjectList[j] = new StarObjectList(i, j);

                    for (int k = 0; k < 15; k++)
                    {
                        // (No type 13 objects)
                        if (k == 13)
                            continue;

                        areaObjectList[i].m_StarObjectList[j].m_TypeObjectList[k] = new TypeObjectList(i, j, k);

                        List<LevelObject> matchingObjects = m_ObjectsToImport[i].Where(obj0 => (obj0.m_Area == i) &&
                            (obj0.m_Layer == j) && (obj0.m_Type == k)).ToList<LevelObject>();

                        int numObjects = matchingObjects.Count;
                        if (numObjects == 0)
                            continue;
                        int listsNeeded = (numObjects / 255) + 1;

                        for (int m = 0; m < listsNeeded; m++)
                        {
                            List<LevelObject> tmp = new List<LevelObject>();
                            for (int n = 0; n < ((numObjects < 256) ? numObjects : 256); n++)
                            {
                                tmp.Add(matchingObjects.ElementAt((m * 256) + n));
                            }
                            areaObjectList[i].m_StarObjectList[j].m_TypeObjectList[k].m_Objects.Add(tmp);
                        }
                    }
                }
            }

            return areaObjectList;
        }

        private static int GetNumObjectListsInArea(AreaObjectList areaObjectList, int[] objectTypes)
        {
            int num_entries = 0;

            for (int j = 0; j < areaObjectList.m_StarObjectList.Length; j++)
            {
                for (int k = 0; k < areaObjectList.m_StarObjectList[j].m_TypeObjectList.Length; k++)
                {
                    if (!objectTypes.Contains(k))
                        continue;

                    num_entries += areaObjectList.m_StarObjectList[j].m_TypeObjectList[k].m_Objects.Count;
                }
            }

            return num_entries;
        }

        private static void WriteObjectTableHeadersAndDataForArea(AreaObjectList areaObjectList, uint object_tables_entries, 
            int num_entries, int[] objectTypes)
        {
            // Write the headers for the lists of object tables but don't write the actual objects yet
            uint object_tables_objects = object_tables_entries + (uint)(num_entries * 8);// address of the first list of objects
            uint next_object_tables_objects = object_tables_objects;
            
            for (int j = 0; j < areaObjectList.m_StarObjectList.Length; j++)
            {
                foreach (int k in OBJECT_WRITE_TYPE_ORDER)
                {
                    if (!objectTypes.Contains(k))
                        continue;

                    for (int m = 0; m < areaObjectList.m_StarObjectList[j].m_TypeObjectList[k].m_Objects.Count; m++)
                    {
                        int object_type = areaObjectList.m_StarObjectList[j].m_TypeObjectList[k].m_Type;
                        int star = areaObjectList.m_StarObjectList[j].m_TypeObjectList[k].m_Star;
                        byte type_and_star = (byte)((star << 5) | object_type);
                        int size = OBJECT_SIZES[object_type];

                        int num_objects = areaObjectList.m_StarObjectList[j].m_TypeObjectList[k].m_Objects.ElementAt(m).Count;

                        m_Overlay.Write8(m_DataOffset + 0x00, type_and_star);
                        m_Overlay.Write8(m_DataOffset + 0x01,
                            (byte)num_objects);
                        m_Overlay.Write16(m_DataOffset + 0x02, 0);
                        m_Overlay.WritePointer(m_DataOffset + 0x04, next_object_tables_objects);

                        uint size_aligned_four = (uint)(((num_objects * size) + 3) & ~3);
                        next_object_tables_objects += size_aligned_four;

                        m_DataOffset += 8;
                    }

                }
            }

            // Now write the actual objects
            for (int j = 0; j < areaObjectList.m_StarObjectList.Length; j++)
            {
                foreach (int k in OBJECT_WRITE_TYPE_ORDER)
                {
                    if (!objectTypes.Contains(k))
                        continue;

                    for (int m = 0; m < areaObjectList.m_StarObjectList[j].m_TypeObjectList[k].m_Objects.Count; m++)
                    {
                        int object_type = areaObjectList.m_StarObjectList[j].m_TypeObjectList[k].m_Type;
                        int star = areaObjectList.m_StarObjectList[j].m_TypeObjectList[k].m_Star;
                        int size = OBJECT_SIZES[object_type];

                        for (int n = 0; n < areaObjectList.m_StarObjectList[j].m_TypeObjectList[k].m_Objects.ElementAt(m).Count; n++)
                        {
                            LevelObject obj = areaObjectList.m_StarObjectList[j].m_TypeObjectList[k].m_Objects.ElementAt(m).ElementAt(n);
                            obj.m_Offset = m_DataOffset;

                            obj.SaveChanges();

                            m_DataOffset += (uint)size;
                        }
                    }

                    m_DataOffset = (uint)((m_DataOffset + 3) & ~3);
                }
            }
        }

        class AreaObjectList
        {
            public int m_Area;
            public StarObjectList[] m_StarObjectList;

            public AreaObjectList(int area)
            {
                m_Area = area;
                m_StarObjectList = new StarObjectList[8];
            }
        }

        class StarObjectList
        {
            public int m_Area;
            public int m_Star;
            public TypeObjectList[] m_TypeObjectList;

            public StarObjectList(int area, int star)
            {
                m_Area = area;
                m_Star = star;
                m_TypeObjectList = new TypeObjectList[15];
            }
        }

        class TypeObjectList
        {
            public int m_Area;
            public int m_Star;
            public int m_Type;
            public List<List<LevelObject>> m_Objects;

            public TypeObjectList(int area, int star, int type)
            {
                m_Area = area;
                m_Star = star;
                m_Type = type;
                m_Objects = new List<List<LevelObject>>();
            }
        }

        private static void WriteTextureAnimations(uint level_area_data)
        {
            uint texture_animation = m_DataOffset;
            for (int i = 0; i < m_NumAreas; i++)
            {
                uint level_area_data_header = level_area_data + (uint)(i * 12);

                if (m_TextureAnimationAreasToImport[i] == null)
                    continue;

                uint texture_animation_header = m_DataOffset;
                m_Overlay.WritePointer(level_area_data_header + 0x04, texture_animation_header);

                m_Overlay.Write32(texture_animation_header, m_TextureAnimationAreasToImport[i].m_NumFrames);
                m_Overlay.Write32(texture_animation_header + 0x10,
                    (uint)m_TextureAnimationAreasToImport[i].m_TextureAnimations.Count);

                uint animation_data_header = texture_animation_header + 0x18;
                m_Overlay.WritePointer(texture_animation_header + 0x14, animation_data_header);
                m_DataOffset = animation_data_header;

                int num_entries = m_TextureAnimationAreasToImport[i].m_TextureAnimations.Count;

                uint material_names_address = (uint)(m_DataOffset + (28 * num_entries));
                uint next_material_name_address = material_names_address;

                // Write animation data headers
                for (int j = 0; j < m_TextureAnimationAreasToImport[i].m_TextureAnimations.Count; j++)
                {
                    m_Overlay.Write32(m_DataOffset + 0x00, 0);
                    m_Overlay.WritePointer(m_DataOffset + 0x04, next_material_name_address);
                    m_Overlay.Write32(m_DataOffset + 0x08, m_TextureAnimationAreasToImport[i].m_TextureAnimations[j].m_DefaultScale);
                    m_Overlay.Write16(m_DataOffset + 0x0C, (ushort)m_TextureAnimationAreasToImport[i].m_TextureAnimations[j].m_ScaleLength);
                    m_Overlay.Write16(m_DataOffset + 0x0E, (ushort)m_TextureAnimationAreasToImport[i].m_TextureAnimations[j].m_ScaleStart);
                    m_Overlay.Write16(m_DataOffset + 0x10, (ushort)m_TextureAnimationAreasToImport[i].m_TextureAnimations[j].m_RotLength);
                    m_Overlay.Write16(m_DataOffset + 0x12, (ushort)m_TextureAnimationAreasToImport[i].m_TextureAnimations[j].m_RotStart);
                    m_Overlay.Write16(m_DataOffset + 0x14, (ushort)m_TextureAnimationAreasToImport[i].m_TextureAnimations[j].m_TransLength);
                    m_Overlay.Write16(m_DataOffset + 0x16, (ushort)m_TextureAnimationAreasToImport[i].m_TextureAnimations[j].m_TransStart);
                    m_Overlay.Write32(m_DataOffset + 0x18, 0);

                    string mat_name = m_TextureAnimationAreasToImport[i].m_TextureAnimations[j].m_MaterialName;
                    m_Overlay.WriteString(next_material_name_address, mat_name, mat_name.Length);

                    next_material_name_address += (uint)((mat_name.Length + 3) & ~3);

                    m_DataOffset += 0x1C;
                }

                m_DataOffset = next_material_name_address;

                // Write scale, rotation and translation tables
                m_Overlay.WritePointer(texture_animation_header + 0x04, m_DataOffset);
                for (int j = 0; j < m_TextureAnimationAreasToImport[i].m_ScaleValues.Count; j++)
                {
                    m_Overlay.Write32(m_DataOffset, m_TextureAnimationAreasToImport[i].m_ScaleValues[j]);

                    m_DataOffset += 0x4;
                }
                m_DataOffset = (uint)((m_DataOffset + 3) & ~3);

                m_Overlay.WritePointer(texture_animation_header + 0x08, m_DataOffset);
                for (int j = 0; j < m_TextureAnimationAreasToImport[i].m_RotationValues.Count; j++)
                {
                    m_Overlay.Write16(m_DataOffset, m_TextureAnimationAreasToImport[i].m_RotationValues[j]);

                    m_DataOffset += 0x2;
                }
                m_DataOffset = (uint)((m_DataOffset + 3) & ~3);

                m_Overlay.WritePointer(texture_animation_header + 0x0C, m_DataOffset);
                for (int j = 0; j < m_TextureAnimationAreasToImport[i].m_TranslationValues.Count; j++)
                {
                    m_Overlay.Write32(m_DataOffset, m_TextureAnimationAreasToImport[i].m_TranslationValues[j]);

                    m_DataOffset += 0x4;
                }
                m_DataOffset = (uint)((m_DataOffset + 3) & ~3);
            }
        }

        private static void ReadObjects(XmlReader reader)
        {
            while (reader.Read())
            {
                reader.MoveToContent();

                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("StandardObject"))
                {
                    ReadStandardObject(reader);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("EntranceObject"))
                {
                    ReadEntranceObject(reader);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("PathObject"))
                {
                    ReadPathObject(reader);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("ViewObject"))
                {
                    ReadViewObject(reader);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("SimpleObject"))
                {
                    ReadSimpleObject(reader);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("TpSrcObject"))
                {
                    ReadTpSrcObject(reader);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("TpDstObject"))
                {
                    ReadTpDstObject(reader);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("FogObject"))
                {
                    ReadFogObject(reader);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("DoorObject"))
                {
                    ReadDoorObject(reader);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("ExitObject"))
                {
                    ReadExitObject(reader);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("MinimapTileIDObject"))
                {
                    ReadMinimapTileIDObject(reader);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("MinimapScaleObject"))
                {
                    ReadMinimapScaleObject(reader);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Type14Object"))
                {
                    ReadType14Object(reader);
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("Objects"))
                {
                    return;
                }
            }

            return;
        }

        private static void ReadTextureAnimationData(XmlReader reader)
        {
            while (reader.Read())
            {
                reader.MoveToContent();

                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("TextureAnimationArea"))
                {
                    ReadTextureAnimationArea(reader);
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("TextureAnimationData"))
                {
                    return;
                }
            }

            return;
        }

        private static void ReadTextureAnimationArea(XmlReader reader)
        {
            int area = int.Parse(reader.GetAttribute("area"));

            TextureAnimationArea texAnimArea = new TextureAnimationArea(area);

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("NumberOfFrames"))
                {
                    texAnimArea.m_NumFrames = uint.Parse(reader.ReadElementContentAsString());
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("ScaleTable"))
                {
                    texAnimArea.m_ScaleValues = ReadTextureAnimationUIntTable(reader, "ScaleTable");
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("RotationTable"))
                {
                    texAnimArea.m_RotationValues = ReadTextureAnimationUShortTable(reader, "RotationTable");
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("TranslationTable"))
                {
                    texAnimArea.m_TranslationValues = ReadTextureAnimationUIntTable(reader, "TranslationTable");
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("TextureAnimation"))
                {
                    texAnimArea.m_TextureAnimations.Add(ReadTextureAnimation(reader));
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("TextureAnimationArea"))
                {
                    break;
                }
            }

            m_TextureAnimationAreasToImport[area] = texAnimArea;
        }

        private static List<uint> ReadTextureAnimationUIntTable(XmlReader reader, string element)
        {
            List<uint> values = new List<uint>();

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("UInt"))
                {
                    values.Add(uint.Parse(reader.ReadElementContentAsString()));
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals(element))
                {
                    return values;
                }
            }

            return values;
        }

        private static List<ushort> ReadTextureAnimationUShortTable(XmlReader reader, string element)
        {
            List<ushort> values = new List<ushort>();

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("UShort"))
                {
                    values.Add(ushort.Parse(reader.ReadElementContentAsString()));
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals(element))
                {
                    return values;
                }
            }

            return values;
        }

        private static TextureAnimationForImport ReadTextureAnimation(XmlReader reader)
        {
            TextureAnimationForImport texAnim = new TextureAnimationForImport();

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("MaterialName"))
                {
                    texAnim.m_MaterialName = reader.ReadElementContentAsString();
                }
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("ScaleStartIndex"))
                {
                    texAnim.m_ScaleStart = uint.Parse(reader.ReadElementContentAsString());
                }
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("ScaleLength"))
                {
                    texAnim.m_ScaleLength = uint.Parse(reader.ReadElementContentAsString());
                }
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("RotationStartIndex"))
                {
                    texAnim.m_RotStart = uint.Parse(reader.ReadElementContentAsString());
                }
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("RotationLength"))
                {
                    texAnim.m_RotLength = uint.Parse(reader.ReadElementContentAsString());
                }
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("TranslationStartIndex"))
                {
                    texAnim.m_TransStart = uint.Parse(reader.ReadElementContentAsString());
                }
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("TranslationLength"))
                {
                    texAnim.m_TransLength = uint.Parse(reader.ReadElementContentAsString());
                }
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("DefaultScaleValue"))
                {
                    texAnim.m_DefaultScale = uint.Parse(reader.ReadElementContentAsString());
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("TextureAnimation"))
                {
                    return texAnim;
                }
            }

            return texAnim;
        }

        private static void ReadStandardObject(XmlReader reader)
        {
            int area = int.Parse(reader.GetAttribute("area"));
            int layer = int.Parse(reader.GetAttribute("star"));

            StandardObject obj = new StandardObject(m_Overlay, 0/*offset assigned later*/, 0/*don't need unique ID's yet*/,
                layer, area);

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("ObjectID"))
                {
                    obj.ID = ushort.Parse(reader.ReadElementContentAsString());
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Position"))
                {
                    Vector3 position = ReadPositionVector3(reader);
                    obj.Position = position;
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("YRotation"))
                {
                    float yRotation = float.Parse(reader.ReadElementContentAsString(), usa);
                    obj.YRotation = yRotation;
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Parameters"))
                {
                    obj.Parameters = ReadParameters(reader, 3);
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("StandardObject"))
                {
                    break;
                }
            }

            m_ObjectsToImport[area].Add(obj);
        }

        private static void ReadEntranceObject(XmlReader reader)
        {
            int entranceID = int.Parse(reader.GetAttribute("entranceID"));

            // For EntranceObjects the area is set to 0
            int area = 0;

            EntranceObject obj = new EntranceObject(m_Overlay, 0, 0, 0, entranceID);

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Position"))
                {
                    Vector3 position = ReadPositionVector3(reader);
                    obj.Position = position;
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("YRotation"))
                {
                    float yRotation = float.Parse(reader.ReadElementContentAsString(), usa);
                    obj.YRotation = yRotation;
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Parameters"))
                {
                    // Should be 4
                    string countAttr = reader.GetAttribute("count");
                    int count = (countAttr != null) ? int.Parse(countAttr) : 5;

                    ushort[] tempParams = ReadParameters(reader, count);

                    // Before this revision (R73) Entrance objects had 5 parameters but the third was a duplicate of 
                    // the Y Rotation, no longer exported but older XML's will still have the 5
                    if (count == 5)
                        tempParams = new ushort[] { tempParams[0], tempParams[1], tempParams[3], tempParams[4] };

                    obj.Parameters = tempParams;

                    // Eventually we want to get rid of this, and use only the below line

                    //obj.Parameters = ReadParameters(reader, 4);
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("EntranceObject"))
                {
                    break;
                }
            }

            m_ObjectsToImport[area].Add(obj);
        }

        private static void ReadPathObject(XmlReader reader)
        {
            // For PathObjects the area is set to 0
            int area = 0;

            PathObject obj = new PathObject(m_Overlay, 0, pathCount++);

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("StartNodeIndex"))
                {
                    obj.Parameters[0] = ushort.Parse(reader.ReadElementContentAsString());
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Length"))
                {
                    obj.Parameters[1] = ushort.Parse(reader.ReadElementContentAsString());
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Parameters"))
                {
                    while (reader.Read())
                    {
                        reader.MoveToContent();
                        if (reader.NodeType.Equals(XmlNodeType.Element))
                        {
                            if (reader.LocalName.Equals("Parameter"))
                            {
                                int paramID = int.Parse(reader.GetAttribute("id")) + 2;
                                ushort param = ushort.Parse(reader.ReadElementContentAsString());

                                if (paramID < 5)
                                    obj.Parameters[paramID] = param;
                            }
                        }
                        else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("Parameters"))
                        {
                            break;
                        }
                    }
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Nodes"))
                {
                    while (reader.Read())
                    {
                        reader.MoveToContent();
                        if (reader.NodeType.Equals(XmlNodeType.Element))
                        {
                            if (reader.LocalName.Equals("PathPointObject"))
                            {
                                ReadPathPointObject(reader);
                            }
                        }
                        else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("Nodes"))
                        {
                            break;
                        }
                    }
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("PathObject"))
                {
                    break;
                }
            }

            m_ObjectsToImport[area].Add(obj);
        }

        private static void ReadPathPointObject(XmlReader reader)
        {
            int index = int.Parse(reader.GetAttribute("index"));

            // For PathPointObjects the area is set to 0
            int area = 0;
            // Node ID can just be -1 here, will be set properly during level loading
            int tmp = -1;

            PathPointObject obj = new PathPointObject(m_Overlay, 0, 0, tmp);

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Position"))
                {
                    Vector3 position = ReadPositionVector3(reader);
                    obj.Position = position;
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("PathPointObject"))
                {
                    break;
                }
            }

            m_ObjectsToImport[area].Add(obj);
        }

        private static void ReadViewObject(XmlReader reader)
        {
            // For ViewObjects the area is set to 0
            int area = 0;

            ViewObject obj = new ViewObject(m_Overlay, 0, 0);

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Position"))
                {
                    Vector3 position = ReadPositionVector3(reader);
                    obj.Position = position;
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("YRotation"))
                {
                    float yRotation = float.Parse(reader.ReadElementContentAsString(), usa);
                    obj.YRotation = yRotation;
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Parameters"))
                {
                    obj.Parameters = ReadParameters(reader, 3);
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("ViewObject"))
                {
                    break;
                }
            }

            m_ObjectsToImport[area].Add(obj);
        }

        private static void ReadSimpleObject(XmlReader reader)
        {
            int area = int.Parse(reader.GetAttribute("area"));
            int layer = int.Parse(reader.GetAttribute("star"));

            SimpleObject obj = new SimpleObject(m_Overlay, 0, 0, layer, area);

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("ObjectID"))
                {
                    obj.ID = ushort.Parse(reader.ReadElementContentAsString());
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Position"))
                {
                    Vector3 position = ReadPositionVector3(reader);
                    obj.Position = position;
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("YRotation"))
                {
                    float yRotation = float.Parse(reader.ReadElementContentAsString(), usa);
                    obj.YRotation = yRotation;
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Parameters"))
                {
                    obj.Parameters = ReadParameters(reader, 1);
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("SimpleObject"))
                {
                    break;
                }
            }

            m_ObjectsToImport[area].Add(obj);
        }

        private static void ReadTpSrcObject(XmlReader reader)
        {
            // For TpSrcObjects the area and layer is set to 0
            int area = 0;
            int layer = 0;

            TpSrcObject obj = new TpSrcObject(m_Overlay, 0, 0, layer);

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Position"))
                {
                    Vector3 position = ReadPositionVector3(reader);
                    obj.Position = position;
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Parameters"))
                {
                    obj.Parameters = ReadParameters(reader, 2);
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("TpSrcObject"))
                {
                    break;
                }
            }

            m_ObjectsToImport[area].Add(obj);
        }

        private static void ReadTpDstObject(XmlReader reader)
        {
            // For TpDstObjects the area and layer is set to 0
            int area = 0;
            int layer = 0;

            TpDstObject obj = new TpDstObject(m_Overlay, 0, 0, layer);

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Position"))
                {
                    Vector3 position = ReadPositionVector3(reader);
                    obj.Position = position;
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Parameters"))
                {
                    obj.Parameters = ReadParameters(reader, 1);
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("TpDstObject"))
                {
                    break;
                }
            }

            m_ObjectsToImport[area].Add(obj);
        }

        private static void ReadFogObject(XmlReader reader)
        {
            // For FogObjects the area and layer is set to 0
            int area = 0;
            int layer = 0;

            FogObject obj = new FogObject(m_Overlay, 0, 0, layer, area);

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Density"))
                {
                    obj.Parameters[0] = ushort.Parse(reader.ReadElementContentAsString());
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Red"))
                {
                    obj.Parameters[1] = ushort.Parse(reader.ReadElementContentAsString());
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Green"))
                {
                    obj.Parameters[2] = ushort.Parse(reader.ReadElementContentAsString());
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Blue"))
                {
                    obj.Parameters[3] = ushort.Parse(reader.ReadElementContentAsString());
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("StartDistance"))
                {
                    obj.Parameters[4] = ushort.Parse(reader.ReadElementContentAsString());
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("EndDistance"))
                {
                    obj.Parameters[5] = ushort.Parse(reader.ReadElementContentAsString());
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("FogObject"))
                {
                    break;
                }
            }

            m_ObjectsToImport[area].Add(obj);
        }

        private static void ReadDoorObject(XmlReader reader)
        {
            // For DoorObjects the area is set to 0
            int area = 0;
            int layer = int.Parse(reader.GetAttribute("star"));

            DoorObject obj = new DoorObject(m_Overlay, 0, 0, layer);

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Position"))
                {
                    Vector3 position = ReadPositionVector3(reader);
                    obj.Position = position;
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("YRotation"))
                {
                    float yRotation = float.Parse(reader.ReadElementContentAsString(), usa);
                    obj.YRotation = yRotation;
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("DoorType"))
                {
                    obj.DoorType = int.Parse(reader.ReadElementContentAsString());
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("OutAreaID"))
                {
                    obj.OutAreaID = int.Parse(reader.ReadElementContentAsString());
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("InAreaID"))
                {
                    obj.InAreaID = int.Parse(reader.ReadElementContentAsString());
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("PlaneSizeX"))
                {
                    obj.PlaneSizeX = int.Parse(reader.ReadElementContentAsString());
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("PlaneSizeY"))
                {
                    obj.PlaneSizeY = int.Parse(reader.ReadElementContentAsString());
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("DoorObject"))
                {
                    break;
                }
            }

            m_ObjectsToImport[area].Add(obj);
        }

        private static void ReadExitObject(XmlReader reader)
        {
            // For ExitObjects the area is set to 0
            int area = 0;
            int layer = int.Parse(reader.GetAttribute("star"));

            ExitObject obj = new ExitObject(m_Overlay, 0, 0, layer);

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Position"))
                {
                    Vector3 position = ReadPositionVector3(reader);
                    obj.Position = position;
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("YRotation"))
                {
                    float yRotation = float.Parse(reader.ReadElementContentAsString(), usa);
                    obj.YRotation = yRotation;
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("DestinationLevel"))
                {
                    obj.LevelID = int.Parse(reader.ReadElementContentAsString());
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("EntranceID"))
                {
                    obj.EntranceID = int.Parse(reader.ReadElementContentAsString());
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Parameters"))
                {
                    while (reader.Read())
                    {
                        reader.MoveToContent();
                        if (reader.NodeType.Equals(XmlNodeType.Element))
                        {
                            if (reader.LocalName.Equals("Parameter"))
                            {
                                int paramID = int.Parse(reader.GetAttribute("id"));
                                ushort param = ushort.Parse(reader.ReadElementContentAsString());

                                if (paramID == 0)
                                    obj.Param1 = param;
                                else if (paramID == 1)
                                    obj.Param2 = param;
                            }
                        }
                        else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("Parameters"))
                        {
                            break;
                        }
                    }
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("ExitObject"))
                {
                    break;
                }
            }

            m_ObjectsToImport[area].Add(obj);
        }

        private static void ReadMinimapTileIDObject(XmlReader reader)
        {
            // For MinimapTileIDObjects the area and layer is set to 0
            int area = 0;

            MinimapTileIDObject obj = new MinimapTileIDObject(m_Overlay, 0, 0, 0, tileCount);

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Location"))
                {
                    // If there are more MinimapTileIDObjects than there are minimap tilemaps in the current area, 
                    // ignore the later ones
                    if (tileCount < m_NumAreas && tileCount < m_OldMinimapTileIDs.Length)
                        ReadFileFromXML(reader, "Tilemap", m_OldMinimapTileIDs[tileCount]);
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("MinimapTileIDObject"))
                {
                    if (tileCount < m_NumAreas && tileCount < m_OldMinimapTileIDs.Length)
                        obj.Parameters[0] = m_OldMinimapTileIDs[tileCount++];
                    else
                        obj.Parameters[0] = 0;
                    break;
                }
            }

            m_ObjectsToImport[area].Add(obj);
        }

        private static void ReadMinimapScaleObject(XmlReader reader)
        {
            // For MinimapScaleObjects the area and layer is set to 0
            int area = 0;
            int layer = 0;

            MinimapScaleObject obj = new MinimapScaleObject(m_Overlay, 0, 0, layer, area);

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Scale"))
                {
                    obj.Parameters[0] = ushort.Parse(reader.ReadElementContentAsString());
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("MinimapScaleObject"))
                {
                    break;
                }
            }

            m_ObjectsToImport[area].Add(obj);
        }

        private static void ReadType14Object(XmlReader reader)
        {
            // For Type14Objects the area and layer is set to 0
            int area = 0;
            int layer = 0;

            Type14Object obj = new Type14Object(m_Overlay, 0, 0, layer, area);

            while (reader.Read())
            {
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Parameters"))
                {
                    obj.Parameters = ReadParameters(reader, 4);
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("Type14Object"))
                {
                    break;
                }
            }

            m_ObjectsToImport[area].Add(obj);
        }

        private static Vector3 ReadPositionVector3(XmlReader reader)
        {
            Vector3 position = new Vector3(0, 0, 0);

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element))
                {
                    if (reader.LocalName.Equals("X"))
                    {
                        position.X = float.Parse(reader.ReadElementContentAsString(), usa);
                    }
                    else if (reader.LocalName.Equals("Y"))
                    {
                        position.Y = float.Parse(reader.ReadElementContentAsString(), usa);
                    }
                    else if (reader.LocalName.Equals("Z"))
                    {
                        position.Z = float.Parse(reader.ReadElementContentAsString(), usa);
                    }
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("Position"))
                {
                    break;
                }
            }

            return position;
        }

        private static ushort[] ReadParameters(XmlReader reader, int numParams = 3)
        {
            ushort[] parameters = new ushort[numParams];

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element))
                {
                    if (reader.LocalName.Equals("Parameter"))
                    {
                        int paramID = int.Parse(reader.GetAttribute("id"));
                        ushort param = ushort.Parse(reader.ReadElementContentAsString());

                        if (paramID < numParams)
                            parameters[paramID] = param;
                    }
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("Parameters"))
                {
                    break;
                }
            }

            return parameters;
        }
    }

    class TextureAnimationArea
    {
        public int m_Area;
        public List<uint> m_ScaleValues;
        public List<ushort> m_RotationValues;
        public List<uint> m_TranslationValues;
        public List<TextureAnimationForImport> m_TextureAnimations;
        public uint m_NumFrames;

        public TextureAnimationArea(int area)
        {
            m_Area = area;
            m_ScaleValues = new List<uint>();
            m_RotationValues = new List<ushort>();
            m_TranslationValues = new List<uint>();
            m_TextureAnimations = new List<TextureAnimationForImport>();
        }
    }

    class TextureAnimationForImport
    {
        public string m_MaterialName;
        public uint m_ScaleStart;
        public uint m_ScaleLength;
        public uint m_RotStart;
        public uint m_RotLength;
        public uint m_TransStart;
        public uint m_TransLength;
        public uint m_DefaultScale;

        public TextureAnimationForImport()
        {
            m_MaterialName = null;
            m_ScaleStart = 0;
            m_ScaleLength = 0;
            m_RotStart = 0;
            m_RotLength = 0;
            m_TransStart = 0;
            m_TransLength = 0;
            m_DefaultScale = 1;
        }
    }
}
