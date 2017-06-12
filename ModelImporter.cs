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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Security.Cryptography;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SM64DSe.ImportExport;

namespace SM64DSe
{
    public partial class ModelImporter : Form
    {
        class VectorComparer : IEqualityComparer<Vector3>
        {
            public bool Equals(Vector3 x, Vector3 y)
            {
                return Helper.VectorsEqual(x, y);
            }

            public int GetHashCode(Vector3 v)
            {
                return v.GetHashCode();
            }
        }

        // importation settings
        private class ModelImportSettings
        {
            public float m_Scale;
            public float m_CustomScale;
            public float m_InGameModelScale;
            public BMDImporter.BMDExtraImportOptions m_ExtraOptions;
            public bool m_GenerateKCL;
            public float m_KCLMinimumFaceSize;

            public ModelImportSettings(float customScale)
            {
                m_Scale = 1f;
                m_CustomScale = customScale;
                m_InGameModelScale = customScale;
                m_ExtraOptions = BMDImporter.BMDExtraImportOptions.DEFAULT;
                m_GenerateKCL = true;
                m_KCLMinimumFaceSize = 0.0005f;
            }
        }
        private static Dictionary<string, ModelImportSettings> m_SavedModelImportSettings = new Dictionary<string, ModelImportSettings>();
        private ModelImportSettings m_ModelImportSettings;

        private BMD m_ImportedModel;

        private Dictionary<string, ModelBase.MaterialDef> m_Materials;
        private Dictionary<string, int> m_MatColTypes = new Dictionary<string, int>();
        private static Dictionary<string, Dictionary<string, int>> m_SavedMaterialCollisionTypes = new Dictionary<string, Dictionary<string, int>>();

        private String m_BMDName, m_KCLName;

        private int[] m_DisplayLists;

        private bool m_MdlLoaded;

        public ModelImporter(String modelName, String kclName, float customScale = 1f)
        {
            InitializeComponent();

            Text = "Model Importer - " + Program.AppTitle + " " + Program.AppVersion;

            m_MdlLoaded = false;
            tbModelName.Text = "None";

            m_BMDName = modelName;
            m_KCLName = kclName;

            m_ImportedModel = new BMD(Program.m_ROM.GetFileFromName(m_BMDName));

            m_ModelImportSettings = m_SavedModelImportSettings.ContainsKey(m_BMDName) ? 
                m_SavedModelImportSettings[m_BMDName] : new ModelImportSettings(customScale);

            tbScale.Text = m_ModelImportSettings.m_Scale.ToString(Helper.USA);
            txtInGameSizePreview.Text = m_ModelImportSettings.m_InGameModelScale.ToString(Helper.USA);
            if (m_ModelImportSettings.m_InGameModelScale == 1f)
            {
                chkInGamePreview.Checked = false;
                txtInGameSizePreview.Enabled = false;
            }

            cbGenerateCollision.Checked = m_ModelImportSettings.m_GenerateKCL;
            txtThreshold.Text = m_ModelImportSettings.m_KCLMinimumFaceSize.ToString(Helper.USA);

            chkAlwaysWriteFullVertexCmd23h.Checked = m_ModelImportSettings.m_ExtraOptions.m_AlwaysWriteFullVertexCmd23h;
            chkStripify.Checked = m_ModelImportSettings.m_ExtraOptions.m_ConvertToTriangleStrips;
            chkKeepVertexOrderDuringStripping.Checked = m_ModelImportSettings.m_ExtraOptions.m_KeepVertexOrderDuringStripping;
            switch (m_ModelImportSettings.m_ExtraOptions.m_TextureQualitySetting)
            {
                case BMDImporter.BMDExtraImportOptions.TextureQualitySetting.SmallestSize:
                    rbAlwaysCompress.Checked = true;
                    break;
                case BMDImporter.BMDExtraImportOptions.TextureQualitySetting.BetterQualityWhereSensible:
                    rbBetterQualityWhereSensible.Checked = true;
                    break;
                case BMDImporter.BMDExtraImportOptions.TextureQualitySetting.BestQuality:
                    rbNeverCompress.Checked = true;
                    break;
            }
            chkVFlipAllTextures.Checked = m_ModelImportSettings.m_ExtraOptions.m_VerticallyFlipAllTextures;

            m_DisplayLists = new int[1];

            glModelView.Initialise();
            glModelView.ProvideDisplayLists(m_DisplayLists);
            glModelView.ProvideScaleRef(ref m_ModelImportSettings.m_Scale);
        }

