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
    public partial class ROMFileSelect : Form
    {
        public String m_SelectedFile = "";

        public ROMFileSelect()
        {
            InitializeComponent();

            LoadFileList(this.tvFiles);
        }

        public ROMFileSelect(String title)
        {
            InitializeComponent();
            this.Text = title;

            LoadFileList(this.tvFiles);
        }

        public static void LoadFileList(TreeView tvFileList)
        {
            NitroROM.FileEntry[] files = Program.m_ROM.GetFileEntries();
            tvFileList.Nodes.Add("root", "ROM File System");

            TreeNode node = tvFileList.Nodes["root"];

            LoadFiles(tvFileList, node, files, new NARC.FileEntry[] { });
        }

        private static void LoadFiles(TreeView tvFileList, TreeNode node, NitroROM.FileEntry[] files, NARC.FileEntry[] filesNARC)
        {
            TreeNode parent = node;
            String[] names = new String[0];
            if (files.Length == 0)
            {
                names = new String[filesNARC.Length];
                for (int i = 0; i < filesNARC.Length; i++)
                    names[i] = filesNARC[i].FullName;
            }
            else if (filesNARC.Length == 0)
            {
                names = new String[files.Length];
                for (int i = 0; i < files.Length; i++)
                    names[i] = files[i].FullName;
            }

            for (int i = 0; i < names.Length; i++)
            {
                String[] parts = names[i].Split('/');

                if (parts.Length == 1)
                {
                    /*if (parts[0].Equals(""))
                        tvFiles.Nodes["root"].Nodes.Add("Overlay");
                    else */if (!parts[0].Equals(""))
                        tvFileList.Nodes["root"].Nodes.Add(parts[0]).Tag = names[i];
                }
                else
                {
                    node = parent;

                    for (int j = 0; j < parts.Length; j++)
                    {
                        if (!node.Nodes.ContainsKey(parts[j]))
                            node.Nodes.Add(parts[j], parts[j]).Tag = names[i];
                        node = node.Nodes[parts[j]];

                        if (parts[j].EndsWith(".narc"))
                        {
                            LoadFiles(tvFileList, node, new NitroROM.FileEntry[] { }, 
                                new NARC(Program.m_ROM, Program.m_ROM.GetFileIDFromName(files[i].FullName)).GetFileEntries());
                        }
                    }
                }
            }
        }

        public static void LoadOverlayList(TreeView tvFileList)
        {
            NitroROM.FileEntry[] files = Program.m_ROM.GetFileEntries();
            TreeNode ovlNode = tvFileList.Nodes.Add("root", "ARM 9 Overlays");

            NitroROM.OverlayEntry[] ovls = Program.m_ROM.GetOverlayEntries();
            for (int i = 0; i < ovls.Length; i++)
            {
                string ind = String.Format("{0:D3}", i);
                ovlNode.Nodes.Add("Overlay_" + ind).Tag = "Overlay_" + ind;
            }
        }

        private void tvFiles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null || e.Node.Tag == null)
                m_SelectedFile = "";
            else
                m_SelectedFile = e.Node.Tag.ToString();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
