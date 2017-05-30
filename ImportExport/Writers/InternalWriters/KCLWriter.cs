/* KCLWriter
 * 
 * Given a ModelBase object, produces a KCL file (collision map).
 */ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace SM64DSe.ImportExport.Writers.InternalWriters
{
    public class KCLWriter : AbstractModelWriter
    {
        public NitroFile m_ModelFile;
        public float m_Scale;
        public float m_FaceSizeThreshold;
        public Dictionary<string, int> m_MatColTypes;

        public const int MAX_BASE_OCTREE_COUNT = 1 << 7;

        public KCLWriter(ModelBase model, NitroFile modelFile, float scale, float faceSizeThreshold,
            Dictionary<string, int> matColTypes) :
            base(model, modelFile.m_Name)
        {
            m_ModelFile = modelFile;
            m_Scale = scale;
            m_FaceSizeThreshold = faceSizeThreshold;
            m_MatColTypes = matColTypes;

            m_Model.EnsureTriangulation();
        }

        public override void WriteModel(bool save = true)
        {
            ConvertToKCL(m_Model, ref m_ModelFile, m_Scale, m_FaceSizeThreshold, m_MatColTypes, save);
        }

        public static void ConvertToKCL(ModelBase model, ref NitroFile kclOut, float scale, float faceSizeThreshold,
            Dictionary<string, int> matColTypes, bool save = true)
        {
            // faceSizeThreshold is used for getting rid of very small faces below a given size, originally 0.001
            List<Triangle> triangles = readLoadedModel(model, faceSizeThreshold, matColTypes);
            scale *= 1000; //Scale of collision file is 1000 times larger than model file
            write_kcl(kclOut, triangles, 15, 1, scale, save);
        }

        private static List<Triangle> readLoadedModel(ModelBase model, float faceSizeThreshold, Dictionary<string, int> matColTypes)
        {
            List<Triangle> triangles = new List<Triangle>();

            foreach (ModelBase.BoneDef bone in model.m_BoneTree)
            {
                foreach (ModelBase.GeometryDef geometry in bone.m_Geometries.Values)
                {
                    foreach (ModelBase.PolyListDef polyList in geometry.m_PolyLists.Values)
                    {
                        string material = polyList.m_MaterialName;

                        foreach (ModelBase.FaceListDef faceList in polyList.m_FaceLists)
                        {
                            foreach (ModelBase.FaceDef face in faceList.m_Faces)
                            {
                                if (face.m_NumVertices == 3)
                                {
                                    Vertex u = new Vertex(
                                        face.m_Vertices[0].m_Position.X, face.m_Vertices[0].m_Position.Y, face.m_Vertices[0].m_Position.Z);
                                    Vertex v = new Vertex(
                                        face.m_Vertices[1].m_Position.X, face.m_Vertices[1].m_Position.Y, face.m_Vertices[1].m_Position.Z);
                                    Vertex w = new Vertex(
                                        face.m_Vertices[2].m_Position.X, face.m_Vertices[2].m_Position.Y, face.m_Vertices[2].m_Position.Z);

                                    //Below line gets rid of faces that are too small, original 0.001
                                    if (cross(v.sub(u), w.sub(u)).norm_sq() < faceSizeThreshold) { continue; } //#TODO: find a better solution
                                    triangles.Add(new Triangle(u, v, w, matColTypes[material]));
                                }
                            }
                        }
                    }
                }
            }

            return triangles;
        }

        private static void write_kcl(NitroFile kcl, List<Triangle> triangles, int max_triangles, int min_width, float scale, bool save = true)
        {
            kcl.Clear();

            //Need to scale each vertex (1000 times scale of model)
            foreach (Triangle t in triangles)
            {
                t.u = t.u.mul(scale);
                t.v = t.v.mul(scale);
                t.w = t.w.mul(scale);
            }

            uint pos = 0;

            List<Face> faces = new List<Face>();
            VertexWelder vertex_welder = new VertexWelder(1 / 64f, (int)(Math.Floor((double)(triangles.Count / 256))));
            VertexWelder normal_welder = new VertexWelder(1 / 1024f, (int)(Math.Floor((double)(4 * triangles.Count / 256))));

            foreach (Triangle t in triangles)
            {
                Face f = new Face();

                Vector a = unit(cross(t.u.sub(t.w), t.n));
                Vector b = unit(cross(t.v.sub(t.u), t.n));
                Vector c = unit(cross(t.w.sub(t.v), t.n));

                f.length = new FixedPoint(dot(t.v.sub(t.u), c), 1 / 65536f);
                f.vertex_index = (ushort)vertex_welder.add(t.u);
                f.normal_index = (ushort)normal_welder.add(t.n);
                f.a_index = (ushort)normal_welder.add(a);
                f.b_index = (ushort)normal_welder.add(b);
                f.c_index = (ushort)normal_welder.add(c);
                f.group = (ushort)t.group;

                faces.Add(f);
            }

            //Vertex Section
            pos += 56;
            kcl.Write32(0x00, pos);//Vertex section offset

            foreach (Vertex vtx in vertex_welder.vertices)
            {
                kcl.Write32(pos, (uint)vtx.x.valToWrite());
                kcl.Write32(pos + 4, (uint)vtx.y.valToWrite());
                kcl.Write32(pos + 8, (uint)vtx.z.valToWrite());

                pos += 12;
            }

            //Vector Section
            kcl.Write32(0x04, pos);//Vector section offset

            foreach (Vector vct in normal_welder.vectors)
            {
                kcl.Write16(pos, (ushort)vct.x.valToWrite());
                kcl.Write16(pos + 2, (ushort)vct.y.valToWrite());
                kcl.Write16(pos + 4, (ushort)vct.z.valToWrite());

                pos += 6;
            }

            pos = (uint)((pos + 3) & ~3);

            //Planes Section
            kcl.Write32(0x08, pos - 0x10);//Planes section offset

            foreach (Face f in faces)
            {
                kcl.Write32(pos, (uint)f.length.valToWrite());
                kcl.Write16(pos + 4, (ushort)f.vertex_index);
                kcl.Write16(pos + 6, (ushort)f.normal_index);
                kcl.Write16(pos + 8, (ushort)f.a_index);
                kcl.Write16(pos + 10, (ushort)f.b_index);
                kcl.Write16(pos + 12, (ushort)f.c_index);
                kcl.Write16(pos + 14, (ushort)f.group);

                pos += 16;
            }


            //Octree Section
            kcl.Write32(0x0C, pos);//Octree offset

            Octree octree = new Octree(triangles, max_triangles, (float)min_width);
            octree.pack(ref kcl, ref pos);

            //Header
            kcl.Write32(0x10, (uint)327680);//Unknown
            kcl.Write32(0x14, (uint)octree.bas.x.valToWrite());
            kcl.Write32(0x18, (uint)octree.bas.y.valToWrite());
            kcl.Write32(0x1C, (uint)octree.bas.z.valToWrite());
            kcl.Write32(0x20, (uint)(~((int)octree.width_x - 1) & 0xFFFFFFFF));
            kcl.Write32(0x24, (uint)(~((int)octree.width_y - 1) & 0xFFFFFFFF));
            kcl.Write32(0x28, (uint)(~((int)octree.width_z - 1) & 0xFFFFFFFF));
            kcl.Write32(0x2C, (uint)(Math.Log(octree.base_width, 2)));
            kcl.Write32(0x30, (uint)(Math.Log(octree.nx, 2)));
            kcl.Write32(0x34, (uint)(Math.Log(octree.nx, 2)) + (uint)(Math.Log(octree.ny, 2)));

            if (save) { kcl.SaveChanges(); }
        }

        public static float dot(Vector a, Vector b)
        {
            return a.x.theValue * b.x.theValue + a.y.theValue * b.y.theValue + a.z.theValue * b.z.theValue;
        }

        public static float dot(Vector a, Vertex b)
        {
            return a.x.theValue * b.x.theValue + a.y.theValue * b.y.theValue + a.z.theValue * b.z.theValue;
        }

        public static float dot(Vertex a, Vector b)
        {
            return a.x.theValue * b.x.theValue + a.y.theValue * b.y.theValue + a.z.theValue * b.z.theValue;
        }

        public static Vector cross(Vector a, Vector b)//Vector, Vector > Vector
        {
            return new Vector(a.y.theValue * b.z.theValue - a.z.theValue * b.y.theValue, a.z.theValue * b.x.theValue - a.x.theValue * b.z.theValue,
                a.x.theValue * b.y.theValue - a.y.theValue * b.x.theValue);
        }

        public static Vector cross(Vertex a, Vertex b)//Vertex, Vertex > Vector
        {
            return new Vector(a.y.theValue * b.z.theValue - a.z.theValue * b.y.theValue, a.z.theValue * b.x.theValue - a.x.theValue * b.z.theValue,
                a.x.theValue * b.y.theValue - a.y.theValue * b.x.theValue);
        }

        public static Vector cross(Vertex a, Vector b)//Vertex, Vector > Vector
        {
            return new Vector(a.y.theValue * b.z.theValue - a.z.theValue * b.y.theValue, a.z.theValue * b.x.theValue - a.x.theValue * b.z.theValue,
                a.x.theValue * b.y.theValue - a.y.theValue * b.x.theValue);
        }

        public static Vector unit(Vector a)
        {
            Vector r = a.truediv(a.norm());
            return r;
        }

        public static bool tribox_overlap(Triangle triangle, Vertex centre, float half_width)
        {
            /*Intersection test for triangle and axis-aligned cube.

              Test if the triangle  intersects the axis-aligned cube given by the center
              and half width. This algorithm is an adapted version of the algorithm
              presented here:
              http://fileadmin.cs.lth.se/cs/Personal/Tomas_Akenine-Moller/code/tribox3.txt
            */

            Vertex u = triangle.u.sub(centre);
            Vertex v = triangle.v.sub(centre);
            Vertex w = triangle.w.sub(centre);
            Vector n = triangle.n;

            // Test for separation along the axes normal to the faces of the cube

            if (max(u.x.theValue, v.x.theValue, w.x.theValue) < -half_width || min(u.x.theValue, v.x.theValue, w.x.theValue) > half_width) return false;
            if (max(u.y.theValue, v.y.theValue, w.y.theValue) < -half_width || min(u.y.theValue, v.y.theValue, w.y.theValue) > half_width) return false;
            if (max(u.z.theValue, v.z.theValue, w.z.theValue) < -half_width || min(u.z.theValue, v.z.theValue, w.z.theValue) > half_width) return false;

            // Test for separation along the axis normal to the face of the triangle

            float d = dot(n, u);
            float r = half_width * (Math.Abs(n.x.theValue) + Math.Abs(n.y.theValue) + Math.Abs(n.z.theValue));
            if (d < -r || d > r) return false;

            // Test for separation along the axes parallel to the cross products of the
            // edges of the triangle and the edges of the cube

            if (edge_test(u, v, w, half_width)) return false;
            if (edge_test(v, w, u, half_width)) return false;
            if (edge_test(w, u, v, half_width)) return false;

            // Triangle and cube intersects
            return true;

        }

        public static bool edge_axis_test(float a1, float a2, float b1, float b2, float c1, float c2, float hw)
        {
            float p = a1 * b1 + a2 * b2;
            float q = a1 * c1 + a2 * c2;
            float r = hw * (Math.Abs(a1) + Math.Abs(a2));
            return max(p, q) < -r || min(p, q) > r;
        }

        public static bool edge_test(Vertex v0, Vertex v1, Vertex v2, float hw)
        {
            Vertex e = v1.sub(v0);
            if (edge_axis_test(e.z.theValue, -e.y.theValue, v0.y.theValue, v0.z.theValue, v2.y.theValue, v2.z.theValue, hw))
                return true;
            if (edge_axis_test(-e.z.theValue, e.x.theValue, v0.x.theValue, v0.z.theValue, v2.x.theValue, v2.z.theValue, hw))
                return true;
            if (edge_axis_test(e.y.theValue, -e.x.theValue, v0.x.theValue, v0.y.theValue, v2.x.theValue, v2.y.theValue, hw))
                return true;
            return false;
        }

        public static float min(params float[] v)
        {
            float min = float.MaxValue;
            foreach (float f in v)
            {
                if (f < min)
                {
                    min = f;
                }
            }
            return min;
        }
        public static float max(params float[] v)
        {
            float max = float.MinValue;
            foreach (float f in v)
            {
                if (f > max)
                {
                    max = f;
                }
            }
            return max;
        }

    }

    public class FixedPoint
    {
        public float theValue;
        public float scale;

        public FixedPoint(float theValue, float scale)
        {
            this.theValue = theValue;
            this.scale = scale;
        }

        public FixedPoint(float scale)
        {
            this.theValue = 0f;
            this.scale = scale;
        }

        public long valToWrite()
        {
            return (long)(theValue / scale);
        }

        public int sizeOf()
        {
            return 4;
        }

    }

    public class Face
    {
        public float lScale = (1 / 65536f);//Scale for length fixed point value
        public FixedPoint length = new FixedPoint(1 / 65536f);
        public ushort vertex_index;
        public ushort normal_index;
        public ushort a_index;
        public ushort b_index;
        public ushort c_index;
        public ushort group;//Collision type, default 0, can change in KCL Editor

        public Face() { }

    }//End Face class

    public class Vertex
    {
        //Uses signed 32 bit integers
        public FixedPoint x;
        public FixedPoint y;
        public FixedPoint z;
        public float scale = (1 / 64f);

        public Vertex()
        {
            this.x = new FixedPoint(0f, scale);
            this.y = new FixedPoint(0f, scale);
            this.z = new FixedPoint(0f, scale);
        }

        public Vertex(float x, float y, float z)
        {
            this.x = new FixedPoint(x, scale);
            this.y = new FixedPoint(y, scale);
            this.z = new FixedPoint(z, scale);
        }

        public Vertex(float x, float y, float z, float scale)
        {
            this.scale = scale;
            this.x = new FixedPoint(x, scale);
            this.y = new FixedPoint(y, scale);
            this.z = new FixedPoint(z, scale);
        }

        public Vertex add(Vertex other)
        {
            return new Vertex(this.x.theValue + other.x.theValue, this.y.theValue + other.y.theValue, this.z.theValue + other.z.theValue);
        }

        public Vertex sub(Vertex other)
        {
            return new Vertex(this.x.theValue - other.x.theValue, this.y.theValue - other.y.theValue, this.z.theValue - other.z.theValue);
        }

        public Vertex mul(float scalar)
        {
            return new Vertex(this.x.theValue * scalar, this.y.theValue * scalar, this.z.theValue * scalar);
        }

        public Vertex rmul(float scalar)
        {
            return new Vertex(scalar * this.x.theValue, scalar * this.y.theValue, scalar * this.z.theValue);
        }

        public Vertex truediv(float scalar)
        {
            return new Vertex(this.x.theValue / scalar, this.y.theValue / scalar, this.z.theValue / scalar);
        }

        public Vertex iadd(Vertex other)
        {
            this.x.theValue += other.x.theValue;
            this.y.theValue += other.y.theValue;
            this.z.theValue += other.z.theValue;
            return this;
        }

        public Vertex isub(Vertex other)
        {
            this.x.theValue -= other.x.theValue;
            this.y.theValue -= other.y.theValue;
            this.z.theValue -= other.z.theValue;
            return this;
        }

        public Vertex imul(float scalar)
        {
            this.x.theValue *= scalar;
            this.y.theValue *= scalar;
            this.z.theValue *= scalar;
            return this;
        }

        public Vertex itruediv(float scalar)
        {
            this.x.theValue /= scalar;
            this.y.theValue /= scalar;
            this.z.theValue /= scalar;
            return this;
        }

        public float norm_sq()
        {
            return this.x.theValue * this.x.theValue + this.y.theValue * this.y.theValue + this.z.theValue * this.z.theValue;
        }

        public float norm()
        {
            return (float)Math.Sqrt(this.norm_sq());
        }

    }//End Vertex class

    public class Vector
    {
        //Uses signed 16 bit integers
        public FixedPoint x;
        public FixedPoint y;
        public FixedPoint z;
        public float scale = (1 / 1024f);

        public Vector()
        {
            this.x = new FixedPoint(0f, scale);
            this.y = new FixedPoint(0f, scale);
            this.z = new FixedPoint(0f, scale);
        }

        public Vector(float x, float y, float z)
        {
            this.x = new FixedPoint(x, scale);
            this.y = new FixedPoint(y, scale);
            this.z = new FixedPoint(z, scale);
        }

        public Vector(float x, float y, float z, float scale)
        {
            this.scale = scale;
            this.x = new FixedPoint(x, scale);
            this.y = new FixedPoint(y, scale);
            this.z = new FixedPoint(z, scale);
        }

        public Vector add(Vector other)
        {
            return new Vector(this.x.theValue + other.x.theValue, this.y.theValue + other.y.theValue, this.z.theValue + other.z.theValue);
        }

        public Vector sub(Vector other)
        {
            return new Vector(this.x.theValue - other.x.theValue, this.y.theValue - other.y.theValue, this.z.theValue - other.z.theValue);
        }

        public Vector mul(float scalar)
        {
            return new Vector(this.x.theValue * scalar, this.y.theValue * scalar, this.z.theValue * scalar);
        }

        public Vector rmul(float scalar)
        {
            return new Vector(scalar * this.x.theValue, scalar * this.y.theValue, scalar * this.z.theValue);
        }

        public Vector truediv(float scalar)
        {
            return new Vector(this.x.theValue / scalar, this.y.theValue / scalar, this.z.theValue / scalar);
        }

        public Vector iadd(Vector other)
        {
            this.x.theValue += other.x.theValue;
            this.y.theValue += other.y.theValue;
            this.z.theValue += other.z.theValue;
            return this;
        }

        public Vector isub(Vector other)
        {
            this.x.theValue -= other.x.theValue;
            this.y.theValue -= other.y.theValue;
            this.z.theValue -= other.z.theValue;
            return this;
        }

        public Vector imul(float scalar)
        {
            this.x.theValue *= scalar;
            this.y.theValue *= scalar;
            this.z.theValue *= scalar;
            return this;
        }

        public Vector itruediv(float scalar)
        {
            this.x.theValue /= scalar;
            this.y.theValue /= scalar;
            this.z.theValue /= scalar;
            return this;
        }

        public float norm_sq()
        {
            return this.x.theValue * this.x.theValue + this.y.theValue * this.y.theValue + this.z.theValue * this.z.theValue;
        }

        public float norm()
        {
            return (float)Math.Sqrt(this.norm_sq());
        }

        public Vector div(Vector other)
        {
            return new Vector(this.x.theValue / other.x.theValue, this.y.theValue / other.y.theValue, this.z.theValue / other.z.theValue);
        }

    }//End Vector class

    public class Triangle
    {
        public Vertex u;
        public Vertex v;
        public Vertex w;
        public Vector n;
        public int group;

        public Triangle(Vertex u, Vertex v, Vertex w, int group)
        {
            this.u = u;
            this.v = v;
            this.w = w;
            this.n = KCLWriter.unit(KCLWriter.cross(v.sub(u), w.sub(u)));
            this.group = group;
        }
    }//End Triangle class

    public class VertexWelder
    {
        // Three randomly chosen large primes
        uint magic_x = 0x8DA6B343;
        uint magic_y = 0xD8163841;
        uint magic_z = 0x61B40079;

        public float threshold;
        public float cell_width;
        public int num_buckets;
        public List<int>[] buckets;
        public List<Vertex> vertices;
        public List<Vector> vectors;

        public VertexWelder(float threshold, int num_buckets)
        {
            this.threshold = threshold;
            this.cell_width = 16 * threshold;
            if (num_buckets == 0)
                num_buckets += 1;
            this.num_buckets = num_buckets;
            this.buckets = new List<int>[num_buckets];
            for (int i = 0; i < num_buckets; i++)//Initialise the List<int> 's in the array
            {
                this.buckets[i] = new List<int>();
            }
            this.vertices = new List<Vertex>();
            this.vectors = new List<Vector>();
        }

        public int vertex_hash(int ix, int iy, int iz)
        {
            int result = (int)(ix * this.magic_x + iy * this.magic_y + iz * this.magic_z) % this.num_buckets;
            if (result < 0)
                result *= (-1);
            return result;
        }

        public int add(Vertex v)//For dealing with type Vertex
        {
            int min_ix = (int)((v.x.theValue - this.threshold) / this.cell_width);
            int min_iy = (int)((v.y.theValue - this.threshold) / this.cell_width);
            int min_iz = (int)((v.z.theValue - this.threshold) / this.cell_width);
            int max_ix = (int)((v.x.theValue + this.threshold) / this.cell_width);
            int max_iy = (int)((v.y.theValue + this.threshold) / this.cell_width);
            int max_iz = (int)((v.z.theValue + this.threshold) / this.cell_width);

            for (int ix = min_ix; ix < max_ix + 1; ix++)
            {
                for (int iy = min_iy; iy < max_iy + 1; iy++)
                {
                    for (int iz = min_iz; iz < max_iz + 1; iz++)
                    {
                        foreach (int index in this.buckets[(int)this.vertex_hash(ix, iy, iz)])
                        {
                            if (v.sub(this.vertices[index]).norm_sq() < this.threshold * this.threshold)
                                return index;
                        }
                    }
                }
            }

            this.vertices.Add(v);
            this.buckets[(int)this.vertex_hash((int)(v.x.theValue / this.cell_width), (int)(v.y.theValue / this.cell_width),
                (int)(v.z.theValue / this.cell_width))].Add(this.vertices.Count - 1);
            return (this.vertices.Count - 1);
        }

        public int add(Vector v)//For dealing with type Vector
        {
            int min_ix = (int)((v.x.theValue - this.threshold) / this.cell_width);
            int min_iy = (int)((v.y.theValue - this.threshold) / this.cell_width);
            int min_iz = (int)((v.z.theValue - this.threshold) / this.cell_width);
            int max_ix = (int)((v.x.theValue + this.threshold) / this.cell_width);
            int max_iy = (int)((v.y.theValue + this.threshold) / this.cell_width);
            int max_iz = (int)((v.z.theValue + this.threshold) / this.cell_width);

            for (int ix = min_ix; ix < max_ix + 1; ix++)
            {
                for (int iy = min_iy; iy < max_iy + 1; iy++)
                {
                    for (int iz = min_iz; iz < max_iz + 1; iz++)
                    {
                        foreach (int index in this.buckets[(int)this.vertex_hash(ix, iy, iz)])
                        {
                            if (v.sub(this.vectors[index]).norm_sq() < this.threshold * this.threshold)
                                return index;
                        }
                    }
                }
            }

            this.vectors.Add(v);
            this.buckets[(int)this.vertex_hash((int)(v.x.theValue / this.cell_width), (int)(v.y.theValue / this.cell_width),
                (int)(v.z.theValue / this.cell_width))].Add(this.vectors.Count - 1);
            return (this.vectors.Count - 1);
        }


    }//End VertexWelder class

    public class Octree
    {
        /*
        Octree(triangles,max_triangles,min_width)

        Returns an octree where the cube of each leaf node intersects less than
        max_triangles of the triangles, unless that would make the width of the cube
        less than min_width.
        */

        public List<Triangle> triangles = new List<Triangle>();
        public int max_triangles;
        public float min_width;
        public List<int> indices = new List<int>();

        public bool is_leaf = false;

        public List<Octree> children = new List<Octree>();

        public float width_x, width_y, width_z, base_width;
        public Vertex bas;
        public int nx, ny, nz;

        //Base octree constructor
        public Octree(List<Triangle> triangles, int max_triangles, float min_width)
        {
            this.triangles = triangles;
            this.max_triangles = max_triangles;
            this.min_width = min_width;

            float min_x = 0, min_y = 0, min_z = 0, max_x = 0, max_y = 0, max_z = 0;

            foreach (Triangle t in triangles)
            {
                float min_x0 = KCLWriter.min(KCLWriter.min(t.u.x.theValue, t.v.x.theValue, t.w.x.theValue));
                if (min_x0 < min_x) min_x = min_x0;
                float min_y0 = KCLWriter.min(KCLWriter.min(t.u.y.theValue, t.v.y.theValue, t.w.y.theValue));
                if (min_y0 < min_y) min_y = min_y0;
                float min_z0 = KCLWriter.min(KCLWriter.min(t.u.z.theValue, t.v.z.theValue, t.w.z.theValue));
                if (min_z0 < min_z) min_z = min_z0;
                float max_x0 = KCLWriter.max(KCLWriter.max(t.u.x.theValue, t.v.x.theValue, t.w.x.theValue));
                if (max_x0 > max_x) max_x = max_x0;
                float max_y0 = KCLWriter.max(KCLWriter.max(t.u.y.theValue, t.v.y.theValue, t.w.y.theValue));
                if (max_y0 > max_y) max_y = max_y0;
                float max_z0 = KCLWriter.max(KCLWriter.max(t.u.z.theValue, t.v.z.theValue, t.w.z.theValue));
                if (max_z0 > max_z) max_z = max_z0;
            }

            // If model only uses two axes, eg. flat square, the base width will get set to min_width (1) which can 
            // create an octree with 100's of thousands of tiny empty or almost empty nodes is very computationally expensive
            if (max_x == 0)
                max_x = KCLWriter.max(max_y, max_z);
            if (max_y == 0)
                max_y = KCLWriter.max(max_x, max_z);
            if (max_z == 0)
                max_z = KCLWriter.max(max_x, max_y);

            this.width_x = (float)Math.Pow(2, (int)(Math.Ceiling(Math.Log(KCLWriter.max(max_x - min_x, min_width), 2))));
            this.width_y = (float)Math.Pow(2, (int)(Math.Ceiling(Math.Log(KCLWriter.max(max_y - min_y, min_width), 2))));
            this.width_z = (float)Math.Pow(2, (int)(Math.Ceiling(Math.Log(KCLWriter.max(max_z - min_z, min_width), 2))));
            this.base_width = KCLWriter.min(width_x, width_y, width_z);
            this.bas = new Vertex(min_x, min_y, min_z);
            
            // Cap the number of boxes at 128.
            while(this.width_x * this.width_y * this.width_z / (this.base_width * this.base_width * this.base_width) >
                KCLWriter.MAX_BASE_OCTREE_COUNT)
            {
                if (this.width_x == this.base_width) this.width_x *= 2;
                if (this.width_y == this.base_width) this.width_y *= 2;
                if (this.width_z == this.base_width) this.width_z *= 2;
                this.base_width *= 2;
            }

            this.nx = (int)Math.Floor(this.width_x / this.base_width);
            this.ny = (int)Math.Floor(this.width_y / this.base_width);
            this.nz = (int)Math.Floor(this.width_z / this.base_width);

            List<int> ind = new List<int>();
            for (int i = 0; i < triangles.Count; i++)
                ind.Add(i);

            for (int k = 0; k < this.nz; k++)
            {
                for (int j = 0; j < this.ny; j++)
                {
                    for (int i = 0; i < this.nx; i++)
                    {
                        this.children.Add(new Octree(this.bas.add((new Vertex(i, j, k)).mul(this.base_width)), this.base_width,
                            ind, this.triangles, this.max_triangles, this.min_width));
                    }
                }
            }

        }//End of Octree constructor

        //Constructor for creating nodes in the octree
        public Octree(Vertex bas, float width, List<int> indices, List<Triangle> triangles, int max_triangles, float min_width)
        {
            Vertex centre = bas.add(new Vertex(width, width, width).truediv(2f));
            this.triangles = triangles;
            this.max_triangles = max_triangles;
            this.min_width = min_width;

            this.width_x /= 2f;
            this.width_y /= 2f;
            this.width_z /= 2f;
            this.base_width = KCLWriter.min(width_x, width_y, width_z);
            this.bas = new Vertex(width / 2f, width / 2f, width / 2f);

            foreach (int i in indices)
            {
                if (KCLWriter.tribox_overlap(this.triangles[i], centre, width / 2f))
                {
                    this.indices.Add(i);
                }
            }
            this.is_leaf = true;

            //Console.WriteLine("node hw: " + width / 2f + " num of indices: " + this.indices.Count);

            if (this.indices.Count > this.max_triangles && width >= (2 * this.min_width))
            {
                for (int k = 0; k < 2; k++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            this.children.Add(new Octree(bas.add(new Vertex((float)i, (float)j, (float)k).truediv(2f).mul(width)), width / 2,
                                    this.indices, this.triangles, this.max_triangles, this.min_width));
                        }
                    }
                }
                this.indices.Clear();
                this.is_leaf = false;
            }
        }

        public Octree getitem(List<int> key)
        {
            return this.children[key[0] + this.nx * (key[1] + this.ny * key[2])];
        }

        public void pack(ref NitroFile kcl, ref uint pos)
        {
            List<Octree> branches = new List<Octree>();
            branches.Add(this);
            uint free_list_offset = 0;
            List<List<int>> list_offsets_ind = new List<List<int>>();
            List<uint> list_offsets_addr = new List<uint>();

            //testing
            int zero_ind_count = 0;
            int ind_already_in_count = 0;
            int list_offsets_added_count = 0;
            int else_branches_append_count = 0;
            //end testing
            int i = 0;
            while (i < branches.Count)
            {
                foreach (Octree node in branches[i].children)
                {
                    if (node.is_leaf)
                    {
                        if (node.indices.Count == 0)
                        {
                            zero_ind_count += 1;
                            continue;
                        }
                        if (indicesInList(list_offsets_ind, node.indices))
                        {
                            ind_already_in_count += 1;
                            continue;
                        }
                        list_offsets_ind.Add(node.indices);
                        list_offsets_addr.Add(free_list_offset);
                        list_offsets_added_count += 1;
                        free_list_offset += (uint)(2 * ((node.indices.Count) + 1));
                    }
                    else
                    {
                        branches.Add(node);
                        else_branches_append_count += 1;
                    }
                }
                i += 1;
            }

            uint list_base = 0;
            foreach (Octree b in branches)
            {
                list_base += (uint)(4 * b.children.Count);
            }
            list_offsets_ind.Add(new List<int>());//Add an empty indices list 
            list_offsets_addr.Add(free_list_offset - 2);//with this address
            uint branch_base = 0;
            uint free_branch_offset = (uint)(4 * (this.children.Count));

            foreach (Octree branch in branches)
            {
                foreach (Octree node in branch.children)
                {
                    if (node.is_leaf)
                    {
                        int key = posIndicesInList(list_offsets_ind, node.indices);
                        kcl.Write32(pos, (uint)(0x80000000 | (list_base + list_offsets_addr[key] - 2 - branch_base)));
                        pos += 4;
                    }
                    else
                    {
                        kcl.Write32(pos, (free_branch_offset - branch_base));
                        pos += 4;
                        free_branch_offset += (uint)(4 * (node.children.Count));
                    }
                }
                branch_base += (uint)(4 * (branch.children.Count));
            }

            list_offsets_ind.RemoveAt(list_offsets_ind.Count - 1);
            list_offsets_addr.RemoveAt(list_offsets_addr.Count - 1);

            foreach (List<int> indices in list_offsets_ind)
            {
                foreach (int index in indices)
                {
                    kcl.Write16(pos, (ushort)(index + 1));
                    pos += 2;
                }
                kcl.Write16(pos, (ushort)0);
                pos += 2;
            }

        }//End pack function

        public bool indicesInList(List<List<int>> bigList, List<int> indices)
        {
            if (bigList.Count == 0)
                return false;
            for (int i = 0; i < bigList.Count; i++)
            {
                List<int> compare = bigList[i];
                if (compare.Count != indices.Count)
                    continue;
                else
                {
                    int wrongFlag = 0;
                    for (int j = 0; j < compare.Count; j++)
                    {
                        if (compare[j] != indices[j])
                            wrongFlag += 1;
                    }
                    if (wrongFlag == 0)//No differences
                        return true;//They're the same
                }
            }
            return false;
        }

        public int posIndicesInList(List<List<int>> bigList, List<int> indices)
        {
            if (bigList.Count == 0)
                return -1;
            for (int i = 0; i < bigList.Count; i++)
            {
                List<int> compare = bigList[i];
                if (compare.Count != indices.Count)
                    continue;
                else
                {
                    int wrongFlag = 0;
                    for (int j = 0; j < compare.Count; j++)
                    {
                        if (compare[j] != indices[j])
                            wrongFlag += 1;
                    }
                    if (wrongFlag == 0)//No differences
                        return i;//They're the same, return position
                }
            }
            return -1;//Not found
        }

    }//End Octree class
}
