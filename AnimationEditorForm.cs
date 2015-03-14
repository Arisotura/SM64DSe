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
using SM64DSe.ImportExport;

namespace SM64DSe
{
    public partial class AnimationEditorForm : Form
    {
        private BMD m_BMD;
        private BCA m_BCA;

        private System.Windows.Forms.Timer m_AnimationTimer;
        private int m_AnimationFrameNumber = 0;
        private int m_AnimationNumFrames = -1;
        private bool m_LoopAnimation = true;
        private bool m_Running = false;

        public AnimationEditorForm()
        {
            InitializeComponent();
            InitTimer();
        }

        private void glModelView_Load(object sender, EventArgs e)
        {
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
        }

        private void glModelView_MouseDown(object sender, MouseEventArgs e)
        {
            if (m_MouseDown != MouseButtons.None) return;
            m_MouseDown = e.Button;
            m_LastMouseClick = e.Location;
            m_LastMouseMove = e.Location;
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

            glModelView.Refresh();
        }

        private void glModelView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != m_MouseDown) return;
            m_MouseDown = MouseButtons.None;
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

        private void glModelView_Resize(object sender, EventArgs e)
        {
            if (!m_GLLoaded) return;
            glModelView.Context.MakeCurrent(glModelView.WindowInfo);

            GL.Viewport(glModelView.ClientRectangle);

            m_AspectRatio = (float)glModelView.Width / (float)glModelView.Height;
            GL.MatrixMode(MatrixMode.Projection);
            Matrix4 projmtx = Matrix4.CreatePerspectiveFieldOfView((float)(70f * Math.PI / 180f), m_AspectRatio, 0.01f, 1000f);
            GL.LoadMatrix(ref projmtx);
        }

        private void glModelView_Paint(object sender, PaintEventArgs e)
        {
            if (!m_GLLoaded) return;
            glModelView.Context.MakeCurrent(glModelView.WindowInfo);

            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref m_CamMatrix);

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

            GL.CallList(m_DisplayList);

