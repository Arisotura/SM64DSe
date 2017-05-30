using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using System.Drawing;
using System.Globalization;
using System.Security.Cryptography;
using System.IO;
using SM64DSe.ImportExport;
using OpenTK.Graphics.OpenGL;

namespace SM64DSe.ImportExport.Loaders
{
    public abstract class AbstractModelLoader
    {
        public ModelBase m_Model;

        protected string m_ModelFileName;
        protected string m_ModelPath;

        public ModelBase LoadModel() { return LoadModel(1f); }
        public abstract ModelBase LoadModel(float scale);
        public abstract Dictionary<string, ModelBase.MaterialDef> GetModelMaterials();

        public AbstractModelLoader(string modelFileName)
        {
            m_Model = new ModelBase(modelFileName);

            m_ModelFileName = modelFileName;
            m_ModelPath = Path.GetDirectoryName(m_ModelFileName);
        }

        protected void AddWhiteMat()
        {
            if (m_Model.m_Materials.ContainsKey("default_white"))
                return;
            ModelBase.MaterialDef mat = new ModelBase.MaterialDef("default_white");
            m_Model.m_Materials.Add("default_white", mat);
        }

        protected void AddWhiteMat(string bone)
        {
            AddWhiteMat();
            if (!m_Model.m_BoneTree.GetBoneByID(bone).GetRoot().m_MaterialsInBranch.Contains("default_white"))
                m_Model.m_BoneTree.GetBoneByID(bone).GetRoot().m_MaterialsInBranch.Add("default_white");
            if (!m_Model.m_BoneTree.GetBoneByID(bone).m_MaterialsInBranch.Contains("default_white"))
                m_Model.m_BoneTree.GetBoneByID(bone).m_MaterialsInBranch.Add("default_white");
        }

        protected void AddTexture(ModelBase.TextureDefBase texture, ModelBase.MaterialDef matDef)
        {
            matDef.m_TextureDefID = texture.m_ID;

            IEnumerable<ModelBase.TextureDefBase> matchingHash = m_Model.m_Textures.Values.Where(
                tex0 => tex0.m_ImgHash.Equals(texture.m_ImgHash));
            if (matchingHash.Count() > 0)
            {
                matDef.m_TextureDefID = matchingHash.ElementAt(0).m_ID;
                return;
            }

            m_Model.m_Textures.Add(texture.m_ID, texture);
        }

    }
}
