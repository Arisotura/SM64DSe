/* NITROIntermediateModelDataLoader
 * 
 * Converts the Nintendo SDK NITRO Intermediate File Format "Intermediate Model Data" (IMD) into a 
 * ModelBase object for use by the writer classes.
 * 
 * The SDK Intermediate formats, having been designed specifically for the DS and used by almost all 
 * DS games offer the most features and are the best format to use when importing SM64DS models. The 
 * Nintendo models used in SM64DS have been generated from IMD and ICA files.
 * 
 * The two main limitations with these formats is that copy of the leaked SDK 3D tools is required to produce 
 * IMD and ICA files and the leaked plugins require 3DS Max v6, Maya version 5.0.1, 6.0.1, 6.5, or 7.0, 
 * Softimage 3D v4.0 or Softimage XSI v4.0 or 4.2.
 * 
 * Unfortunately, for legal reasons these cannot be distributed with SM64DSe nor can the documentation.
 * 
 * Current limitiations:
 *  - The option "Force Full Weight" under "Imd Options" in the NITRO Export dialogue must be selected.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing;
using OpenTK;

namespace SM64DSe.ImportExport.Loaders.ExternalLoaders
{
    public class NITROIntermediateModelDataLoader : AbstractModelLoader
    {
        protected model_info model_info;
        protected AccurateVertex[] vtx_pos_data;
        protected Color[] vtx_color_data;
        protected Dictionary<int, string> m_OriginalNodeIndices = new Dictionary<int, string>(); // Need this to get Bone ID from matrix
        protected Dictionary<int, string> m_OriginalMaterialIndices = new Dictionary<int, string>();

        public NITROIntermediateModelDataLoader(string modelFileName) :
            base(modelFileName) { }

        public override ModelBase LoadModel(float scale)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(m_ModelFileName);

            XmlNode model_info = doc.SelectSingleNode("/imd/body/model_info");
            ReadModelInfo(model_info);

            if (this.model_info.vertex_style.Equals("index"))
            {
                XmlNode vtx_pos_data = doc.SelectSingleNode("/imd/body/vtx_pos_data");
                XmlNode vtx_color_data = doc.SelectSingleNode("/imd/body/vtx_color_data");
                ReadIndexedVertexData(vtx_pos_data, vtx_color_data);
            }

            XmlNode tex_image_array = doc.SelectSingleNode("/imd/body/tex_image_array");
            XmlNode tex_palette_array = doc.SelectSingleNode("/imd/body/tex_palette_array");
            ReadTextures(tex_image_array, tex_palette_array);

            XmlNode material_array = doc.SelectSingleNode("/imd/body/material_array");
            ReadMaterials(material_array);

            XmlNode node_array = doc.SelectSingleNode("/imd/body/node_array");
            XmlNode polygon_array = doc.SelectSingleNode("/imd/body/polygon_array");
            XmlNode matrix_array = doc.SelectSingleNode("/imd/body/matrix_array");
            ReadNodes(node_array, polygon_array, matrix_array);

            m_Model.ScaleModel(scale);

            return m_Model;
        }

        protected void ReadModelInfo(XmlNode model_info)
        {
            this.model_info = new model_info();

            this.model_info.pos_scale = int.Parse(model_info.Attributes["pos_scale"].Value);
            this.model_info.scaling_rule = model_info.Attributes["scaling_rule"].Value;
            this.model_info.vertex_style = model_info.Attributes["vertex_style"].Value;
            this.model_info.magnify = Helper.ParseFloat(model_info.Attributes["magnify"].Value);
            this.model_info.tool_start_frame = int.Parse(model_info.Attributes["tool_start_frame"].Value);
            this.model_info.tex_matrix_mode = model_info.Attributes["tex_matrix_mode"].Value;
            this.model_info.compress_node = model_info.Attributes["compress_node"].Value;
            this.model_info.node_size = Array.ConvertAll(
                model_info.Attributes["node_size"].Value.Split(Strings.WHITESPACE, StringSplitOptions.RemoveEmptyEntries),
                Convert.ToInt32);
            this.model_info.compress_material = model_info.Attributes["compress_material"].Value.Equals("on");
            this.model_info.material_size = Array.ConvertAll(
                model_info.Attributes["material_size"].Value.Split(Strings.WHITESPACE, 
                StringSplitOptions.RemoveEmptyEntries),
                Convert.ToInt32);
            this.model_info.output_texture = model_info.Attributes["output_texture"].Value;
            this.model_info.force_full_weight = model_info.Attributes["force_full_weight"].Value.Equals("on");
            this.model_info.use_primitive_strip = model_info.Attributes["use_primitive_strip"].Value.Equals("on");
        }

        protected void ReadIndexedVertexData(XmlNode vtx_pos_data, XmlNode vtx_color_data)
        {
            if (vtx_pos_data != null)
            {
                int pos_size = int.Parse(vtx_pos_data.Attributes["pos_size"].Value);
                this.vtx_pos_data = new AccurateVertex[pos_size];
                string[] valuesStr = vtx_pos_data.InnerText.
                    Split(Strings.WHITESPACE, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < pos_size; i++)
                {
                    string x = valuesStr[(i * 3) + 0];
                    string y = valuesStr[(i * 3) + 1];
                    string z = valuesStr[(i * 3) + 2];
                    this.vtx_pos_data[i] = new AccurateVertex(x, y, z);
                }
            }
            if (vtx_color_data != null)
            {
                int color_size = int.Parse(vtx_color_data.Attributes["color_size"].Value);
                this.vtx_color_data = new Color[color_size];
                string[] valuesStr = vtx_color_data.InnerText.
                    Split(Strings.WHITESPACE, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < color_size; i++)
                {
                    int r = (int)(((float)int.Parse(valuesStr[(i * 3) + 0], Helper.USA) / 31f) * 255f);
                    int g = (int)(((float)int.Parse(valuesStr[(i * 3) + 1], Helper.USA) / 31f) * 255f);
                    int b = (int)(((float)int.Parse(valuesStr[(i * 3) + 2], Helper.USA) / 31f) * 255f);
                    this.vtx_color_data[i] = Color.FromArgb(r, g, b);
                }
            }
        }

        protected void ReadTextures(XmlNode tex_image_array, XmlNode tex_palette_array)
        {
            if (tex_image_array == null) return;
            int texImageArray_size = int.Parse(tex_image_array.Attributes["size"].Value);
            int texPaletteArray_size = (tex_palette_array != null) ? int.Parse(tex_palette_array.Attributes["size"].Value) : 0;

            if (texImageArray_size < 1) return;

            XmlNodeList tex_images = tex_image_array.SelectNodes("tex_image");
            XmlNodeList tex_palettes = (tex_palette_array != null) ? tex_palette_array.SelectNodes("tex_palette") : null;

            foreach (XmlNode tex_image in tex_images)
            {
                int index = int.Parse(tex_image.Attributes["index"].Value);
                string name = tex_image.Attributes["name"].Value;
                int width = int.Parse(tex_image.Attributes["width"].Value);
                int height = int.Parse(tex_image.Attributes["height"].Value);
                int original_width = int.Parse(tex_image.Attributes["original_width"].Value);
                int original_height = int.Parse(tex_image.Attributes["original_height"].Value);
                string format = tex_image.Attributes["format"].Value;
                ModelBase.TextureFormat textureFormat;
                switch (format)
                {
                    case "palette4": textureFormat = ModelBase.TextureFormat.Nitro_Palette4; break;
                    case "palette16": textureFormat = ModelBase.TextureFormat.Nitro_Palette16; break;
                    case "palette256": textureFormat = ModelBase.TextureFormat.Nitro_Palette256; break;
                    case "tex4x4": textureFormat = ModelBase.TextureFormat.Nitro_Tex4x4; break;
                    case "a3i5": textureFormat = ModelBase.TextureFormat.Nitro_A3I5; break;
                    case "a5i3": textureFormat = ModelBase.TextureFormat.Nitro_A5I3; break;
                    case "direct": textureFormat = ModelBase.TextureFormat.Nitro_Direct; break;
                    default: goto case "direct";
                }
                string color0_mode = (textureFormat == ModelBase.TextureFormat.Nitro_Palette4 ||
                    textureFormat == ModelBase.TextureFormat.Nitro_Palette16 ||
                    textureFormat == ModelBase.TextureFormat.Nitro_Palette256) ?
                    tex_image.Attributes["color0_mode"].Value : null;
                string palette_name = (textureFormat != ModelBase.TextureFormat.Nitro_Direct) ?
                    tex_image.Attributes["palette_name"].Value : null;
                string path = (tex_image.Attributes["path"] != null) ? tex_image.Attributes["path"].Value : null;

                XmlNode bitmap = tex_image.SelectSingleNode("bitmap");
                int bitmap_size = int.Parse(bitmap.Attributes["size"].Value);
                int numBytesWord = (textureFormat != ModelBase.TextureFormat.Nitro_Tex4x4) ? 2 : 4;
                byte[] dataTex = UShortStringArrayToByteArray(bitmap.InnerText, bitmap_size, numBytesWord);

                ModelBase.TextureDefBase texture = null;

                if (textureFormat != ModelBase.TextureFormat.Nitro_Direct && palette_name != null)
                {
                    byte[] dataPal = null;

                    if (textureFormat != ModelBase.TextureFormat.Nitro_Tex4x4)
                    {
                        dataPal = ReadPaletteData(palette_name, tex_palettes);
                    }
                    else
                    {
                        XmlNode tex4x4_palette_idx = tex_image.SelectSingleNode("tex4x4_palette_idx");
                        int tex4x4PaletteIdx_size = int.Parse(tex4x4_palette_idx.Attributes["size"].Value);
                        dataPal = UShortStringArrayToByteArray(tex4x4_palette_idx.InnerText, tex4x4PaletteIdx_size, 2);
                    }

                    texture = new ModelBase.TextureDefNitro(name, dataTex, palette_name, dataPal,
                        width, height, 
                        (byte)((color0_mode != null && color0_mode.Equals("transparency")) ? 1 : 0), 
                        textureFormat);
                }
                else
                {
                    texture = new ModelBase.TextureDefNitro(name, dataTex, width, height,
                        0, textureFormat);
                }

                m_Model.m_Textures.Add(name, texture);
            }
        }

        protected byte[] ReadPaletteData(string palette_name, XmlNodeList tex_palettes)
        {
            XmlNode tex_palette = null;
            foreach (XmlNode node in tex_palettes)
            {
                if (node.Attributes["name"].Value.Equals(palette_name))
                {
                    tex_palette = node;
                    break;
                }
            }

            int index = int.Parse(tex_palette.Attributes["index"].Value);
            string name = tex_palette.Attributes["name"].Value;
            int color_size = int.Parse(tex_palette.Attributes["color_size"].Value);

            return UShortStringArrayToByteArray(tex_palette.InnerText, color_size, 2);
        }

        protected static byte[] UShortStringArrayToByteArray(string dataStr, int numWords, int numBytesInWord)
        {
            byte[] data = new byte[numWords * numBytesInWord];

            // AB12 -> [] { 12, AB }
            // AB12CD34 -> [] { 34, CD, 12, AB }
            string[] words = dataStr.Split(Strings.WHITESPACE, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < numWords; i++)
            {
                ushort value = ushort.Parse(words[i], System.Globalization.NumberStyles.HexNumber);
                byte[] valueBytes = BitConverter.GetBytes(value);
                Array.Copy(valueBytes, 0, data, (i * numBytesInWord), valueBytes.Length);
            }

            return data;
        }

        protected void ReadMaterials(XmlNode material_array)
        {
            if (material_array == null) return;
            int materialArray_size = int.Parse(material_array.Attributes["size"].Value);
            if (materialArray_size < 1) return;

            XmlNodeList materials = material_array.SelectNodes("material");

            foreach (XmlNode material in materials)
            {
                string name = material.Attributes["name"].Value;
                int index = int.Parse(material.Attributes["index"].Value);
                bool[] lights = new bool[4];
                for (int i = 0; i < 4; i++)
                    lights[i] = material.Attributes["light" + i].Value.Equals("on");
                string face = material.Attributes["face"].Value;
                ModelBase.MaterialDef.PolygonDrawingFace polygonDrawingFace;
                switch (face)
                {
                    case "front": polygonDrawingFace = ModelBase.MaterialDef.PolygonDrawingFace.Front; break;
                    case "back": polygonDrawingFace = ModelBase.MaterialDef.PolygonDrawingFace.Back; break;
                    case "both": polygonDrawingFace = ModelBase.MaterialDef.PolygonDrawingFace.FrontAndBack; break;
                    default: goto case "front";
                }
                byte alpha = byte.Parse(material.Attributes["alpha"].Value);
                bool wire_mode = material.Attributes["wire_mode"].Value.Equals("on");
                string polygon_mode = material.Attributes["polygon_mode"].Value;
                ModelBase.MaterialDef.PolygonMode polygonMode;
                switch (polygon_mode)
                {
                    case "modulate": polygonMode = ModelBase.MaterialDef.PolygonMode.Modulation; break;
                    case "decal": polygonMode = ModelBase.MaterialDef.PolygonMode.Decal; break;
                    case "toon_highlight": polygonMode = ModelBase.MaterialDef.PolygonMode.Toon_HighlightShading; break;
                    case "shadow": polygonMode = ModelBase.MaterialDef.PolygonMode.Shadow; break;
                    default: goto case "modulate";
                }
                int polygon_id = int.Parse(material.Attributes["polygon_id"].Value);
                bool fog_flag = material.Attributes["fog_flag"].Value.Equals("on");
                bool depth_test_decal = material.Attributes["depth_test_decal"].Value.Equals("on");
                bool translucent_update_depth = material.Attributes["translucent_update_depth"].Value.Equals("on");
                bool render_1_pixel = material.Attributes["render_1_pixel"].Value.Equals("on");
                bool far_clipping = material.Attributes["far_clipping"].Value.Equals("on");
                byte[] diffuse = Array.ConvertAll<string, byte>(
                    material.Attributes["diffuse"].Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries), Convert.ToByte);
                byte[] ambient = Array.ConvertAll<string, byte>(
                    material.Attributes["ambient"].Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries), Convert.ToByte);
                byte[] specular = Array.ConvertAll<string, byte>(
                    material.Attributes["specular"].Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries), Convert.ToByte);
                byte[] emission = Array.ConvertAll<string, byte>(
                    material.Attributes["emission"].Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries), Convert.ToByte);
                bool shininess_table_flag = material.Attributes["shininess_table_flag"].Value.Equals("on");
                int tex_image_idx = int.Parse(material.Attributes["tex_image_idx"].Value);
                int tex_palette_idx = int.Parse(material.Attributes["tex_palette_idx"].Value);
                ModelBase.MaterialDef.TexTiling[] tex_tiling = new ModelBase.MaterialDef.TexTiling[2];
                string[] tmpsplit = (material.Attributes["tex_tiling"] != null) ? 
                    material.Attributes["tex_tiling"].Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries) : 
                    null;
                if (tex_image_idx >= 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        switch (tmpsplit[i])
                        {
                            case "clamp": tex_tiling[i] = ModelBase.MaterialDef.TexTiling.Clamp; break;
                            case "repeat": tex_tiling[i] = ModelBase.MaterialDef.TexTiling.Repeat; break;
                            case "flip": tex_tiling[i] = ModelBase.MaterialDef.TexTiling.Flip; break;
                            default: goto case "repeat";
                        }
                    }
                }
                float[] tex_scale = (tex_image_idx >= 0) ? Array.ConvertAll(
                    material.Attributes["tex_scale"].Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries),
                    Convert.ToSingle) : null;
                float tex_rotate = (tex_image_idx >= 0) ? Helper.ParseFloat(material.Attributes["tex_rotate"].Value) : 0f;
                float[] tex_translate = (tex_image_idx >= 0) ? Array.ConvertAll(
                    material.Attributes["tex_translate"].Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries),
                    Convert.ToSingle) : null;
                string tex_gen_mode = (tex_image_idx >= 0) ? material.Attributes["tex_gen_mode"].Value : null;
                ModelBase.TexGenMode texGenMode;
                switch (tex_gen_mode)
                {
                    case "none": texGenMode = ModelBase.TexGenMode.None; break;
                    case null: goto case "none";
                    case "tex": texGenMode = ModelBase.TexGenMode.Tex; break;
                    case "nrm": texGenMode = ModelBase.TexGenMode.Normal; break;
                    case "pos": texGenMode = ModelBase.TexGenMode.Pos; break;
                    default: goto case "none";
                }
                string tex_gen_st_src = (material.Attributes["tex_gen_st_src"] != null) ? material.Attributes["tex_gen_st_src"].Value : null;
                string tex_effect_mtx = (material.Attributes["tex_effect_mtx"] != null) ? material.Attributes["tex_effect_mtx"].Value : null;

                ModelBase.MaterialDef matDef = new ModelBase.MaterialDef(name);
                matDef.m_TextureDefID = (tex_image_idx >= 0) ? m_Model.m_Textures.Values.ElementAt(tex_image_idx).m_ID : null;
                matDef.m_Lights = lights;
                matDef.m_PolygonDrawingFace = polygonDrawingFace;
                matDef.m_Alpha = alpha;
                matDef.m_WireMode = wire_mode;
                matDef.m_PolygonMode = polygonMode;
                matDef.m_FogFlag = fog_flag;
                matDef.m_DepthTestDecal = depth_test_decal;
                matDef.m_RenderOnePixelPolygons = render_1_pixel;
                matDef.m_FarClipping = far_clipping;
                matDef.m_Diffuse = Color.FromArgb(
                    (int)(diffuse[0] / 31f * 255f), (int)(diffuse[1] / 31f * 255f), (int)(diffuse[2] / 31f * 255f));
                matDef.m_Ambient = Color.FromArgb(
                    (int)(ambient[0] / 31f * 255f), (int)(ambient[1] / 31f * 255f), (int)(ambient[2] / 31f * 255f));
                matDef.m_Specular = Color.FromArgb(
                    (int)(specular[0] / 31f * 255f), (int)(specular[1] / 31f * 255f), (int)(specular[2] / 31f * 255f));
                matDef.m_Emission = Color.FromArgb(
                    (int)(emission[0] / 31f * 255f), (int)(emission[1] / 31f * 255f), (int)(emission[2] / 31f * 255f));
                matDef.m_ShininessTableEnabled = shininess_table_flag;
                matDef.m_TexTiling = tex_tiling;
                matDef.m_TextureScale = (tex_scale != null) ? new Vector2(tex_scale[0], tex_scale[1]) : Vector2.One;
                matDef.m_TextureRotation = tex_rotate;
                matDef.m_TextureTranslation = (tex_translate != null) ? new Vector2(tex_translate[0], tex_translate[1]) : Vector2.Zero;
                matDef.m_TexGenMode = texGenMode;

                m_Model.m_Materials.Add(matDef.m_ID, matDef);
                m_OriginalMaterialIndices.Add(index, matDef.m_ID);
            }
        }

        protected void ReadMatrices(XmlNode matrix_array, Dictionary<int, string> originalNodeIndices)
        {
            if (matrix_array == null) return;
            int matrixArray_size = int.Parse(matrix_array.Attributes["size"].Value);
            if (matrixArray_size < 1) return;
            else if (matrixArray_size > 31) Console.WriteLine("WARN: More than 31 matrices, your model will not import correctly.");

            XmlNodeList matrices = matrix_array.SelectNodes("matrix");

            foreach (XmlNode matrix in matrices)
            {
                int index = int.Parse(matrix.Attributes["index"].Value);
                int mtx_weight = int.Parse(matrix.Attributes["mtx_weight"].Value);
                int node_idx = (mtx_weight == 1) ? int.Parse(matrix.Attributes["node_idx"].Value) : -1;
                int envelope_head = (mtx_weight >= 2) ? int.Parse(matrix.Attributes["envelope_head"].Value) : -1;
                if (mtx_weight >= 2) Console.WriteLine("WARN: Weight envelopes not supported yet, only direct assignment to a matrix.");

                m_Model.m_BoneTransformsMap.Add(originalNodeIndices[node_idx], index);
            }
        }

        protected void ReadNodes(XmlNode node_array, XmlNode polygon_array, XmlNode matrix_array)
        {
            if (node_array == null) return;
            int nodeArray_size = int.Parse(node_array.Attributes["size"].Value);

            if (nodeArray_size < 1) return;

            XmlNodeList nodes = node_array.SelectNodes("node");

            foreach (XmlNode node in nodes)
                m_OriginalNodeIndices.Add(int.Parse(node.Attributes["index"].Value), node.Attributes["name"].Value);

            ReadMatrices(matrix_array, m_OriginalNodeIndices);

            var queue = new Queue<XmlNode>();
            var geometryQueue = new Queue<XmlNode>();
            queue.Enqueue(nodes[0]);

            while (queue.Count > 0)
            {
                XmlNode node = queue.Dequeue();

                int index = int.Parse(node.Attributes["index"].Value);
                string name = node.Attributes["name"].Value;
                string kind = node.Attributes["kind"].Value;
                int parent = int.Parse(node.Attributes["parent"].Value);
                int child = int.Parse(node.Attributes["child"].Value);
                int brother_next = int.Parse(node.Attributes["brother_next"].Value);
                int brother_prev = int.Parse(node.Attributes["brother_prev"].Value);
                bool draw_mtx = node.Attributes["draw_mtx"].Value.Equals("on");
                string scale_compensate = (node.Attributes["scale_compensate"] != null) ? node.Attributes["scale_compensate"].Value : null;
                string billboard = node.Attributes["billboard"].Value; // Either "on", "off" or "y_on"
                float[] scale = Array.ConvertAll(
                    node.Attributes["scale"].Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries),
                    Convert.ToSingle);
                float[] rotate = Array.ConvertAll(
                    node.Attributes["rotate"].Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries),
                    Convert.ToSingle);
                float[] translate = Array.ConvertAll(
                    node.Attributes["translate"].Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries),
                    Convert.ToSingle);
                bool visibility = node.Attributes["visibility"].Value.Equals("on");
                int display_size = int.Parse(node.Attributes["display_size"].Value);
                int vertex_size = (kind.Equals("mesh")) ? int.Parse(node.Attributes["vertex_size"].Value) : -1;
                int polygon_size = (kind.Equals("mesh")) ? int.Parse(node.Attributes["polygon_size"].Value) : -1;
                int triangle_size = (kind.Equals("mesh")) ? int.Parse(node.Attributes["triangle_size"].Value) : -1;
                int quad_size = (kind.Equals("mesh")) ? int.Parse(node.Attributes["quad_size"].Value) : -1;
                float[] volume_min = (kind.Equals("mesh")) ? Array.ConvertAll(
                    node.Attributes["volume_min"].Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries),
                    Convert.ToSingle) : null;
                float[] volume_max = (kind.Equals("mesh")) ? Array.ConvertAll(
                    node.Attributes["volume_max"].Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries),
                    Convert.ToSingle) : null;
                float volume_r = (kind.Equals("mesh")) ? Helper.ParseFloat(node.Attributes["volume_r"].Value) : -1;

                ModelBase.BoneDef boneDef = new ModelBase.BoneDef(name);
                boneDef.SetScale(new Vector3(scale[0], scale[1], scale[2]));
                boneDef.SetRotation(new Vector3(rotate[0] * Helper.Deg2Rad, rotate[1] * Helper.Deg2Rad, rotate[2] * Helper.Deg2Rad));
                boneDef.SetTranslation(new Vector3(translate[0], translate[1], translate[2]));
                boneDef.CalculateBranchTransformations();

                if (display_size > 0)
                {
                    geometryQueue.Enqueue(node);
                }

                if (parent == -1)
                    m_Model.m_BoneTree.AddRootBone(boneDef);
                else
                    m_Model.m_BoneTree.GetBoneByID(m_OriginalNodeIndices[parent]).AddChild(boneDef);

                /* Child node takes priority and siblings are stored consecutively by name in alphanumerical order 
                 * (a→z and 0→9). The model root node must be the first <node> in <node_array> (index 0).
                 * "child" attribute gives first child, alphanumerically.
                 */ 

                if (child != -1)
                    queue.Enqueue(nodes[child]);
                if (brother_next != -1)
                    queue.Enqueue(nodes[brother_next]);
            }

            // In oder to correctly read the geometry we need to already have the list of bones read in so that bone ID's 
            // can be assigned to vertices
            while (geometryQueue.Count > 0)
            {
                XmlNode node = geometryQueue.Dequeue();

                XmlNodeList displays = node.SelectNodes("display");

                ModelBase.BoneDef boneDef = m_Model.m_BoneTree.GetBoneByID(node.Attributes["name"].Value);
                ModelBase.GeometryDef geometryDef = new ModelBase.GeometryDef("geometry-0");

                foreach (XmlNode display in displays)
                {
                    int display_index = int.Parse(display.Attributes["index"].Value);
                    int display_material = int.Parse(display.Attributes["material"].Value);
                    int display_polygon = int.Parse(display.Attributes["polygon"].Value);
                    int display_priority = int.Parse(display.Attributes["priority"].Value);

                    ReadPolygon(display_polygon, polygon_array, m_OriginalMaterialIndices[display_material], geometryDef);

                    boneDef.m_MaterialsInBranch.Add(m_Model.m_Materials.ElementAt(display_material).Key);
                }
                
                boneDef.m_Geometries.Add(geometryDef.m_ID, geometryDef);
            }
            
            m_Model.ApplyTransformations();
        }

        protected void ReadPolygon(int polygonIndex, XmlNode polygon_array, string materialID, ModelBase.GeometryDef geometryDef)
        {
            if (polygon_array == null) return;
            int polygonArray_size = int.Parse(polygon_array.Attributes["size"].Value);
            if (polygonArray_size < 1) return;

            XmlNodeList polygons = polygon_array.SelectNodes("polygon");
            XmlNode polygon = polygons[polygonIndex];
            
            int index = int.Parse(polygon.Attributes["index"].Value);
            string name = polygon.Attributes["name"].Value;
            int vertex_size = int.Parse(polygon.Attributes["vertex_size"].Value);
            int polygon_size = int.Parse(polygon.Attributes["polygon_size"].Value);
            int triangle_size = int.Parse(polygon.Attributes["triangle_size"].Value);
            int quad_size = int.Parse(polygon.Attributes["quad_size"].Value);
            float[] volume_min = Array.ConvertAll(
                polygon.Attributes["volume_min"].Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries), 
                Convert.ToSingle);
            float[] volume_max = Array.ConvertAll(
                polygon.Attributes["volume_max"].Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries),
                Convert.ToSingle);
            float volume_r = Helper.ParseFloat(polygon.Attributes["volume_r"].Value);
            int mtx_prim_size = int.Parse(polygon.Attributes["mtx_prim_size"].Value);
            bool nrm_flag = polygon.Attributes["nrm_flag"].Value.Equals("on");
            bool clr_flag = polygon.Attributes["clr_flag"].Value.Equals("on");
            bool tex_flag = polygon.Attributes["tex_flag"].Value.Equals("on");

            XmlNodeList mtx_prims = polygon.SelectNodes("mtx_prim");

            foreach (XmlNode mtx_prim in mtx_prims)
            {
                int mtxPrim_index = int.Parse(mtx_prim.Attributes["index"].Value);

                ModelBase.PolyListDef polyListDef = new ModelBase.PolyListDef(name + mtxPrim_index, materialID);

                XmlNode mtx_list = mtx_prim.SelectSingleNode("mtx_list");
                int mtxList_size = int.Parse(mtx_list.Attributes["size"].Value);
                Dictionary<int, int> localToGlobalMatrixIDMap = new Dictionary<int, int>();
                int[] mtxList_data = Array.ConvertAll(
                    mtx_list.InnerText.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries),
                    Convert.ToInt32);
                for (int i = 0; i < mtxList_size; i++)
                {
                    localToGlobalMatrixIDMap.Add(i, mtxList_data[i]);
                }

                XmlNode primitive_array = mtx_prim.SelectSingleNode("primitive_array");
                int primitiveArray_size = int.Parse(primitive_array.Attributes["size"].Value);

                XmlNodeList primitives = primitive_array.SelectNodes("primitive");

                int mtx = -1;
                int boneID = -1;
                Vector2? tex = null;
                Vector3? nrm = null;
                AccurateVertex pos = new AccurateVertex("0", "0", "0");
                Color clr = Color.White;
                foreach (XmlNode primitive in primitives)
                {
                    int primitive_index = int.Parse(primitive.Attributes["index"].Value);
                    string primitive_type = primitive.Attributes["type"].Value;
                    int primitive_vertex_size = int.Parse(primitive.Attributes["vertex_size"].Value);

                    ModelBase.PolyListType polyListType;
                    switch (primitive_type)
                    {
                        case "triangles": polyListType = ModelBase.PolyListType.Triangles; break;
                        case "quads": polyListType = ModelBase.PolyListType.Polygons; break;
                        case "triangle_strip": polyListType = ModelBase.PolyListType.TriangleStrip; break;
                        case "quad_strip": polyListType = ModelBase.PolyListType.QuadrilateralStrip; break;
                        default: goto case "triangles";
                    }

                    ModelBase.FaceListDef faceListDef = new ModelBase.FaceListDef(polyListType);

                    List<ModelBase.VertexDef> vertexList = new List<ModelBase.VertexDef>();
                    foreach (XmlNode child in primitive.ChildNodes)
                    {
                        switch (child.LocalName)
                        {
                            case "mtx":
                                {
                                    mtx = int.Parse(child.Attributes["idx"].Value);
                                    boneID = m_Model.m_BoneTree.GetBoneIndex(
                                        m_Model.m_BoneTransformsMap.GetBySecond(localToGlobalMatrixIDMap[mtx]));
                                }
                                break;
                            case "tex":
                                {
                                    float[] texArr = Array.ConvertAll(
                                        child.Attributes["st"].Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries),
                                        Convert.ToSingle);
                                    ModelBase.TextureDefBase texture = m_Model.m_Textures[m_Model.m_Materials[materialID].m_TextureDefID];
                                    tex = new Vector2((float)(texArr[0] / texture.GetWidth()), (float)(texArr[1] / texture.GetHeight()));
                                }
                                break;
                            case "nrm":
                                {
                                    float[] nrmArr = Array.ConvertAll(
                                        child.Attributes["xyz"].Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries),
                                        Convert.ToSingle);
                                    nrm = new Vector3(nrmArr[0], nrmArr[1], nrmArr[2]);
                                }
                                break;
                            case "clr":
                                {
                                    int[] clrArr = Array.ConvertAll(
                                        child.Attributes["rgb"].Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries),
                                        Convert.ToInt32);
                                    clr = Color.FromArgb(
                                        (int)(clrArr[0] / 31f * 255f), (int)(clrArr[1] / 31f * 255f), (int)(clrArr[2] / 31f * 255f));
                                }
                                break;
                            case "clr_idx":
                                {
                                    int idx = int.Parse(child.Attributes["idx"].Value);

                                    clr = vtx_color_data[idx];
                                }
                                break;
                            case "pos_xyz":
                                {
                                    string[] pos_xyz = child.Attributes["xyz"].Value.Split(new string[] { " " }, 
                                        StringSplitOptions.RemoveEmptyEntries);
                                    pos.x = new BasicBigFloat(pos_xyz[0]);
                                    pos.y = new BasicBigFloat(pos_xyz[1]);
                                    pos.z = new BasicBigFloat(pos_xyz[2]);

                                    AddVertexToList(boneID, tex, nrm, pos, this.model_info.pos_scale, clr, vertexList);
                                }
                                break;
                            case "pos_s": goto case "pos_xyz";
                            case "pos_xy":
                                {
                                    string[] pos_xy = child.Attributes["xy"].Value.Split(new string[] { " " },
                                        StringSplitOptions.RemoveEmptyEntries);
                                    pos.x = new BasicBigFloat(pos_xy[0]);
                                    pos.y = new BasicBigFloat(pos_xy[1]);

                                    AddVertexToList(boneID, tex, nrm, pos, this.model_info.pos_scale, clr, vertexList);
                                }
                                break;
                            case "pos_xz":
                                {
                                    string[] pos_xz = child.Attributes["xz"].Value.Split(new string[] { " " },
                                        StringSplitOptions.RemoveEmptyEntries);
                                    pos.x = new BasicBigFloat(pos_xz[0]);
                                    pos.z = new BasicBigFloat(pos_xz[1]);

                                    AddVertexToList(boneID, tex, nrm, pos, this.model_info.pos_scale, clr, vertexList);
                                }
                                break;
                            case "pos_yz":
                                {
                                    string[] pos_yz = child.Attributes["yz"].Value.Split(new string[] { " " },
                                        StringSplitOptions.RemoveEmptyEntries);
                                    pos.y = new BasicBigFloat(pos_yz[0]);
                                    pos.z = new BasicBigFloat(pos_yz[1]);

                                    AddVertexToList(boneID, tex, nrm, pos, this.model_info.pos_scale, clr, vertexList);
                                }
                                break;
                            case "pos_diff":
                                {
                                    // (-0.125 ≤ real numbers ≤ 0.125) (x3)
                                    string[] pos_diff = child.Attributes["xyz"].Value.Split(new string[] { " " },
                                        StringSplitOptions.RemoveEmptyEntries);
                                    pos.x = BasicBigFloat.Add(pos.x, new BasicBigFloat(pos_diff[0]));
                                    pos.y = BasicBigFloat.Add(pos.y, new BasicBigFloat(pos_diff[1]));
                                    pos.z = BasicBigFloat.Add(pos.z, new BasicBigFloat(pos_diff[2]));

                                    AddVertexToList(boneID, tex, nrm, pos, this.model_info.pos_scale, clr, vertexList);
                                }
                                break;
                            case "pos_idx":
                                {
                                    int idx = int.Parse(child.Attributes["idx"].Value);

                                    pos = this.vtx_pos_data[idx];

                                    AddVertexToList(boneID, tex, nrm, pos, this.model_info.pos_scale, clr, vertexList);
                                }
                                break;
                        }
                    }

                    switch (polyListType)
                    {
                        case ModelBase.PolyListType.Triangles:
                            {
                                int numVertsPerFace = 3;
                                int numFaces = primitive_vertex_size / numVertsPerFace;
                                for (int i = 0; i < numFaces; i++)
                                {
                                    ModelBase.FaceDef face = new ModelBase.FaceDef(numVertsPerFace);
                                    for (int j = 0; j < numVertsPerFace; j++)
                                    {
                                        face.m_Vertices[j] = vertexList[(i * numVertsPerFace) + j];
                                    }
                                    faceListDef.m_Faces.Add(face);
                                }
                            }
                            break;
                        case ModelBase.PolyListType.Polygons:
                            {
                                int numVertsPerFace = 4;
                                int numFaces = primitive_vertex_size / numVertsPerFace;
                                for (int i = 0; i < numFaces; i++)
                                {
                                    ModelBase.FaceDef face = new ModelBase.FaceDef(numVertsPerFace);
                                    for (int j = 0; j < numVertsPerFace; j++)
                                    {
                                        face.m_Vertices[j] = vertexList[(i * numVertsPerFace) + j];
                                    }
                                    faceListDef.m_Faces.Add(face);
                                }
                            }
                            break;
                        case ModelBase.PolyListType.TriangleStrip:
                            {
                                //3+(N-1) vertices per N triangles
                                //(N-3)+1 Triangles per N Vertices
                                int numFaces = primitive_vertex_size - 2;
                                for (int i = 0; i < numFaces; i++)
                                {
                                    ModelBase.FaceDef face = new ModelBase.FaceDef(3);
                                    if ((i & 1) == 0)
                                    {
                                        face.m_Vertices[0] = vertexList[i + 0];
                                        face.m_Vertices[1] = vertexList[i + 1];
                                        face.m_Vertices[2] = vertexList[i + 2];
                                    }
                                    else
                                    {
                                        face.m_Vertices[0] = vertexList[i + 2];
                                        face.m_Vertices[1] = vertexList[i + 1];
                                        face.m_Vertices[2] = vertexList[i + 0];
                                    }
                                    faceListDef.m_Faces.Add(face);
                                    //Because of how normals are defined in triangle strips, every 2nd triangle is clockwise, whereas all others are anti-clockwise
                                }
                            }
                            break;
                        case ModelBase.PolyListType.QuadrilateralStrip:
                            {
                                //4+(N-1)*2 vertices per N quads
                                //((N-4)/2) + 1 Quads. per N Vertices
                                int numFaces = ((primitive_vertex_size - 4) / 2) + 1;
                                int numVertsPerFace = 4;
                                for (int n = 0, p = 0; n < numFaces; n++, p = p + 2)
                                {
                                    ModelBase.FaceDef face = new ModelBase.FaceDef(numVertsPerFace);
                                    face.m_Vertices[0] = vertexList[p + 0];
                                    face.m_Vertices[1] = vertexList[p + 1];
                                    face.m_Vertices[2] = vertexList[p + 3];
                                    face.m_Vertices[3] = vertexList[p + 2];
                                    faceListDef.m_Faces.Add(face);
                                }
                            }
                            break;
                    }

                    polyListDef.m_FaceLists.Add(faceListDef);
                }

                geometryDef.m_PolyLists.Add("PolyList-" + geometryDef.m_PolyLists.Count, polyListDef);
            }
        }

        protected static void AddVertexToList(int boneID, Vector2? tex, Vector3? nrm, Vector3 pos, int pos_scale, 
            Color clr, List<ModelBase.VertexDef> vertexList)
        {
            Vector3 scaledPos = Vector3.Multiply(pos, (1 << pos_scale));
            ModelBase.VertexDef vertex = new ModelBase.VertexDef(scaledPos, tex, nrm, clr, boneID);
            vertexList.Add(vertex);
        }

        protected static void AddVertexToList(int boneID, Vector2? tex, Vector3? nrm, AccurateVertex pos, int pos_scale,
            Color clr, List<ModelBase.VertexDef> vertexList)
        {
            AccurateVertex scaledPos = new AccurateVertex(
                BasicBigFloat.IntMultiply(pos.x, (1 << pos_scale)), 
                BasicBigFloat.IntMultiply(pos.y, (1 << pos_scale)), 
                BasicBigFloat.IntMultiply(pos.z, (1 << pos_scale)));
            AddVertexToList(boneID, tex, nrm, new Vector3(scaledPos.x.ToFloat(), scaledPos.y.ToFloat(), scaledPos.z.ToFloat()),
                0, clr, vertexList);
        }

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

    public struct model_info
    {
        public int pos_scale;
        public string scaling_rule;
        public string vertex_style;
        public float magnify;
        public int tool_start_frame;
        public string tex_matrix_mode;
        public string compress_node;
        public int[] node_size;
        public bool compress_material;
        public int[] material_size;
        public string output_texture;
        public bool force_full_weight;
        public bool use_primitive_strip;
    }

    public struct AccurateVertex
    {
        public BasicBigFloat x;
        public BasicBigFloat y;
        public BasicBigFloat z;

        public AccurateVertex(BasicBigFloat x, BasicBigFloat y, BasicBigFloat z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public AccurateVertex(string x, string y, string z)
        {
            this.x = new BasicBigFloat(x);
            this.y = new BasicBigFloat(y);
            this.z = new BasicBigFloat(z);
        }
    }
}
