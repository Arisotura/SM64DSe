using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Globalization;
using OpenTK;
using System.Drawing;
using SM64DSe.SM64DSFormats;

namespace SM64DSe.ImportExport
{
    public class ModelBase
    {
        public class BoneDefRoot : IEnumerable<BoneDef>
        {
            public static readonly int MAX_BONE_COUNT = 32;

            private List<BoneDef> m_Bones;

            public BoneDefRoot()
            {
                m_Bones = new List<BoneDef>();
            }

            public void AddRootBone(BoneDef root)
            {
                if (GetBoneIndex(root.m_ID) == -1)
                {
                    m_Bones.Add(root);
                    root.CalculateInverseTransformation();
                }
            }

            public BoneDef GetBoneByID(string id)
            {
                var queue = new Queue<BoneDef>();
                foreach (BoneDef root in m_Bones)
                {
                    foreach (var node in root.GetBranch())
                    {
                        if (node.m_ID.Equals(id))
                        {
                            return node;
                        }
                    }
                }
                return null; // Not found
            }

            public BoneDef GetBoneByIndex(int index)
            {
                return (index < Count) ? GetAsList().ElementAt(index) : null; 
            }

            public IEnumerator<BoneDef> GetEnumerator()
            {
                List<BoneDef> bones = GetAsList();
                foreach (BoneDef bone in bones)
                {
                    yield return bone;
                }
            }

            public List<BoneDef> GetAsList()
            {
                List<BoneDef> bones = new List<BoneDef>();
                foreach (BoneDef root in m_Bones)
                {
                    List<BoneDef> branch = root.GetBranch();
                    foreach (BoneDef entry in branch)
                    {
                        bones.Add(entry);
                    }
                }
                return bones;
            }

            public List<string> GetBoneIDList()
            {
                List<BoneDef> bones = GetAsList();
                List<string> boneIDs = new List<string>(bones.Count);
                foreach (BoneDef bone in bones)
                {
                    boneIDs.Add(bone.m_ID);
                }
                return boneIDs;
            }

            public Dictionary<string, BoneDef> GetAsDictionary()
            {
                Dictionary<string, BoneDef> bones = new Dictionary<string, BoneDef>();
                foreach (BoneDef bone in GetAsList())
                {
                    bones.Add(bone.m_ID, bone);
                }
                return bones;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            public List<BoneDef> GetRootBones()
            {
                return m_Bones;
            }

            public int GetBoneIndex(BoneDef bone)
            {
                return GetAsList().IndexOf(bone);
            }

            public int GetBoneIndex(string id)
            {
                return GetBoneIDList().IndexOf(id);
            }

            public int GetParentOffset(BoneDef bone)
            {
                if (bone.m_Parent != null)
                    return (GetBoneIndex(bone.m_Parent.m_ID) - GetBoneIndex(bone.m_ID));
                else
                    return 0;
            }

            public int GetNextSiblingOffset(BoneDef bone)
            {
                List<BoneDef> bonesInBranch = bone.GetRoot().GetBranch();

                // sibling offset, unlike parent offset should not be negative, bones should only point forward to their next sibling
                IEnumerable<BoneDef> siblings = bonesInBranch.Where(bone0 => GetParentOffset(bone0) != 0 &&
                    bone0.m_Parent == bone.m_Parent);
                if (siblings.Count() > 0)
                {
                    int indexCurrent = siblings.ToList().IndexOf(bone);
                    if (indexCurrent == (siblings.Count() - 1))
                        return 0;
                    else
                        return (GetBoneIndex(siblings.ElementAt(indexCurrent + 1)) - GetBoneIndex(bone));
                }

                return 0;
            }

            public bool RemoveBoneByID(string id)
            {
                BoneDef root;
                for (int i = 0; i < m_Bones.Count; i++)
                {
                    root = m_Bones.ElementAt(i);
                    if (root.m_ID.Equals(id))
                    {
                        m_Bones.RemoveAt(i);
                        return true;
                    }
                    foreach (var node in root.GetBranch())
                    {
                        if (node.m_ID.Equals(id))
                        {
                            return node.m_Parent.RemoveChild(node);
                        }
                    }
                }
                return false; // Not found
            }

            public void Clear()
            {
                m_Bones.Clear();
            }

            public int Count
            {
                get { return GetAsList().Count; }
            }
        }

        public class BoneDef
        {
            private readonly List<BoneDef> m_Children = new List<BoneDef>();

            public string m_ID;
            public BoneDef m_Parent;

            public Vector3 m_Scale;
            public Vector3 m_Rotation;
            public Vector3 m_Translation;
            public Matrix4 m_LocalTransformation;
            public Matrix4 m_GlobalTransformation;
            public Matrix4 m_LocalInverseTransformation;
            public Matrix4 m_GlobalInverseTransformation;

            public uint[] m_20_12Scale;
            public ushort[] m_4_12Rotation;
            public uint[] m_20_12Translation;

            public Dictionary<string, GeometryDef> m_Geometries;
            public List<string> m_MaterialsInBranch;

            public bool m_HasChildren { get { return m_Children.Count > 0; } }

            public bool m_Billboard;

