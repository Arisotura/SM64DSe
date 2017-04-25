using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using SM64DSe.ImportExport;
using SM64DSe.ImportExport.Loaders.InternalLoaders;

namespace SM64DSe
{
    public partial class KCLEditorForm : Form
    {
        private bool m_GLLoaded;
        private float m_AspectRatio;
        private float m_PickingDepth;
        private const float k_zNear = 0.01f;
        private const float k_zFar = 1000f;
        private const float k_FOV = (float)(70f * Math.PI) / 180f;

        private bool m_WireFrameView = false;

        private int[] m_KCLMeshDLists = new int[4];// Picking, Fill, WireFrame, Highlight

        public List<KCL.ColFace> m_Planes;

        private List<Color> m_Colours;

        private KCL m_KCL;

        private Dictionary<string, int> m_MatColTypes = new Dictionary<string, int>();
        private static Dictionary<string, Dictionary<string, int>> m_SavedMaterialCollisionTypes = new Dictionary<string, Dictionary<string, int>>();

        public KCLEditorForm(NitroFile kclIn)
        {
            InitializeComponent();
            LoadKCL(kclIn);
            LoadColours();
            cmbPolygonMode.Items.Add("Fill");
            cmbPolygonMode.Items.Add("Wireframe");
            cmbPolygonMode.SelectedIndex = 0;
        }

        private void LoadColours()
        {
            List<int> uniqueCollisionTypes = new List<int>();
            foreach (KCL.ColFace plane in m_Planes)
            {
                if (!uniqueCollisionTypes.Contains(plane.type))
                    uniqueCollisionTypes.Add(plane.type);
            }
            uniqueCollisionTypes.Sort();
            m_Colours = KCLLoader.GetColours(uniqueCollisionTypes[uniqueCollisionTypes.Count - 1] + 1);
        }

        public void LoadKCL(NitroFile kcl)
        {
            m_KCL = new KCL(kcl);

            m_Planes = m_KCL.m_Planes;

            lbxPlanes.Items.Clear();

            for (int i = 0; i < m_Planes.Count; i++)
            {
                lbxPlanes.Items.Add("Plane " + i.ToString("00000"));
            }

            LoadColours();
        }

        private void WriteChanges()
        {
            NitroFile kclFile = m_KCL.m_File;

            uint planeStart = (kclFile.Read32(8));

            planeStart += (uint)(0x10);

            for (int i = 0; i < m_Planes.Count; i++)
            {
                uint posColType = (uint)(planeStart + (i * 16) + 0x0E);//Get the address of this plane's Collision Type variable

                kclFile.Write16(posColType, (ushort)m_Planes[i].type);//Write the new value to file
            }

            kclFile.SaveChanges();
        }

        private void glModelView_Load(object sender, EventArgs e)
        {
            m_GLLoaded = true;

            glModelView.Context.MakeCurrent(glModelView.WindowInfo);

            m_PickingFrameBuffer = new uint[9];
            m_PickingDepth = 0f;

            GL.Viewport(glModelView.ClientRectangle);

            m_AspectRatio = (float)glModelView.Width / (float)glModelView.Height;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Matrix4 projmtx = Matrix4.CreatePerspectiveFieldOfView(k_FOV, m_AspectRatio, k_zNear, k_zFar);
            GL.MultMatrix(ref projmtx);

            GL.Enable(EnableCap.DepthTest);
            GL.ClearDepth(1.0);

            // lighting!
            GL.Light(LightName.Light0, LightParameter.Position, new Vector4(1.0f, 1.0f, 1.0f, 0.0f));
            GL.Light(LightName.Light0, LightParameter.Ambient, Color.SkyBlue);
            GL.Light(LightName.Light0, LightParameter.Diffuse, Color.SkyBlue);
            GL.Light(LightName.Light0, LightParameter.Specular, Color.SkyBlue);

            GL.Enable(EnableCap.Normalize);

            m_CamRotation = new Vector2(0.0f, (float)Math.PI / 8.0f);
            m_CamTarget = new Vector3(0.0f, 0.0f, 0.0f);
            m_CamDistance = 1.0f;//6.5f;
            UpdateCamera();

            GL.LineWidth(1f);

            m_PixelFactorX = ((2f * (float)Math.Tan(k_FOV / 2f) * m_AspectRatio) / (float)(glModelView.Width));
            m_PixelFactorY = ((2f * (float)Math.Tan(k_FOV / 2f)) / (float)(glModelView.Height));

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Greater, 0.0f);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            RenderKCLMesh();
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
        }

