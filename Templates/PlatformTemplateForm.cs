using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using SM64DSe.SM64DSFormats;

namespace SM64DSe.Templates
{
    public partial class PlatformTemplateForm : Form
    {
        private CLPS m_CLPS;
        private ROMFileSelect m_ROMFileSelect;

        public PlatformTemplateForm()
        {
            InitializeComponent();
            txtFlags.Tag = (uint)2;

            m_CLPS = new CLPS(new List<CLPS.Entry>()
            {
                new CLPS.Entry(0x00, 0, 0x3f, 0x0, 0x0, 0x00, 0, 0, 0, 0xff)
            });

            m_ROMFileSelect = new ROMFileSelect();
        }

        private string FormatCLPS()
        {
            string res = "{";
            foreach (CLPS.Entry clps in m_CLPS)
            {
                res += string.Format(Environment.NewLine + "\tCLPS(0x{0:x2}, {1:x1}, 0x{2:x2}, 0x{3:x1}, 0x{4:x1}, " +
                    "0x{5:x2}, {6:x1}, {7:x1}, {8:x1}, 0x{9:x2}),",
                    clps.m_Texture,
                    clps.m_Water,
                    clps.m_ViewID,
                    clps.m_Traction,
                    clps.m_CamBehav,
                    clps.m_Behav,
                    clps.m_CamThrough,
                    clps.m_Toxic,
                    clps.m_Unk26,
                    clps.m_WindID);
            }
            return res.Remove(res.Length - 1) + Environment.NewLine + "}";
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (!CommonTemplate.ValidateIdentifier(txtFilename.Text, "filename")) { return; }
            if (!CommonTemplate.ValidateIdentifier(txtLevel.Text, "level filename")) { return; }
            if (txtHackPath.Text == "")
            {
                MessageBox.Show("ASMPatchTemplate path is empty.");
                return;
            }
            if (txtModelFile.Text == "")
            {
                MessageBox.Show("Model file path is empty.");
                return;
            }
            if (txtClsnFile.Text == "")
            {
                MessageBox.Show("Mesh collider file path is empty.");
                return;
            }
            DirectoryInfo source = new DirectoryInfo(txtHackPath.Text + @"/source");
            if (chkLevelSpecific.Checked)
            {
                source = new DirectoryInfo(source.FullName + "/L_" + txtLevel.Text);
                if (!source.Exists)
                {
                    MessageBox.Show("The level \"" + txtLevel.Text + "\" does not exist.");
                    return;
                }
                else if (!File.Exists(source.FullName + "/" + txtLevelMain.Text + ".cpp"))
                {
                    MessageBox.Show("The .cpp file \"" + txtLevel.Text + "\".cpp does not exist in " +
                        source.FullName + ".");
                    return;
                }
            }
            else
                source = source.CreateSubdirectory("P_" + txtFilename.Text);

            using (FileStream hFile = new FileStream("Templates/Platform.h", FileMode.Open),
                cppFile = new FileStream("Templates/Platform.cpp", FileMode.Open),
                newHFile = new FileStream(source.FullName + "/" + txtFilename.Text + ".h", FileMode.Create),
                newCppFile = new FileStream(source.FullName + "/" + txtFilename.Text + ".cpp", FileMode.Create))
            {
                StreamReader hFileR = new StreamReader(hFile),
                             cppFileR = new StreamReader(cppFile);
                StreamWriter newHFileW = new StreamWriter(newHFile),
                             newCppFileW = new StreamWriter(newCppFile);

                Dictionary<string, string> defs = new Dictionary<string, string>
                {
                    { "_NAME", txtFilename.Text.ToUpper() },
                    { "_Name", (chkLevelSpecific.Checked ? new Regex("[a-z]").Replace(txtLevel.Text, "") + "_" : "" ) + txtFilename.Text },
                    { "_FileName", txtFilename.Text },
                    { "_ObjectID", txtObjectID.Text },
                    { "_ActorID", txtActorID.Text },
                    { "_BehaviorPriority", txtBehavPrior.Text },
                    { "_RenderPriority", txtRenderPrior.Text },
                    { "_NumCLPSes", m_CLPS.Count.ToString() },
                    { "_CLPS", FormatCLPS() },
                    { "_ModelOv0ID", string.Format("0x{0,4:x4}", Program.m_ROM.GetFileEntries()[Program.m_ROM.GetFileIDFromName(txtModelFile.Text)].InternalID) },
                    { "_ClsnOv0ID", string.Format("0x{0,4:x4}", Program.m_ROM.GetFileEntries()[Program.m_ROM.GetFileIDFromName(txtClsnFile.Text)].InternalID) },
                    { "_Flags", string.Format("0x{0,8:x8}", (uint)txtFlags.Tag) },
                    { "_ClsnRangeOffsetY", string.Format("0x{0,8:x8}_f", (uint)(double.Parse(txtClsnRangeOffsetY.Text) * 4096000 + .5)) },
                    { "_ClsnRange", string.Format("0x{0,8:x8}_f", (uint)(double.Parse(txtClsnRange.Text) * 4096000 + .5)) },
                    { "_DrawDistance", string.Format("0x{0,8:x8}_f", (uint)(double.Parse(txtDrawDist.Text) * 4096000 + .5)) },
                    { "_ShadowDrawDistance", string.Format("0x{0,8:x8}_f", (uint)(double.Parse(txtShadowDrawDist.Text) * 4096000 + .5)) },
                    { "_ClsnScale", string.Format("0x{0:x}_f", (uint)(double.Parse(txtClsnScale.Text) * 4096 + .5)) }
                };
                if (chkCreateUpdateClsnFunc.Checked) defs.Add("_CreateUpdateClsnFunc", "");
                if (chkMoving.Checked) defs.Add("_Moving", "");
                if (chkLevelSpecific.Checked) defs.Add("_LevelSpecific", "");

                List<string> hFileLines = new List<string>();
                while (!hFileR.EndOfStream) { hFileLines.Add(hFileR.ReadLine()); }
                CommonTemplate.FillTemplate(hFileLines, defs);
                hFileLines.ForEach(x => newHFileW.WriteLine(x));
                newHFileW.Flush();

                List<string> cppFileLines = new List<string>();
                while (!cppFileR.EndOfStream) { cppFileLines.Add(cppFileR.ReadLine()); }
                CommonTemplate.FillTemplate(cppFileLines, defs);
                cppFileLines.ForEach(x => newCppFileW.WriteLine(x));
                newCppFileW.Flush();
            }

            if (chkLevelSpecific.Checked)
            {
                List<string> fileLines = new List<string>() { "#include \"" + txtFilename.Text + ".h\"" };
                using (FileStream file = new FileStream(source.FullName + "/" + txtLevelMain.Text + ".cpp", FileMode.Open))
                {
                    StreamReader fileR = new StreamReader(file);

                    fileLines = new List<string>() { "#include \"" + txtFilename.Text + ".h\"" };
                    while (!fileR.EndOfStream) { fileLines.Add(fileR.ReadLine()); }
                    int index = -1;
                    Regex initFunc = new Regex(@"\A\s*void\s+init\(\s*\)");
                    Regex leftBracket = new Regex(@"\A\s*\{");
                    for (int i = 0; i < fileLines.Count - 1; ++i)
                    {
                        if (initFunc.IsMatch(fileLines[i]) && leftBracket.IsMatch(fileLines[i + 1]))
                        {
                            index = i + 2;
                            break;
                        }
                    }

                    if (index == -1)
                    {
                        MessageBox.Show("Nowhere to write the init snippet for this object in " +
                            file.Name + ".");
                    }
                    else
                    {
                        fileLines.Insert(index, string.Format(
                            "\tOBJ_TO_ACTOR_ID_TABLE[{0}] = {1};" + Environment.NewLine +
                            "\tACTOR_SPAWN_TABLE[{1}] = (unsigned)&{2}_{3}::spawnData;" + Environment.NewLine +
                            "\t{2}_{3}::modelFile.Construct(0x{4,4:x4});" + Environment.NewLine +
                            "\t{2}_{3}::clsnFile .Construct(0x{5,4:x4});" + Environment.NewLine +
                            "\t",
                            txtObjectID.Text,
                            txtActorID.Text,
                            new Regex("[a-z]").Replace(txtLevel.Text, ""),
                            txtFilename.Text,
                            Program.m_ROM.GetFileEntries()[Program.m_ROM.GetFileIDFromName(txtModelFile.Text)].InternalID,
                            Program.m_ROM.GetFileEntries()[Program.m_ROM.GetFileIDFromName(txtClsnFile .Text)].InternalID));
                    }
                }

                using (FileStream file = new FileStream(source.FullName + "/" + txtLevelMain.Text + ".cpp", FileMode.Create))
                {
                    StreamWriter fileW = new StreamWriter(file);
                    fileLines.ForEach(x => fileW.WriteLine(x));
                    fileW.Flush();
                }
            }

            CommonTemplate.DocumentObject(
                txtName.Text,
                (chkLevelSpecific.Checked ? new Regex("[a-z]").Replace(txtLevel.Text, "") + "_" : "") + txtFilename.Text,
                1,
                ushort.Parse(txtObjectID.Text.Substring(2), System.Globalization.NumberStyles.HexNumber),
                ushort.Parse(txtActorID.Text.Substring(2), System.Globalization.NumberStyles.HexNumber),
                txtDescription.Text,
                chkLevelSpecific.Checked ? "7=" + txtBankValue.Text : "none");
        }

