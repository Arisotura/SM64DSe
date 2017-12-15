/* OBJLoader
 * 
 * Converts a Wavefront OBJ model into a ModelBase object for use in BMDImporter and KCLImporter.
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
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Security.Cryptography;
using SM64DSe.ImportExport.Loaders;

namespace SM64DSe.ImportExport.Loaders.ExternalLoaders
{
    public class OBJLoader : AbstractModelLoader
    {
        public List<Vector4> m_Vertices;
        public List<Vector2> m_TexCoords;
        public List<Vector3> m_Normals;
        public List<Color> m_Colours;
        public List<int> m_VertexBoneIDs;
        private bool m_SketchupHack;

        public OBJLoader(string modelFileName) : 
            base(modelFileName)
        {
            m_Vertices = new List<Vector4>();
            m_TexCoords = new List<Vector2>();
            m_Normals = new List<Vector3>();
            m_Colours = new List<Color>();
            m_VertexBoneIDs = new List<int>();

            m_SketchupHack = false;
        }

        public override ModelBase LoadModel(float scale)
        {
            if (m_ModelFileName == null || "".Equals(m_ModelFileName))
                throw new SystemException("You must specify the filename of the model to load via the constructor before " +
                    "calling LoadModel()");

            Stream fs = File.OpenRead(m_ModelFileName);
            StreamReader sr = new StreamReader(fs);

            string curmaterial = null;

            bool foundObjects = LoadDefaultBones(m_ModelFileName);
            string currentBone = null;
            int currentBoneIndex = -1;
            if (!foundObjects)
            {
                currentBone = "default_bone_name";
                ModelBase.BoneDef defaultBone = new ModelBase.BoneDef(currentBone);
                defaultBone.m_Geometries.Add("geometry-0", new ModelBase.GeometryDef("geometry-0"));
                m_Model.m_BoneTree.AddRootBone(defaultBone);
                currentBoneIndex = m_Model.m_BoneTree.GetBoneIndex(defaultBone);
            }

            string curline;
            while ((curline = sr.ReadLine()) != null)
            {
                curline = curline.Trim();

                // skip empty lines and comments
                if (curline.Length < 1) continue;
                if (curline[0] == '#') continue;

                string[] parts = curline.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 1) continue;

                switch (parts[0])
                {
                    case "mtllib": // material lib file
                        {
                            string filename = curline.Substring(parts[0].Length + 1).Trim();
                            LoadMaterials(m_ModelPath + Path.DirectorySeparatorChar + filename);
                        }
                        break;

                    case "bonelib": // bone definitions file
                        {
                            string filename = curline.Substring(parts[0].Length + 1).Trim();
                            LoadBoneDefinitionsForOBJ(m_ModelPath + Path.DirectorySeparatorChar + filename);
                        }
                        break;

                    case "o": // object (bone)
                        if (parts.Length < 2) continue;
                        currentBone = parts[1];
                        m_Model.m_BoneTree.GetBoneByID(currentBone).m_Geometries.Add(currentBone, new ModelBase.GeometryDef(currentBone));
                        currentBoneIndex = m_Model.m_BoneTree.GetBoneIndex(currentBone);
                        break;

                    case "usemtl": // material name
                        if (parts.Length < 2) continue;
                        curmaterial = parts[1];
                        if (!m_Model.m_Materials.ContainsKey(curmaterial))
                        {
                            curmaterial = "default_white";
                            AddWhiteMat(currentBone);
                        }
                        break;

                    case "v": // vertex
                        {
                            if (parts.Length < 4) continue;
                            float x = Helper.ParseFloat(parts[1]);
                            float y = Helper.ParseFloat(parts[2]);
                            float z = Helper.ParseFloat(parts[3]);
                            float w = 1f; //(parts.Length < 5) ? 1f : Helper.ParseFloat(parts[4]);

                            m_Vertices.Add(new Vector4(x, y, z, w));
                            m_VertexBoneIDs.Add(currentBoneIndex);
                        }
                        break;

                    case "vt": // texcoord
                        {
                            if (parts.Length < 2) continue;
                            float s = Helper.ParseFloat(parts[1]);
                            float t = (parts.Length < 3) ? 0f : Helper.ParseFloat(parts[2]);

                            m_TexCoords.Add(new Vector2(s, t));
                        }
                        break;

                    case "vn": // normal
                        {
                            if (parts.Length < 4) continue;
                            float x = Helper.ParseFloat(parts[1]);
                            float y = Helper.ParseFloat(parts[2]);
                            float z = Helper.ParseFloat(parts[3]);

                            Vector3 vec = new Vector3(x, y, z).Normalized();
                            m_Normals.Add(vec);
                        }
                        break;

                    case "vc": // vertex colour (non-standard "Extended OBJ" Blender plugin only)
                        {
                            if (parts.Length < 4) continue;
                            float r = Helper.ParseFloat(parts[1]);
                            float g = Helper.ParseFloat(parts[2]);
                            float b = Helper.ParseFloat(parts[3]);

                            Color vcolour = Color.FromArgb((int)(r * 255.0f), (int)(g * 255.0f), (int)(b * 255.0f));
                            m_Colours.Add(vcolour);
                        }
                        break;

                    case "f": // face
                        {
                            if (parts.Length < 4) continue;
                            int nvtx = parts.Length - 1;

                            if (curmaterial != null)
                            {
                                // If a new object is defined but a material to use not set, we need to use the previous one and add 
                                // it to the current bone and its parent
                                if (!m_Model.m_BoneTree.GetBoneByID(currentBone).GetRoot().m_MaterialsInBranch.Contains(curmaterial))
                                    m_Model.m_BoneTree.GetBoneByID(currentBone).GetRoot().m_MaterialsInBranch.Add(curmaterial);
                                if (!m_Model.m_BoneTree.GetBoneByID(currentBone).m_MaterialsInBranch.Contains(curmaterial))
                                    m_Model.m_BoneTree.GetBoneByID(currentBone).m_MaterialsInBranch.Add(curmaterial);
                            }
                            else
                            {
                                // No "usemtl" command before declaring face
                                curmaterial = "default_white";
                                AddWhiteMat(currentBone);
                            }

                            ModelBase.BoneDef bone = m_Model.m_BoneTree.GetBoneByID(currentBone);
                            ModelBase.GeometryDef geomDef = bone.m_Geometries.Values.ElementAt(0);
                            string polyListKey = "polylist-" + curmaterial;
                            ModelBase.PolyListDef polyList;
                            if (!geomDef.m_PolyLists.TryGetValue(polyListKey, out polyList))
                            {
                                polyList = new ModelBase.PolyListDef(polyListKey, curmaterial);
                                polyList.m_FaceLists.Add(new ModelBase.FaceListDef());
                                geomDef.m_PolyLists.Add(polyList.m_ID, polyList);
                            }

                            ModelBase.FaceDef face = new ModelBase.FaceDef(nvtx);

                            for (int i = 0; i < nvtx; i++)
                            {
                                string vtx = parts[i + 1];
                                string[] idxs = vtx.Split(new char[] { '/' });

                                ModelBase.VertexDef vert = ModelBase.EMPTY_VERTEX;

                                vert.m_Position = new Vector3(m_Vertices[int.Parse(idxs[0]) - 1].Xyz);
                                if (m_Model.m_Materials[curmaterial].m_TextureDefID != null && idxs.Length >= 2 && idxs[1].Length > 0)
                                {
                                    vert.m_TextureCoordinate = m_TexCoords[int.Parse(idxs[1]) - 1];
                                }
                                else
                                {
                                    vert.m_TextureCoordinate = null;
                                }
                                if (m_Model.m_Materials[curmaterial].m_Lights.Contains(true) && idxs.Length >= 3 && idxs[2].Trim().Length > 0)
                                {
                                    vert.m_Normal = new Vector3(m_Normals[int.Parse(idxs[2]) - 1]);
                                }
                                else
                                {
                                    vert.m_Normal = null;
                                }
                                // Vertex colours (non-standard "Extended OBJ" Blender plugin only)
                                if (idxs.Length >= 4 && !idxs[3].Equals(""))
                                {
                                    Color tmp = m_Colours[int.Parse(idxs[3]) - 1];
                                    vert.m_VertexColour = Color.FromArgb(tmp.A, tmp.R, tmp.G, tmp.B);
                                }
                                else
                                {
                                    vert.m_VertexColour = Color.White;
                                }
                                vert.m_VertexBoneIndex = currentBoneIndex;

                                face.m_Vertices[i] = vert;
                            }

                            polyList.m_FaceLists[0].m_Faces.Add(face);
                        }
                        break;
                }
            }

            int count = 0;
            foreach (ModelBase.BoneDef boneDef in m_Model.m_BoneTree)
            {
                m_Model.m_BoneTransformsMap.Add(boneDef.m_ID, count);
                count++;
            }

            sr.Close();

            m_Model.ScaleModel(scale);

            return m_Model;
        }

        private void LoadMaterials(string filename)
        {
            Stream fs;
            try
            {
                fs = File.OpenRead(filename);
            }
            catch
            {
                MessageBox.Show("Material library not found:\n\n" + filename + "\n\nA default white material will be used instead.");
                AddWhiteMat();
                return;
            }
            StreamReader sr = new StreamReader(fs);

            string curmaterial = "";

            string imagesNotFound = "";

            string curline;
            while ((curline = sr.ReadLine()) != null)
            {
                curline = curline.Trim();

                // skip empty lines and comments
                if (curline.Length < 1) continue;
                if (curline[0] == '#')
                {
                    if (curline == "#Materials exported from Google Sketchup")
                        m_SketchupHack = true;

                    continue;
                }

                string[] parts = curline.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 1) continue;

                switch (parts[0])
                {
                    case "newmtl": // new material definition
                        {
                            if (parts.Length < 2) continue;
                            curmaterial = parts[1];

                            ModelBase.MaterialDef mat = new ModelBase.MaterialDef(curmaterial);
                            if (!m_Model.m_Materials.ContainsKey(curmaterial))
                                m_Model.m_Materials.Add(curmaterial, mat);
                        }
                        break;

                    case "d":
                    case "Tr": // opacity
                        {
                            if (parts.Length < 2) continue;
                            float o = Helper.ParseFloat(parts[1]);
                            if (m_SketchupHack)
                                o *= 31;

                            ModelBase.MaterialDef mat = (ModelBase.MaterialDef)m_Model.m_Materials[curmaterial];
                            mat.m_Alpha = (byte)(Math.Max(0, Math.Min(31, (int)(o * 31))));
                        }
                        break;

                    case "Kd": // diffuse colour
                        {
                            if (parts.Length < 4) continue;
                            float r = Helper.ParseFloat(parts[1]);
                            float g = Helper.ParseFloat(parts[2]);
                            float b = Helper.ParseFloat(parts[3]);
                            Color col = Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));

                            ModelBase.MaterialDef mat = (ModelBase.MaterialDef)m_Model.m_Materials[curmaterial];
                            mat.m_Diffuse = col;
                        }
                        break;

                    case "Ka": // ambient colour
                        {
                            if (parts.Length < 4) continue;
                            float r = Helper.ParseFloat(parts[1]);
                            float g = Helper.ParseFloat(parts[2]);
                            float b = Helper.ParseFloat(parts[3]);
                            Color col = Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));

                            ModelBase.MaterialDef mat = (ModelBase.MaterialDef)m_Model.m_Materials[curmaterial];
                            mat.m_Ambient = col;
                        }
                        break;

                    case "Ks": // specular colour
                        {
                            if (parts.Length < 4) continue;
                            float r = Helper.ParseFloat(parts[1]);
                            float g = Helper.ParseFloat(parts[2]);
                            float b = Helper.ParseFloat(parts[3]);
                            Color col = Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));

                            ModelBase.MaterialDef mat = (ModelBase.MaterialDef)m_Model.m_Materials[curmaterial];
                            mat.m_Specular = col;
                        }
                        break;

                    case "map_Kd":
                    case "mapKd": // diffuse map (texture)
                        {
                            string texname = curline.Substring(parts[0].Length + 1).Trim();
                            string fullPath = (File.Exists(texname)) ? texname : (m_ModelPath + Path.DirectorySeparatorChar + texname);
                            ModelBase.TextureDefBase texture = new
                                ModelBase.TextureDefExternalBitmap(texname, fullPath);
                            AddTexture(texture, m_Model.m_Materials[curmaterial]);
                            break;
                        }

                    case "PDF":
                        {
                            ModelBase.MaterialDef mat = (ModelBase.MaterialDef)m_Model.m_Materials[curmaterial];
                            switch (parts[1].ToLowerInvariant())
                            {
                                case "front":
                                    mat.m_PolygonDrawingFace = ModelBase.MaterialDef.PolygonDrawingFace.Front; break;
                                case "back":
                                    mat.m_PolygonDrawingFace = ModelBase.MaterialDef.PolygonDrawingFace.Back; break;
                                case "both":
                                    mat.m_PolygonDrawingFace = ModelBase.MaterialDef.PolygonDrawingFace.FrontAndBack; break;
                                default: goto case "front";
                            }
                        }
                        break;
                }
            }

            if (!imagesNotFound.Equals(""))
                MessageBox.Show("The following images were not found:\n\n" + imagesNotFound);

            sr.Close();
        }

        protected void LoadBoneDefinitionsForOBJ(string filename)
        {
            Stream fs;
            try
            {
                fs = File.OpenRead(filename);
            }
            catch
            {
                MessageBox.Show("Specified Bone definitions not found:\n\n" + filename + "\n\nUsing default values.");
                return;
            }
            StreamReader sr = new StreamReader(fs);

            m_Model.m_BoneTree.Clear();

            ModelBase.BoneDef bone = null;

            string curline;
            while ((curline = sr.ReadLine()) != null)
            {
                curline = curline.Trim();

                // skip empty lines and comments
                if (curline.Length < 1) continue;
                if (curline[0] == '#')
                {
                    continue;
                }

                string[] parts = curline.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 1) continue;

                switch (parts[0])
                {
                    case "newbone": // new bone definition
                        {
                            if (parts.Length < 2) continue;

                            bone = new ModelBase.BoneDef(parts[1]);
                        }
                        break;

                    case "parent_offset": // Offset in bones to parent bone (signed 16-bit. 0=no parent, -1=parent is the previous bone, ...)
                        {
                            if (parts.Length < 2) continue;
                            short parent_offset = short.Parse(parts[1]);

                            if ((parent_offset < 0 && m_Model.m_BoneTree.Count == 0) || parent_offset > 0)
                            {
                                throw new SystemException("Child bones cannot be defined before their parent in: " + filename);
                            }

                            if (parent_offset == 0)
                            {
                                m_Model.m_BoneTree.AddRootBone(bone);
                            }
                            else if (parent_offset < 0)
                            {
                                List<ModelBase.BoneDef> listOfBones = m_Model.m_BoneTree.GetAsList();
                                listOfBones[listOfBones.Count + parent_offset].AddChild(bone);
                            }
                        }
                        break;

                    case "has_children": // 1 if the bone has children, 0 otherwise
                        {
                            if (parts.Length < 2) continue;
                            bool has_children = (short.Parse(parts[1]) == 1);
                            // No longer needed
                        }
                        break;

                    case "sibling_offset": // Offset in bones to the next sibling bone (0=bone is last child of its parent)
                        {
                            if (parts.Length < 2) continue;
                            short sibling_offset = short.Parse(parts[1]);
                            // No longer needed
                        }
                        break;

                    case "scale": // Scale transformation
                        {
                            if (parts.Length < 4) continue;
                            uint[] scale = new uint[] { uint.Parse(parts[1], System.Globalization.NumberStyles.HexNumber), 
                                uint.Parse(parts[2], System.Globalization.NumberStyles.HexNumber), 
                                uint.Parse(parts[3], System.Globalization.NumberStyles.HexNumber) };

                            bone.SetScale(scale);
                        }
                        break;

                    case "rotation": // Rotation transformation
                        {
                            if (parts.Length < 4) continue;
                            ushort[] rotation = new ushort[] { ushort.Parse(parts[1], System.Globalization.NumberStyles.HexNumber), 
                                ushort.Parse(parts[2], System.Globalization.NumberStyles.HexNumber), 
                                ushort.Parse(parts[3], System.Globalization.NumberStyles.HexNumber) };

                            bone.SetRotation(rotation);
                        }
                        break;

                    case "translation": // Scale transformation
                        {
                            if (parts.Length < 4) continue;
                            uint[] translation = new uint[] { uint.Parse(parts[1], System.Globalization.NumberStyles.HexNumber), 
                                uint.Parse(parts[2], System.Globalization.NumberStyles.HexNumber), 
                                uint.Parse(parts[3], System.Globalization.NumberStyles.HexNumber) };

                            bone.SetTranslation(translation);
                        }
                        break;
                    case "billboard": // Always rendered facing camera
                        {
                            if (parts.Length < 2) continue;
                            bool billboard = (short.Parse(parts[1]) == 1);

                            bone.m_Billboard = billboard;
                        }
                        break;
                }
            }
            
            // Calculate transformations and inverse transformations
            foreach (ModelBase.BoneDef boneDef in m_Model.m_BoneTree.GetRootBones())
            {
                boneDef.CalculateBranchTransformations();
            }

            sr.Close();
        }

        /* Creates a list of bones based on object names found in OBJ file and assigns them 
         * default values. These will be replaced if a bone definition file is found.
         * By default there is one root parent bone and every other bone is a child bone with one child (until the end)
         */
        protected bool LoadDefaultBones(string modelFileName)
        {
            Stream fs = File.OpenRead(m_ModelFileName);
            StreamReader sr = new StreamReader(fs);

            bool foundObjects = false;

            ModelBase.BoneDef bone = null;

            string curline;
            while ((curline = sr.ReadLine()) != null)
            {
                curline = curline.Trim();

                // skip empty lines and comments
                if (curline.Length < 1) continue;
                if (curline[0] == '#') continue;

                string[] parts = curline.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 1) continue;

                switch (parts[0])
                {
                    case "o": // object (bone)
                        {
                            bone = new ModelBase.BoneDef(parts[1]);

                            if (m_Model.m_BoneTree.Count == 0)
                            {
                                m_Model.m_BoneTree.AddRootBone(bone);
                            }
                            else
                            {
                                m_Model.m_BoneTree.GetAsList()[m_Model.m_BoneTree.Count - 1].AddChild(bone);
                            }

                            foundObjects = true;
                        }
                        break;
                }
            }

            sr.Close();

            return foundObjects;
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
}
