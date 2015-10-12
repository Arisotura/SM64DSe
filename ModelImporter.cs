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

        private LevelSettings m_LevelSettings;

        // model settings
        private Vector3 m_Scale;
        private BMDImporter.BMDExtraImportOptions m_ExtraOptions = BMDImporter.BMDExtraImportOptions.DEFAULT;
        private float m_CustomScale;
        private float m_InGameModelScale;

        // camera
        private Vector2 m_CamRotation;
        private Vector3 m_CamTarget;
        private float m_CamDistance;
        private Vector3 m_CamPosition;
        private bool m_UpsideDown;
        private Matrix4 m_CamMatrix;
        private float m_PixelFactorX, m_PixelFactorY;

        // mouse
        private MouseButtons m_MouseDown;
        private Point m_LastMouseClick, m_LastMouseMove;
        private Point m_MouseCoords;
        private uint m_UnderCursor;

        // display
        private BMD m_MarioHeadModel;
        private BMD m_MarioBodyModel;
        private int m_PDisplayList;
        private int m_DisplayList;
        private uint[] m_PickingFrameBuffer;

        private BMD m_ImportedModel;

        private Dictionary<string, ModelBase.MaterialDef> m_Materials;
        private Dictionary<string, int> m_MatColTypes = new Dictionary<string, int>();

        // misc
        private Vector3 m_MarioPosition;
        private float m_MarioRotation;

        private String m_BMDName, m_KCLName;

        private bool m_GLLoaded;
        private bool m_MdlLoaded;

        public ModelImporter(String modelName, String kclName, float customScale = 1f)
        {
            InitializeComponent();

            Text = "[EXPERIMENTAL] Model importer - " + Program.AppTitle + " " + Program.AppVersion;

            m_GLLoaded = false;
            m_MdlLoaded = false;
            tbModelName.Text = "None";

            m_BMDName = modelName;
            m_KCLName = kclName;

            m_ImportedModel = new BMD(Program.m_ROM.GetFileFromName(m_BMDName));

            m_Scale = new Vector3(1f, 1f, 1f);
            m_CustomScale = customScale;

            m_InGameModelScale = customScale;
            txtInGameSizePreview.Text = m_InGameModelScale.ToString(Helper.USA);
            if (m_InGameModelScale == 1f)
            {
                chkInGamePreview.Checked = false;
                txtInGameSizePreview.Enabled = false;
            }

            m_DisplayList = 0;
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

                BMDImporter importer = new BMDImporter();
                m_ImportedModel = importer.ConvertModelToBMD(ref m_ImportedModel.m_File,
                    m_ModelFileName, Vector3.One, m_ExtraOptions, false);
                m_Materials = importer.GetModelMaterials(m_ModelFileName);

                m_MdlLoaded = true;

                PrerenderModel();

                PopulateColTypes();

                tbModelName.Text = m_ModelFileName;
            }
            else if (required)
            {
                m_EarlyClosure = true;
                Close();
            }
        }

        private void PopulateColTypes()
        {
            m_MatColTypes.Clear();
            foreach (string matName in m_Materials.Keys)
            {
                m_MatColTypes.Add(matName, 0);
            }

            gridColTypes.ColumnCount = 2;
            gridColTypes.Columns[0].HeaderText = "Material";
            gridColTypes.Columns[1].HeaderText = "Col. Type";

            int numMats = m_Materials.Count;
            gridColTypes.RowCount = numMats;
            for (int i = 0; i < numMats; i++)
            {
                gridColTypes.Rows[i].Cells[0].Value = m_MatColTypes.Keys.ElementAt(i);
                gridColTypes.Rows[i].Cells[1].Value = m_MatColTypes.Values.ElementAt(i).ToString();
            }
        }

        private void UpdateCamera()
        {
            Vector3 up;

            if (Math.Cos(m_CamRotation.Y) < 0)
            {
                m_UpsideDown = true;
                up = new Vector3(0.0f, -1.0f, 0.0f);
            }
            else
            {
                m_UpsideDown = false;
                up = new Vector3(0.0f, 1.0f, 0.0f);
            }

            m_CamPosition.X = m_CamDistance * (float)Math.Cos(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y);
            m_CamPosition.Y = m_CamDistance * (float)Math.Sin(m_CamRotation.Y);
            m_CamPosition.Z = m_CamDistance * (float)Math.Sin(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y);

            Vector3 skybox_target;
            skybox_target.X = -(float)Math.Cos(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y);
            skybox_target.Y = -(float)Math.Sin(m_CamRotation.Y);
            skybox_target.Z = -(float)Math.Sin(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y);

            Vector3.Add(ref m_CamPosition, ref m_CamTarget, out m_CamPosition);

            m_CamMatrix = Matrix4.LookAt(m_CamPosition, m_CamTarget, up);
        }

        private void PrerenderModel()
        {
            m_MarioHeadModel = ModelCache.GetModel("data/player/mario_head_cap.bmd");
            m_MarioBodyModel = ModelCache.GetModel("data/player/mario_model.bmd");
            int[] mheaddl = ModelCache.GetDisplayLists(m_MarioHeadModel);
            int[] mbodydl = ModelCache.GetDisplayLists(m_MarioBodyModel);

            Vector3 mariopos = Vector3.Multiply(m_MarioPosition, m_Scale);

            if (m_DisplayList == 0)
                m_DisplayList = GL.GenLists(1);
            GL.NewList(m_DisplayList, ListMode.Compile);

            GL.FrontFace(FrontFaceDirection.Ccw);

            GL.Disable(EnableCap.Lighting);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Begin(BeginMode.Lines);
            GL.Color3(1f, 0f, 0f);
            GL.Vertex3(0f, 0f, 0f);
            GL.Vertex3(500f, 0f, 0f);
            GL.Color3(0f, 1f, 0f);
            GL.Vertex3(0f, 0f, 0f);
            GL.Vertex3(0f, 500f, 0f);
            GL.Color3(0f, 0f, 1f);
            GL.Vertex3(0f, 0f, 0f);
            GL.Vertex3(0f, 0f, 500f);
            GL.End();

            GL.PushMatrix();
            GL.Translate(mariopos);
            GL.Begin(BeginMode.Lines);
            GL.Color3(1f, 1f, 0f);
            GL.Vertex3(0f, 0f, 0f);
            GL.Vertex3(0f, -500f, 0f);
            GL.End();
            GL.Rotate(m_MarioRotation, Vector3.UnitY);
            GL.Scale(0.008f, 0.008f, 0.008f);
            GL.CallList(mbodydl[0]);
            GL.CallList(mbodydl[1]);
            GL.Translate(0f, 11.25f, 0f);
            GL.Rotate(-90f, Vector3.UnitY);
            GL.Rotate(180f, Vector3.UnitX);
            GL.CallList(mheaddl[0]);
            GL.CallList(mheaddl[1]);
            GL.PopMatrix();

            Vector3 previewScale = m_Scale;
            previewScale = Vector3.Multiply(m_Scale, m_InGameModelScale);

            if (m_MdlLoaded)
            {
                GL.Disable(EnableCap.Lighting);
                GL.PushMatrix();
                GL.Scale(previewScale);
                GL.FrontFace(FrontFaceDirection.Ccw);

                m_ImportedModel.PrepareToRender();

                // Render model converted to BMD
                int[] dl = new int[2];

                dl[0] = GL.GenLists(1);
                GL.NewList(dl[0], ListMode.Compile);
                m_ImportedModel.Render(RenderMode.Opaque, 1f);
                //GL.EndList();

                dl[1] = GL.GenLists(1);
                GL.NewList(dl[1], ListMode.Compile);
                m_ImportedModel.Render(RenderMode.Translucent, 1f);
                GL.EndList();

                GL.PopMatrix();

                //GL.EndList();

                //GL.CallList(dl[0]);
                //GL.CallList(dl[1]);
                // End
            }

            if (m_PDisplayList == 0)
                m_PDisplayList = GL.GenLists(1);
            GL.NewList(m_PDisplayList, ListMode.Compile);

            GL.Color4(Color.FromArgb(0x66666666));
            GL.PushMatrix();
            GL.Translate(mariopos);
            GL.Rotate(m_MarioRotation, Vector3.UnitY);
            GL.Scale(0.008f, 0.008f, 0.008f);
            GL.CallList(mbodydl[2]);
            GL.Translate(0f, 11.25f, 0f);
            GL.Rotate(-90f, Vector3.UnitY);
            GL.Rotate(180f, Vector3.UnitX);
            GL.CallList(mheaddl[2]);
            GL.PopMatrix();

            GL.EndList();
        }

        private bool VectorInList(List<Vector3> l, Vector3 p)
        {
            foreach (Vector3 v in l)
                if (Helper.VectorsEqual(v, p))
                    return true;
            return false;
        }

        private int AddToList(List<Vector3> l, Vector3 p)
        {
            int i = 0;
            foreach (Vector3 v in l)
            {
                if (Helper.VectorsEqual(v, p))
                    return i;
                i++;
            }

            l.Add(p);
            return l.Count - 1;
        }

        private void glModelView_Load(object sender, EventArgs e)
        {
            m_MarioPosition = Vector3.Zero;
            m_MarioRotation = 0f;
            m_PickingFrameBuffer = new uint[9];
            m_GLLoaded = true;

            GL.Viewport(glModelView.ClientRectangle);

            float ratio = (float)glModelView.Width / (float)glModelView.Height;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Matrix4 projmtx = Matrix4.CreatePerspectiveFieldOfView((float)((70.0f * Math.PI) / 180.0f), ratio, 0.01f, 1000.0f);
            GL.MultMatrix(ref projmtx);

            m_PixelFactorX = ((2f * (float)Math.Tan((35f * Math.PI) / 180f) * ratio) / (float)(glModelView.Width));
            m_PixelFactorY = ((2f * (float)Math.Tan((35f * Math.PI) / 180f)) / (float)(glModelView.Height));

            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Greater, 0f);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.Texture2D);

            GL.LineWidth(2.0f);

            m_CamRotation = new Vector2(0.0f, (float)Math.PI / 8.0f);
            m_CamTarget = new Vector3(0.0f, 0.0f, 0.0f);
            m_CamDistance = 1.0f;
            UpdateCamera();

            GL.ClearColor(Color.FromArgb(0, 0, 32));

            //LoadModel(true);
            m_EarlyClosure = true;
        }

        private void glModelView_Paint(object sender, PaintEventArgs e)
        {
            if (!m_GLLoaded) return;
            glModelView.Context.MakeCurrent(glModelView.WindowInfo);

            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref m_CamMatrix);

            GL.Disable(EnableCap.AlphaTest);
            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.Dither);
            GL.Disable(EnableCap.LineSmooth);
            GL.Disable(EnableCap.PolygonSmooth);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Disable(EnableCap.Lighting);

            GL.CallList(m_PDisplayList);

            GL.Flush();
            GL.ReadPixels(m_MouseCoords.X - 1, glModelView.Height - m_MouseCoords.Y + 1, 3, 3, PixelFormat.Bgra, PixelType.UnsignedByte, m_PickingFrameBuffer);

            GL.ClearColor(0.0f, 0.0f, 0.125f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref m_CamMatrix);

            GL.Enable(EnableCap.AlphaTest);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.Dither);
            GL.Enable(EnableCap.LineSmooth);
            GL.Enable(EnableCap.PolygonSmooth);
            GL.DepthMask(true);
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.CallList(m_DisplayList);
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            glModelView.SwapBuffers();
        }

        private void glModelView_Resize(object sender, EventArgs e)
        {
            if (!m_GLLoaded) return;
            glModelView.Context.MakeCurrent(glModelView.WindowInfo);

            GL.Viewport(glModelView.ClientRectangle);

            float ratio = (float)glModelView.Width / (float)glModelView.Height;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Matrix4 projmtx = Matrix4.CreatePerspectiveFieldOfView((float)((70.0f * Math.PI) / 180.0f), ratio, 0.01f, 1000.0f);
            GL.MultMatrix(ref projmtx);

            m_PixelFactorX = ((2f * (float)Math.Tan((35f * Math.PI) / 180f) * ratio) / (float)(glModelView.Width));
            m_PixelFactorY = ((2f * (float)Math.Tan((35f * Math.PI) / 180f)) / (float)(glModelView.Height));
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
                MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            ImportModel();
        }

        private void ImportModel()
        {
            float faceSizeThreshold = 0.001f;
            if (txtThreshold.Text == "")
                faceSizeThreshold = 0.001f;//Default value
            else
            {
                try { faceSizeThreshold = float.Parse(txtThreshold.Text, Helper.USA); }
                catch { MessageBox.Show(txtThreshold.Text + "\nis not a valid float value. Please enter a value in format 0.123"); return; }
            }
            NitroFile kcl;//This'll hold the KCL file that is to be replaced, either a level's or an object's
            //If it's an object it'll be scaled down - need to get back to original value
            slStatus.Text = "Importing model...";
            glModelView.Refresh();
            BMDImporter importer = new BMDImporter();
            m_ImportedModel = importer.ConvertModelToBMD(ref m_ImportedModel.m_File,
                m_ModelFileName, m_Scale, m_ExtraOptions, true);

            PrerenderModel();
            glModelView.Refresh();
            if (cbGenerateCollision.Checked)
            {
                float kclScale = (!chkInGamePreview.Checked) ? m_Scale.X : (m_Scale.X * m_InGameModelScale);
                slStatus.Text = "Importing collision map... This may take a few minutes, please be patient.";
                try
                {
                    kcl = Program.m_ROM.GetFileFromName(m_KCLName);
                    new KCLImporter().ConvertModelToKCL(kcl, m_ModelFileName, kclScale, faceSizeThreshold, m_MatColTypes);
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("NitroROM: cannot find file"))
                        MessageBox.Show("This object has no collision data, however the model will still be imported.");
                    else
                        MessageBox.Show("An error occurred importing the collision map:\n\n" +
                            e.Message + "\n\n" + e.StackTrace);
                }
            }
            slStatus.Text = "Finished importing.";

            RefreshScale(1f);
            tbScale.Text = "1";

            try { ((LevelEditorForm)Owner).UpdateLevelModel(); }
            catch { }
        }

        private static void ClampRotation(ref float val, float twopi)
        {
            if (val > twopi)
            {
                while (val > twopi)
                    val -= twopi;
            }
            else if (val < -twopi)
            {
                while (val < -twopi)
                    val += twopi;
            }
        }

        private void glModelView_MouseDown(object sender, MouseEventArgs e)
        {
            if (m_MouseDown != MouseButtons.None) return;
            m_MouseDown = e.Button;
            m_LastMouseClick = e.Location;
            m_LastMouseMove = e.Location;

            if ((m_PickingFrameBuffer[4] == m_PickingFrameBuffer[1]) &&
                (m_PickingFrameBuffer[4] == m_PickingFrameBuffer[3]) &&
                (m_PickingFrameBuffer[4] == m_PickingFrameBuffer[5]) &&
                (m_PickingFrameBuffer[4] == m_PickingFrameBuffer[7]))
                m_UnderCursor = m_PickingFrameBuffer[4];
            else
                m_UnderCursor = 0xFFFFFFFF;
        }

        private void glModelView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != m_MouseDown) return;
            m_MouseDown = MouseButtons.None;
            m_UnderCursor = 0xFFFFFFFF;
        }

        private void glModelView_MouseMove(object sender, MouseEventArgs e)
        {
            float xdelta = (float)(e.X - m_LastMouseMove.X);
            float ydelta = (float)(e.Y - m_LastMouseMove.Y);

            m_MouseCoords = e.Location;
            m_LastMouseMove = e.Location;

            if (m_MouseDown != MouseButtons.None)
            {
                if (m_UnderCursor == 0x66666666)
                {
                    if (m_MouseDown == MouseButtons.Right)
                    {
                        if (m_UpsideDown)
                            xdelta = -xdelta;

                        // TODO take obj/camera rotation into account?
                        m_MarioRotation += xdelta * 0.5f;

                        if (m_MarioRotation >= 180f)
                        {
                            m_MarioRotation = (float)(-360f + m_MarioRotation);
                        }
                        else if (m_MarioRotation < -180f)
                        {
                            m_MarioRotation = (float)(360f + m_MarioRotation);
                        }
                    }
                    else if (m_MouseDown == MouseButtons.Left)
                    {
                        Vector3 between;
                        Vector3.Subtract(ref m_CamPosition, ref m_MarioPosition, out between);

                        float objz = (((between.X * (float)Math.Cos(m_CamRotation.X)) + (between.Z * (float)Math.Sin(m_CamRotation.X))) * (float)Math.Cos(m_CamRotation.Y)) + (between.Y * (float)Math.Sin(m_CamRotation.Y));
                        objz /= m_Scale.X;

                        xdelta *= m_PixelFactorX * objz;
                        ydelta *= -m_PixelFactorY * objz;

                        float _xdelta = (xdelta * (float)Math.Sin(m_CamRotation.X)) - (ydelta * (float)Math.Sin(m_CamRotation.Y) * (float)Math.Cos(m_CamRotation.X));
                        float _ydelta = ydelta * (float)Math.Cos(m_CamRotation.Y);
                        float _zdelta = (xdelta * (float)Math.Cos(m_CamRotation.X)) + (ydelta * (float)Math.Sin(m_CamRotation.Y) * (float)Math.Sin(m_CamRotation.X));

                        Vector3 offset = new Vector3(_xdelta, _ydelta, -_zdelta);
                        Vector3.Add(ref m_MarioPosition, ref offset, out m_MarioPosition);
                    }

                    PrerenderModel();
                }
                else
                {
                    if (m_MouseDown == MouseButtons.Right)
                    {
                        /*if (btnReverseRot.Checked)
                        {
                            xdelta = -xdelta;
                            ydelta = -ydelta;
                        }*/

                        if (m_UpsideDown)
                            xdelta = -xdelta;

                        m_CamRotation.X -= xdelta * 0.002f;
                        m_CamRotation.Y -= ydelta * 0.002f;

                        ClampRotation(ref m_CamRotation.X, (float)Math.PI * 2.0f);
                        ClampRotation(ref m_CamRotation.Y, (float)Math.PI * 2.0f);
                    }
                    else if (m_MouseDown == MouseButtons.Left)
                    {
                        xdelta *= 0.005f;
                        ydelta *= 0.005f;

                        m_CamTarget.X -= xdelta * (float)Math.Sin(m_CamRotation.X);
                        m_CamTarget.X -= ydelta * (float)Math.Cos(m_CamRotation.X) * (float)Math.Sin(m_CamRotation.Y);
                        m_CamTarget.Y += ydelta * (float)Math.Cos(m_CamRotation.Y);
                        m_CamTarget.Z += xdelta * (float)Math.Cos(m_CamRotation.X);
                        m_CamTarget.Z -= ydelta * (float)Math.Sin(m_CamRotation.X) * (float)Math.Sin(m_CamRotation.Y);
                    }

                    UpdateCamera();
                }
            }

            glModelView.Refresh();
        }

        private void glModelView_MouseWheel(object sender, MouseEventArgs e)
        {
            if ((m_MouseDown == MouseButtons.Left) && (m_UnderCursor == 0x66666666))
            {
                float delta = -(e.Delta / 120f);
                delta = ((delta < 0f) ? -1f : 1f) * (float)Math.Pow(delta, 2f) * 0.05f;
                delta /= m_Scale.X;

                Vector3 offset = Vector3.Zero;
                offset.X += delta * (float)Math.Cos(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y);
                offset.Y += delta * (float)Math.Sin(m_CamRotation.Y);
                offset.Z += delta * (float)Math.Sin(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y);

                float xdist = delta * (m_MouseCoords.X - (glModelView.Width / 2f)) * m_PixelFactorX;
                float ydist = delta * (m_MouseCoords.Y - (glModelView.Height / 2f)) * m_PixelFactorY;

                offset.X -= (xdist * (float)Math.Sin(m_CamRotation.X)) + (ydist * (float)Math.Sin(m_CamRotation.Y) * (float)Math.Cos(m_CamRotation.X));
                offset.Y += ydist * (float)Math.Cos(m_CamRotation.Y);
                offset.Z += (xdist * (float)Math.Cos(m_CamRotation.X)) - (ydist * (float)Math.Sin(m_CamRotation.Y) * (float)Math.Sin(m_CamRotation.X));

                Vector3.Add(ref m_MarioPosition, ref offset, out m_MarioPosition);

                PrerenderModel();
            }
            else
            {
                float delta = -((e.Delta / 120.0f) * 0.1f);
                m_CamTarget.X += delta * (float)Math.Cos(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y);
                m_CamTarget.Y += delta * (float)Math.Sin(m_CamRotation.Y);
                m_CamTarget.Z += delta * (float)Math.Sin(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y);

                UpdateCamera();
            }
            glModelView.Refresh();
        }

        private void cbZMirror_CheckedChanged(object sender, EventArgs e)
        {
            //m_ExtraOptions.m_ZMirror = cbZMirror.Checked;
            PrerenderModel();
            glModelView.Refresh();
        }

        private void tbScale_TextChanged(object sender, EventArgs e)
        {
            float val;
            if (float.TryParse(tbScale.Text, out val) || float.TryParse(tbScale.Text, NumberStyles.Float, Helper.USA, out val))
            {
                RefreshScale(val);
            }
        }

        private void RefreshScale(float val)
        {
            m_Scale = new Vector3(val, val, val);
            PrerenderModel();
            glModelView.Refresh();
        }

        private void ModelImporter_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (m_EarlyClosure) return;

            GL.DeleteLists(m_PDisplayList, 1);
            GL.DeleteLists(m_DisplayList, 1);

            m_ImportedModel.Release();

            ModelCache.RemoveModel(m_MarioHeadModel);
            ModelCache.RemoveModel(m_MarioBodyModel);
        }

        private void ModelImporter_Load(object sender, EventArgs e)
        {
            m_LevelSettings = ((LevelEditorForm)Owner).m_LevelSettings;
        }

        private void cbSwapYZ_CheckedChanged(object sender, EventArgs e)
        {
            //m_ExtraOptions.m_SwapYZ = cbSwapYZ.Checked;
            PrerenderModel();
            glModelView.Refresh();
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

        private void btnEditTextures_Click(object sender, EventArgs e)
        {
            new TextureEditorForm(m_BMDName, this).Show(this);
        }

        private void chkInGamePreview_CheckedChanged(object sender, EventArgs e)
        {
            if (chkInGamePreview.Checked)
            {
                try { m_InGameModelScale = float.Parse(txtInGameSizePreview.Text, Helper.USA); }
                catch { MessageBox.Show("Please enter a valid scale in the format 1.23"); }
                txtInGameSizePreview.Enabled = true;
            }
            else if (!chkInGamePreview.Checked)
            {
                m_InGameModelScale = m_CustomScale;
                txtInGameSizePreview.Text = m_InGameModelScale.ToString(Helper.USA);
                txtInGameSizePreview.Enabled = false;
            }

            PrerenderModel();
            glModelView.Refresh();
        }

        private void txtInGameSizePreview_TextChanged(object sender, EventArgs e)
        {
            if (chkInGamePreview.Checked)
            {
                try
                {
                    m_InGameModelScale = float.Parse(txtInGameSizePreview.Text, Helper.USA);
                    PrerenderModel();
                    glModelView.Refresh();
                }
                catch { }
            }
        }

        private void chkAlwaysWriteFullVertexCmd23h_CheckedChanged(object sender, EventArgs e)
        {
            m_ExtraOptions.m_AlwaysWriteFullVertexCmd23h = chkAlwaysWriteFullVertexCmd23h.Checked;
        }

        private void chkStripify_CheckedChanged(object sender, EventArgs e)
        {
            m_ExtraOptions.m_ConvertToTriangleStrips = chkStripify.Checked;

            if (chkStripify.Checked)
                chkKeepVertexOrderDuringStripping.Enabled = true;
            else
                chkKeepVertexOrderDuringStripping.Enabled = false;
        }

        private void chkKeepVertexOrderDuringStripping_CheckedChanged(object sender, EventArgs e)
        {
            m_ExtraOptions.m_KeepVertexOrderDuringStripping = chkKeepVertexOrderDuringStripping.Checked;
        }

        private void chkVFlipAllTextures_CheckedChanged(object sender, EventArgs e)
        {
            m_ExtraOptions.m_VerticallyFlipAllTextures = chkVFlipAllTextures.Checked;
        }

        private void rbAlwaysCompress_CheckedChanged(object sender, EventArgs e)
        {
            m_ExtraOptions.m_TextureQualitySetting = BMDImporter.BMDExtraImportOptions.TextureQualitySetting.SmallestSize;
        }

        private void rbBetterQualityWhereSensible_CheckedChanged(object sender, EventArgs e)
        {
            m_ExtraOptions.m_TextureQualitySetting = BMDImporter.BMDExtraImportOptions.TextureQualitySetting.BetterQualityWhereSensible;
        }

        private void rbNeverCompress_CheckedChanged(object sender, EventArgs e)
        {
            m_ExtraOptions.m_TextureQualitySetting = BMDImporter.BMDExtraImportOptions.TextureQualitySetting.BestQuality;
        }
    }
}
