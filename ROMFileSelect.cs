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
            TreeNode node = tvFileList.Nodes.Add("root", "ROM File System");

            EnsureAllDirsExist(tvFileList); //just in case a directory doesn't have files
            LoadFiles(tvFileList, node, files, new NARC.FileEntry[] { });

            node.Expand();
        }

        public static void EnsureAllDirsExist(TreeView tvFileList)
        {
            NitroROM.DirEntry[] dirs = Program.m_ROM.GetDirEntries();

            for (int i = 1; i < dirs.Length; ++i)
                EnsureDirExists(dirs[i].FullName, dirs[i].FullName, tvFileList.Nodes["root"]);
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
                        {
                            string dirName = "";
                            for (int k = 0; k <= j; ++k)
                                dirName += parts[k] + "/";
                            if (j == parts.Length - 1)
                                dirName = dirName.Substring(0, dirName.Length - 1);

                            node.Nodes.Add(parts[j], parts[j]).Tag = dirName;
                        }
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

            ovlNode.Expand();
        }

        public static void SelectFileOrDirHelper(string fullName, string fullNamePart,
            TreeNode parent)
        {
            int strEnd = fullNamePart.IndexOf('/');
            string strToMatch = strEnd != -1 ? fullNamePart.Substring(0, strEnd) : fullNamePart;

            foreach (TreeNode node in parent.Nodes)
            {
                if (node.Name == strToMatch)
                {
                    if (strEnd == -1)
                    {
                        node.EnsureVisible();
                        node.TreeView.SelectedNode = node;
                        break;
                    }
                    else
                        SelectFileOrDirHelper(fullName, fullNamePart.Substring(strEnd + 1), node);
                }
            }
        }

        public static void EnsureDirExists(string fullName, string fullNamePart,
            TreeNode parent)
        {
            int strEnd = fullNamePart.IndexOf('/');
            string strToMatch = strEnd != -1 ? fullNamePart.Substring(0, strEnd) : fullNamePart;

            foreach (TreeNode node in parent.Nodes)
            {
                if (node.Name == strToMatch)
                {
                    if (strEnd != -1)
                        EnsureDirExists(fullName, fullNamePart.Substring(strEnd + 1), node);

                    return;
                }
            }

            TreeNode newNode = parent.Nodes.Add(strToMatch, strToMatch);
            newNode.Tag = parent.Tag + strToMatch + "/";
            if(strEnd != -1)
                EnsureDirExists(fullName, fullNamePart.Substring(strEnd + 1), newNode);
        }

        public static TreeNode GetFileOrDirHelper(string fullName, string fullNamePart,
            TreeNode parent)
        {
            int strEnd = fullNamePart.IndexOf('/');
            string strToMatch = strEnd != -1 ? fullNamePart.Substring(0, strEnd) : fullNamePart;

            foreach (TreeNode node in parent.Nodes)
            {
                if (node.Name == strToMatch)
                {
                    if (strEnd == -1)
                        return node;
                    else
                        return GetFileOrDirHelper(fullName, fullNamePart.Substring(strEnd + 1), node);
                }
            }

            return null;
        }

        public static TreeNode GetFileOrDir(string fullName, TreeNode root)
        {
            return GetFileOrDirHelper(fullName, fullName, root);
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
