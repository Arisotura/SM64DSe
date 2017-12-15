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
        private const int CURRENT_VERSION = 2;
        private static readonly LevelExporterV1 k_LevelExporterV1 = new LevelExporterV1();
        private static readonly LevelExporterV1 k_LevelExporterV2 = new LevelExporterV2();

        public static void ExportLevelDataToXML(Level level, string fileName, int version)
        {
            switch (version)
            {
                default: 
                case CURRENT_VERSION:
                    k_LevelExporterV2.ExportLevelDataToXML(level, fileName);
                    break;
                case 1:
                    k_LevelExporterV1.ExportLevelDataToXML(level, fileName);
                    break;
            }
        }

        public static void ExportLevelDataToXML(Level level, string fileName = "level.xml")
        {
            ExportLevelDataToXML(level, fileName, CURRENT_VERSION);
        }
    }
}