            public BoneDef(string id)
            {
                this.m_ID = id;
                m_Geometries = new Dictionary<string, GeometryDef>();
                m_MaterialsInBranch = new List<string>();
                SetScale(new Vector3(1f, 1f, 1f));
                SetRotation(new Vector3(0f, 0f, 0f));
                SetTranslation(new Vector3(0f, 0f, 0f));
            }

            public void AddChild(BoneDef item)
            {
                item.m_Parent = this;

                this.m_Children.Add(item);
                item.CalculateBranchTransformations();
            }

            public bool RemoveChild(BoneDef item)
            {
                return m_Children.Remove(item);
            }

            public BoneDef GetRoot()
            {
                if (m_Parent == null)
                    return this;
                else
                    return this.m_Parent.GetRoot();
            }

            public string GetRootID()
            {
                return this.GetRoot().m_ID;
            }

            public List<BoneDef> GetBranch()
            {
                List<BoneDef> bones = new List<BoneDef>();
                GetBranchNodes(bones);
                return bones;
            }

            private void GetBranchNodes(List<BoneDef> bones)
            {
                bones.Add(this);

                foreach (BoneDef child in m_Children)
                    child.GetBranchNodes(bones);
            }

            public List<BoneDef> GetChildren()
            {
                return m_Children;
            }

            public void SetScale(uint[] scale)
            {
                m_20_12Scale = scale;
                m_Scale = ConvertUInt20_12ToVector3(m_20_12Scale);
            }

            public void SetScale(Vector3 scale)
            {
                m_Scale.X = scale.X;
                m_Scale.Y = scale.Y;
                m_Scale.Z = scale.Z;
                m_20_12Scale = ConvertVector3ToUInt20_12(m_Scale);
            }

            public void SetRotation(ushort[] rotation)
            {
                m_4_12Rotation = rotation;
                m_Rotation = ConvertUShort4_12ToVector3(m_4_12Rotation);
            }

            public void SetRotation(Vector3 rotation)
            {
                m_Rotation.X = rotation.X;
                m_Rotation.Y = rotation.Y;
                m_Rotation.Z = rotation.Z;
                m_4_12Rotation = ConvertVector3ToUShort4_12(m_Rotation);
            }

            public void SetTranslation(uint[] translation)
            {
                m_20_12Translation = translation;
                m_Translation = ConvertUInt20_12ToVector3(m_20_12Translation);
            }

            public void SetTranslation(Vector3 translation)
            {
                m_Translation.X = translation.X;
                m_Translation.Y = translation.Y;
                m_Translation.Z = translation.Z;
                m_20_12Translation = ConvertVector3ToUInt20_12(m_Translation);
            }

            public void CalculateBranchTransformations()
            {
                CalculateTransformation();
                CalculateInverseTransformation();

                foreach (BoneDef child in m_Children)
                    child.CalculateBranchTransformations();
            }

            public void CalculateTransformation()
            {
                m_LocalTransformation = Helper.SRTToMatrix(m_Scale, m_Rotation, m_Translation);
                m_GlobalTransformation = m_LocalTransformation;

                if (m_Parent != null)
                {
                    Matrix4.Mult(ref m_LocalTransformation, ref m_Parent.m_GlobalTransformation, out m_GlobalTransformation);
                }
            }

            public void CalculateInverseTransformation()
            {
                Vector3 invScale = new Vector3((1f) / m_Scale.X, (1f) / m_Scale.Y, (1f) / m_Scale.Z);
                Vector3 invRot = new Vector3((-1) * m_Rotation.X, (-1) * m_Rotation.Y, (-1) * m_Rotation.Z);
                Vector3 invTrans = new Vector3((-1) * m_Translation.X, (-1) * m_Translation.Y, (-1) * m_Translation.Z);

                Matrix4 ret = Matrix4.Identity;

                if (m_Parent != null)
                {
                    Matrix4.Mult(ref ret, ref m_Parent.m_GlobalInverseTransformation, out ret);
                }

                Matrix4 inv = Helper.InverseSRTToMatrix(invScale, invRot, invTrans);
                Matrix4.Mult(ref ret, ref inv, out ret);

                m_LocalInverseTransformation = inv;
                m_GlobalInverseTransformation = ret;
            }

            public static Vector3 ConvertUInt20_12ToVector3(uint[] values)
            {
                return new Vector3((float)(int)values[0] / 4096.0f, (float)(int)values[1] / 4096.0f, (float)(int)values[2] / 4096.0f);
            }

            public static uint[] ConvertVector3ToUInt20_12(Vector3 vector)
            {
                return new uint[] { (uint)(vector.X * 4096.0f), 
                    (uint)(vector.Y * 4096.0f), (uint)(vector.Z * 4096.0f) };
            }

            public static Vector3 ConvertUShort4_12ToVector3(ushort[] values)
            {
                return new Vector3(((float)(short)values[0] * (float)Math.PI) / 2048.0f,
                    ((float)(short)values[1] * (float)Math.PI) / 2048.0f, ((float)(short)values[2] * (float)Math.PI) / 2048.0f);
            }

