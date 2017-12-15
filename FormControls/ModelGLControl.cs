using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;

namespace SM64DSe.FormControls
{
    public class ModelGLControl : OpenTK.GLControl
    {
        // camera
        protected Vector2 m_CamRotation;
        protected Vector3 m_CamTarget;
        protected float m_CamDistance;
        protected Vector3 m_CamPosition;
        protected bool m_UpsideDown;
        protected Matrix4 m_CamMatrix;
        protected float m_PixelFactorX, m_PixelFactorY;

        protected Matrix4 m_ProjectionMatrix;

        // mouse
        protected MouseButtons m_MouseDown;
        protected Point m_LastMouseClick, m_LastMouseMove;
        protected Point m_MouseCoords;

        // display
        protected int[] m_DisplayLists;
        public delegate void CallListForDisplayLists();
        protected CallListForDisplayLists m_CallListForDisplayLists;

        protected bool m_GLLoaded;
        protected float m_AspectRatio;

        protected bool m_Initialised;

        public ModelGLControl()
            : base(new OpenTK.Graphics.GraphicsMode(new OpenTK.Graphics.ColorFormat(32), 24, 8))
        {
            this.BackColor = System.Drawing.Color.Black;
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Location = new System.Drawing.Point(0, 0);
            this.VSync = false;

            m_DisplayLists = new int[0];
            m_CallListForDisplayLists = CallListForDisplayLists_Default;
        }

        public void Initialise()
        {
            this.Load += new System.EventHandler(this.On_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.On_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.On_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.On_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.On_MouseUp);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.On_MouseWheel);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.On_KeyDown);
            this.Resize += new System.EventHandler(this.On_Resize);
          
