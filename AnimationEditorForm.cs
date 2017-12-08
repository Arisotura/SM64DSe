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

        private int[] m_DisplayLists = new int[1];

        private BMDImporter.BCAImportationOptions m_BCAImportationOptions;

        private ROMFileSelect m_ROMFileSelect = new ROMFileSelect();

        public AnimationEditorForm()
        {
            InitializeComponent();
            InitTimer();
            glModelView.Initialise();
            glModelView.ProvideDisplayLists(m_DisplayLists);
            m_BCAImportationOptions = BMDImporter.BCAImportationOptions.DEFAULT;
        }

        private void PrerenderModel()
        {
            if (m_DisplayLists[0] == 0)
            {
                m_DisplayLists[0] = GL.GenLists(1);
            }
            GL.NewList(m_DisplayLists[0], ListMode.Compile);

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
            m_ROMFileSelect.ReInitialize("Please select a model (BMD) file to open.", new String[] { ".bmd" });
            var result = m_ROMFileSelect.ShowDialog();
            if (result == DialogResult.OK)
            {
                StopTimer();

                m_BMD = new BMD(Program.m_ROM.GetFileFromName(m_ROMFileSelect.m_SelectedFile));
                txtBMDName.Text = m_BMD.m_FileName;

                PrerenderModel();
            }
        }

        private void btnOpenBCA_Click(object sender, EventArgs e)
        {
            bool wasRunning = m_Running;

            m_ROMFileSelect.ReInitialize("Please select an animation (BCA) file to open.", new String[] { ".bca" });
            var result = m_ROMFileSelect.ShowDialog();
            if (result == DialogResult.OK)
            {
                StopTimer();

                m_BCA = new BCA(Program.m_ROM.GetFileFromName(m_ROMFileSelect.m_SelectedFile));
                txtBCAName.Text = m_BCA.m_FileName;

                m_AnimationFrameNumber = 0;
                m_AnimationNumFrames = m_BCA.m_NumFrames;
                txtCurrentFrameNum.Text = "" + m_AnimationFrameNumber;
                txtNumFrames.Text = "" + (m_BCA.m_NumFrames - 1);

                if (wasRunning)
                {
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
            catch { }
        }

        private void btnSelectInputAnimation_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = Strings.MODEL_ANIMATION_FORMATS_FILTER;

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
            ofd.Filter = Strings.MODEL_FORMATS_FILTER;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtInputModel.Text = ofd.FileName;
            }
        }

        private void chkOptimise_CheckedChanged(object sender, EventArgs e)
        {
            m_BCAImportationOptions.m_Optimise = chkOptimise.Checked;
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

            float scale;
            if (!Helper.TryParseFloat(txtScale.Text, out scale))
            {
                MessageBox.Show("Please enter a valid Scale value as a decimal value, eg. 1.234");
                return;
            }

            string animationFormat = txtInputAnimation.Text.Substring(txtInputAnimation.Text.Length - 3).ToLowerInvariant();

            bool wasRunning = m_Running;
            StopTimer();

            try
            {
                ModelBase loadedModel = null;
                switch (animationFormat)
                {
                    case "dae":
                        {
                            if (txtInputModel.Text != null && !txtInputModel.Text.Equals(""))
                            {
                                loadedModel = BMDImporter.LoadModel(txtInputAnimation.Text, scale);
                                m_BMD = BMDImporter.CallBMDWriter(ref m_BMD.m_File, loadedModel, 
                                    BMDImporter.BMDExtraImportOptions.DEFAULT, true);
                            }
                            // >>> TODO <<<
                            // Below line in necessary to an obscure bug with NARC files, if you have two file from the same 
                            // NARC open and modify and save the first, when you then go to save the second, it won't have 
                            // picked up the changes from the first file and when saved will write the original first file and 
                            // the modified second file.
                            NitroFile animationFile = Program.m_ROM.GetFileFromName(m_BCA.m_FileName);
                            m_BCA = BMDImporter.ConvertAnimatedDAEToBCA(ref animationFile, txtInputAnimation.Text, m_BCAImportationOptions, true);
                        }
                        break;
                    case "ica":
                        {
                            if (txtInputModel.Text != null && !txtInputModel.Text.Equals(""))
                            {
                                loadedModel = BMDImporter.LoadModel(txtInputModel.Text, scale);
                                m_BMD = BMDImporter.CallBMDWriter(ref m_BMD.m_File, loadedModel, 
                                    BMDImporter.BMDExtraImportOptions.DEFAULT, true);
                            }
                            NitroFile animationFile = Program.m_ROM.GetFileFromName(m_BCA.m_FileName);
                            m_BCA = BMDImporter.ConvertICAToBCA(ref animationFile, txtInputAnimation.Text, loadedModel,
                                scale, BMDImporter.BMDExtraImportOptions.DEFAULT, m_BCAImportationOptions, true);
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

        private void AnimationEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopTimer();
        }
    }
}