        private void btnHackPath_Click(object sender, EventArgs e)
        {
            txtHackPath.Text = CommonTemplate.ShowFolderDialog(txtHackPath.Text); 
        }

        private void txtUInt16_Validating(object sender, CancelEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string text = textBox.Text;
            if (text.StartsWith("0x"))
                text = text.Substring(2);
            ushort value;
            if (!ushort.TryParse(text, System.Globalization.NumberStyles.HexNumber, null, out value))
            {
                e.Cancel = true;
                return;
            }
            textBox.Text = string.Format("0x{0,4:x4}", value);
        }

        private void txtDouble_Validating(object sender, CancelEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string text = textBox.Text;
            double dummy;
            if (!double.TryParse(text, out dummy))
                e.Cancel = true;
        }

        private void btnModelFile_Click(object sender, EventArgs e)
        {
            m_ROMFileSelect.Text = "Select a BMD file";
            if (m_ROMFileSelect.ShowDialog() == DialogResult.OK && !m_ROMFileSelect.m_SelectedFile.EndsWith("/"))
            {
                txtModelFile.Text = m_ROMFileSelect.m_SelectedFile;
                btnImportModel.Enabled = true;
            }
        }

        private void btnClsnFile_Click(object sender, EventArgs e)
        {
            m_ROMFileSelect.Text = "Select a KCL file";
            if (m_ROMFileSelect.ShowDialog() == DialogResult.OK && !m_ROMFileSelect.m_SelectedFile.EndsWith("/"))
            {
                txtClsnFile.Text = m_ROMFileSelect.m_SelectedFile;
                btnImportClsn.Enabled = true;
            }
        }

