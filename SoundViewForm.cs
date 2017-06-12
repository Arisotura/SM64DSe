using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SM64DSe.SM64DSFormats;

namespace SM64DSe
{
    public partial class SoundViewForm : Form
    {
        public SoundViewForm()
        {
            InitializeComponent();
            m_SoundData = new SM64DSFormats.SDAT(new NitroFile(Program.m_ROM,
                Program.m_ROM.GetFileIDFromInternalID(0x0587)));

            foreach (SWAR waveArc in m_SoundData.m_WaveArcs)
            {
                TreeNode node = tvSWAR.Nodes.Add(waveArc.m_Name, waveArc.m_Name);
                for (int i = 0; i < waveArc.m_Waves.Length; ++i)
                {
                    node.Nodes.Add("Wave " + i);
                }
            }
        }

        private SM64DSFormats.SDAT m_SoundData;

        private void PlayWave()
        {
            TreeNode node = tvSWAR.SelectedNode;
            if (node.Parent != null)
            {
                AudioPlaybackEngine.Instance.PlaySound(
                    m_SoundData.m_WaveArcs[node.Parent.Index].m_Waves[
                        node.Index]);
            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            PlayWave();
        }

        private void tvSWAR_DoubleClick(object sender, EventArgs e)
        {
            PlayWave();
        }

        private void tvSWAR_AfterSelect(object sender, TreeViewEventArgs e)
        {
            btnPlay.Enabled = tvSWAR.SelectedNode != null &&
                tvSWAR.SelectedNode.Parent != null;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            AudioPlaybackEngine.Instance.StopSound();
        }

        private void SoundViewForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            AudioPlaybackEngine.Instance.StopSound();
        }
    }
}
