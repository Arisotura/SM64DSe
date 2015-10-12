using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SM64DSe.ImportExport.LevelImportExport
{
    public class LevelImporter
    {
        protected NitroOverlay m_Overlay;
        protected int m_LevelID;
        protected int m_NumAreas;
        protected string m_FileName;
        protected string m_Path;

        protected List<INitroROMBlock> m_FilesToSave;

        protected ushort m_OldBMDFileID;
        protected ushort m_OldKCLFileID;
        protected ushort m_OldNCGFileID;
        protected ushort m_OldNCLFileID;
        protected ushort[] m_OldMinimapTileIDs;

        protected uint m_DataOffset = 0;
        protected List<LevelObject>[] m_ObjectsToImport;
        protected TextureAnimationArea[] m_TextureAnimationAreasToImport;

        protected static int[] OBJECT_SIZES = { 16, 16, 6, 6, 14, 8, 8, 8, 8, 12, 14, 2, 2, 0, 4 };
        protected static int[] MISC_TYPES = { 1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 14 };
        protected static int[] STANDARD_SIMPLE_TYPES = { 0, 5 };
        // The below is needed because unless objects are written with entrance objects last, the view 
        // will not be correct upon entering the level and cannot be fixed. Split by standard and simple objects 
        // and miscelaneous objects
        protected static int[] OBJECT_WRITE_TYPE_ORDER = { 0, 5, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 1 };

        protected int pathCount = 0;
        protected int tileCount = 0;

        public LevelImporter(string filename, NitroOverlay overlay, int levelID, ushort[] minimapTileIDs)
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
        }

        public virtual int ImportLevel(bool saveChanges = true) { return 0; }

        protected void ClearOverlayAndAddLoaderCode()
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

        public void SaveChangesToAllFiles()
        {
            foreach (INitroROMBlock file in m_FilesToSave)
            {
                file.SaveChanges();
            }
        }

        protected void ProcessLevelData()
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

        protected AreaObjectList[] SplitObjectsByAreaStarType()
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

        protected int GetNumObjectListsInArea(AreaObjectList areaObjectList, int[] objectTypes)
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

        protected void WriteObjectTableHeadersAndDataForArea(AreaObjectList areaObjectList, uint object_tables_entries,
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

        protected class AreaObjectList
        {
            public int m_Area;
            public StarObjectList[] m_StarObjectList;

            public AreaObjectList(int area)
            {
                m_Area = area;
                m_StarObjectList = new StarObjectList[8];
            }
        }

        protected class StarObjectList
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

        protected class TypeObjectList
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

        protected class TextureAnimationArea
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

        protected class TextureAnimationForImport
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

        protected void WriteTextureAnimations(uint level_area_data)
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

                    next_material_name_address += (uint)(((mat_name.Length + 1) + 3) & ~3); // remember that there'll be a null byte after name

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
    }
}
