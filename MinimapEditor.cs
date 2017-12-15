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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SM64DSe
{
    public partial class MinimapEditor : Form
    {
        private int m_NumAreas;
        private int m_CurArea;

        private int m_Zoom = 2;

        private Level m_Level;

        private ROMFileSelect m_ROMFileSelect = new ROMFileSelect();

        private NitroFile m_PalFile;
        private NitroFile m_TileSetFile;
        private NitroFile[] m_TileMapFiles;
        private NitroFile m_TileMapFile;

        private int m_SizeX, m_SizeY;// Width and Height in pixels, divide by 8 to get number of tiles
        private int m_BPP;
        private int m_PaletteRow;
        private bool m_IsUsingTileMap;

        public MinimapEditor(Level level)
        {
            m_Level = level;

            InitializeComponent();
        }

        private void RedrawMinimap(Boolean usingTmap, int sizeX, int sizeY, int bpp, int paletteRow = 0)
        {
            m_TileSetFile = Program.m_ROM.GetFileFromName(txtSelNCG.Text);
            if (chkNCGDcmp.Checked)
            {
                m_TileSetFile.ForceDecompression();
            }

            m_PalFile = Program.m_ROM.GetFileFromName(txtSelNCL.Text);
            dmnPaletteRow.Items.Clear();
            for (int i = m_PalFile.m_Data.Length, j = 0; i > 0; i -= 32, j++)
            {
                dmnPaletteRow.Items.Insert(0, j);
            }
            
            if (!txtSelNSC.Text.Equals(""))
            {
                m_IsUsingTileMap = true;
                m_TileMapFile = Program.m_ROM.GetFileFromName(txtSelNSC.Text);
                if (chkNSCDcmp.Checked)
                {
                    m_TileMapFile.ForceDecompression();
                }
            }
            else
            {
                m_IsUsingTileMap = false;
            }

            Bitmap bmp = LoadImage(m_IsUsingTileMap, sizeX, sizeY, bpp, paletteRow);

            pbxMinimapGfx.Image = new Bitmap(bmp, new Size(sizeX * m_Zoom, sizeY * m_Zoom));
            pbxMinimapGfx.Refresh();

            LoadPalette();
        }

        public Bitmap LoadImage(Boolean usingTMap, int sizeX, int sizeY, int bpp, int paletteRow = 0)
        {
            Bitmap bmp = new Bitmap(sizeX, sizeY);

            uint tileoffset = 0, tilenum = 0;
            ushort tilecrap = 0;
            for (int my = 0; my < sizeY; my += 8)
            {
                for (int mx = 0; mx < sizeX; mx += 8)
                {
                    if (usingTMap)
                    {
                        tilecrap = m_TileMapFile.Read16(tileoffset);
                        tilenum = (uint)(tilecrap & 0x03FF);
                    }

                    for (int ty = 0; ty < 8; ty++)
                    {
                        for (int tx = 0; tx < 8; tx++)
                        {
                            if (bpp == 8)
                            {
                                uint totaloffset = (uint)(tilenum * 64 + ty * 8 + tx);//Address of current pixel
                                byte palentry = m_TileSetFile.Read8(totaloffset);//Offset of current pixel's entry in palette file
                                //Palentry is double to get the position of the colour in the palette file
                                ushort pixel = m_PalFile.Read16((uint)(palentry * 2));//Colour of current pixel from palette file
                                bmp.SetPixel(mx + tx, my + ty, Helper.BGR15ToColor(pixel));
                            }
                            else if (bpp == 4)
                            {
                                float totaloffset = (float)((float)(tilenum * 64 + ty * 8 + tx) / 2f);//Address of current pixel
                                byte palentry = 0;
                                if (totaloffset % 1 == 0)
                                {
                                    palentry = m_TileSetFile.Read8((uint)totaloffset);//Offset of current pixel's entry in palette file
                                    palentry = (byte)(palentry & 0x0F);// Get 4 right bits
                                }
                                else
                                {
                                    palentry = m_TileSetFile.Read8((uint)totaloffset);//Offset of current pixel's entry in palette file
                                    palentry = (byte)(palentry >> 4);// Get 4 left bits
                                }
                                //Palentry is double to get the position of the colour in the palette file
                                ushort pixel = m_PalFile.Read16((uint)((palentry * 2) + (m_PaletteRow * 32)));//Colour of current pixel from palette file
                                bmp.SetPixel(mx + tx, my + ty, Helper.BGR15ToColor(pixel));
                            }
                        }
                    }

                    tileoffset += 2;
                    if (!usingTMap)
                        tilenum++;
                }
            }

            return bmp;
        }

        private void LoadPalette()
        {
            // Read palette colours
            Color[] paletteColours = new Color[256];
            for (int i = 0; i < m_PalFile.m_Data.Length / 2; i++)
            {
                // Colour in BGR15 format (16 bits) written to every even address 0,2,4...
                ushort palColour = m_PalFile.Read16((uint)(i * 2));
                paletteColours[i] = Helper.BGR15ToColor(palColour);
            }

            gridPalette.SetColours(paletteColours);
        }

        public void ImportBMP_4BPP(string fileName, int sizeX, int sizeY, int numTilesX, int numTilesY, byte[] tilePaletteRows)
        {
            ImportBMP(fileName, 4, sizeX, sizeY, false, numTilesX, numTilesY, tilePaletteRows);
        }

        public void ImportBMP_8BPP(string fileName, bool replaceMinimap)
        {
            ImportBMP(fileName, 8, m_SizeX, m_SizeY, replaceMinimap);
        }

        public void ImportBMP(string filename, int bpp, int sizeX, int sizeY, bool replaceMinimap = false, int numTilesX = 0, 
            int numTilesY = 0, byte[] tilePaletteRows = null)
        {
            // The tile maps (NSC / ISC) files for minimaps are always arranged a particular way - 0, 1, 2...15, 32 for 128 x 128
            Bitmap bmp = new Bitmap(filename);

            Color[] palette = bmp.Palette.Entries.ToArray<Color>();
            if (palette.Length > 256)
            {
                MessageBox.Show("Too many colours\n\nYou must import an indexed bitmap with a maximum of 256 colours.");
                return;
            }

            // Write new palette
            m_PalFile = Program.m_ROM.GetFileFromName(txtSelNCL.Text);
            m_PalFile.Clear();
            for (int i = 0; i < palette.Length; i++)
            {
                // Colour in BGR15 format (16 bits) written to every even address 0,2,4...
                m_PalFile.Write16((uint)i * 2, (ushort)(Helper.ColorToBGR15(palette[i])));
            }
            // Pad the palette to a multiple of 16 colours
            byte nColourSlots = (byte)((palette.Length % 16 != 0) ? ((palette.Length + 16) & ~15) : palette.Length);
            for (int i = palette.Length; i < nColourSlots; i++)
            {
                m_PalFile.Write16((uint)i * 2, 0);
            }

            m_PalFile.SaveChanges();
            
            // Fill current tmapfiles to use full mapsize x mapsize
            if (m_IsUsingTileMap)
            {
                m_TileMapFile = Program.m_ROM.GetFileFromName(txtSelNSC.Text);
                m_TileMapFile.Clear();
                sizeX = bmp.Width;
                sizeY = bmp.Height;
                uint addr = 0;
                int curTile = 0;
                int row = (int)(sizeX / 8);

                for (int my = 0; my < sizeY; my += 8)
                {
                    for (int mx = 0; mx < sizeX; mx += 8)
                    {
                        m_TileMapFile.Write16(addr, (ushort)curTile);
                        curTile++;
                        addr += 2;
                    }
                }
                if (chkNSCDcmp.Checked)
                    m_TileMapFile.ForceCompression();
                m_TileMapFile.SaveChanges();
            }// End If usingTMap

            //Check to see if there's already an identical tile and if so, change the current value to that
            //Works, but not if you want to keep existing data eg. multiple maps
            
            //List<List<byte>> tiles = new List<List<byte>>();
            //List<byte> curTilePal = new List<byte>();
            //uint tileoffset = 0;
            //for (int my = 0; my < sizeY; my += 8)
            //{
            //    for (int mx = 0; mx < sizeX; mx += 8)
            //    {
            //        ushort tilecrap = tmapfile.Read16(tileoffset);
            //        uint tilenum = (uint)(tilecrap & 0x03FF);

            //        curTilePal = new List<byte>();
            //        for (int ty = 0; ty < 8; ty++)
            //        {
            //            for (int tx = 0; tx < 8; tx++)
            //            {
            //                uint totaloffset = (uint)(tilenum * 64 + ty * 8 + tx);//Position of current pixel's entry
            //                curTilePal.Add((byte)(Array.IndexOf(palette, bmp.GetPixel(mx + tx, my + ty))));
            //            }
            //        }

            //        tiles.Add(curTilePal);

            //        if (posInList(tiles, curTilePal) != -1)
            //        {
            //            tmapfile.Write16(tileoffset, (ushort)(posInList(tiles, curTilePal)));
            //        }

            //        tileoffset += 2;
            //    }
            //}

            //Write the new image to file
            m_TileSetFile = Program.m_ROM.GetFileFromName(txtSelNCG.Text);
            m_TileSetFile.Clear();
            uint tileoffset = 0;
            uint tileNum = 0;
            for (int my = 0; my < sizeY; my += 8)
            {
                for (int mx = 0; mx < sizeX; mx += 8)
                {
                    for (int ty = 0; ty < 8; ty++)
                    {
                        for (int tx = 0; tx < 8; tx++)
                        {
                            if (bpp == 8)
                            {
                                uint totaloffset = (uint)(tileNum * 64 + ty * 8 + tx);//Position of current pixel's entry
                                byte palentry = (byte)(Array.IndexOf(palette, bmp.GetPixel(mx + tx, my + ty)));
                                m_TileSetFile.Write8(totaloffset, (byte)(palentry));
                            }
                            else if (bpp == 4)
                            {
                                float totaloffset = (float)((float)(tileNum * 64 + ty * 8 + tx) / 2f);//Address of current pixel
                                byte palentry = (byte)(Array.IndexOf(palette, bmp.GetPixel(mx + tx, my + ty)));

                                int currentTileIndex = (int)tileNum;
                                byte currentTileRowIndex = tilePaletteRows[currentTileIndex];

                                byte rowStartColourOffset = (byte)(16 * currentTileRowIndex);
                                byte rowEndColourOffset = (byte)(16 * (currentTileRowIndex + 1) - 1);

                                if (palentry < rowStartColourOffset || palentry > rowEndColourOffset) // Referencing colour outisde its row
                                {
                                    Color referencedColour = Helper.BGR15ToColor(m_PalFile.Read16((uint)(palentry * 2)));

                                    // Find the same colour in the correct row and set the current pixel to reference that instead
                                    for (int col = rowStartColourOffset; col < rowEndColourOffset; col++)
                                    {
                                        uint offset = (uint)(col * 2);

                                        if (offset >= m_PalFile.m_Data.Length) break;

                                        Color currentColour = Helper.BGR15ToColor(m_PalFile.Read16(offset));
                                        if (currentColour.Equals(referencedColour))
                                        {
                                            palentry = (byte)col;
                                            break;
                                        }
                                    }
                                }

                                if (totaloffset % 1 == 0)
                                {
                                    // Right 4 bits
                                    m_TileSetFile.Write8((uint)totaloffset, (byte)palentry);
                                    //(byte)((tsetfile.Read8((uint)totaloffset) & 0xF0) | palentry));
                                }
                                else
                                {
                                    // Left 4 bits
                                    m_TileSetFile.Write8((uint)totaloffset, (byte)((palentry << 4) | 
                                        (m_TileSetFile.Read8((uint)totaloffset) & 0x0F)));
                                }
                            }
                        }
                    }

                    tileoffset += 2;
                    tileNum++;
                }
            }

            if (chkNCGDcmp.Checked)
                m_TileSetFile.ForceCompression();
            m_TileSetFile.SaveChanges();

            // If it's a minimap that's being replaced, fill the tile maps to allow for multiple maps 
            // and ensure the image's displayed at the right size since minimap sizes are hard-coded, 
            // they are not based on the size of the imported image.
            if (replaceMinimap)
            {
                try
                {
                    if (chk128.Checked)
                        FillMinimapTiles(128);
                    else if (chk256.Checked)
                        FillMinimapTiles(256);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message + ex.Source + ex.StackTrace); }
            }

            m_SizeX = sizeX;
            m_SizeY = sizeY;
        }

        public void SwitchBackground(int swapped)
        {
            m_PalFile = Program.m_ROM.GetFileFromName(txtSelNCL.Text);
            //The background colour is the first colour stored in the palette
            ushort first = m_PalFile.Read16((uint)0);//Read the first colour in the palette file
            ushort swappedColour = m_PalFile.Read16((uint)(swapped * 2));//Read the colour to be swapped
            //Colour in BGR15 format (16 bits) written to every even address 0,2,4...
            m_PalFile.Write16((uint)0, swappedColour);//Write new background colour to first entry
            m_PalFile.Write16((uint)(swapped * 2), first);//Write the previously first colour to the colour being swapped

            m_PalFile.SaveChanges();

            //Swap all palette file entries for the swapped colours in the graphic file
            m_TileSetFile = Program.m_ROM.GetFileFromName(txtSelNCG.Text);
            if (chkNCGDcmp.Checked)
                m_TileSetFile.ForceDecompression();
            uint tileoffset = 0, tilenum = 0;
            ushort tilecrap = 0;
            for (int my = 0; my < m_SizeY; my += 8)
            {
                for (int mx = 0; mx < m_SizeX; mx += 8)
                {
                    if (m_IsUsingTileMap)
                    {
                        tilecrap = m_TileMapFile.Read16(tileoffset);
                        tilenum = (uint)(tilecrap & 0x03FF);
                    }

                    for (int ty = 0; ty < 8; ty++)
                    {
                        for (int tx = 0; tx < 8; tx++)
                        {
                            if (m_BPP == 8)
                            {
                                uint totaloffset = (uint)(tilenum * 64 + ty * 8 + tx);//Position of current pixel's entry
                                byte palentry = m_TileSetFile.Read8(totaloffset);
                                if (palentry == 0)//If the current pixel points to first colour in palette, 
                                    m_TileSetFile.Write8(totaloffset, (byte)(swapped));//point it to the swapped colour
                                if (palentry == (byte)swapped)//If the current pixel points to the swapped colour in palette, 
                                    m_TileSetFile.Write8(totaloffset, (byte)0);//point it to the first colour
                            }
                            else if (m_BPP == 4)
                            {
                                float totaloffset = (float)((float)(tilenum * 64 + ty * 8 + tx) / 2f);//Address of current pixel
                                byte palentry = 0;
                                if (totaloffset % 1 == 0)
                                {
                                    // Right 4 bits
                                    palentry = m_TileSetFile.Read8((uint)totaloffset);//Offset of current pixel's entry in palette file
                                    palentry = (byte)(palentry & 0x0F);// Get 4 right bits
                                    if (palentry == 0)//If the current pixel points to first colour in palette, 
                                        m_TileSetFile.Write8((uint)totaloffset, (byte)((m_TileSetFile.Read8((uint)totaloffset) & 0xF0) | swapped));//point it to the swapped colour
                                    if (palentry == (byte)swapped)//If the current pixel points to the swapped colour in palette, 
                                        m_TileSetFile.Write8((uint)totaloffset, (byte)((m_TileSetFile.Read8((uint)totaloffset) & 0xF0) | 0));//point it to the first colour
                                }
                                else
                                {
                                    // Left 4 bits
                                    palentry = m_TileSetFile.Read8((uint)totaloffset);//Offset of current pixel's entry in palette file
                                    palentry = (byte)(palentry >> 4);
                                    if (palentry == 0)//If the current pixel points to first colour in palette, 
                                        m_TileSetFile.Write8((uint)totaloffset, (byte)((swapped << 4) | (m_TileSetFile.Read8((uint)totaloffset) & 0x0F)));//point it to the swapped colour
                                    if (palentry == (byte)swapped)//If the current pixel points to the swapped colour in palette, 
                                        m_TileSetFile.Write8((uint)totaloffset, (byte)(0 | (m_TileSetFile.Read8((uint)totaloffset) & 0x0F)));//point it to the first colour
                                }
                            }
                        }
                    }

                    tileoffset += 2;
                    if (!m_IsUsingTileMap)
                        tilenum++;
                }
            }

            if (chkNCGDcmp.Checked)
                m_TileSetFile.ForceCompression();
            m_TileSetFile.SaveChanges();

            RedrawMinimap(m_IsUsingTileMap, m_SizeX, m_SizeY, m_BPP, m_PaletteRow);
        }

        private void btnAreaXX_Click(object sender, EventArgs e)
        {
            ToolStripButton myself = (ToolStripButton)sender;
            if (!myself.Checked)
            {
                int pos = tsMinimapEditor.Items.IndexOf(tslBeforeAreaBtns) + 1;
                for (int i = 0; i < m_NumAreas; i++, pos++)
                    ((ToolStripButton)tsMinimapEditor.Items[pos]).Checked = false;

                myself.Checked = true;
                m_CurArea = (int)myself.Tag;

                try
                {
                    LoadMinimapFiles();

                    RedrawMinimap(m_IsUsingTileMap, m_SizeX, m_SizeY, m_BPP);
                }
                catch { myself.Enabled = false; };// The particular tile map doesn't exist
            }
        }

        private void MinimapEditor_Load(object sender, EventArgs e)
        {
            m_NumAreas = m_Level.m_NumAreas;
            m_CurArea = 0;

            m_TileMapFiles = new NitroFile[m_NumAreas];

            txtCoordScale.Text = "" + ((m_Level.m_LevelSettings.MinimapCoordinateScale) / 1000f);

            int i, pos = tsMinimapEditor.Items.IndexOf(tslBeforeAreaBtns) + 1;
            for (i = 0; i < m_NumAreas; i++, pos++)
            {
                ToolStripButton btn = new ToolStripButton(i.ToString(), null, new EventHandler(btnAreaXX_Click));
                btn.Tag = i;
                tsMinimapEditor.Items.Insert(pos, btn);
            }

            ((ToolStripButton)tsMinimapEditor.Items[pos - i]).Checked = true;

            for (int j = 1024; j >= 0; j -= 8)
            {
                dmnWidth.Items.Add(j);
                dmnHeight.Items.Add(j);
            }

            for (int j = 15; j >= 0; j--)
            {
                dmnPaletteRow.Items.Add(j);
            }

            txtZoom.Text = "" + m_Zoom;

            LoadMinimapFiles();

            RedrawMinimap(m_IsUsingTileMap, m_SizeX, m_SizeY, m_BPP);
        }

        private void LoadMinimapFiles()
        {

            m_PalFile = Program.m_ROM.GetFileFromInternalID(m_Level.m_LevelSettings.MinimapPalFileID);
            m_TileSetFile = Program.m_ROM.GetFileFromInternalID(m_Level.m_LevelSettings.MinimapTsetFileID);
            for (int j = 0; j < m_NumAreas; j++)
            {
                try
                {
                    if (j < m_Level.m_MinimapFileIDs.Length && m_Level.m_MinimapFileIDs[j] != 0)
                    {
                        m_TileMapFiles[j] = (Program.m_ROM.GetFileFromInternalID(m_Level.m_MinimapFileIDs[j]));
                        tsMinimapEditor.Items[1 + j].Enabled = true;
                    }
                    else
                        tsMinimapEditor.Items[1 + j].Enabled = false;
                }
                catch//If the file doesn't exist
                {
                    tsMinimapEditor.Items[1 + j].Enabled = false;
                }
            }

            m_TileMapFile = m_TileMapFiles[m_CurArea];
            m_TileMapFile.ForceDecompression();// Only to get accurate size below

            m_IsUsingTileMap = true;

            m_SizeX = m_SizeY = (int)(Math.Sqrt(m_TileMapFile.m_Data.Length / 2) * 8);// Minimaps are squares
            m_BPP = 8;// Bits per pixel is always 8 for the minimaps
            dmnHeight.Text = dmnWidth.Text = "" + m_SizeX;
            m_PaletteRow = 0; dmnPaletteRow.Text = "" + m_PaletteRow;
            cbxBPP.SelectedIndex = 1;
            if (m_SizeX == 128)
            {
                chk128.Checked = true;
                chk256.Checked = false;
            }
            else if (m_SizeX == 256)
            {
                chk128.Checked = false;
                chk256.Checked = true;
            }

            txtSelNCG.Text = m_TileSetFile.m_Name;
            txtSelNCL.Text = m_PalFile.m_Name;
            txtSelNSC.Text = m_TileMapFile.m_Name;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            m_IsUsingTileMap = (txtSelNSC.Text.Trim().Length > 0);
            if (m_BPP == 8)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Select an Indexed Bitmap Image";
                ofd.Filter = "Bitmap (.bmp)|*.bmp";
                DialogResult result = ofd.ShowDialog();
                if (result == DialogResult.OK)
                {
                    try
                    {
                        ImportBMP_8BPP(ofd.FileName, chkIsMinimap.Checked);

                        if (chkIsMinimap.Checked)
                        {
                            if (chk128.Checked)
                                m_SizeX = m_SizeY = 128;
                            else if (chk256.Checked)
                                m_SizeX = m_SizeY = 256;
                        }
                        RedrawMinimap(m_IsUsingTileMap, m_SizeX, m_SizeY, m_BPP, m_PaletteRow);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + ex.Source + "\n\nAn error occured:\nCheck they're all valid files." +
                            "\nCheck whether they have/haven't already been decompressed.\nCheck it's a valid size." +
                            "\nCheck that you're using the correct bits per pixel.");
                    }
                }
            }
            else if (m_BPP == 4)
            {
                new ImportImage4BPP().ShowDialog(this);

                RedrawMinimap(m_IsUsingTileMap, m_SizeX, m_SizeY, m_BPP, m_PaletteRow);
            }
        }

        private void FillMinimapTiles(int size)
        {
            List<int> validFiles = new List<int>();
            for (int j = 0; j < m_NumAreas; j++)
            {
                try
                {
                    if (m_Level.m_MinimapFileIDs[j] != 0)
                    {
                        m_TileMapFiles[j] = (Program.m_ROM.GetFileFromInternalID(m_Level.m_MinimapFileIDs[j]));
                        validFiles.Add(j);
                    }
                }
                catch { }// Doesn't exist
            }
            for (int i = 0; i < validFiles.Count; i++)
            {
                try { m_TileMapFiles[validFiles[i]].Clear(); }
                catch { continue; }

                uint addr = 0;
                int curTile = 0;
                int row = (int)(size / 8);
                // Images are arranged left to right, top to bottom. Eg. for 128x128 maps:
                // If imported image is 256 x 256,
                // | 1 | 2 |
                // | 3 | 4 |
                    
                // If it's 384 x 384,
                // | 1 | 2 | 3 |
                // | 4 | 5 | 6 |
                // | 7 | 8 | 9 |

                int numInRow = m_SizeX / size;
                curTile += (i < numInRow) ? (row * i) : (row * (i - numInRow)) + ((row * row * numInRow) * (i / numInRow));
                int count = 0;
                for (int my = 0; my < size; my += 8)
                {
                    for (int mx = 0; mx < size; mx += 8)
                    {
                        if (count == row && validFiles.Count > 1)
                        {
                            curTile += row;
                            count = 0;
                        }
                        m_TileMapFiles[validFiles[i]].Write16(addr, (ushort)curTile);
                        curTile++;
                        count++;
                        addr += 2;
                    }
                }// End For

                if (chkNSCDcmp.Checked)
                    m_TileMapFiles[validFiles[i]].ForceCompression();
                m_TileMapFiles[validFiles[i]].SaveChanges();
            }
        }

        private void btnSetBackground_Click(object sender, EventArgs e)
        {
            int palIndex = gridPalette.GetSelectedColourIndex();
            if (palIndex < 0)
            {
                MessageBox.Show("Please select a colour first.");
            }
            else
            {
                SwitchBackground(palIndex);
            }
        }

        public void gridPalette_CurrentCellChanged(object sender, System.EventArgs e)
        {
            
        }

        private void txtCoordScale_TextChanged(object sender, EventArgs e)
        {
            if (txtCoordScale.Text != "")
            {
                try
                {
                    m_Level.m_LevelSettings.MinimapCoordinateScale = (ushort)(Convert.ToSingle(txtCoordScale.Text) * 1000);
                }
                catch
                {
                    MessageBox.Show("Please enter a valid float value in format 1.23");
                }
            }
        }

        public int GetPosInList(List<List<byte>> bigList, List<byte> indices)
        {
            if (bigList.Count == 0)
                return -1;
            for (int i = 0; i < bigList.Count; i++)
            {
                List<byte> compare = bigList[i];
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

        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog export = new SaveFileDialog();
            export.FileName = "Minimap_" + m_CurArea;//Default name
            export.DefaultExt = ".bmp";//Default file extension
            export.Filter = "Bitmap BMP (.bmp)|*.bmp";//Filter by .obj
            if (export.ShowDialog() == DialogResult.Cancel)
                return;
            try
            {
                m_SizeX = int.Parse(dmnWidth.Text);
                m_SizeY = int.Parse(dmnHeight.Text);
                m_BPP = int.Parse(cbxBPP.Items[cbxBPP.SelectedIndex].ToString());
                m_PaletteRow = (m_BPP == 4) ? int.Parse(dmnPaletteRow.Text) : 0;
                Bitmap bmp = LoadImage(m_IsUsingTileMap, m_SizeX, m_SizeY, m_BPP, m_PaletteRow);

                bmp.Save(export.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.Source);
            }
        }

        private void btnSelNCG_Click(object sender, EventArgs e)
        {
            m_ROMFileSelect.ReInitialize("Please select a Graphic (ICG/NCG) file.", new String[] { "_icg.bin","_ncg.bin" });
            var result = m_ROMFileSelect.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtSelNCG.Text = m_ROMFileSelect.m_SelectedFile;
            }
        }

        private void btnSelNCL_Click(object sender, EventArgs e)
        {
            m_ROMFileSelect.ReInitialize("Please select a Palette (ICL/NCL) file.", new String[] { "_icl.bin", "_ncl.bin" });
            var result = m_ROMFileSelect.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtSelNCL.Text = m_ROMFileSelect.m_SelectedFile;
            }
        }

        private void btnSelNSC_Click(object sender, EventArgs e)
        {
            m_ROMFileSelect.ReInitialize("Please select a Tile (ISC/NSC) file.", new String[] { "_isc.bin", "_nsc.bin" });
            var result = m_ROMFileSelect.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtSelNSC.Text = m_ROMFileSelect.m_SelectedFile;
            }
        }

        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            if (txtSelNCG.Text.Equals("") || txtSelNCL.Text.Equals(""))
                MessageBox.Show("You must select a Graphic (NCG) and a Palette (NCL) file,");
            else
            {
                try
                {
                    if (!txtSelNSC.Text.Equals(""))
                    {
                        m_IsUsingTileMap = true;
                    }
                    else
                        m_IsUsingTileMap = false;

                    m_SizeX = int.Parse(dmnWidth.Text);
                    m_SizeY = int.Parse(dmnHeight.Text);
                    m_BPP = int.Parse(cbxBPP.Items[cbxBPP.SelectedIndex].ToString());
                    m_PaletteRow = int.Parse(dmnPaletteRow.Text);

                    RedrawMinimap(m_IsUsingTileMap, m_SizeX, m_SizeY, m_BPP, m_PaletteRow);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    MessageBox.Show(ex.Message + ex.Source + "\n\n" + 
                        "An error occured:\n" + 
                        "Check they're all valid files.\n" + 
                        "Check whether they have/haven't already been decompressed.\n" + 
                        "Check it's a valid size.\n" + 
                        "Check that you're using the correct bits per pixel.\n" + 
                        "Check that if the image is 4BPP that the Palette Row exists.");
                }
            }
        }

        private void txtZoom_TextChanged(object sender, EventArgs e)
        {
            try
            {
                m_Zoom = int.Parse(txtZoom.Text);

                RedrawMinimap(m_IsUsingTileMap, m_SizeX, m_SizeY, m_BPP, m_PaletteRow);
            }
            catch { }
        }

        private void chk128_CheckedChanged(object sender, EventArgs e)
        {
            if (chk128.Checked)
                chk256.Checked = false;
            else
                chk256.Checked = true;
        }

        private void chk256_CheckedChanged(object sender, EventArgs e)
        {
            if (chk256.Checked)
                chk128.Checked = false;
            else
                chk128.Checked = true;
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e) { }

        private void btnExportToACT_Click(object sender, EventArgs e)
        {
            if (m_PalFile != null)
            {
                SaveFileDialog export = new SaveFileDialog();
                export.FileName = m_PalFile.m_Name;
                export.DefaultExt = ".ACT";
                export.Filter = "Adobe Colour Table ACT (.act)|*.act";
                if (export.ShowDialog() == DialogResult.OK)
                {
                    ExportPaletteAsACT(export.FileName);
                }
            }
        }

        private void ExportPaletteAsACT(string fileName)
        {
            byte[] data = new byte[256 * 3];

            int numColours = m_PalFile.m_Data.Length / 2;
            for (int i = 0; i < numColours; i++)
            {
                Color currentColour = Helper.BGR15ToColor(m_PalFile.Read16((uint)(i * 2)));
                data[(i * 3) + 0] = (byte)currentColour.R;
                data[(i * 3) + 1] = (byte)currentColour.G;
                data[(i * 3) + 2] = (byte)currentColour.B;
            }

            System.IO.File.WriteAllBytes(fileName, data);
        }

        private void cbxBPP_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_BPP = int.Parse(cbxBPP.Items[cbxBPP.SelectedIndex].ToString());
        }

    }
}
