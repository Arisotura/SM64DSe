using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace SM64DSe.ImportExport.LevelImportExport
{
    class LevelExporterV1
    {
        protected virtual string VERSION { get { return "1"; } }

        public virtual void ExportLevelDataToXML(Level level, string fileName = "level.xml")
        {
            string exportPath = Path.GetDirectoryName(fileName);

            System.IO.File.WriteAllBytes(exportPath + "/OVL_" + (level.m_LevelID + 103) + ".bin", level.m_Overlay.m_Data);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.NewLineChars = "\r\n";
            settings.NewLineHandling = NewLineHandling.Replace;
            using (XmlWriter writer = XmlWriter.Create(fileName, settings))
            {
                writer.WriteStartDocument();
                writer.WriteComment(Program.AppTitle + " " + Program.AppVersion + " " + Program.AppDate);
                writer.WriteStartElement("Level");
                writer.WriteAttributeString("id", level.m_LevelID.ToString());
                writer.WriteAttributeString("version", VERSION);

                WriteLevelSettingsToXML(writer, exportPath, level);

                WriteCLPSToXML(writer, level);

                WriteLevelDataToXML(writer, exportPath, level);

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        protected virtual void WriteLevelSettingsToXML(XmlWriter writer, string exportPath, Level level)
        {
            // Start LevelSettings
            writer.WriteStartElement("LevelSettings");

            writer.WriteStartElement("ObjectBankSettings");
            for (int i = 0; i <= 7; i++)
            {
                writer.WriteStartElement("Bank");
                writer.WriteAttributeString("id", i.ToString());
                writer.WriteString(level.m_LevelSettings.ObjectBanks[i].ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            WriteFilenameToXML(writer, exportPath, "LevelModel", level.m_LevelSettings.BMDFileID,
                new Dictionary<string, string> { /*{ "overrideFileID", "FALSE" }*/ });
            WriteFilenameToXML(writer, exportPath, "CollisionMap", level.m_LevelSettings.KCLFileID,
                new Dictionary<string, string> { /*{ "overrideFileID", "FALSE" }*/ });

            writer.WriteStartElement("MinimapSettings");
            WriteFilenameToXML(writer, exportPath, "Tileset", level.m_LevelSettings.MinimapTsetFileID,
                new Dictionary<string, string> { /*{ "overrideFileID", "FALSE" }*/ });
            WriteFilenameToXML(writer, exportPath, "Palette", level.m_LevelSettings.MinimapPalFileID,
                new Dictionary<string, string> { /*{ "overrideFileID", "FALSE" }*/ });
            writer.WriteElementString("CoordinateScale", level.m_LevelSettings.MinimapCoordinateScale.ToString());
            writer.WriteStartElement("MinimapTilemapIndices");
            for (int area = 0; area < level.m_NumAreas; area++)
            {
                writer.WriteStartElement("MinimapTilemapIndex");
                writer.WriteAttributeString("area", area.ToString());
                writer.WriteString(level.m_MinimapIndices[area].ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteElementString("NumberOfAreas", level.m_NumAreas.ToString());
            writer.WriteElementString("CameraStartZoomedOut", level.m_LevelSettings.CameraStartZoomedOut.ToString());
            writer.WriteElementString("Background", level.m_LevelSettings.Background.ToString());
            writer.WriteStartElement("Music");
            for (int i = 0; i < 3; i++)
                writer.WriteElementString("Byte", level.m_LevelSettings.MusicBytes[i].ToString());
            writer.WriteEndElement();

            Program.m_ROM.BeginRW();
            writer.WriteElementString("ActSelectorID",
                Program.m_ROM.Read8((uint)(Helper.GetActSelectorIDTableAddress() + level.m_LevelID)).ToString());
            Program.m_ROM.EndRW();

            writer.WriteEndElement();
            // End LevelSettings
        }

        protected virtual void WriteFilenameToXML(XmlWriter writer, string exportPath, string element, ushort fileID, Dictionary<string, string> attributes = null)
        {
            writer.WriteStartElement(element);
            if (attributes != null)
            {
                foreach (string attr in attributes.Keys)
                    writer.WriteAttributeString(attr, attributes[attr]);
            }
            NitroFile file = Program.m_ROM.GetFileFromInternalID(fileID);
            string filename = file.m_Name;
            //writer.WriteElementString("Filename", filename);
            writer.WriteComment(filename);
            writer.WriteElementString("Location", "." + filename.Substring(filename.LastIndexOf('/')));
            writer.WriteEndElement();

            System.IO.File.WriteAllBytes(exportPath + "/" +
                filename.Substring(filename.LastIndexOf('/'), filename.Length - (filename.LastIndexOf('/'))),
                file.m_Data);
        }

        protected virtual void WriteCLPSToXML(XmlWriter writer, Level level)
        {
            writer.WriteStartElement("CLPS");

            byte[] entryBytes;
            foreach (CLPS.Entry entry in level.m_CLPS)
            {
                writer.WriteStartElement("Entry");
                writer.WriteStartElement("Value");

                entryBytes = entry.FlagsBytes;
                for (int i = 0; i < entryBytes.Length; i++)
                    writer.WriteElementString("Byte", entryBytes[i].ToString());

                writer.WriteEndElement();
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        protected virtual void WriteLevelDataToXML(XmlWriter writer, string exportPath, Level level)
        {
            // Start LevelData
            writer.WriteStartElement("LevelData");

            WriteLevelObjectsToXML(writer, exportPath, level);

            WriteTextureAnimationDataToXML(writer, level);

            writer.WriteEndElement();
            // End LevelData
        }

        protected virtual void WriteLevelObjectsToXML(XmlWriter writer, string exportPath, Level level)
        {
            writer.WriteStartElement("Objects");
            for (int i = 0; i < level.m_LevelObjects.Count; i++)
            {
                LevelObject obj = level.m_LevelObjects.ElementAt(i).Value;
                switch (obj.m_Type)
                {
                    case (LevelObject.Type.STANDARD):
                        WriteStandardObjectToXML(writer, (StandardObject)obj);
                        break;
                    case (LevelObject.Type.ENTRANCE):
                        WriteEntranceObjectToXML(writer, (EntranceObject)obj);
                        break;
                    // 2 - Path points are written by Path objects
                    case (LevelObject.Type.PATH):
                        WritePathObjectToXML(writer, level, (PathObject)obj);
                        break;
                    case (LevelObject.Type.VIEW):
                        WriteViewObjectToXML(writer, (ViewObject)obj);
                        break;
                    case (LevelObject.Type.SIMPLE):
                        WriteSimpleObjectToXML(writer, (SimpleObject)obj);
                        break;
                    case (LevelObject.Type.TELEPORT_SOURCE):
                        WriteTpSrcObjectToXML(writer, (TpSrcObject)obj);
                        break;
                    case (LevelObject.Type.TELEPORT_DESTINATION):
                        WriteTpDstObjectToXML(writer, (TpDstObject)obj);
                        break;
                    case (LevelObject.Type.FOG):
                        WriteFogObjectToXML(writer, (FogObject)obj);
                        break;
                    case (LevelObject.Type.DOOR):
                        WriteDoorObjectToXML(writer, (DoorObject)obj);
                        break;
                    case (LevelObject.Type.EXIT):
                        WriteExitObjectToXML(writer, (ExitObject)obj);
                        break;
                    case (LevelObject.Type.MINIMAP_TILE_ID):
                        WriteMinimapTileIDObjectToXML(writer, exportPath, (MinimapTileIDObject)obj);
                        break;
                    case (LevelObject.Type.MINIMAP_SCALE):
                        WriteMinimapScaleObjectToXML(writer, (MinimapScaleObject)obj);
                        break;
                    case (LevelObject.Type.UNKNOWN_14):
                        WriteType14ObjectToXML(writer, (Type14Object)obj);
                        break;
                    default:
                        break;
                }
            }
            writer.WriteEndElement();
        }

        protected virtual void WriteStandardObjectToXML(XmlWriter writer, StandardObject obj)
        {
            writer.WriteStartElement("StandardObject");

            writer.WriteAttributeString("area", obj.m_Area.ToString());
            writer.WriteAttributeString("star", obj.m_Layer.ToString());
            writer.WriteComment(ObjectDatabase.m_ObjectInfo[obj.ID].m_InternalName);

            writer.WriteElementString("ObjectID", obj.ID.ToString());

            WriteObjectPositionToXML(writer, obj);

            writer.WriteElementString("YRotation", Helper.ToString(obj.YRotation));

            writer.WriteStartElement("Parameters");
            writer.WriteAttributeString("count", "3");
            for (int j = 0; j < 3; j++)
            {
                writer.WriteStartElement("Parameter");
                writer.WriteAttributeString("id", j.ToString());
                writer.WriteString(obj.Parameters[j].ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        protected virtual void WriteEntranceObjectToXML(XmlWriter writer, EntranceObject obj)
        {
            writer.WriteStartElement("EntranceObject");
            writer.WriteAttributeString("entranceID", obj.m_EntranceID.ToString());

            WriteObjectPositionToXML(writer, obj);

            writer.WriteElementString("YRotation", Helper.ToString(obj.YRotation));

            writer.WriteStartElement("Parameters");
            writer.WriteAttributeString("count", "4");
            for (int j = 0; j < 4; j++)
            {
                writer.WriteStartElement("Parameter");
                writer.WriteAttributeString("id", j.ToString());
                writer.WriteString(obj.Parameters[j].ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        protected virtual void WritePathNodeObjectToXML(XmlWriter writer, PathPointObject obj, int index = -1)
        {
            writer.WriteStartElement("PathPointObject");
            if (index > -1)
                writer.WriteAttributeString("index", index.ToString());

            WriteObjectPositionToXML(writer, obj);

            writer.WriteEndElement();
        }

        protected virtual void WritePathObjectToXML(XmlWriter writer, Level level, PathObject obj)
        {
            writer.WriteStartElement("PathObject");

            writer.WriteElementString("StartNodeIndex", obj.Parameters[0].ToString());
            writer.WriteElementString("Length", obj.Parameters[1].ToString());

            writer.WriteStartElement("Parameters");
            writer.WriteAttributeString("count", "3");
            for (int j = 0; j < 3; j++)
            {
                writer.WriteStartElement("Parameter");
                writer.WriteAttributeString("id", j.ToString());
                writer.WriteString(obj.Parameters[j + 2].ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            IEnumerable<LevelObject> nodes = level.GetAllObjectsByType(LevelObject.Type.PATH_NODE);
            writer.WriteStartElement("Nodes");
            for (int j = obj.Parameters[0]; j < (obj.Parameters[0] + obj.Parameters[1]); j++)
            {
                WritePathNodeObjectToXML(writer, (PathPointObject)nodes.ElementAt(j), j);
            }
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        protected virtual void WriteViewObjectToXML(XmlWriter writer, ViewObject obj)
        {
            writer.WriteStartElement("ViewObject");

            WriteObjectPositionToXML(writer, obj);

            writer.WriteElementString("YRotation", Helper.ToString(obj.YRotation));

            writer.WriteStartElement("Parameters");
            writer.WriteAttributeString("count", "3");
            for (int j = 0; j < 3; j++)
            {
                writer.WriteStartElement("Parameter");
                writer.WriteAttributeString("id", j.ToString());
                writer.WriteString(obj.Parameters[j].ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        protected virtual void WriteSimpleObjectToXML(XmlWriter writer, SimpleObject obj)
        {
            writer.WriteStartElement("SimpleObject");

            writer.WriteAttributeString("area", obj.m_Area.ToString());
            writer.WriteAttributeString("star", obj.m_Layer.ToString());
            if (obj.ID != 511)
                writer.WriteComment(ObjectDatabase.m_ObjectInfo[obj.ID].m_InternalName);
            writer.WriteElementString("ObjectID", obj.ID.ToString());

            WriteObjectPositionToXML(writer, obj);

            writer.WriteStartElement("Parameters");
            writer.WriteAttributeString("count", "1");
            for (int j = 0; j < 1; j++)
            {
                writer.WriteStartElement("Parameter");
                writer.WriteAttributeString("id", j.ToString());
                writer.WriteString(obj.Parameters[j].ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        protected virtual void WriteTpSrcObjectToXML(XmlWriter writer, TpSrcObject obj)
        {
            writer.WriteStartElement("TpSrcObject");

            WriteObjectPositionToXML(writer, obj);

            writer.WriteStartElement("Parameters");
            writer.WriteAttributeString("count", "2");
            for (int j = 0; j < 2; j++)
            {
                writer.WriteStartElement("Parameter");
                writer.WriteAttributeString("id", j.ToString());
                writer.WriteString(obj.Parameters[j].ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        protected virtual void WriteTpDstObjectToXML(XmlWriter writer, TpDstObject obj)
        {
            writer.WriteStartElement("TpDstObject");

            WriteObjectPositionToXML(writer, obj);

            writer.WriteStartElement("Parameters");
            writer.WriteAttributeString("count", "1");
            for (int j = 0; j < 1; j++)
            {
                writer.WriteStartElement("Parameter");
                writer.WriteAttributeString("id", j.ToString());
                writer.WriteString(obj.Parameters[j].ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        protected virtual void WriteFogObjectToXML(XmlWriter writer, FogObject obj)
        {
            writer.WriteStartElement("FogObject");

            writer.WriteElementString("Density", obj.Parameters[0].ToString());
            writer.WriteElementString("Red", obj.Parameters[1].ToString());
            writer.WriteElementString("Green", obj.Parameters[2].ToString());
            writer.WriteElementString("Blue", obj.Parameters[3].ToString());
            writer.WriteElementString("StartDistance", obj.Parameters[4].ToString());
            writer.WriteElementString("EndDistance", obj.Parameters[5].ToString());

            writer.WriteEndElement();
        }

        protected virtual void WriteDoorObjectToXML(XmlWriter writer, DoorObject obj)
        {
            writer.WriteStartElement("DoorObject");
            writer.WriteAttributeString("star", obj.m_Layer.ToString());

            WriteObjectPositionToXML(writer, obj);

            writer.WriteElementString("YRotation", Helper.ToString(obj.YRotation));

            writer.WriteElementString("DoorType", obj.DoorType.ToString());
            writer.WriteElementString("OutAreaID", obj.OutAreaID.ToString());
            writer.WriteElementString("InAreaID", obj.InAreaID.ToString());
            writer.WriteElementString("PlaneSizeX", obj.PlaneSizeX.ToString());
            writer.WriteElementString("PlaneSizeY", obj.PlaneSizeY.ToString());

            writer.WriteEndElement();
        }

        protected virtual void WriteExitObjectToXML(XmlWriter writer, ExitObject obj)
        {
            writer.WriteStartElement("ExitObject");
            writer.WriteAttributeString("star", obj.m_Layer.ToString());

            WriteObjectPositionToXML(writer, obj);

            writer.WriteElementString("YRotation", Helper.ToString(obj.YRotation));

            writer.WriteElementString("DestinationLevel", obj.LevelID.ToString());
            writer.WriteElementString("EntranceID", obj.EntranceID.ToString());

            writer.WriteStartElement("Parameters");

            writer.WriteStartElement("Parameter");
            writer.WriteAttributeString("count", "2");
            writer.WriteAttributeString("id", "0");
            writer.WriteString(obj.Param1.ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("Parameter");
            writer.WriteAttributeString("id", "1");
            writer.WriteString(obj.Param2.ToString());
            writer.WriteEndElement();

            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        protected virtual void WriteMinimapTileIDObjectToXML(XmlWriter writer, string exportPath, MinimapTileIDObject obj)
        {
            writer.WriteStartElement("MinimapTileIDObject");

            //writer.WriteElementString("TileID", obj.Parameters[0].ToString());
            writer.WriteComment(obj.Parameters[0].ToString());

            if (obj.Parameters[0] != 0)
            {
                WriteFilenameToXML(writer, exportPath, "Tilemap", obj.Parameters[0],
                    new Dictionary<string, string> { /*{ "overrideFileID", "FALSE" }*/ });
            }

            writer.WriteEndElement();
        }

        protected virtual void WriteMinimapScaleObjectToXML(XmlWriter writer, MinimapScaleObject obj)
        {
            writer.WriteStartElement("MinimapScaleObject");

            writer.WriteElementString("Scale", obj.Parameters[0].ToString());

            writer.WriteEndElement();
        }

        protected virtual void WriteType14ObjectToXML(XmlWriter writer, Type14Object obj)
        {
            writer.WriteStartElement("Type14Object");

            writer.WriteStartElement("Parameters");
            writer.WriteAttributeString("count", "4");
            for (int j = 0; j < 4; j++)
            {
                writer.WriteStartElement("Parameter");
                writer.WriteAttributeString("id", j.ToString());
                writer.WriteString(obj.Parameters[j].ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        protected virtual void WriteObjectPositionToXML(XmlWriter writer, LevelObject obj)
        {
            writer.WriteStartElement("Position");
            writer.WriteElementString("X", Helper.ToString(obj.Position.X));
            writer.WriteElementString("Y", Helper.ToString(obj.Position.Y));
            writer.WriteElementString("Z", Helper.ToString(obj.Position.Z));
            writer.WriteEndElement();
        }

        protected virtual void WriteTextureAnimationDataToXML(XmlWriter writer, Level level)
        {
            // Start TextureAnimationData
            writer.WriteStartElement("TextureAnimationData");

            for (int area = 0; area < level.m_NumAreas; area++)
            {
                List<LevelTexAnim> texAnims = level.m_TexAnims.Where(ta => ta.m_Area == area).ToList();

                if (texAnims.Count < 1) continue;

                foreach (LevelTexAnim texAnim in texAnims)
                {
                    writer.WriteStartElement("TextureAnimationArea");
                    writer.WriteAttributeString("area", texAnim.m_Area.ToString());

                    writer.WriteElementString("NumberOfFrames", texAnim.m_NumFrames.ToString());
                    writer.WriteElementString("NumberOfAnimations", texAnim.m_NumDefs.ToString());

                    List<float> scales = texAnim.m_Defs.SelectMany(y => y.m_ScaleValues).ToList();
                    List<int> scalesInt = texAnim.m_Defs.SelectMany(y => y.m_ScaleValuesInt).ToList();
                    List<float> rotations = texAnim.m_Defs.SelectMany(y => y.m_RotationValues).ToList();
                    List<int> rotationsInt = texAnim.m_Defs.SelectMany(y => y.m_RotationValuesInt).ToList();
                    List<float> translations = texAnim.m_Defs.SelectMany(y => y.m_TranslationXValues).ToList();
                    List<int> translationsInt = texAnim.m_Defs.SelectMany(y => y.m_TranslationXValuesInt).ToList();

                    writer.WriteStartElement("ScaleTable");
                    for (int i = 0; i < scalesInt.Count; i++)
                    {
                        writer.WriteElementString("UInt", scalesInt[i].ToString());
                    }
                    writer.WriteEndElement();
                    writer.WriteStartElement("RotationTable");
                    for (int i = 0; i < rotationsInt.Count; i++)
                    {
                        writer.WriteElementString("UShort", rotationsInt[i].ToString());
                    }
                    writer.WriteEndElement();
                    writer.WriteStartElement("TranslationTable");
                    for (int i = 0; i < translationsInt.Count; i++)
                    {
                        writer.WriteElementString("UInt", translationsInt[i].ToString());
                    }
                    writer.WriteEndElement();

                    foreach (LevelTexAnim.Def def in texAnim.m_Defs)
                    {
                        writer.WriteStartElement("TextureAnimation");

                        writer.WriteElementString("MaterialName", def.m_MaterialName);
                        writer.WriteElementString("ScaleStartIndex", Helper.FindSubList(scales, def.m_ScaleValues).ToString());
                        writer.WriteElementString("ScaleLength", def.m_NumScaleValues.ToString());
                        writer.WriteElementString("RotationStartIndex", Helper.FindSubList(rotations, def.m_RotationValues).ToString());
                        writer.WriteElementString("RotationLength", def.m_NumRotationValues.ToString());
                        writer.WriteElementString("TranslationStartIndex", Helper.FindSubList(translations, def.m_TranslationXValues).ToString());
                        writer.WriteElementString("TranslationLength", def.m_NumTranslationXValues.ToString());
                        writer.WriteElementString("DefaultScaleValue", def.m_DefaultScale.ToString());

                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
            // End TextureAnimationData
        }
    }
}