        public bool m_EarlyClosure;

        private static string m_ModelFileName;
        private static string m_ModelPath;
        private static string m_ModelFormat = "obj";

        private void LoadModel(bool required)
        {
            if (ofdLoadModel.ShowDialog(this) == DialogResult.OK)
            {
                m_ModelFileName = ofdLoadModel.FileName.Replace('/', '\\');
                m_ModelPath = m_ModelFileName.Substring(0, m_ModelFileName.LastIndexOf('\\') + 1);
                m_ModelFormat = ofdLoadModel.FileName.Substring(ofdLoadModel.FileName.Length - 3, 3).ToLower();

                ModelBase loadedModel = BMDImporter.LoadModel(m_ModelFileName);
                m_ImportedModel = BMDImporter.ConvertModelToBMD(ref m_ImportedModel.m_File,
                    m_ModelFileName, 1f, m_ModelImportSettings.m_ExtraOptions, false);
                m_Materials = loadedModel.m_Materials;

                m_MdlLoaded = true;

                PrerenderModel();

                glModelView.SetShowMarioReference(true);
                glModelView.Refresh();

                PopulateColTypes();

                tbModelName.Text = m_ModelFileName;
            }
            else if (required)
            {
                m_EarlyClosure = true;
                Close();
            }
        }

        private void PopulateColTypes(bool forceClear = false)
        {
            bool reset = true;
            if (!forceClear && Properties.Settings.Default.RememberMaterialCollisionTypeAssignments)
            {
                string key = m_ModelPath + m_ModelFileName;
                if (m_SavedMaterialCollisionTypes.ContainsKey(key))
                {
                    Dictionary<string, int> materialCollisionTypeAssignments = m_SavedMaterialCollisionTypes[key];
                    if (materialCollisionTypeAssignments.Keys.Count == m_Materials.Keys.Count &&
                        materialCollisionTypeAssignments.Keys.Except(m_Materials.Keys).Count() < 1)
                    {
                        m_MatColTypes = materialCollisionTypeAssignments;
                        reset = false;
                    }
                    else
                    {
                        m_SavedMaterialCollisionTypes.Remove(key);
                    }
                }
            }

            if (reset)
            {
                m_MatColTypes.Clear();
                foreach (string matName in m_Materials.Keys)
                {
                    m_MatColTypes.Add(matName, 0);
                }
            }

            gridColTypes.ColumnCount = 2;
            gridColTypes.Columns[0].HeaderText = "Material";
            gridColTypes.Columns[0].ReadOnly = true;
            gridColTypes.Columns[1].HeaderText = "Col. Type";

            int numMats = m_Materials.Count;
            gridColTypes.RowCount = numMats;
            for (int i = 0; i < numMats; i++)
            {
                gridColTypes.Rows[i].Cells[0].Value = m_MatColTypes.Keys.ElementAt(i);
                gridColTypes.Rows[i].Cells[1].Value = m_MatColTypes.Values.ElementAt(i).ToString();
            }
        }

        private void PrerenderModel()
        {
            if (m_DisplayLists[0] == 0)
            {
                m_DisplayLists[0] = GL.GenLists(1);
            }
            GL.NewList(m_DisplayLists[0], ListMode.Compile);

            Vector3 previewScale = new Vector3(m_ModelImportSettings.m_Scale * m_ModelImportSettings.m_InGameModelScale);

            if (m_MdlLoaded)
            {
                GL.Disable(EnableCap.Lighting);
                GL.PushMatrix();
                GL.Scale(previewScale);
                GL.FrontFace(FrontFaceDirection.Ccw);

                m_ImportedModel.PrepareToRender();

                m_ImportedModel.Render(RenderMode.Opaque, 1f);
                m_ImportedModel.Render(RenderMode.Translucent, 1f);

                GL.PopMatrix();
            }
            GL.EndList();

            m_EarlyClosure = false;
        }

