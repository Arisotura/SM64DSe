/* SM64DSe allows the importing of models to BMD format.
 * Currently supported:
 * 
 * NITRO Intermediate Model Data (IMD) and Intermediate Character Animations (ICA):
 * 
 * Import the modelFile from a NITRO IMD and optionally a NITRO ICA file. Being part of the offical 
 * SDK these formats produce the best results.
 * 
 * 
 * COLLADA DAE:
 *  
 * Imports the modelFile from a COLLADA DAE modelFile complete with full joints and skinning (rigging).
 * 
 * Notes:
 * BMD does not support vertex weights, instead each vertex is only assigned to one bone - where a 
 * DAE modelFile uses multiple < 1.0 vertex weights, the largest value is used to assign the vertex to 
 * a joint.
 * 
 * This is the recommended format for importing as it matches the features of BMD almost exactly.
 * 
 * 
 * Wavefront OBJ:
 * 
 * Imports an OBJ modelFile.
 * 
 * Notes: 
 * Supports the Blender-specific "Extended OBJ" plugin's vertex colours. 
 * OBJ does not support joints so each object, "o" command is treated as a bone with a custom *.bones file 
 * used to read the hierarchy and properties.
 * 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using OpenTK.Graphics.OpenGL;
using System.Security.Cryptography;
using System.Xml;
using System.Text.RegularExpressions;
using SM64DSe.ImportExport.Loaders;
using SM64DSe.ImportExport.Writers.InternalWriters;
using SM64DSe.ImportExport.Writers;
using SM64DSe.ImportExport.Loaders.ExternalLoaders;
using SM64DSe.ImportExport.Loaders.InternalLoaders;

namespace SM64DSe.ImportExport
{
    public class BMDImporter
    {
        public static ModelBase LoadModel(string fileName, float scale)
        {
            return GetModelLoader(fileName).LoadModel(scale);
        }

        public static ModelBase LoadModel(string fileName)
        {
            return LoadModel(fileName, 1f);
        }

        public static BMD ConvertModelToBMD(ref NitroFile modelFile, string fileName, bool save = true)
        {
            return ConvertModelToBMD(ref modelFile, fileName, 1f, BMDExtraImportOptions.DEFAULT, save);
        }

        public static BMD ConvertModelToBMD(ref NitroFile modelFile, string fileName, float scale, BMDExtraImportOptions extraOptions, 
            bool save = true)
        {
            BMD importedModel = null;

            ModelBase loadedModel = GetModelLoader(fileName).LoadModel(scale);

            importedModel = CallBMDWriter(ref modelFile, loadedModel, extraOptions, save);

            return importedModel;
        }

        public static BMD ConvertIMDToBMD(ref NitroFile modelFile, string fileName, bool save = true)
        {
            return ConvertIMDToBMD(ref modelFile, fileName, 1f, BMDExtraImportOptions.DEFAULT, save);
        }

        public static BMD ConvertIMDToBMD(ref NitroFile modelFile, string fileName, float scale, BMDExtraImportOptions extraOptions,
            bool save = true)
        {
            BMD importedModel = new BMD(modelFile);

            ModelBase loadedModel = new NITROIntermediateModelDataLoader(fileName).LoadModel(scale);

            importedModel = CallBMDWriter(ref modelFile, loadedModel, extraOptions, save);

            return importedModel;
        }

        public static BMD ConvertDAEToBMD(ref NitroFile modelFile, string fileName, bool save = true)
        {
            return ConvertDAEToBMD(ref modelFile, fileName, 1f, BMDExtraImportOptions.DEFAULT, save);
        }

        public static BMD ConvertDAEToBMD(ref NitroFile modelFile, string fileName, float scale, BMDExtraImportOptions extraOptions, 
            bool save = true)
        {
            BMD importedModel = new BMD(modelFile);

            ModelBase loadedModel = new DAELoader(fileName).LoadModel(scale);

            importedModel = CallBMDWriter(ref modelFile, loadedModel, extraOptions, save);

            return importedModel;
        }

        public static BMD ConvertOBJToBMD(ref NitroFile modelFile, string fileName, bool save = true)
        {
            return ConvertOBJToBMD(ref modelFile, fileName, 1f, BMDExtraImportOptions.DEFAULT, save);
        }

        public static BMD ConvertOBJToBMD(ref NitroFile modelFile, string fileName, float scale, BMDExtraImportOptions extraOptions,
            bool save = true)
        {
            BMD importedModel = new BMD(modelFile);

            ModelBase loadedModel = new OBJLoader(fileName).LoadModel(scale);

            importedModel = CallBMDWriter(ref modelFile, loadedModel, extraOptions, save);

            return importedModel;
        }

        public static BCA ConvertAnimatedDAEToBCA(ref NitroFile animationFile, string fileName, BMDImporter.BCAImportationOptions bcaImportationOptions, bool save = true)
        {
            ModelBase loadedModel = new DAELoader(fileName).LoadModel();

            BCA importedAnimation = CallBCAWriter(ref animationFile, loadedModel, bcaImportationOptions, save);

            return importedAnimation;
        }

        public static BCA ConvertICAToBCA(ref NitroFile animationFile, string fileName, ModelBase loadedModel, bool save = true)
        {
            return ConvertICAToBCA(ref animationFile, fileName, loadedModel, 1f, 
                BMDExtraImportOptions.DEFAULT, BCAImportationOptions.DEFAULT, save);
        }

        public static BCA ConvertICAToBCA(ref NitroFile animationFile, string fileName, ModelBase loadedModel, float scale,
            BMDExtraImportOptions extraOptions, BMDImporter.BCAImportationOptions bcaImportationOptions, bool save = true)
        {
            ModelBase animatedModel = new NITROIntermediateCharacterAnimationLoader(loadedModel, fileName).LoadModel(scale);

            BCA importedAnimation = CallBCAWriter(ref animationFile, animatedModel, bcaImportationOptions, save);

            return importedAnimation;
        }

        public static BMD CallBMDWriter(ref NitroFile modelFile, ModelBase model, BMDExtraImportOptions extraOptions, bool save = true)
        {
            AbstractModelWriter bmdWriter = new BMDWriter(model, ref modelFile, extraOptions);

            bmdWriter.WriteModel(save);

            return new BMD(modelFile);
        }

        public static BCA CallBCAWriter(ref NitroFile animationFile, ModelBase model, BMDImporter.BCAImportationOptions bcaImportationOptions, bool save = true)
        {
            AbstractModelWriter bcaWriter = new BCAWriter(model, ref animationFile, bcaImportationOptions);

            bcaWriter.WriteModel(save);

            return new BCA(animationFile);
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
                case "bmd":
                    return new BMDLoader(fileName, new BMD(Program.m_ROM.GetFileFromName(fileName)));
                case "kcl":
                    return new KCLLoader(fileName, new KCL(Program.m_ROM.GetFileFromName(fileName)));
            }
        }

        public struct BMDExtraImportOptions
        {
            public bool m_ConvertToTriangleStrips;
            public bool m_KeepVertexOrderDuringStripping;
            public bool m_AlwaysWriteFullVertexCmd23h;
            public bool m_VerticallyFlipAllTextures;
            public TextureQualitySetting m_TextureQualitySetting;

            public BMDExtraImportOptions(bool convertToTriangleStrips, 
                                         bool keepVertexOrderDuringStripping,
                                         bool alwaysWriteFullVertexCmd23h,
                                         bool verticallyFlipAllTextures, 
                                         TextureQualitySetting textureQualitySetting)
            {
                m_ConvertToTriangleStrips = convertToTriangleStrips;
                m_KeepVertexOrderDuringStripping = keepVertexOrderDuringStripping;
                m_AlwaysWriteFullVertexCmd23h = alwaysWriteFullVertexCmd23h;
                m_VerticallyFlipAllTextures = verticallyFlipAllTextures;
                m_TextureQualitySetting = textureQualitySetting;
            }

            public static BMDExtraImportOptions DEFAULT = 
                new BMDExtraImportOptions(true, false, true, false, TextureQualitySetting.SmallestSize);

            public enum TextureQualitySetting 
            {
                SmallestSize, 
                BetterQualityWhereSensible, 
                BestQuality
            };
        }

        public struct BCAImportationOptions
        {
            public bool m_Optimise;

            public BCAImportationOptions(bool optimise)
            {
                m_Optimise = optimise;
            }

            public static BCAImportationOptions DEFAULT =
                new BCAImportationOptions(true);
        }
    }
}