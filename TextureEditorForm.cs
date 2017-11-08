using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using SM64DSe.SM64DSFormats;
using SM64DSe.ImportExport.Writers.InternalWriters;
using SM64DSe.ImportExport;
using SM64DSe.ImportExport.Loaders.InternalLoaders;

namespace SM64DSe
{
    public partial class TextureEditorForm : Form
    {
        private BMD m_Model;
        private BTP m_BTP;
        private string m_ModelName;

        private ROMFileSelect m_ROMFileSelect = new ROMFileSelect();
        private FolderBrowserDialog m_FolderBrowserDialogue = new FolderBrowserDialog();

        private System.Windows.Forms.Timer m_BTPTimer;
        private int timerCount = 0;

        public TextureEditorForm(string fileName)
        {
            InitializeComponent();

            m_ModelName = fileName;
        }

        public TextureEditorForm()
            : this(null) { }

        private void TextureEditorForm_Load(object sender, System.EventArgs e)
        {
            if (m_ModelName == null)
            {
                m_ROMFileSelect.Text = "Select a BMD file to load";
                DialogResult result = m_ROMFileSelect.ShowDialog();
                if (result != DialogResult.OK)
                {
                    Close();
                }
                else
                {
                    m_ModelName = m_ROMFileSelect.m_SelectedFile;
                }
            }

            LoadTextures();
            InitTimer();
        }

        private void LoadTextures()
        {
            // Load the model
            m_Model = new BMD(Program.m_ROM.GetFileFromName(m_ModelName));

            lbxTextures.Items.Clear();

            for (int i = 0; i < m_Model.m_TextureIDs.Count; i++)
            {
                lbxTextures.Items.Add(m_Model.m_TextureIDs.Keys.ElementAt(i));
            }

            lbxPalettes.Items.Clear();

            for (int i = 0; i < m_Model.m_PaletteIDs.Keys.Count; i++)
            {
                lbxPalettes.Items.Add(m_Model.m_PaletteIDs.Keys.ElementAt(i));
            }
        }

        private void InitTimer()
        {
            m_BTPTimer = new System.Windows.Forms.Timer();
            m_BTPTimer.Interval = (int)(1000f / 30f);
            m_BTPTimer.Tick += new EventHandler(m_BTPTimer_Tick);
        }

        private void StartTimer()
        {
            timerCount = 0;
            btnMatPreview.Enabled = false;
            btnMatPreviewStop.Enabled = true;
            m_BTPTimer.Start();
        }

        private void StopTimer()
        {
            m_BTPTimer.Stop();
            btnMatPreview.Enabled = true;
            btnMatPreviewStop.Enabled = false;
        }