        private void RenderKCLMesh()
        {
            m_KCLMeshDLists[0] = GL.GenLists(1);
            GL.NewList(m_KCLMeshDLists[0], ListMode.Compile);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            for (int i = 0; i < m_Planes.Count; i++)
            {
                GL.Begin(BeginMode.Triangles);
                GL.Color4(Color.FromArgb(i));
                GL.Vertex3(m_Planes[i].point1);
                GL.Vertex3(m_Planes[i].point2);
                GL.Vertex3(m_Planes[i].point3);
                GL.End();
            }
            GL.EndList();

            m_KCLMeshDLists[1] = GL.GenLists(1);
            GL.NewList(m_KCLMeshDLists[1], ListMode.Compile);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Enable(EnableCap.PolygonOffsetFill);
            GL.PolygonOffset(1f, 1f);
            for (int i = 0; i < m_Planes.Count; i++)
            {
                Color planeColour = m_Colours[m_Planes[i].type];

                GL.Begin(BeginMode.Triangles);
                GL.Color3(planeColour);
                GL.Vertex3(m_Planes[i].point1);
                GL.Vertex3(m_Planes[i].point2);
                GL.Vertex3(m_Planes[i].point3);
                GL.End();
            }
            GL.Disable(EnableCap.PolygonOffsetFill);
            GL.EndList();

            m_KCLMeshDLists[2] = GL.GenLists(1);
            GL.NewList(m_KCLMeshDLists[2], ListMode.Compile);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            for (int i = 0; i < m_Planes.Count; i++)
            {
                GL.Begin(BeginMode.LineStrip);
                GL.Color3(Color.Orange);
                GL.Vertex3(m_Planes[i].point1);
                GL.Vertex3(m_Planes[i].point2);
                GL.Vertex3(m_Planes[i].point3);
                GL.End();
            }
            GL.EndList();
        }

