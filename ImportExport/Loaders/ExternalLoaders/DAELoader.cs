/* DAELoader
 * 
 * Using the classes produced by the XSD tool from the COLLADA 1.4.1 specification to parse COLLADA DAE models, converts a 
 * COLLADA DAE model into a ModelBase object for use by the Writer classes.
 * 
 * Supports joints, skinning and animations (see below notes) as well as plain static meshes.
 * 
 * Animation support is currently limited to models whose transformations are defined using separate scale, rotation and 
 * translation components. Models whose transformations are defined using a transformation matrix are only supported for 
 * joints with a depth of 2 (root and its child nodes only).
 * 
 * The following SM64DSe-specific <extra> tags are supported within an <effect> element:
 *	<extra>
 *		<technique profile="SM64DSe">
 *			<lights>1 0 0 0</lights>
 *			<environment_mapping>1</environment_mapping>
 *			<double_sided>0</double_sided>
 *			<tex_tiling>repeat flip</tex_tiling>
 *			<tex_scale>1.000000 1.000000</tex_scale>
 *			<tex_rotate>0.000000</tex_rotate>
 *			<tex_translate>0.000000 0.000000</tex_translate>
 *		</technique>
 *	</extra>
 * For accepted values, see IMD loader.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using System.Drawing;
using System.Xml;
using Collada141;
using System.Windows.Forms;
using System.IO;

namespace SM64DSe.ImportExport.Loaders.ExternalLoaders
{
    public class DAELoader : AbstractModelLoader
    {
        COLLADA m_COLLADAModel;

        library_images library_images;
        library_effects library_effects;
        library_materials library_materials;
        library_geometries library_geometries;
        library_controllers library_controllers;
        library_visual_scenes library_visual_scenes;
        library_animations library_animations;

        public DAELoader(string modelFileName) : 
            base(modelFileName)
        {
            m_COLLADAModel = COLLADA.Load(modelFileName);
        }

        public override ModelBase LoadModel(float scale)
        {
            foreach (var item in m_COLLADAModel.Items)
            {
                if (item.GetType().Equals(typeof(library_images))) this.library_images = item as library_images;
                if (item.GetType().Equals(typeof(library_effects))) this.library_effects = item as library_effects;
                if (item.GetType().Equals(typeof(library_materials))) this.library_materials = item as library_materials;
                if (item.GetType().Equals(typeof(library_geometries))) this.library_geometries = item as library_geometries;
                if (item.GetType().Equals(typeof(library_controllers))) this.library_controllers = item as library_controllers;
                if (item.GetType().Equals(typeof(library_visual_scenes))) this.library_visual_scenes = item as library_visual_scenes;
                if (item.GetType().Equals(typeof(library_animations))) this.library_animations = item as library_animations;
            }

            ReadMaterials();

            ReadVisualScenes();

            ReadAnimations();
            
            m_Model.ScaleModel(scale);
            m_Model.ScaleAnimations(scale);

            return m_Model;
        }

        private void ReadMaterials()
        {
            if (this.library_materials == null || this.library_materials.material.Length == 0)
            {
                AddWhiteMat();
                return;
            }

            foreach (material mat in this.library_materials.material)
            {
                string id = mat.id;
                string effectID = mat.instance_effect.url.Substring(1);

                ModelBase.MaterialDef matDef = new ModelBase.MaterialDef(id);

                ReadMaterialEffect(matDef, effectID);

                m_Model.m_Materials.Add(id, matDef);
            }

            return;
        }

        private void ReadMaterialEffect(ModelBase.MaterialDef matDef, string effectID)
        {
            effect matEffect = this.library_effects.effect.Where(eff => eff.id.Equals(effectID)).ElementAt(0);

            foreach (var profileCommon in matEffect.Items)
            {
                if (profileCommon.technique == null || profileCommon.technique.Item == null) continue;

                common_color_or_texture_type diffuse = null;
                common_color_or_texture_type ambient = null;
                common_color_or_texture_type specular = null;
                common_color_or_texture_type emission = null;
                common_float_or_param_type transparency = null;

                if ((profileCommon.technique.Item as effectFx_profile_abstractProfile_COMMONTechniquePhong) != null)
                {
                    diffuse = (profileCommon.technique.Item as effectFx_profile_abstractProfile_COMMONTechniquePhong).diffuse;
                    ambient = (profileCommon.technique.Item as effectFx_profile_abstractProfile_COMMONTechniquePhong).ambient;
                    specular = (profileCommon.technique.Item as effectFx_profile_abstractProfile_COMMONTechniquePhong).specular;
                    emission = (profileCommon.technique.Item as effectFx_profile_abstractProfile_COMMONTechniquePhong).emission;
                    transparency = (profileCommon.technique.Item as effectFx_profile_abstractProfile_COMMONTechniquePhong).transparency;
                }
                else if ((profileCommon.technique.Item as effectFx_profile_abstractProfile_COMMONTechniqueLambert) != null)
                {
                    diffuse = (profileCommon.technique.Item as effectFx_profile_abstractProfile_COMMONTechniqueLambert).diffuse;
                    ambient = (profileCommon.technique.Item as effectFx_profile_abstractProfile_COMMONTechniqueLambert).ambient;
                    emission = (profileCommon.technique.Item as effectFx_profile_abstractProfile_COMMONTechniqueLambert).emission;
                    transparency = (profileCommon.technique.Item as effectFx_profile_abstractProfile_COMMONTechniqueLambert).transparency;
                }
                else if ((profileCommon.technique.Item as effectFx_profile_abstractProfile_COMMONTechniqueBlinn) != null)
                {
                    diffuse = (profileCommon.technique.Item as effectFx_profile_abstractProfile_COMMONTechniqueBlinn).diffuse;
                    ambient = (profileCommon.technique.Item as effectFx_profile_abstractProfile_COMMONTechniqueBlinn).ambient;
                    specular = (profileCommon.technique.Item as effectFx_profile_abstractProfile_COMMONTechniqueBlinn).specular;
                    emission = (profileCommon.technique.Item as effectFx_profile_abstractProfile_COMMONTechniqueBlinn).emission;
                    transparency = (profileCommon.technique.Item as effectFx_profile_abstractProfile_COMMONTechniqueBlinn).transparency;
                }
                else if ((profileCommon.technique.Item as effectFx_profile_abstractProfile_COMMONTechniqueConstant) != null)
                {
                    emission = (profileCommon.technique.Item as effectFx_profile_abstractProfile_COMMONTechniqueConstant).emission;
                    transparency = (profileCommon.technique.Item as effectFx_profile_abstractProfile_COMMONTechniqueConstant).transparency;
                }

                if (diffuse != null && diffuse.Item != null)
                {
                    if ((diffuse.Item as common_color_or_texture_typeColor) != null)
                    {
                        var diffuseColour = diffuse.Item as common_color_or_texture_typeColor;

                        matDef.m_Diffuse = Color.FromArgb((int)(diffuseColour.Values[0] * 255f),
                            (int)(diffuseColour.Values[1] * 255f), (int)(diffuseColour.Values[2] * 255f));

                    }
                    else if ((diffuse.Item as common_color_or_texture_typeTexture) != null)
                    {
                        var diffuseTexture = diffuse.Item as common_color_or_texture_typeTexture;

                        string samplerID = diffuseTexture.texture;
                        string surfaceID = null;
                        string imageID = null;
                        if (profileCommon.Items != null)
                        {
                            foreach (var item0 in profileCommon.Items)
                            {
                                var newparam = item0 as common_newparam_type;
                                if (newparam == null || !newparam.sid.Equals(samplerID)) continue;

                                surfaceID = (newparam.Item as fx_sampler2D_common).source;
                                break;
                            }
                            
                            foreach (var item0 in profileCommon.Items)
                            {
                                var newparam = item0 as common_newparam_type;
                                if (newparam == null || !newparam.sid.Equals(surfaceID)) continue;

                                imageID = (newparam.Item as fx_surface_common).init_from[0].Value;
                                break;
                            }
                        }
                        // Sometimes models reference a non-existant image ID; if that's the case, don't throw error, 
                        // just ignore and set colour to white.
                        // Some exporters such as 3DS Max output the Image ID the texture.texture attribute. This is 
                        // wrong but I'm allowing it for convenience
                        IEnumerable<image> matchingImage;
                        if (this.library_images != null && this.library_images.image != null &&
                            (matchingImage = this.library_images.image.Where(img => img.id.Equals(imageID))).Count() > 0 ||
                            (matchingImage = this.library_images.image.Where(img => img.id.Equals(samplerID))).Count() > 0)
                        {
                            string texName = (string)matchingImage.ElementAt(0).Item;
                            if (texName.Contains(m_ModelPath))
                                texName.Replace(m_ModelPath, "");
                            ModelBase.TextureDefBase texture = new ModelBase.TextureDefExternalBitmap(
                                texName, m_ModelPath + Path.DirectorySeparatorChar + texName);
                            AddTexture(texture, matDef);
                        }
                        else
                        {
                            Console.WriteLine("Warning: Material: " + matDef.m_ID + " referenced non-existant Image with ID: " + imageID);
                        }

                        matDef.m_Diffuse = Color.White;
                    }
                }

                if (ambient != null && ambient.Item != null)
                {
                    var ambientColour = ambient.Item as common_color_or_texture_typeColor;

                    matDef.m_Ambient = Color.FromArgb((int)(ambientColour.Values[0] * 255f),
                        (int)(ambientColour.Values[1] * 255f), (int)(ambientColour.Values[2] * 255f));
                }

                if (specular != null && specular.Item != null)
                {
                    var specularColour = specular.Item as common_color_or_texture_typeColor;

                    matDef.m_Specular = Color.FromArgb((int)(specularColour.Values[0] * 255f),
                        (int)(specularColour.Values[1] * 255f), (int)(specularColour.Values[2] * 255f));
                }

                if (emission != null && emission.Item != null)
                {
                    var emissionColour = emission.Item as common_color_or_texture_typeColor;

                    matDef.m_Emission = Color.FromArgb((int)(emissionColour.Values[0] * 255f),
                        (int)(emissionColour.Values[1] * 255f), (int)(emissionColour.Values[2] * 255f));
                }

                if (transparency != null && transparency.Item != null)
                {
                    var value = (transparency.Item as common_float_or_param_typeFloat).Value;
                    matDef.m_Alpha = (byte)(value * 31f);
                }

                if (profileCommon.extra != null)
                {
                    ReadEffectExtra(matDef, profileCommon.extra);
                }

                break;
            }

            if (matEffect.extra != null)
            {
                ReadEffectExtra(matDef, matEffect.extra);
            }
        }

        private static void ReadEffectExtra(ModelBase.MaterialDef matDef, extra[] extras)
        {
            foreach (extra ext in extras)
            {
                if (ext.technique == null) continue;
                foreach (technique tnq in ext.technique)
                {
                    if (tnq.Any == null) continue;
                    foreach (XmlElement elem in tnq.Any)
                    {
                        if (elem.LocalName.ToLowerInvariant().Equals("lights"))
                        {
                            bool[] lights = new bool[4];
                            byte[] vals = Array.ConvertAll(
                                elem.InnerText.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries), Convert.ToByte);
                            for (int i = 0; i < 4; i++)
                                lights[i] = (vals[i] == 1) ? true : false;

                            matDef.m_Lights = lights;
                        }
                        else if (elem.LocalName.ToLowerInvariant().Equals("environment_mapping"))
                        {
                            if (elem.InnerText.Equals("1"))
                                matDef.m_TexGenMode = ModelBase.TexGenMode.Normal;
                        }
                        else if (elem.LocalName.ToLowerInvariant().Equals("double_sided"))
                        {
                            if (elem.InnerText.Equals("1"))
                                matDef.m_PolygonDrawingFace = ModelBase.MaterialDef.PolygonDrawingFace.FrontAndBack;
                        }
                        else if (elem.LocalName.ToLowerInvariant().Equals("tex_tiling"))
                        {
                            string[] tmpSplit = elem.InnerText.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 0; i < 2; i++)
                            {
                                switch (tmpSplit[i])
                                {
                                    case "clamp": matDef.m_TexTiling[i] = ModelBase.MaterialDef.TexTiling.Clamp; break;
                                    case "repeat": matDef.m_TexTiling[i] = ModelBase.MaterialDef.TexTiling.Repeat; break;
                                    case "flip": matDef.m_TexTiling[i] = ModelBase.MaterialDef.TexTiling.Flip; break;
                                    default: goto case "repeat";
                                }
                            }
                        }
                        else if (elem.LocalName.ToLowerInvariant().Equals("tex_scale"))
                        {
                            float[] tex_scale = Array.ConvertAll(
                                elem.InnerText.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries),
                                Convert.ToSingle);
                            matDef.m_TextureScale = new Vector2(tex_scale[0], tex_scale[1]);
                        }
                        else if (elem.LocalName.ToLowerInvariant().Equals("tex_rotation"))
                        {
                            matDef.m_TextureRotation = Helper.ParseFloat(elem.InnerText);
                        }
                        else if (elem.LocalName.ToLowerInvariant().Equals("tex_translation"))
                        {
                            float[] tex_translation = Array.ConvertAll(
                                elem.InnerText.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries),
                                Convert.ToSingle);
                            matDef.m_TextureTranslation = new Vector2(tex_translation[0], tex_translation[1]);
                        }
                        else if (elem.LocalName.ToLowerInvariant().Equals("fog_enable"))
                        {
                            matDef.m_FogFlag = elem.InnerText.Equals("1");
                        }
                    }
                }
            }
        }

        private void ReadVisualScenes()
        {
            foreach (visual_scene visualScene in this.library_visual_scenes.visual_scene)
            {
                foreach (node nodeNODE in visualScene.node.Where(node0 => node0.type.Equals(NodeType.NODE)))
                {
                    ReadNode(nodeNODE, nodeNODE, false);
                }

                break; // Only going to read first visual_scene
            }
        }

        private void ReadNode(node joint, node parent, bool inSkeleton)
        {
            string id = (joint.id != null ? joint.id : (joint.name != null ? joint.name : m_Model.m_BoneTree.Count.ToString()));

            Vector3 nodeScale = Vector3.One;
            Vector3 nodeRotation = Vector3.Zero;
            Vector3 nodeTranslation = Vector3.Zero;

            ReadNodeTransformations(joint, ref nodeScale, ref nodeRotation, ref nodeTranslation);

            if (joint.instance_geometry != null && joint.instance_geometry.Length > 0)
            {
                // Making an assumption that <instance_geometry> will never appear within a skeleton, not sure if it can?

                ModelBase.BoneDef rootBone = new ModelBase.BoneDef(id);

                rootBone.SetScale(nodeScale);
                rootBone.SetRotation(nodeRotation);
                rootBone.SetTranslation(nodeTranslation);

                m_Model.m_BoneTree.AddRootBone(rootBone);

                m_Model.m_BoneTransformsMap.Add(id, m_Model.m_BoneTree.GetBoneIndex(id));

                foreach (instance_geometry instanceGeometry in joint.instance_geometry)
                {
                    string geometryID = instanceGeometry.url.Replace("#", "");

                    Dictionary<string, string> bindMaterials = new Dictionary<string, string>();

                    if (instanceGeometry.bind_material != null)
                    {
                        foreach (instance_material instanceMaterial in instanceGeometry.bind_material.technique_common)
                        {
                            bindMaterials.Add(instanceMaterial.symbol, instanceMaterial.target.Replace("#", ""));
                        }
                    }

                    ModelBase.GeometryDef geometry = ReadGeometry(geometryID, id, bindMaterials);
                    rootBone.m_Geometries.Add(geometryID, geometry);
                }
            }
            else if (joint.instance_controller != null && joint.instance_controller.Length > 0)
            {
                // Making an assumption that <instance_controller> will never appear within a skeleton, not sure if it can?

                instance_controller instanceController = joint.instance_controller[0];

                string controllerID = instanceController.url.Replace("#", "");
                if (instanceController.skeleton != null && instanceController.skeleton.Length > 0)
                {
                    string skeletonRoot = instanceController.skeleton[0].Replace("#", "");
                    ReadSkeleton(skeletonRoot);

                    controller cntl = this.library_controllers.controller.Where(cntl0 => cntl0.id.Equals(controllerID)).ElementAt(0);
                    if (cntl.Item as skin != null)
                    {
                        // Currently there is only support for skin controllers. Where a skin uses as its source another 
                        // controller eg. a morph, we'll just recursively go through until we find a skin with a geometry 
                        // as the source and return the geometry's ID. 
                        // I've seen a skin use a skin as a source where the first skin gave joint indices of -1 from 3DS Max and 
                        // OpenCOLLADA, in cases like this, we do as above and just take the geometry ID and attach the skin being 
                        // read to that.
                        string skinSourceID = null;
                        var queue = new Queue<controller>();
                        queue.Enqueue(cntl);
                        while (queue.Count > 0)
                        {
                            controller cont = queue.Dequeue();
                            string srcID = (cont.Item as skin).source1.Replace("#", "");
                            if (this.library_geometries != null &&
                                this.library_geometries.geometry.Where(geom0 => geom0.id.Equals(srcID)).Count() < 1)
                            {
                                IEnumerable<controller> res = this.library_controllers.controller.Where(cont0 => cont0.id.Equals(srcID));
                                if (res.Count() > 0)
                                {
                                    queue.Enqueue(res.ElementAt(0));
                                }
                            }
                            else
                            {
                                skinSourceID = srcID;
                            }
                        }

                        int[] vertexBoneIDs = ReadSkinController(controllerID, skeletonRoot, skinSourceID);

                        Dictionary<string, string> bindMaterials = new Dictionary<string, string>();
                        if (instanceController.bind_material != null)
                        {
                            foreach (instance_material instanceMaterial in instanceController.bind_material.technique_common)
                            {
                                if (!bindMaterials.ContainsKey(instanceMaterial.symbol))
                                {
                                    bindMaterials.Add(instanceMaterial.symbol, instanceMaterial.target.Substring(1));
                                }
                            }
                        }

                        ModelBase.GeometryDef geomDef = ReadGeometry(skinSourceID, skeletonRoot, bindMaterials, vertexBoneIDs);
                        m_Model.m_BoneTree.GetBoneByID(skeletonRoot).m_Geometries.Add(skinSourceID, geomDef);
                    }
                }
            }
            else if (inSkeleton)
            {
                ModelBase.BoneDef boneDef = new ModelBase.BoneDef(joint.id);

                if (joint == parent)
                {
                    m_Model.m_BoneTree.AddRootBone(boneDef);
                }
                else
                {
                    ModelBase.BoneDef parentBone = m_Model.m_BoneTree.GetBoneByID(parent.id);
                    parentBone.AddChild(boneDef);
                }

                boneDef.SetScale(nodeScale);
                boneDef.SetRotation(nodeRotation);
                boneDef.SetTranslation(nodeTranslation);

                if (joint.node1 == null || joint.node1.Length == 0)
                    return;
                foreach (node child in joint.node1)
                {
                    if (child.type.Equals(NodeType.NODE))
                    {
                        Console.WriteLine("Warning: node: " + joint.id + " has a child of type \"NODE\" within a skeleton, failure likely");
                    }

                    ReadNode(child, joint, true);
                }
            }
        }

        private void ReadSkeleton(string rootNodeID)
        {
            node skeletonRoot = null;
            node skeletonRootParent = null;

            foreach (visual_scene visualScene in this.library_visual_scenes.visual_scene)
            {
                foreach (node node0 in visualScene.node)
                {
                    node result = FindNodeInTree(node0, rootNodeID);
                    if (result != null)
                    {
                        skeletonRoot = result;

                        if (result != node0)
                        {
                            bool foundRoot = IsRootNodeOfChild(node0, skeletonRoot);
                            if (foundRoot)
                                skeletonRootParent = node0;
                        }

                        break;
                    }
                }
            }

            ReadNode(skeletonRoot, skeletonRoot, true);

            m_Model.m_BoneTree.GetBoneByID(skeletonRoot.id).CalculateBranchTransformations();
        }

        private int[] ReadSkinController(string id, string skeletonRoot, string geometryID)
        {
            controller controller = this.library_controllers.controller.Where(cntl => cntl.id.Equals(id)).ElementAt(0);

            if (controller.Item as skin == null)
                return null;

            skin skin = controller.Item as skin;

            string[] jointNames = new string[0];
            Matrix4[] inverseBindPoses = new Matrix4[0];

            foreach (InputLocal input in skin.joints.input)
            {
                if (input.semantic.Equals("JOINT"))
                {
                    string sourceID = input.source.Replace("#", "");
                    source jointNamesSource = skin.source.Where(src => src.id.Equals(sourceID)).ElementAt(0);
                    if (jointNamesSource.Item as Name_array != null)
                    {
                        jointNames = (jointNamesSource.Item as Name_array).Values;
                        for (int i = 0; i < jointNames.Length; i++)
                        {
                            string jointID = FindIDFromSIDInSkeleton(skeletonRoot, jointNames[i]);
                            jointNames[i] = jointID;
                        }
                    }
                    else if (jointNamesSource.Item as IDREF_array != null)
                    {
                        jointNames = (jointNamesSource.Item as IDREF_array).Value.
                            Split(Strings.WHITESPACE, StringSplitOptions.RemoveEmptyEntries);
                    }
                }
                else if (input.semantic.Equals("INV_BIND_MATRIX"))
                {
                    string sourceID = input.source.Replace("#", "");
                    source invBindMatrixSource = skin.source.Where(src => src.id.Equals(sourceID)).ElementAt(0);
                    if (invBindMatrixSource.Item as float_array != null)
                    {
                        float[] vals = Array.ConvertAll<double, float>((invBindMatrixSource.Item as float_array).Values, Convert.ToSingle);
                        if (invBindMatrixSource.technique_common != null && invBindMatrixSource.technique_common.accessor != null)
                        {
                            accessor acc = invBindMatrixSource.technique_common.accessor;
                            ulong count = acc.count;
                            ulong stride = acc.stride;
                            inverseBindPoses = new Matrix4[count];

                            int matrixIndex = 0;
                            for (ulong i = 0; i < (count * stride); i += stride, matrixIndex++)
                            {
                                float[] tmp = new float[16];
                                Array.Copy(vals, (int)i, tmp, 0, 16);
                                Matrix4 invBindMatrix = Helper.FloatArrayToMatrix4(tmp);
                                inverseBindPoses[matrixIndex] = invBindMatrix;
                            }
                        }
                    }
                }
            }

            if (jointNames.Length > 31)
            {
                Console.WriteLine("DAELoader: WARN: More than 31 matrices, your model will not import correctly.");
            }

            for (int i = 0; i < jointNames.Length; i++)
            {
                Queue<ModelBase.BoneDef> hierarchy = new Queue<ModelBase.BoneDef>();
                ModelBase.BoneDef child = m_Model.m_BoneTree.GetBoneByID(jointNames[i]);
                if (child == null)
                {
                    Console.WriteLine("DAELoader: WARN: Skin controller ["+ id + "] contains reference to joint name [" +
                        jointNames[i] + "] before joint has been read, skipping processing but failure likely");
                    continue;
                }
                hierarchy.Enqueue(child);
                while (child.m_Parent != null)
                {
                    if (!m_Model.m_BoneTransformsMap.GetFirstToSecond().ContainsKey(child.m_Parent.m_ID))
                    {
                        hierarchy.Enqueue(child.m_Parent);
                        child = child.m_Parent;
                    }
                    else
                    {
                        break;
                    }
                }

                while (hierarchy.Count > 0)
                {
                    ModelBase.BoneDef current = hierarchy.Dequeue();
                    if (!m_Model.m_BoneTransformsMap.GetFirstToSecond().ContainsKey(current.m_ID))
                    {
                        m_Model.m_BoneTransformsMap.Add(current.m_ID, m_Model.m_BoneTransformsMap.Count);
                    }
                }
            }

            //for (int i = 0; i < jointNames.Length; i++)
            //{
            //    m_Model.m_BoneTree.GetBoneByID(jointNames[i]).m_GlobalInverseTransformation = inverseBindPoses[i];
            //}

            float[] weights = new float[0];
            long offsetJoint = -1;
            long offsetWeight = -1;
            int[] vcount = new int[0];
            int[] v = new int[0];

            int[] vertexBoneIDs = new int[skin.vertex_weights.count];

            foreach (InputLocalOffset input in skin.vertex_weights.input)
            {
                if (input.semantic.Equals("JOINT"))
                {
                    offsetJoint = (long)input.offset;
                }
                else if (input.semantic.Equals("WEIGHT"))
                {
                    offsetWeight = (long)input.offset;

                    string sourceID = input.source.Replace("#", "");
                    source weightSource = skin.source.Where(src => src.id.Equals(sourceID)).ElementAt(0);
                    if (weightSource.Item as float_array != null)
                    {
                        weights = Array.ConvertAll<double, float>((weightSource.Item as float_array).Values, Convert.ToSingle);
                    }
                }
            }

            vcount = Array.ConvertAll<string, int>(
                skin.vertex_weights.vcount.Split(Strings.WHITESPACE, StringSplitOptions.RemoveEmptyEntries),
                Convert.ToInt32);

            v = Array.ConvertAll<string, int>(
                skin.vertex_weights.v.Split(Strings.WHITESPACE, StringSplitOptions.RemoveEmptyEntries),
                Convert.ToInt32);
            int currentVertexIndex = 0;

            // For each vertex, <v> contains n number of joint ID-weight pairs where n is the value specified in <vcount> 
            // for the current vertex. BMD does not support weights, instead each vertex is assigned to a single joint. 
            // When there is more than one weight for a vertex, the vertex is assigned to the joint with the highest weighting:
            // eg. <v>0 0 1 1 2 3 3 5</v>
            // contains the following pairs, (0,0), (1,1), (2,3), (3,5) for a vertex
            // where the first index is an index into the joint names source > bone0, bone1, bone2, bone3
            // and the second index is an index into the weights source > 0.5, 0.25, 0.15, 0.1
            // In this case, the first pair, (0,0) has the highest weighting so this vertex will be assigned to bone0

            List<string> boneIDList = m_Model.m_BoneTree.GetBoneIDList();
            for (int i = 0; i < v.Length; )
            {
                int numInfluences = vcount[currentVertexIndex];
                List<Tuple<int, float>> influences = new List<Tuple<int, float>>();

                for (int j = 0; j < numInfluences; j++)
                {
                    int indexJoint = v[i + (j * 2) + offsetJoint];
                    int indexWeight = v[i + (j * 2) + offsetWeight];

                    int mappedBoneID = boneIDList.IndexOf(jointNames[indexJoint]);
                    float weight = weights[indexWeight];

                    influences.Add(new Tuple<int, float>(mappedBoneID, weight));
                }

                int highestWeightJoint = 0;
                float highestWeightValue = 0.0f;
                for (int j = 0; j < numInfluences; j++)
                {
                    if (influences[j].Item2 > highestWeightValue)
                    {
                        highestWeightValue = influences[j].Item2;
                        highestWeightJoint = influences[j].Item1;
                    }
                }

                vertexBoneIDs[currentVertexIndex] = highestWeightJoint;

                i += (vcount[currentVertexIndex] * 2);
                currentVertexIndex++;
            }

            return vertexBoneIDs;
        }

        private static void ReadNodeTransformations(node nodeNODE, ref Vector3 nodeScale, ref Vector3 nodeRotation, ref Vector3 nodeTranslation)
        {
            List<Tuple<Vector3, TransformationType>> transforms = new List<Tuple<Vector3, TransformationType>>();
            bool matrixBacked = false;
            for (int i = 0; i < ((nodeNODE.Items != null) ? nodeNODE.Items.Length : 0); i++)
            {
                var item = nodeNODE.Items[i];

                if (nodeNODE.ItemsElementName[i].Equals(ItemsChoiceType2.matrix))
                {
                    Matrix4 nodeMatrix = Helper.DoubleArrayToMatrix4((item as matrix).Values);
                    Helper.DecomposeSRTMatrix2(nodeMatrix, out nodeScale, out nodeRotation, out nodeTranslation);
                    matrixBacked = true;
                }
                else
                {
                    if (nodeNODE.ItemsElementName[i].Equals(ItemsChoiceType2.translate))
                    {
                        var translate = item as TargetableFloat3;
                        Vector3 tmp = new Vector3((float)translate.Values[0], (float)translate.Values[1], (float)translate.Values[2]);
                        transforms.Add(new Tuple<Vector3, TransformationType>(tmp, TransformationType.TranslationXYZ));
                    }
                    else if (nodeNODE.ItemsElementName[i].Equals(ItemsChoiceType2.rotate))
                    {
                        /*
                         * <rotate sid="rotateZ">0 0 1 0</rotate>
                         * <rotate sid="rotateY">0 1 0 45</rotate>
                         * <rotate sid="rotateX">1 0 0 0</rotate>
                         */
                        var rotate = item as rotate;
                        TransformationType rotType = GetRotationType(rotate);
                        Vector3 rot = Vector3.Zero;
                        switch (rotType)
                        {
                            case TransformationType.RotationX:
                                rot = new Vector3((float)(rotate.Values[3] * Helper.Deg2Rad), 0.0f, 0.0f);
                                break;
                            case TransformationType.RotationY:
                                rot = new Vector3(0.0f, (float)(rotate.Values[3] * Helper.Deg2Rad), 0.0f);
                                break;
                            case TransformationType.RotationZ:
                                rot = new Vector3(0.0f, 0.0f, (float)(rotate.Values[3] * Helper.Deg2Rad));
                                break;
                        }
                        transforms.Add(new Tuple<Vector3, TransformationType>(rot, rotType));
                    }
                    else if (nodeNODE.ItemsElementName[i].Equals(ItemsChoiceType2.scale))
                    {
                        var scale = item as TargetableFloat3;
                        Vector3 tmp = new Vector3((float)scale.Values[0], (float)scale.Values[1], (float)scale.Values[2]);
                        transforms.Add(new Tuple<Vector3, TransformationType>(tmp, TransformationType.ScaleXYZ));
                    }
                }
            }
            if (!matrixBacked)
            {
                // If matrix not used, multiply each of the transformations in the reverse of the order they appear.
                // IMPORTANT NOTE: The order must be Scale, Rotation, Translation (appear in file TRzRyRxS)
                List<Tuple<Vector3, TransformationType>> scale =
                    transforms.Where(tran => tran.Item2.Equals(TransformationType.ScaleXYZ)).ToList();
                List<Tuple<Vector3, TransformationType>> rotate =
                    transforms.Where(tran => tran.Item2.Equals(TransformationType.RotationX) ||
                        tran.Item2.Equals(TransformationType.RotationY) || tran.Item2.Equals(TransformationType.RotationZ)).ToList();
                List<Tuple<Vector3, TransformationType>> translate =
                    transforms.Where(tran => tran.Item2.Equals(TransformationType.TranslationXYZ)).ToList();

                if (scale.Count <= 1 && rotate.Count <= 3 && translate.Count <= 1)
                {
                    // Making an assumption that the order is SRT if less than 5 tranformations, if it's greater than 
                    // 5 or there are 5 but not in the order in above condition, use matrix decomposition method.
                    nodeScale = (scale.Count == 1) ? scale[0].Item1 : nodeScale;
                    foreach (Tuple<Vector3, TransformationType> rot in rotate)
                    {
                        if (rot.Item2 == TransformationType.RotationX)
                        {
                            nodeRotation.X = rot.Item1.X;
                        }
                        if (rot.Item2 == TransformationType.RotationY)
                        {
                            nodeRotation.Y = rot.Item1.Y;
                        }
                        if (rot.Item2 == TransformationType.RotationZ)
                        {
                            nodeRotation.Z = rot.Item1.Z;
                        }
                    }
                    nodeTranslation = (translate.Count == 1) ? translate[0].Item1 : nodeTranslation;
                }
                else
                {
                    // If not we need to multiply the matrices and then decompose the final matrix
                    Matrix4 result = Matrix4.Identity;
                    for (int i = transforms.Count - 1; i >= 0; i--)
                    {
                        Tuple<Vector3, TransformationType> current = transforms.ElementAt(i);
                        Vector3 vec = current.Item1;
                        switch (current.Item2)
                        {
                            case TransformationType.ScaleXYZ:
                                Matrix4 mscale = Matrix4.CreateScale(vec);
                                Matrix4.Mult(ref result, ref mscale, out result);
                                break;
                            case TransformationType.RotationX:
                                Matrix4 mxrot = Matrix4.CreateRotationX(vec.X);
                                Matrix4.Mult(ref result, ref mxrot, out result);
                                break;
                            case TransformationType.RotationY:
                                Matrix4 myrot = Matrix4.CreateRotationY(vec.Y);
                                Matrix4.Mult(ref result, ref myrot, out result);
                                break;
                            case TransformationType.RotationZ:
                                Matrix4 mzrot = Matrix4.CreateRotationZ(vec.Z);
                                Matrix4.Mult(ref result, ref mzrot, out result);
                                break;
                            case TransformationType.TranslationXYZ:
                                Matrix4 mtrans = Matrix4.CreateTranslation(vec);
                                Matrix4.Mult(ref result, ref mtrans, out result);
                                break;
                        }
                    }

                    Helper.DecomposeSRTMatrix2(result, out nodeScale, out nodeRotation, out nodeTranslation);
                }
            }
        }

        private static TransformationType GetRotationType(rotate rotate)
        {
            int ind = -1;
            for (int j = 0; j < 3; j++)
            {
                if (rotate.Values[j] == 1.0)
                {
                    ind = j;
                    break;
                }
            }
            switch (ind)
            {
                case 0:
                    return TransformationType.RotationX;
                case 1:
                    return TransformationType.RotationY;
                case 2:
                    return TransformationType.RotationZ;
            }
            return TransformationType.None;
        }

        private ModelBase.GeometryDef ReadGeometry(string id, string boneID, Dictionary<string, string> bindMaterials)
        {
            return ReadGeometry(id, boneID, bindMaterials, null);
        }

        private ModelBase.GeometryDef ReadGeometry(string id, string boneID, Dictionary<string, string> bindMaterials, int[] vertexBoneIDs)
        {
            ModelBase.GeometryDef geomDef = new ModelBase.GeometryDef(id);
            int boneIndex = m_Model.m_BoneTree.GetBoneIndex(boneID);
            geometry geom = library_geometries.geometry.Where(geom0 => geom0.id.Equals(id)).ElementAt(0);

            Dictionary<string, source> sources = new Dictionary<string, source>();

            if (geom.Item as mesh != null)
            {
                mesh geomMesh = geom.Item as mesh;
                if (geomMesh.source == null || geomMesh.Items == null) return geomDef;
                Dictionary<string, string> geometryVertices = new Dictionary<string,string>();
                geometryVertices.Add(geomMesh.vertices.id, 
                    geomMesh.vertices.input.Where(input0 => input0.semantic.Equals("POSITION")).ElementAt(0).source.Replace("#", ""));

                foreach (source src in geomMesh.source)
                {
                    string sourceID = src.id;
                    if (src.Item as float_array != null)
                        sources.Add(sourceID, src);
                }
                foreach (var item in geomMesh.Items)
                {
                    if ((item as triangles != null) || (item as polylist != null) || (item as polygons != null) || 
                        (item as tristrips != null))
                    {
                        ModelBase.PolyListDef polyListDef;
                        string material = null;
                        ulong count = 0;
                        InputLocalOffset[] inputs = new InputLocalOffset[0];
                        int[] vcount = new int[0];
                        List<int[]> p = new List<int[]>();
                        ModelBase.PolyListType polyListType = ModelBase.PolyListType.Polygons;

                        if (item as triangles != null)
                        {
                            triangles tris = item as triangles;
                            polyListType = ModelBase.PolyListType.Triangles;
                            string matAttr = (tris.material != null) ? tris.material : "default_white";
                            material = (bindMaterials != null && bindMaterials.Count > 0 && bindMaterials.ContainsKey(matAttr)) ?
                                bindMaterials[matAttr] : matAttr;
                            count = tris.count;
                            if (count > 0)
                            {
                                inputs = tris.input;
                                vcount = new int[] { 3 };
                                p.Add(Array.ConvertAll<string, int>
                                    (tris.p.Split(Strings.WHITESPACE, StringSplitOptions.RemoveEmptyEntries), Convert.ToInt32));
                            }
                        }
                        else if (item as polylist != null)
                        {
                            polylist plist = item as polylist;
                            polyListType = ModelBase.PolyListType.Polygons;
                            string matAttr = (plist.material != null) ? plist.material : "default_white";
                            material = (bindMaterials != null && bindMaterials.Count > 0 && bindMaterials.ContainsKey(matAttr)) ?
                                bindMaterials[matAttr] : matAttr;
                            count = plist.count;
                            if (count > 0)
                            {
                                inputs = plist.input;
                                vcount = Array.ConvertAll<string, int>
                                    (plist.vcount.Split(Strings.WHITESPACE, StringSplitOptions.RemoveEmptyEntries), Convert.ToInt32);
                                p.Add(Array.ConvertAll<string, int>
                                    (plist.p.Split(Strings.WHITESPACE, StringSplitOptions.RemoveEmptyEntries), Convert.ToInt32));
                            }
                        }
                        else if (item as polygons != null)
                        {
                            polygons pgons = item as polygons;
                            polyListType = ModelBase.PolyListType.Polygons;
                            string matAttr = (pgons.material != null) ? pgons.material : "default_white";
                            material = (bindMaterials != null && bindMaterials.Count > 0 && bindMaterials.ContainsKey(matAttr)) ?
                                bindMaterials[matAttr] : matAttr;
                            count = pgons.count;
                            if (count > 0)
                            {
                                inputs = pgons.input;
                                vcount = new int[count];
                                int[] pTmp = new int[0];
                                int counter = 0;
                                for (int i = 0; i < pgons.Items.Length; i++)
                                {
                                    var element = pgons.Items[i];
                                    if (element as string != null)
                                    {
                                        int[] tmp = Array.ConvertAll<string, int>
                                            ((element as string).Split(Strings.WHITESPACE, StringSplitOptions.RemoveEmptyEntries),
                                            Convert.ToInt32);
                                        vcount[i] = tmp.Length / inputs.Length;
                                        Array.Resize(ref pTmp, pTmp.Length + (vcount[i] * inputs.Length));
                                        Array.Copy(tmp, 0, pTmp, counter, tmp.Length);
                                        counter += tmp.Length;
                                    }
                                }
                                p.Add(pTmp);
                            }
                        }
                        else if (item as tristrips != null)
                        {
                            tristrips tristrips = item as tristrips;
                            polyListType = ModelBase.PolyListType.TriangleStrip;
                            string matAttr = (tristrips.material != null) ? tristrips.material : "default_white";
                            material = (bindMaterials != null && bindMaterials.Count > 0 && bindMaterials.ContainsKey(matAttr)) ?
                                bindMaterials[matAttr] : matAttr;
                            count = tristrips.count;
                            if (count > 0)
                            {
                                inputs = tristrips.input;
                                vcount = new int[] { 3 };
                                // Go through <p> elements and convert it so the format is similar to <polylist> for parsing below
                                // Eg. given: (0,(1,(2),(3),4),5)
                                // convert to separate triangles: (0,1,2),(1,2,3),(2,3,4),(3,4,5)
                                // These will be converted back to triangle strips when writing the BMD model
                                for (int i = 0; i < tristrips.p.Length; i++)
                                {
                                    var element = tristrips.p[i];
                                    if (element as string != null)
                                    {
                                        int[] tmp = Array.ConvertAll<string, int>
                                            ((element as string).Split(Strings.WHITESPACE, StringSplitOptions.RemoveEmptyEntries),
                                            Convert.ToInt32);
                                        int numTris = ((tmp.Length / inputs.Length) - 3) + 1;
                                        int numVertsToTris = numTris * 3;
                                        int[] tmpConv = new int[numVertsToTris * inputs.Length];
                                        Array.Copy(tmp, tmpConv, (3 * inputs.Length));
                                        if (tmp.Length > (3 * inputs.Length))
                                        {
                                            int startInd = 3 * inputs.Length;
                                            for (int sourceInd = startInd, destInd = startInd; sourceInd < tmp.Length;
                                                sourceInd += inputs.Length, destInd += (3 * inputs.Length))
                                            {
                                                Array.Copy(tmp, sourceInd - (2 * inputs.Length), tmpConv, destInd, (3 * inputs.Length));
                                            }
                                        }
                                        p.Add(tmpConv);
                                    }
                                }
                            }
                        }

                        if (material.Equals("default_white") && !m_Model.m_Materials.ContainsKey("default_white"))
                        {
                            AddWhiteMat();
                        }

                        polyListDef = new ModelBase.PolyListDef("pl_" + geomDef.m_PolyLists.Count, material);

                        // The parent (root) bone should have a list of all materials used by itself and its children
                        if (!m_Model.m_BoneTree.GetBoneByID(boneID).GetRoot().m_MaterialsInBranch.Contains(material))
                            m_Model.m_BoneTree.GetBoneByID(boneID).GetRoot().m_MaterialsInBranch.Add(material);
                        if (!m_Model.m_BoneTree.GetBoneByID(boneID).m_MaterialsInBranch.Contains(material))
                            m_Model.m_BoneTree.GetBoneByID(boneID).m_MaterialsInBranch.Add(material);

                        if (count == 0) continue;

                        ModelBase.MaterialDef currentMaterial = m_Model.m_Materials[material];

                        int maxOffset = -1;
                        int vertexOffset = -1, normalOffset = -1, texCoordOffset = -1, colourOffset = -1;
                        string vertexSource = "", normalSource = "", texCoordSource = "", colourSource = "";
                        foreach (InputLocalOffset input in inputs)
                        {
                            if (input.semantic.Equals("VERTEX"))
                            {
                                vertexOffset = (int)input.offset;
                                vertexSource = geometryVertices[input.source.Replace("#", "")];
                            }
                            else if (input.semantic.Equals("NORMAL"))
                            {
                                if (currentMaterial.m_Lights.Contains(true))
                                {
                                    normalOffset = (int)input.offset;
                                    normalSource = input.source.Replace("#", "");
                                }
                            }
                            else if (input.semantic.Equals("TEXCOORD"))
                            {
                                texCoordOffset = (int)input.offset;
                                texCoordSource = input.source.Replace("#", "");
                            }
                            else if (input.semantic.Equals("COLOR"))
                            {
                                colourOffset = (int)input.offset;
                                colourSource = input.source.Replace("#", "");
                            }

                            if ((int)input.offset > maxOffset) { maxOffset = (int)input.offset; }
                        }
                        int inputCount = maxOffset + 1;

                        foreach (int[] pArr in p)
                        {
                            ModelBase.FaceListDef faceList = new ModelBase.FaceListDef(polyListType);

                            bool even = true;
                            for (ulong pIndex = 0, vcountInd = 0; pIndex < (ulong)pArr.Length; vcountInd++)
                            {
                                ModelBase.FaceDef faceDef = new ModelBase.FaceDef(
                                    vcount[(polyListType.Equals(ModelBase.PolyListType.Triangles) || 
                                    polyListType.Equals(ModelBase.PolyListType.TriangleStrip)) ? 0 : vcountInd]);
                                List<ModelBase.VertexDef> vertices = new List<ModelBase.VertexDef>();

                                for (int i = 0; i < faceDef.m_NumVertices; i++)
                                {
                                    ModelBase.VertexDef vert = ModelBase.EMPTY_VERTEX;

                                    int vertexIndex = pArr[pIndex + (ulong)vertexOffset];
                                    float[] tmp = GetValueFromFloatArraySource(sources[vertexSource], vertexIndex);
                                    vert.m_Position = new Vector3(tmp[0], tmp[1], tmp[2]);

                                    if (normalOffset != -1)
                                    {
                                        tmp = GetValueFromFloatArraySource(sources[normalSource], pArr[pIndex + (ulong)normalOffset]);
                                        vert.m_Normal = new Vector3(tmp[0], tmp[1], tmp[2]).Normalized();
                                    }
                                    else
                                    {
                                        vert.m_Normal = null;
                                    }

                                    if (texCoordOffset != -1 && m_Model.m_Materials[material].m_TextureDefID != null)
                                    {
                                        tmp = GetValueFromFloatArraySource(sources[texCoordSource], pArr[pIndex + (ulong)texCoordOffset]);
                                        vert.m_TextureCoordinate = new Vector2(tmp[0], tmp[1]);
                                    }
                                    else
                                    {
                                        vert.m_TextureCoordinate = null;
                                    }

                                    if (colourOffset != -1)
                                    {
                                        tmp = GetValueFromFloatArraySource(sources[colourSource], pArr[pIndex + (ulong)colourOffset]);
                                        vert.m_VertexColour = Color.FromArgb((int)(tmp[0] * 255f),
                                            (int)(tmp[1] * 255f), (int)(tmp[2] * 255f));
                                    }
                                    else
                                    {
                                        vert.m_VertexColour = Color.White;
                                    }

                                    vert.m_VertexBoneIndex = (vertexBoneIDs != null) ? vertexBoneIDs[vertexIndex] : boneIndex;

                                    vertices.Add(vert);

                                    pIndex += (ulong)inputCount;
                                }

                                if (polyListType.Equals(ModelBase.PolyListType.TriangleStrip))
                                {
                                    if (even)
                                    {
                                        for (int v = 0; v < vertices.Count; v++)
                                            faceDef.m_Vertices[v] = vertices[v];
                                    }
                                    else
                                    {
                                        for (int v = 0; v < vertices.Count; v++)
                                            faceDef.m_Vertices[2 - v] = vertices[v];
                                    }
                                    even = !even;
                                }
                                else
                                {
                                    for (int v = 0; v < vertices.Count; v++)
                                        faceDef.m_Vertices[v] = vertices[v];
                                }

                                faceList.m_Faces.Add(faceDef);
                            }

                            polyListDef.m_FaceLists.Add(faceList);
                        }

                        geomDef.m_PolyLists.Add("pl_" + geomDef.m_PolyLists.Count, polyListDef);
                    }
                }
            }

            return geomDef;
        }

        private const float FRAMES_PER_SECOND = 30.0f;
        private const float INTERVAL = 1.0f / FRAMES_PER_SECOND;

        private void ReadAnimations()
        {
            if (this.library_animations == null)
                return;

            if (this.library_animations.animation != null && this.library_animations.animation.Length > 0)
            {
                Dictionary<string, ModelBase.AnimationDef> boneAnimations = new Dictionary<string, ModelBase.AnimationDef>();

                Queue<animation> animations = new Queue<animation>();
                foreach (animation anim in this.library_animations.animation)
                    animations.Enqueue(anim);

                while (animations.Count > 0)
                {
                    animation anim = animations.Dequeue();

                    string id = anim.id;

                    List<source> animSources = new List<source>();
                    foreach (var item in anim.Items.Where(item0 => item0.GetType().Equals(typeof(source))))
                        animSources.Add(item as source);
                    List<sampler> animSamplers = new List<sampler>();
                    foreach (var item in anim.Items.Where(item0 => item0.GetType().Equals(typeof(sampler))))
                        animSamplers.Add(item as sampler);
                    List<channel> animChannels = new List<channel>();
                    foreach (var item in anim.Items.Where(item0 => item0.GetType().Equals(typeof(channel))))
                        animChannels.Add(item as channel);

                    foreach (channel channel in animChannels)
                    {
                        string samplerID = channel.source.Replace("#", "");
                        sampler channelSampler = animSamplers.Where(samp => samp.id.Equals(samplerID)).ElementAt(0);

                        string inputSourceID =
                            channelSampler.input.Where(input => input.semantic.Equals("INPUT")).ElementAt(0).source.Replace("#", "");
                        string outputSourceID =
                            channelSampler.input.Where(input => input.semantic.Equals("OUTPUT")).ElementAt(0).source.Replace("#", "");

                        float[] inputTime = Array.ConvertAll<double, float>(
                            (animSources.Where(src => src.id.Equals(inputSourceID)).ElementAt(0).Item as float_array).Values,
                            Convert.ToSingle);

                        TransformationType animType = TransformationType.None;

                        string targetPath = channel.target;
                        string targetNodeID = targetPath.Substring(0, targetPath.IndexOf("/"));
                        string targetTransformationSID = targetPath.Substring(targetPath.IndexOf("/") + 1);

                        string targetParam = (targetTransformationSID.IndexOf('.') != -1) ?
                            targetTransformationSID.Substring(targetTransformationSID.IndexOf('.') + 1) : null;
                        targetTransformationSID = (targetParam != null) ? targetTransformationSID.Replace("." + targetParam, "") :
                            targetTransformationSID;

                        string boneID = targetNodeID;

                        node targetNode = FindNodeInLibraryVisualScenes(targetNodeID);
                        for (int i = 0; i < targetNode.Items.Length; i++)
                        {
                            var item = targetNode.Items[i];

                            if (item.GetType().Equals(typeof(matrix)) &&
                                ((item as matrix).sid != null && (item as matrix).sid.Equals(targetTransformationSID)))
                            {
                                animType = TransformationType.TransformationMatrix;
                                break;
                            }
                            else if (item.GetType().Equals(typeof(TargetableFloat3)) &&
                                ((item as TargetableFloat3).sid != null && (item as TargetableFloat3).sid.Equals(targetTransformationSID)))
                            {
                                if (targetNode.ItemsElementName[i].Equals(ItemsChoiceType2.scale))
                                {
                                    switch ((targetParam != null) ? targetParam.ToUpperInvariant() : null)
                                    {
                                        case null: animType = TransformationType.ScaleXYZ; break;
                                        case "X": animType = TransformationType.ScaleX; break;
                                        case "Y": animType = TransformationType.ScaleY; break;
                                        case "Z": animType = TransformationType.ScaleZ; break;
                                    }
                                }
                                else if (targetNode.ItemsElementName[i].Equals(ItemsChoiceType2.translate))
                                {
                                    switch ((targetParam != null) ? targetParam.ToUpperInvariant() : null)
                                    {
                                        case null: animType = TransformationType.TranslationXYZ; break;
                                        case "X": animType = TransformationType.TranslationX; break;
                                        case "Y": animType = TransformationType.TranslationY; break;
                                        case "Z": animType = TransformationType.TranslationZ; break;
                                    }
                                }
                                break;
                            }
                            else if (item.GetType().Equals(typeof(rotate)) &&
                                ((item as rotate).sid != null && (item as rotate).sid.Equals(targetTransformationSID)))
                            {
                                animType = GetRotationType((item as rotate));
                                break;
                            }
                        }

                        if (animType.Equals(TransformationType.None)) continue;

                        float smallestDifference = GetSmallestDifference(inputTime);
                        float smallestInterval = smallestDifference;

                        float[][] outputs = (!animType.Equals(TransformationType.TransformationMatrix)) ?
                            GetIndividualParamValuesFromNonMatrixInputFloatArray(
                                animSources.Where(src => src.id.Equals(outputSourceID)).ElementAt(0)) :
                            GetIndividualValuesFromMatrixInput(
                                animSources.Where(src => src.id.Equals(outputSourceID)).ElementAt(0));
                        float[][] finalOutputs = new float[outputs.Length][];

                        if (smallestInterval < INTERVAL)
                        {
                            // If imported models uses a lower framerate, "convert" it to use 30fps by interpolating the 
                            // values at double the framerate and then choosing the closest values to the 30fps intervals
                            smallestInterval *= 2f;

                            for (int i = 0; i < outputs.Length; i++)
                            {
                                finalOutputs[i] =
                                    InterpolateFramesAndExtractOneOverFrameRateFPS(inputTime, smallestInterval, outputs[i]);
                            }
                        }
                        else
                        {
                            finalOutputs = outputs;
                        }

                        int numFrames = finalOutputs[0].Length;

                        if (!boneAnimations.ContainsKey(boneID))
                            boneAnimations.Add(boneID, new ModelBase.AnimationDef(boneID + "-animation", boneID, numFrames));
                        ModelBase.AnimationDef animationDef = boneAnimations[boneID];

                        switch (animType)
                        {
                            case TransformationType.ScaleXYZ:
                                {
                                    AddNonConstantAnimationDefComponent(finalOutputs[0], numFrames, animationDef, 
                                        ModelBase.AnimationComponentType.ScaleX);
                                    AddNonConstantAnimationDefComponent(finalOutputs[1], numFrames, animationDef,
                                        ModelBase.AnimationComponentType.ScaleY);
                                    AddNonConstantAnimationDefComponent(finalOutputs[2], numFrames, animationDef,
                                        ModelBase.AnimationComponentType.ScaleZ);
                                }
                                break;
                            case TransformationType.ScaleX:
                                AddNonConstantAnimationDefComponent(finalOutputs[0], numFrames, animationDef,
                                        ModelBase.AnimationComponentType.ScaleX);
                                break;
                            case TransformationType.ScaleY:
                                AddNonConstantAnimationDefComponent(finalOutputs[0], numFrames, animationDef,
                                        ModelBase.AnimationComponentType.ScaleY);
                                break;
                            case TransformationType.ScaleZ:
                                AddNonConstantAnimationDefComponent(finalOutputs[0], numFrames, animationDef,
                                        ModelBase.AnimationComponentType.ScaleZ);
                                break;
                            case TransformationType.RotationX:
                                finalOutputs[0] = Array.ConvertAll(finalOutputs[0], x => x * Helper.Deg2Rad);
                                AddNonConstantAnimationDefComponent(finalOutputs[0], numFrames, animationDef,
                                        ModelBase.AnimationComponentType.RotateX);
                                break;
                            case TransformationType.RotationY:
                                finalOutputs[0] = Array.ConvertAll(finalOutputs[0], x => x * Helper.Deg2Rad);
                                AddNonConstantAnimationDefComponent(finalOutputs[0], numFrames, animationDef,
                                        ModelBase.AnimationComponentType.RotateY);
                                break;
                            case TransformationType.RotationZ:
                                finalOutputs[0] = Array.ConvertAll(finalOutputs[0], x => x * Helper.Deg2Rad);
                                AddNonConstantAnimationDefComponent(finalOutputs[0], numFrames, animationDef,
                                        ModelBase.AnimationComponentType.RotateZ);
                                break;
                            case TransformationType.TranslationXYZ:
                                {
                                    AddNonConstantAnimationDefComponent(finalOutputs[0], numFrames, animationDef,
                                        ModelBase.AnimationComponentType.TranslateX);
                                    AddNonConstantAnimationDefComponent(finalOutputs[1], numFrames, animationDef,
                                        ModelBase.AnimationComponentType.TranslateY);
                                    AddNonConstantAnimationDefComponent(finalOutputs[2], numFrames, animationDef,
                                        ModelBase.AnimationComponentType.TranslateZ);
                                }
                                break;
                            case TransformationType.TranslationX:
                                AddNonConstantAnimationDefComponent(finalOutputs[0], numFrames, animationDef,
                                        ModelBase.AnimationComponentType.TranslateX);
                                break;
                            case TransformationType.TranslationY:
                                AddNonConstantAnimationDefComponent(finalOutputs[0], numFrames, animationDef,
                                        ModelBase.AnimationComponentType.TranslateY);
                                break;
                            case TransformationType.TranslationZ:
                                AddNonConstantAnimationDefComponent(finalOutputs[0], numFrames, animationDef,
                                        ModelBase.AnimationComponentType.TranslateZ);
                                break;
                            case TransformationType.TransformationMatrix:
                                {
                                    float[][] decomposedValues = MergeAndDecomposeMatrixTransformations(finalOutputs, numFrames);
                                    AddNonConstantAnimationDefComponent(decomposedValues[0], numFrames, animationDef,
                                        ModelBase.AnimationComponentType.ScaleX);
                                    AddNonConstantAnimationDefComponent(decomposedValues[1], numFrames, animationDef,
                                        ModelBase.AnimationComponentType.ScaleY);
                                    AddNonConstantAnimationDefComponent(decomposedValues[2], numFrames, animationDef,
                                        ModelBase.AnimationComponentType.ScaleZ);
                                    AddNonConstantAnimationDefComponent(decomposedValues[3], numFrames, animationDef,
                                        ModelBase.AnimationComponentType.RotateX);
                                    AddNonConstantAnimationDefComponent(decomposedValues[4], numFrames, animationDef,
                                        ModelBase.AnimationComponentType.RotateY);
                                    AddNonConstantAnimationDefComponent(decomposedValues[5], numFrames, animationDef,
                                        ModelBase.AnimationComponentType.RotateZ);
                                    AddNonConstantAnimationDefComponent(decomposedValues[6], numFrames, animationDef,
                                        ModelBase.AnimationComponentType.TranslateX);
                                    AddNonConstantAnimationDefComponent(decomposedValues[7], numFrames, animationDef,
                                        ModelBase.AnimationComponentType.TranslateY);
                                    AddNonConstantAnimationDefComponent(decomposedValues[8], numFrames, animationDef,
                                        ModelBase.AnimationComponentType.TranslateZ);
                                }
                                break;
                        }
                    }

                    foreach (var item in anim.Items)
                    {
                        if (item as animation != null)
                            animations.Enqueue(item as animation);
                    }
                }

                // Make sure each animation contains a Scale, Rotation and Translation component. If not specified in library_animations, 
                // then these are just the bone's transformations throughout.
                foreach (ModelBase.AnimationDef anim in boneAnimations.Values)
                {
                    if (!anim.m_AnimationComponents.ContainsKey(ModelBase.AnimationComponentType.ScaleX))
                        anim.m_AnimationComponents.Add(ModelBase.AnimationComponentType.ScaleX,
                            new ModelBase.AnimationComponentDataDef(new float[] { m_Model.m_BoneTree.GetBoneByID(anim.m_BoneID).m_Scale.X },
                                anim.m_NumFrames, true, 1, true, ModelBase.AnimationComponentType.ScaleX));
                    if (!anim.m_AnimationComponents.ContainsKey(ModelBase.AnimationComponentType.ScaleY))
                        anim.m_AnimationComponents.Add(ModelBase.AnimationComponentType.ScaleY,
                            new ModelBase.AnimationComponentDataDef(new float[] { m_Model.m_BoneTree.GetBoneByID(anim.m_BoneID).m_Scale.Y },
                                anim.m_NumFrames, true, 1, true, ModelBase.AnimationComponentType.ScaleX));
                    if (!anim.m_AnimationComponents.ContainsKey(ModelBase.AnimationComponentType.ScaleZ))
                        anim.m_AnimationComponents.Add(ModelBase.AnimationComponentType.ScaleZ,
                            new ModelBase.AnimationComponentDataDef(new float[] { m_Model.m_BoneTree.GetBoneByID(anim.m_BoneID).m_Scale.Z },
                                anim.m_NumFrames, true, 1, true, ModelBase.AnimationComponentType.ScaleZ));
                    if (!anim.m_AnimationComponents.ContainsKey(ModelBase.AnimationComponentType.RotateX))
                        anim.m_AnimationComponents.Add(ModelBase.AnimationComponentType.RotateX,
                            new ModelBase.AnimationComponentDataDef(new float[] { m_Model.m_BoneTree.GetBoneByID(anim.m_BoneID).m_Rotation.X },
                                anim.m_NumFrames, true, 1, true, ModelBase.AnimationComponentType.RotateX));
                    if (!anim.m_AnimationComponents.ContainsKey(ModelBase.AnimationComponentType.RotateY))
                        anim.m_AnimationComponents.Add(ModelBase.AnimationComponentType.RotateY,
                            new ModelBase.AnimationComponentDataDef(new float[] { m_Model.m_BoneTree.GetBoneByID(anim.m_BoneID).m_Rotation.Y },
                                anim.m_NumFrames, true, 1, true, ModelBase.AnimationComponentType.RotateX));
                    if (!anim.m_AnimationComponents.ContainsKey(ModelBase.AnimationComponentType.RotateZ))
                        anim.m_AnimationComponents.Add(ModelBase.AnimationComponentType.RotateZ,
                            new ModelBase.AnimationComponentDataDef(new float[] { m_Model.m_BoneTree.GetBoneByID(anim.m_BoneID).m_Rotation.Z },
                                anim.m_NumFrames, true, 1, true, ModelBase.AnimationComponentType.RotateZ));
                    if (!anim.m_AnimationComponents.ContainsKey(ModelBase.AnimationComponentType.TranslateX))
                        anim.m_AnimationComponents.Add(ModelBase.AnimationComponentType.TranslateX,
                            new ModelBase.AnimationComponentDataDef(new float[] { m_Model.m_BoneTree.GetBoneByID(anim.m_BoneID).m_Translation.X },
                                anim.m_NumFrames, true, 1, true, ModelBase.AnimationComponentType.TranslateX));
                    if (!anim.m_AnimationComponents.ContainsKey(ModelBase.AnimationComponentType.TranslateY))
                        anim.m_AnimationComponents.Add(ModelBase.AnimationComponentType.TranslateY,
                            new ModelBase.AnimationComponentDataDef(new float[] { m_Model.m_BoneTree.GetBoneByID(anim.m_BoneID).m_Translation.Y },
                                anim.m_NumFrames, true, 1, true, ModelBase.AnimationComponentType.TranslateX));
                    if (!anim.m_AnimationComponents.ContainsKey(ModelBase.AnimationComponentType.TranslateZ))
                        anim.m_AnimationComponents.Add(ModelBase.AnimationComponentType.TranslateZ,
                            new ModelBase.AnimationComponentDataDef(new float[] { m_Model.m_BoneTree.GetBoneByID(anim.m_BoneID).m_Translation.Z },
                                anim.m_NumFrames, true, 1, true, ModelBase.AnimationComponentType.TranslateZ));
                }

                m_Model.m_Animations = boneAnimations;
            }
        }

        private static void AddNonConstantAnimationDefComponent(float[] values, int numFrames, ModelBase.AnimationDef animationDef, 
            ModelBase.AnimationComponentType animationComponentType)
        {
            animationDef.m_AnimationComponents.Add(animationComponentType,
                new ModelBase.AnimationComponentDataDef(
                values, numFrames, false, 1, false, animationComponentType));
        }

        private static float[][] MergeAndDecomposeMatrixTransformations(float[][] individualValues, int numFrames)
        {
            float[][] merged = new float[9][];
            for (int i = 0; i < merged.Length; i++)
                merged[i] = new float[numFrames];

            for (int i = 0; i < numFrames; i++)
            {
                float[] vals = new float[] { 
                    individualValues[0][i], individualValues[1][i], individualValues[2][i], individualValues[3][i], 
                    individualValues[4][i], individualValues[5][i], individualValues[6][i], individualValues[7][i], 
                    individualValues[8][i], individualValues[9][i], individualValues[10][i], individualValues[11][i], 
                    individualValues[12][i], individualValues[13][i], individualValues[14][i], individualValues[15][i] };
                Matrix4 mat = Helper.FloatArrayToMatrix4(vals);
                Vector3 scale, rotation, translation;
                Helper.DecomposeSRTMatrix2(mat, out scale, out rotation, out translation);

                merged[0][i] = scale.X;
                merged[1][i] = scale.Y;
                merged[2][i] = scale.Z;
                merged[3][i] = rotation.X;
                merged[4][i] = rotation.Y;
                merged[5][i] = rotation.Z;
                merged[6][i] = translation.X;
                merged[7][i] = translation.Y;
                merged[8][i] = translation.Z;
            }

            return merged;
        }

        protected float[] InterpolateFramesAndExtractOneOverFrameRateFPS(float[] time, float smallestInterval, float[] outputValues)
        {
            float timeByFPS = (float)Math.Round((double)(time[time.Length - 1] * FRAMES_PER_SECOND), 1);
            int numFrames = (timeByFPS % 1f == 0f) ? (int)(timeByFPS + 1) : (int)Math.Ceiling(timeByFPS);
            float[] alignedFrameValues = new float[numFrames];

            float lastFrameTime = time[time.Length - 1];
            List<float> interpolatedFrames = new List<float>();

            List<float> interpolatedTime = new List<float>();

            // Interpolate and then convert to (1 / FRAMES_PER_SECOND) frames using closest to keyframe time
            for (int kf = 0; kf < time.Length - 1; kf++)
            {
                float time_diff = time[kf + 1] - time[kf];
                if (time_diff < 0)
                    time_diff *= (-1);
                float value_diff = outputValues[kf + 1] - outputValues[kf];

                int numFramesInGap = (int)(1f / Math.Round((time_diff / smallestInterval), 2));
                List<float> interp = new List<float>();

                if (numFramesInGap == 1)
                {
                    interp.Add(outputValues[kf]);
                    interpolatedTime.Add(time[kf]);
                }
                else
                {
                    for (int inf = 0; inf < numFramesInGap; inf++)
                    {
                        interp.Add(outputValues[kf] + (inf * ((value_diff > 0) ? -1 : 1) * (value_diff / numFramesInGap)));
                        interpolatedTime.Add(time[kf] + (inf * (time_diff / numFramesInGap)));
                    }
                }
                interpolatedFrames.AddRange(interp);
            }
            interpolatedFrames.Add(outputValues[outputValues.Length - 1]);
            interpolatedTime.Add(time[time.Length - 1]);

            float[] tmpTime = interpolatedTime.ToArray();
            for (int kf = 0; kf < numFrames; kf++)
            {
                float frameTime = (float)kf * INTERVAL;
                int indexClosest = GetIndexClosest(tmpTime, frameTime);
                alignedFrameValues[kf] = interpolatedFrames[indexClosest];
            }
            //alignedFrameValues[numFrames - 1] = interpolatedFrames[numInterpolatedFrames - 1];

            return alignedFrameValues;
        }

        private static int GetIndexClosest(float[] values, float target)
        {
            int index = -1;
            float closest = int.MaxValue;
            float minDifference = int.MaxValue;
            for (int i = 0; i < values.Length; i++)
            {
                double difference = Math.Abs(values[i] - target);
                if (minDifference > difference)
                {
                    minDifference = (float)difference;
                    closest = values[i];
                    index = i;
                }
            }

            return index;
        }

        private static float[][] GetIndividualParamValuesFromNonMatrixInputFloatArray(source src)
        {
            float[][] outs = null;

            if (src.technique_common != null && src.technique_common.accessor != null)
            {
                accessor accs = src.technique_common.accessor;
                double[] vals = (src.Item as float_array).Values;
                int numParams = accs.param.Length;

                outs = new float[numParams][];

                for (int i = 0; i < numParams; i++)
                {
                    outs[i] = new float[accs.count];
                }
                int counter = 0;
                for (int i = 0; i < vals.Length; i += numParams, counter++)
                {
                    for (int j = 0; j < numParams; j++)
                    {
                        outs[j][counter] = (float)vals[i + j];
                    }
                }
            }

            return outs;
        }

        private static float[][] GetIndividualValuesFromMatrixInput(source src)
        {
            float[][] outs = null;

            if (src.technique_common != null && src.technique_common.accessor != null)
            {
                accessor accs = src.technique_common.accessor;
                double[] vals = (src.Item as float_array).Values;

                outs = new float[16][];

                for (int i = 0; i < 16; i++)
                {
                    outs[i] = new float[accs.count];
                }
                int counter = 0;
                for (int i = 0; i < vals.Length; i += 16, counter++)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        outs[j][counter] = (float)vals[i + j];
                    }
                }
            }

            return outs;
        }

        private static float GetSmallestDifference(float[] values)
        {
            float smallest = float.MaxValue;

            for (int i = 0; i < values.Length - 1; i++)
            {
                float difference = values[i + 1] - values[i];
                if (difference < 0)
                    difference *= (-1);

                if (difference < smallest)
                    smallest = difference;
            }

            return smallest;
        }

        private node FindNodeInTree(node parent, string nodeID)
        {
            if (parent.id == null)
                return null;

            if (parent.id.Equals(nodeID))
            {
                return parent;
            }
            else if (parent.node1 != null && parent.node1.Length > 0)
            {
                foreach (node child in parent.node1)
                {
                    node result = FindNodeInTree(child, nodeID);
                    if (result != null)
                        return result;
                }
            }
            return null;
        }

        private node FindNodeInLibraryVisualScenes(string id)
        {
            foreach (visual_scene visualScene in this.library_visual_scenes.visual_scene)
            {
                foreach (node node0 in visualScene.node)
                {
                    node result = FindNodeInTree(node0, id);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            return null;
        }

        private static bool IsRootNodeOfChild(node parent, node child)
        {
            if (parent.node1 == null || parent.node1.Length == 0)
                return false;
            else
            {
                foreach (node n in parent.node1)
                {
                    if (child.id.Equals(n.id))
                        return true;
                    else if (IsRootNodeOfChild(child, n))
                        return true;
                }
            }
            return false;
        }

        private string FindIDFromSIDInSkeleton(string skeletonRoot, string sid)
        {
            string id = null;

            node skeletonRootNode = null;

            foreach (visual_scene visualScene in this.library_visual_scenes.visual_scene)
            {
                foreach (node node0 in visualScene.node)
                {
                    node result = FindNodeInTree(node0, skeletonRoot);
                    if (result != null)
                    {
                        skeletonRootNode = result;
                        break;
                    }
                }
            }

            var queue = new Queue<node>();
            queue.Enqueue(skeletonRootNode);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();

                if (node.sid != null)
                {
                    if (node.sid.Equals(sid))
                    {
                        id = node.id;
                        break;
                    }
                }
                else
                {
                    if (node.id.Equals(sid))
                    {
                        id = node.id;
                        break;
                    }
                }

                if (node.node1 != null)
                {
                    foreach (var child in node.node1)
                        queue.Enqueue(child);
                }
            }

            return id;
        }


        private float[] GetValueFromFloatArraySource(source src, int index)
        {
            float[] values = new float[0];

            float[] floatArray = new float[0];

            if (src.Item as float_array != null)
            {
                floatArray = Array.ConvertAll<double, float>((src.Item as float_array).Values, Convert.ToSingle);
            }
            if (src.technique_common != null && src.technique_common.accessor != null)
            {
                ulong count = src.technique_common.accessor.count;
                ulong stride = src.technique_common.accessor.stride;

                values = new float[stride];
                Array.Copy(floatArray, (int)((ulong)index * stride), values, 0, (int)stride);
            }

            return values;
        }

        public enum TransformationType
        {
            TranslationXYZ,
            TranslationX,
            TranslationY,
            TranslationZ,
            RotationX,
            RotationY,
            RotationZ,
            ScaleXYZ,
            ScaleX,
            ScaleY,
            ScaleZ,
            TransformationMatrix,
            None
        };

        public override Dictionary<string, ModelBase.MaterialDef> GetModelMaterials()
        {
            if (m_Model.m_Materials.Count > 0)
                return m_Model.m_Materials;
            else
            {
                LoadModel();
                return m_Model.m_Materials;
            }
        }
    }
}
