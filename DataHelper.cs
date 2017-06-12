using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM64DSe
{
    public class DataHelper
    {
        public static byte Read8(byte[] data, uint addr) { return data[addr]; }
        public static ushort Read16(byte[] data, uint addr) { return (ushort)(data[addr] | (data[addr + 1] << 8)); }
        public static uint Read32(byte[] data, uint addr) { return (uint)(data[addr] | (data[addr + 1] << 8) | (data[addr + 2] << 16) | (data[addr + 3] << 24)); }
        public static ulong Read64(byte[] data, uint addr) { return (ulong)(data[addr] | (data[addr + 1] << 8) | (data[addr + 2] << 16) | (data[addr + 3] << 24) | (data[addr + 4] << 32) | (data[addr + 5] << 40) | (data[addr + 6] << 48) | (data[addr + 7] << 56)); }

        // reads a string until the specified length or until a null byte
        // if length is zero, no length limit is applied
        public static string ReadString(byte[] data, uint addr, int len)
        {
            string result = "";

            for (int i = 0; ; i++)
            {
                if ((len > 0) && (i >= len)) break;

                char ch = (char)data[addr + i];
                if (ch == 0) break;

                result += ch;
            }

            return result;
        }

        public static void Write8(byte[] data, uint addr, byte value) { data[addr] = value; }
        public static void Write16(byte[] data, uint addr, ushort value) { data[addr] = (byte)(value & 0xFF); data[addr + 1] = (byte)(value >> 8); }
        public static void Write32(byte[] data, uint addr, uint value) { data[addr] = (byte)(value & 0xFF); data[addr + 1] = (byte)((value >> 8) & 0xFF); data[addr + 2] = (byte)((value >> 16) & 0xFF); data[addr + 3] = (byte)(value >> 24); }

        public static void WriteString(byte[] data, uint addr, string str, int len)
        {
            int i = 0;
            for (; ; i++)
            {
                if ((len > 0) && (i >= len)) break;
                if (i >= str.Length) break;

                data[addr + i] = (byte)str[i];
            }

            if ((len == 0) || (i < len))
                data[addr + i] = 0;
        }

        public static byte[] GetBytes8(byte value) { return new byte[1] { value }; }
        public static byte[] GetBytes16(ushort value) { return new byte[2] { (byte)(value & 0xFF), (byte)(value >> 8) }; }
        public static byte[] GetBytes32(uint value) { return new byte[4] { (byte)(value & 0xFF), (byte)((value >> 8) & 0xFF), (byte)((value >> 16) & 0xFF), (byte)(value >> 24) }; }
        public static byte[] GetBytes64(ulong value) { return new byte[8] { (byte)(value & 0xFF), (byte)((value >> 8) & 0xFF), (byte)((value >> 16) & 0xFF), (byte)((value >> 24) & 0xFF), (byte)((value >> 32) & 0xFF), (byte)((value >> 40) & 0xFF), (byte)((value >> 48) & 0xFF), (byte)((value >> 56) & 0xFF) }; }
    }
}