            m_Initialised = true;
        }

        public void ProvideDisplayLists(int[] displayLists)
        {
            m_DisplayLists = displayLists;
        }

        public void ProvideCallListForDisplayLists(CallListForDisplayLists callListForDisplayLists)
        {
            m_CallListForDisplayLists = callListForDisplayLists;
        }

        public Point GetLastMouseClick()
        {
            return m_LastMouseClick;
        }

        public virtual void PrepareForClose()
        {
            foreach (int dl in m_DisplayLists)
            {
                GL.DeleteLists(dl, 1);
            }
        }

        protected virtual void On_Load(object sender, EventArgs e)
        {
            m_GLLoaded = true;

            GL.Viewport(ClientRectangle);

            float ratio = (float)Width / (float)Height;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            m_ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView((float)((70.0f * Math.PI) / 180.0f), ratio, 0.01f, 1000.0f);
            GL.MultMatrix(ref m_ProjectionMatrix);

            m_PixelFactorX = ((2f * (float)Math.Tan((35f * Math.PI) / 180f) * ratio) / (float)(Width));
            m_PixelFactorY = ((2f * (float)Math.Tan((35f * Math.PI) / 180f)) / (float)(Height));

            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Greater, 0f);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.Texture2D);

            GL.LineWidth(1.0f);

            m_CamRotation = new Vector2(0.0f, (float)Math.PI / 8.0f);
            m_CamTarget = new Vector3(0.0f, 0.0f, 0.0f);
            m_CamDistance = 1.0f;
            UpdateCamera();

            GL.ClearColor(Color.FromArgb(0, 0, 32));
        }

        protected virtual void On_MouseDown(object sender, MouseEventArgs e)
        {
            if (m_MouseDown != MouseButtons.None) return;
            m_MouseDown = e.Button;
            m_LastMouseClick = e.Location;
            m_LastMouseMove = e.Location;
        }

        protected virtual void On_MouseMove(object sender, MouseEventArgs e)
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
                    {
                        xdelta = -xdelta;
                    }

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

            Refresh();
        }

        protected virtual void On_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != m_MouseDown) return;
            m_MouseDown = MouseButtons.None;
        }

        protected virtual void On_MouseWheel(object sender, MouseEventArgs e)
        {
            float delta = -((e.Delta / 120.0f) * 0.1f);
            m_CamTarget.X += delta * (float)Math.Cos(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y);
            m_CamTarget.Y += delta * (float)Math.Sin(m_CamRotation.Y);
            m_CamTarget.Z += delta * (float)Math.Sin(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y);

            UpdateCamera();

            Refresh();
        }

        protected virtual void On_KeyDown(object sender, KeyEventArgs e)
        {
            float zoomMultiplier = e.Shift ? 4.0f : 1.0f;
            if (e.KeyCode == Keys.Home)
            {
                ZoomCamera(-0.5f * zoomMultiplier);
                Refresh();
            } 
            else if (e.KeyCode == Keys.End)
            {
                ZoomCamera(0.5f * zoomMultiplier);
                Refresh();
            }
        }

        protected virtual void On_Paint(object sender, PaintEventArgs e)
        {
            if (!m_GLLoaded) return;
            Context.MakeCurrent(WindowInfo);

            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref m_CamMatrix);

            GL.Flush();

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

            m_CallListForDisplayLists();

            SwapBuffers();
        }

        protected virtual void On_Resize(object sender, EventArgs e)
        {
            if (!m_GLLoaded) return;
            Context.MakeCurrent(WindowInfo);

            GL.Viewport(ClientRectangle);

            m_AspectRatio = (float)Width / (float)Height;
            GL.MatrixMode(MatrixMode.Projection);
            Matrix4 projmtx = Matrix4.CreatePerspectiveFieldOfView((float)(70f * Math.PI / 180f), m_AspectRatio, 0.01f, 1000f);
            GL.LoadMatrix(ref projmtx);
        }

        protected void CallListForDisplayLists_Default()
        {
            foreach (int displayList in m_DisplayLists)
            {
                GL.CallList(displayList);
            }
        }

        protected void UpdateCamera()
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

        protected void ZoomCamera(float delta)
        {
            m_CamTarget.X += delta * (float)Math.Cos(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y);
            m_CamTarget.Y += delta * (float)Math.Sin(m_CamRotation.Y);
            m_CamTarget.Z += delta * (float)Math.Sin(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y);

            UpdateCamera();
        }

        protected static void ClampRotation(ref float val, float twopi)
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
    }

    public class ModelGLControlWithPicking : ModelGLControl
    {
        // picking
        protected uint[] m_PickingFrameBuffer;
        protected float m_PickingDepth;
        private const float k_zNear = 0.01f;
        private const float k_zFar = 1000f;
        private const float k_FOV = (float)(70f * Math.PI) / 180f;

        // mouse
        protected uint m_UnderCursor;

        // display
        protected int[] m_PickingDisplayLists;
        protected CallListForDisplayLists m_CallListForPickingDisplayLists;

        public ModelGLControlWithPicking()
            : base()
        {
            m_PickingDisplayLists = new int[1];
            m_CallListForPickingDisplayLists = CallListForPickingDisplayLists_Default;
        }

        public void ProvidePickingDisplayLists(int[] pickingDisplayLists)
        {
            m_PickingDisplayLists = pickingDisplayLists;
        }

        public void ProvideCallListForPickingDisplayLists(CallListForDisplayLists callListForPickingDisplayLists)
        {
            m_CallListForPickingDisplayLists = callListForPickingDisplayLists;
        }

        public uint GetColourUnderCursor()
        {
            return m_UnderCursor;
        }

        public override void PrepareForClose()
        {
            base.PrepareForClose();
            foreach (int dl in m_PickingDisplayLists)
            {
                GL.DeleteLists(dl, 1);
            }
        }

        protected override void On_Load(object sender, EventArgs e)
        {
            m_PickingFrameBuffer = new uint[9];
            m_PickingDepth = 0f;

            base.On_Load(sender, e);
        }

        protected override void On_MouseDown(object sender, MouseEventArgs e)
        {
            if (m_MouseDown != MouseButtons.None) return;
            
            base.On_MouseDown(sender, e);

            if ((m_PickingFrameBuffer[4] == m_PickingFrameBuffer[1]) &&
                (m_PickingFrameBuffer[4] == m_PickingFrameBuffer[3]) &&
                (m_PickingFrameBuffer[4] == m_PickingFrameBuffer[5]) &&
                (m_PickingFrameBuffer[4] == m_PickingFrameBuffer[7]))
            {
                m_UnderCursor = m_PickingFrameBuffer[4];
            }
            else
            {
                m_UnderCursor = 0xFFFFFFFF;
            }
        }

        protected override void On_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != m_MouseDown) return;

            base.On_MouseUp(sender, e);

            m_UnderCursor = 0xFFFFFFFF;
        }

        protected override void On_Paint(object sender, PaintEventArgs e)
        {
            if (!m_GLLoaded) return;
            Context.MakeCurrent(WindowInfo);

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

            m_CallListForPickingDisplayLists();

            GL.Flush();
            GL.ReadPixels(m_MouseCoords.X - 1, Height - m_MouseCoords.Y + 1, 3, 3, PixelFormat.Bgra, PixelType.UnsignedByte, m_PickingFrameBuffer);
            // depth math from http://www.opengl.org/resources/faq/technical/depthbuffer.htm
            GL.ReadPixels(m_MouseCoords.X, Height - m_MouseCoords.Y, 1, 1, PixelFormat.DepthComponent, PixelType.Float, ref m_PickingDepth);
            m_PickingDepth = -(k_zFar * k_zNear / (m_PickingDepth * (k_zFar - k_zNear) - k_zFar));

            base.On_Paint(sender, e);
        }

        protected void CallListForPickingDisplayLists_Default()
        {
            foreach (int pickingDisplayList in m_PickingDisplayLists)
            {
                GL.CallList(pickingDisplayList);
            }
        }
    }

    public class ModelGLControlWithMarioSizeReference : ModelGLControlWithPicking
    {
        protected BMD m_MarioHeadModel;
        protected BMD m_MarioBodyModel;

        protected Vector3 m_MarioPosition;
        protected float m_MarioRotation;
        protected bool m_ShowMarioReference;

        protected int m_DisplayListMario;

        protected float m_Scale = 1f;

        public ModelGLControlWithMarioSizeReference() 
            : base() 
        {
            m_CallListForDisplayLists = CallListForDisplayListsPlusMario;
        }

        public void ProvideScaleRef(ref float scale)
        {
            m_Scale = scale;
        }

        public void SetShowMarioReference(bool showMarioReference)
        {
            m_ShowMarioReference = showMarioReference;
        }

        public override void PrepareForClose()
        {
            base.PrepareForClose();

            GL.DeleteLists(m_DisplayListMario, 1);

            if (m_MarioHeadModel != null) ModelCache.RemoveModel(m_MarioHeadModel);
            if (m_MarioBodyModel != null) ModelCache.RemoveModel(m_MarioBodyModel);
        }

        protected override void On_Load(object sender, EventArgs e)
        {
            m_MarioHeadModel = ModelCache.GetModel("data/player/mario_head_cap.bmd");
            m_MarioBodyModel = ModelCache.GetModel("data/player/mario_model.bmd");
            PrerenderMarioModel();

            base.On_Load(sender, e);
        }

        protected override void On_MouseMove(object sender, MouseEventArgs e)
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
                        objz /= m_Scale;

                        xdelta *= m_PixelFactorX * objz;
                        ydelta *= -m_PixelFactorY * objz;

                        float _xdelta = (xdelta * (float)Math.Sin(m_CamRotation.X)) - (ydelta * (float)Math.Sin(m_CamRotation.Y) * (float)Math.Cos(m_CamRotation.X));
                        float _ydelta = ydelta * (float)Math.Cos(m_CamRotation.Y);
                        float _zdelta = (xdelta * (float)Math.Cos(m_CamRotation.X)) + (ydelta * (float)Math.Sin(m_CamRotation.Y) * (float)Math.Sin(m_CamRotation.X));

                        Vector3 offset = new Vector3(_xdelta, _ydelta, -_zdelta);
                        Vector3.Add(ref m_MarioPosition, ref offset, out m_MarioPosition);
                    }

                    PrerenderMarioModel();
                }
                else
                {
                    if (m_MouseDown == MouseButtons.Right)
                    {
                        if (m_UpsideDown)
                        {
                            xdelta = -xdelta;
                        }

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

            Refresh();
        }

        protected override void On_MouseWheel(object sender, MouseEventArgs e)
        {
            if ((m_MouseDown == MouseButtons.Left) && (m_UnderCursor == 0x66666666))
            {
                float delta = -(e.Delta / 120f);
                delta = ((delta < 0f) ? -1f : 1f) * (float)Math.Pow(delta, 2f) * 0.05f;
                delta /= m_Scale;

                Vector3 offset = Vector3.Zero;
                offset.X += delta * (float)Math.Cos(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y);
                offset.Y += delta * (float)Math.Sin(m_CamRotation.Y);
                offset.Z += delta * (float)Math.Sin(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y);

                float xdist = delta * (m_MouseCoords.X - (Width / 2f)) * m_PixelFactorX;
                float ydist = delta * (m_MouseCoords.Y - (Height / 2f)) * m_PixelFactorY;

                offset.X -= (xdist * (float)Math.Sin(m_CamRotation.X)) + (ydist * (float)Math.Sin(m_CamRotation.Y) * (float)Math.Cos(m_CamRotation.X));
                offset.Y += ydist * (float)Math.Cos(m_CamRotation.Y);
                offset.Z += (xdist * (float)Math.Cos(m_CamRotation.X)) - (ydist * (float)Math.Sin(m_CamRotation.Y) * (float)Math.Sin(m_CamRotation.X));

                Vector3.Add(ref m_MarioPosition, ref offset, out m_MarioPosition);

                PrerenderMarioModel();

                Refresh();
            }
            else
            {
                base.On_MouseWheel(sender, e);
            }
        }

        protected void CallListForDisplayListsPlusMario()
        {
            foreach (int displayList in m_DisplayLists)
            {
                if (displayList > 0)
                {
                    GL.CallList(displayList);
                }
            }
            if (m_ShowMarioReference)
            {
                GL.CallList(m_DisplayListMario);
            }
        }

        protected void PrerenderMarioModel()
        {
            int[] mheaddl = ModelCache.GetDisplayLists(m_MarioHeadModel);
            int[] mbodydl = ModelCache.GetDisplayLists(m_MarioBodyModel);

            Vector3 mariopos = Vector3.Multiply(m_MarioPosition, m_Scale);

            if (m_DisplayListMario == 0)
            {
                m_DisplayListMario = GL.GenLists(1);
            }
            GL.NewList(m_DisplayListMario, ListMode.Compile);

            GL.FrontFace(FrontFaceDirection.Ccw);

            GL.Disable(EnableCap.Lighting);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Begin(PrimitiveType.Lines);
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
            GL.Begin(PrimitiveType.Lines);
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

            GL.EndList();

            if (m_PickingDisplayLists[0] == 0)
            {
                m_PickingDisplayLists[0] = GL.GenLists(1);
            }
            GL.NewList(m_PickingDisplayLists[0], ListMode.Compile);

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
    }
}
