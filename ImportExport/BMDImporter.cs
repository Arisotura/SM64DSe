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

namespace SM64DSe.ImportExport
{
    public class BMDImporter
    {
        public ModelBase m_LoadedModel;

        public BMD ConvertModelToBMD(ref NitroFile modelFile, string fileName, bool save = true)
        {
            return ConvertModelToBMD(ref modelFile, fileName, Vector3.One, BMDExtraImportOptions.DEFAULT, save);
        }

        public BMD ConvertModelToBMD(ref NitroFile modelFile, string fileName, Vector3 scale, BMDExtraImportOptions extraOptions, 
            bool save = true)
        {
            BMD importedModel = null;

            string modelFormat = fileName.Substring(fileName.Length - 3, 3).ToLower();
            switch (modelFormat)
            {
                case "obj":
                    importedModel = ConvertOBJToBMD(ref modelFile, fileName, scale, extraOptions, save);
                    break;
                case "dae":
                    importedModel = ConvertDAEToBMD(ref modelFile, fileName, scale, extraOptions, save);
                    break;
                case "imd":
                    importedModel = ConvertIMDToBMD(ref modelFile, fileName, scale, extraOptions, save);
                    break;
                default:
                    importedModel = ConvertOBJToBMD(ref modelFile, fileName, scale, extraOptions, save);
                    break;
            }

            return importedModel;
        }

        public BMD ConvertIMDToBMD(ref NitroFile modelFile, string fileName, bool save = true)
        {
            return ConvertIMDToBMD(ref modelFile, fileName, Vector3.One, BMDExtraImportOptions.DEFAULT, save);
        }

        public BMD ConvertIMDToBMD(ref NitroFile modelFile, string fileName, Vector3 scale, BMDExtraImportOptions extraOptions,
            bool save = true)
        {
            BMD importedModel = new BMD(modelFile);

            if (m_LoadedModel == null)
                m_LoadedModel = new NITROIntermediateModelDataLoader(fileName).LoadModel(scale);

            importedModel = CallBMDWriter(ref modelFile, m_LoadedModel, extraOptions, save);

            return importedModel;
        }

        public BMD ConvertDAEToBMD(ref NitroFile modelFile, string fileName, bool save = true)
        {
            return ConvertDAEToBMD(ref modelFile, fileName, Vector3.One, BMDExtraImportOptions.DEFAULT, save);
        }

        public BMD ConvertDAEToBMD(ref NitroFile modelFile, string fileName, Vector3 scale, BMDExtraImportOptions extraOptions, 
            bool save = true)
        {
            BMD importedModel = new BMD(modelFile);

            if (m_LoadedModel == null)
                m_LoadedModel = new DAELoader(fileName).LoadModel(scale);

            importedModel = CallBMDWriter(ref modelFile, m_LoadedModel, extraOptions, save);

            return importedModel;
        }

        public BMD ConvertOBJToBMD(ref NitroFile modelFile, string fileName, bool save = true)
        {
            return ConvertOBJToBMD(ref modelFile, fileName, Vector3.One, BMDExtraImportOptions.DEFAULT, save);
        }

        public BMD ConvertOBJToBMD(ref NitroFile modelFile, string fileName, Vector3 scale, BMDExtraImportOptions extraOptions,
            bool save = true)
        {
            BMD importedModel = new BMD(modelFile);

            if (m_LoadedModel == null)
                m_LoadedModel = new OBJLoader(fileName).LoadModel(scale);

            importedModel = CallBMDWriter(ref modelFile, m_LoadedModel, extraOptions, save);

            return importedModel;
        }

        public Dictionary<string, ModelBase.MaterialDef> GetModelMaterials(string fileName)
        {
            string modelFormat = fileName.Substring(fileName.Length - 3, 3).ToLower();
            switch (modelFormat)
            {
                case "obj":
                    return (m_LoadedModel != null) ? m_LoadedModel.m_Materials : new OBJLoader(fileName).GetModelMaterials();
                case "dae":
                    return (m_LoadedModel != null) ? m_LoadedModel.m_Materials : new DAELoader(fileName).GetModelMaterials();
                case "imd":
                    return (m_LoadedModel != null) ? m_LoadedModel.m_Materials :
                        new NITROIntermediateModelDataLoader(fileName).GetModelMaterials();
                default:
                    return (m_LoadedModel != null) ? m_LoadedModel.m_Materials : new OBJLoader(fileName).GetModelMaterials();
            }
        }

        public BCA ConvertAnimatedDAEToBCA(ref NitroFile animationFile, string fileName, bool save = true)
        {
            if (m_LoadedModel == null)
                m_LoadedModel = new DAELoader(fileName).LoadModel();

            BCA importedAnimation = CallBCAWriter(ref animationFile, m_LoadedModel, save);

            return importedAnimation;
        }

        public BCA ConvertICAToBCA(ref NitroFile animationFile, string fileName, bool save = true)
        {
            return ConvertICAToBCA(ref animationFile, fileName, Vector3.One, BMDExtraImportOptions.DEFAULT, save);
        }

        public BCA ConvertICAToBCA(ref NitroFile animationFile, string fileName, Vector3 scale,
            BMDExtraImportOptions extraOptions, bool save = true)
        {
            m_LoadedModel = new NITROIntermediateCharacterAnimationLoader(m_LoadedModel, fileName).LoadModel(scale);

            BCA importedAnimation = CallBCAWriter(ref animationFile, m_LoadedModel, save);

            return importedAnimation;
        }

        protected BMD CallBMDWriter(ref NitroFile modelFile, ModelBase model, BMDExtraImportOptions extraOptions, bool save = true)
        {
            AbstractModelWriter bmdWriter = new BMDWriter(model, ref modelFile, extraOptions);

            bmdWriter.WriteModel(save);

            return new BMD(modelFile);
        }

        protected BCA CallBCAWriter(ref NitroFile animationFile, ModelBase model, bool save = true)
        {
            AbstractModelWriter bcaWriter = new BCAWriter(model, ref animationFile);

            bcaWriter.WriteModel(save);

            return new BCA(animationFile);
        }

        public struct BMDExtraImportOptions
        {
            public bool m_ConvertToTriangleStrips;
            public bool m_KeepVertexOrderDuringStripping;
            public bool m_AlwaysWriteFullVertexCmd23h;
            public bool m_VerticallyFlipAllTextures;
            public TextureQualitySetting m_TextureQualitySetting;
            //public bool m_SwapYZ;
            //public bool m_ZMirror;

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

    }
}