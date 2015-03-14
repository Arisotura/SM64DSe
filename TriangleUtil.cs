using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace SM64DSe
{
    static class TriangleUtil
    {
        public static bool AxisTestX(ref Vector3 v1, ref Vector3 v2, float a, float b, float fa, float fb, ref Vector3 chalfsize)
        {
            float p1 = (a * v1.Y) - (b * v1.Z);
            float p2 = (a * v2.Y) - (b * v2.Z);
            float min, max;
            if (p1 < p2) { min = p1; max = p2; } else { min = p2; max = p1; }
            float rad = (fa * chalfsize.Y) + (fb * chalfsize.Z);
            return (min < rad) && (max > -rad);
        }

        public static bool AxisTestY(ref Vector3 v1, ref Vector3 v2, float a, float b, float fa, float fb, ref Vector3 chalfsize)
        {
            float p1 = (-a * v1.X) + (b * v1.Z);
            float p2 = (-a * v2.X) + (b * v2.Z);
            float min, max;
            if (p1 < p2) { min = p1; max = p2; } else { min = p2; max = p1; }
            float rad = (fa * chalfsize.X) + (fb * chalfsize.Z);
            return (min < rad) && (max > -rad);
        }

        public static bool AxisTestZ(ref Vector3 v1, ref Vector3 v2, float a, float b, float fa, float fb, ref Vector3 chalfsize)
        {
            float p1 = (a * v1.X) - (b * v1.Y);
            float p2 = (a * v2.X) - (b * v2.Y);
            float min, max;
            if (p1 < p2) { min = p1; max = p2; } else { min = p2; max = p1; }
            float rad = (fa * chalfsize.X) + (fb * chalfsize.Y);
            return (min < rad) && (max > -rad);
        }

        public static bool PlaneBoxOverlap(ref Vector3 normal, ref Vector3 vert, ref Vector3 maxbox)
        {
            Vector3 vmin, vmax;
            float v;

            v = vert.X;
            if (normal.X > 0f)
            {
                vmin.X = -maxbox.X - v;
                vmax.X = maxbox.X - v;
            }
            else
            {
                vmin.X = maxbox.X - v;
                vmax.X = -maxbox.X - v;
            }

            v = vert.Y;
            if (normal.Y > 0f)
            {
                vmin.Y = -maxbox.Y - v;
                vmax.Y = maxbox.Y - v;
            }
            else
            {
                vmin.Y = maxbox.Y - v;
                vmax.Y = -maxbox.Y - v;
            }

            v = vert.Z;
            if (normal.Z > 0f)
            {
                vmin.Z = -maxbox.Z - v;
                vmax.Z = maxbox.Z - v;
            }
            else
            {
                vmin.Z = maxbox.Z - v;
                vmax.Z = -maxbox.Z - v;
            }

            float dot0; 
            Vector3.Dot(ref normal, ref vmin, out dot0); if (dot0 > 0f) return false;
            Vector3.Dot(ref normal, ref vmax, out dot0); if (dot0 >= 0f) return true;

            return false;
        }

        // test whether a triangle is intersecting with the given axis-aligned box
        // based off http://fileadmin.cs.lth.se/cs/Personal/Tomas_Akenine-Moller/code/tribox3.txt
        public static bool TriangleIntersectsCube(ref Vector3 t0, ref Vector3 t1, ref Vector3 t2, ref Vector3 cstart, ref Vector3 csize)
        {
            Vector3 chalfsize; Vector3.Divide(ref csize, 2f, out chalfsize);
            Vector3 ccenter; Vector3.Add(ref cstart, ref chalfsize, out ccenter);
            Vector3 v0; Vector3.Subtract(ref t0, ref ccenter, out v0);
            Vector3 v1; Vector3.Subtract(ref t1, ref ccenter, out v1);
            Vector3 v2; Vector3.Subtract(ref t2, ref ccenter, out v2);
            Vector3 e0; Vector3.Subtract(ref v1, ref v0, out e0);
            Vector3 e1; Vector3.Subtract(ref v2, ref v1, out e1);
            Vector3 e2; Vector3.Subtract(ref v0, ref v2, out e2);

            float fex = (float)Math.Abs(e0.X);
            float fey = (float)Math.Abs(e0.Y);
            float fez = (float)Math.Abs(e0.Z);
            if (!AxisTestX(ref v0, ref v2, e0.Z, e0.Y, fez, fey, ref chalfsize)) return false;
            if (!AxisTestY(ref v0, ref v2, e0.Z, e0.X, fez, fex, ref chalfsize)) return false;
            if (!AxisTestZ(ref v1, ref v2, e0.Y, e0.X, fey, fex, ref chalfsize)) return false;

            fex = (float)Math.Abs(e1.X);
            fey = (float)Math.Abs(e1.Y);
            fez = (float)Math.Abs(e1.Z);
            if (!AxisTestX(ref v0, ref v2, e1.Z, e1.Y, fez, fey, ref chalfsize)) return false;
            if (!AxisTestY(ref v0, ref v2, e1.Z, e1.X, fez, fex, ref chalfsize)) return false;
            if (!AxisTestZ(ref v0, ref v1, e1.Y, e1.X, fey, fex, ref chalfsize)) return false;

            fex = (float)Math.Abs(e2.X);
            fey = (float)Math.Abs(e2.Y);
            fez = (float)Math.Abs(e2.Z);
            if (!AxisTestX(ref v0, ref v1, e2.Z, e2.Y, fez, fey, ref chalfsize)) return false;
            if (!AxisTestY(ref v0, ref v1, e2.Z, e2.X, fez, fex, ref chalfsize)) return false;
            if (!AxisTestZ(ref v1, ref v2, e2.Y, e2.X, fey, fex, ref chalfsize)) return false;

            float min = Math.Min(v0.X, Math.Min(v1.X, v2.X));
            float max = Math.Max(v0.X, Math.Max(v1.X, v2.X));
            if (min > chalfsize.X || max < -chalfsize.X) return false;

            min = Math.Min(v0.Y, Math.Min(v1.Y, v2.Y));
            max = Math.Max(v0.Y, Math.Max(v1.Y, v2.Y));
            if (min > chalfsize.Y || max < -chalfsize.Y) return false;

            min = Math.Min(v0.Z, Math.Min(v1.Z, v2.Z));
            max = Math.Max(v0.Z, Math.Max(v1.Z, v2.Z));
            if (min > chalfsize.Z || max < -chalfsize.Z) return false;

            Vector3 normal; Vector3.Cross(ref e0, ref e1, out normal);
            if (!PlaneBoxOverlap(ref normal, ref v0, ref chalfsize)) return false;

            return true;
        }
    }
}
