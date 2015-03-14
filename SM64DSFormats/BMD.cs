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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SM64DSe.SM64DSFormats;

namespace SM64DSe
{
    public class BMD
    {
        public Texture ReadTexture(uint texid, uint palid)
        {
            uint texentry = m_TexChunksOffset + (texid * 20);
            uint palentry = (palid == 0xFFFFFFFF) ? 0xFFFFFFFF : (m_PalChunksOffset + (palid * 16));

            string texname = m_File.ReadString(m_File.Read32(texentry), 0);
            string palname = (palentry == 0xFFFFFFFF) ? "<NO PALETTE>" : m_File.ReadString(m_File.Read32(palentry), 0);

            string texkey = texname + "|" + palname;
            if (m_Textures.ContainsKey(texkey))
                return m_Textures[texkey];

            Texture ret = new Texture();

            ret.m_TexID = texid;
            ret.m_TexName = texname;
            ret.m_PalID = palid;
            ret.m_PalName = palname;
            ret.m_EntryOffset = texentry;
            ret.m_PalEntryOffset = palentry;

            uint texdataoffset = m_File.Read32(texentry + 0x04);
            uint texdatasize = m_File.Read32(texentry + 0x08);
            uint texparam = m_File.Read32(texentry + 0x10);
            ret.m_TexDataSize = texdatasize;
            ret.m_Params = texparam;

            uint paldataoffset = 0xFFFFFFFF;
            uint paldatasize = 0;
            if (palentry != 0xFFFFFFFF)
            {
                paldataoffset = m_File.Read32(palentry + 0x04);
                paldatasize = m_File.Read32(palentry + 0x08);
            }

            ret.m_PalOffset = paldataoffset;
            ret.m_PalSize = paldatasize;

            uint textype = (texparam >> 26) & 0x7;
            ret.m_TexType = textype;
            if (textype == 0)
                return null;

            ret.m_Width = (uint)(8 << (int)((texparam >> 20) & 0x7));
            ret.m_Height = (uint)(8 << (int)((texparam >> 23) & 0x7));

            if ((palentry == 0xFFFFFFFF) && (textype != 7))
                throw new Exception("BMD decoder: paletted texture with no associated palette entry; WTF");

            byte zero_alpha = 0xFF;
            if ((texparam & 0x20000000) == 0x20000000)
                zero_alpha = 0;

            // texture stored as 8bit ARGB
            ret.m_Data = new byte[ret.m_Width * ret.m_Height * 4];

            switch (textype)
            {
                case 1: // a3i5
                    {
                        for (uint _in = 0, _out = 0; _in < texdatasize; _in++, _out += 4)
                        {
                            byte texel = m_File.Read8(texdataoffset + _in);
                            ushort color = m_File.Read16((uint)(paldataoffset + ((texel & 0x1F) << 1)));

                            byte red = (byte)((color & 0x001F) << 3);
                            byte green = (byte)((color & 0x03E0) >> 2);
                            byte blue = (byte)((color & 0x7C00) >> 7);
                            byte _alpha = (byte)(((texel & 0xE0) >> 3) + ((texel & 0xE0) >> 6));
                            byte alpha = (byte)((_alpha << 3) | (_alpha >> 2));

                            ret.m_Data[_out] = blue;
                            ret.m_Data[_out + 1] = green;
                            ret.m_Data[_out + 2] = red;
                            ret.m_Data[_out + 3] = alpha;
                        }
                    }
                    break;

                case 2: // 4-color
                    {
                        for (int _in = 0, _out = 0; _in < texdatasize; _in++, _out += 16)
                        {
                            byte texels = m_File.Read8((uint)(texdataoffset + _in));

                            ushort color = m_File.Read16((uint)(paldataoffset + ((texels << 1) & 0x6)));
                            byte red = (byte)((color & 0x001F) << 3);
                            byte green = (byte)((color & 0x03E0) >> 2);
                            byte blue = (byte)((color & 0x7C00) >> 7);

                            ret.m_Data[_out] = blue;
                            ret.m_Data[_out + 1] = green;
                            ret.m_Data[_out + 2] = red;
                            ret.m_Data[_out + 3] = (byte)(((texels & 0x03) != 0) ? (byte) 0xFF : zero_alpha);

                            color = m_File.Read16((uint)(paldataoffset + ((texels >> 1) & 0x6)));
                            red = (byte)((color & 0x001F) << 3);
                            green = (byte)((color & 0x03E0) >> 2);
                            blue = (byte)((color & 0x7C00) >> 7);

                            ret.m_Data[_out + 4] = blue;
                            ret.m_Data[_out + 5] = green;
                            ret.m_Data[_out + 6] = red;
                            ret.m_Data[_out + 7] = (byte)(((texels & 0x0C) != 0) ? (byte) 0xFF : zero_alpha);

                            color = m_File.Read16((uint)(paldataoffset + ((texels >> 3) & 0x6)));
                            red = (byte)((color & 0x001F) << 3);
                            green = (byte)((color & 0x03E0) >> 2);
                            blue = (byte)((color & 0x7C00) >> 7);

                            ret.m_Data[_out + 8] = blue;
                            ret.m_Data[_out + 9] = green;
                            ret.m_Data[_out + 10] = red;
                            ret.m_Data[_out + 11] = (byte)(((texels & 0x30) != 0) ? (byte) 0xFF : zero_alpha);

                            color = m_File.Read16((uint)(paldataoffset + ((texels >> 5) & 0x6)));
                            red = (byte)((color & 0x001F) << 3);
                            green = (byte)((color & 0x03E0) >> 2);
                            blue = (byte)((color & 0x7C00) >> 7);

                            ret.m_Data[_out + 12] = blue;
                            ret.m_Data[_out + 13] = green;
                            ret.m_Data[_out + 14] = red;
                            ret.m_Data[_out + 15] = (byte)(((texels & 0xC0) != 0) ? (byte) 0xFF : zero_alpha);
                        }
                    }
                    break;

                case 3: // 16-color
                    {
                        for (int _in = 0, _out = 0; _in < texdatasize; _in++, _out += 8)
                        {
                            byte texels = m_File.Read8((uint)(texdataoffset + _in));

                            ushort color = m_File.Read16((uint)(paldataoffset + ((texels << 1) & 0x1E)));
                            byte red = (byte)((color & 0x001F) << 3);
                            byte green = (byte)((color & 0x03E0) >> 2);
                            byte blue = (byte)((color & 0x7C00) >> 7);

                            ret.m_Data[_out] = blue;
                            ret.m_Data[_out + 1] = green;
                            ret.m_Data[_out + 2] = red;
                            ret.m_Data[_out + 3] = (byte)(((texels & 0x0F) != 0) ? (byte) 0xFF : zero_alpha);

                            color = m_File.Read16((uint)(paldataoffset + ((texels >> 3) & 0x1E)));
                            red = (byte)((color & 0x001F) << 3);
                            green = (byte)((color & 0x03E0) >> 2);
                            blue = (byte)((color & 0x7C00) >> 7);

                            ret.m_Data[_out + 4] = blue;
                            ret.m_Data[_out + 5] = green;
                            ret.m_Data[_out + 6] = red;
                            ret.m_Data[_out + 7] = (byte)(((texels & 0xF0) != 0) ? (byte) 0xFF : zero_alpha);
                        }
                    }
                    break;

                case 4: // 256-color
                    {
                        for (int _in = 0, _out = 0; _in < texdatasize; _in++, _out += 4)
                        {
                            byte texel = m_File.Read8((uint)(texdataoffset + _in));

                            ushort color = m_File.Read16((uint)(paldataoffset + (texel << 1)));
                            byte red = (byte)((color & 0x001F) << 3);
                            byte green = (byte)((color & 0x03E0) >> 2);
                            byte blue = (byte)((color & 0x7C00) >> 7);

                            ret.m_Data[_out] = blue;
                            ret.m_Data[_out + 1] = green;
                            ret.m_Data[_out + 2] = red;
                            ret.m_Data[_out + 3] = (byte)((texel != 0) ? (byte) 0xFF : zero_alpha);
                        }
                    }
                    break;

                case 5: // motherfucking compressed
                    {   // as usual, shitty code but works
                        int yout = 0, xout = 0;

                        for (int _in = 0; _in < texdatasize; _in += 4)
                        {
                            uint blox = m_File.Read32((uint)(texdataoffset + _in));
                            ushort palidx_data = m_File.Read16((uint)(texdataoffset + texdatasize + (_in >> 1)));

                            for (int y = 0; y < 4; y++)
                            {
                                for (int x = 0; x < 4; x++)
                                {
                                    byte texel = (byte)(blox & 0x3);
                                    blox >>= 2;

                                    uint pal_offset = (uint)((palidx_data & 0x3FFF) << 2);
                                    ushort color_mode = (ushort)(palidx_data >> 14);
                                    uint color = 0xFFFFFFFF;

                                    switch (texel)
                                    {
                                        case 0: color = m_File.Read16(paldataoffset + pal_offset); break;
                                        case 1: color = m_File.Read16(paldataoffset + pal_offset + 2); break;
                                        case 2:
                                            {
                                                switch (color_mode)
                                                {
                                                    case 0:
                                                    case 2: color = m_File.Read16(paldataoffset + pal_offset + 4); break;
                                                    case 1:
                                                        {
                                                            ushort c0 = m_File.Read16(paldataoffset + pal_offset);
                                                            ushort c1 = m_File.Read16(paldataoffset + pal_offset + 2);
                                                            color = Helper.BlendColorsBGR15(c0, 1, c1, 1);
                                                        }
                                                        break;
                                                    case 3:
                                                        {
                                                            ushort c0 = m_File.Read16(paldataoffset + pal_offset);
                                                            ushort c1 = m_File.Read16(paldataoffset + pal_offset + 2);
                                                            color = Helper.BlendColorsBGR15(c0, 5, c1, 3);
                                                        }
                                                        break;
                                                }
                                            }
                                            break;
                                        case 3:
                                            {
                                                switch (color_mode)
                                                {
                                                    case 0:
                                                    case 1: color = 0xFFFFFFFF; break;
                                                    case 2: color = m_File.Read16(paldataoffset + pal_offset + 6); break;
                                                    case 3:
                                                        {
                                                            ushort c0 = m_File.Read16(paldataoffset + pal_offset);
                                                            ushort c1 = m_File.Read16(paldataoffset + pal_offset + 2);
                                                            color = Helper.BlendColorsBGR15(c0, 3, c1, 5);
                                                        }
                                                        break;
                                                }
                                            }
                                            break;
                                    }

                                    int _out = (int)(((yout * ret.m_Width) + xout) * 4);
                                    int yoff = (int)(y * ret.m_Width * 4);
                                    int xoff = (int)(x * 4);

                                    if (color == 0xFFFFFFFF)
                                    {
                                        ret.m_Data[_out + yoff + xoff] = 0;
                                        ret.m_Data[_out + yoff + xoff + 1] = 0;
                                        ret.m_Data[_out + yoff + xoff + 2] = 0;
                                        ret.m_Data[_out + yoff + xoff + 3] = 0;
                                    }
                                    else
                                    {
                                        byte red = (byte)((color & 0x001F) << 3);
                                        byte green = (byte)((color & 0x03E0) >> 2);
                                        byte blue = (byte)((color & 0x7C00) >> 7);

                                        ret.m_Data[_out + yoff + xoff] = blue;
                                        ret.m_Data[_out + yoff + xoff + 1] = green;
                                        ret.m_Data[_out + yoff + xoff + 2] = red;
                                        ret.m_Data[_out + yoff + xoff + 3] = 0xFF;
                                    }
                                }
                            }

                            xout += 4;
                            if (xout >= ret.m_Width)
                            {
                                xout = 0;
                                yout += 4;
                            }
                        }
                    }
                    break;

                case 6: // a5i3
                    {
                        for (int _in = 0, _out = 0; _in < texdatasize; _in++, _out += 4)
                        {
                            byte texel = m_File.Read8((uint)(texdataoffset + _in));
                            ushort color = m_File.Read16((uint)(paldataoffset + ((texel & 0x07) << 1)));

                            byte red = (byte)((color & 0x001F) << 3);
                            byte green = (byte)((color & 0x03E0) >> 2);
                            byte blue = (byte)((color & 0x7C00) >> 7);
                            byte alpha = (byte)((texel & 0xF8) | ((texel & 0xF8) >> 5));

                            ret.m_Data[_out] = blue;
                            ret.m_Data[_out + 1] = green;
                            ret.m_Data[_out + 2] = red;
                            ret.m_Data[_out + 3] = alpha;
                        }
                    }
                    break;

                case 7: // direct color
                    {
                        for (int _in = 0, _out = 0; _in < texdatasize; _in += 2, _out += 4)
                        {
                            ushort color = m_File.Read16((uint)(texdataoffset + _in));
                            byte red = (byte)((color & 0x001F) << 3);
                            byte green = (byte)((color & 0x03E0) >> 2);
                            byte blue = (byte)((color & 0x7C00) >> 7);

                            ret.m_Data[_out] = blue;
                            ret.m_Data[_out + 1] = green;
                            ret.m_Data[_out + 2] = red;
                            ret.m_Data[_out + 3] = (byte)(((color & 0x8000) != 0) ? (byte) 0xFF : zero_alpha);
                        }
                    }
                    break;
            }

            /*Bitmap lol = new Bitmap((int)ret.m_Width, (int)ret.m_Height);
            for (int y = 0; y < (int)ret.m_Height; y++)
                for (int x = 0; x < (int)ret.m_Width; x++)
                    lol.SetPixel(x, y, Color.FromArgb(ret.m_Data[((y * ret.m_Width) + x) * 4 + 3],
                        ret.m_Data[((y * ret.m_Width) + x) * 4 + 2],
                        ret.m_Data[((y * ret.m_Width) + x) * 4 + 1],
                        ret.m_Data[((y * ret.m_Width) + x) * 4]));
            lol.Save(ret.m_TexName + ".png", System.Drawing.Imaging.ImageFormat.Png);*/

            m_Textures[texname] = ret;
            return ret;
        }
        bool lolol = false;
        private void ProcessGXCommand(MaterialGroup matgroup, byte cmd, ref uint pos)
        {
            VertexList vtxlist = null;
            if (matgroup.m_Geometry.Count > 0)
                vtxlist = matgroup.m_Geometry[matgroup.m_Geometry.Count - 1];

            System.IO.StreamWriter sw = null;
            if (lolol)
                sw = new System.IO.StreamWriter(System.IO.File.Open("hurrdurr.txt", System.IO.FileMode.Append));

            // if (((cmd & 0xF0) == 0x10) && (cmd != 0x14))
            //    throw new Exception(String.Format("MATRIX COMMAND {0:X2}", cmd));
            
            switch (cmd)
            {
                // nop
                case 0x00: break;

                // matrix commands
                case 0x10: pos += 4; break;
                case 0x11: break;
                case 0x12: pos += 4; break;
                case 0x13: pos += 4; break;

                // matrix restore (used for animation)
                case 0x14:
                    {
                        uint param = m_File.Read32(pos); pos += 4;

                        m_CurVertex.m_MatrixID = param & 0x1F;
                    }
                    break;

                // more matrix commands
                case 0x15: break;
                case 0x16: pos += 64; break;
                case 0x17: pos += 48; break;
                case 0x18: pos += 64; break;
                case 0x19: pos += 48; break;
                case 0x1A: pos += 36; break;
                case 0x1B: pos += 12; break;
                case 0x1C: pos += 12; break;

                // set color
                case 0x20:
                    {
                        uint raw = m_File.Read32(pos); pos += 4;

                        byte red = (byte)((raw << 3) & 0xF8);
                        byte green = (byte)((raw >> 2) & 0xF8);
                        byte blue = (byte)((raw >> 7) & 0xF8);

                        byte alpha = (byte)((matgroup.m_PolyAttribs >> 13) & 0xF8);
                        alpha |= (byte)(alpha >> 5);

                        m_CurVertex.m_Color = Color.FromArgb(alpha, red, green, blue);
                        if (lolol) sw.Write(String.Format("COLOR {0}\n", m_CurVertex.m_Color));
                    }
                    break;

                // normal
                case 0x21:
                    {
                        uint param = m_File.Read32(pos); pos += 4;

                        short x = (short)((param << 6) & 0xFFC0);
                        short y = (short)((param >> 4) & 0xFFC0);
                        short z = (short)((param >> 14) & 0xFFC0);
                        m_CurVertex.m_Normal.X = (float)x / 512.0f;
                        m_CurVertex.m_Normal.Y = (float)y / 512.0f;
                        m_CurVertex.m_Normal.Z = (float)z / 512.0f;
                    }
                    break;

                // texcoord
                case 0x22:
                    {
                        uint param = m_File.Read32(pos); pos += 4;

                        short s = (short)(param & 0xFFFF);
                        short t = (short)(param >> 16);
                        m_CurVertex.m_TexCoord.X = (float)s / 16.0f;
                        m_CurVertex.m_TexCoord.Y = (float)t / 16.0f;

                        Vector2 texsize = new Vector2((float)matgroup.m_Texture.m_Width, (float)matgroup.m_Texture.m_Height);

                        if (lolol) sw.Write(String.Format("TEXCOORD {0} (TEXSIZE {1} SCALE {2} TRANS {3})\n",
                            m_CurVertex.m_TexCoord, texsize, matgroup.m_TexCoordScale, matgroup.m_TexCoordTrans));

                        Vector2.Add(ref m_CurVertex.m_TexCoord, ref matgroup.m_TexCoordTrans, out m_CurVertex.m_TexCoord);
                        Vector2.Multiply(ref m_CurVertex.m_TexCoord, ref matgroup.m_TexCoordScale, out m_CurVertex.m_TexCoord);
                        /* if ((matgroup.m_TexParams & 0xC0000000) != 0)
                         {
                             m_CurVertex.m_TexCoord.Y += matgroup.m_Texture.m_Height;
                         }*/

                        //Vector2 texsize = new Vector2((float)matgroup.m_Texture.m_Width, (float)matgroup.m_Texture.m_Height);
                        Vector2.Divide(ref m_CurVertex.m_TexCoord, ref texsize, out m_CurVertex.m_TexCoord);

                        // s = s*matrix[0] + t*matrix[4] + matrix[8]/16 + matrix[12]/16
                        // t = s*matrix[1] + t*matrix[5] + matrix[9]/16 + matrix[13]/16

                        // s = s*sscale + 1/16*(strans*sscale)
                        // t = t*tscale + 1/16*(ttrans*tscale)
                    }
                    break;

                // define vertex
                case 0x23:
                    {
                        uint param1 = m_File.Read32(pos); pos += 4;
                        uint param2 = m_File.Read32(pos); pos += 4;

                        short x = (short)(param1 & 0xFFFF);
                        short y = (short)(param1 >> 16);
                        short z = (short)(param2 & 0xFFFF);
                        m_CurVertex.m_Position.X = ((float)x / 4096.0f) * m_ScaleFactor;
                        m_CurVertex.m_Position.Y = ((float)y / 4096.0f) * m_ScaleFactor;
                        m_CurVertex.m_Position.Z = ((float)z / 4096.0f) * m_ScaleFactor;

                        vtxlist.m_VertexList.Add(m_CurVertex);
                        if (lolol) sw.Write(String.Format("VERTEX {0}\n", m_CurVertex.m_Position));
                    }
                    break;

                case 0x24:
                    {
                        uint param = m_File.Read32(pos); pos += 4;

                        short x = (short)((param << 6) & 0xFFC0);
                        short y = (short)((param >> 4) & 0xFFC0);
                        short z = (short)((param >> 14) & 0xFFC0);
                        m_CurVertex.m_Position.X = ((float)x / 4096.0f) * m_ScaleFactor;
                        m_CurVertex.m_Position.Y = ((float)y / 4096.0f) * m_ScaleFactor;
                        m_CurVertex.m_Position.Z = ((float)z / 4096.0f) * m_ScaleFactor;

                        vtxlist.m_VertexList.Add(m_CurVertex);
                        if (lolol) sw.Write(String.Format("VERTEX {0}\n", m_CurVertex.m_Position));
                    }
                    break;

                case 0x25:
                    {
                        uint param = m_File.Read32(pos); pos += 4;

                        short x = (short)(param & 0xFFFF);
                        short y = (short)(param >> 16);
                        m_CurVertex.m_Position.X = ((float)x / 4096.0f) * m_ScaleFactor;
                        m_CurVertex.m_Position.Y = ((float)y / 4096.0f) * m_ScaleFactor;

                        vtxlist.m_VertexList.Add(m_CurVertex);
                        if (lolol) sw.Write(String.Format("VERTEX {0}\n", m_CurVertex.m_Position));
                    }
                    break;

                case 0x26:
                    {
                        uint param = m_File.Read32(pos); pos += 4;

                        short x = (short)(param & 0xFFFF);
                        short z = (short)(param >> 16);
                        m_CurVertex.m_Position.X = ((float)x / 4096.0f) * m_ScaleFactor;
                        m_CurVertex.m_Position.Z = ((float)z / 4096.0f) * m_ScaleFactor;

                        vtxlist.m_VertexList.Add(m_CurVertex);
                        if (lolol) sw.Write(String.Format("VERTEX {0}\n", m_CurVertex.m_Position));
                    }
                    break;

                case 0x27:
                    {
                        uint param = m_File.Read32(pos); pos += 4;

                        short y = (short)(param & 0xFFFF);
                        short z = (short)(param >> 16);
                        m_CurVertex.m_Position.Y = ((float)y / 4096.0f) * m_ScaleFactor;
                        m_CurVertex.m_Position.Z = ((float)z / 4096.0f) * m_ScaleFactor;

                        vtxlist.m_VertexList.Add(m_CurVertex);
                        if (lolol) sw.Write(String.Format("VERTEX {0}\n", m_CurVertex.m_Position));
                    }
                    break;

                case 0x28:
                    {
                        uint param = m_File.Read32(pos); pos += 4;

                        short x = (short)((param << 6) & 0xFFC0);
                        short y = (short)((param >> 4) & 0xFFC0);
                        short z = (short)((param >> 14) & 0xFFC0);
                        m_CurVertex.m_Position.X += (((float)x / 262144.0f) * m_ScaleFactor);
                        m_CurVertex.m_Position.Y += (((float)y / 262144.0f) * m_ScaleFactor);
                        m_CurVertex.m_Position.Z += (((float)z / 262144.0f) * m_ScaleFactor);

                        vtxlist.m_VertexList.Add(m_CurVertex);
                        if (lolol) sw.Write(String.Format("VERTEX {0}\n", m_CurVertex.m_Position));
                    }
                    break;

                case 0x29: pos += 4; break;
                case 0x2A: pos += 4; break;
                case 0x2B: pos += 4; break;

                // lighting commands
                case 0x30: pos += 4; break;
                case 0x31: pos += 4; break;
                case 0x32: pos += 4; break;
                case 0x33: pos += 4; break;
                case 0x34: pos += 128; break;

                // Begin vertex list
                case 0x40:
                    {
                        uint param = m_File.Read32(pos); pos += 4;

                        vtxlist = new VertexList();
                        matgroup.m_Geometry.Add(vtxlist);
                        vtxlist.m_PolyType = param & 0x3;
                        vtxlist.m_VertexList = new List<Vertex>();
                        if (lolol) sw.Write(String.Format("BEGIN {0}\n", param));
                    }
                    break;

                // End vertex list (dummy)
                case 0x41: if (lolol) sw.Write("END\n"); break;

                // misc.
                case 0x50: pos += 4; break;
                case 0x60: pos += 4; break;
                case 0x70: pos += 12; break;
                case 0x71: pos += 8; break;
                case 0x72: pos += 4; break;

                default: throw new Exception(String.Format("Unknown GX command {0:X2} in BMD file", cmd));
            }

            if (lolol)
                sw.Close();
        }


