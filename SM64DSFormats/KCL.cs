/*
    Copyright 2012 Kuribo64

    This file is part of SM64DSe.

    SM64DSe is free software: you can redistribute it and/or modify it under
    the terms of the GNU General Public License as published by the Free
    Software Foundation, either version 3 of the License, or (at your option)
    any later version.

    SM64DSe is distributed in the hope that it will be useful, but WITHOUT ANY 
    WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS 
    FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along 
    with SM64DSe. If not, see http://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;

namespace SM64DSe
{
    public class KCL
    {
        public KCL(NitroFile file)
        {
            m_File = file;

	        m_Planes = new List<ColFace>();

	        m_PointsSectionOffset = m_File.Read32(0x00);
	        m_NormalsSectionOffset = m_File.Read32(0x04);
	        m_PlanesSectionOffset = m_File.Read32(0x08);
	        m_OctreeSectionOffset = m_File.Read32(0x0C);

            int planeid = 0;
	        for (uint offset = m_PlanesSectionOffset + 0x10; offset < m_OctreeSectionOffset; offset += 0x10)
	        {
		        uint length = m_File.Read32(offset);

		        ushort pt_id = m_File.Read16(offset + 0x04);
		        int pt_x = (int)m_File.Read32((uint)(m_PointsSectionOffset + (pt_id*12)    ));
		        int pt_y = (int)m_File.Read32((uint)(m_PointsSectionOffset + (pt_id*12) + 4));
		        int pt_z = (int)m_File.Read32((uint)(m_PointsSectionOffset + (pt_id*12) + 8));

		        ushort nr_id = m_File.Read16(offset + 0x06);
		        short nr_x = (short)m_File.Read16((uint)(m_NormalsSectionOffset + (nr_id*6)    ));
		        short nr_y = (short)m_File.Read16((uint)(m_NormalsSectionOffset + (nr_id*6) + 2));
		        short nr_z = (short)m_File.Read16((uint)(m_NormalsSectionOffset + (nr_id*6) + 4));

		        ushort d1_id = m_File.Read16(offset + 0x08);
		        short d1_x = (short)m_File.Read16((uint)(m_NormalsSectionOffset + (d1_id*6)    ));
                short d1_y = (short)m_File.Read16((uint)(m_NormalsSectionOffset + (d1_id*6) + 2));
		        short d1_z = (short)m_File.Read16((uint)(m_NormalsSectionOffset + (d1_id*6) + 4));

		        ushort d2_id = m_File.Read16(offset + 0x0A);
		        short d2_x = (short)m_File.Read16((uint)(m_NormalsSectionOffset + (d2_id*6)    ));
		        short d2_y = (short)m_File.Read16((uint)(m_NormalsSectionOffset + (d2_id*6) + 2));
		        short d2_z = (short)m_File.Read16((uint)(m_NormalsSectionOffset + (d2_id*6) + 4));

		        ushort d3_id = m_File.Read16(offset + 0x0C);
		        short d3_x = (short)m_File.Read16((uint)(m_NormalsSectionOffset + (d3_id*6)    ));
		        short d3_y = (short)m_File.Read16((uint)(m_NormalsSectionOffset + (d3_id*6) + 2));
		        short d3_z = (short)m_File.Read16((uint)(m_NormalsSectionOffset + (d3_id*6) + 4));

                ColFace plane = new ColFace((float)(length / 65536000f), 
                    new Vector3((float)pt_x / 64000f, (float)pt_y / 64000f, (float)pt_z / 64000f), 
                    new Vector3((float)nr_x / 1024f, (float)nr_y / 1024f, (float)nr_z / 1024f), 
                    new Vector3((float)d1_x / 1024f, (float)d1_y / 1024f, (float)d1_z / 1024f), 
                    new Vector3((float)d2_x / 1024f, (float)d2_y / 1024f, (float)d2_z / 1024f), 
                    new Vector3((float)d3_x / 1024f, (float)d3_y / 1024f, (float)d3_z / 1024f), 
                    m_File.Read16(offset + 0x0E));

               /* if (planeid == 31)
                    MessageBox.Show(string.Format("PLANE 32:\n{0}\n{1}\n{2}\n\n{3}\n{4}\n{5}\n\n{6}\n{7}\n{8}\n{9}\n{10}\n{11}",
                        d1_x, d1_y, d1_z,
                        d2_x, d2_y, d2_z,
                        plane.m_Position, plane.m_Normal, plane.m_Dir1, plane.m_Dir2, plane.m_Dir3, plane.m_Length));*/

                planeid++;

               /* if (Math.Abs(plane.m_Dir1.Length - 1f) > 0.0001f || Math.Abs(plane.m_Dir2.Length - 1f) > 0.0001f ||
                    Math.Abs(plane.m_Dir3.Length - 1f) > 0.0001f || Math.Abs(plane.m_Normal.Length - 1f) > 0.0001f || 
                    plane.m_Length < 0f)
                    MessageBox.Show(string.Format("WRONG PLANE | {0} | {1} | {2} | {3} | {4}",
                        plane.m_Dir1.Length, plane.m_Dir2.Length, plane.m_Dir3.Length, plane.m_Normal.Length, plane.m_Length));

                if (plane.m_Dir1.Length < 0.001f || plane.m_Dir2.Length < 0.001f ||
                    plane.m_Dir3.Length < 0.001f || plane.m_Normal.Length < 0.001f ||
                    Math.Abs(plane.m_Length) < 0.001f)
                    MessageBox.Show(string.Format("ZERO PLANE | {0} | {1} | {2} | {3} | {4}",
                        plane.m_Dir1.Length, plane.m_Dir2.Length, plane.m_Dir3.Length, plane.m_Normal.Length, plane.m_Length));

                Vector3 lol1 = Vector3.Cross(plane.m_Dir1, plane.m_Normal);
                float lol1len = plane.m_Length / (float)Math.Cos(Math.Acos(Math.Min(1f,Vector3.Dot(lol1, plane.m_Dir3))));
                Vector3 lol2 = Vector3.Cross(plane.m_Normal, plane.m_Dir2);
                float lol2len = plane.m_Length / (float)Math.Cos(Math.Acos(Math.Min(1f,Vector3.Dot(lol2, plane.m_Dir3))));
                if (Helper.VectorsEqual(plane.m_Position, Vector3.Multiply(lol1, lol1len)) ||
                    Helper.VectorsEqual(plane.m_Position, Vector3.Multiply(lol2, lol2len)))
                    MessageBox.Show(string.Format("WEIRD PLANE {5:X8} | {0} | {1} | {2} | {3} | {4}\n{6} | {7} / cos(acos({8})) = cos({9}) = {10}",
                        plane.m_Dir1, plane.m_Dir2, plane.m_Dir3, plane.m_Normal, plane.m_Length,
                        offset, lol2, plane.m_Length, Vector3.Dot(lol2, plane.m_Dir3), Math.Acos(Vector3.Dot(lol2, plane.m_Dir3)), Math.Cos(Math.Acos(Vector3.Dot(lol2, plane.m_Dir3)))));
                */
		        m_Planes.Add(plane);
	        }

            OctreeNode.maxkids = 0;
            int shift = (int)m_File.Read32(0x2C);
            Vector3 octreestart = new Vector3((float)(int)m_File.Read32(0x14) / 64000f, (float)(int)m_File.Read32(0x18) / 64000f, (float)(int)m_File.Read32(0x1C) / 64000f);
            float _cubesize = (float)(1 << shift) / 1024f;
            Vector3 cubesize = new Vector3(_cubesize, _cubesize, _cubesize);
            Vector3 octreesize = new Vector3((~m_File.Read32(0x20) >> shift) + 1, (~m_File.Read32(0x24) >> shift) + 1, (~m_File.Read32(0x28) >> shift) + 1);
            OctreeNode.m_List = new List<OctreeNode>();
            uint loloffset = m_OctreeSectionOffset;
            for (int z = 0; z < octreesize.Z; z++)
                for (int y = 0; y < octreesize.Y; y++)
                    for (int x = 0; x < octreesize.X; x++)
                    { new OctreeNode(m_File, m_OctreeSectionOffset, loloffset, octreestart + new Vector3(cubesize.X * x, cubesize.Y * y, cubesize.Z * z), cubesize); loloffset += 4; }

            //MessageBox.Show(OctreeNode.m_List.Count.ToString());
        }

        public class ColFace
        {
            public float length;
            public Vector3 point1;
            public Vector3 point2;
            public Vector3 point3;
            public Vector3 normal;
            public int type;
            public Vector3 dir1;
            public Vector3 dir2;
            public Vector3 dir3;

            public ColFace(float lengthIn, Vector3 originPoint, Vector3 normalIn, Vector3 dir1, Vector3 dir2, Vector3 dir3, int typeIn)
            {
                length = lengthIn;
                normal = normalIn;
                point1 = originPoint;
                /*
                Collision Tools v0.6 by blank
                v0 = vertices[t.vertex_index] //The given vertex
                v2 = v0 + cross(n,a)*t.length/dot(cross(n,a),c)
                v1 = v0 + cross(n,b)*t.length/dot(cross(n,b),c)
                */
                Vector3 crossB = Vector3.Cross(normal, dir2);
                Vector3 crossA = Vector3.Cross(normal, dir1);
                float dotB = Vector3.Dot(crossB, dir3);
                float dotA = Vector3.Dot(crossA, dir3);
                point2 = point1 + crossB * (dotB != 0f ? length / dotB : 0f);
                point3 = point1 + crossA * (dotA != 0f ? length / dotA : 0f);

                type = typeIn;
                this.dir1 = dir1;
                this.dir2 = dir2;
                this.dir3 = dir3;
            }
        }

        public class OctreeNode
        {
            public static List<OctreeNode> m_List;
            public static int maxkids = 0;

            public Vector3 m_Pos, m_Size;
            public int m_NumPlanes;
            public bool m_LOL;
            public List<int> m_PlaneList;

            public OctreeNode(NitroFile file, uint baseoffset, uint offset, Vector3 pos, Vector3 size)
            {
                m_Pos = pos;
                m_Size = size;
                m_LOL = false;
                m_PlaneList = new List<int>();

                uint node = file.Read32(offset);
                if ((node & 0x80000000) != 0)
                {
                    uint lolz = baseoffset + (node & 0x7FFFFFFF) + 2;
                    int n = 0;
                    string lmao = "";
                    for (; ; )
                    {
                        ushort p = file.Read16(lolz);
                        if (p == 0)
                            break;
                        else if (p == 37)
                            m_LOL = true;

                        m_PlaneList.Add(p - 1);
                        lmao += (p - 1).ToString() + " ";
                        lolz += 2;
                        n++;
                    }

                    if (n > maxkids)
                        maxkids = n;

                    //if (m_LOL)
                     //   MessageBox.Show(lmao);

                    m_NumPlanes = n;
                    OctreeNode.m_List.Add(this);
                }
                else
                {
                    uint parentoffset = baseoffset + node;
                    uint child0offset = parentoffset;
                    size /= 2f;
                    for (int z = 0; z < 2; z++)
                        for (int y = 0; y < 2; y++)
                            for (int x = 0; x < 2; x++)
                            { new OctreeNode(file, child0offset, parentoffset, pos + new Vector3(size.X * x, size.Y * y, size.Z * z), size); parentoffset += 4; }
                }
            }

            public bool ContainsPoint(Vector3 point)
            {
                return point.X >= m_Pos.X && point.X <= m_Pos.X + m_Size.X &&
                       point.Y >= m_Pos.Y && point.Y <= m_Pos.Y + m_Size.Y &&
                       point.Z >= m_Pos.Z && point.Z <= m_Pos.Z + m_Size.Z;
            }

            public bool IntersectsRay(ref Vector3 start, ref Vector3 dir)
            {
                //Add padding to separate edge cases from no-intersect cases
                float tMin = -0.1f, tMax = 1.1f;

                //Algorithm borrowed and modified from tavianator.com/fast-branchless-raybounding-box-intersections/
                //Divisions by 0 should be handled by IEEE-754 floating-point standards.
                //NaNs are assumed to be rays scraping the edge of a box and therefore count as intersections.
                //Besides, a false positive is better than a false negative.
                float t0 = (m_Pos.X            - start.X) / dir.X;
                float t1 = (m_Pos.X + m_Size.X - start.X) / dir.X;
                tMin = Math.Max(tMin, Math.Min(t0, t1));
                tMax = Math.Min(tMax, Math.Max(t0, t1));

                t0 = (m_Pos.Y            - start.Y) / dir.Y;
                t1 = (m_Pos.Y + m_Size.Y - start.Y) / dir.Y;
                tMin = Math.Max(tMin, Math.Min(t0, t1));
                tMax = Math.Min(tMax, Math.Max(t0, t1));

                t0 = (m_Pos.Z            - start.Z) / dir.Z;
                t1 = (m_Pos.Z + m_Size.Z - start.Z) / dir.Z;
                tMin = Math.Max(tMin, Math.Min(t0, t1));
                tMax = Math.Min(tMax, Math.Max(t0, t1));

                return double.IsNaN(tMin) || double.IsNaN(tMax) ||
                    (tMin <= 1 && tMax >= 0 && tMax >= tMin);
            }
        }

        public struct RaycastResult
        {
            public Vector3 m_Point;
            public Vector3 m_Normal;
            public float m_T;

            public RaycastResult(Vector3 point, Vector3 normal, float t)
            {
                m_Point = point;
                m_Normal = normal;
                m_T = t;
            }
        }

        public RaycastResult? Raycast(Vector3 start, Vector3 dir)
        {
            float tFirst = (float)double.PositiveInfinity;
            Vector3? currPoint = null;
            Vector3? currNorm = null;

            foreach(OctreeNode octbox in OctreeNode.m_List)
            {
                //The condition glitches at times and since a stage model has less than
                //about 2500 polygons, it's not worth fixing right now.
                if (true /*octbox.IntersectsRay(ref start, ref dir)*/)
                {
                    for (int i = 0; i < octbox.m_PlaneList.Count; ++i)
                    {
                        if (octbox.m_PlaneList[i] >= m_Planes.Count) continue;
                        ColFace tri = m_Planes[octbox.m_PlaneList[i]];

                        //Find point of intersection
                        float dot = Vector3.Dot(tri.normal, dir);
                        if (dot >= 0) continue; //Either they don't intersect or they graze. No grazes allowed here!

                        float t = Vector3.Dot(tri.normal, tri.point1 - start) / dot;
                        if (t < 0 || t > 1 || t >= tFirst) continue; //Nope! Gotta hit the plane! (or too far back)

                        //Find out if the intersection point is inside the triangle.
                        Vector3 intersection = dir * t + start;
                        Vector3 basisX = tri.point2 - tri.point1,
                                basisY = tri.point3 - tri.point1,
                                intFromPoint1 = intersection - tri.point1;

                        //OpenTK got rid of a function that multiplies a matrix3 by a vector3
                        Matrix3 planeBasis = new Matrix3(basisX.X, basisY.X, tri.normal.X,
                                                         basisX.Y, basisY.Y, tri.normal.Y,
                                                         basisX.Z, basisY.Z, tri.normal.Z).Inverted();
                        Vector3 resultant = planeBasis.Column0 * intFromPoint1.X +
                                            planeBasis.Column1 * intFromPoint1.Y +
                                            planeBasis.Column2 * intFromPoint1.Z;

                        if (resultant.X >= 0 && resultant.Y >= 0 && (resultant.X + resultant.Y) <= 1)
                        {
                            currPoint = intersection;
                            currNorm = tri.normal;
                            tFirst = t;
                        }
                            
                    }
                }
            }

            return currPoint != null ? (RaycastResult?)new RaycastResult((Vector3)currPoint, (Vector3)currNorm, tFirst) : null;
        }

        public NitroFile m_File;
        public List<ColFace> m_Planes;

        uint m_PointsSectionOffset;
        uint m_NormalsSectionOffset;
        uint m_PlanesSectionOffset;
        uint m_OctreeSectionOffset;
    }

}