        private void RenderHighlight()
        {
            m_KCLMeshDLists[3] = GL.GenLists(1);
            GL.NewList(m_KCLMeshDLists[3], ListMode.Compile);
            foreach (int idx in lbxPlanes.SelectedIndices)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                GL.Begin(BeginMode.Triangles);
                GL.Color3(Color.RoyalBlue);
                GL.Vertex3(m_Planes[idx].point1);
                GL.Vertex3(m_Planes[idx].point2);
                GL.Vertex3(m_Planes[idx].point3);
                GL.End();
            }
            GL.EndList();
        }

        private void glModelView_Paint(object sender, PaintEventArgs e)
        {
            if (!m_GLLoaded) return;
            glModelView.Context.MakeCurrent(glModelView.WindowInfo);

            // Pass 1 - picking mode rendering (render stuff with fake colors that identify triangles)

            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref m_CamMatrix);

            GL.Disable(EnableCap.AlphaTest);
            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.Dither);
            GL.Disable(EnableCap.LineSmooth);
            GL.Disable(EnableCap.PolygonSmooth);
            GL.Disable(EnableCap.Lighting);

            // Picking
            GL.CallList(m_KCLMeshDLists[0]);

            GL.Flush();
            GL.ReadPixels(m_MouseCoords.X - 1, glModelView.Height - m_MouseCoords.Y + 1, 3, 3, PixelFormat.Bgra, PixelType.UnsignedByte, m_PickingFrameBuffer);

            // depth math from http://www.opengl.org/resources/faq/technical/depthbuffer.htm
            GL.ReadPixels(m_MouseCoords.X, glModelView.Height - m_MouseCoords.Y, 1, 1, PixelFormat.DepthComponent, PixelType.Float, ref m_PickingDepth);
            m_PickingDepth = -(k_zFar * k_zNear / (m_PickingDepth * (k_zFar - k_zNear) - k_zFar));

            GL.DepthMask(true);
            GL.ClearColor(0.0f, 0.0f, 0.125f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            GL.Enable(EnableCap.AlphaTest);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.Dither);
            GL.Enable(EnableCap.LineSmooth);
            GL.Enable(EnableCap.PolygonSmooth);

            GL.LoadMatrix(ref m_CamMatrix);

            // Solid polygons
            if (!m_WireFrameView)
            {
                GL.CallList(m_KCLMeshDLists[1]);
            }

            // WireFrame overlay
            GL.CallList(m_KCLMeshDLists[2]);

            // Highlighted triangles
            GL.CallList(m_KCLMeshDLists[3]);

            glModelView.SwapBuffers();
        }

        //Code for moving the camera, rotating etc.

        private void glModelView_MouseDown(object sender, MouseEventArgs e)
        {
            if (m_MouseDown != MouseButtons.None) return;

            m_MouseDown = e.Button;
            m_LastMouseClick = e.Location;
            m_LastMouseMove = e.Location;
        }

        private void glModelView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != m_MouseDown) return;

            if ((Math.Abs(e.X - m_LastMouseClick.X) < 3) && (Math.Abs(e.Y - m_LastMouseClick.Y) < 3) &&
                (m_PickingFrameBuffer[4] == m_PickingFrameBuffer[1]) &&
                (m_PickingFrameBuffer[4] == m_PickingFrameBuffer[3]) &&
                (m_PickingFrameBuffer[4] == m_PickingFrameBuffer[5]) &&
                (m_PickingFrameBuffer[4] == m_PickingFrameBuffer[7]))
            {
                int sel = (int)m_PickingFrameBuffer[4];
                //Console.WriteLine((int)m_PickingFrameBuffer[4]);

                if (!lbxPlanes.SelectedIndices.Contains(sel))
                    lbxPlanes.SelectedIndices.Add(sel);
                else
                    lbxPlanes.SelectedIndices.Remove(sel);
            }

            m_MouseDown = MouseButtons.None;
        }

        private void glModelView_MouseMove(object sender, MouseEventArgs e)
        {
            float xdelta = (float)(e.X - m_LastMouseMove.X);
            float ydelta = (float)(e.Y - m_LastMouseMove.Y);

            m_MouseCoords = e.Location;
            m_LastMouseMove = e.Location;

            if (m_MouseDown != MouseButtons.None)
            {
                if (m_MouseDown == MouseButtons.Right)
                {
                    if (m_UpsideDown)
                        xdelta = -xdelta;

                    m_CamRotation.X -= xdelta * 0.002f;
                    m_CamRotation.Y -= ydelta * 0.002f;

                    ClampRotation(ref m_CamRotation.X, (float)Math.PI * 2.0f);
                    ClampRotation(ref m_CamRotation.Y, (float)Math.PI * 2.0f);
                }
                else if (m_MouseDown == MouseButtons.Left/* && !m_ShiftPressed*/)
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

            glModelView.Refresh();
        }

        private void glModelView_MouseWheel(object sender, MouseEventArgs e)
        {
            float delta = -((e.Delta / 120.0f) * 0.1f);
            m_CamTarget.X += delta * (float)Math.Cos(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y);
            m_CamTarget.Y += delta * (float)Math.Sin(m_CamRotation.Y);
            m_CamTarget.Z += delta * (float)Math.Sin(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y);

            UpdateCamera();
            glModelView.Refresh();
        }

        private void glModelView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.RShiftKey)
                m_ShiftPressed = true;
        }

        private void glModelView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.RShiftKey)
                m_ShiftPressed = false;
        }

        private void ClampRotation(ref float val, float twopi)
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

            glModelView.Refresh();
        }

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
        private uint m_LastClicked;

        private uint[] m_PickingFrameBuffer;

        private bool m_ShiftPressed;

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (lbxPlanes.SelectedIndex > -1)
            {
                int lastChange;
                int.TryParse(txtColType.Text, out lastChange);
                m_Planes[lbxPlanes.SelectedIndex].type = lastChange;// Make sure to get value of current plane
            }
            WriteChanges();

            LoadKCL(m_KCL.m_File);
            glModelView.Refresh();
        }

        void txtColType_TextChanged(object sender, System.EventArgs e)
        {
            int newColType;
            int.TryParse(txtColType.Text, out newColType);
            foreach (int idx in lbxPlanes.SelectedIndices)
            {
                m_Planes[idx].type = newColType;
            }
        }

        private void lbxPlanes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxPlanes.SelectedIndices.Count == 1)
            {
                int selPos = lbxPlanes.SelectedIndex;

                txtV1.Text = m_Planes[selPos].point1.ToString();
                txtV2.Text = m_Planes[selPos].point2.ToString();
                txtV3.Text = m_Planes[selPos].point3.ToString();
                txtColType.Text = m_Planes[selPos].type.ToString();
                txtNormal.Text = m_Planes[selPos].normal.ToString();
                txtD1.Text = m_Planes[selPos].dir1.ToString();
                txtD2.Text = m_Planes[selPos].dir2.ToString();
                txtD3.Text = m_Planes[selPos].dir3.ToString();
            }
            RenderHighlight();
            glModelView.Refresh();
        }

        private void cmbPolygonMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPolygonMode.SelectedIndex == 0)
                m_WireFrameView = false;
            else if (cmbPolygonMode.SelectedIndex == 1)
                m_WireFrameView = true;

            glModelView.Refresh();
        }

        private void btnExportKCLModel_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveModel = new SaveFileDialog();
            saveModel.FileName = "CollisionMap";//Default name
            saveModel.DefaultExt = ".dae";//Default file extension
            saveModel.Filter = "COLLADA DAE (.dae)|*.dae|Wavefront OBJ (.obj)|*.obj";//Filter by .DAE and .OBJ
            if (saveModel.ShowDialog() == DialogResult.Cancel)
                return;

            BMD_BCA_KCLExporter.ExportKCLModel(m_KCL, saveModel.FileName);
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (var form = new ROMFileSelect("Please select a collision map (KCL) file to open."))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    LoadKCL((Program.m_ROM.GetFileFromName(form.m_SelectedFile)));
                    RenderKCLMesh();
                    GL.DeleteLists(m_KCLMeshDLists[3], 1); m_KCLMeshDLists[3] = 0;
                    glModelView.Refresh();
                }
            }
        }

        private void btnOpenModel_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = Helper.MODEL_FORMATS_FILTER;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtModelName.Text = ofd.FileName;
                m_MatColTypes = new KCLImporter().GetMaterialsList(ofd.FileName);
                if (Properties.Settings.Default.RememberMaterialCollisionTypeAssignments)
                {
                    string key = ofd.FileName;
                    if (m_SavedMaterialCollisionTypes.ContainsKey(key))
                    {
                        Dictionary<string, int> materialCollisionTypeAssignments = m_SavedMaterialCollisionTypes[key];
                        if (materialCollisionTypeAssignments.Keys.Count == m_MatColTypes.Keys.Count &&
                            materialCollisionTypeAssignments.Keys.Except(m_MatColTypes.Keys).Count() < 1)
                        {
                            m_MatColTypes = materialCollisionTypeAssignments;
                        }
                        else
                        {
                            m_SavedMaterialCollisionTypes.Remove(key);
                        }
                    }
                }
                PopulateColTypes();
            }
        }

        private void PopulateColTypes()
        {
            gridColTypes.ColumnCount = 2;
            gridColTypes.Columns[0].HeaderText = "Material";
            gridColTypes.Columns[0].ReadOnly = true;
            gridColTypes.Columns[1].HeaderText = "Col. Type";

            int numMats = m_MatColTypes.Count;
            gridColTypes.RowCount = numMats;
            for (int i = 0; i < numMats; i++)
            {
                gridColTypes.Rows[i].Cells[0].Value = m_MatColTypes.Keys.ElementAt(i);
                gridColTypes.Rows[i].Cells[1].Value = m_MatColTypes.Values.ElementAt(i);
            }
        }

        private void btnImportColMap_Click(object sender, EventArgs e)
        {
            float scale;
            if (!(float.TryParse(txtScale.Text, out scale) || float.TryParse(txtScale.Text, NumberStyles.Float, new CultureInfo("en-US"), out scale)))
            {
                MessageBox.Show("Please enter a valid float value for scale, eg. 1.23");
            }
            float faceSizeThreshold;
            if (!(float.TryParse(txtThreshold.Text, out faceSizeThreshold) || float.TryParse(txtThreshold.Text, NumberStyles.Float, new CultureInfo("en-US"), out faceSizeThreshold)))
            {
                MessageBox.Show("Please enter a valid float value, eg. 1.23");
            }

            try
            {
                new KCLImporter().ConvertModelToKCL(m_KCL.m_File, txtModelName.Text, scale, faceSizeThreshold, m_MatColTypes);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.Source + ex.StackTrace);
            }

            LoadKCL(m_KCL.m_File);
            RenderKCLMesh();
            GL.DeleteLists(m_KCLMeshDLists[3], 1); m_KCLMeshDLists[3] = 0;
            glModelView.Refresh();
        }

        private void btnAssignTypes_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gridColTypes.RowCount; i++)
            {
                m_MatColTypes[gridColTypes.Rows[i].Cells[0].Value.ToString()] = int.Parse(gridColTypes.Rows[i].Cells[1].Value.ToString());
            }
        }

    }
}