        public BMD(NitroFile file)
        {
            m_File = file;
            m_FileName = file.m_Name;
            
            /* if (m_File.m_ID == 741)
                 lolol = true;
             else*/
            lolol = false;

            // Keep a list of pointers so it's easier to add/remove entries, space etc.
            m_PointerList = new List<PointerReference>();

            m_ScaleFactor = (float)(1 << (int)m_File.Read32(0x0));

            // ModelChunk refers to Bone
            m_NumModelChunks = m_File.Read32(0x04);
            m_ModelChunksOffset = m_File.Read32(0x08);
            AddPointer(0x08);
            for (int i = 0; i < m_NumModelChunks; i++)
            {
                AddPointer((uint)(m_ModelChunksOffset + (i * 64) + 0x04));
                AddPointer((uint)(m_ModelChunksOffset + (i * 64) + 0x34));
                AddPointer((uint)(m_ModelChunksOffset + (i * 64) + 0x38));
            }

            // PolyChunk refers to Display List
            m_NumPolyChunks = m_File.Read32(0x0C);
            m_PolyChunksOffset = m_File.Read32(0x10);
            AddPointer(0x10);
            for (int i = 0; i < m_NumPolyChunks; i++)
            {
                // Offset to Display List within Display List entries
                AddPointer((uint)(m_PolyChunksOffset + (i * 8) + 4));
                // Offsets within the Display List 16 byte headers
                AddPointer(m_File.Read32((uint)(m_PolyChunksOffset + (i * 8) + 4)) + 0x04);
                AddPointer(m_File.Read32((uint)(m_PolyChunksOffset + (i * 8) + 4)) + 0x0C);
            }
            m_NumTexChunks = m_File.Read32(0x14);
            m_TexChunksOffset = m_File.Read32(0x18);
            m_TextureIDs = new Dictionary<string, uint>();
            AddPointer(0x18);
            for (int i = 0; i < m_NumTexChunks; i++)
            {
                AddPointer((uint)(m_TexChunksOffset + (i * 20) + 0));
                AddPointer((uint)(m_TexChunksOffset + (i * 20) + 4));
                m_TextureIDs.Add(m_File.ReadString(m_File.Read32((uint)(m_TexChunksOffset + (20 * i))), 0), (uint)i);
            }
            m_NumPalChunks = m_File.Read32(0x1C);
            m_PalChunksOffset = m_File.Read32(0x20);
            m_PaletteIDs = new Dictionary<string, uint>();
            AddPointer(0x20);
            for (int i = 0; i < m_NumPalChunks; i++)
            {
                AddPointer((uint)(m_PalChunksOffset + (i * 16) + 0));
                AddPointer((uint)(m_PalChunksOffset + (i * 16) + 4));
                m_PaletteIDs.Add(m_File.ReadString(m_File.Read32((uint)(m_PalChunksOffset + (16 * i))), 0), (uint)i);
            }
            m_NumMatChunks = m_File.Read32(0x24);
            m_MatChunksOffset = m_File.Read32(0x28);
            AddPointer(0x28);
            for (int i = 0; i < m_NumMatChunks; i++)
            {
                AddPointer((uint)(m_MatChunksOffset + (i * 48) + 0));
            }
            m_BoneMapOffset = m_File.Read32(0x2C);
            AddPointer(0x2C);

            m_Textures = new Dictionary<string, Texture>();
            m_ModelChunks = new ModelChunk[m_NumModelChunks];

            for (uint c = 0; c < m_NumModelChunks; c++)
            {
                ModelChunk mdchunk = new ModelChunk(this);
                m_ModelChunks[c] = mdchunk;

                uint mdchunkoffset = m_ModelChunksOffset + (c * 64);

                mdchunk.m_ID = m_File.Read32(mdchunkoffset);
                mdchunk.m_Name = m_File.ReadString(m_File.Read32(mdchunkoffset + 0x04), 0);

                // transforms part
                {
                    int xscale = (int)m_File.Read32(mdchunkoffset + 0x10);
                    int yscale = (int)m_File.Read32(mdchunkoffset + 0x14);
                    int zscale = (int)m_File.Read32(mdchunkoffset + 0x18);
                    short xrot = (short)m_File.Read16(mdchunkoffset + 0x1C);
                    short yrot = (short)m_File.Read16(mdchunkoffset + 0x1E);
                    short zrot = (short)m_File.Read16(mdchunkoffset + 0x20);
                    int xtrans = (int)m_File.Read32(mdchunkoffset + 0x24);
                    int ytrans = (int)m_File.Read32(mdchunkoffset + 0x28);
                    int ztrans = (int)m_File.Read32(mdchunkoffset + 0x2C);

                    mdchunk.m_Scale = new Vector3((float)xscale / 4096.0f, (float)yscale / 4096.0f, (float)zscale / 4096.0f);
                    mdchunk.m_Rotation = new Vector3(((float)xrot * (float)Math.PI) / 2048.0f, ((float)yrot * (float)Math.PI) / 2048.0f, ((float)zrot * (float)Math.PI) / 2048.0f);
                    mdchunk.m_Translation = new Vector3((float)xtrans / 4096.0f, (float)ytrans / 4096.0f, (float)ztrans / 4096.0f);
                    mdchunk.m_Transform = Helper.SRTToMatrix(mdchunk.m_Scale, mdchunk.m_Rotation, mdchunk.m_Translation);

                    // Used when exporting bones
                    mdchunk.m_20_12Scale = new uint[] { (uint)xscale, (uint)yscale, (uint)zscale };
                    mdchunk.m_4_12Rotation = new ushort[] { (ushort)xrot, (ushort)yrot, (ushort)zrot };
                    mdchunk.m_20_12Translation = new uint[] { (uint)xtrans, (uint)ytrans, (uint)ztrans };

                    // if the chunk has a parent, apply the parent's transform to the chunk's transform.
                    // we don't need to go further than one level because the paren't transform already
                    // went through its parents' transforms.
                    short parent_offset = (short)m_File.Read16(mdchunkoffset + 0x8);
                    if (parent_offset < 0)
                    {
                        int parentchunkid = (int)(c + parent_offset);
                        Matrix4.Mult(ref mdchunk.m_Transform, ref m_ModelChunks[parentchunkid].m_Transform, out mdchunk.m_Transform);
                    }
                    mdchunk.m_ParentOffset = parent_offset;
                }
                // If 0x0A is set to 1 the bone has children, if 0 it doesn't
                mdchunk.m_HasChildren = (m_File.Read16(mdchunkoffset + 0x0A) == 1);

                mdchunk.m_SiblingOffset = (short)(m_File.Read16(mdchunkoffset + 0x0C));

                uint flags = m_File.Read32(mdchunkoffset + 0x3C);
                mdchunk.m_Billboard = ((flags & 0x1) == 0x1);

                uint numpairs = m_File.Read32(mdchunkoffset + 0x30);
                uint matlist = m_File.Read32(mdchunkoffset + 0x34);
                uint polylist = m_File.Read32(mdchunkoffset + 0x38);

                mdchunk.m_MatGroups = new MaterialGroup[numpairs];

                for (uint i = 0; i < numpairs; i++)
                {
                    MaterialGroup matgroup = new MaterialGroup();
                    mdchunk.m_MatGroups[i] = matgroup;

                    byte matID = m_File.Read8(matlist + i);
                    byte polyID = m_File.Read8(polylist + i);

                    uint mchunkoffset = (uint)(m_MatChunksOffset + (matID * 48));

                    matgroup.m_ID = matID;
                    matgroup.m_Name = m_File.ReadString(m_File.Read32(mchunkoffset), 0);
                    uint texid = m_File.Read32(mchunkoffset + 0x04);
                    uint palid = m_File.Read32(mchunkoffset + 0x08);
                    matgroup.m_TexParams = m_File.Read32(mchunkoffset + 0x20);
                    matgroup.m_PolyAttribs = m_File.Read32(mchunkoffset + 0x24);
                    matgroup.m_DifAmbColors = m_File.Read32(mchunkoffset + 0x28);
                    matgroup.m_SpeEmiColors = m_File.Read32(mchunkoffset + 0x2C);

                    if ((matgroup.m_PolyAttribs & 0x30) == 0x10)
                        matgroup.m_TexEnvMode = TextureEnvMode.Decal;
                    else
                        matgroup.m_TexEnvMode = TextureEnvMode.Modulate;

                    switch (matgroup.m_PolyAttribs & 0xC0)
                    {
                        case 0x00: matgroup.m_CullMode = CullFaceMode.FrontAndBack; break;
                        case 0x40: matgroup.m_CullMode = CullFaceMode.Front; break;
                        case 0x80: matgroup.m_CullMode = CullFaceMode.Back; break;
                    }

                    matgroup.m_DiffuseColor = Helper.BGR15ToColor((ushort)matgroup.m_DifAmbColors);
                    matgroup.m_AmbientColor = Helper.BGR15ToColor((ushort)(matgroup.m_DifAmbColors >> 16));
                    matgroup.m_SpecularColor = Helper.BGR15ToColor((ushort)matgroup.m_SpeEmiColors);
                    matgroup.m_EmissionColor = Helper.BGR15ToColor((ushort)(matgroup.m_SpeEmiColors >> 16));

                    switch (matgroup.m_TexParams >> 30)
                    {
                        case 0:
                            matgroup.m_TexCoordScale = new Vector2(1.0f, 1.0f);
                            matgroup.m_TexCoordTrans = new Vector2(0.0f, 0.0f);
                            break;

                        case 1:
                            {
                                int sscale = (int)m_File.Read32(mchunkoffset + 0x0C);
                                int tscale = (int)m_File.Read32(mchunkoffset + 0x10);
                                int strans = (int)m_File.Read32(mchunkoffset + 0x18);
                                int ttrans = (int)m_File.Read32(mchunkoffset + 0x1C);

                                matgroup.m_TexCoordScale = new Vector2((float)sscale / 4096.0f, (float)tscale / 4096.0f);
                                matgroup.m_TexCoordTrans = new Vector2((float)strans / 4096.0f, (float)ttrans / 4096.0f);
                                //matgroup.m_TexCoordTrans = new Vector2(0.0f, 16.0f);
                                /*System.Windows.Forms.MessageBox.Show(String.Format("textransform: scale:{0} trans:{1} rot:{2:X8}",
                                    matgroup.m_TexCoordScale, matgroup.m_TexCoordTrans,
                                    m_File.Read32(mchunkoffset + 0x1C)));*/
                            }
                            break;

                        case 2:
                            goto case 1;

                        case 3:
                            goto case 1;

                        default:
                            break;
                        // throw new Exception(String.Format("BMD: unsupported texture coord transform mode {0}", matgroup.m_TexParams >> 30));
                    }

                    if (texid != 0xFFFFFFFF)
                    {
                        matgroup.m_Texture = ReadTexture(texid, palid);
                        matgroup.m_TexParams |= matgroup.m_Texture.m_Params;
                    }
                    else
                        matgroup.m_Texture = null;

                    uint pchunkoffset = m_File.Read32((uint)(m_PolyChunksOffset + (polyID * 8) + 4));
                    uint dloffset = m_File.Read32(pchunkoffset + 0x0C);
                    uint dlsize = m_File.Read32(pchunkoffset + 0x08);
                    uint numbones = m_File.Read32(pchunkoffset);
                    uint bonesoffset = m_File.Read32(pchunkoffset + 0x04);

                    matgroup.m_BoneIDs = new ushort[numbones];
                    for (uint b = 0; b < numbones; b++)
                    {
                        byte idx1 = m_File.Read8(bonesoffset + b);
                        matgroup.m_BoneIDs[b] = m_File.Read16((uint)(m_BoneMapOffset + (2 * idx1)));
                    }

                    matgroup.m_Geometry = new List<VertexList>();

                    m_CurVertex.m_Position = new Vector3(0, 0, 0);
                    m_CurVertex.m_TexCoord = new Vector2(0, 0);
                    if ((matgroup.m_DifAmbColors & 0x8000) == 0x8000)
                    {
                        byte alpha = (byte)((matgroup.m_PolyAttribs >> 13) & 0xF8);
                        alpha |= (byte)(alpha >> 5);
                        matgroup.m_Alpha = alpha;

                        m_CurVertex.m_Color = Color.FromArgb(alpha, matgroup.m_DiffuseColor);
                    }
                    else
                        m_CurVertex.m_Color = Color.Black;

                    m_CurVertex.m_MatrixID = 0;

                    uint dlend = dloffset + dlsize;
                    for (uint pos = dloffset; pos < dlend; )
                    {
                        byte cmd1 = m_File.Read8(pos++);
                        byte cmd2 = m_File.Read8(pos++);
                        byte cmd3 = m_File.Read8(pos++);
                        byte cmd4 = m_File.Read8(pos++);

                        ProcessGXCommand(matgroup, cmd1, ref pos);
                        ProcessGXCommand(matgroup, cmd2, ref pos);
                        ProcessGXCommand(matgroup, cmd3, ref pos);
                        ProcessGXCommand(matgroup, cmd4, ref pos);
                    }
                }
            }

            foreach (ModelChunk mdchunk in m_ModelChunks)
            {
                foreach (MaterialGroup matgroup in mdchunk.m_MatGroups)
                {
                    matgroup.m_BoneMatrices = new Matrix4[matgroup.m_BoneIDs.Length];
                    for (uint b = 0; b < matgroup.m_BoneIDs.Length; b++)
                        matgroup.m_BoneMatrices[b] = m_ModelChunks[matgroup.m_BoneIDs[b]].m_Transform;
                }
            }
        }