            public static ushort[] ConvertVector3ToUShort4_12(Vector3 vector)
            {
                return new ushort[] { (ushort)((vector.X * 2048.0f) / Math.PI), 
                    (ushort)((vector.Y * 2048.0f) / Math.PI), (ushort)((vector.Z * 2048.0f) / Math.PI) };
            }

            public int Count
            {
                get { return this.m_Children.Count; }
            }
        }

        public class GeometryDef
        {
            public string m_ID;
            public Dictionary<string, PolyListDef> m_PolyLists;

            public GeometryDef(string id)
            {
                m_ID = id;
                m_PolyLists = new Dictionary<string, PolyListDef>();
            }
        }

        public class PolyListDef
        {
            public string m_ID;
            public string m_MaterialName;
            public List<FaceListDef> m_FaceLists;

            public PolyListDef(string id, string materialName)
            {
                m_ID = id;
                m_MaterialName = materialName;
                m_FaceLists = new List<FaceListDef>();
            }
        }

        public enum PolyListType
        {
            Polygons,
            Triangles,
            TriangleStrip,
            QuadrilateralStrip
        };

        public class FaceListDef
        {
            public List<FaceDef> m_Faces;
            public PolyListType m_Type;

            public FaceListDef() :
                this(PolyListType.Polygons) { }

            public FaceListDef(PolyListType type)
            {
                m_Faces = new List<FaceDef>();
                m_Type = type;
            }
        }

        public class FaceDef
        {
            public VertexDef[] m_Vertices;
            public int m_NumVertices;

            public FaceDef() { }

            public FaceDef(int numVertices)
            {
                m_NumVertices = numVertices;
                m_Vertices = new VertexDef[m_NumVertices];
            }

            public override bool Equals(object obj)
            {
                var other = obj as FaceDef;
                if (other == null) return false;

                if (m_NumVertices != other.m_NumVertices) return false;
                for (int v = 0; v < m_NumVertices; v++)
                {
                    if (!m_Vertices[v].Equals(other.m_Vertices[v])) return false;
                }

                return true;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 13;
                    hash = hash * 7 + m_NumVertices.GetHashCode();
                    hash = hash * 7 + m_Vertices.GetHashCode();
                    return hash;
                }
            }
        }

        // NOTE: VertexDef is a Value Type. This is to avoid issues such as scaling the same vertex twice 
        // because it's been referenced in two faces with faces[i].m_Vertices[2] = faces[i - 1].m_Vertices[0]
        public struct VertexDef
        {
            public Vector3 m_Position;
            public Vector2? m_TextureCoordinate;
            public Vector3? m_Normal;
            public Color m_VertexColour;
            public int m_VertexBoneIndex;

            public VertexDef(Vector3 position, Vector2? textureCoordinate, Vector3? normal, Color vertexColour, int vertexBoneID)
            {
                m_Position = position;
                m_TextureCoordinate = textureCoordinate;
                m_Normal = normal;
                m_VertexColour = vertexColour;
                m_VertexBoneIndex = vertexBoneID;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is VertexDef))
                    return false;
                VertexDef fv = (VertexDef)obj;

                if (!(fv.m_Position.X == m_Position.X && fv.m_Position.Y == m_Position.Y && fv.m_Position.Z == this.m_Position.Z))
                    return false;

                if (!(fv.m_TextureCoordinate == null && m_TextureCoordinate == null))
                {
                    if (!(((Vector2)fv.m_TextureCoordinate).X == ((Vector2)m_TextureCoordinate).X && 
                        ((Vector2)fv.m_TextureCoordinate).Y == ((Vector2)m_TextureCoordinate).Y))
                        return false;
                }

                if (!(fv.m_Normal == null && m_Normal == null))
                {
                    if (!(((Vector3)fv.m_Normal).X == ((Vector3)m_Normal).X && ((Vector3)fv.m_Normal).Y == ((Vector3)m_Normal).Y &&
                        ((Vector3)fv.m_Normal).Z == ((Vector3)m_Normal).Z))
                        return false;
                }

                if (!(fv.m_VertexColour.R == m_VertexColour.R && fv.m_VertexColour.G == m_VertexColour.G &&
                    fv.m_VertexColour.B == m_VertexColour.B && fv.m_VertexColour.A == m_VertexColour.A))
                    return false;

                if (!(fv.m_VertexBoneIndex == m_VertexBoneIndex))
                    return false;

                return true;
            }