            glModelView.SwapBuffers();
        }

        private bool m_GLLoaded;
        private float m_AspectRatio;

        private int m_DisplayList;
        private uint[] m_PickingFrameBuffer;

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

            Vector3.Add(ref m_CamPosition, ref m_CamTarget, out m_CamPosition);

            m_CamMatrix = Matrix4.LookAt(m_CamPosition, m_CamTarget, up);
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

        private void PrerenderModel()
        {
            if (m_DisplayList == 0)
                m_DisplayList = GL.GenLists(1);
            GL.NewList(m_DisplayList, ListMode.Compile);

            GL.FrontFace(FrontFaceDirection.Ccw);

            GL.Disable(EnableCap.Lighting);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.Disable(EnableCap.Lighting);
            GL.PushMatrix();
            
            GL.Scale(1f, 1f, 1f);
            GL.FrontFace(FrontFaceDirection.Ccw);

            m_BMD.PrepareToRender();

            int[] dl = new int[2];

            dl[0] = GL.GenLists(1);
            GL.NewList(dl[0], ListMode.Compile);
            m_BMD.Render(RenderMode.Opaque, 1f, m_BCA, m_AnimationFrameNumber);
            //GL.EndList();

            dl[1] = GL.GenLists(1);
            GL.NewList(dl[1], ListMode.Compile);
            m_BMD.Render(RenderMode.Translucent, 1f, m_BCA, m_AnimationFrameNumber);
            GL.EndList();

            GL.PopMatrix();

            glModelView.Refresh();
        }

        private void InitTimer()
        {
            m_AnimationTimer = new System.Windows.Forms.Timer();
            m_AnimationTimer.Interval = (int)(1000f / 30f);
            m_AnimationTimer.Tick += new EventHandler(m_AnimationTimer_Tick);
        }

        private void StartTimer()
        {
            m_AnimationFrameNumber = 0;
            m_AnimationTimer.Start();
            m_Running = true;
        }

        private void StopTimer()
        {
            m_AnimationTimer.Stop();
            m_Running = false;
        }

        private void m_AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (m_AnimationFrameNumber < m_AnimationNumFrames - 1)
            {
                IncrementFrame();
            }
            else
            {
                StopTimer();

                if (m_LoopAnimation)
                {
                    StartTimer();
                }
            }
        }

        private void IncrementFrame()
        {
            if (m_AnimationFrameNumber < m_AnimationNumFrames - 1)
                SetFrame(++m_AnimationFrameNumber);
            else
                SetFrame(0);
        }

        private void SetFrame(int frame)
        {
            m_AnimationFrameNumber = frame;

            PrerenderModel();

            txtCurrentFrameNum.Text = "" + m_AnimationFrameNumber;
        }

        private void btnOpenBMD_Click(object sender, EventArgs e)
        {
            using (var form = new ROMFileSelect("Please select a model (BMD) file to open."))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    StopTimer();

                    m_BMD = new BMD(Program.m_ROM.GetFileFromName(form.m_SelectedFile));
                    txtBMDName.Text = m_BMD.m_FileName;
                    
                    PrerenderModel();
                }
            }
        }

        private void btnOpenBCA_Click(object sender, EventArgs e)
        {
            bool wasRunning = m_Running;
            using (var form = new ROMFileSelect("Please select an animation (BCA) file to open."))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    StopTimer();

                    m_BCA = new BCA(Program.m_ROM.GetFileFromName(form.m_SelectedFile));
                    txtBCAName.Text = m_BCA.m_FileName;
                    
                    m_AnimationFrameNumber = 0;
                    m_AnimationNumFrames = m_BCA.m_NumFrames;
                    txtCurrentFrameNum.Text = "" + m_AnimationFrameNumber;
                    txtNumFrames.Text = "" + (m_BCA.m_NumFrames - 1);

                    if (wasRunning)
                        StartTimer();
                }
            }
        }

        private void btnPlayAnimation_Click(object sender, EventArgs e)
        {
            if (m_BMD == null || m_BCA == null)
                return;

            StartTimer();
        }

        private void chkLoopAnimation_CheckedChanged(object sender, EventArgs e)
        {
            m_LoopAnimation = !m_LoopAnimation;
        }

        private void btnStopAnimation_Click(object sender, EventArgs e)
        {
            StopTimer();
        }

        private void btnFirstFrame_Click(object sender, EventArgs e)
        {
            SetFrame(0);
        }

        private void btnLastFrame_Click(object sender, EventArgs e)
        {
            SetFrame(m_AnimationNumFrames - 1);
        }

        private void btnPreviousFrame_Click(object sender, EventArgs e)
        {
            if (m_AnimationFrameNumber > 0)
                SetFrame(--m_AnimationFrameNumber);
            else
                SetFrame((m_AnimationFrameNumber = m_AnimationNumFrames  - 1));
        }

        private void btnNextFrame_Click(object sender, EventArgs e)
        {
            if (m_AnimationFrameNumber == m_AnimationNumFrames - 1 && !m_LoopAnimation)
                return;
            else
                IncrementFrame();
        }

        private void btnExportToDAE_Click(object sender, EventArgs e)
        {
            if (m_BMD == null)
                return;

            SaveFileDialog saveModel = new SaveFileDialog();
            saveModel.FileName = "SM64DS_Animated_Model_" + 
                m_BMD.m_FileName.Substring(m_BMD.m_FileName.LastIndexOf("/") + 1) + ".DAE";//Default name
            saveModel.DefaultExt = ".dae";//Default file extension
            saveModel.Filter = "COLLADA DAE (.dae)|*.dae";//Filter by .DAE
            if (saveModel.ShowDialog() == DialogResult.Cancel)
                return;

            if (m_BCA != null)
                BMD_BCA_KCLExporter.ExportAnimatedModel(new BMD(m_BMD.m_File), new BCA(m_BCA.m_File), saveModel.FileName);
            else
                BMD_BCA_KCLExporter.ExportBMDModel(new BMD(m_BMD.m_File), saveModel.FileName);
        }

        private void txtCurrentFrameNum_TextChanged(object sender, EventArgs e)
        {
            if (txtCurrentFrameNum.Text == null || txtCurrentFrameNum.Text.Equals(""))
                return;

            try
            {
                int goFrame = int.Parse(txtCurrentFrameNum.Text);
                if (goFrame > -1 && goFrame < m_AnimationNumFrames - 1)
                {
                    SetFrame(goFrame);
                }
            }
            catch (Exception ex) { }
        }

        private void btnSelectInputAnimation_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = Helper.MODEL_ANIMATION_FORMATS_FILTER;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtInputAnimation.Text = ofd.FileName;
                string modelFormat = ofd.FileName.Substring(ofd.FileName.Length - 3, 3).ToLower();
                if (modelFormat.Equals("dae"))
                    txtInputModel.Text = ofd.FileName;
            }
        }

        private void btnSelectInputModel_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = Helper.MODEL_FORMATS_FILTER;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtInputModel.Text = ofd.FileName;
            }
        }

        private void btnImportAnimation_Click(object sender, EventArgs e)
        {
            if (m_BMD == null || m_BCA == null)
            {
                MessageBox.Show("Please select a valid model (BMD) and animation (BCA) to replace.");
                return;
            }
            if (txtInputAnimation.Text == null || txtInputAnimation.Text.Equals(""))
            {
                MessageBox.Show("Please select an animation file or model to import.");
                return;
            }

            Vector3 scale;
            try
            {
                float val = float.Parse(txtScale.Text, Helper.USA);
                scale = new Vector3(val, val, val);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please enter a valid Scale value as a decimal value, eg. 1.234");
                return;
            }

            string animationFormat = txtInputAnimation.Text.Substring(txtInputAnimation.Text.Length - 3).ToLowerInvariant();

            bool wasRunning = m_Running;
            StopTimer();

            BMDImporter importer = new BMDImporter();

            try
            {
                switch (animationFormat)
                {
                    case "dae":
                        {
                            if (txtInputModel.Text != null && !txtInputModel.Text.Equals(""))
                                m_BMD = importer.ConvertDAEToBMD(ref m_BMD.m_File, txtInputAnimation.Text, scale, 
                                    BMDImporter.BMDExtraImportOptions.DEFAULT, true);
                            // >>> TODO <<<
                            // Below line in necessary to an obscure bug with NARC files, if you have two file from the same 
                            // NARC open and modify and save the first, when you then go to save the second, it won't have 
                            // picked up the changes from the first file and when saved will write the original first file and 
                            // the modified second file.
                            NitroFile animationFile = Program.m_ROM.GetFileFromName(m_BCA.m_FileName);
                            m_BCA = importer.ConvertAnimatedDAEToBCA(ref animationFile, txtInputAnimation.Text, true);
                        }
                        break;
                    case "ica":
                        {
                            if (txtInputModel.Text != null && !txtInputModel.Text.Equals(""))
                                m_BMD = importer.ConvertIMDToBMD(ref m_BMD.m_File, txtInputModel.Text, scale,
                                    BMDImporter.BMDExtraImportOptions.DEFAULT, true);
                            NitroFile animationFile = Program.m_ROM.GetFileFromName(m_BCA.m_FileName);
                            m_BCA = importer.ConvertICAToBCA(ref animationFile, txtInputAnimation.Text, scale,
                                    BMDImporter.BMDExtraImportOptions.DEFAULT, true);
                        }
                        break;
                }

                m_AnimationFrameNumber = 0;
                m_AnimationNumFrames = m_BCA.m_NumFrames;
                txtCurrentFrameNum.Text = "" + m_AnimationFrameNumber;
                txtNumFrames.Text = "" + (m_BCA.m_NumFrames - 1);

                PrerenderModel();

                if (wasRunning) StartTimer();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: \n" + ex.Message + "\n\n" + ex.StackTrace);
                m_BMD = new BMD(Program.m_ROM.GetFileFromName(m_BMD.m_FileName));
                m_BCA = new BCA(Program.m_ROM.GetFileFromName(m_BCA.m_FileName));
            }
        }
    }
}
