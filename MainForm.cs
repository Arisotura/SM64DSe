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
using System.Net;
using System.Web;

namespace SM64DSe
{
    public partial class MainForm : Form
    {
        private void LoadROM(string filename)
        {
            if (!File.Exists(filename))
            {
                MessageBox.Show("The specified file doesn't exist.", Program.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Program.m_ROMPath != "")
            {
                while (Program.m_LevelEditors.Count > 0)
                    Program.m_LevelEditors[0].Close();

                lbxLevels.Items.Clear();

                Program.m_ROM.EndRW();
            }

            Program.m_ROMPath = filename;
            try { Program.m_ROM = new NitroROM(Program.m_ROMPath); }
            catch (Exception ex)
            {
                string msg;

                if (ex is IOException)
                    msg = "The ROM couldn't be opened. Close any program that may be using it and try again.";
                else
                    msg = "The following error occured while loading the ROM:\n" + ex.Message;

                MessageBox.Show(msg, Program.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (Program.m_ROM != null)
                {
                    Program.m_ROM.EndRW();
                }
                Program.m_ROMPath = "";
                return;
            }

            Program.m_ROM.BeginRW();
            if (Program.m_ROM.NeedsPatch())
            {
                DialogResult res = MessageBox.Show(
                    "This ROM needs to be patched before the editor can work with it.\n\n" +
                    "Do you want to first make a backup of it in case the patching\n" +
                    "operation goes wrong somehow?",
                    Program.AppTitle, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (res == DialogResult.Yes)
                {
                    sfdSaveFile.FileName = Program.m_ROMPath.Substring(0, Program.m_ROMPath.Length - 4) + "_bak.nds";
                    if (sfdSaveFile.ShowDialog(this) == DialogResult.OK)
                    {
                        Program.m_ROM.EndRW();
                        File.Copy(Program.m_ROMPath, sfdSaveFile.FileName, true);
                    }
                }
                else if (res == DialogResult.Cancel)
                {
                    Program.m_ROM.EndRW();
                    Program.m_ROMPath = "";
                    return;
                }

                // switch to buffered RW mode (faster for patching)
                Program.m_ROM.EndRW();
                Program.m_ROM.BeginRW(true);

                try { Program.m_ROM.Patch(); }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "An error occured while patching your ROM.\n" + 
                        "No changes have been made to your ROM.\n" + 
                        "Try using a different ROM. If the error persists, report it to Mega-Mario, with the details below:\n\n" + 
                        ex.Message + "\n" + 
                        ex.StackTrace,
                        Program.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Console.WriteLine(ex.StackTrace);
                    Program.m_ROM.EndRW(false);
                    Program.m_ROMPath = "";
                    return;
                }
            }

            Program.m_ROM.LoadTables();
            Program.m_ROM.EndRW();

            // Program.m_ShaderCache = new ShaderCache();

            lbxLevels.Items.AddRange(Strings.LevelNames);

            this.tvFileList.Nodes.Clear();
            ROMFileSelect.LoadFileList(this.tvFileList);
            this.tvARM9Overlays.Nodes.Clear();
            ROMFileSelect.LoadOverlayList(this.tvARM9Overlays);

            btnASMHacking.Enabled = true;
            btnTools.Enabled = true;
            btnMore.Enabled = true;
        }

        private void EnableOrDisableASMHackingCompilationAndGenerationFeatures()
        {
            if (Program.m_ROM.m_Version != NitroROM.Version.EUR)
            {
                btnASMHacking.DropDownItems.Remove(mnitASMHackingCompilation);
                btnASMHacking.DropDownItems.Remove(mnitASMHackingGeneration);
                btnASMHacking.DropDownItems.Remove(tssASMHacking001);
            }
            else
            {
                if (btnASMHacking.DropDownItems.IndexOf(mnitASMHackingCompilation) < 0)
                {
                    btnASMHacking.DropDownItems.Insert(0, mnitASMHackingCompilation);
                    btnASMHacking.DropDownItems.Insert(1, mnitASMHackingGeneration);
                    btnASMHacking.DropDownItems.Insert(2, tssASMHacking001);
                }
            }
        }

        public MainForm(string[] args)
        {
            InitializeComponent();

            Text = Program.AppTitle + " " + Program.AppVersion + " " + Program.AppDate;
            Program.m_ROMPath = "";
            Program.m_LevelEditors = new List<LevelEditorForm>();

            btnMore.DropDownItems.Add("Dump Object Info", null, btnDumpObjInfo_Click);

            slStatusLabel.Text = "Ready";
            ObjectDatabase.Initialize();

            if (args.Length >= 1) LoadROM(args[0]);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            ObjectDatabase.LoadFallback();
            try { ObjectDatabase.Load(); }
            catch { }

            if (!Properties.Settings.Default.AutoUpdateODB)
                return;

            ObjectDatabase.m_WebClient.DownloadProgressChanged += new System.Net.DownloadProgressChangedEventHandler(this.ODBDownloadProgressChanged);
            //ObjectDatabase.m_WebClient.DownloadFileCompleted += new AsyncCompletedEventHandler(this.ODBDownloadDone);
            ObjectDatabase.m_WebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(this.ODBDownloadDone);

            ObjectDatabase.Update(false);
        }

        private void ODBDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (!spbStatusProgress.Visible)
            {
                slStatusLabel.Text = "Updating object database...";
                spbStatusProgress.Visible = true;
            }

            spbStatusProgress.Value = e.ProgressPercentage;
        }

        private void ODBDownloadDone(object sender, DownloadStringCompletedEventArgs e)
        {
            spbStatusProgress.Visible = false;

            if (e.Cancelled || (e.Error != null))
            {
                slStatusLabel.Text = "Object database update " + (e.Cancelled ? "cancelled" : "failed") + ".";
            }
            else
            {
                if (e.Result == "noupdate")
                {
                    slStatusLabel.Text = "Object database already up to date.";
                }
                else
                {
                    slStatusLabel.Text = "Object database updated.";

                    try
                    {
                        File.WriteAllText("objectdb.xml", e.Result);
                    }
                    catch
                    {
                        slStatusLabel.Text = "Object database update failed.";
                    }
                }
            }

            try { ObjectDatabase.Load(); }
            catch { }
        }

        private void btnOpenROM_Click(object sender, EventArgs e)
        {
            btnEditLevel.Enabled = false;
            btnEditCollisionMap.Enabled = false;
            btnTools.Enabled = false;
            
            if (ofdOpenFile.ShowDialog(this) == DialogResult.OK)
                LoadROM(ofdOpenFile.FileName);
        }

        private void OpenLevel(int levelid)
        {
            if ((levelid < 0) || (levelid >= 52))
                return;

            foreach (LevelEditorForm lvledit in Program.m_LevelEditors)
            {
                if (lvledit.m_LevelID == levelid)
                {
                    lvledit.Focus();
                    return;
                }
            }

           // try
            {
                LevelEditorForm newedit = new LevelEditorForm(Program.m_ROM, levelid);
                newedit.Show();
                Program.m_LevelEditors.Add(newedit);
            }
            /*catch (Exception ex)
            {
                MessageBox.Show("The following error occured while opening the level:\n" + ex.Message,
                    Program.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
        }


        private void btnEditLevel_Click(object sender, EventArgs e)
        {
            OpenLevel(lbxLevels.SelectedIndex);
        }

        private void lbxLevels_DoubleClick(object sender, EventArgs e)
        {
            OpenLevel(lbxLevels.SelectedIndex);
        }

        private void lbxLevels_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnEditLevel.Enabled = (lbxLevels.SelectedIndex != -1);
            btnEditCollisionMap.Enabled = (lbxLevels.SelectedIndex != -1);
        }

        private void btnDumpObjInfo_Click(object sender, EventArgs e)
        {
            if (Program.m_ROM.m_Version != NitroROM.Version.EUR)
            {
                MessageBox.Show("Only compatible with EUR ROMs.", Program.AppTitle);
                return;
            }

            DumpObjectInfo();
        }

        private void btnUpdateODB_Click(object sender, EventArgs e)
        {
            ObjectDatabase.Update(true);
        }

        private void btnEditorSettings_Click(object sender, EventArgs e)
        {
            new SettingsForm().ShowDialog(this);
        }

        private void btnHalp_Click(object sender, EventArgs e)
        {
            string msg = Program.AppTitle + " " + Program.AppVersion + " " + Program.AppDate + "\n\n" +
                "A level editor for Super Mario 64 DS.\n" +
                "Coding and design by Mega-Mario (StapleButter), with help from others (see credits).\n" +
                "Provided to you by Kuribo64, the SM64DS hacking department.\n" +
                "\n" +
                "Credits:\n" +
                "- Treeki: the overlay decompression (Jap77), the object list and other help\n" +
                "- Dirbaio: other help\n" +
                "- blank: help with generating collision\n" + 
                "- mibts: ASM hacking template v2, BCA optimisation, level editor enhancements and other help\n" + 
                "- Fiachra Murray: current developer and maintainer\n" + 
                "\n" +
                Program.AppTitle + " is free software. If you paid for it, notify Mega-Mario about it.\n" +
                "\n" +
                "Visit Kuribo64's site (http://kuribo64.net/) for more details.";

            MessageBox.Show(msg, "About " + Program.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void mnitDumpAllOvls_Click(object sender, EventArgs e)
        {
            if (Program.m_ROM == null)
                return;
            for (int i = 0; i < 155; i++)
            {
                NitroOverlay overlay = new NitroOverlay(Program.m_ROM, (uint)i);
                string filename = "DecompressedOverlays/overlay_" + i.ToString("0000") + ".bin";
                string dir = "DecompressedOverlays";
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                System.IO.File.WriteAllBytes(filename, overlay.m_Data);
            }
            slStatusLabel.Text = "All overlays have been successfully dumped.";
        }

        private void btnEditCollisionMap_Click(object sender, EventArgs e)
        {
            uint overlayID = Program.m_ROM.GetLevelOverlayID(lbxLevels.SelectedIndex);
            NitroOverlay currentOverlay = new NitroOverlay(Program.m_ROM, overlayID);
            NitroFile currentKCL = Program.m_ROM.GetFileFromInternalID(currentOverlay.Read16((uint)(0x6A)));
            if (!Properties.Settings.Default.UseSimpleModelAndCollisionMapImporters)
            {
                ModelAndCollisionMapEditor kclForm =
                    new ModelAndCollisionMapEditor(null, currentKCL.m_Name, 1f, ModelAndCollisionMapEditor.StartMode.CollisionMap);
                kclForm.Show();
            }
            else
            {
                KCLEditorForm kclForm = new KCLEditorForm(currentKCL);
                kclForm.Show();
            }
        }

        private void mnitDecompressOverlaysWithinGame_Click(object sender, EventArgs e)
        {
            if (Program.m_ROM == null)
                return;
            Program.m_ROM.BeginRW();
            Helper.DecompressOverlaysWithinGame();
            Program.m_ROM.EndRW();
            slStatusLabel.Text = "All overlays have been decompressed successfully.";
        }

        private void mnitHexDumpToBinaryFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a hex dump to open.";
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    string hexDump = File.ReadAllText(ofd.FileName);
                    byte[] binaryData = Helper.HexDumpToBinary(hexDump);
                    System.IO.File.WriteAllBytes(ofd.FileName + ".bin", binaryData);

                    slStatusLabel.Text = "Hex dump successfully converted to binary file.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                }
            }
        }

        private void mnitAdditionalPatches_Click(object sender, EventArgs e)
        {
            AdditionalPatchesForm addPatchesForm = new AdditionalPatchesForm();
            addPatchesForm.Show();
        }

        private string m_SelectedFile;
        private string m_SelectedOverlay;

        private void tvFileList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null || e.Node.Tag == null)
                m_SelectedFile = null;
            else
                m_SelectedFile = e.Node.Tag.ToString();

            if(m_SelectedFile != null)
            {
                string status;
                if (!m_SelectedFile.StartsWith("ARCHIVE"))
                {
                    status = m_SelectedFile.Last() == '/' ?
                        String.Format("Type [Directory], ID [0x{0:X4}]", Program.m_ROM.GetDirIDFromName(m_SelectedFile.TrimEnd('/'))) :
                        String.Format("Type [File], ID [0x{0:X4}], Overlay 0 ID [0x{1:X4}]",
                            Program.m_ROM.GetFileIDFromName(m_SelectedFile),
                            Program.m_ROM.GetFileEntries()[Program.m_ROM.GetFileIDFromName(m_SelectedFile)].InternalID);
                }
                else
                {
                    status = null;
                }
                slStatusLabel.Text = status;
            }
        }

        private void btnExtractRaw_Click(object sender, EventArgs e)
        {
            if (m_SelectedFile == null || m_SelectedFile.Equals(""))
                return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = m_SelectedFile;
            if (sfd.ShowDialog() == DialogResult.Cancel)
                return;

            System.IO.File.WriteAllBytes(sfd.FileName, Program.m_ROM.GetFileFromName(m_SelectedFile).m_Data);
        }

        private void btnReplaceRaw_Click(object sender, EventArgs e)
        {
            if (m_SelectedFile == null || m_SelectedFile.Equals(""))
                return;

            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.Cancel)
                return;

            NitroFile file = Program.m_ROM.GetFileFromName(m_SelectedFile);
            file.Clear();
            file.WriteBlock(0, System.IO.File.ReadAllBytes(ofd.FileName));
            file.SaveChanges();
        }

        private void mnitEditSDATINFOBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SDATInfoEditor sdatInfoEditor = new SDATInfoEditor();
            sdatInfoEditor.Show();
        }

        private void btnLZDecompressWithHeader_Click(object sender, EventArgs e)
        {
            NitroFile file = Program.m_ROM.GetFileFromName(m_SelectedFile);
            // NitroFile automatically decompresses on load if LZ77 header present
            file.SaveChanges();
        }

        private void btnLZForceDecompression_Click(object sender, EventArgs e)
        {
            NitroFile file = Program.m_ROM.GetFileFromName(m_SelectedFile);
            try
            {
                file.ForceDecompression();
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error trying to force decompression of \"" + file.m_Name + "\", " +
                    "this file may not use LZ77 compression (no header)\n\n" + ex.Message + "\n\n" + ex.StackTrace);
            }
            file.SaveChanges();
        }

        private void btnLZCompressWithHeader_Click(object sender, EventArgs e)
        {
            NitroFile file = Program.m_ROM.GetFileFromName(m_SelectedFile);
            try
            {
                file.Compress();
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error trying to compress the file \"" + file.m_Name + "\" with " +
                    "LZ77 compression (with header)\n\n" + ex.Message + "\n\n" + ex.StackTrace);
            }
            file.SaveChanges();
        }

        private void btnLZForceCompression_Click(object sender, EventArgs e)
        {
            NitroFile file = Program.m_ROM.GetFileFromName(m_SelectedFile);
            try
            {
                file.ForceCompression();
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error trying to compress the file \"" + file.m_Name + "\" with " +
                    "LZ77 compression (no header)\n\n" + ex.Message + "\n\n" + ex.StackTrace);
            }
            file.SaveChanges();
        }

        private void btnDecompressOverlay_Click(object sender, EventArgs e)
        {
            uint ovlID = uint.Parse(m_SelectedOverlay.Substring(8));
            NitroOverlay ovl = new NitroOverlay(Program.m_ROM, ovlID);
            ovl.SaveChanges();
        }

        private void btnExtractOverlay_Click(object sender, EventArgs e)
        {
            if (m_SelectedOverlay == null)
                return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = m_SelectedOverlay;
            if (sfd.ShowDialog() == DialogResult.Cancel)
                return;

            uint ovlID = uint.Parse(m_SelectedOverlay.Substring(8));
            System.IO.File.WriteAllBytes(sfd.FileName, new NitroOverlay(Program.m_ROM, ovlID).m_Data);
        }

        private void btnReplaceOverlay_Click(object sender, EventArgs e)
        {
            if (m_SelectedOverlay == null)
                return;

            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.Cancel)
                return;

            uint ovlID = uint.Parse(m_SelectedOverlay.Substring(8));
            NitroOverlay ovl = new NitroOverlay(Program.m_ROM, ovlID);
            ovl.Clear();
            ovl.WriteBlock(0, System.IO.File.ReadAllBytes(ofd.FileName));
            ovl.SaveChanges();
        }

        private void tvARM9Overlays_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null || e.Node.Tag == null)
                m_SelectedOverlay = null;
            else
                m_SelectedOverlay = e.Node.Tag.ToString();
        }

        private void mnitToggleSuitabilityForNSMBeASMPatchingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // The v3 patch applied at the beginning prevents patched ROM's working with NSMBe's useful 
            // ASM patching feature - this simply toggles the application of that patch on and off.

            Program.m_ROM.BeginRW();

            bool suitable = (Program.m_ROM.Read32(0x4AF4) == 0xDEC00621 && Program.m_ROM.Read32(0x4AF8) == 0x2106C0DE) ? true : false;
            if (!suitable)
            {
                Program.m_ROM.Write32(0x4AF4, 0xDEC00621);
                Program.m_ROM.Write32(0x4AF8, 0x2106C0DE);
                uint binend = (Program.m_ROM.Read32(0x2C) + 0x4000);
                Program.m_ROM.Write32(binend, 0xDEC00621);
                Program.m_ROM.Write32(0x4AEC, 0x00000000);
            }
            else
            {
                Program.m_ROM.Write32(0x4AF4, 0x00000000);
                Program.m_ROM.Write32(0x4AF8, 0x00000000);
                uint binend = (Program.m_ROM.Read32(0x2C) + 0x4000);
                Program.m_ROM.Write32(binend, 0x00000000);
                Program.m_ROM.Write32(0x4AEC, 0x02061504);
            }

            Program.m_ROM.EndRW();

            MessageBox.Show("ROM is " + ((suitable) ? "no longer " : "now ") + "suitable for use with NSMBe's ASM patch insertion feature");
        }

        private void mnitToolsModelAndCollisionMapImporter_Click(object sender, EventArgs e)
        {
            new ModelAndCollisionMapEditor().Show();
        }

        private void mnitToolsCollisionMapEditor_Click(object sender, EventArgs e)
        {
            new ModelAndCollisionMapEditor(ModelAndCollisionMapEditor.StartMode.CollisionMap).Show();
        }

        private void mnitToolsModelAnimationEditor_Click(object sender, EventArgs e)
        {
            AnimationEditorForm animationEditorForm = new AnimationEditorForm();
            animationEditorForm.Show();
        }

        private void mnitToolsTextEditor_Click(object sender, EventArgs e)
        {
            new TextEditorForm().Show();
        }

        private void mnitToolsImageEditor_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not yet implemented");
        }

        private void mnitToolsBTPEditor_Click(object sender, EventArgs e)
        {
            new TextureEditorForm().Show();
        }

        private void mnitToolsSoundBrowser_Click(object sender, EventArgs e)
        {
            new SoundViewForm().Show();
        }

        private void mnitASMHackingCompilationCodeCompiler_Click(object sender, EventArgs e)
        {
            new CodeCompilerForm().Show();
        }

        private void mnitASMHackingCompilationFixCodeOffsets_Click(object sender, EventArgs e)
        {
            new CodeFixerForm().Show();
        }

        private void platformEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Templates.PlatformTemplateForm().Show();
        }
    }
}