            public override int GetHashCode()
            {
                int hash = 13;
                hash = (hash * 7) + m_Position.GetHashCode();
                hash = (hash * 7) + ((m_TextureCoordinate != null) ? m_TextureCoordinate.GetHashCode() : -1);
                hash = (hash * 7) + ((m_Normal != null) ? m_TextureCoordinate.GetHashCode() : -1);
                hash = (hash * 7) + m_VertexColour.GetHashCode();
                hash = (hash * 7) + m_VertexBoneIndex * 397;
                return hash;
            }
        }

        public static readonly VertexDef EMPTY_VERTEX = new VertexDef(Vector3.Zero, null, null, Color.White, 0);

        public class MaterialDef
        {
            public string m_ID;
            public string m_TextureDefID;
            public bool[] m_Lights;
            public PolygonDrawingFace m_PolygonDrawingFace;
            public byte m_Alpha;
            public bool m_WireMode;
            public PolygonMode m_PolygonMode;
            public bool m_FogFlag;
            public bool m_DepthTestDecal;
            public bool m_RenderOnePixelPolygons;
            public bool m_FarClipping;
            public Color m_Diffuse;
            public Color m_Ambient;
            public Color m_Specular;
            public Color m_Emission;
            public bool m_ShininessTableEnabled;
            public TexTiling[] m_TexTiling;
            public Vector2 m_TextureScale;
            public float m_TextureRotation;
            public Vector2 m_TextureTranslation;
            public TexGenMode m_TexGenMode;

            public MaterialDef(string id)
            {
                m_ID = id;
                m_TextureDefID = null;
                m_Lights = new bool[] { false, false, false, false };
                m_PolygonDrawingFace = PolygonDrawingFace.Front;
                m_Alpha = 31;
                m_WireMode = false;
                m_PolygonMode = PolygonMode.Modulation;
                m_FogFlag = true;
                m_DepthTestDecal = false;
                m_RenderOnePixelPolygons = false;
                m_FarClipping = true;
                m_Diffuse = Color.White;
                m_Ambient = Color.White;
                m_Specular = Color.Black;
                m_Emission = Color.Black;
                m_ShininessTableEnabled = false;
                m_TexTiling = new TexTiling[] { TexTiling.Repeat, TexTiling.Repeat };
                m_TextureScale = new Vector2(1f, 1f);
                m_TextureRotation = 0.0f;
                m_TextureTranslation = new Vector2(0f, 0f);
                m_TexGenMode = TexGenMode.None;
            }

            public MaterialDef(string id, MaterialDef that)
            {
                m_ID = id;
                m_TextureDefID = that.m_TextureDefID;
                m_Lights = new bool[4];
                Array.Copy(that.m_Lights, m_Lights, 4);
                m_PolygonDrawingFace = that.m_PolygonDrawingFace;
                m_Alpha = that.m_Alpha;
                m_WireMode = that.m_WireMode;
                m_PolygonMode = that.m_PolygonMode;
                m_FogFlag = that.m_FogFlag;
                m_DepthTestDecal = that.m_DepthTestDecal;
                m_RenderOnePixelPolygons = that.m_RenderOnePixelPolygons;
                m_FarClipping = that.m_FarClipping;
                m_Diffuse = that.m_Diffuse;
                m_Ambient = that.m_Ambient;
                m_Specular = that.m_Specular;
                m_Emission = that.m_Emission;
                m_ShininessTableEnabled = that.m_ShininessTableEnabled;
                m_TexTiling = new TexTiling[2];
                Array.Copy(that.m_TexTiling, m_TexTiling, 2);
                m_TextureScale = that.m_TextureScale;
                m_TextureRotation = that.m_TextureRotation;
                m_TextureTranslation = that.m_TextureTranslation;
                m_TexGenMode = that.m_TexGenMode;
            }

            public enum PolygonDrawingFace
            {
                Front,
                Back,
                FrontAndBack
            };

            public enum PolygonMode
            {
                Modulation,
                Decal,
                Toon_HighlightShading,
                Shadow
            };

            public enum TexTiling
            {
                Clamp,
                Repeat,
                Flip
            };
        }

        public enum TextureFormat
        {
            Nitro_A3I5 = 1,
            Nitro_Palette4 = 2,
            Nitro_Palette16 = 3,
            Nitro_Palette256 = 4,
            Nitro_Tex4x4 = 5,
            Nitro_A5I3 = 6,
            Nitro_Direct = 7,
            ExternalBitmap = 8,
            InMemoryBitmap = 9
        };

        public enum TexGenMode
        {
            None = 0,
            Tex = 1,
            Normal = 2,
            Pos = 3
        };

        public class TextureDefBase
        {
            public string m_ID;
            public string m_ImgHash;
            public TextureFormat m_Format;
            protected int m_Width;
            protected int m_Height;
            protected string m_TexName;
            protected string m_PalName;

            public virtual int GetWidth() { return m_Width; }
            public virtual int GetHeight() { return m_Height; }
            public virtual string GetTexName() { return m_TexName; }
            public virtual string GetPalName() { return m_PalName; }
            public virtual string CalculateHash() { return null; }
            public virtual bool IsNitro() { return false; }
            public virtual Bitmap GetBitmap() { return null; }
            public virtual byte[] GetNitroTexData() { return null; }
            public virtual bool HasNitroPalette() { return false; }
            public virtual byte[] GetNitroPalette() { return null; }
            public virtual byte GetColor0Mode() { return 0; }
        }

        public class TextureDefBitmapBase : TextureDefBase
        {
            protected void TexAndPalNamesFromFilename(string name)
            {
                string path_separator = (name.Contains("/")) ? "/" : "\\";
                m_TexName = name.Substring(name.LastIndexOf(path_separator) + 1).Replace('.', '_');
                m_PalName = m_TexName + "_pl";
            }

            public override string CalculateHash()
            {
                Bitmap tex = GetBitmap();

                int width = 8, height = 8;
                while (width < GetWidth()) width *= 2;
                while (height < GetHeight()) height *= 2;

                // cheap resizing for textures whose dimensions aren't power-of-two
                if ((width != GetWidth()) || (height != GetHeight()))
                {
                    Bitmap newbmp = new Bitmap(width, height);
                    Graphics g = Graphics.FromImage(newbmp);
                    g.DrawImage(tex, new Rectangle(0, 0, width, height));
                    tex = newbmp;
                }

                byte[] map = new byte[tex.Width * tex.Height * 4];
                for (int y = 0; y < tex.Height; y++)
                {
                    for (int x = 0; x < tex.Width; x++)
                    {
                        Color pixel = tex.GetPixel(x, y);
                        int pos = ((y * tex.Width) + x) * 4;

                        map[pos] = pixel.B;
                        map[pos + 1] = pixel.G;
                        map[pos + 2] = pixel.R;
                        map[pos + 3] = pixel.A;
                    }
                }

                string imghash = Helper.HexString(Helper.m_MD5.ComputeHash(map));
                return imghash;
            }
        }

        public class TextureDefExternalBitmap : TextureDefBitmapBase
        {
            public string m_FileName;

            public TextureDefExternalBitmap(string id, string fileName)
            {
                m_ID = id;
                m_FileName = fileName;
                m_Format = TextureFormat.ExternalBitmap;
                try
                {
                    using (Bitmap tmp = new Bitmap(fileName))
                    {
                        m_Width = tmp.Width;
                        m_Height = tmp.Height;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error loading image: " + fileName, ex);
                }
                TexAndPalNamesFromFilename(fileName);
                m_ImgHash = CalculateHash();
            }

            public override Bitmap GetBitmap()
            {
                return new Bitmap(m_FileName);
            }
        }

        public class TextureDefInMemoryBitmap : TextureDefBitmapBase
        {
            protected Bitmap m_Bitmap;

            public TextureDefInMemoryBitmap(string id, Bitmap bmp)
            {
                m_ID = id;
                m_Format = TextureFormat.InMemoryBitmap;
                m_Bitmap = bmp;
                m_TexName = id;
                m_PalName = id + "_pl";
                m_Width = bmp.Width;
                m_Height = bmp.Height;
                m_ImgHash = CalculateHash();
            }

            public override Bitmap GetBitmap()
            {
                return new Bitmap(m_Bitmap);
            }
        }

        public class TextureDefNitro : TextureDefBase
        {
            protected byte[] m_TexData;
            protected byte[] m_PalData;
            protected byte m_Color0Mode;

            public TextureDefNitro(string texID, byte[] texData, int width, int height, byte color0Mode, TextureFormat format)
            {
                m_ID = texID;
                m_TexName = texID;
                m_TexData = texData;
                m_Format = format;
                m_Width = width;
                m_Height = height;
                m_Color0Mode = color0Mode;
                m_ImgHash = CalculateHash();
            }

            public TextureDefNitro(string texID, byte[] texData, string palID, byte[] palData, 
                int width, int height, byte color0Mode, TextureFormat format)
            {
                m_ID = texID;
                m_TexName = texID;
                m_TexData = texData;
                m_PalName = palID;
                m_PalData = palData;
                m_Width = width;
                m_Height = height;
                m_Color0Mode = color0Mode;
                m_Format = format;
                m_ImgHash = CalculateHash();
            }

            public TextureDefNitro(NitroTexture nitroTexture)
                : this(nitroTexture.m_TextureName, nitroTexture.m_RawTextureData, nitroTexture.m_PaletteName, 
                nitroTexture.m_RawPaletteData, nitroTexture.m_Width, nitroTexture.m_Height, 
                nitroTexture.m_Colour0Mode, (TextureFormat)nitroTexture.m_TexType)
            { }

            public override string CalculateHash()
            {
                if (!HasNitroPalette())
                    return Helper.HexString(Helper.m_MD5.ComputeHash(m_TexData));
                else
                {
                    byte[] hashtmp = new byte[m_TexData.Length + m_PalData.Length];
                    Array.Copy(m_TexData, hashtmp, m_TexData.Length);
                    Array.Copy(m_PalData, 0, hashtmp, m_TexData.Length, m_PalData.Length);
                    return Helper.HexString(Helper.m_MD5.ComputeHash(hashtmp));
                }
            }

            public override Bitmap GetBitmap()
            {
                return NitroTexture.FromDataAndType(0xFFFFFFFF, m_TexName, 0xFFFFFFFF, m_PalName,
                    m_TexData, m_PalData, m_Width, m_Height, m_Color0Mode, (int)m_Format).ToBitmap();
            }

            public override bool IsNitro()
            {
                return true;
            }

            public override byte[] GetNitroTexData()
            {
                return m_TexData;
            }

            public override bool HasNitroPalette()
            {
                return (m_Format != TextureFormat.Nitro_Direct);
            }

            public override byte[] GetNitroPalette()
            {
                return m_PalData;
            }

            public override byte GetColor0Mode()
            {
                return m_Color0Mode;
            }
        }

        public class AnimationDef
        {
            public string m_ID;
            public string m_BoneID;
            public Dictionary<AnimationComponentType, AnimationComponentDataDef> m_AnimationComponents;
            public int m_NumFrames;

            public AnimationDef(string id, string boneID, int numFrames)
            {
                m_ID = id;
                m_BoneID = boneID;
                m_NumFrames = numFrames;
                m_AnimationComponents = new Dictionary<AnimationComponentType, AnimationComponentDataDef>();
            }

            public AnimationDef(string id, string boneID, int numFrames, 
                Dictionary<AnimationComponentType, AnimationComponentDataDef> animationComponents)
            {
                m_ID = id;
                m_BoneID = boneID;
                m_NumFrames = numFrames;
                m_AnimationComponents = animationComponents;
            }

            public int GetTotalNumberOfFrameValues()
            {
                int sum = 0;
                foreach (AnimationComponentDataDef comp in m_AnimationComponents.Values)
                    sum += comp.GetNumValues();

                return sum;
            }

            public int GetScaleValuesCount()
            {
                return m_AnimationComponents[AnimationComponentType.ScaleX].GetNumValues() +
                    m_AnimationComponents[AnimationComponentType.ScaleY].GetNumValues() +
                    m_AnimationComponents[AnimationComponentType.ScaleZ].GetNumValues();
            }

            public int GetRotateValuesCount()
            {
                return m_AnimationComponents[AnimationComponentType.RotateX].GetNumValues() +
                    m_AnimationComponents[AnimationComponentType.RotateY].GetNumValues() +
                    m_AnimationComponents[AnimationComponentType.RotateZ].GetNumValues();
            }

            public int GetTranslateValuesCount()
            {
                return m_AnimationComponents[AnimationComponentType.TranslateX].GetNumValues() +
                    m_AnimationComponents[AnimationComponentType.TranslateY].GetNumValues() +
                    m_AnimationComponents[AnimationComponentType.TranslateZ].GetNumValues();
            }

            public Vector3 GetFrameScale(int frame)
            {
                return new Vector3(m_AnimationComponents[AnimationComponentType.ScaleX].GetFrameValue(frame),
                    m_AnimationComponents[AnimationComponentType.ScaleY].GetFrameValue(frame),
                    m_AnimationComponents[AnimationComponentType.ScaleZ].GetFrameValue(frame));
            }

            public Vector3 GetFrameRotation(int frame)
            {
                return new Vector3(m_AnimationComponents[AnimationComponentType.RotateX].GetFrameValue(frame),
                    m_AnimationComponents[AnimationComponentType.RotateY].GetFrameValue(frame),
                    m_AnimationComponents[AnimationComponentType.RotateZ].GetFrameValue(frame));
            }

            public Vector3 GetFrameTranslation(int frame)
            {
                return new Vector3(m_AnimationComponents[AnimationComponentType.TranslateX].GetFrameValue(frame),
                    m_AnimationComponents[AnimationComponentType.TranslateY].GetFrameValue(frame),
                    m_AnimationComponents[AnimationComponentType.TranslateZ].GetFrameValue(frame));
            }

            public BCA.SRTContainer GetFrame(int frame)
            {
                Vector3 scale = new Vector3(GetFrameScale(frame));
                Vector3 rotation = new Vector3(GetFrameRotation(frame));
                Vector3 translation = new Vector3(GetFrameTranslation(frame));

                return new BCA.SRTContainer(scale, rotation, translation);
            }

            public BCA.SRTContainer[] GetAllFrames()
            {
                BCA.SRTContainer[] frames = new BCA.SRTContainer[m_NumFrames];

                for (int i = 0; i < m_NumFrames; i++)
                {
                    frames[i] = GetFrame(i);
                }

                return frames;
            }
        }
        
        // Note: Rotation is stored in Radians

        public class AnimationComponentDataDef
        {
            public AnimationComponentType m_AnimationComponentType;
            private float[] m_Values;
            private int m_NumFrames;
            private bool m_IsConstant;
            private int m_FrameStep;
            private bool m_UseIdentity;

            public AnimationComponentDataDef(float[] values, int numFrames, bool isConstant, int frameStep, bool useIdentity, 
                AnimationComponentType animationComponentType)
            {
                m_Values = values;
                m_NumFrames = numFrames;
                m_IsConstant = isConstant;
                m_FrameStep = frameStep;
                m_UseIdentity = useIdentity;
                m_AnimationComponentType = animationComponentType;
            }

            public float GetValue(int index) { return m_Values[index]; }
            public void SetValue(int index, float value) { m_Values[index] = value; }

            public byte[] GetFixedPointValues()
            {
                byte[] result = new byte[0];
                if (m_AnimationComponentType == AnimationComponentType.RotateX ||
                    m_AnimationComponentType == AnimationComponentType.RotateY ||
                    m_AnimationComponentType == AnimationComponentType.RotateZ)
                {
                    result = new byte[m_Values.Length * sizeof(ushort)];
                    Buffer.BlockCopy(Array.ConvertAll<float, ushort>(m_Values, x => (ushort)((x * 2048.0f) / Math.PI)), 
                        0, result, 0, result.Length);
                }
                else
                {
                    result = new byte[m_Values.Length * sizeof(uint)];
                    Buffer.BlockCopy(Array.ConvertAll<float, uint>(m_Values, x => (uint)(x * 4096f)), 
                        0, result, 0, result.Length);
                }
                return result;
            }

            public int GetNumValues() { return m_Values.Length; }

            public int GetFrameStep() { return m_FrameStep; }
            public bool GetIsConstant() { return m_IsConstant; }
            public bool GetUseIdentity() { return m_UseIdentity; }

            public float GetFrameValue(int frameNum)
            {
                if (m_IsConstant)
                {
                    return m_Values[0];
                }
                else
                {
                    if (m_FrameStep == 1)
                    {
                        return m_Values[frameNum];
                    }
                    else
                    {
                        // Odd frames
                        if ((frameNum & 1) != 0)
                        {

                            if ((frameNum / m_FrameStep) + 1 > m_Values.Length - 1)
                            {
                                // if floor(frameNum / 2) + 1 > number of values, use floor(frameNum / 2)
                                return m_Values[(frameNum / m_FrameStep)];
                            }
                            else if (frameNum == (m_NumFrames - 1))
                            {
                                // else if it's the last frame, don't interpolate
                                return m_Values[(frameNum / m_FrameStep) + 1];
                            }
                            else
                            {
                                float val1 = m_Values[frameNum >> 1];
                                float val2 = m_Values[(frameNum >> 1) + 1];
                                if (m_AnimationComponentType == AnimationComponentType.RotateX ||
                                    m_AnimationComponentType == AnimationComponentType.RotateY ||
                                    m_AnimationComponentType == AnimationComponentType.RotateZ)
                                {
                                    if (val1 < 0f && val2 > 0f)
                                    {
                                        if (Math.Abs(val2 - (val1 + (Math.PI * 2f))) < Math.Abs(val2 - val1))
                                        {
                                            val2 -= (float)(Math.PI * 2f);
                                        }
                                    }
                                    else if (val1 > 0f && val2 < 0f)
                                    {
                                        if (Math.Abs(val1 - (val2 + (Math.PI * 2f))) < Math.Abs(val1 - val2))
                                        {
                                            val2 += (float)(Math.PI * 2f);
                                        }
                                    }
                                }
                                return val1 + (((val1 + val2) / 2f) * (frameNum % m_FrameStep));
                            }
                        }
                        else
                        {
                            // Even frames
                            return m_Values[frameNum / m_FrameStep];
                        }
                    }
                }
            }
        }

        public enum AnimationComponentType
        {
            ScaleX,
            ScaleY,
            ScaleZ,
            RotateX,
            RotateY,
            RotateZ,
            TranslateX,
            TranslateY,
            TranslateZ
        };

        public BoneDefRoot m_BoneTree;
        public Dictionary<string, MaterialDef> m_Materials;
        public Dictionary<string, TextureDefBase> m_Textures;
        public Dictionary<string, AnimationDef> m_Animations;
        public BiDictionaryOneToOne<string, int> m_BoneTransformsMap;

        public string m_ModelFileName;
        public string m_ModelPath;

        public ModelBase(string modelFileName)
        {
            m_ModelFileName = modelFileName;
            m_ModelPath = Path.GetDirectoryName(m_ModelFileName);

            m_BoneTree = new BoneDefRoot();
            m_Materials = new Dictionary<string, MaterialDef>();
            m_Textures = new Dictionary<string, TextureDefBase>();
            m_Animations = new Dictionary<string, AnimationDef>();
            m_BoneTransformsMap = new BiDictionaryOneToOne<string, int>();
        }

        public void ScaleModel(float scale)
        {
            foreach (BoneDef bone in m_BoneTree)
            {
                Vector3 translation = bone.m_Translation;
                bone.SetTranslation(Vector3.Multiply(translation, scale));

                foreach (GeometryDef geometry in bone.m_Geometries.Values)
                {
                    foreach (PolyListDef polyList in geometry.m_PolyLists.Values)
                    {
                        foreach (FaceListDef faceList in polyList.m_FaceLists)
                        {
                            foreach (FaceDef face in faceList.m_Faces)
                            {
                                for (int vert = 0; vert < face.m_Vertices.Length; vert++)
                                {
                                    face.m_Vertices[vert].m_Position.X *= scale;
                                    face.m_Vertices[vert].m_Position.Y *= scale;
                                    face.m_Vertices[vert].m_Position.Z *= scale;
                                }
                            }
                        }
                    }
                }
            }
            foreach (BoneDef root in m_BoneTree.GetRootBones())
            {
                root.CalculateBranchTransformations();
            }
        }

        public void ScaleAnimations(float scale)
        {
            foreach (AnimationDef animDef in m_Animations.Values)
            {
                foreach (AnimationComponentDataDef comp in animDef.m_AnimationComponents.Values)
                {
                    // Bone's translation was already scaled, don't want it scaled a second time in the animation
                    if (comp.GetUseIdentity()) continue; 

                    switch (comp.m_AnimationComponentType)
                    {
                        case AnimationComponentType.TranslateX:
                            {
                                for (int i = 0; i < comp.GetNumValues(); i++)
                                    comp.SetValue(i, comp.GetValue(i) * scale);
                            }
                            break;
                        case AnimationComponentType.TranslateY:
                            {
                                for (int i = 0; i < comp.GetNumValues(); i++)
                                    comp.SetValue(i, comp.GetValue(i) * scale);
                            }
                            break;
                        case AnimationComponentType.TranslateZ:
                            {
                                for (int i = 0; i < comp.GetNumValues(); i++)
                                    comp.SetValue(i, comp.GetValue(i) * scale);
                            }
                            break;
                    }
                }
            }
        }

        public void ApplyTransformations()
        {
            // Now apply the vertex's bone's transformation to each vertex
            foreach (BoneDef bone in m_BoneTree)
            {
                foreach (GeometryDef geometry in bone.m_Geometries.Values)
                {
                    foreach (PolyListDef polyList in geometry.m_PolyLists.Values)
                    {
                        foreach (FaceListDef faceList in polyList.m_FaceLists)
                        {
                            foreach (FaceDef face in faceList.m_Faces)
                            {
                                for (int vert = 0; vert < face.m_Vertices.Length; vert++)
                                {
                                    BoneDef currentVertexBone = m_BoneTree.GetAsList()[face.m_Vertices[vert].m_VertexBoneIndex];

                                    Vector3 vertex = face.m_Vertices[vert].m_Position;
                                    Vector3.Transform(ref vertex, ref currentVertexBone.m_GlobalTransformation, out vertex);
                                    face.m_Vertices[vert].m_Position = vertex;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void ApplyInverseTransformations()
        {
            // DEBUG ONLY
            //foreach (BoneDef bone in m_BoneTree)
            //    Console.WriteLine(bone.m_ID + ".m_GlobalInverseTransformation: " + bone.m_GlobalInverseTransformation.ToString());

            // Now apply the vertex's bone's reverse transformation to each vertex
            foreach (BoneDef bone in m_BoneTree)
            {
                foreach (GeometryDef geometry in bone.m_Geometries.Values)
                {
                    foreach (ModelBase.PolyListDef polyList in geometry.m_PolyLists.Values)
                    {
                        foreach (FaceListDef faceList in polyList.m_FaceLists)
                        {
                            foreach (FaceDef face in faceList.m_Faces)
                            {
                                for (int vert = 0; vert < face.m_Vertices.Length; vert++)
                                {
                                    BoneDef currentVertexBone = m_BoneTree.GetAsList()[face.m_Vertices[vert].m_VertexBoneIndex];

                                    Vector3 vertex = face.m_Vertices[vert].m_Position;
                                    Vector3.Transform(ref vertex, ref currentVertexBone.m_GlobalInverseTransformation, out vertex);
                                    face.m_Vertices[vert].m_Position = vertex;
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool IsTriangulated()
        {
            foreach (BoneDef bone in m_BoneTree)
            {
                foreach (GeometryDef geometry in bone.m_Geometries.Values)
                {
                    foreach (ModelBase.PolyListDef polyList in geometry.m_PolyLists.Values)
                    {
                        foreach (FaceListDef faceList in polyList.m_FaceLists)
                        {
                            if (faceList.m_Type != PolyListType.Triangles || faceList.m_Type != PolyListType.TriangleStrip)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        public void EnsureTriangulation()
        {
            if (!IsTriangulated())
            {
                Triangulate();
            }
        }

        private void Triangulate()
        {
            foreach (ModelBase.BoneDef bone in m_BoneTree)
            {
                foreach (GeometryDef geometry in bone.m_Geometries.Values)
                {
                    foreach (PolyListDef polyList in geometry.m_PolyLists.Values)
                    {
                        List<int> removeFLs = new List<int>();
                        List<FaceListDef> triangulated = new List<FaceListDef>();
                        for (int fl = 0; fl < polyList.m_FaceLists.Count; fl++)
                        {
                            FaceListDef faceList = polyList.m_FaceLists[fl];
                            if (!(faceList.m_Type == PolyListType.Triangles || faceList.m_Type == PolyListType.TriangleStrip))
                            {
                                FaceListDef triangles = new FaceListDef(PolyListType.Triangles);

                                for (int pg = 0; pg < faceList.m_Faces.Count; pg++)
                                {
                                    FaceDef face = faceList.m_Faces[pg];
                                    int numTriangles;
                                    switch (faceList.m_Type)
                                    {
                                        default:
                                        case PolyListType.Polygons:
                                            numTriangles = 1 + (face.m_NumVertices - 3);
                                            break;
                                        case PolyListType.QuadrilateralStrip:
                                            numTriangles = 2;
                                            break;
                                    }
                                    for (int t = 0; t < numTriangles; t++)
                                    {
                                        FaceDef triangle = new FaceDef(3);
                                        triangle.m_Vertices[0] = face.m_Vertices[0];
                                        triangle.m_Vertices[1] = face.m_Vertices[t + 1];
                                        triangle.m_Vertices[2] = face.m_Vertices[t + 2];
                                        triangles.m_Faces.Add(triangle);
                                    }
                                }

                                removeFLs.Add(fl);
                                triangulated.Add(triangles);
                            }
                        }

                        for (int i = removeFLs.Count - 1; i >= 0; i--)
                        {
                            polyList.m_FaceLists.RemoveAt(removeFLs[i]);
                        }

                        polyList.m_FaceLists.AddRange(triangulated);
                    }
                }
            }
        }

    }
}
