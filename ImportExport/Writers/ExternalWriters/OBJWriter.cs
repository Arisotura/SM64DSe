/* OBJWriter
 * 
 * Given a ModelBase object created by a Loader class, generates a Wavefront OBJ model. This OBJWriter
 * supports the non-standard "vc" vertex colour command, as used by the custom "Exteneded OBJ" Blender plugin.
 * 
 * Because OBJ does not support joints and skinning, bones are simulated by splitting the model into several 
 * chunks using the "o" object command and assigning vertices to bones on a per face basis and by creating 
 * a custom .bonelib file which the OBJLoader can then load in.
 */ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Globalization;
using OpenTK;

namespace SM64DSe.ImportExport.Writers.ExternalWriters
{
    public class OBJWriter : AbstractModelWriter
    {
        public OBJWriter(ModelBase model, string modelFileName) :
            base(model, modelFileName) { }

        public override void WriteModel(bool save = true)
        {
            StreamWriter objWriter = new StreamWriter(m_ModelFileName);
            StreamWriter mtlWriter = new StreamWriter(m_ModelFileName.Substring(0, m_ModelFileName.Length - 4) + ".mtl");
            string dir = Path.GetDirectoryName(m_ModelFileName);
            string baseFileName = Path.GetFileNameWithoutExtension(m_ModelFileName);
            objWriter.Write("#" + Program.AppTitle + " " + Program.AppVersion + " " + Program.AppDate + "\n\n");
            objWriter.Write("mtllib " + baseFileName + ".mtl" + "\n\n"); // Specify name of material library
            objWriter.Write("bonelib " + baseFileName + ".bones" + "\n\n"); // Specify name of bones list

            // OBJ does not support skinning, bones or vertex-specific object assignment so instead we use the bone ID 
            // specified in the first vertex of each face to assign that whole face to an object
            int boneIndex = 0;
            List<ModelBase.BoneDef> flatBoneList = m_Model.m_BoneTree.GetAsList();
            foreach (ModelBase.BoneDef bone in m_Model.m_BoneTree)
            {
                foreach (ModelBase.GeometryDef geometry in bone.m_Geometries.Values)
                {
                    foreach (ModelBase.PolyListDef polyList in geometry.m_PolyLists.Values)
                    {
                        int fl = 0;
                        while (fl < polyList.m_FaceLists.Count)
                        {
                            ModelBase.FaceListDef faceList = polyList.m_FaceLists[fl];

                            int f = 0;
                            int count = faceList.m_Faces.Count;
                            while (f < faceList.m_Faces.Count)
                            {
                                ModelBase.FaceDef face = faceList.m_Faces[f];

                                int objSplitBoneID = face.m_Vertices[0].m_VertexBoneIndex;
                                if (objSplitBoneID != boneIndex)
                                {
                                    ModelBase.BoneDef newBone = flatBoneList[objSplitBoneID];
                                    // For models using "areas" (multiple-parent bones) the vertices all seem to be 
                                    // assigned to the first parent bone. We don't want to move faces about in such 
                                    // cases so only move faces between bones within the same branch.
                                    if (bone.GetBranch().Contains(newBone))
                                    {
                                        if (!newBone.m_Geometries.ContainsKey(geometry.m_ID))
                                            newBone.m_Geometries.Add(geometry.m_ID, new ModelBase.GeometryDef(geometry.m_ID));
                                        if (!newBone.m_Geometries[geometry.m_ID].m_PolyLists.ContainsKey(polyList.m_MaterialName))
                                        {
                                            ModelBase.PolyListDef tmp = new ModelBase.PolyListDef(polyList.m_ID, polyList.m_MaterialName);
                                            tmp.m_FaceLists.Add(new ModelBase.FaceListDef());
                                            newBone.m_Geometries[geometry.m_ID].m_PolyLists.Add(polyList.m_MaterialName, tmp);
                                        }
                                        if (!newBone.m_MaterialsInBranch.Contains(polyList.m_MaterialName))
                                            newBone.m_MaterialsInBranch.Add(polyList.m_MaterialName);

                                        newBone.m_Geometries[geometry.m_ID].m_PolyLists[polyList.m_MaterialName].m_FaceLists[0].m_Faces.Add(face);
                                        faceList.m_Faces.RemoveAt(f);
                                    }
                                }

                                f++;
                            }

                            fl++;
                        }
                    }
                }
                
                boneIndex++;
            }

            // Write mtllib
            foreach (ModelBase.MaterialDef material in m_Model.m_Materials.Values)
            {
                //For every texture,
                string textureName = (material.m_TextureDefID != null) ? m_Model.m_Textures[material.m_TextureDefID].m_ID : null;
                //Create new material
                mtlWriter.Write("newmtl " /*+ ((i * 2) + j)*/ + material.m_ID + "\n");
                //Specify ambient colour - RGB 0-1
                mtlWriter.Write("Ka " + Helper.ToString(material.m_Ambient.R / 255.0f) +
                    " " + Helper.ToString(material.m_Ambient.G / 255.0f) +
                    " " + Helper.ToString(material.m_Ambient.B / 255.0f) + "\n");
                //Specify diffuse colour - RGB 0-1
                mtlWriter.Write("Kd " + Helper.ToString(material.m_Diffuse.R / 255.0f) +
                    " " + Helper.ToString(material.m_Diffuse.G / 255.0f) +
                    " " + Helper.ToString(material.m_Diffuse.B / 255.0f) + "\n");
                //Specify specular colour - RGB 0-1
                mtlWriter.Write("Ks " + Helper.ToString(material.m_Specular.R / 255.0f) +
                    " " + Helper.ToString(material.m_Specular.G / 255.0f) +
                    " " + Helper.ToString(material.m_Specular.B / 255.0f) + "\n");
                //Specify specular colour co-efficient - RGB 0-1
                //mtllib += "Ns " + material.m_SpeEmiColors + "\n";
                //Specify transparency - RGB Alpha channel 0-1
                mtlWriter.Write("d " + Helper.ToString(material.m_Alpha / 31f) + "\n");
                //Specify texture type 0 - 10
                //uint textype = (currentTexture.m_Params >> 26) & 0x7;
                mtlWriter.Write("illum 2\n");
                if (textureName != null && !textureName.Equals(""))
                {
                    //Specify name of texture image
                    mtlWriter.Write("map_Kd " + textureName + ".png" + "\n\n");
                    ExportTextureToPNG(dir, m_Model.m_Textures[material.m_TextureDefID]);
                }
                else
                    mtlWriter.Write("\n\n");
            }

            WriteBonesFileOBJ(m_ModelFileName.Substring(0, m_ModelFileName.Length - 4) + ".bones", m_Model.m_BoneTree);

            // Write each bone to file as a separate mesh/object using o command
            List<Vector3> verts = new List<Vector3>();
            List<Vector2> textureCoords = new List<Vector2>();
            List<Color> vertexColours = new List<Color>();
            foreach (ModelBase.BoneDef bone in m_Model.m_BoneTree)
            {
                objWriter.Write("o " + bone.m_ID + "\n");

                // Get a list of all verices and texture co-ordinates for the current bone
                List<Vector3> vertsCurBone = new List<Vector3>();
                List<Vector2> textureCoordsCurBone = new List<Vector2>();
                List<Color> vertexColoursCurBone = new List<Color>();

                foreach (ModelBase.GeometryDef geometry in bone.m_Geometries.Values)
                {
                    foreach (ModelBase.PolyListDef polyList in geometry.m_PolyLists.Values)
                    {
                        foreach (ModelBase.FaceListDef faceList in polyList.m_FaceLists)
                        {
                            foreach (ModelBase.FaceDef face in faceList.m_Faces)
                            {
                                foreach (ModelBase.VertexDef vert in face.m_Vertices)
                                {
                                    if (!vertsCurBone.Contains(vert.m_Position))
                                    {
                                        vertsCurBone.Add(vert.m_Position);
                                        verts.Add(vert.m_Position);
                                    }
                                    if (vert.m_TextureCoordinate != null && !textureCoordsCurBone.Contains((Vector2)vert.m_TextureCoordinate))
                                    {
                                        textureCoordsCurBone.Add((Vector2)vert.m_TextureCoordinate);
                                        textureCoords.Add((Vector2)vert.m_TextureCoordinate);
                                    }
                                    if (vert.m_VertexColour != null && !vertexColoursCurBone.Contains((Color)vert.m_VertexColour))
                                    {
                                        vertexColoursCurBone.Add((Color)vert.m_VertexColour);
                                        vertexColours.Add((Color)vert.m_VertexColour);
                                    }
                                }
                            }
                        }
                    }
                }

                // Print a list of all vertices, texture co-ordinates and vertex colours
                foreach (Vector3 vert in vertsCurBone)
                    objWriter.Write("v " + Helper.ToString(vert.X) + " " + Helper.ToString(vert.Y) + " " + Helper.ToString(vert.Z) + "\n");
                foreach (Vector2 textureCoord in textureCoordsCurBone)
                    objWriter.Write("vt " + Helper.ToString(textureCoord.X) + " " + Helper.ToString(textureCoord.Y) + "\n");
                foreach (Color vColour in vertexColoursCurBone)
                    objWriter.Write("vc " + Helper.ToString(vColour.R / 255.0f) + " " + Helper.ToString(vColour.G / 255.0f) + " " +
                        Helper.ToString(vColour.B / 255.0f) + "\n");

                // For each material used in the current bone, print all faces
                foreach (ModelBase.GeometryDef geometry in bone.m_Geometries.Values)
                {
                    foreach (ModelBase.PolyListDef polyList in geometry.m_PolyLists.Values)
                    {
                        objWriter.Write("usemtl " + polyList.m_MaterialName + "\n");

                        foreach (ModelBase.FaceListDef faceList in polyList.m_FaceLists)
                        {
                            foreach (ModelBase.FaceDef face in faceList.m_Faces)
                            {
                                // Each face is a triangle or a quad, as they have already been extracted individually from
                                // the vertex lists
                                // Note: Indices start at 1 in OBJ
                                int numVerticesInFace = face.m_NumVertices;

                                objWriter.Write("f ");
                                foreach (ModelBase.VertexDef vert in face.m_Vertices)
                                {
                                    objWriter.Write((verts.LastIndexOf(vert.m_Position) + 1) +
                                        "/" + ((vert.m_TextureCoordinate != null) ?
                                        (textureCoords.LastIndexOf((Vector2)vert.m_TextureCoordinate) + 1).ToString() : "") +
                                        "//" + ((vert.m_VertexColour != null) ?
                                        (vertexColours.LastIndexOf((Color)vert.m_VertexColour) + 1).ToString() : "") + " ");
                                }
                                objWriter.Write("\n");
                            }
                        }
                    }
                }
            }

            objWriter.Close();
            mtlWriter.Close();
        }