        public void PrepareToRender()
        {
            foreach (ModelChunk mdchunk in m_ModelChunks)
                mdchunk.PrepareToRender();
        }

        public void Release()
        {
            foreach (ModelChunk mdchunk in m_ModelChunks)
                mdchunk.Release();
        }

        public bool Render(float scale)
        {
            bool ro = Render(RenderMode.Opaque, scale);
            bool rt = Render(RenderMode.Translucent, scale);
            return ro || rt;
        }

        public bool Render(RenderMode mode, float scale)
        {
            return Render(mode, scale, null, -1);
        }

        public bool Render(RenderMode mode, float scale, BCA animation, int frame)
        {
            bool rendered_something = false;

            for (int i = 0; i < m_ModelChunks.Length; i++)
            {
                ModelChunk mdchunk = m_ModelChunks[i];

                if (animation != null && frame > -1)
                {
                    if (mdchunk.Render(mode, scale, animation, frame))
                        rendered_something = true;
                }
                else
                {
                    if (mdchunk.Render(mode, scale))
                        rendered_something = true;
                }
            }

            return rendered_something;
        }

        public List<PointerReference> m_PointerList;

        public void AddPointer(uint _ref)
        {
            uint _ptr = m_File.Read32(_ref);
            m_PointerList.Add(new PointerReference(_ref, _ptr));
        }

