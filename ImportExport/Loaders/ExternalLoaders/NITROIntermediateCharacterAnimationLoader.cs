/* NITROIntermediateCharacterAnimationLoader
 *
 * Given a NITRO Intermediate Character Animation (ICA) file and a ModelBase object, adds animations to the ModelBase 
 * object for use in the Writer classes.
 * 
 * The "Intermediate Character Animation" (ICA) very closely resembles the BCA format used in SM64DS. I have 
 * no idea why they felt a need to create a format as complex as NSBCA when ICA (and BCA) are so simple.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SM64DSe.ImportExport.Loaders.ExternalLoaders
{
    public class NITROIntermediateCharacterAnimationLoader : AbstractModelLoader
    {
        protected string m_ICAName;
        protected node_anm_info node_anm_info;
        protected float[] node_scale_data;
        protected float[] node_rotate_data;
        protected float[] node_translate_data;

        public NITROIntermediateCharacterAnimationLoader(ModelBase model, string animationFileName) :
            base(null)
        {
            m_Model = model;
            m_ICAName = animationFileName;
        }

        public override ModelBase LoadModel(float scale)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(m_ICAName);

            XmlNode node_anm_info = doc.SelectSingleNode("/ica/body/node_anm_info");
            ReadNodeAnmInfo(node_anm_info);

            XmlNode node_scale_data = doc.SelectSingleNode("/ica/body/node_scale_data");
            this.node_scale_data = ReadNodeTransformationData(node_scale_data);

            XmlNode node_rotate_data = doc.SelectSingleNode("/ica/body/node_rotate_data");
            this.node_rotate_data = ReadNodeTransformationData(node_rotate_data);
            ConvertRotateDataToRadians();

            XmlNode node_translate_data = doc.SelectSingleNode("/ica/body/node_translate_data");
            this.node_translate_data = ReadNodeTransformationData(node_translate_data);

            XmlNode node_anm_array = doc.SelectSingleNode("/ica/body/node_anm_array");
            ReadNodeAnimations(node_anm_array);

            m_Model.ScaleAnimations(scale);

            return m_Model;
        }

        protected void ReadNodeAnmInfo(XmlNode node_anm_info)
        {
            if (node_anm_info == null) return;

            this.node_anm_info = new node_anm_info();
            this.node_anm_info.frame_size = int.Parse(node_anm_info.Attributes["frame_size"].Value);
            this.node_anm_info.scaling_rule = node_anm_info.Attributes["scaling_rule"].Value;
            this.node_anm_info.magnify = Helper.ParseFloat(node_anm_info.Attributes["magnify"].Value);
            this.node_anm_info.tool_start_frame = int.Parse(node_anm_info.Attributes["tool_start_frame"].Value);
            this.node_anm_info.tool_end_frame = int.Parse(node_anm_info.Attributes["tool_end_frame"].Value);
            this.node_anm_info.interpolation = node_anm_info.Attributes["interpolation"].Value;
            this.node_anm_info.interp_end_to_start = (this.node_anm_info.interpolation.Equals("linear") &&
                node_anm_info.Attributes["interp_end_to_start"].Value.Equals("on"));
            this.node_anm_info.compress_node = node_anm_info.Attributes["compress_node"].Value;
            this.node_anm_info.node_size = Array.ConvertAll(
                node_anm_info.Attributes["node_size"].Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries),
                Convert.ToInt32);
            this.node_anm_info.frame_step_mode = node_anm_info.Attributes["frame_step_mode"].Value;
            this.node_anm_info.tolerance_scale = Helper.ParseFloat(node_anm_info.Attributes["tolerance_scale"].Value);
            this.node_anm_info.tolerance_rotate = Helper.ParseFloat(node_anm_info.Attributes["tolerance_rotate"].Value);
            this.node_anm_info.tolerance_translate = Helper.ParseFloat(node_anm_info.Attributes["tolerance_translate"].Value);
        }

        protected float[] ReadNodeTransformationData(XmlNode transformData)
        {
            int size = int.Parse(transformData.Attributes["size"].Value);
            if (size < 1) return new float[0];

            return Array.ConvertAll(
                transformData.InnerText.Split(new string[] { " ", "\n", "\r\n", "\t" }, StringSplitOptions.RemoveEmptyEntries),
                Convert.ToSingle);
        }

        protected void ConvertRotateDataToRadians()
        {
            for (int i = 0; i < this.node_rotate_data.Length; i++)
            {
                this.node_rotate_data[i] *= Helper.Deg2Rad;
            }
        }

        protected void ReadNodeAnimations(XmlNode node_anm_array)
        {
            if (node_anm_array == null) return;
            int size = int.Parse(node_anm_array.Attributes["size"].Value);
            if (size < 1) return;

            XmlNodeList node_anms = node_anm_array.SelectNodes("node_anm");

            foreach (XmlNode node_anm in node_anms)
            {
                int index = int.Parse(node_anm.Attributes["index"].Value);
                string boneID = m_Model.m_BoneTree.GetBoneByIndex(index).m_ID;

                XmlNode scale_x = node_anm.SelectSingleNode("scale_x");
                XmlNode scale_y = node_anm.SelectSingleNode("scale_y");
                XmlNode scale_z = node_anm.SelectSingleNode("scale_z");
                XmlNode rotate_x = node_anm.SelectSingleNode("rotate_x");
                XmlNode rotate_y = node_anm.SelectSingleNode("rotate_y");
                XmlNode rotate_z = node_anm.SelectSingleNode("rotate_z");
                XmlNode translate_x = node_anm.SelectSingleNode("translate_x");
                XmlNode translate_y = node_anm.SelectSingleNode("translate_y");
                XmlNode translate_z = node_anm.SelectSingleNode("translate_z");

                Dictionary<ModelBase.AnimationComponentType, ModelBase.AnimationComponentDataDef> animationComponentDataDefs =
                    new Dictionary<ModelBase.AnimationComponentType, ModelBase.AnimationComponentDataDef>();

                animationComponentDataDefs.Add(ModelBase.AnimationComponentType.ScaleX,
                    ReadAnimationComponent(scale_x, ModelBase.AnimationComponentType.ScaleX, index));
                animationComponentDataDefs.Add(ModelBase.AnimationComponentType.ScaleY,
                    ReadAnimationComponent(scale_y, ModelBase.AnimationComponentType.ScaleY, index));
                animationComponentDataDefs.Add(ModelBase.AnimationComponentType.ScaleZ,
                    ReadAnimationComponent(scale_z, ModelBase.AnimationComponentType.ScaleZ, index));
                animationComponentDataDefs.Add(ModelBase.AnimationComponentType.RotateX,
                    ReadAnimationComponent(rotate_x, ModelBase.AnimationComponentType.RotateX, index));
                animationComponentDataDefs.Add(ModelBase.AnimationComponentType.RotateY,
                    ReadAnimationComponent(rotate_y, ModelBase.AnimationComponentType.RotateY, index));
                animationComponentDataDefs.Add(ModelBase.AnimationComponentType.RotateZ,
                    ReadAnimationComponent(rotate_z, ModelBase.AnimationComponentType.RotateZ, index));
                animationComponentDataDefs.Add(ModelBase.AnimationComponentType.TranslateX,
                    ReadAnimationComponent(translate_x, ModelBase.AnimationComponentType.TranslateX, index));
                animationComponentDataDefs.Add(ModelBase.AnimationComponentType.TranslateY,
                    ReadAnimationComponent(translate_y, ModelBase.AnimationComponentType.TranslateY, index));
                animationComponentDataDefs.Add(ModelBase.AnimationComponentType.TranslateZ,
                    ReadAnimationComponent(translate_z, ModelBase.AnimationComponentType.TranslateZ, index));

                ModelBase.AnimationDef animationDef = new ModelBase.AnimationDef(boneID + "-animation", boneID, this.node_anm_info.frame_size,
                    animationComponentDataDefs);
                m_Model.m_Animations.Add(animationDef.m_ID, animationDef);
            }
        }

        protected ModelBase.AnimationComponentDataDef ReadAnimationComponent(XmlNode descriptor, ModelBase.AnimationComponentType componentType, 
            int boneIndex)
        {
            int frame_step = int.Parse(descriptor.Attributes["frame_step"].Value);
            int data_size = int.Parse(descriptor.Attributes["data_size"].Value);
            int data_head = int.Parse(descriptor.Attributes["data_head"].Value);
            bool isConstant = (data_size == 1);
            bool useIdentity = (isConstant && data_head == 0);

            float[] values = new float[data_size];
            if (!useIdentity)
            {
                switch (componentType)
                {
                    case ModelBase.AnimationComponentType.ScaleX:
                        Array.Copy(this.node_scale_data, data_head, values, 0, data_size);
                        break;
                    case ModelBase.AnimationComponentType.ScaleY: goto case ModelBase.AnimationComponentType.ScaleX;
                    case ModelBase.AnimationComponentType.ScaleZ: goto case ModelBase.AnimationComponentType.ScaleX;
                    case ModelBase.AnimationComponentType.RotateX:
                        Array.Copy(this.node_rotate_data, data_head, values, 0, data_size);
                        break;
                    case ModelBase.AnimationComponentType.RotateY: goto case ModelBase.AnimationComponentType.RotateX;
                    case ModelBase.AnimationComponentType.RotateZ: goto case ModelBase.AnimationComponentType.RotateX;
                    case ModelBase.AnimationComponentType.TranslateX:
                        Array.Copy(this.node_translate_data, data_head, values, 0, data_size);
                        break;
                    case ModelBase.AnimationComponentType.TranslateY: goto case ModelBase.AnimationComponentType.TranslateX;
                    case ModelBase.AnimationComponentType.TranslateZ: goto case ModelBase.AnimationComponentType.TranslateX;
                }
            }
            else
            {
                ModelBase.BoneDef boneDef = m_Model.m_BoneTree.GetBoneByIndex(boneIndex);
                switch (componentType)
                {
                    case ModelBase.AnimationComponentType.ScaleX:
                        values[0] = boneDef.m_Scale.X; break;
                    case ModelBase.AnimationComponentType.ScaleY:
                        values[0] = boneDef.m_Scale.Y; break;
                    case ModelBase.AnimationComponentType.ScaleZ:
                        values[0] = boneDef.m_Scale.Z; break;
                    case ModelBase.AnimationComponentType.RotateX:
                        values[0] = boneDef.m_Rotation.X; break;
                    case ModelBase.AnimationComponentType.RotateY:
                        values[0] = boneDef.m_Rotation.Y; break;
                    case ModelBase.AnimationComponentType.RotateZ:
                        values[0] = boneDef.m_Rotation.Z; break;
                    case ModelBase.AnimationComponentType.TranslateX:
                        values[0] = boneDef.m_Translation.X; break;
                    case ModelBase.AnimationComponentType.TranslateY:
                        values[0] = boneDef.m_Translation.Y; break;
                    case ModelBase.AnimationComponentType.TranslateZ:
                        values[0] = boneDef.m_Translation.Z; break;
                }
            }

            return new ModelBase.AnimationComponentDataDef(values, this.node_anm_info.frame_size, isConstant, frame_step, 
                useIdentity, componentType);
        }

        public override Dictionary<string, ModelBase.MaterialDef> GetModelMaterials()
        {
            return m_Model.m_Materials;
        }
    }

    public struct node_anm_info
    {
        public int frame_size;
        public string scaling_rule;
        public float magnify;
        public int tool_start_frame;
        public int tool_end_frame;
        public string interpolation;
        public bool interp_end_to_start;
        public string compress_node;
        public int[] node_size;
        public string frame_step_mode;
        public float tolerance_scale;
        public float tolerance_rotate;
        public float tolerance_translate;
    }
}
