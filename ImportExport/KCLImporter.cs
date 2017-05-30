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
using SM64DSe.ImportExport.Loaders;

namespace SM64DSe.ImportExport
{
    public class KCLImporter
    {
        public static ModelBase LoadModel(string fileName, float scale)
        {
            return GetModelLoader(fileName).LoadModel(scale);
        }

        public static ModelBase LoadModel(string fileName)
        {
            return LoadModel(fileName, 1f);
        }

        public static KCL ConvertModelToKCL(NitroFile modelFile, string fileName, float scale, float faceSizeThreshold,
            Dictionary<string, int> matColTypes, bool save = true)
        {
            KCL importedModel = null;

            ModelBase loadedModel = GetModelLoader(fileName).LoadModel(scale);

            importedModel = CallKCLWriter(modelFile, loadedModel, scale, faceSizeThreshold, matColTypes, save);

            return importedModel;
        }

        public static Dictionary<string, int> GetMaterialsList(string fileName)
        {
            Dictionary<string, ModelBase.MaterialDef> materials = GetModelLoader(fileName).GetModelMaterials();

            Dictionary<string, int> matColTypes = new Dictionary<string, int>();

            foreach (string key in materials.Keys)
            {
                matColTypes.Add(key, 0);
            }

            return matColTypes;
        }

        public static KCL ConvertIMDToKCL(NitroFile modelFile, string fileName, float scale, float faceSizeThreshold,
            Dictionary<string, int> matColTypes, bool save = true)
        {
            KCL importedModel = new KCL(modelFile);

            ModelBase loadedModel = new NITROIntermediateModelDataLoader(fileName).LoadModel();

            importedModel = CallKCLWriter(modelFile, loadedModel, scale, faceSizeThreshold, matColTypes, save);

            return importedModel;
        }

        public static KCL ConvertDAEToKCL(NitroFile modelFile, string fileName, float scale, float faceSizeThreshold,
            Dictionary<string, int> matColTypes, bool save = true)
        {
            KCL importedModel = new KCL(modelFile);

            ModelBase loadedModel = new DAELoader(fileName).LoadModel();

            importedModel = CallKCLWriter(modelFile, loadedModel, scale, faceSizeThreshold, matColTypes, save);

            return importedModel;
        }

        public static KCL ConvertOBJToKCL(NitroFile modelFile, string fileName, float scale, float faceSizeThreshold,
            Dictionary<string, int> matColTypes, bool save = true)
        {
            KCL importedModel = new KCL(modelFile);

            ModelBase loadedModel = new OBJLoader(fileName).LoadModel();

            importedModel = CallKCLWriter(modelFile, loadedModel, scale, faceSizeThreshold, matColTypes, save);

            return importedModel;
        }

        public static KCL CallKCLWriter(NitroFile modelFile, ModelBase model, float scale, float faceSizeThreshold,
            Dictionary<string, int> matColTypes, bool save = true)
        {
            AbstractModelWriter kclWriter = new KCLWriter(model, modelFile, scale, faceSizeThreshold, matColTypes);

            kclWriter.WriteModel(save);

            return new KCL(modelFile);
        }

        public static AbstractModelLoader GetModelLoader(string fileName)
        {
            string modelFormat = fileName.Substring(fileName.Length - 3, 3).ToLower();
            switch (modelFormat)
            {
                default:
                case "obj":
                    return new OBJLoader(fileName);
                case "dae":
                    return new DAELoader(fileName);
                case "imd":
                    return new NITROIntermediateModelDataLoader(fileName);
            }
        }
    }
}