        private void RemovePointer(uint _ref)
        {
            for (int i = 0; i < m_PointerList.Count; )
            {
                if (m_PointerList[i].m_ReferenceAddr == _ref)
                    m_PointerList.RemoveAt(i);
                else
                    i++;
            }
        }

        public void AddSpace(uint offset, uint amount)
        {
            // move the data
            byte[] block = m_File.ReadBlock(offset, (uint)(m_File.m_Data.Length - offset));
            m_File.WriteBlock(offset + amount, block);

            // write zeroes in the newly created space
            for (int i = 0; i < amount; i++)
                m_File.Write8((uint)(offset + i), 0);

            // update the pointers
            for (int i = 0; i < m_PointerList.Count; i++)
            {
                PointerReference ptrref = m_PointerList[i];
                if (ptrref.m_ReferenceAddr >= offset)
                    ptrref.m_ReferenceAddr += amount;
                if (ptrref.m_PointerAddr >= offset)
                {
                    ptrref.m_PointerAddr += amount;
                    m_File.Write32(ptrref.m_ReferenceAddr, ptrref.m_PointerAddr);
                }
                m_PointerList[i] = ptrref;
            }

            foreach (Texture tex in m_Textures.Values)
            {
                if (tex.m_EntryOffset >= offset)
                    tex.m_EntryOffset += amount;
                if (tex.m_PalEntryOffset >= offset)
                    tex.m_PalEntryOffset += amount;
                if (tex.m_PalOffset >= offset)
                    tex.m_PalOffset += amount;
            }
        }