        private static void WriteBonesFileOBJ(string filename, ModelBase.BoneDefRoot boneTree)
        {
            // Write information about each bone, whether it's a parent or child to file
            // This is specific to this editor and will need re-added when re-importing if model modified

            StreamWriter bonesWriter = new StreamWriter(filename);

            foreach (ModelBase.BoneDef bone in boneTree)
            {
                bonesWriter.Write("newbone " + bone.m_ID + "\n");
                // Offset in bones to parent bone (signed 16-bit. 0=no parent, -1=parent is the previous bone, ...)
                bonesWriter.Write("parent_offset " + boneTree.GetParentOffset(bone) + "\n");
                // 1 if the bone has children, 0 otherwise
                bonesWriter.Write("has_children " + ((bone.m_HasChildren) ? "1" : "0") + "\n");
                // Offset in bones to the next sibling bone (0=bone is last child of its parent)
                bonesWriter.Write("sibling_offset " + boneTree.GetNextSiblingOffset(bone) + "\n");
                // Scale component
                bonesWriter.Write("scale " + bone.m_20_12Scale[0].ToString("X8") + " " + bone.m_20_12Scale[1].ToString("X8") + " " + 
                    bone.m_20_12Scale[2].ToString("X8") + "\n");
                // Rotation component
                bonesWriter.Write("rotation " + bone.m_4_12Rotation[0].ToString("X4") + " " + bone.m_4_12Rotation[1].ToString("X4") + " " + 
                    bone.m_4_12Rotation[2].ToString("X4") + "\n");
                // Translation component
                bonesWriter.Write("translation " + bone.m_20_12Translation[0].ToString("X8") + " " + bone.m_20_12Translation[0].ToString("X8") + 
                    " " + bone.m_20_12Translation[2].ToString("X8") + "\n\n");
            }

            bonesWriter.Close();
        }

    }
}
