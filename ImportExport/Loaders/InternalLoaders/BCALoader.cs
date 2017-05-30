/* BCALoader
 * 
 * Given a BCA object and a ModelBase object, adds animations to the ModelBase object for use in the 
 * Writer classes.
 */ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace SM64DSe.ImportExport.Loaders.InternalLoaders
{
    public class BCALoader : AbstractModelLoader
    {
        BCA m_BCA;

        public BCALoader(ModelBase model, BCA animation) :
            base(null)
        {
            m_BCA = animation;
            m_Model = model;
        }

        public override ModelBase LoadModel(float scale)
        {
            List<ModelBase.BoneDef> flatBoneList = m_Model.m_BoneTree.GetAsList();
            for (int i = 0; i < m_BCA.m_AnimationData.Length; i++)
            {
                string boneID = flatBoneList[i].m_ID;

                BCA.SRTContainer[] boneTransformations = m_BCA.GetAllLocalSRTValuesForBone(i);

                Dictionary<ModelBase.AnimationComponentType, ModelBase.AnimationComponentDataDef> animationComponentDataDefs =
                    new Dictionary<ModelBase.AnimationComponentType, ModelBase.AnimationComponentDataDef>();

                float[] valuesSx = new float[boneTransformations.Length];
                float[] valuesSy = new float[boneTransformations.Length];
                float[] valuesSz = new float[boneTransformations.Length];
                float[] valuesRx = new float[boneTransformations.Length];
                float[] valuesRy = new float[boneTransformations.Length];
                float[] valuesRz = new float[boneTransformations.Length];
                float[] valuesTx = new float[boneTransformations.Length];
                float[] valuesTy = new float[boneTransformations.Length];
                float[] valuesTz = new float[boneTransformations.Length];
                for (int j = 0; j < boneTransformations.Length; j++)
                {
                    valuesSx[j] = boneTransformations[j].m_Scale.X;
                    valuesSy[j] = boneTransformations[j].m_Scale.Y;
                    valuesSz[j] = boneTransformations[j].m_Scale.Z;
                    valuesRx[j] = boneTransformations[j].m_Rotation.X;
                    valuesRy[j] = boneTransformations[j].m_Rotation.Y;
                    valuesRz[j] = boneTransformations[j].m_Rotation.Z;
                    valuesTx[j] = boneTransformations[j].m_Translation.X;
                    valuesTy[j] = boneTransformations[j].m_Translation.Y;
                    valuesTz[j] = boneTransformations[j].m_Translation.Z;
                }

                animationComponentDataDefs.Add(ModelBase.AnimationComponentType.ScaleX,
                    new ModelBase.AnimationComponentDataDef(valuesSx, m_BCA.m_NumFrames, false, 1, false, ModelBase.AnimationComponentType.ScaleX));
                animationComponentDataDefs.Add(ModelBase.AnimationComponentType.ScaleY,
                    new ModelBase.AnimationComponentDataDef(valuesSy, m_BCA.m_NumFrames, false, 1, false, ModelBase.AnimationComponentType.ScaleY));
                animationComponentDataDefs.Add(ModelBase.AnimationComponentType.ScaleZ,
                    new ModelBase.AnimationComponentDataDef(valuesSz, m_BCA.m_NumFrames, false, 1, false, ModelBase.AnimationComponentType.ScaleZ));
                animationComponentDataDefs.Add(ModelBase.AnimationComponentType.RotateX,
                    new ModelBase.AnimationComponentDataDef(valuesRx, m_BCA.m_NumFrames, false, 1, false, ModelBase.AnimationComponentType.RotateX));
                animationComponentDataDefs.Add(ModelBase.AnimationComponentType.RotateY,
                    new ModelBase.AnimationComponentDataDef(valuesRy, m_BCA.m_NumFrames, false, 1, false, ModelBase.AnimationComponentType.RotateY));
                animationComponentDataDefs.Add(ModelBase.AnimationComponentType.RotateZ,
                    new ModelBase.AnimationComponentDataDef(valuesRz, m_BCA.m_NumFrames, false, 1, false, ModelBase.AnimationComponentType.RotateZ));
                animationComponentDataDefs.Add(ModelBase.AnimationComponentType.TranslateX,
                    new ModelBase.AnimationComponentDataDef(valuesTx, m_BCA.m_NumFrames, false, 1, false, ModelBase.AnimationComponentType.TranslateX));
                animationComponentDataDefs.Add(ModelBase.AnimationComponentType.TranslateY,
                    new ModelBase.AnimationComponentDataDef(valuesTy, m_BCA.m_NumFrames, false, 1, false, ModelBase.AnimationComponentType.TranslateY));
                animationComponentDataDefs.Add(ModelBase.AnimationComponentType.TranslateZ,
                    new ModelBase.AnimationComponentDataDef(valuesTz, m_BCA.m_NumFrames, false, 1, false, ModelBase.AnimationComponentType.TranslateZ));

                ModelBase.AnimationDef animation = new ModelBase.AnimationDef(boneID + "-animation", boneID, m_BCA.m_NumFrames, 
                    animationComponentDataDefs);
                m_Model.m_Animations.Add(animation.m_ID, animation);
            }

            return m_Model;
        }

        public override Dictionary<string, ModelBase.MaterialDef> GetModelMaterials()
        {
            return m_Model.m_Materials;
        }
    }
}
