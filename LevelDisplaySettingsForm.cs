using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SM64DSe
{
    public partial class LevelDisplaySettingsForm : Form
    {
        private LevelEditorForm m_Parent;

        public LevelDisplaySettingsForm(LevelEditorForm parent)
        {
            InitializeComponent();

            m_Parent = parent;
            checkTextures.Checked = m_Parent.LevelModelDisplayFlags.RenderTextures;
            checkVtxColors.Checked = m_Parent.LevelModelDisplayFlags.RenderVertexColours;
            checkWireframe.Checked = m_Parent.LevelModelDisplayFlags.RenderWireframe;
            checkPolylistTypes.Checked = m_Parent.LevelModelDisplayFlags.RenderPolyListType;
        }

        private void checkTextures_Click(object sender, EventArgs e)
        {
            m_Parent.LevelModelDisplayFlags.RenderTextures = ((CheckBox)sender).Checked;
            m_Parent.UpdateDisplayFlags();
        }

        private void checkVtxColors_Click(object sender, EventArgs e)
        {
            m_Parent.LevelModelDisplayFlags.RenderVertexColours = ((CheckBox)sender).Checked;
            m_Parent.UpdateDisplayFlags();
        }

        private void checkWireframe_Click(object sender, EventArgs e)
        {
            m_Parent.LevelModelDisplayFlags.RenderWireframe = ((CheckBox)sender).Checked;
            m_Parent.UpdateDisplayFlags();
        }

        private void checkPolylistTypes_Click(object sender, EventArgs e)
        {
            m_Parent.LevelModelDisplayFlags.RenderPolyListType = ((CheckBox)sender).Checked;
            m_Parent.UpdateDisplayFlags();
        }
    }
}
