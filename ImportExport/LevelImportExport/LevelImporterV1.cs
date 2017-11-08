using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using OpenTK;

namespace SM64DSe.ImportExport.LevelImportExport
{
    class LevelImporterV1
    {
        protected string m_ImportPath;
        protected List<NitroFile> m_FilesToSave;

        public LevelImporterV1()
        {
            m_FilesToSave = new List<NitroFile>();
        }

        public virtual void ImportLevel(Level level, string fileName, bool saveChanges = true)
        {
            level.m_CLPS.m_Entries.Clear();
            level.RemoveAllObjects();
            level.m_TexAnims.Clear();
            level.m_DynLibIDs.Clear();

            ImportXML(level, fileName);

            if (saveChanges)
            {
                try { level.SaveChanges(); }
                catch (Exception ex) { new ExceptionMessageBox("Error saving level, changes have not been saved", ex).ShowDialog(); return; }

                try { SaveChangesToImportedFiles(); }
                catch (Exception ex) { new ExceptionMessageBox("Error saving level files, overlay changes have already been saved", ex).ShowDialog(); return; }
            }
        }

        public virtual void SaveChangesToImportedFiles()
        {
            foreach (NitroFile file in m_FilesToSave)
            {
                file.SaveChanges();
            }
        }

        protected virtual void ImportXML(Level level, string fileName)
        {
            m_ImportPath = Path.GetDirectoryName(fileName);
            using (XmlReader reader = XmlReader.Create(fileName, new XmlReaderSettings() { CloseInput = true }))
            {
                reader.MoveToContent();

                while (reader.Read())
                {
                    if (reader.NodeType.Equals(XmlNodeType.Element))
                    {
                        switch (reader.LocalName)
                        {
                            case "LevelSettings":
                                ReadLevelSettingsFromXML(reader, level);
                                break;
                            case "CLPS":
                                ReadCLPSDataFromXML(reader, level);
                                break;
                            case "LevelData":
                                ReadLevelDataFromXML(reader, level);
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
        }

        protected virtual void ReadLevelSettingsFromXML(XmlReader reader, Level level)
        {
            LevelSettings levelSettings = new LevelSettings();
            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element))
                {
                    if (reader.LocalName.Equals("ObjectBankSettings"))
                    {
                        ReadObjectBankSettings(reader.ReadSubtree(), levelSettings);
                    }
                    else if (reader.LocalName.Equals("LevelModel"))
                    {
                        ReadFileFromXML(reader, "LevelModel", level.m_LevelSettings.BMDFileID);
                        levelSettings.BMDFileID = level.m_LevelSettings.BMDFileID;
                    }
                    else if (reader.LocalName.Equals("CollisionMap"))
                    {
                        ReadFileFromXML(reader, "CollisionMap", level.m_LevelSettings.KCLFileID);
                        levelSettings.KCLFileID = level.m_LevelSettings.KCLFileID;
                    }
                    else if (reader.LocalName.Equals("MinimapSettings"))
                    {
                        ReadMinimapSettings(reader.ReadSubtree(), level, levelSettings);
                    }
                    else if (reader.LocalName.Equals("NumberOfAreas"))
                    {
                        level.m_NumAreas = byte.Parse(reader.ReadElementContentAsString());
                    }
                    else if (reader.LocalName.Equals("CameraStartZoomedOut"))
                    {
                        levelSettings.CameraStartZoomedOut = byte.Parse(reader.ReadElementContentAsString());
                    }
                    else if (reader.LocalName.Equals("Background"))
                    {
                        levelSettings.Background = byte.Parse(reader.ReadElementContentAsString());
                        levelSettings.QuestionMarks = 0xF;
                    }
                    else if (reader.LocalName.Equals("Music"))
                    {
                        ReadMusicBytes(reader.ReadSubtree(), levelSettings);
                    }
                    else if (reader.LocalName.Equals("ActSelectorID"))
                    {
                        levelSettings.ActSelectorID = byte.Parse(reader.ReadElementContentAsString());
                    }

                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement))
                {
                    if (reader.LocalName.Equals("LevelSettings"))
                    {
                        level.m_LevelSettings = levelSettings;
                        break;
                    }
                }
            }
        }

        protected virtual void ReadObjectBankSettings(XmlReader reader, LevelSettings levelSettings)
        {
            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Bank"))
                {
                    int bankID = int.Parse(reader.GetAttribute("id"));
                    uint bankSetting = (uint)reader.ReadElementContentAsInt();

                    levelSettings.ObjectBanks[bankID] = bankSetting;
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("ObjectBankSettings"))
                {
                    return;
                }
            }

            return;
        }

