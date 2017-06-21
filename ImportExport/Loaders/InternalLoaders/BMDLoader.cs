/* BMDLoader
 * 
 * Given a BMD object, produces a ModelBase object for use in the Writer classes.
 */ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using SM64DSe.ImportExport.Writers.InternalWriters;
using System.Windows.Forms;
using OpenTK;

namespace SM64DSe.ImportExport.Loaders.InternalLoaders
{
    public class BMDLoader : AbstractModelLoader
    {
        BMD m_BMD;

        public BMDLoader(string modelFileName, BMD bmd) :
            base(modelFileName)
        {
            m_BMD = bmd;
        }

        public override ModelBase LoadModel(float scale)
        {
            foreach (BMD.ModelChunk mdchunk in m_BMD.m_ModelChunks)
            {
                ModelBase.BoneDef bone = new ModelBase.BoneDef(mdchunk.m_Name);
                bone.SetScale(mdchunk.m_20_12Scale);
                bone.SetRotation(mdchunk.m_4_12Rotation);
                bone.SetTranslation(mdchunk.m_20_12Translation);
                bone.m_Billboard = mdchunk.m_Billboard;

                if (mdchunk.m_ParentOffset == 0)
                {
                    m_Model.m_BoneTree.AddRootBone(bone);
                }
                else
                {
                    List<ModelBase.BoneDef> listOfBones = m_Model.m_BoneTree.GetAsList();
                    listOfBones[listOfBones.Count + mdchunk.m_ParentOffset].AddChild(bone);
                }

                m_Model.m_BoneTransformsMap.Add(bone.m_ID, m_Model.m_BoneTransformsMap.Count);

                ModelBase.GeometryDef geomDef = null;
                if (mdchunk.m_MatGroups.Length > 0)
                {
                    geomDef = new ModelBase.GeometryDef("geometry-0");
                    bone.m_Geometries.Add(geomDef.m_ID, geomDef);
                }

                foreach (BMD.MaterialGroup matgroup in mdchunk.m_MatGroups)
                {
                    string polyListKey = "polylist-" + matgroup.m_Name;
                    ModelBase.PolyListDef polyListDef;
                    if (!geomDef.m_PolyLists.TryGetValue(polyListKey, out polyListDef))
                    {
                        polyListDef = new ModelBase.PolyListDef(polyListKey, matgroup.m_Name);
                        geomDef.m_PolyLists.Add(polyListDef.m_ID, polyListDef);
                    }

                    ModelBase.MaterialDef material = new ModelBase.MaterialDef(matgroup.m_Name);
                    material.m_Diffuse = matgroup.m_DiffuseColor;
                    material.m_Ambient = matgroup.m_AmbientColor;
                    material.m_Specular = matgroup.m_SpecularColor;
                    material.m_Emission = matgroup.m_EmissionColor;
                    bool hasTextures = (matgroup.m_Texture != null);
                    if (hasTextures)
                    {
                        if (!m_Model.m_Textures.ContainsKey(matgroup.m_Texture.m_TextureName))
                        {
                            ModelBase.TextureDefBase texture = new ModelBase.TextureDefNitro(matgroup.m_Texture);
                            m_Model.m_Textures.Add(texture.m_ID, texture);
                        }

                        material.m_TextureDefID = matgroup.m_Texture.m_TextureName;
                    }
                    material.m_Alpha = matgroup.m_Alpha;
                    if ((matgroup.m_PolyAttribs & 0xC0) == 0xC0)
                        material.m_PolygonDrawingFace = ModelBase.MaterialDef.PolygonDrawingFace.FrontAndBack;
                    else if ((matgroup.m_PolyAttribs & 0xC0) == 0x80)
                        material.m_PolygonDrawingFace = ModelBase.MaterialDef.PolygonDrawingFace.Front;
                    else if ((matgroup.m_PolyAttribs & 0xC0) == 0x40)
                        material.m_PolygonDrawingFace = ModelBase.MaterialDef.PolygonDrawingFace.Back;

                    material.m_TexGenMode = (ModelBase.TexGenMode)(matgroup.m_TexParams >> 30);

                    material.m_TextureScale = matgroup.m_TexCoordScale;
                    material.m_TextureRotation = matgroup.m_TexCoordRot;
                    material.m_TextureTranslation = matgroup.m_TexCoordTrans;

                    byte sRepeat = (byte)((matgroup.m_TexParams & 0x10000) >> 0x10);
                    byte tRepeat = (byte)((matgroup.m_TexParams & 0x20000) >> 0x11);
                    byte sFlip = (byte)((matgroup.m_TexParams & 0x40000) >> 0x12);
                    byte tFlip = (byte)((matgroup.m_TexParams & 0x80000) >> 0x13);

                    material.m_TexTiling[0] = (sRepeat == 1) ? ModelBase.MaterialDef.TexTiling.Repeat : ModelBase.MaterialDef.TexTiling.Clamp;
                    material.m_TexTiling[0] = (sFlip == 1) ? ModelBase.MaterialDef.TexTiling.Flip : material.m_TexTiling[0];
                    material.m_TexTiling[1] = (tRepeat == 1) ? ModelBase.MaterialDef.TexTiling.Repeat : ModelBase.MaterialDef.TexTiling.Clamp;
                    material.m_TexTiling[1] = (tFlip == 1) ? ModelBase.MaterialDef.TexTiling.Flip : material.m_TexTiling[1];

                    material.m_FogFlag = (matgroup.m_PolyAttribs & 0x8000) > 0;

                    byte lights = (byte)(matgroup.m_PolyAttribs & 0x0F);
                    for (int i = 0; i < 4; i++)
                    {
                        byte value = (byte)(lights >> i);
                        material.m_Lights[i] = (value == 1);
                    }

                    if (!m_Model.m_Materials.ContainsKey(material.m_ID))
                        m_Model.m_Materials.Add(material.m_ID, material);

                    if (!bone.m_MaterialsInBranch.Contains(matgroup.m_Name))
                    {
                        bone.m_MaterialsInBranch.Add(matgroup.m_Name);
                    }
                    ModelBase.BoneDef upToRoot = bone;
                    while ((upToRoot = upToRoot.m_Parent) != null)
                    {
                        if (!upToRoot.m_MaterialsInBranch.Contains(matgroup.m_Name))
                            upToRoot.m_MaterialsInBranch.Add(matgroup.m_Name);
                    }

                    foreach (BMD.VertexList geometry in matgroup.m_Geometry)
                    {
                        uint polyType = geometry.m_PolyType;
                        List<BMD.Vertex> vtxList = geometry.m_VertexList;

                        switch (polyType)
                        {
                            case 0://Separate Triangles
                                {
                                    ModelBase.FaceListDef faceList = new ModelBase.FaceListDef(ModelBase.PolyListType.Triangles);
                                    int numFaces = vtxList.Count / 3;
                                    for (int a = 0, b = 0; a < numFaces; a++, b = b + 3)
                                    {
                                        ModelBase.FaceDef face = new ModelBase.FaceDef(3);

                                        face.m_Vertices[0] = new ModelBase.VertexDef(vtxList[b + 0].m_Position, vtxList[b + 0].m_TexCoord,
                                            vtxList[b + 0].m_Normal, vtxList[b + 0].m_Color, (int)matgroup.m_BoneIDs[vtxList[b + 0].m_MatrixID]);
                                        face.m_Vertices[1] = new ModelBase.VertexDef(vtxList[b + 1].m_Position, vtxList[b + 1].m_TexCoord,
                                            vtxList[b + 1].m_Normal, vtxList[b + 1].m_Color, (int)matgroup.m_BoneIDs[vtxList[b + 1].m_MatrixID]);
                                        face.m_Vertices[2] = new ModelBase.VertexDef(vtxList[b + 2].m_Position, vtxList[b + 2].m_TexCoord,
                                            vtxList[b + 2].m_Normal, vtxList[b + 2].m_Color, (int)matgroup.m_BoneIDs[vtxList[b + 2].m_MatrixID]);

                                        faceList.m_Faces.Add(face);
                                    }
                                    polyListDef.m_FaceLists.Add(faceList);
                                    break;
                                }
                            case 1://Separate Quadrilaterals
                                {
                                    ModelBase.FaceListDef faceList = new ModelBase.FaceListDef(ModelBase.PolyListType.Polygons);
                                    int numFaces = vtxList.Count / 4;
                                    for (int a = 0, b = 0; a < numFaces; a++, b = b + 4)
                                    {
                                        ModelBase.FaceDef face = new ModelBase.FaceDef(4);

                                        face.m_Vertices[0] = new ModelBase.VertexDef(vtxList[b + 0].m_Position, vtxList[b + 0].m_TexCoord,
                                            vtxList[b + 0].m_Normal, vtxList[b + 0].m_Color, (int)matgroup.m_BoneIDs[vtxList[b + 0].m_MatrixID]);
                                        face.m_Vertices[1] = new ModelBase.VertexDef(vtxList[b + 1].m_Position, vtxList[b + 1].m_TexCoord,
                                            vtxList[b + 1].m_Normal, vtxList[b + 1].m_Color, (int)matgroup.m_BoneIDs[vtxList[b + 1].m_MatrixID]);
                                        face.m_Vertices[2] = new ModelBase.VertexDef(vtxList[b + 2].m_Position, vtxList[b + 2].m_TexCoord,
                                            vtxList[b + 2].m_Normal, vtxList[b + 2].m_Color, (int)matgroup.m_BoneIDs[vtxList[b + 2].m_MatrixID]);
                                        face.m_Vertices[3] = new ModelBase.VertexDef(vtxList[b + 3].m_Position, vtxList[b + 3].m_TexCoord,
                                            vtxList[b + 3].m_Normal, vtxList[b + 3].m_Color, (int)matgroup.m_BoneIDs[vtxList[b + 3].m_MatrixID]);

                                        faceList.m_Faces.Add(face);
                                    }
                                    polyListDef.m_FaceLists.Add(faceList);
                                    break;
                                }
                            case 2://Triangle Strips
                                {
                                    //3+(N-1) vertices per N triangles
                                    //(N-3)+1 Triangles per N Vertices
                                    int numFaces = vtxList.Count - 2;
                                    if (vtxList.Count < 3)//Should never be
                                        break;
                                    ModelBase.FaceListDef faceList = new ModelBase.FaceListDef(ModelBase.PolyListType.TriangleStrip);
                                    //Convert all faces with more than 3 vertices to ones with only 3
                                    for (int n = 0; n < numFaces; n++)
                                    {
                                        if (n % 2 == 0)
                                        {
                                            ModelBase.FaceDef face = new ModelBase.FaceDef(3);

                                            face.m_Vertices[0] = new ModelBase.VertexDef(vtxList[n + 0].m_Position, vtxList[n + 0].m_TexCoord,
                                                vtxList[n + 0].m_Normal, vtxList[n + 0].m_Color, (int)matgroup.m_BoneIDs[vtxList[n + 0].m_MatrixID]);
                                            face.m_Vertices[1] = new ModelBase.VertexDef(vtxList[n + 1].m_Position, vtxList[n + 1].m_TexCoord,
                                                vtxList[n + 1].m_Normal, vtxList[n + 1].m_Color, (int)matgroup.m_BoneIDs[vtxList[n + 1].m_MatrixID]);
                                            face.m_Vertices[2] = new ModelBase.VertexDef(vtxList[n + 2].m_Position, vtxList[n + 2].m_TexCoord,
                                                vtxList[n + 2].m_Normal, vtxList[n + 2].m_Color, (int)matgroup.m_BoneIDs[vtxList[n + 2].m_MatrixID]);

                                            faceList.m_Faces.Add(face);
                                        }
                                        else
                                        {
                                            ModelBase.FaceDef face = new ModelBase.FaceDef(3);

                                            face.m_Vertices[0] = new ModelBase.VertexDef(vtxList[n + 2].m_Position, vtxList[n + 2].m_TexCoord,
                                                vtxList[n + 2].m_Normal, vtxList[n + 2].m_Color, (int)matgroup.m_BoneIDs[vtxList[n + 2].m_MatrixID]);
                                            face.m_Vertices[1] = new ModelBase.VertexDef(vtxList[n + 1].m_Position, vtxList[n + 1].m_TexCoord,
                                                vtxList[n + 1].m_Normal, vtxList[n + 1].m_Color, (int)matgroup.m_BoneIDs[vtxList[n + 1].m_MatrixID]);
                                            face.m_Vertices[2] = new ModelBase.VertexDef(vtxList[n + 0].m_Position, vtxList[n + 0].m_TexCoord,
                                                vtxList[n + 0].m_Normal, vtxList[n + 0].m_Color, (int)matgroup.m_BoneIDs[vtxList[n + 0].m_MatrixID]);

                                            faceList.m_Faces.Add(face);
                                        }
                                        //Because of how normals are defined in triangle strips, every 2nd triangle is clockwise, whereas all others are anti-clockwise
                                    }
                                    polyListDef.m_FaceLists.Add(faceList);
                                    break;
                                }
                            case 3://Quadrilateral Strips
                                {
                                    //4+(N-1)*2 vertices per N quads
                                    //((N-4)/2) + 1 Quads. per N Vertices
                                    int numFaces = ((vtxList.Count - 4) / 2) + 1;
                                    if (vtxList.Count < 4)//Should never be
                                        break;
                                    ModelBase.FaceListDef faceList = new ModelBase.FaceListDef(ModelBase.PolyListType.Polygons);
                                    for (int n = 0, p = 0; n < numFaces; n++, p = p + 2)
                                    {
                                        ModelBase.FaceDef face = new ModelBase.FaceDef(4);

                                        face.m_Vertices[0] = new ModelBase.VertexDef(vtxList[p + 0].m_Position, vtxList[p + 0].m_TexCoord,
                                            vtxList[p + 0].m_Normal, vtxList[p + 0].m_Color, (int)matgroup.m_BoneIDs[vtxList[p + 0].m_MatrixID]);
                                        face.m_Vertices[1] = new ModelBase.VertexDef(vtxList[p + 1].m_Position, vtxList[p + 1].m_TexCoord,
                                            vtxList[p + 1].m_Normal, vtxList[p + 1].m_Color, (int)matgroup.m_BoneIDs[vtxList[p + 1].m_MatrixID]);
                                        face.m_Vertices[2] = new ModelBase.VertexDef(vtxList[p + 3].m_Position, vtxList[p + 3].m_TexCoord,
                                            vtxList[p + 3].m_Normal, vtxList[p + 3].m_Color, (int)matgroup.m_BoneIDs[vtxList[p + 3].m_MatrixID]);
                                        face.m_Vertices[3] = new ModelBase.VertexDef(vtxList[p + 2].m_Position, vtxList[p + 2].m_TexCoord,
                                            vtxList[p + 2].m_Normal, vtxList[p + 2].m_Color, (int)matgroup.m_BoneIDs[vtxList[p + 2].m_MatrixID]);

                                        faceList.m_Faces.Add(face);
                                    }
                                    polyListDef.m_FaceLists.Add(faceList);
                                    break;
                                }
                            default: MessageBox.Show("Unknown polygon type."); break;
                        }//End polyType switch
                    }
                }

                bone.CalculateBranchTransformations();
            }

            m_Model.ApplyTransformations();

            return m_Model;
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
