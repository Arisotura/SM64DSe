using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SM64DSe.ImportExport.Writers;
using SM64DSe.ImportExport.Writers.ExternalWriters;
using SM64DSe.ImportExport.Loaders;
using SM64DSe.ImportExport.Loaders.InternalLoaders;

namespace SM64DSe.ImportExport
{
    public class BMD_BCA_KCLExporter
    {
        public static void ExportBMDModel(BMD model, string fileName)
        {
            AbstractModelLoader bmdLoader = new BMDLoader(fileName, model);

            ModelBase loadedModel = bmdLoader.LoadModel();

            ExportModel(loadedModel, fileName);
        }

        public static void ExportAnimatedModel(BMD model, BCA animation, string fileName)
        {
            AbstractModelLoader animatedLoader = new BCALoader(new BMDLoader(fileName, model).LoadModel(), animation);

            ModelBase loadedModel = animatedLoader.LoadModel();

            ExportModelToDAE(loadedModel, fileName);
        }

        public static void ExportKCLModel(KCL model, string fileName)
        {
            AbstractModelLoader kclLoader = new KCLLoader(fileName, model);

            ModelBase loadedModel = kclLoader.LoadModel();

            ExportModel(loadedModel, fileName);
        }

        public static void ExportModel(ModelBase model, string fileName)
        {
            string modelFormat = fileName.Substring(fileName.Length - 3, 3).ToLower();
            switch (modelFormat)
            {
                case "obj":
                    ExportModelToOBJ(model, fileName);
                    break;
                case "dae":
                    ExportModelToDAE(model, fileName);
                    break;
                default:
                    ExportModelToDAE(model, fileName);
                    break;
            }
        }

        public static void ExportModelToOBJ(ModelBase model, string fileName)
        {
            AbstractModelWriter objWriter = new OBJWriter(model, fileName);

            objWriter.WriteModel();
        }

        public static void ExportModelToDAE(ModelBase model, string fileName)
        {
            AbstractModelWriter daeWriter = new DAEWriter(model, fileName);

            daeWriter.WriteModel();
        }
    }
}