        // File specified in <Location> will overwrite file with specified file ID
        protected virtual void ReadFileFromXML(XmlReader reader, string element, ushort fileID)
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
                byte[] fileToImport = System.IO.File.ReadAllBytes(m_ImportPath + "/" + location);
                currentFile.m_Data = fileToImport;
                m_FilesToSave.Add(currentFile);
            }
        }

        protected virtual void ReadMinimapSettings(XmlReader reader, 
            Level level, LevelSettings levelSettings)
        {
            level.m_MinimapIndices = new byte[Level.k_MaxNumAreas];
            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Tileset"))
                {
                    ReadFileFromXML(reader, "Tileset", level.m_LevelSettings.MinimapTsetFileID);
                    levelSettings.MinimapTsetFileID = level.m_LevelSettings.MinimapTsetFileID;
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Palette"))
                {
                    ReadFileFromXML(reader, "Palette", level.m_LevelSettings.MinimapPalFileID);
                    levelSettings.MinimapPalFileID = level.m_LevelSettings.MinimapPalFileID;
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("CoordinateScale"))
                {
                    levelSettings.MinimapCoordinateScale = ushort.Parse(reader.ReadElementContentAsString());
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("MinimapTilemapIndices"))
                {
                    while (reader.Read())
                    {
                        reader.MoveToContent();
                        if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("MinimapTilemapIndex"))
                        {
                            int area = int.Parse(reader.GetAttribute("area"));
                            if (area < Level.k_MaxNumAreas)
                            {
                                byte index = byte.Parse(reader.ReadElementContentAsString());
                                level.m_MinimapIndices[area] = index;
                            }
                        }
                        else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("MinimapTilemapIndices"))
                        {
                            break;
                        }
                    }
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("MinimapSettings"))
                {
                    return;
                }
            }

            return;
        }

        protected virtual void ReadMusicBytes(XmlReader reader, LevelSettings levelSettings)
        {
            int byteID = 0;

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Byte"))
                {
                    byte value = byte.Parse(reader.ReadElementContentAsString());
                    levelSettings.MusicBytes[byteID++] = value;
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("Music"))
                {
                    return;
                }
            }

            return;
        }

        protected virtual void ReadCLPSDataFromXML(XmlReader reader, Level level)
        {
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
                        CLPS.Entry clps = new CLPS.Entry();
                        clps.flags = DataHelper.Read32(clpsEntry, 0);
                        clps.flags |= (ulong)DataHelper.Read32(clpsEntry, 4) << 32;
                        level.m_CLPS.Add(clps);
                    }
                    else if (reader.LocalName.Equals("CLPS"))
                    {
                        break;
                    }
                }
            }
        }

        protected virtual void ReadLevelDataFromXML(XmlReader reader, Level level)
        {
            if (level.m_NumAreas == 0) level.m_NumAreas = 1;

            // First read in all the objects and texture animations, then worry about writing them
            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element))
                {
                    if (reader.LocalName.Equals("Objects"))
                    {
                        ReadObjects(reader, level);
                    }
                    if (reader.LocalName.Equals("TextureAnimationData"))
                    {
                        ReadTextureAnimationData(reader, level);
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

        protected virtual void ReadObjects(XmlReader reader, Level level)
        {
            while (reader.Read())
            {
                reader.MoveToContent();

                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("StandardObject"))
                {
                    ReadStandardObject(reader, level);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("EntranceObject"))
                {
                    ReadEntranceObject(reader, level);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("PathObject"))
                {
                    ReadPathObject(reader, level);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("ViewObject"))
                {
                    ReadViewObject(reader, level);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("SimpleObject"))
                {
                    ReadSimpleObject(reader, level);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("TpSrcObject"))
                {
                    ReadTpSrcObject(reader, level);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("TpDstObject"))
                {
                    ReadTpDstObject(reader, level);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("FogObject"))
                {
                    ReadFogObject(reader, level);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("DoorObject"))
                {
                    ReadDoorObject(reader, level);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("ExitObject"))
                {
                    ReadExitObject(reader, level);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("MinimapTileIDObject"))
                {
                    ReadMinimapTileIDObject(reader, level);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("MinimapScaleObject"))
                {
                    ReadMinimapScaleObject(reader, level);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Type14Object"))
                {
                    ReadType14Object(reader, level);
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("Objects"))
                {
                    return;
                }
            }

            return;
        }

        protected virtual void ReadTextureAnimationData(XmlReader reader, Level level)
        {
            while (reader.Read())
            {
                reader.MoveToContent();

                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("TextureAnimationArea"))
                {
                    ReadTextureAnimationArea(reader, level);
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("TextureAnimationData"))
                {
                    return;
                }
            }

            return;
        }

        protected virtual void ReadTextureAnimationArea(XmlReader reader, Level level)
        {
            int area = int.Parse(reader.GetAttribute("area"));

            List<LevelTexAnim> texAnimsForArea = level.m_TexAnims.Where(t => t.m_Area == area).ToList();
            LevelTexAnim texAnimArea;
            if (texAnimsForArea.Count < 1)
            {
                texAnimArea = new LevelTexAnim(area);
                level.m_TexAnims.Add(texAnimArea);
            }
            else
            {
                texAnimArea = texAnimsForArea[0];
            }

            List<float> scales = new List<float>();
            List<float> rotations = new List<float>();
            List<float> translations = new List<float>();

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("NumberOfFrames"))
                {
                    texAnimArea.m_NumFrames = (uint)reader.ReadElementContentAsInt();
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("ScaleTable"))
                {
                    scales = ReadTextureAnimationScaleTable(reader);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("RotationTable"))
                {
                    rotations = ReadTextureAnimationRotationTable(reader);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("TranslationTable"))
                {
                    translations = ReadTextureAnimationTranslationTable(reader);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("TextureAnimation"))
                {
                    texAnimArea.m_Defs.Add(ReadTextureAnimation(reader, scales, rotations, translations));
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("TextureAnimationArea"))
                {
                    break;
                }
            }
        }

        protected virtual List<float> ReadTextureAnimationTranslationTable(XmlReader reader)
        {
            return ReadTextureAnimationUIntTable(reader, "TranslationTable")
                .ConvertAll(x => x / 4096f);
        }

        protected virtual List<float> ReadTextureAnimationRotationTable(XmlReader reader)
        {
            return ReadTextureAnimationUShortTable(reader, "RotationTable")
                .ConvertAll(x => x / 4096.0f * 360.0f);
        }

        protected virtual List<float> ReadTextureAnimationScaleTable(XmlReader reader)
        {
            return ReadTextureAnimationUIntTable(reader, "ScaleTable")
                .ConvertAll(x => x / 4096f);
        }

        private List<uint> ReadTextureAnimationUIntTable(XmlReader reader, string element)
        {
            List<uint> values = new List<uint>();

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("UInt"))
                {
                    values.Add((uint)reader.ReadElementContentAsInt());
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

        protected virtual LevelTexAnim.Def ReadTextureAnimation(XmlReader reader, 
            List<float> scales, List<float> rotations, List<float> translations)
        {
            LevelTexAnim.Def texAnim = new LevelTexAnim.Def();

            int scaleStart = 0, scaleLength = 0;
            int rotationStart = 0, rotationLength = 0;
            int translationStart = 0, translationLength = 0;

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("MaterialName"))
                {
                    texAnim.m_MaterialName = reader.ReadElementContentAsString();
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("DefaultScaleValue"))
                {
                    texAnim.m_DefaultScale = (uint)reader.ReadElementContentAsInt() / 4096f;
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("ScaleStartIndex"))
                {
                    scaleStart = reader.ReadElementContentAsInt();
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("ScaleLength"))
                {
                    scaleLength = reader.ReadElementContentAsInt();
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("RotationStartIndex"))
                {
                    rotationStart = reader.ReadElementContentAsInt();
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("RotationLength"))
                {
                    rotationLength = reader.ReadElementContentAsInt();
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("TranslationStartIndex"))
                {
                    translationStart = reader.ReadElementContentAsInt();
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("TranslationLength"))
                {
                    translationLength = reader.ReadElementContentAsInt();
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("TextureAnimation"))
                {
                    texAnim.m_ScaleValues = scales.GetRange(scaleStart, scaleLength);
                    texAnim.m_RotationValues = rotations.GetRange(rotationStart, rotationLength);
                    texAnim.m_TranslationXValues = translations.GetRange(translationStart, translationLength);
                    return texAnim;
                }
            }

            return texAnim;
        }

        protected virtual void ReadStandardObject(XmlReader reader, Level level)
        {
            int area = int.Parse(reader.GetAttribute("area"));
            int layer = int.Parse(reader.GetAttribute("star"));

            ushort id = 0;
            Vector3 position = Vector3.Zero;
            float yRotation = 0.0f;
            ushort[] parameters = new ushort[3];

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("ObjectID"))
                {
                    id = ushort.Parse(reader.ReadElementContentAsString());
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Position"))
                {
                    position = ReadPositionVector3(reader);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("YRotation"))
                {
                    yRotation = Helper.ParseFloat(reader.ReadElementContentAsString());
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Parameters"))
                {
                    parameters = ReadParameters(reader, 3);
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("StandardObject"))
                {
                    break;
                }
            }

            StandardObject obj = level.AddStandardObject(id, layer, area);
            obj.Position = position;
            obj.YRotation = yRotation;
            obj.Parameters = parameters;
        }

        protected virtual void ReadEntranceObject(XmlReader reader, Level level)
        {
            EntranceObject obj = level.AddEntranceObject(0);

            obj.m_EntranceID = int.Parse(reader.GetAttribute("entranceID"));

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Position"))
                {
                    obj.Position = ReadPositionVector3(reader);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("YRotation"))
                {
                    obj.YRotation = Helper.ParseFloat(reader.ReadElementContentAsString());
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
        }

        protected virtual void ReadPathObject(XmlReader reader, Level level)
        {
            PathObject obj = level.AddPathObject();

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
                                ReadPathPointObject(reader, level, obj);
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
        }

        protected virtual void ReadPathPointObject(XmlReader reader, Level level, PathObject parent)
        {
            int index = int.Parse(reader.GetAttribute("index"));

            PathPointObject obj = level.AddPathPointObject(parent);

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
        }

        protected virtual void ReadViewObject(XmlReader reader, Level level)
        {
            ViewObject obj = level.AddViewObject();

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
        }

        protected virtual void ReadSimpleObject(XmlReader reader, Level level)
        {
            int area = int.Parse(reader.GetAttribute("area"));
            int layer = int.Parse(reader.GetAttribute("star"));

            ushort id = 0;
            Vector3 position = Vector3.Zero;
            float yRotation = 0.0f;
            ushort[] parameters = new ushort[1];

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("ObjectID"))
                {
                    id = ushort.Parse(reader.ReadElementContentAsString());
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Position"))
                {
                    position = ReadPositionVector3(reader);
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("YRotation"))
                {
                    yRotation = Helper.ParseFloat(reader.ReadElementContentAsString());
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Parameters"))
                {
                    parameters = ReadParameters(reader, 1);
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("SimpleObject"))
                {
                    break;
                }
            }

            SimpleObject obj = level.AddSimpleObject(id, layer, area);
            obj.Position = position;
            obj.YRotation = yRotation;
            obj.Parameters = parameters;
        }

        protected virtual void ReadTpSrcObject(XmlReader reader, Level level)
        {
            TpSrcObject obj = level.AddTpSrcObject(0);

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
        }

        protected virtual void ReadTpDstObject(XmlReader reader, Level level)
        {
            TpDstObject obj = level.AddTpDstObject(0);

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
        }

        protected virtual void ReadFogObject(XmlReader reader, Level level)
        {
            FogObject obj = level.AddFogObject(0, 0);

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
        }

        protected virtual void ReadDoorObject(XmlReader reader, Level level)
        {
            int layer = int.Parse(reader.GetAttribute("star"));

            DoorObject obj = level.AddDoorObject(layer);

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
                    obj.DoorType = reader.ReadElementContentAsInt();
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("OutAreaID"))
                {
                    obj.OutAreaID = reader.ReadElementContentAsInt();
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("InAreaID"))
                {
                    obj.InAreaID = reader.ReadElementContentAsInt();
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("PlaneSizeX"))
                {
                    obj.PlaneSizeX = reader.ReadElementContentAsInt();
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("PlaneSizeY"))
                {
                    obj.PlaneSizeY = reader.ReadElementContentAsInt();
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("DoorObject"))
                {
                    break;
                }
            }
        }

        protected virtual void ReadExitObject(XmlReader reader, Level level)
        {
            int layer = int.Parse(reader.GetAttribute("star"));

            ExitObject obj = level.AddExitObject(layer);

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
                    obj.LevelID = reader.ReadElementContentAsInt();
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("EntranceID"))
                {
                    obj.EntranceID = reader.ReadElementContentAsInt();
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
        }

        protected virtual void ReadMinimapTileIDObject(XmlReader reader, Level level)
        {
            int tileCount = level.GetAllObjectsByType(LevelObject.Type.MINIMAP_TILE_ID).Count();
            int nMaxMinimapTileIDs = level.m_MinimapFileIDs.Length;

            MinimapTileIDObject obj = level.AddMinimapTileIDObject(0, 0);

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("Location"))
                {
                    // If there are more MinimapTileIDObjects than there are minimap tilemaps in the current area, 
                    // ignore the later ones
                    if (tileCount < level.m_NumAreas && tileCount < nMaxMinimapTileIDs)
                        ReadFileFromXML(reader, "Tilemap", level.m_MinimapFileIDs[tileCount]);
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("MinimapTileIDObject"))
                {
                    if (tileCount < level.m_NumAreas && tileCount < nMaxMinimapTileIDs)
                        obj.Parameters[0] = level.m_MinimapFileIDs[tileCount++];
                    else
                        obj.Parameters[0] = 0;
                    break;
                }
            }
        }

        protected virtual void ReadMinimapScaleObject(XmlReader reader, Level level)
        {
            MinimapScaleObject obj = level.AddMinimapScaleObject(0, 0);

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
        }

        protected virtual void ReadType14Object(XmlReader reader, Level level)
        {
            Type14Object obj = level.AddType14Object(0, 0);

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
        }

        protected virtual Vector3 ReadPositionVector3(XmlReader reader)
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

        protected virtual ushort[] ReadParameters(XmlReader reader, int numParams = 3)
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
