using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Forms;
using System.Xml;
using OpenTK;
using System.IO;

namespace SM64DSe.ImportExport.LevelImportExport
{
    public class LevelDataXML_Importer : LevelImporter
    {
        public LevelDataXML_Importer(string filename, NitroOverlay overlay, int levelID, ushort[] minimapTileIDs) 
            : base(filename, overlay, levelID, minimapTileIDs) { }

        public override int ImportLevel(bool saveChanges = true)
        {
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

        private void ImportXML()
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

        private void ReadLevelSettingsFromXML(XmlReader reader)
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

        private void ReadObjectBankSettings(XmlReader reader)
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
        private void ReadFileFromXML(XmlReader reader, string element, ushort fileID)
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

        private void ReadMinimapSettings(XmlReader reader)
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

        private void ReadMusicBytes(XmlReader reader)
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

        private void ReadCLPSDataFromXML(XmlReader reader)
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

        private void ReadLevelDataFromXML(XmlReader reader)
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

        private void ReadObjects(XmlReader reader)
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

        private void ReadTextureAnimationData(XmlReader reader)
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

        private void ReadTextureAnimationArea(XmlReader reader)
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

        private List<uint> ReadTextureAnimationUIntTable(XmlReader reader, string element)
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

        private List<ushort> ReadTextureAnimationUShortTable(XmlReader reader, string element)
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

        private TextureAnimationForImport ReadTextureAnimation(XmlReader reader)
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

        private void ReadStandardObject(XmlReader reader)
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
                    float yRotation = Helper.ParseFloat(reader.ReadElementContentAsString());
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

        private void ReadEntranceObject(XmlReader reader)
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
                    float yRotation = Helper.ParseFloat(reader.ReadElementContentAsString());
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

        private void ReadPathObject(XmlReader reader)
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

        private void ReadPathPointObject(XmlReader reader)
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

        private void ReadViewObject(XmlReader reader)
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
                    float yRotation = Helper.ParseFloat(reader.ReadElementContentAsString());
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

        private void ReadSimpleObject(XmlReader reader)
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
                    float yRotation = Helper.ParseFloat(reader.ReadElementContentAsString());
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

        private void ReadTpSrcObject(XmlReader reader)
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

        private void ReadTpDstObject(XmlReader reader)
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

        private void ReadFogObject(XmlReader reader)
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

        private void ReadDoorObject(XmlReader reader)
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
                    float yRotation = Helper.ParseFloat(reader.ReadElementContentAsString());
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

        private void ReadExitObject(XmlReader reader)
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
                    float yRotation = Helper.ParseFloat(reader.ReadElementContentAsString());
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

        private void ReadMinimapTileIDObject(XmlReader reader)
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

        private void ReadMinimapScaleObject(XmlReader reader)
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

        private void ReadType14Object(XmlReader reader)
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

        private Vector3 ReadPositionVector3(XmlReader reader)
        {
            Vector3 position = new Vector3(0, 0, 0);

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element))
                {
                    if (reader.LocalName.Equals("X"))
                    {
                        position.X = Helper.ParseFloat(reader.ReadElementContentAsString());
                    }
                    else if (reader.LocalName.Equals("Y"))
                    {
                        position.Y = Helper.ParseFloat(reader.ReadElementContentAsString());
                    }
                    else if (reader.LocalName.Equals("Z"))
                    {
                        position.Z = Helper.ParseFloat(reader.ReadElementContentAsString());
                    }
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("Position"))
                {
                    break;
                }
            }

            return position;
        }

        private ushort[] ReadParameters(XmlReader reader, int numParams = 3)
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
}
