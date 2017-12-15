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
using System.Drawing;
using OpenTK;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace SM64DSe
{
    static class Helper
    {
        public static readonly CultureInfo USA = new CultureInfo("en-US");
        public static readonly MD5CryptoServiceProvider m_MD5 = new MD5CryptoServiceProvider();
        public static readonly Color LIGHT_RED = Color.FromArgb(230, 100, 100);
        
        private static readonly DateTime FirstOfJanuary1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static ushort ColorToBGR15(Color color)
        {
            uint r = (uint)((color.R & 0xF8) >> 3);
            uint g = (uint)((color.G & 0xF8) << 2);
            uint b = (uint)((color.B & 0xF8) << 7);
            return (ushort)(r | g | b);
        }

        public static Color BGR15ToColor(ushort bgr15)
        {
            byte red = (byte)((bgr15 << 3) & 0xF8);
            byte green = (byte)((bgr15 >> 2) & 0xF8);
            byte blue = (byte)((bgr15 >> 7) & 0xF8);

            // compensate the lower 3 bits (so that for example 7FFF -> (FF,FF,FF) instead of (F8,F8,F8))
            red |= (byte)(red >> 5);
            green |= (byte)(green >> 5);
            blue |= (byte)(blue >> 5);

            return Color.FromArgb(red, green, blue);
        }

        public static uint BytesToUInt32(byte[] values, int index)
        {
            return (uint)(values[index] | (values[index + 1] << 8) | (values[index + 2] << 16) | (values[index + 3] << 24));
        }

        public static ushort BytesToUShort16(byte[] values, int index)
        {
            return (ushort)(values[index] | (values[index + 1] << 8));
        }

        public static ushort BytesToUShort16(byte[] values, int index, ushort defaultValue)
        {
            return (index < values.Length) ? BytesToUShort16(values, index) : defaultValue;
        }

        public static ushort BlendColorsBGR15(ushort c1, int w1, ushort c2, int w2)
        {
            int r1 = c1 & 0x1F;
            int g1 = (c1 >> 5) & 0x1F;
            int b1 = (c1 >> 10) & 0x1F;
            int r2 = c2 & 0x1F;
            int g2 = (c2 >> 5) & 0x1F;
            int b2 = (c2 >> 10) & 0x1F;

            int rf = ((r1 * w1) + (r2 * w2)) / (w1 + w2);
            int gf = ((g1 * w1) + (g2 * w2)) / (w1 + w2);
            int bf = ((b1 * w1) + (b2 * w2)) / (w1 + w2);
            return (ushort)(rf | (gf << 5) | (bf << 10));
        }

        public static string GetHexColourString(Color colour)
        {
            return String.Format("#{0:X2}{1:X2}{2:X2}", colour.R, colour.G, colour.B);
        }

        public static bool VectorsEqual(Vector3 a, Vector3 b)
        {
            float epsilon = 0.00001f;
            if (Math.Abs(a.X - b.X) > epsilon) return false;
            if (Math.Abs(a.Y - b.Y) > epsilon) return false;
            if (Math.Abs(a.Z - b.Z) > epsilon) return false;
            return true;
        }

        public static void RoundVector(ref Vector3 v, float fpcoef)
        {
            v.X = (float)Math.Round((double)(v.X * fpcoef)) / fpcoef;
            v.Y = (float)Math.Round((double)(v.Y * fpcoef)) / fpcoef;
            v.Z = (float)Math.Round((double)(v.Z * fpcoef)) / fpcoef;
        }

        public static bool PointOnLine(Vector3 p, Vector3 a, Vector3 b)
        {
            Vector3 line; Vector3.Subtract(ref b, ref a, out line);
            Vector3 d; Vector3.Subtract(ref p, ref a, out d);
            Vector3 ratio; Vector3.Divide(ref d, ref line, out ratio);

            float epsilon = 0.00001f;
            if (Math.Abs(ratio.X - ratio.Y) > epsilon || Math.Abs(ratio.X - ratio.Z) > epsilon)
                return false;

            if (ratio.X < 0f || ratio.X > 1f)
                return false;

            return true;
        }

        public static int GetDictionaryStringKeyIndex(List<Object> keys, Object key)
        {
            int index = -1;
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys.ElementAt(i).Equals(key))
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        public static int snapControlHorizontally(Control control, int snapPos)
        {
            int newX = snapPos;
            int newY = control.Location.Y;
            control.Location = new Point(newX, newY);
            return snapPos + control.Width;
        }

        public static int snapControlVertically(Control control, int snapPos, int padding = 0)
        {
            int newX = control.Location.X;
            int newY = snapPos+padding;
            control.Location = new Point(newX, newY);
            return snapPos + control.Height;
        }

        public static Matrix4 SRTToMatrix(Vector3 scale, Vector3 rot, Vector3 trans)
        {
            Matrix4 ret = Matrix4.Identity;

            Matrix4 mscale = Matrix4.CreateScale(scale);
            Matrix4 mxrot = Matrix4.CreateRotationX(rot.X);
            Matrix4 myrot = Matrix4.CreateRotationY(rot.Y);
            Matrix4 mzrot = Matrix4.CreateRotationZ(rot.Z);
            Matrix4 mtrans = Matrix4.CreateTranslation(trans);

            Matrix4.Mult(ref ret, ref mscale, out ret);
            Matrix4.Mult(ref ret, ref mxrot, out ret);
            Matrix4.Mult(ref ret, ref myrot, out ret);
            Matrix4.Mult(ref ret, ref mzrot, out ret);
            Matrix4.Mult(ref ret, ref mtrans, out ret);

            return ret;
        }

        public static Matrix4 InverseSRTToMatrix(Vector3 scale, Vector3 rot, Vector3 trans)
        {
            Matrix4 ret = Matrix4.Identity;

            Matrix4 mscale = Matrix4.CreateScale(scale);
            Matrix4 mxrot = Matrix4.CreateRotationX(rot.X);
            Matrix4 myrot = Matrix4.CreateRotationY(rot.Y);
            Matrix4 mzrot = Matrix4.CreateRotationZ(rot.Z);
            Matrix4 mtrans = Matrix4.CreateTranslation(trans);

            Matrix4.Mult(ref ret, ref mtrans, out ret);
            Matrix4.Mult(ref ret, ref mzrot, out ret);
            Matrix4.Mult(ref ret, ref myrot, out ret);
            Matrix4.Mult(ref ret, ref mxrot, out ret);
            Matrix4.Mult(ref ret, ref mscale, out ret);

            return ret;
        }

        public static void DecomposeSRTMatrix2(Matrix4 matrix, out Vector3 scale, out Vector3 rotation, out Vector3 translation)
        {
            Quaternion quat;
            Decompose(matrix, out scale, out quat, out translation);
            matrix.Row0 = new Vector4(Vector3.Divide(matrix.Row0.Xyz, scale.X), 0);
            matrix.Row1 = new Vector4(Vector3.Divide(matrix.Row1.Xyz, scale.Y), 0);
            matrix.Row2 = new Vector4(Vector3.Divide(matrix.Row2.Xyz, scale.Z), 0);
            matrix.Row3 = new Vector4(0, 0, 0, 1);
            rotation = FromRotMatToEulerZYXInt(matrix);
        }

        /*
         * Below two methods taken from SharpDX library. See License.SharpDX.txt.
         */

        /// <summary>
        /// Decomposes a matrix into a scale, rotation, and translation.
        /// </summary>
        /// <param name="scale">When the method completes, contains the scaling component of the decomposed matrix.</param>
        /// <param name="rotation">When the method completes, contains the rotation component of the decomposed matrix.</param>
        /// <param name="translation">When the method completes, contains the translation component of the decomposed matrix.</param>
        /// <remarks>
        /// This method is designed to decompose an SRT transformation matrix only.
        /// </remarks>
        public static bool Decompose(Matrix4 matrix, out Vector3 scale, out Quaternion rotation, out Vector3 translation)
        {
            //Source: Unknown
            //References: http://www.gamedev.net/community/forums/topic.asp?topic_id=441695

            //Get the translation.
            translation.X = matrix.M41;
            translation.Y = matrix.M42;
            translation.Z = matrix.M43;

            //Scaling is the length of the rows.
            scale.X = (float)Math.Sqrt((matrix.M11 * matrix.M11) + (matrix.M12 * matrix.M12) + (matrix.M13 * matrix.M13));
            scale.Y = (float)Math.Sqrt((matrix.M21 * matrix.M21) + (matrix.M22 * matrix.M22) + (matrix.M23 * matrix.M23));
            scale.Z = (float)Math.Sqrt((matrix.M31 * matrix.M31) + (matrix.M32 * matrix.M32) + (matrix.M33 * matrix.M33));

            //If any of the scaling factors are zero, than the rotation matrix can not exist.
            if (scale.X == 0.0f ||
                scale.Y == 0.0f ||
                scale.Z == 0.0f)
            {
                rotation = Quaternion.Identity;
                return false;
            }

            //The rotation is the left over matrix after dividing out the scaling.
            Matrix4 rotationmatrix = new Matrix4();
            rotationmatrix.M11 = matrix.M11 / scale.X;
            rotationmatrix.M12 = matrix.M12 / scale.X;
            rotationmatrix.M13 = matrix.M13 / scale.X;

            rotationmatrix.M21 = matrix.M21 / scale.Y;
            rotationmatrix.M22 = matrix.M22 / scale.Y;
            rotationmatrix.M23 = matrix.M23 / scale.Y;

            rotationmatrix.M31 = matrix.M31 / scale.Z;
            rotationmatrix.M32 = matrix.M32 / scale.Z;
            rotationmatrix.M33 = matrix.M33 / scale.Z;

            rotationmatrix.M44 = 1f;

            RotationMatrix(ref rotationmatrix, out rotation);
            return true;
        }

        /// <summary>
        /// Creates a quaternion given a rotation matrix.
        /// </summary>
        /// <param name="matrix">The rotation matrix.</param>
        /// <param name="result">When the method completes, contains the newly created quaternion.</param>
        public static void RotationMatrix(ref Matrix4 matrix, out Quaternion result)
        {
            float sqrt;
            float half;
            float scale = matrix.M11 + matrix.M22 + matrix.M33;
            result = new Quaternion();

            if (scale > 0.0f)
            {
                sqrt = (float)Math.Sqrt(scale + 1.0f);
                result.W = sqrt * 0.5f;
                sqrt = 0.5f / sqrt;

                result.X = (matrix.M23 - matrix.M32) * sqrt;
                result.Y = (matrix.M31 - matrix.M13) * sqrt;
                result.Z = (matrix.M12 - matrix.M21) * sqrt;
            }
            else if ((matrix.M11 >= matrix.M22) && (matrix.M11 >= matrix.M33))
            {
                sqrt = (float)Math.Sqrt(1.0f + matrix.M11 - matrix.M22 - matrix.M33);
                half = 0.5f / sqrt;

                result.X = 0.5f * sqrt;
                result.Y = (matrix.M12 + matrix.M21) * half;
                result.Z = (matrix.M13 + matrix.M31) * half;
                result.W = (matrix.M23 - matrix.M32) * half;
            }
            else if (matrix.M22 > matrix.M33)
            {
                sqrt = (float)Math.Sqrt(1.0f + matrix.M22 - matrix.M11 - matrix.M33);
                half = 0.5f / sqrt;

                result.X = (matrix.M21 + matrix.M12) * half;
                result.Y = 0.5f * sqrt;
                result.Z = (matrix.M32 + matrix.M23) * half;
                result.W = (matrix.M31 - matrix.M13) * half;
            }
            else
            {
                sqrt = (float)Math.Sqrt(1.0f + matrix.M33 - matrix.M11 - matrix.M22);
                half = 0.5f / sqrt;

                result.X = (matrix.M31 + matrix.M13) * half;
                result.Y = (matrix.M32 + matrix.M23) * half;
                result.Z = 0.5f * sqrt;
                result.W = (matrix.M12 - matrix.M21) * half;
            }
        }

        public const float Tau = 2.0f * (float)Math.PI;
        public const float Deg2Rad = (float)(Tau / 360.0f);
        public const float Rad2Deg = (float)(360.0f / Tau);

        public static Vector3 FromRotMatToEulerZYXInt(Matrix4 mat)
        {
            //x''', y''', z''' are stored in rows of mat
            Vector3 angles = new Vector3(0, 0, 0);

            angles.Y = (float)-Math.Asin(mat.Row0.Z);
            if(Math.Abs(angles.Y) * 0x10000 / Tau > (float)0x4000 - 0.5)
            {
                angles.Z = 0;
                angles.X = (float)Math.Atan2(-mat.Row2.Y, mat.Row1.Y);
            }
            else
            {
                angles.Z = (float)Math.Atan2(mat.Row0.Y, mat.Row0.X);
                angles.X = (float)Math.Atan2(mat.Row1.Z, mat.Row2.Z);
            }

            //Whew!
            return angles;
        }

        static Vector3 NormalizeAnglesDeg(Vector3 angles)
        {
            angles.X = NormalizeAngleDeg(angles.X);
            angles.Y = NormalizeAngleDeg(angles.Y);
            angles.Z = NormalizeAngleDeg(angles.Z);
            return angles;
        }

        static float NormalizeAngleDeg(float angle)
        {
            while (angle > 360f)
                angle -= 360f;
            while (angle < 0f)
                angle += 360f;
            return angle;
        }

        static Vector3 NormalizeAnglesRad(Vector3 angles)
        {
            angles.X = NormalizeAngleRad(angles.X);
            angles.Y = NormalizeAngleRad(angles.Y);
            angles.Z = NormalizeAngleRad(angles.Z);
            return angles;
        }

        static float NormalizeAngleRad(float angle)
        {
            while (angle > Math.PI)
                angle -= (float)Math.PI;
            while (angle < 0f)
                angle += (float)Math.PI;
            return angle;
        }

        public static Matrix4 StringArrayToMatrix4(string[] vals)
        {
            float[] vals_float = new float[vals.Length];
            for (int i = 0; i < vals.Length; i++)
            {
                vals_float[i] = float.Parse(vals[i], USA);
            }
            Matrix4 matrix = FloatArrayToMatrix4(vals_float);
            return matrix;
        }

        public static Matrix4 FloatArrayToMatrix4(float[] vals)
        {
            Vector4 row0 = new Vector4(vals[0], vals[4],
                vals[8], vals[12]);
            Vector4 row1 = new Vector4(vals[1], vals[5],
                vals[9], vals[13]);
            Vector4 row2 = new Vector4(vals[2], vals[6],
                vals[10], vals[14]);
            Vector4 row3 = new Vector4(vals[3], vals[7],
                vals[11], vals[15]);
            Matrix4 matrix = new Matrix4(row0, row1, row2, row3);
            return matrix;
        }

        public static Matrix4 DoubleArrayToMatrix4(double[] vals)
        {
            return FloatArrayToMatrix4(Array.ConvertAll<double, float>(vals, Convert.ToSingle));
        }

        public static uint GetActSelectorIDTableAddress()
        {
            switch (Program.m_ROM.m_Version)
            {
                default:
                case NitroROM.Version.EUR:
                    return 0x75298;
                case NitroROM.Version.USA_v1:
                    return 0x731F0;
                case NitroROM.Version.USA_v2:
                    return 0x73F10;
                case NitroROM.Version.JAP:
                    return 0x73744;
            }
        }

        public static void DecompressOverlaysWithinGame()
        {
            if (CheckAllOverlaysDecompressed())
                return;

            for (int i = 0; i < 155; i++)
            {
                NitroOverlay overlay = new NitroOverlay(Program.m_ROM, (uint)i);
                // Overlay is decompressed when initialised above automatically if needed, just need to save changes
                overlay.SaveChanges();
            }
        }

        public static bool CheckAllOverlaysDecompressed()
        {
            bool allDecompressed = true;
            for (int i = 0; i < 155; i++)
            {
                if (CheckOverlayCompressed((uint)i) == true)
                {
                    allDecompressed = false;
                    break;
                }
            }
            return allDecompressed;
        }

        public static bool CheckOverlayCompressed(uint id)
        {
            uint OVTEntryAddr = Program.m_ROM.GetOverlayEntryOffset(id);
            Byte flags = Program.m_ROM.Read8(OVTEntryAddr + 0x1F);

            if ((flags & 0x01) == 0x01)
                return true;
            else
                return false;
        }

        /*
         * Converts a Hex Dump to binary file
         */
        public static byte[] HexDumpToBinary(string hexdump)
        {
            string[] lines = hexdump.Split('\n');
            string[][] hexBytes = new string[lines.Length][];
            // For now just assume <ADDRESS> <hex data 1> ... <hex data 8>
            int startIndex = (lines[0].Split(' ').Length >= 17) ? 1 : 0;
            int endIndex = (lines[0].Split(' ').Length >= 17) ? 16 : 15;
            int numBytes = 0;

            // Get string array of each individual byte in hex format
            for (int i = 0; i < lines.Length; i++)
            {
                string[] split = lines[i].Replace('-', ' ').Split(' ');// Some files have "-" separator in middle
                if (split.Length == 1)
                    break;// End of file
                string[] hexData = new string[(endIndex + 1) - startIndex];
                for (int j = startIndex, k = 0; j <= endIndex; j++, k++)
                {
                    hexData[k] = split[j];
                }
                hexBytes[i] = hexData;
                numBytes += hexData.Length;
            }

            // Convert string hex representation of each byte to actual byte
            byte[] binaryData = new byte[numBytes];
            int count = 0;
            for (int i = 0; i < hexBytes.Length; i++)
            {
                if (hexBytes[i] == null)
                    break;
                for (int j = 0; j < hexBytes[i].Length; j++)
                {
                    binaryData[count] = byte.Parse(hexBytes[i][j], System.Globalization.NumberStyles.HexNumber);
                    count++;
                }
            }

            return binaryData;
        }

        public static string HexString(byte[] crap)
        {
            string ret = "";
            foreach (byte b in crap)
                ret += b.ToString("X2");
            return ret;
        }

        public static void AlignWriter(BinaryWriter binWriter, uint multiple)
        {
            if ((binWriter.BaseStream.Position & 3) != 0x00)
            {
                binWriter.Write(new byte[(multiple - binWriter.BaseStream.Position % multiple) % multiple]);
            }
        }

        public static void WritePosAndRestore(BinaryWriter binWriter, uint address, uint adder)
        {
            uint oldPos = (uint)binWriter.BaseStream.Position;
            binWriter.BaseStream.Position = address;
            binWriter.Write(oldPos + adder);
            binWriter.BaseStream.Position = oldPos;
        }
        
        public static IEnumerable<T> ArrayOf<T>(T value, int size)
        {
            for (int i = 0; i < size; ++i)
                yield return value;
        }

        public static int FindSubList<T>(List<T> list, List<T> subList)
        {
            for (int i = 0; i <= list.Count - subList.Count; ++i)
            {
                if (list.GetRange(i, subList.Count).SequenceEqual(subList))
                {
                    return i;
                }
            }
            return -1;
        }

        public static int FindLastSubList<T>(List<T> list, List<T> subList)
        {
            int index = -1;
            for (int i = 0; i <= list.Count - subList.Count; ++i)
                if (list.GetRange(i, subList.Count).SequenceEqual(subList))
                    index = i;
            return index;
        }

        public static void ResizeList<T>(List<T> list, int size, T filler = default(T))
        {
            if (size < list.Count)
                list.RemoveRange(size, list.Count - size);
            else if (size > list.Count)
                list.AddRange(ArrayOf(filler, size - list.Count));
        }

        public static List<int> FloatListTo20_12IntList(List<float> floatList)
        {
            return floatList.ConvertAll(z => (int)Math.Floor(z * 4096.0f + 0.5f)).ToList();
        }

        public static List<int> FloatListToRotationIntList(List<float> floatList)
        {
            return floatList.ConvertAll(z => (int)Math.Floor(z / 360.0f * 4096.0f + 0.5f)).ToList();
        }

        public static bool TryParseFloat(TextBox textBox, out float result)
        {
            if (!TryParseFloat(textBox.Text, out result))
            {
                textBox.BackColor = LIGHT_RED;
                return false;
            }
            else
            {
                textBox.BackColor = Color.White;
                return true;
            }
        }

        public static bool TryParseFloat(string value, out float result)
        {
            return (float.TryParse(value, NumberStyles.Float, USA, out result) || float.TryParse(value, out result));
        }

        public static float ParseFloat(string value)
        {
            return float.Parse(value, USA);
        }

        public static string ToString(float value)
        {
            return value.ToString(USA);
        }

        public static string ToString(float value, byte nDecimalPlaces)
        {
            return value.ToString("N" + nDecimalPlaces, USA);
        }

        public static string ToString4DP(float value)
        {
            return value.ToString("N4", USA);
        }

        public static string ToString(Vector3 value)
        {
            if (value == null) return "null";
            return '(' + ToString(value.X) + ", " + ToString(value.Y) + ", " + ToString(value.Z) + ')';
        }

        public static bool TryParseInt(TextBox textBox, ref int result)
        {
            if (!TryParseInt(textBox.Text, ref result))
            {
                textBox.BackColor = LIGHT_RED;
                return false;
            }
            else
            {
                textBox.BackColor = Color.White;
                return true;
            }
        }

        private static bool TryParseInt(string value, ref int result)
        {
            return (int.TryParse(value, out result));
        }

        public static long CurrentTimeMillis()
        {
            return (long)((DateTime.UtcNow - FirstOfJanuary1970).TotalMilliseconds);
        }

        public static bool IsOpenGLAvailable()
        {
            return (OpenTK.Graphics.GraphicsContext.CurrentContext != null);
        }
    }

    // Taken from http://stackoverflow.com/questions/1440392/use-byte-as-key-in-dictionary
    // By user: JaredPar

    public class ByteArrayComparer : IEqualityComparer<byte[]>
    {
        public bool Equals(byte[] left, byte[] right)
        {
            if (left == null || right == null)
            {
                return left == right;
            }
            return left.SequenceEqual(right);
        }
        public int GetHashCode(byte[] key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            int sum = 0;
            foreach (byte cur in key)
            {
                sum = 33 * sum + cur;
            }
            return sum;
        }
    }
}
