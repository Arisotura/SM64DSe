using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SM64DSe.ImportExport.LevelImportExport
{
    class LevelExporterV2 : LevelExporterV1
    {
        protected override string VERSION { get { return "2"; } }

        protected override void WriteCLPSToXML(XmlWriter writer, Level level)
        {
            writer.WriteStartElement("CLPS");

            foreach (CLPS.Entry entry in level.m_CLPS)
            {
                writer.WriteStartElement("Entry");

                writer.WriteElementString("TerrainType", entry.m_Texture.ToString());
                writer.WriteElementString("Water", BoolToString(entry.m_Water > 0));
                writer.WriteElementString("ViewID", entry.m_ViewID.ToString());
                writer.WriteElementString("Traction", entry.m_Traction.ToString());
                writer.WriteElementString("CameraBehaviour", entry.m_CamBehav.ToString());
                writer.WriteElementString("Behaviour", entry.m_Behav.ToString());
                writer.WriteElementString("TransparentToCamera", BoolToString(entry.m_CamThrough > 0));
                writer.WriteElementString("Toxic", BoolToString(entry.m_Toxic > 0));
                writer.WriteElementString("Unknown26", entry.m_Unk26.ToString());
                writer.WriteElementString("Padding1", entry.m_Pad1.ToString());
                writer.WriteElementString("WindID", entry.m_WindID.ToString());
                writer.WriteElementString("Padding2", entry.m_Pad2.ToString());

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        protected override void WriteTextureAnimationDataToXML(XmlWriter writer, Level level)
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

                    List<float> scaleValues = texAnim.m_Defs.SelectMany(y => y.m_ScaleValues).ToList();
                    List<int> scaleValuesInt = texAnim.m_Defs.SelectMany(y => y.m_ScaleValuesInt).ToList();
                    List<float> rotationValues = texAnim.m_Defs.SelectMany(y => y.m_RotationValues).ToList();
                    List<int> rotationValuesInt = texAnim.m_Defs.SelectMany(y => y.m_RotationValuesInt).ToList();
                    List<float> translationValues = texAnim.m_Defs.SelectMany(y => y.m_CombinedTranslationValues).ToList();
                    List<int> translationValuesInt = texAnim.m_Defs.SelectMany(y => y.m_CombinedTranslationValuesInt).ToList();

                    WriteIntArray(writer, scaleValuesInt, "ScaleTable");
                    WriteIntArray(writer, rotationValuesInt, "RotationTable");
                    WriteIntArray(writer, translationValuesInt, "TranslationTable");

                    foreach (LevelTexAnim.Def def in texAnim.m_Defs)
                    {
                        writer.WriteStartElement("TextureAnimation");

                        writer.WriteElementString("MaterialName", def.m_MaterialName);
                        writer.WriteElementString("DefaultScaleValue", def.m_DefaultScale.ToString());
                        writer.WriteElementString("ScaleStartIndex", Helper.FindSubList(scaleValues, def.m_ScaleValues).ToString());
                        writer.WriteElementString("ScaleLength", def.m_NumScaleValues.ToString());
                        writer.WriteElementString("RotationStartIndex", Helper.FindSubList(rotationValues, def.m_RotationValues).ToString());
                        writer.WriteElementString("RotationLength", def.m_NumRotationValues.ToString());
                        writer.WriteElementString("TranslationXStartIndex", Helper.FindSubList(translationValues, def.m_TranslationXValues).ToString());
                        writer.WriteElementString("TranslationXLength", def.m_NumTranslationXValues.ToString());
                        writer.WriteElementString("TranslationYStartIndex", Helper.FindSubList(translationValues, def.m_TranslationYValues).ToString());
                        writer.WriteElementString("TranslationYLength", def.m_NumTranslationYValues.ToString());

                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
            // End TextureAnimationData
        }

        protected virtual void WriteIntArray(XmlWriter writer, List<int> values, string elementName = "IntArray")
        {
            writer.WriteStartElement(elementName);
            int length = values.Count;
            writer.WriteAttributeString("length", length.ToString());
            for (int i = 0; i < length; i++)
            {
                writer.WriteElementString("value", values[i].ToString());
            }
            writer.WriteEndElement();
        }

        protected string BoolToString(bool value)
        {
            return value.ToString().ToLower();
        }
    }
}