        public void RemoveSpace(uint offset, uint amount)
        {
            // move the data
            byte[] block = m_File.ReadBlock(offset + amount, (uint)(m_File.m_Data.Length - offset - amount));
            m_File.WriteBlock(offset, block);
            Array.Resize(ref m_File.m_Data, (int)(m_File.m_Data.Length - amount));

            // update the pointers
            for (int i = 0; i < m_PointerList.Count; i++)
            {
                PointerReference ptrref = m_PointerList[i];
                if (ptrref.m_ReferenceAddr >= (offset + amount))
                    ptrref.m_ReferenceAddr -= amount;
                if (ptrref.m_PointerAddr >= (offset + amount))
                {
                    ptrref.m_PointerAddr -= amount;
                    m_File.Write32(ptrref.m_ReferenceAddr, ptrref.m_PointerAddr);
                }
                m_PointerList[i] = ptrref;
            }

            foreach (Texture tex in m_Textures.Values)
            {
                if (tex.m_EntryOffset >= (offset + amount))
                    tex.m_EntryOffset -= amount;
                if (tex.m_PalEntryOffset >= (offset + amount))
                    tex.m_PalEntryOffset -= amount;
                if (tex.m_PalOffset >= (offset + amount))
                    tex.m_PalOffset -= amount;
            }
        }


