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
    }
}
