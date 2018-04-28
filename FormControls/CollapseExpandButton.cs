using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SM64DSe.FormControls
{
    public class CollapseExpandButton : PictureBox
    {
        private static readonly int COLLAPSED_HEIGHT = 16;

        private Control m_Control;
        private int m_FullHeight;
        private int m_CollapsedHeight;

        private bool m_Initialised;
        private bool m_IsCollapsed;

        public delegate void OnClickFunction();

        private OnClickFunction m_OnClick;

        public CollapseExpandButton()
            : base()
        {
            this.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Image = SM64DSe.Properties.Resources.collapseButton;
            this.Size = new System.Drawing.Size(16, 16);
            this.TabStop = false;

            this.Click += new EventHandler(this.On_Click);
        }

        public void Initialise(Control control, OnClickFunction onClick)
        {
            m_Control = control;
            m_OnClick = onClick;
            m_FullHeight = m_Control.Height;
            m_CollapsedHeight = COLLAPSED_HEIGHT;
            m_IsCollapsed = false;

            m_Initialised = true;
        }

        protected virtual void On_Click(object sender, EventArgs e)
        {
            if (!m_Initialised)
            {
                throw new InvalidOperationException("CollapseExpandButton must be initialised with a Control");
            }

            if (m_IsCollapsed)
            {
                m_Control.Height = m_FullHeight;
                this.Image = SM64DSe.Properties.Resources.collapseButton;
            }
            else
            {
                m_Control.Height = m_CollapsedHeight;
                this.Image = SM64DSe.Properties.Resources.expandButton;
            }
            m_IsCollapsed = !m_IsCollapsed;

            if (m_OnClick != null)
            {
                m_OnClick.Invoke();
            }
        }
    }
}