        public class Texture
        {
            public uint m_TexID, m_PalID;
            public string m_TexName, m_PalName;

            public uint m_Width, m_Height;
            public uint m_Params; // typically width/height and type
            public byte[] m_Data;
            public uint m_TexDataSize;
            public uint m_TexType;

            public uint m_EntryOffset, m_PalEntryOffset;
            public uint m_PalOffset, m_PalSize;

            public override bool Equals(Object obj)
            {
                var tx = obj as Texture;
                if (tx == null)
                    return false;

                if (tx.m_Data == null || this.m_Data == null)
                {
                    if (!(tx.m_Data == this.m_Data))
                        return false;
                }
                else
                {
                    if (tx.m_Data.SequenceEqual(this.m_Data) == false)
                        return false;
                }

                if (!(this.m_EntryOffset == tx.m_EntryOffset))
                    return false;
                if (!(this.m_Height == tx.m_Height))
                    return false;
                if (!(this.m_PalEntryOffset == tx.m_PalEntryOffset))
                    return false;
                if (!(this.m_PalID == tx.m_PalID))
                    return false;

                if (this.m_PalName == null || tx.m_PalName == null)
                {
                    if (this.m_PalName != tx.m_PalName)
                        return false;
                }
                else if (!(this.m_PalName.Equals(tx.m_PalName)))
                    return false;

                if (!(this.m_PalOffset == tx.m_PalOffset))
                    return false;
                if (!(this.m_PalSize == tx.m_PalSize))
                    return false;
                if (!(this.m_Params == tx.m_Params))
                    return false;
                if (!(this.m_TexDataSize == tx.m_TexDataSize))
                    return false;

                if (!(this.m_TexID == tx.m_TexID))
                    return false;

                if (this.m_TexName == null || tx.m_TexName == null)
                {
                    if (this.m_TexName != tx.m_TexName)
                        return false;
                }
                else if (!(this.m_TexName.Equals(tx.m_TexName)))
                    return false;

                if (!(this.m_TexType == tx.m_TexType))
                    return false;
                if (!(this.m_Width == tx.m_Width))
                    return false;

                return true;
            }
        }

