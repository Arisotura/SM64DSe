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
    public partial class CodeCompilerForm : Form
    {
        public CodeCompilerForm()
        {
            InitializeComponent();
        }

        private void btnFolder_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.SelectedPath = System.IO.Path.GetDirectoryName(Program.m_ROMPath);
            folderBrowserDialog.ShowDialog();
            txtFolder.Text = folderBrowserDialog.SelectedPath;
        }

        private void btnCompile_Click(object sender, EventArgs e)
        {
            if (!Patcher.PatchMaker.PatchToSupportBigASMHacks())
                return;

            //code and patcher borrowed from NSMBe and edited.
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(txtFolder.Text);
            Patcher.PatchMaker pm = new Patcher.PatchMaker(dir,
                chkDynamic.Checked ? 0x02400000 :
                uint.Parse(txtOffset.Text, System.Globalization.NumberStyles.HexNumber));

            if (chkDynamic.Checked)
                pm.makeDynamicLibrary(txtFolder.Text + "/" + txtOutput.Text);
            else
            {
                pm.compilePatch();
                pm.generatePatch(txtFolder.Text + "/" + txtOutput.Text);
            }
        }

        private void chkDynamic_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDynamic.Checked)
                txtOffset.Enabled = false;
            else
                txtOffset.Enabled = true;
        }

        private void btnClean_Click(object sender, EventArgs e)
        {
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(txtFolder.Text);
            Patcher.PatchCompiler.cleanPatch(dir);
        }
    }
}
