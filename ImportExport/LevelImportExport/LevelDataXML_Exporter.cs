using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Globalization;
using System.IO;

namespace SM64DSe.ImportExport.LevelImportExport
{
    public static class LevelDataXML_Exporter
    {
        private static NitroOverlay m_Overlay;
        private static int m_LevelID;
        private static LevelSettings m_LevelSettings;
        private static Dictionary<uint, LevelObject> m_LevelObjects;
        private static List<LevelTexAnim>[] m_TexAnims;
        private static int m_NumAreas;
        private static string m_FileName;
        private static string m_Path;

        public static void ExportLevelDataToXML(NitroOverlay overlay, int levelID, LevelSettings levelSettings, 
            Dictionary<uint, LevelObject> levelObjects, List<LevelTexAnim>[] texAnims, string filename = "level.xml")
        {
            m_Overlay = overlay;
            m_LevelID = levelID;
            m_LevelSettings = levelSettings;
            m_LevelObjects = levelObjects;
            m_TexAnims = texAnims;
            m_NumAreas = m_Overlay.Read8(0x74);
            m_FileName = filename;
            m_Path = Path.GetDirectoryName(m_FileName);

            ExportXML();

            System.IO.File.WriteAllBytes(m_Path + "/OVL_" + (m_LevelID + 103) + ".bin", m_Overlay.m_Data);
        }

