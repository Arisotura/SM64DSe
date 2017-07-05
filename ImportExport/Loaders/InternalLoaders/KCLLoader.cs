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

        public override ModelBase LoadModel(float scale)
        {
            ModelBase.BoneDef rootBone = new ModelBase.BoneDef("CollisionMap");
            m_Model.m_BoneTree.AddRootBone(rootBone);
            rootBone.CalculateBranchTransformations();
            m_Model.m_BoneTransformsMap.Add(rootBone.m_ID, m_Model.m_BoneTransformsMap.Count);

            ModelBase.GeometryDef geometry = new ModelBase.GeometryDef("geometry-0");
            rootBone.m_Geometries.Add(geometry.m_ID, geometry);

            List<int> uniqueCollisionTypes = new List<int>();

            foreach (KCL.ColFace plane in m_KCL.m_Planes)
            {
                if (!uniqueCollisionTypes.Contains(plane.type))
                    uniqueCollisionTypes.Add(plane.type);
            }
            uniqueCollisionTypes.Sort();
            CollisionMapColours collisionMapColours = new CollisionMapColours();

            foreach (int type in uniqueCollisionTypes)
            {
                ModelBase.MaterialDef material = new ModelBase.MaterialDef("material-" + type);
                material.m_Diffuse = collisionMapColours[type];
                m_Model.m_Materials.Add(material.m_ID, material);

                rootBone.m_MaterialsInBranch.Add(material.m_ID);
                ModelBase.PolyListDef tmp = new ModelBase.PolyListDef("polylist-" + type, material.m_ID);
                tmp.m_FaceLists.Add(new ModelBase.FaceListDef(ModelBase.PolyListType.Triangles));
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
                    face.m_Vertices[vert].m_TextureCoordinate = null;
                    face.m_Vertices[vert].m_Normal = null;
                    face.m_Vertices[vert].m_VertexColour = Color.White;
                    face.m_Vertices[vert].m_VertexBoneIndex = 0;
                }

                geometry.m_PolyLists["polylist-" + plane.type].m_FaceLists[0].m_Faces.Add(face);
            }

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

        public class CollisionMapColours
        {
            private SortedDictionary<int, Color> m_Colours;

            public CollisionMapColours()
            {
                m_Colours = new SortedDictionary<int, Color>();
            }

            public Color this[int key]
            {
                get
                {
                    Color colour;
                    if (!m_Colours.TryGetValue(key, out colour))
                    {
                        colour = GetColour(key);
                        m_Colours[key] = colour;
                    }
                    return colour;
                }
            }

            public static Color GetColour(int type)
            {
                byte r = (byte)(255 - (39 * type));
                byte g = (byte)(255 - (49 * type));
                byte b = (byte)(0 + (41 * type));
                return Color.FromArgb(255, r, g, b);
            }
        }
    }
}
