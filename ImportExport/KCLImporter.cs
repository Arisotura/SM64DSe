using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SM64DSe.ImportExport.Writers;
using SM64DSe.ImportExport.Writers.InternalWriters;
using SM64DSe.ImportExport.Loaders.ExternalLoaders;
using OpenTK;
using System.Xml;
using System.IO;
using System.Globalization;

namespace SM64DSe.ImportExport
{
    public class KCLImporter
    {
        public ModelBase m_LoadedModel;

        public KCL ConvertModelToKCL(NitroFile modelFile, string fileName, float scale, float faceSizeThreshold,
            Dictionary<string, int> matColTypes, bool save = true)
        {
            KCL importedModel = null;

            string modelFormat = fileName.Substring(fileName.Length - 3, 3).ToLower();
            switch (modelFormat)
            {
                case "obj":
                    importedModel = ConvertOBJToKCL(modelFile, fileName, scale, faceSizeThreshold, matColTypes, save);
                    break;
                case "dae":
                    importedModel = ConvertDAEToKCL(modelFile, fileName, scale, faceSizeThreshold, matColTypes, save);
                    break;
                case "imd":
                    importedModel = ConvertIMDToKCL(modelFile, fileName, scale, faceSizeThreshold, matColTypes, save);
                    break;
                default:
                    importedModel = ConvertOBJToKCL(modelFile, fileName, scale, faceSizeThreshold, matColTypes, save);
                    break;
            }

            return importedModel;
        }

        public Dictionary<string, int> GetMaterialsList(string fileName)
        {
            Dictionary<string, ModelBase.MaterialDef> materials = new Dictionary<string, ModelBase.MaterialDef>();
            string modelFormat = fileName.Substring(fileName.Length - 3, 3).ToLower();
            switch (modelFormat)
            {
                case "obj":
                    materials = new OBJLoader(fileName).GetModelMaterials();
                    break;
                case "dae":
                    materials = new DAELoader(fileName).GetModelMaterials();
                    break;
                case "imd":
                    materials = new NITROIntermediateModelDataLoader(fileName).GetModelMaterials();
                    break;
                default:
                    materials = new OBJLoader(fileName).GetModelMaterials();
                    break;
            }

            Dictionary<string, int> matColTypes = new Dictionary<string, int>();

            foreach (string key in materials.Keys)
            {
                matColTypes.Add(key, 0);
            }

            return matColTypes;
        }

        public KCL ConvertIMDToKCL(NitroFile modelFile, string fileName, float scale, float faceSizeThreshold,
            Dictionary<string, int> matColTypes, bool save = true)
        {
            KCL importedModel = new KCL(modelFile);

            if (m_LoadedModel == null)
                m_LoadedModel = new NITROIntermediateModelDataLoader(fileName).LoadModel();

            importedModel = CallKCLWriter(modelFile, m_LoadedModel, fileName, scale, faceSizeThreshold, matColTypes, save);

            return importedModel;
        }

        public KCL ConvertDAEToKCL(NitroFile modelFile, string fileName, float scale, float faceSizeThreshold,
            Dictionary<string, int> matColTypes, bool save = true)
        {
            KCL importedModel = new KCL(modelFile);

            if (m_LoadedModel == null)
                m_LoadedModel = new DAELoader(fileName).LoadModel();

            importedModel = CallKCLWriter(modelFile, m_LoadedModel, fileName, scale, faceSizeThreshold, matColTypes, save);

            return importedModel;
        }

        public KCL ConvertOBJToKCL(NitroFile modelFile, string fileName, float scale, float faceSizeThreshold,
            Dictionary<string, int> matColTypes, bool save = true)
        {
            KCL importedModel = new KCL(modelFile);

            if (m_LoadedModel == null)
                m_LoadedModel = new OBJLoader(fileName).LoadModel();

            importedModel = CallKCLWriter(modelFile, m_LoadedModel, fileName, scale, faceSizeThreshold, matColTypes, save);

            return importedModel;
        }

        protected KCL CallKCLWriter(NitroFile modelFile, ModelBase model, string fileName, float scale, float faceSizeThreshold,
            Dictionary<string, int> matColTypes, bool save = true)
        {
            AbstractModelWriter kclWriter = new KCLWriter(model, modelFile, scale, faceSizeThreshold, matColTypes);

            kclWriter.WriteModel(save);

            return new KCL(modelFile);
        }
    }
}
