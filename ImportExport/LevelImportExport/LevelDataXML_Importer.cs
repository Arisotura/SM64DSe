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
    public class LevelDataXML_Importer
    {
        private const int CURRENT_VERSION = 2;
        private static readonly LevelImporterV1 k_LevelImporterV1 = new LevelImporterV1();
        private static readonly LevelImporterV2 k_LevelImporterV2 = new LevelImporterV2();

        public static void ImportLevel(Level level, string fileName, bool saveChanges = true)
        {
            int version;
            using (XmlReader reader = XmlReader.Create(fileName, new XmlReaderSettings() { CloseInput = true }))
            {
                reader.MoveToContent();

                if (!(reader.NodeType.Equals(XmlNodeType.Element) && "Level".Equals(reader.LocalName)))
                {
                    throw new InvalidDataException("The specified file is not a level exported by SM64DSe");
                }
                else
                {
                    string versionAttribute = reader.GetAttribute("version");
                    if (Strings.IsBlank(versionAttribute))
                    {
                        version = 0;
                    }
                    else
                    {
                        if (!int.TryParse(versionAttribute, out version))
                        {
                            throw new InvalidDataException("The version of the specified file's level format is not recognised");
                        }
                        if (version > CURRENT_VERSION)
                        {
                            throw new InvalidDataException("The specified level was exported by a newer version of SM64DSe and cannot be imported");
                        }
                    }
                }
            }

            switch (version)
            {
                default: 
                case CURRENT_VERSION:
                    k_LevelImporterV2.ImportLevel(level, fileName, saveChanges);
                    break;
                case 1:
                    k_LevelImporterV1.ImportLevel(level, fileName, saveChanges);
                    break;
            }
        }
    }
}