        private static void ExportXML()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.NewLineChars = "\r\n";
            settings.NewLineHandling = NewLineHandling.Replace;
            using (XmlWriter writer = XmlWriter.Create(m_FileName, settings))
            {
                writer.WriteStartDocument();
                writer.WriteComment(Program.AppTitle + " " + Program.AppVersion + " " + Program.AppDate);
                writer.WriteStartElement("Level");
                writer.WriteAttributeString("id", m_LevelID.ToString());

                WriteLevelSettingsToXML(writer);

                WriteCLPSToXML(writer);

                WriteLevelDataToXML(writer);

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private static void WriteLevelSettingsToXML(XmlWriter writer)
        {
            // Start LevelSettings
            writer.WriteStartElement("LevelSettings");

            writer.WriteStartElement("ObjectBankSettings");
            for (int i = 0; i <= 7; i++)
            {
                writer.WriteStartElement("Bank");
                writer.WriteAttributeString("id", i.ToString());
                writer.WriteString(m_LevelSettings.ObjectBanks[i].ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            WriteFilenameToXML(writer, "LevelModel", m_LevelSettings.BMDFileID,
                new Dictionary<string, string> { /*{ "overrideFileID", "FALSE" }*/ });
            WriteFilenameToXML(writer, "CollisionMap", m_LevelSettings.KCLFileID,
                new Dictionary<string, string> { /*{ "overrideFileID", "FALSE" }*/ });

            writer.WriteStartElement("MinimapSettings");
            WriteFilenameToXML(writer, "Tileset", m_LevelSettings.MinimapTsetFileID,
                new Dictionary<string, string> { /*{ "overrideFileID", "FALSE" }*/ });
            WriteFilenameToXML(writer, "Palette", m_LevelSettings.MinimapPalFileID,
                new Dictionary<string, string> { /*{ "overrideFileID", "FALSE" }*/ });
            writer.WriteElementString("CoordinateScale", m_LevelSettings.MinimapCoordinateScale.ToString());
            writer.WriteEndElement();

            writer.WriteElementString("NumberOfAreas", m_NumAreas.ToString());
            writer.WriteElementString("CameraStartZoomedOut", m_LevelSettings.CameraStartZoomedOut.ToString());
            writer.WriteElementString("Background", m_LevelSettings.Background.ToString());
            writer.WriteStartElement("Music");
            for (int i = 0; i < 3; i++)
                writer.WriteElementString("Byte", m_LevelSettings.MusicBytes[i].ToString());
            writer.WriteEndElement();

            Program.m_ROM.BeginRW();
            writer.WriteElementString("ActSelectorID",
                Program.m_ROM.Read8((uint)(Helper.GetActSelectorIDTableAddress() + m_LevelID)).ToString());
            Program.m_ROM.EndRW();

            writer.WriteEndElement();
            // End LevelSettings
        }

        private static void WriteFilenameToXML(XmlWriter writer, string element, ushort fileID, Dictionary<string, string> attributes = null)
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

            System.IO.File.WriteAllBytes(m_Path + "/" + 
                filename.Substring(filename.LastIndexOf('/'), filename.Length - (filename.LastIndexOf('/'))), 
                file.m_Data);
        }

        private static void WriteCLPSToXML(XmlWriter writer)
        {
            writer.WriteStartElement("CLPS");

            uint clps_addr = m_Overlay.ReadPointer(0x60);
            uint clps_num = m_Overlay.Read16(clps_addr + 0x06);
            uint clps_size = (uint)(8 + (clps_num * 8));
            byte[][] entries = new byte[clps_num][];
            uint entry = clps_addr + 0x08;

            for (int i = 0; i < clps_num; i++)
            {
                entries[i] = new byte[8];

                for (int j = 0; j < 8; j++)
                    entries[i][j] = m_Overlay.Read8((uint)(entry + (j)));

                entry += 8;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                writer.WriteStartElement("Entry");
                writer.WriteStartElement("Value");
                for (int j = 0; j < entries[i].Length; j++)
                    writer.WriteElementString("Byte", entries[i][j].ToString());

                writer.WriteEndElement();
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        private static void WriteLevelDataToXML(XmlWriter writer)
        {
            // Start LevelData
            writer.WriteStartElement("LevelData");

            WriteLevelObjectsToXML(writer);

            WriteTextureAnimationDataToXML(writer);

            writer.WriteEndElement();
            // End LevelData
        }

        private static void WriteLevelObjectsToXML(XmlWriter writer)
        {
            writer.WriteStartElement("Objects");
            for (int i = 0; i < m_LevelObjects.Count; i++)
            {
                LevelObject obj = m_LevelObjects.ElementAt(i).Value;
                switch (obj.m_Type)
                {
                    case (0):
                        WriteStandardObjectToXML(writer, (StandardObject)obj);
                        break;
                    case (1):
                        WriteEntranceObjectToXML(writer, (EntranceObject)obj);
                        break;
                    // 2 - Path points are written by Path objects
                    case (3):
                        WritePathObjectToXML(writer, (PathObject)obj);
                        break;
                    case (4):
                        WriteViewObjectToXML(writer, (ViewObject)obj);
                        break;
                    case (5):
                        WriteSimpleObjectToXML(writer, (SimpleObject)obj);
                        break;
                    case (6):
                        WriteTpSrcObjectToXML(writer, (TpSrcObject)obj);
                        break;
                    case (7):
                        WriteTpDstObjectToXML(writer, (TpDstObject)obj);
                        break;
                    case (8):
                        WriteFogObjectToXML(writer, (FogObject)obj);
                        break;
                    case (9):
                        WriteDoorObjectToXML(writer, (DoorObject)obj);
                        break;
                    case (10):
                        WriteExitObjectToXML(writer, (ExitObject)obj);
                        break;
                    case (11):
                        WriteMinimapTileIDObjectToXML(writer, (MinimapTileIDObject)obj);
                        break;
                    case (12):
                        WriteMinimapScaleObjectToXML(writer, (MinimapScaleObject)obj);
                        break;
                    case (14):
                        WriteType14ObjectToXML(writer, (Type14Object)obj);
                        break;
                    default:
                        break;
                }
            }
            writer.WriteEndElement();
        }

        private static void WriteStandardObjectToXML(XmlWriter writer, StandardObject obj)
        {
            writer.WriteStartElement("StandardObject");

            writer.WriteAttributeString("area", obj.m_Area.ToString());
            writer.WriteAttributeString("star", obj.m_Layer.ToString());
            writer.WriteComment(ObjectDatabase.m_ObjectInfo[obj.ID].m_InternalName);

            writer.WriteElementString("ObjectID", obj.ID.ToString());

            writer.WriteStartElement("Position");
            writer.WriteElementString("X", Helper.ToString(obj.Position.X));
            writer.WriteElementString("Y", Helper.ToString(obj.Position.Y));
            writer.WriteElementString("Z", Helper.ToString(obj.Position.Z));
            writer.WriteEndElement();

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

        private static void WriteEntranceObjectToXML(XmlWriter writer, EntranceObject obj)
        {
            writer.WriteStartElement("EntranceObject");
            writer.WriteAttributeString("entranceID", obj.m_EntranceID.ToString());

            writer.WriteStartElement("Position");
            writer.WriteElementString("X", Helper.ToString(obj.Position.X));
            writer.WriteElementString("Y", Helper.ToString(obj.Position.Y));
            writer.WriteElementString("Z", Helper.ToString(obj.Position.Z));
            writer.WriteEndElement();

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

        private static void WritePathNodeObjectToXML(XmlWriter writer, PathPointObject obj, int index = -1)
        {
            writer.WriteStartElement("PathPointObject");
            if (index > -1)
                writer.WriteAttributeString("index", index.ToString());

            writer.WriteStartElement("Position");
            writer.WriteElementString("X", Helper.ToString(obj.Position.X));
            writer.WriteElementString("Y", Helper.ToString(obj.Position.Y));
            writer.WriteElementString("Z", Helper.ToString(obj.Position.Z));
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        private static void WritePathObjectToXML(XmlWriter writer, PathObject obj)
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

            IEnumerable<LevelObject> nodes = m_LevelObjects.Values.Where(obj0 => (obj0.m_Type) == 2);
            writer.WriteStartElement("Nodes");
            for (int j = obj.Parameters[0]; j < (obj.Parameters[0] + obj.Parameters[1]); j++)
            {
                WritePathNodeObjectToXML(writer, (PathPointObject)nodes.ElementAt(j), j);
            }
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        private static void WriteViewObjectToXML(XmlWriter writer, ViewObject obj)
        {
            writer.WriteStartElement("ViewObject");

            //writer.WriteAttributeString("area", obj.m_Area.ToString());// Need?
            //writer.WriteAttributeString("star", obj.m_Layer.ToString());// Need?

            writer.WriteStartElement("Position");
            writer.WriteElementString("X", Helper.ToString(obj.Position.X));
            writer.WriteElementString("Y", Helper.ToString(obj.Position.Y));
            writer.WriteElementString("Z", Helper.ToString(obj.Position.Z));
            writer.WriteEndElement();

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

        private static void WriteSimpleObjectToXML(XmlWriter writer, SimpleObject obj)
        {
            writer.WriteStartElement("SimpleObject");

            writer.WriteAttributeString("area", obj.m_Area.ToString());
            writer.WriteAttributeString("star", obj.m_Layer.ToString());
            if (obj.ID != 511)
                writer.WriteComment(ObjectDatabase.m_ObjectInfo[obj.ID].m_InternalName);
            writer.WriteElementString("ObjectID", obj.ID.ToString());

            writer.WriteStartElement("Position");
            writer.WriteElementString("X", Helper.ToString(obj.Position.X));
            writer.WriteElementString("Y", Helper.ToString(obj.Position.Y));
            writer.WriteElementString("Z", Helper.ToString(obj.Position.Z));
            writer.WriteEndElement();

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

        private static void WriteTpSrcObjectToXML(XmlWriter writer, TpSrcObject obj)
        {
            writer.WriteStartElement("TpSrcObject");

            writer.WriteStartElement("Position");
            writer.WriteElementString("X", Helper.ToString(obj.Position.X));
            writer.WriteElementString("Y", Helper.ToString(obj.Position.Y));
            writer.WriteElementString("Z", Helper.ToString(obj.Position.Z));
            writer.WriteEndElement();

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

        private static void WriteTpDstObjectToXML(XmlWriter writer, TpDstObject obj)
        {
            writer.WriteStartElement("TpDstObject");

            writer.WriteStartElement("Position");
            writer.WriteElementString("X", Helper.ToString(obj.Position.X));
            writer.WriteElementString("Y", Helper.ToString(obj.Position.Y));
            writer.WriteElementString("Z", Helper.ToString(obj.Position.Z));
            writer.WriteEndElement();

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

        private static void WriteFogObjectToXML(XmlWriter writer, FogObject obj)
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

        private static void WriteDoorObjectToXML(XmlWriter writer, DoorObject obj)
        {
            writer.WriteStartElement("DoorObject");
            writer.WriteAttributeString("star", obj.m_Layer.ToString());

            writer.WriteStartElement("Position");
            writer.WriteElementString("X", Helper.ToString(obj.Position.X));
            writer.WriteElementString("Y", Helper.ToString(obj.Position.Y));
            writer.WriteElementString("Z", Helper.ToString(obj.Position.Z));
            writer.WriteEndElement();

            writer.WriteElementString("YRotation", Helper.ToString(obj.YRotation));

            writer.WriteElementString("DoorType", obj.DoorType.ToString());
            writer.WriteElementString("OutAreaID", obj.OutAreaID.ToString());
            writer.WriteElementString("InAreaID", obj.InAreaID.ToString());
            writer.WriteElementString("PlaneSizeX", obj.PlaneSizeX.ToString());
            writer.WriteElementString("PlaneSizeY", obj.PlaneSizeY.ToString());

            writer.WriteEndElement();
        }

        private static void WriteExitObjectToXML(XmlWriter writer, ExitObject obj)
        {
            writer.WriteStartElement("ExitObject");
            writer.WriteAttributeString("star", obj.m_Layer.ToString());

            writer.WriteStartElement("Position");
            writer.WriteElementString("X", Helper.ToString(obj.Position.X));
            writer.WriteElementString("Y", Helper.ToString(obj.Position.Y));
            writer.WriteElementString("Z", Helper.ToString(obj.Position.Z));
            writer.WriteEndElement();

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

        private static void WriteMinimapTileIDObjectToXML(XmlWriter writer, MinimapTileIDObject obj)
        {
            writer.WriteStartElement("MinimapTileIDObject");

            //writer.WriteElementString("TileID", obj.Parameters[0].ToString());
            writer.WriteComment(obj.Parameters[0].ToString());

            if (obj.Parameters[0] != 0)
            {
                WriteFilenameToXML(writer, "Tilemap", obj.Parameters[0],
                    new Dictionary<string, string> { /*{ "overrideFileID", "FALSE" }*/ });
            }

            writer.WriteEndElement();
        }

        private static void WriteMinimapScaleObjectToXML(XmlWriter writer, MinimapScaleObject obj)
        {
            writer.WriteStartElement("MinimapScaleObject");

            writer.WriteElementString("Scale", obj.Parameters[0].ToString());

            writer.WriteEndElement();
        }

        private static void WriteType14ObjectToXML(XmlWriter writer, Type14Object obj)
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

        private static void WriteTextureAnimationDataToXML(XmlWriter writer)
        {
            // Start TextureAnimationData
            writer.WriteStartElement("TextureAnimationData");

            for (int area = 0; area < m_NumAreas; area++)
            {
                uint objlistptr = m_Overlay.ReadPointer(0x70);
                uint texAnimAddr = (uint)(objlistptr + (area * 12) + 4);
                bool areaHasTexAnims = (m_Overlay.Read32(texAnimAddr) != 0);

                if (!areaHasTexAnims)
                    continue;

                writer.WriteStartElement("TextureAnimationArea");
                writer.WriteAttributeString("area", area.ToString());

                uint texAnimHeader = m_Overlay.ReadPointer(texAnimAddr);
                writer.WriteElementString("NumberOfFrames", m_Overlay.Read32(texAnimHeader).ToString());
                writer.WriteElementString("NumberOfAnimations", m_Overlay.Read32(texAnimHeader + 0x10).ToString());

                int scaleSize = 0;
                int rotSize = 0;
                int transSize = 0;
                for (int j = 0; j < m_TexAnims[area].Count; j++)
                {
                    if ((m_TexAnims[area].ElementAt(j).getScaleTblStart() + (int)m_TexAnims[area].ElementAt(j).getScaleTblSize()) > scaleSize)
                        scaleSize += (int)m_TexAnims[area].ElementAt(j).getScaleTblSize();
                    if ((m_TexAnims[area].ElementAt(j).getRotTblStart() + (int)m_TexAnims[area].ElementAt(j).getRotTblSize()) > rotSize)
                        rotSize += (int)m_TexAnims[area].ElementAt(j).getRotTblSize();
                    if ((m_TexAnims[area].ElementAt(j).getTransTblStart() + (int)m_TexAnims[area].ElementAt(j).getTransTblSize()) > transSize)
                        transSize += (int)m_TexAnims[area].ElementAt(j).getTransTblSize();
                }

                writer.WriteStartElement("ScaleTable");
                for (int i = 0; i < scaleSize; i++)
                {
                    writer.WriteElementString("UInt", m_Overlay.Read32(m_TexAnims[area].ElementAt(0).m_BaseScaleTblAddr +
                        (uint)(i * 4)).ToString());
                }
                writer.WriteEndElement();
                writer.WriteStartElement("RotationTable");
                for (int i = 0; i < rotSize; i++)
                {
                    writer.WriteElementString("UShort", m_Overlay.Read16(m_TexAnims[area].ElementAt(0).m_BaseRotTblAddr +
                        (uint)(i * 2)).ToString());
                }
                writer.WriteEndElement();
                writer.WriteStartElement("TranslationTable");
                for (int i = 0; i < transSize; i++)
                {
                    writer.WriteElementString("UInt", m_Overlay.Read32(m_TexAnims[area].ElementAt(0).m_BaseTransTblAddr +
                        (uint)(i * 4)).ToString());
                }
                writer.WriteEndElement();

                for (int i = 0; i < m_TexAnims[area].Count; i++)
                {
                    writer.WriteStartElement("TextureAnimation");

                    writer.WriteElementString("MaterialName", m_Overlay.ReadString(m_TexAnims[area].ElementAt(i).m_MatNameOffset, 0));
                    writer.WriteElementString("ScaleStartIndex", m_TexAnims[area].ElementAt(i).getScaleTblStart().ToString());
                    writer.WriteElementString("ScaleLength", m_TexAnims[area].ElementAt(i).getScaleTblSize().ToString());
                    writer.WriteElementString("RotationStartIndex", m_TexAnims[area].ElementAt(i).getRotTblStart().ToString());
                    writer.WriteElementString("RotationLength", m_TexAnims[area].ElementAt(i).getRotTblSize().ToString());
                    writer.WriteElementString("TranslationStartIndex", m_TexAnims[area].ElementAt(i).getTransTblStart().ToString());
                    writer.WriteElementString("TranslationLength", m_TexAnims[area].ElementAt(i).getTransTblSize().ToString());
                    writer.WriteElementString("DefaultScaleValue", m_Overlay.Read32(m_TexAnims[area].ElementAt(i).m_Offset + 0x08).ToString());

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            // End TextureAnimationData
        }
    }
}
