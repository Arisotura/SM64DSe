/* KCLLoader
 * 
 * Given a KCL object, produces a ModelBase object for use in the Writer classes.
 */ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using System.Drawing;

namespace SM64DSe.ImportExport.Loaders.InternalLoaders
{
    public class KCLLoader : AbstractModelLoader
    {
        KCL m_KCL;

        public KCLLoader(string modelFileName, KCL kcl) :
            base(modelFileName) 
        {
            m_KCL = kcl;
        }

        public override ModelBase LoadModel(OpenTK.Vector3 scale)
        {
            ModelBase.BoneDef rootBone = new ModelBase.BoneDef("CollisionMap");
            m_Model.m_BoneTree.AddRootBone(rootBone);

            ModelBase.GeometryDef geometry = new ModelBase.GeometryDef("geometry-0");
            rootBone.m_Geometries.Add(geometry.m_ID, geometry);

            List<int> uniqueCollisionTypes = new List<int>();

            foreach (KCL.ColFace plane in m_KCL.m_Planes)
            {
                if (!uniqueCollisionTypes.Contains(plane.type))
                    uniqueCollisionTypes.Add(plane.type);
            }
            uniqueCollisionTypes.Sort();
            List<Color> uniqueColours = GetColours(uniqueCollisionTypes[uniqueCollisionTypes.Count - 1] + 1);

            foreach (int type in uniqueCollisionTypes)
            {
                ModelBase.MaterialDef material = new ModelBase.MaterialDef("material-" + type, m_Model.m_Materials.Count);
                material.m_Diffuse = uniqueColours[type];
                m_Model.m_Materials.Add(material.m_ID, material);

                rootBone.m_MaterialsInBranch.Add(material.m_ID);
                ModelBase.PolyListDef tmp = new ModelBase.PolyListDef("polylist-" + type, material.m_ID);
                tmp.m_FaceLists.Add(new ModelBase.FaceListDef());
                rootBone.m_Geometries[geometry.m_ID].m_PolyLists.Add("polylist-" + type, tmp);
            }

            foreach (KCL.ColFace plane in m_KCL.m_Planes)
            {
                ModelBase.FaceDef face = new ModelBase.FaceDef(3);

                face.m_Vertices[0].m_Position = plane.point1;
                face.m_Vertices[1].m_Position = plane.point2;
                face.m_Vertices[2].m_Position = plane.point3;

                for (int vert = 0; vert < face.m_Vertices.Length; vert++)
                {
                    face.m_Vertices[vert].m_TextureCoordinate = Vector2.Zero;
                    face.m_Vertices[vert].m_Normal = null;
                    face.m_Vertices[vert].m_VertexColour = Color.White;
                    face.m_Vertices[vert].m_VertexBoneID = 0;
                }

                geometry.m_PolyLists["polylist-" + plane.type].m_FaceLists[0].m_Faces.Add(face);
            }

            return m_Model;
        }

        public static List<Color> GetColours(int amount)
        {
            List<Color> theColours = new List<Color>();

            for (int i = 255; i > 0; i = i - 50)
            {
                for (int j = 0; j < 255; j = j + 40)
                {
                    for (int k = 255; k > 0; k = k - 40)
                    {
                        Color newColour = Color.FromArgb(180, k, i, j);
                        theColours.Add(newColour);

                        if (theColours.Count >= amount)
                            return theColours;
                    }
                }
            }
            return theColours;
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