        private bool VectorInList(List<Vector3> l, Vector3 p)
        {
            foreach (Vector3 v in l)
            {
                if (Helper.VectorsEqual(v, p))
                {
                    return true;
                }
            }
            return false;
        }

        private int AddToList(List<Vector3> l, Vector3 p)
        {
            int i = 0;
            foreach (Vector3 v in l)
            {
                if (Helper.VectorsEqual(v, p))
                {
                    return i;
                }
                i++;
            }

            l.Add(p);
            return l.Count - 1;
        }

        private void btnOpenModel_Click(object sender, EventArgs e)
        {
            try
            {
                LoadModel(false);
                slStatus.Text = "The above is a preview of how your model will appear. No changes have been made yet.";
            }
            catch (Exception ex)
            {
                new ExceptionMessageBox("Error loading model", ex).ShowDialog();
                return;
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            ImportModel();
        }

        private void ImportModel()
        {
            NitroFile kcl;//This'll hold the KCL file that is to be replaced, either a level's or an object's
            //If it's an object it'll be scaled down - need to get back to original value
            slStatus.Text = "Importing model...";
            glModelView.Refresh();
            m_ImportedModel = BMDImporter.ConvertModelToBMD(ref m_ImportedModel.m_File,
                m_ModelFileName, m_ModelImportSettings.m_Scale, m_ModelImportSettings.m_ExtraOptions, true);

            PrerenderModel();
            glModelView.Refresh();
            if (m_ModelImportSettings.m_GenerateKCL)
            {
                float kclScale = (!chkInGamePreview.Checked) ? m_ModelImportSettings.m_Scale : (m_ModelImportSettings.m_Scale * m_ModelImportSettings.m_InGameModelScale);
                slStatus.Text = "Importing collision map... This may take a few minutes, please be patient.";
                try
                {
                    kcl = Program.m_ROM.GetFileFromName(m_KCLName);
                    KCLImporter.ConvertModelToKCL(kcl, m_ModelFileName, kclScale, m_ModelImportSettings.m_KCLMinimumFaceSize, m_MatColTypes);
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("NitroROM: cannot find file"))
                    {
                        MessageBox.Show("This object has no collision data, however the model will still be imported.");
                    }
                    else
                    {
                        new ExceptionMessageBox("Error importing collision map", e).ShowDialog();
                    }
                    return;
                }
            }
            slStatus.Text = "Finished importing.";

            m_SavedModelImportSettings[m_BMDName] = m_ModelImportSettings;
            m_SavedMaterialCollisionTypes[m_ModelPath + m_ModelFileName] = m_MatColTypes;

            RefreshScale(1f);
            tbScale.Text = "1";

            if (Owner != null)
            {
                try 
                {
                    ((LevelEditorForm)Owner).UpdateLevelModel();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        private void tbScale_TextChanged(object sender, EventArgs e)
        {
            float val;
            if (Helper.TryParseFloat(tbScale, out val))
            {
                RefreshScale(val);
            }
        }

        private void RefreshScale(float val)
        {
            m_ModelImportSettings.m_Scale = val;
            if (m_MdlLoaded)
            {
                PrerenderModel();
                glModelView.Refresh();
            }
        }

        private void ModelImporter_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (m_EarlyClosure) return;

            glModelView.PrepareForClose();

            m_ImportedModel.Release();
        }

        private void ModelImporter_Load(object sender, EventArgs e) { }

        private void cbGenerateCollision_CheckedChanged(object sender, System.EventArgs e)
        {
            m_ModelImportSettings.m_GenerateKCL = cbGenerateCollision.Checked;
        }

        private void txtThreshold_TextChanged(object sender, System.EventArgs e)
        {
            Helper.TryParseFloat(txtThreshold, out m_ModelImportSettings.m_KCLMinimumFaceSize);
        }

        private void btnAssignTypes_Click(object sender, EventArgs e)
        {
            int count = 0;
            for (int i = 0; i < m_MatColTypes.Count; i++)
            {
                string mat = m_MatColTypes.Keys.ElementAt(i);
                m_MatColTypes[mat] = int.Parse(gridColTypes.Rows[count++].Cells[1].Value.ToString());
            }
        }

        private void btnClearTypes_Click(object sender, EventArgs e)
        {
            PopulateColTypes(true);
        }

        private void btnEditTextures_Click(object sender, EventArgs e)
        {
            new TextureEditorForm(m_BMDName).Show(this);
        }

        private void chkInGamePreview_CheckedChanged(object sender, EventArgs e)
        {
            if (chkInGamePreview.Checked)
            {
                Helper.TryParseFloat(txtInGameSizePreview.Text, out m_ModelImportSettings.m_InGameModelScale);
                txtInGameSizePreview.Enabled = true;
            }
            else if (!chkInGamePreview.Checked)
            {
                m_ModelImportSettings.m_InGameModelScale = m_ModelImportSettings.m_CustomScale;
                txtInGameSizePreview.Text = m_ModelImportSettings.m_InGameModelScale.ToString(Helper.USA);
                txtInGameSizePreview.Enabled = false;
            }

            if (m_MdlLoaded)
            {
                PrerenderModel();
                glModelView.Refresh();
            }
        }

        private void txtInGameSizePreview_TextChanged(object sender, EventArgs e)
        {
            if (chkInGamePreview.Checked)
            {
                if (Helper.TryParseFloat(txtInGameSizePreview, out m_ModelImportSettings.m_InGameModelScale))
                {
                    if (m_MdlLoaded)
                    {
                        PrerenderModel();
                        glModelView.Refresh();
                    }
                }
            }
        }

        private void chkAlwaysWriteFullVertexCmd23h_CheckedChanged(object sender, EventArgs e)
        {
            m_ModelImportSettings.m_ExtraOptions.m_AlwaysWriteFullVertexCmd23h = chkAlwaysWriteFullVertexCmd23h.Checked;
        }

        private void chkStripify_CheckedChanged(object sender, EventArgs e)
        {
            m_ModelImportSettings.m_ExtraOptions.m_ConvertToTriangleStrips = chkStripify.Checked;

            if (chkStripify.Checked)
                chkKeepVertexOrderDuringStripping.Enabled = true;
            else
                chkKeepVertexOrderDuringStripping.Enabled = false;
        }

        private void chkKeepVertexOrderDuringStripping_CheckedChanged(object sender, EventArgs e)
        {
            m_ModelImportSettings.m_ExtraOptions.m_KeepVertexOrderDuringStripping = chkKeepVertexOrderDuringStripping.Checked;
        }

        private void chkVFlipAllTextures_CheckedChanged(object sender, EventArgs e)
        {
            m_ModelImportSettings.m_ExtraOptions.m_VerticallyFlipAllTextures = chkVFlipAllTextures.Checked;
        }

        private void rbAlwaysCompress_CheckedChanged(object sender, EventArgs e)
        {
            m_ModelImportSettings.m_ExtraOptions.m_TextureQualitySetting = BMDImporter.BMDExtraImportOptions.TextureQualitySetting.SmallestSize;
        }

        private void rbBetterQualityWhereSensible_CheckedChanged(object sender, EventArgs e)
        {
            m_ModelImportSettings.m_ExtraOptions.m_TextureQualitySetting = BMDImporter.BMDExtraImportOptions.TextureQualitySetting.BetterQualityWhereSensible;
        }

        private void rbNeverCompress_CheckedChanged(object sender, EventArgs e)
        {
            m_ModelImportSettings.m_ExtraOptions.m_TextureQualitySetting = BMDImporter.BMDExtraImportOptions.TextureQualitySetting.BestQuality;
        }
    }
}