        private void lbxTextures_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxTextures.SelectedIndex == -1 || lbxTextures.SelectedIndex >= lbxTextures.Items.Count)
                return;
            string texName = lbxTextures.Items[lbxTextures.SelectedIndex].ToString();
            if (rbTexAllInBMD.Checked && m_Model.m_Textures.ContainsKey(texName))
            {
                if (m_Model.m_Textures[texName].m_PaletteID >= 0 && m_Model.m_Textures[texName].m_PaletteID < lbxPalettes.Items.Count)
                    lbxPalettes.SelectedIndex = (int)m_Model.m_Textures[texName].m_PaletteID;
            }
            if (rbTexAllInBMD.Checked && lbxPalettes.SelectedIndex != -1)
            {
                string palName = lbxPalettes.SelectedItem.ToString();
                NitroTexture currentTexture = NitroTexture.ReadFromBMD(m_Model, m_Model.m_TextureIDs[texName],
                    m_Model.m_PaletteIDs[palName]);

                RefreshImage(currentTexture);

                lblTexture.Text = "Texture: (ID " + m_Model.m_TextureIDs[texName] + ")";
            }
            if (rbTexAsRefInBTP.Checked)
            {
                txtBTPTextureName.Text = texName;
                if (m_Model.m_TextureIDs.ContainsKey(texName))
                    lblTexture.Text = "Texture: (ID " + m_Model.m_TextureIDs[texName] + ")";
            }
        }

        private void lbxPalettes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxPalettes.SelectedIndex < 0)
                return;
            string palName = lbxPalettes.Items[lbxPalettes.SelectedIndex].ToString();
            if (rbTexAsRefInBTP.Checked)
                txtBTPPaletteName.Text = palName;
            if (lbxTextures.SelectedIndex != -1 && lbxPalettes.SelectedIndex != -1 && lbxPalettes.SelectedIndex < lbxPalettes.Items.Count)
            {
                string texName = lbxTextures.Items[lbxTextures.SelectedIndex].ToString();
                if (m_Model.m_TextureIDs.ContainsKey(texName) && m_Model.m_PaletteIDs.ContainsKey(palName))
                {
                    NitroTexture currentTexture = NitroTexture.ReadFromBMD(m_Model, m_Model.m_TextureIDs[texName],
                        m_Model.m_PaletteIDs[palName]);

                    RefreshImage(currentTexture);

                    lblPalette.Text = "Palette: (ID " + m_Model.m_PaletteIDs[palName] + ")";
                }
            }
        }

        private void RefreshImage(NitroTexture currentTexture)
        {
            pbxTexture.Image = currentTexture.ToBitmap();
            pbxTexture.Refresh();
        }

        private void btnExportAll_Click(object sender, EventArgs e)
        {
            m_FolderBrowserDialogue.SelectedPath = System.IO.Path.GetDirectoryName(Program.m_ROMPath);
            DialogResult result = m_FolderBrowserDialogue.ShowDialog();
            if (result == DialogResult.OK)
            {
                string folderName = m_FolderBrowserDialogue.SelectedPath;
                for (int i = 0; i < m_Model.m_Textures.Values.Count; i++)
                {
                    NitroTexture currentTexture = m_Model.m_Textures.Values.ElementAt(i);

                    SaveTextureAsPNG(currentTexture, folderName + "/" + currentTexture.m_TextureName + ".png");
                }
                MessageBox.Show("Successfully exported " + m_Model.m_Textures.Values.Count + " texture(s) to:\n" + folderName);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (lbxTextures.SelectedIndex != -1 && lbxPalettes.SelectedIndex != -1)
            {
                string texName = lbxTextures.Items[lbxTextures.SelectedIndex].ToString();
                string palName = lbxPalettes.Items[lbxPalettes.SelectedIndex].ToString();
                NitroTexture currentTexture = NitroTexture.ReadFromBMD(m_Model, m_Model.m_TextureIDs[texName],
                    m_Model.m_PaletteIDs[palName]);

                SaveFileDialog export = new SaveFileDialog();
                export.FileName = currentTexture.m_TextureName;//Default name
                export.DefaultExt = ".bmp";//Because the texture has a palette
                export.Filter = "Image Files (*.bmp,*.png) | *.bmp;*.png";
                if (export.ShowDialog() == DialogResult.Cancel)
                    return;

                string extension = export.FileName.Substring(export.FileName.Length - 4, 4);
                if (extension.Equals(".png", StringComparison.CurrentCultureIgnoreCase))
                    SaveTextureAsPNG(currentTexture, export.FileName);
                else
                    SaveTextureAsBMP(currentTexture, export.FileName);
            }
            else
            {
                MessageBox.Show("Please select a texture first.");
            }
        }

        private static void SaveTextureAsBMP(NitroTexture currentTexture, String fileName)
        {
            try
            {
                currentTexture.ToBitmap().Save(fileName, System.Drawing.Imaging.ImageFormat.Bmp);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while trying to save texture " + currentTexture.m_TextureName + ".\n\n " +
                    ex.Message + "\n" + ex.Data + "\n" + ex.StackTrace + "\n" + ex.Source);
            }
        }

        private static void SaveTextureAsPNG(NitroTexture currentTexture, String fileName)
        {
            try
            {
                currentTexture.ToBitmap().Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while trying to save texture " + currentTexture.m_TextureName + ".\n\n " +
                    ex.Message + "\n" + ex.Data + "\n" + ex.StackTrace + "\n" + ex.Source);
            }
        }

        private void btnReplaceSelected_Click(object sender, EventArgs e)
        {
            // TODO: Ideally this will be done by loading the BMD to a ModelBase object, modifying the 
            // selected textures and then writing the ModelBase object to BMD again.

            if (lbxTextures.SelectedIndex != -1)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Select an image";
                DialogResult result = ofd.ShowDialog();
                if (result == DialogResult.Cancel) return;

                int texIndex = lbxTextures.SelectedIndex = (int)m_Model.m_Textures[lbxTextures.SelectedItem.ToString()].m_TextureID;
                int palIndex = lbxPalettes.SelectedIndex = (int)m_Model.m_Textures[lbxTextures.SelectedItem.ToString()].m_PaletteID;

                try
                {
                    Bitmap bmp = new Bitmap(ofd.FileName);

                    NitroTexture tex = NitroTexture.FromBitmapAndType(
                        (uint)texIndex, m_Model.m_Textures[lbxTextures.SelectedItem.ToString()].m_TextureName, 
                        (uint)palIndex, m_Model.m_Textures[lbxTextures.SelectedItem.ToString()].m_PaletteName, 
                        bmp, BestTexTypeForBitmap(bmp, chkCompressReplacedTextures.Checked));

                    // Update texture entry
                    uint curoffset = m_Model.m_Textures[lbxTextures.SelectedItem.ToString()].m_EntryOffset;

                    m_Model.m_File.Write32(curoffset + 0x08, (uint)tex.m_TextureDataLength);
                    m_Model.m_File.Write16(curoffset + 0x0C, (ushort)(8 << (int)((tex.m_DSTexParam >> 20) & 0x7)));
                    m_Model.m_File.Write16(curoffset + 0x0E, (ushort)(8 << (int)((tex.m_DSTexParam >> 23) & 0x7)));
                    m_Model.m_File.Write32(curoffset + 0x10, tex.m_DSTexParam);

                    // Update palette entry
                    if (tex.m_RawPaletteData != null)
                    {
                        curoffset = m_Model.m_Textures[lbxTextures.SelectedItem.ToString()].m_PalEntryOffset;

                        m_Model.m_File.Write32(curoffset + 0x08, (uint)tex.m_RawPaletteData.Length);
                        m_Model.m_File.Write32(curoffset + 0x0C, 0xFFFFFFFF);
                    }

                    // Write new texture and texture palette data

                    // Check whether we need to make room for additional data
                    uint oldTexDataSize = (uint)m_Model.m_Textures[lbxTextures.SelectedItem.ToString()].m_RawTextureData.Length;
                    uint newTexDataSize = (uint)((tex.m_RawTextureData.Length + 3) & ~3);
                    uint oldPalDataSize = (uint)m_Model.m_Textures[lbxTextures.SelectedItem.ToString()].m_PaletteDataLength;
                    uint newPalDataSize = (uint)((tex.m_PaletteDataLength + 3) & ~3);

                    uint texDataOffset = m_Model.m_File.Read32(m_Model.m_Textures[lbxTextures.SelectedItem.ToString()].m_EntryOffset + 0x04);
                    // If necessary, make room for additional texture data
                    if (newTexDataSize > oldTexDataSize) m_Model.AddSpace(texDataOffset + oldTexDataSize, newTexDataSize - oldTexDataSize);

                    m_Model.m_File.WriteBlock(texDataOffset, tex.m_RawTextureData);

                    uint palDataOffset = m_Model.m_File.Read32(m_Model.m_Textures[lbxTextures.SelectedItem.ToString()].m_PalEntryOffset + 0x04);
                    // If necessary, make room for additional palette data
                    if (newPalDataSize > oldPalDataSize) m_Model.AddSpace(palDataOffset + oldPalDataSize, newPalDataSize - oldPalDataSize);
                    // Reload palette data offset
                    palDataOffset = m_Model.m_File.Read32(m_Model.m_Textures[lbxTextures.SelectedItem.ToString()].m_PalEntryOffset + 0x04);

                    if (tex.m_RawPaletteData != null) m_Model.m_File.WriteBlock(palDataOffset, tex.m_RawPaletteData);

                    m_Model.m_File.SaveChanges();

                    LoadTextures();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + ex.Source + ex.TargetSite + ex.StackTrace);
                }
            }
            else
            {
                MessageBox.Show("Please select a texture first.");
            }
        }

        private static int BestTexTypeForBitmap(Bitmap bmp, bool compress = true)
        {
            bool alpha = NitroTexture.BitmapUsesTranslucency(bmp);
            int nColours = NitroTexture.CountColoursInBitmap(bmp);

            if (alpha)
            {
                return ((nColours <= 8) ? 6 : 1);
            }
            else
            {
                if (compress) return 5;
                else if (nColours <= 4) return 2;
                else if (nColours <= 16) return 3;
                else if (nColours <= 256) return 4;
                else return 7;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            m_Model.m_File.SaveChanges();
        }

        private void btnLoadBTP_Click(object sender, EventArgs e)
        {
            m_ROMFileSelect.Text = "Select a BTP file to load";
            DialogResult result = m_ROMFileSelect.ShowDialog();
            if (result == DialogResult.OK)
            {
                ClearBTPTextBoxes();
                LoadBTP(m_ROMFileSelect.m_SelectedFile);
            }
        }

        private void LoadBTP(String filename)
        {
            try
            {
                NitroFile file = Program.m_ROM.GetFileFromName(filename);
                m_BTP = new BTP(file);

                LoadOnlyBTPReferencedTextures();

                PopulateBTPListBoxes();

                EnableBTPFormControls();
            }
            catch (Exception ex) { MessageBox.Show("Error loading BTP:\n" + ex.Message + "\n" + ex.StackTrace); }
        }

        private void PopulateBTPListBoxes()
        {
            lbxBTPFrames.Items.Clear();
            lbxBTPMaterials.Items.Clear();

            for (int i = 0; i < m_BTP.NumFrames(); i++)
            {
                lbxBTPFrames.Items.Add(String.Format("{0:D3}", i));
            }

            for (int i = 0; i < m_BTP.m_MaterialData.Keys.Count; i++)
            {
                lbxBTPMaterials.Items.Add(m_BTP.m_MaterialData.Keys.ElementAt(i));
            }

            if (lbxBTPMaterials.Items.Count > 0)
                lbxBTPMaterials.SelectedIndex = 0;
        }

        private void EnableBTPFormControls()
        {
            rbTexAsRefInBTP.Enabled = true; rbTexAsRefInBTP.Checked = true;

            btnBTPAddTexture.Enabled = true; btnBTPAddTexture.Visible = true;
            btnBTPRemoveTexture.Enabled = true; btnBTPRemoveTexture.Visible = true;
            btnBTPAddPalette.Enabled = true; btnBTPAddPalette.Visible = true;
            btnBTPRemovePalette.Enabled = true; btnBTPRemovePalette.Visible = true;
            btnBTPRenameTexture.Enabled = true; btnBTPRenameTexture.Visible = true;
            btnBTPRenamePalette.Enabled = true; btnBTPRenamePalette.Visible = true;

            txtBTPTextureName.Enabled = true; txtBTPTextureName.Visible = true;
            txtBTPPaletteName.Enabled = true; txtBTPPaletteName.Visible = true;
        }

        private void DisableBTPFormControls()
        {
            btnBTPAddTexture.Enabled = false; btnBTPAddTexture.Visible = false;
            btnBTPRemoveTexture.Enabled = false; btnBTPRemoveTexture.Visible = false;
            btnBTPAddPalette.Enabled = false; btnBTPAddPalette.Visible = false;
            btnBTPRemovePalette.Enabled = false; btnBTPRemovePalette.Visible = false;
            btnBTPRenameTexture.Enabled = false; btnBTPRenameTexture.Visible = false;
            btnBTPRenamePalette.Enabled = false; btnBTPRenamePalette.Visible = false;

            txtBTPTextureName.Enabled = false; txtBTPTextureName.Visible = false;
            txtBTPPaletteName.Enabled = false; txtBTPPaletteName.Visible = false;
        }

        private void m_BTPTimer_Tick(object sender, EventArgs e)
        {
            string matName = lbxBTPMaterials.Items[lbxBTPMaterials.SelectedIndex].ToString();

            if (timerCount >= m_BTP.m_MaterialData[matName].m_NumFrames)
            {
                StopTimer();
                return;
            }

            for (int i = m_BTP.m_MaterialData[matName].m_StartOffsetFrameChanges; i <
                m_BTP.m_MaterialData[matName].m_StartOffsetFrameChanges + m_BTP.m_MaterialData[matName].m_NumFrameChanges; i++)
            {
                if (timerCount == m_BTP.m_Frames[i].m_FrameNum)
                {
                    lbxBTPFrames.SelectedIndex = m_BTP.m_Frames[i].m_FrameChangeID;
                    break;
                }
            }

            timerCount++;
        }

        private void ClearBTPTextBoxes()
        {
            txtBTPFrameLength.Text = "";
            txtBTPFramePalID.Text = "";
            txtBTPFrameTexID.Text = "";
            txtBTPMatNumFrameChanges.Text = "";
            txtBTPMatStartOffsetFrameChanges.Text = "";
            lbxBTPFrames.Items.Clear();
            lbxBTPMaterials.Items.Clear();
        }

        private void lbxBTPMaterials_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = lbxBTPMaterials.SelectedIndex;

            if (index == -1)
                return;

            string matName = lbxBTPMaterials.Items[index].ToString();

            txtBTPMatNumFrameChanges.Text = m_BTP.m_MaterialData[matName].m_NumFrameChanges.ToString();
            txtBTPMatStartOffsetFrameChanges.Text = m_BTP.m_MaterialData[matName].m_StartOffsetFrameChanges.ToString();
            txtBTPMaterialName.Text = matName;
        }

        private void lbxBTPFrames_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = lbxBTPFrames.SelectedIndex;

            if (index == -1 || index >= lbxBTPFrames.Items.Count)
                return;

            for (int i = 0; i < m_BTP.m_MaterialData.Values.Count; i++)
            {
                BTP.BTPMaterialData matData = m_BTP.m_MaterialData.Values.ElementAt(i);
                if (index >= matData.m_StartOffsetFrameChanges &&
                    index < matData.m_StartOffsetFrameChanges + matData.m_NumFrameChanges)
                {
                    lbxBTPMaterials.SelectedIndex = i;
                }
            }

            txtBTPFrameTexID.Text = m_BTP.m_Frames[index].m_TextureID.ToString();
            txtBTPFramePalID.Text = m_BTP.m_Frames[index].m_PaletteID.ToString();
            txtBTPFrameLength.Text = m_BTP.m_Frames[index].m_Length.ToString();

            if (m_BTP.m_Frames[index].m_TextureID >= 0 && m_BTP.m_Frames[index].m_TextureID < lbxTextures.Items.Count)
                lbxTextures.SelectedIndex = (int)m_BTP.m_Frames[index].m_TextureID;
            if (m_BTP.m_Frames[index].m_PaletteID >= 0 && m_BTP.m_Frames[index].m_PaletteID < lbxPalettes.Items.Count)
                lbxPalettes.SelectedIndex = (int)m_BTP.m_Frames[index].m_PaletteID;
        }

        private void btnMatPreview_Click(object sender, EventArgs e)
        {
            int index = lbxBTPMaterials.SelectedIndex;

            if (lbxBTPMaterials.Items.Count <= 0)
                return;
            if (!(index >= 0 && index < m_BTP.m_MaterialData.Count))
                lbxBTPMaterials.SelectedIndex = index = 0;

            string matName = lbxBTPMaterials.Items[index].ToString();
            if (m_BTP.m_MaterialData[matName].m_NumFrameChanges > 0 &&
                m_BTP.m_MaterialData[matName].m_StartOffsetFrameChanges < m_BTP.NumFrames())
            {
                StartTimer();
            }
        }

        private void rbTexAllInBMD_CheckedChanged(object sender, EventArgs e)
        {
            if (rbTexAllInBMD.Checked)
            {
                rbTexAsRefInBTP.Checked = false;
                DisableBTPFormControls();
                LoadTextures();
            }
        }

        private void rbTexAsRefInBTP_CheckedChanged(object sender, EventArgs e)
        {
            if (rbTexAsRefInBTP.Checked)
            {
                rbTexAllInBMD.Checked = false;
                EnableBTPFormControls();
                LoadOnlyBTPReferencedTextures();
            }
        }

        private void LoadOnlyBTPReferencedTextures()
        {
            /* This is necessary as the texture and palette ID's in the BTP file don't reference the 
             * texture and palette ID's within the BMD model - they reference the ones named in the BTP file
            */

            lbxTextures.Items.Clear();
            lbxPalettes.Items.Clear();

            for (int i = 0; i < m_BTP.m_TextureNames.Count; i++)
            {
                lbxTextures.Items.Add(m_BTP.m_TextureNames[i]);
            }

            for (int i = 0; i < m_BTP.m_PaletteNames.Count; i++)
            {
                lbxPalettes.Items.Add(m_BTP.m_PaletteNames[i]);
            }
        }

        private void btnBTPAddTexture_Click(object sender, EventArgs e)
        {
            string newTexName = txtBTPTextureName.Text;
            if (newTexName.Length > 0)
            {
                m_BTP.AddTexture(newTexName);
                LoadOnlyBTPReferencedTextures();
            }
        }

        private void btnBTPAddPalette_Click(object sender, EventArgs e)
        {
            string newPalName = txtBTPPaletteName.Text;
            if (newPalName.Length > 0)
            {
                m_BTP.AddPalette(newPalName);
                LoadOnlyBTPReferencedTextures();
            }
        }

        private void btnBTPRemoveTexture_Click(object sender, EventArgs e)
        {
            int index = lbxTextures.SelectedIndex;
            if (index >= 0 && index < lbxTextures.Items.Count)
            {
                m_BTP.RemoveTexture(lbxTextures.Items[index].ToString());
                LoadOnlyBTPReferencedTextures();
            }
        }

        private void btnBTPRemovePalette_Click(object sender, EventArgs e)
        {
            int index = lbxPalettes.SelectedIndex;
            if (index >= 0 && index < lbxPalettes.Items.Count)
            {
                m_BTP.RemovePalette(lbxPalettes.Items[index].ToString());
                LoadOnlyBTPReferencedTextures();
            }
        }

        private void btnBTPAddFrame_Click(object sender, EventArgs e)
        {
            int index = lbxBTPFrames.SelectedIndex;
            if (txtBTPFrameLength.Text.Equals("") || txtBTPFrameTexID.Text.Equals("") || txtBTPFramePalID.Text.Equals(""))
            {
                MessageBox.Show("Invalid frame values entered.");
                return;
            }
            try
            {
                uint textureID = uint.Parse(txtBTPFrameTexID.Text);
                uint paletteID = uint.Parse(txtBTPFramePalID.Text);
                int length = int.Parse(txtBTPFrameLength.Text);

                m_BTP.AddFrame(textureID, paletteID, length, (index != -1) ? index + 1 : index);

                LoadOnlyBTPReferencedTextures();
                PopulateBTPListBoxes();
            }
            catch { MessageBox.Show("Invalid frame values entered."); }
        }

        private void btnBTPRemoveFrame_Click(object sender, EventArgs e)
        {
            int index = lbxBTPFrames.SelectedIndex;
            if (index >= 0)
            {
                m_BTP.RemoveFrame(index);
                LoadOnlyBTPReferencedTextures();
                PopulateBTPListBoxes();
                if (index < m_BTP.m_Frames.Count)
                    lbxBTPFrames.SelectedIndex = index;
                else
                    lbxBTPFrames.SelectedIndex = m_BTP.m_Frames.Count - 1;
            }
        }

        private void btnBTPAddMaterial_Click(object sender, EventArgs e)
        {
            string matName = txtBTPMaterialName.Text;
            if (!matName.Equals(""))
            {
                try
                {
                    ushort numFrameChanges = ushort.Parse(txtBTPMatNumFrameChanges.Text);
                    ushort startOffsetFrameChanges = ushort.Parse(txtBTPMatStartOffsetFrameChanges.Text);

                    m_BTP.AddMaterial(matName, numFrameChanges, startOffsetFrameChanges);

                    PopulateBTPListBoxes();
                }
                catch { MessageBox.Show("Invalid material data entered."); }
            }
        }

        private void btnBTPRemoveMaterial_Click(object sender, EventArgs e)
        {
            int index = lbxBTPMaterials.SelectedIndex;
            if (index != -1)
            {
                m_BTP.RemoveMaterial(lbxBTPMaterials.Items[index].ToString());
                PopulateBTPListBoxes();
                if (index < m_BTP.m_MaterialData.Count)
                    lbxBTPMaterials.SelectedIndex = index;
                else
                    lbxBTPMaterials.SelectedIndex = m_BTP.m_MaterialData.Count - 1;
            }
        }

        private void btnMatPreviewStop_Click(object sender, EventArgs e)
        {
            StopTimer();
        }

        private void btnSaveBTP_Click(object sender, EventArgs e)
        {
            try
            {
                m_BTP.SaveChanges();
                LoadBTP(m_BTP.m_FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred when trying to save the BTP file: " + ex.Message + "\n\n" +
                    ex.StackTrace);
            }
        }

        private void txtBTPMatStartOffsetFrameChanges_TextChanged(object sender, EventArgs e)
        {
            int index = lbxBTPMaterials.SelectedIndex;
            if (index >= 0 && !txtBTPMatStartOffsetFrameChanges.Text.Equals(""))
            {
                try
                {
                    string matName = lbxBTPMaterials.Items[index].ToString();
                    ushort startOffsetFrameChanges = ushort.Parse(txtBTPMatStartOffsetFrameChanges.Text);
                    m_BTP.SetMaterialStartOffsetFrameChanges(matName, startOffsetFrameChanges);
                }
                catch { }
            }
        }

        private void txtBTPMatNumFrameChanges_TextChanged(object sender, EventArgs e)
        {
            int index = lbxBTPMaterials.SelectedIndex;
            if (index >= 0 && !txtBTPMatNumFrameChanges.Text.Equals(""))
            {
                try
                {
                    string matName = lbxBTPMaterials.Items[index].ToString();
                    ushort numFrameChanges = ushort.Parse(txtBTPMatNumFrameChanges.Text);
                    m_BTP.SetMaterialNumFrameChanges(matName, numFrameChanges);
                }
                catch { }
            }
        }

        private void txtBTPFrameTexID_TextChanged(object sender, EventArgs e)
        {
            int index = lbxBTPFrames.SelectedIndex;
            if (index >= 0 && !txtBTPFrameTexID.Text.Equals(""))
            {
                try
                {
                    uint textureID = uint.Parse(txtBTPFrameTexID.Text);
                    m_BTP.m_Frames[index].m_TextureID = textureID;
                }
                catch { }
            }
        }

        private void txtBTPFramePalID_TextChanged(object sender, EventArgs e)
        {
            int index = lbxBTPFrames.SelectedIndex;
            if (index >= 0 && !txtBTPFramePalID.Text.Equals(""))
            {
                try
                {
                    uint paletteID = uint.Parse(txtBTPFramePalID.Text);
                    m_BTP.m_Frames[index].m_PaletteID = paletteID;
                }
                catch { }
            }
        }

        private void txtBTPFrameLength_TextChanged(object sender, EventArgs e)
        {
            int index = lbxBTPFrames.SelectedIndex;
            if (index >= 0 && !txtBTPFrameLength.Text.Equals(""))
            {
                try
                {
                    int length = int.Parse(txtBTPFrameLength.Text);
                    m_BTP.m_Frames[index].m_Length = length;
                }
                catch { }
            }
        }

        private void btnBTPRenameTexture_Click(object sender, EventArgs e)
        {
            int index = lbxTextures.SelectedIndex;
            string newName = txtBTPTextureName.Text;
            if (index >= 0 && !newName.Equals(""))
            {
                m_BTP.m_TextureNames[index] = newName;
                LoadOnlyBTPReferencedTextures();
            }
        }

        private void btnBTPRenamePalette_Click(object sender, EventArgs e)
        {
            int index = lbxPalettes.SelectedIndex;
            string newName = txtBTPPaletteName.Text;
            if (index >= 0 && !newName.Equals(""))
            {
                m_BTP.m_PaletteNames[index] = newName;
                LoadOnlyBTPReferencedTextures();
            }
        }

    }
}