        private void txtFlags_Validating(object sender, CancelEventArgs e)
        {
            CommonTemplate.ValidateFlags(sender, e);
        }

        private void txtFlags_Validated(object sender, EventArgs e)
        {
            uint flags = (uint)txtFlags.Tag;
            chkNoBehavior.Checked  = (flags & 1 << 0x00) != 0;
            chkNoRender.Checked    = (flags & 1 << 0x01) != 0;
            chkYoshiAimEgg.Checked = (flags & 1 << 0x1c) != 0;
        }

        private void chkNoBehavior_CheckedChanged(object sender, EventArgs e)
        {
            txtFlags.Tag = (uint)(((uint)txtFlags.Tag & ~((1 << 0x00))) | (uint)((chkNoBehavior.Checked ? 1 : 0) << 0x00));
            txtFlags.Text = CommonTemplate.FlagsToString((uint)txtFlags.Tag);
        }

        private void chkNoRender_CheckedChanged(object sender, EventArgs e)
        {
            txtFlags.Tag = (uint)((uint)txtFlags.Tag & ~(1 << 0x01) | (uint)(chkNoRender.Checked ? 1 : 0) << 0x01);
            txtFlags.Text = CommonTemplate.FlagsToString((uint)txtFlags.Tag);
        }

        private void chkYoshiAimEgg_CheckedChanged(object sender, EventArgs e)
        {
            txtFlags.Tag = (uint)((uint)txtFlags.Tag & ~(1 << 0x1c) | (uint)(chkYoshiAimEgg.Checked ? 1 : 0) << 0x1c);
            txtFlags.Text = CommonTemplate.FlagsToString((uint)txtFlags.Tag);
        }

        private void btnClps_Click(object sender, EventArgs e)
        {
            new CLPS_Form(m_CLPS).ShowDialog();
        }

        private void chkLevelSpecific_CheckedChanged(object sender, EventArgs e)
        {
            lblLevel.Enabled = txtLevel.Enabled =
                lblLevelMain.Enabled = txtLevelMain.Enabled =
                lblBankReq.Enabled =
                lblBank.Enabled = txtBank.Enabled =
                lblBankValue.Enabled = txtBankValue.Enabled = chkLevelSpecific.Checked;
        }

        private void btnImportModel_Click(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.UseSimpleModelAndCollisionMapImporters)
            {
                ModelAndCollisionMapEditor form =
                    new ModelAndCollisionMapEditor(txtModelFile.Text, null, 0.008f);
                form.ShowDialog();
            }
            else
            {
                ModelImporter form = new ModelImporter(txtModelFile.Text, null, 0.008f);
                form.ShowDialog();
            }
        }

        private void btnImportClsn_Click(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.UseSimpleModelAndCollisionMapImporters)
            {
                ModelAndCollisionMapEditor kclForm =
                    new ModelAndCollisionMapEditor(null, txtClsnFile.Text, 1f, ModelAndCollisionMapEditor.StartMode.CollisionMap);
                kclForm.ShowDialog();
            }
            else
            {
                new KCLEditorForm(new NitroFile(Program.m_ROM, Program.m_ROM.GetFileIDFromName(txtClsnFile.Text))).ShowDialog();
            }
        }

        private void txtBankValue_Validating(object sender, CancelEventArgs e)
        {
            uint bankValue;
            if (!uint.TryParse(txtBankValue.Text, out bankValue))
                e.Cancel = true;
        }
    }
}