        public struct Vertex
        {
            public Vector3 m_Position;
            public Color m_Color;
            public Vector2 m_TexCoord;
            public Vector3 m_Normal;

            public uint m_MatrixID; // used for animations and such :)
        }

        public class VertexList
        {
            public uint m_PolyType;
            public List<Vertex> m_VertexList;
        }

        public class MaterialGroup
        {
            public MaterialGroup()
            {
                m_GLTextureID = 0;
            }

            public void UploadTexture()
            {
                if (m_Texture == null)
                    return;

                if (m_GLTextureID != 0)
                    return;

                m_GLTextureID = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, m_GLTextureID);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Four, (int)m_Texture.m_Width, (int)m_Texture.m_Height,
                    0, PixelFormat.Bgra, PixelType.UnsignedByte, m_Texture.m_Data);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                    (int)(((m_TexParams & 0x10000) == 0x10000) ? (((m_TexParams & 0x40000) == 0x40000) ?
                    TextureWrapMode.MirroredRepeat : TextureWrapMode.Repeat) : TextureWrapMode.Clamp));
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                    (int)(((m_TexParams & 0x20000) == 0x20000) ? (((m_TexParams & 0x80000) == 0x80000) ?
                    TextureWrapMode.MirroredRepeat : TextureWrapMode.Repeat) : TextureWrapMode.Clamp));
            }

            public bool IsTranslucent()
            {
                return ((m_PolyAttribs & 0x001F0000) != 0x001F0000) ||
                        ((m_TexParams & 0x1C000000) == 0x04000000) ||
                        ((m_TexParams & 0x1C000000) == 0x18000000);
            }

            public void Release()
            {
                if (m_GLTextureID != 0)
                    GL.DeleteTexture(m_GLTextureID);
            }


            public uint m_ID;
            public string m_Name;

            public uint m_PolyAttribs;
            public uint m_DifAmbColors;
            public uint m_SpeEmiColors;
            public uint m_TexParams; // typically texture repeat bits

            public TextureEnvMode m_TexEnvMode;
            public CullFaceMode m_CullMode;

            public Color m_DiffuseColor, m_AmbientColor;
            public Color m_SpecularColor, m_EmissionColor;

            public byte m_Alpha = 255;

            public Vector2 m_TexCoordScale;
            public Vector2 m_TexCoordTrans;

            public ushort[] m_BoneIDs;
            public Matrix4[] m_BoneMatrices;

            public Texture m_Texture;
            public List<VertexList> m_Geometry;

            public int m_GLTextureID;
        }

        public class ModelChunk
        {
            public uint m_ID;
            public string m_Name;
            public MaterialGroup[] m_MatGroups;

            public BMD m_Model;

            public Matrix4 m_Transform;
            public bool m_Billboard;

            public short m_ParentOffset;
            public short m_SiblingOffset;
            public bool m_HasChildren;
            public Vector3 m_Scale;
            public Vector3 m_Rotation;
            public Vector3 m_Translation;
            public uint[] m_20_12Scale;
            public ushort[] m_4_12Rotation;
            public uint[] m_20_12Translation;

            public ModelChunk(BMD model)
            {
                m_Model = model;
            }

            public void PrepareToRender()
            {
                foreach (MaterialGroup matgroup in m_MatGroups)
                    matgroup.UploadTexture();
            }

            public void Release()
            {
                foreach (MaterialGroup matgroup in m_MatGroups)
                    matgroup.Release();
            }

            public bool Render(RenderMode mode, float scale)
            {
                return Render(mode, scale, null, -1);
            }

            public bool Render(RenderMode mode, float scale, BCA animation, int frame)
            {
                BeginMode[] beginmodes = { BeginMode.Triangles, BeginMode.Quads, BeginMode.TriangleStrip, BeginMode.QuadStrip };
                bool rendered_something = false;

                if (m_MatGroups.Length == 0)
                    return false;

                bool usesAnimation = (animation != null && frame > -1);
                Matrix4[] animMatrices = (usesAnimation) ?
                    animation.GetAllMatricesForFrame(m_Model.m_ModelChunks, frame) : null;

                foreach (MaterialGroup matgroup in m_MatGroups)
                {
                    if (matgroup.m_Geometry.Count == 0)
                        continue;

                    if (mode == RenderMode.Picking)
                    {
                        GL.DepthMask(!matgroup.IsTranslucent());

                        if ((!m_Billboard) && ((matgroup.m_PolyAttribs & 0xC0) != 0xC0))
                        {
                            GL.Enable(EnableCap.CullFace);
                            GL.CullFace(matgroup.m_CullMode);
                        }
                        else
                            GL.Disable(EnableCap.CullFace);

                        foreach (BMD.VertexList vtxlist in matgroup.m_Geometry)
                        {
                            if (vtxlist.m_VertexList.Count == 0)
                                continue;

                            GL.Begin(beginmodes[vtxlist.m_PolyType]);

                            foreach (BMD.Vertex vtx in vtxlist.m_VertexList)
                            {
                                Vector3 finalvtx = vtx.m_Position;

                                if (!usesAnimation)
                                {
                                    Matrix4 bonemtx = matgroup.m_BoneMatrices[vtx.m_MatrixID];
                                    Vector3.Transform(ref finalvtx, ref bonemtx, out finalvtx);
                                }
                                else
                                {
                                    int boneID = matgroup.m_BoneIDs[vtx.m_MatrixID];
                                    Matrix4 animMatrix = animMatrices[boneID];
                                    Vector3.Transform(ref finalvtx, ref animMatrix, out finalvtx);
                                }
                                Vector3.Multiply(ref finalvtx, scale, out finalvtx);

                                GL.Vertex3(finalvtx);
                            }

                            GL.End();
                            rendered_something = true;
                        }
                    }
                    else
                    {
                        if ((mode == RenderMode.Opaque) && (matgroup.IsTranslucent())) continue;
                        if ((mode == RenderMode.Translucent) && (!matgroup.IsTranslucent())) continue;

                        if ((!m_Billboard) && ((matgroup.m_PolyAttribs & 0xC0) != 0xC0))
                        {
                            GL.Enable(EnableCap.CullFace);
                            GL.CullFace(matgroup.m_CullMode);
                        }
                        else
                            GL.Disable(EnableCap.CullFace);

                        /*if ((matgroup.m_PolyAttribs & 0xF) != 0x0)
                        {
                            GL.Enable(EnableCap.Lighting);

                            if ((matgroup.m_PolyAttribs & 0x1) != 0x0) GL.Enable(EnableCap.Light0);
                            else GL.Disable(EnableCap.Light0);
                            if ((matgroup.m_PolyAttribs & 0x2) != 0x0) GL.Enable(EnableCap.Light1);
                            else GL.Disable(EnableCap.Light1);
                            if ((matgroup.m_PolyAttribs & 0x4) != 0x0) GL.Enable(EnableCap.Light2);
                            else GL.Disable(EnableCap.Light2);
                            if ((matgroup.m_PolyAttribs & 0x8) != 0x0) GL.Enable(EnableCap.Light3);
                            else GL.Disable(EnableCap.Light3);

                            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, matgroup.m_DiffuseColor);
                            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, matgroup.m_AmbientColor);
                            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, matgroup.m_SpecularColor);
                            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, matgroup.m_EmissionColor);
                        }
                        else*/
                        GL.Disable(EnableCap.Lighting);

                        GL.BindTexture(TextureTarget.Texture2D, matgroup.m_GLTextureID);

                        if (matgroup.m_GLTextureID != 0)
                            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)matgroup.m_TexEnvMode);

                        foreach (BMD.VertexList vtxlist in matgroup.m_Geometry)
                        {
                            if (vtxlist.m_VertexList.Count == 0)
                                continue;

                            GL.Begin(beginmodes[vtxlist.m_PolyType]);

                            foreach (BMD.Vertex vtx in vtxlist.m_VertexList)
                            {
                                GL.Color4(vtx.m_Color);
                                if (matgroup.m_GLTextureID != 0)
                                    GL.TexCoord2(vtx.m_TexCoord);

                                if ((matgroup.m_PolyAttribs & 0xF) != 0x0)
                                    GL.Normal3(vtx.m_Normal);

                                Vector3 finalvtx = vtx.m_Position;
                                if (!usesAnimation)
                                {
                                    Matrix4 bonemtx = matgroup.m_BoneMatrices[vtx.m_MatrixID];
                                    Vector3.Transform(ref finalvtx, ref bonemtx, out finalvtx);
                                }
                                else
                                {
                                    int boneID = matgroup.m_BoneIDs[vtx.m_MatrixID];
                                    Matrix4 animMatrix = animMatrices[boneID];
                                    Vector3.Transform(ref finalvtx, ref animMatrix, out finalvtx);
                                }
                                Vector3.Multiply(ref finalvtx, scale, out finalvtx);
                                GL.Vertex3(finalvtx);
                            }

                            GL.End();
                            rendered_something = true;
                        }
                    }
                }

                return rendered_something;
            }
        }


        public NitroFile m_File;
        public string m_FileName;

        private BoundingBox m_BBox;
        public BoundingBox BoundingBox
        {
            get { return m_BBox; }
        }

        public float m_ScaleFactor;
        public uint m_NumModelChunks, m_ModelChunksOffset;
        public uint m_NumPolyChunks, m_PolyChunksOffset;
        public uint m_NumTexChunks, m_TexChunksOffset;
        public uint m_NumPalChunks, m_PalChunksOffset;
        public uint m_NumMatChunks, m_MatChunksOffset;
        public uint m_BoneMapOffset;

        public Dictionary<string, Texture> m_Textures;
        public Dictionary<string, uint> m_TextureIDs;
        public Dictionary<string, uint> m_PaletteIDs;
        public ModelChunk[] m_ModelChunks;

        private Vertex m_CurVertex;
    }

    // Doesn't use ReadPointer, but Read32
    public struct PointerReference
    {
        public PointerReference(uint _ref, uint _ptr) { m_ReferenceAddr = _ref; m_PointerAddr = _ptr; }
        public uint m_ReferenceAddr; // where the pointer is stored
        public uint m_PointerAddr; // where the pointer points
    }
}
