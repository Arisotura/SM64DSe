using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace SM64DSe
{
    public partial class AdditionalPatchesForm : Form
    {
        List<Patch> m_Patches = new List<Patch>();

        int editingIndex = -1;// Used to differentiate between editing and creating a new patch

        static string nl = Environment.NewLine;
        
        public AdditionalPatchesForm()
        {
            InitializeComponent();

            m_Patches = Patch.XMLToPatch();

            populatePatchListTable();

            clearEditData();
        }

        private void populatePatchListTable()
        {
            // Start writing to file
            Program.m_ROM.BeginRW();

            int numPatches = m_Patches.Count;

            gridPatches.ColumnCount = 6;
            gridPatches.Columns[0].HeaderText = "Name / Description";
            gridPatches.Columns[0].Width = 250;
            gridPatches.Columns[1].HeaderText = "Applied";
            gridPatches.Columns[2].HeaderText = "EUR Compatible";
            gridPatches.Columns[3].HeaderText = "US v1 Compatible";
            gridPatches.Columns[4].HeaderText = "US v2 Compatible";
            gridPatches.Columns[5].HeaderText = "JAP Compatible";
            gridPatches.RowCount = numPatches;

            for (int i = 0; i < m_Patches.Count; i++)
            {
                gridPatches.Rows[i].Cells[0].Value = m_Patches[i].m_PatchName;
                gridPatches.Rows[i].Cells[1].Value = m_Patches[i].CheckIsApplied(Program.m_ROM);
                gridPatches.Rows[i].Cells[2].Value = m_Patches[i].m_VersionSupport[0];
                gridPatches.Rows[i].Cells[3].Value = m_Patches[i].m_VersionSupport[1];
                gridPatches.Rows[i].Cells[4].Value = m_Patches[i].m_VersionSupport[2];
                gridPatches.Rows[i].Cells[5].Value = m_Patches[i].m_VersionSupport[3];
            }

            Program.m_ROM.EndRW();
        }

        private void AdditionalPatchesForm_Load(object sender, EventArgs e)
        {

        }

        private void btnApplyPatch_Click(object sender, EventArgs e)
        {
            if (gridPatches.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a patch to apply.");
                return;
            }

            Program.m_ROM.BeginRW();

            try
            {
                m_Patches[gridPatches.SelectedRows[0].Index].ApplyPatch(Program.m_ROM);
                MessageBox.Show("Success");
                // Restore Data is generated on patch application, so reload patches
                Patch.PatchToXML(m_Patches);
                m_Patches = Patch.XMLToPatch();
                RefreshRow(gridPatches.SelectedRows[0].Index);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong: \n\n" + ex.Message);
            }

            Program.m_ROM.EndRW();
        }

        private void btnUndoPatch_Click(object sender, EventArgs e)
        {
            if (gridPatches.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a patch to undo.");
                return;
            }

            Program.m_ROM.BeginRW();

            try
            {
                m_Patches[gridPatches.SelectedRows[0].Index].RemovePatch(Program.m_ROM);
                MessageBox.Show("Success");
                RefreshRow(gridPatches.SelectedRows[0].Index);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong: \n\n" + ex.Message + "\n" + ex.Data + "\n" + ex.StackTrace + "\n" + ex.Source);
            }

            Program.m_ROM.EndRW();
        }

        private void RefreshRow(int index)
        {
            Program.m_ROM.BeginRW();

            gridPatches.Rows[index].Cells[0].Value = m_Patches[index].m_PatchName;
            gridPatches.Rows[index].Cells[1].Value = m_Patches[index].CheckIsApplied(Program.m_ROM);
            gridPatches.Rows[index].Cells[2].Value = m_Patches[index].m_VersionSupport[0];
            gridPatches.Rows[index].Cells[3].Value = m_Patches[index].m_VersionSupport[1];
            gridPatches.Rows[index].Cells[4].Value = m_Patches[index].m_VersionSupport[2];
            gridPatches.Rows[index].Cells[5].Value = m_Patches[index].m_VersionSupport[3];

            Program.m_ROM.EndRW();
        }

        private void btnEditPatch_Click(object sender, EventArgs e)
        {
            if (gridPatches.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a patch to edit.");
                return;
            }

            editingIndex = gridPatches.SelectedRows[0].Index;
            clearEditData();

            txtPatchName.Text = m_Patches[gridPatches.SelectedRows[0].Index].m_PatchName;
            txtAuthor.Text = m_Patches[gridPatches.SelectedRows[0].Index].m_Author;

            // Display patch data
            DisplayPatchData("EUR", m_Patches[gridPatches.SelectedRows[0].Index].m_EURPatch);
            DisplayPatchData("USv1", m_Patches[gridPatches.SelectedRows[0].Index].m_USv1Patch);
            DisplayPatchData("USv2", m_Patches[gridPatches.SelectedRows[0].Index].m_USv2Patch);
            DisplayPatchData("JAP", m_Patches[gridPatches.SelectedRows[0].Index].m_JAPPatch);
            
            chkApplyToFile.Checked = m_Patches[gridPatches.SelectedRows[0].Index].m_FileToPatch != null;
            if (chkApplyToFile.Checked) txtApplyToFile.Text = m_Patches[gridPatches.SelectedRows[0].Index].m_FileToPatch;
            chkApplyToOverlay.Checked = m_Patches[gridPatches.SelectedRows[0].Index].m_OverlayID != null;
            if (chkApplyToOverlay.Checked) txtOverlayID.Text = m_Patches[gridPatches.SelectedRows[0].Index].m_OverlayID;
            chkDecompressAllOverlays.Checked = m_Patches[gridPatches.SelectedRows[0].Index].m_DecompressAllOverlays;
        }

        /* 
         * Convert each AddressDataPair to string in format
         * ADDRESS: 
         * BYTE BYTE BYTE BYTE
         * BYTE BYTE BYTE BYTE
         */ 
        private void DisplayPatchData(string version, List<Patch.AddressDataPair> patchData)
        {
            List<string> lines = txtPatchData.Text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.None).ToList();
            int startLine = lines.IndexOf("START " + version);
            int endLine = lines.IndexOf("END " + version);

            if (startLine == -1 || endLine == -1)
                return;

            string[] patchStrings = new string[patchData.Count];
            for (int i = 0; i < patchData.Count; i++)
            {
                uint address = patchData.ElementAt(i).m_Address;
                string addressStr = String.Format("{0:X8}", address) + ": " + nl;
                patchStrings[i] = addressStr;
                int numRows = patchData.ElementAt(i).m_Data.Length / 4;
                int mod4 = patchData.ElementAt(i).m_Data.Length % 4;
                if (mod4 != 0)
                    numRows++;

                for (int j = 0; j < numRows; j++)
                {
                    int numBytes = (j != numRows - 1) ? 4 : ((mod4 == 0) ? 4 : mod4);
                    for (int k = (j * 4); k < (j * 4) + numBytes; k++)
                    {
                        patchStrings[i] += String.Format("{0:X2}", patchData.ElementAt(i).m_Data[k]) + " ";
                    }
                    patchStrings[i] += nl;
                }

                lines.Insert(endLine, patchStrings[i]);
                endLine = lines.IndexOf("END " + version);
            }

            txtPatchData.Text = String.Join(nl, lines.ToArray());
        }

        private List<Patch.AddressDataPair> ParseEnteredPatchData(string version)
        {
            List<Patch.AddressDataPair> addressDataPairList = new List<Patch.AddressDataPair>();
            List<string> lines = txtPatchData.Text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            int startLine = lines.IndexOf("START " + version);
            int endLine = lines.IndexOf("END " + version);

            for (int i = startLine; i < endLine; i++)
            {
                if (lines.ElementAt(i).Contains(":"))
                {
                    Patch.AddressDataPair adp = new Patch.AddressDataPair();
                    try
                    {
                        int addressLength = lines.ElementAt(i).IndexOf(":");
                        uint address = uint.Parse(lines.ElementAt(i).Replace("0x", "").Replace("(0x)", "").Substring(0, addressLength), 
                            System.Globalization.NumberStyles.HexNumber);
                        adp.m_Address = address;
                    }
                    catch { MessageBox.Show("Error parsing address on line " + i + "\n\n" + lines.ElementAt(i)); return null; }

                    i++;

                    List<byte> byteList = new List<byte>();
                    int count = i;
                    while (count < endLine && !lines.ElementAt(count).Contains(":"))
                    {
                        string bytes = lines.ElementAt(count).Replace(" ", "").Replace("0x", "").Replace("(0x)", "");
                        int numBytes = bytes.Length / 2;
                        if (bytes.Length % 2 != 0)
                        {
                            bytes.Insert(bytes.Length - 1, "0");
                            numBytes++;
                        }

                        for (int j = 0; j < numBytes; j++)
                        {
                            try
                            {
                                byteList.Add(byte.Parse(bytes.Substring(j * 2, 2), System.Globalization.NumberStyles.HexNumber));
                            }
                            catch { MessageBox.Show("Error parsing data on line " + i + ", character " +
                                (j * 2) + "\n\n" + bytes.Substring(j * 2, 2)); return null;
                            }
                        }

                        count++;
                    }
                    adp.m_Data = byteList.ToArray();
                    addressDataPairList.Add(adp);
                }
            }

            return addressDataPairList;
        }

        private void clearEditData()
        {
            txtPatchName.Text = "";
            txtAuthor.Text = "";
            txtApplyToFile.Text = "";
            chkApplyToFile.Checked = false;
            txtOverlayID.Text = "";
            chkApplyToOverlay.Checked = false;
            chkDecompressAllOverlays.Checked = false;

            txtPatchData.Text = "START EUR" + nl + nl + "END EUR" + nl +nl + "START USv1" + nl + nl + 
                "END USv1" + nl + nl + "START USv2" + nl + nl +"END USv2" + nl + nl + "START JAP" + nl + nl + "END JAP" + nl;
        }

        private void btnDeletePatch_Click(object sender, EventArgs e)
        {
            if (gridPatches.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a patch to delete.");
                return;
            }

            DialogResult dlgeResult = MessageBox.Show("Are you sure you want to delete this patch?", "Confirm Deletion", MessageBoxButtons.YesNo);
            if (dlgeResult == DialogResult.No)
                return;

            // Remove current patch from list of patches
            m_Patches.RemoveAt(gridPatches.SelectedRows[0].Index);
            // Write updated list of patches to XML file
            Patch.PatchToXML(m_Patches);

            Patch.XMLToPatch();
            populatePatchListTable();
        }

        private void btnNewPatch_Click(object sender, EventArgs e)
        {
            editingIndex = -1;
            clearEditData();
        }

        private void btnSavePatch_Click(object sender, EventArgs e)
        {
            string name = txtPatchName.Text;
            string author = (txtAuthor.Text != "") ? txtAuthor.Text : null;

            bool isFileToPatch = chkApplyToFile.Checked;
            bool isOverlayToPatch = chkApplyToOverlay.Checked;
            String fileToPatch = (isFileToPatch) ? txtApplyToFile.Text : null;
            String overlayID = (isOverlayToPatch) ? txtOverlayID.Text : null;
            bool decompressAllOverlays = chkDecompressAllOverlays.Checked;
            List<Patch.AddressDataPair> eurPatch = new List<Patch.AddressDataPair>(), usv1Patch = new List<Patch.AddressDataPair>(),
                usv2Patch = new List<Patch.AddressDataPair>(), japPatch = new List<Patch.AddressDataPair>();
            List<Patch.AddressDataPair> eurRestoreData = new List<Patch.AddressDataPair>(), usv1RestoreData = new List<Patch.AddressDataPair>(),
                usv2RestoreData = new List<Patch.AddressDataPair>(), japRestoreData = new List<Patch.AddressDataPair>();

            // Read entered patch data
            eurPatch = ParseEnteredPatchData("EUR");
            usv1Patch = ParseEnteredPatchData("USv1");
            usv2Patch = ParseEnteredPatchData("USv2");
            japPatch = ParseEnteredPatchData("JAP");

            // Generate the restore data for current ROM version by reading the current data at the addresses to be patched
            Program.m_ROM.BeginRW();
            if (decompressAllOverlays)
                Helper.DecompressOverlaysWithinGame();

            switch (Program.m_ROM.m_Version)
            {
                case NitroROM.Version.EUR:
                    eurRestoreData = Patch.GenerateRestoreData(eurPatch);
                    break;
                case NitroROM.Version.USA_v1:
                    usv1RestoreData = Patch.GenerateRestoreData(usv1Patch);
                    break;
                case NitroROM.Version.USA_v2:
                    usv2RestoreData = Patch.GenerateRestoreData(usv2Patch);
                    break;
                case NitroROM.Version.JAP:
                    japRestoreData = Patch.GenerateRestoreData(japPatch);
                    break;
            }

            Program.m_ROM.EndRW();

            // Finally, add the parsed details to the list of patches
            if (editingIndex == -1)
                m_Patches.Add(new Patch(name, author, eurPatch, usv1Patch, usv2Patch, japPatch, eurRestoreData, usv1RestoreData, 
                    usv2RestoreData, japRestoreData, fileToPatch, overlayID, decompressAllOverlays));
            else
                m_Patches[editingIndex] = new Patch(name, author, eurPatch, usv1Patch, usv2Patch, japPatch, eurRestoreData, usv1RestoreData, 
                    usv2RestoreData, japRestoreData, fileToPatch, overlayID, decompressAllOverlays);

            // Write the updated patches to XML
            Patch.PatchToXML(m_Patches);

            // Reload the list of patches
            m_Patches = Patch.XMLToPatch();
            populatePatchListTable();
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            using (var form = new ROMFileSelect("Please select a file to open."))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    txtApplyToFile.Text = form.m_SelectedFile;
                }
            }
        }

        private void chkApplyToFile_CheckedChanged(object sender, EventArgs e)
        {
            if (chkApplyToFile.Checked)
                chkApplyToOverlay.Checked = false;
        }

        private void chkApplyToOverlay_CheckedChanged(object sender, EventArgs e)
        {
            if (chkApplyToOverlay.Checked)
                chkApplyToFile.Checked = false;
        }
    }

    public class Patch
    {
        public string m_PatchName;
        public string m_Author;
        public List<AddressDataPair> m_EURPatch;
        public List<AddressDataPair> m_USv1Patch;
        public List<AddressDataPair> m_USv2Patch;
        public List<AddressDataPair> m_JAPPatch;
        public List<AddressDataPair> m_EURRestoreData;
        public List<AddressDataPair> m_USv1RestoreData;
        public List<AddressDataPair> m_USv2RestoreData;
        public List<AddressDataPair> m_JAPRestoreData;
        public string m_FileToPatch;// Optionally you can specify the patch be applied to a particular file (addresses relative to start)
        private bool m_IsFileToPatch;
        public string m_OverlayID;// Optionally set if an overlay is being patched, stored as string so we can check if null
        public bool m_DecompressAllOverlays;

        bool m_IsApplied;
        public bool[] m_VersionSupport = new bool[] { false, false, false, false };// Whether each region is supported, EUR, USv1, USv2, JAP

        public Patch(string name, string author, List<AddressDataPair> eurPatch, List<AddressDataPair> usv1Patch, List<AddressDataPair> usv2Patch,
            List<AddressDataPair> japPatch, List<AddressDataPair> eurRestoreData, List<AddressDataPair> usv1RestoreData,
            List<AddressDataPair> usv2RestoreData, List<AddressDataPair> japRestoreData, string fileToPatch = null, string overlayID = null, bool decompressAllOverlays = false)
        {
            m_PatchName = name;
            m_Author = author;
            m_EURPatch = eurPatch;
            m_USv1Patch = usv1Patch;
            m_USv2Patch = usv2Patch;
            m_JAPPatch = japPatch;
            m_EURRestoreData = eurRestoreData;
            m_USv1RestoreData = usv1RestoreData;
            m_USv2RestoreData = usv2RestoreData;
            m_JAPRestoreData = japRestoreData;

            m_IsFileToPatch = (!(fileToPatch == null || fileToPatch.Equals("")));
            m_FileToPatch = (m_IsFileToPatch) ? fileToPatch : null;
            m_OverlayID = overlayID;
            m_DecompressAllOverlays = decompressAllOverlays;

            m_VersionSupport = new bool[] { (eurPatch.Count > 0), (usv1Patch.Count > 0), (usv2Patch.Count > 0), (japPatch.Count > 0) };
        }

        public Patch() { }

        /*
         * This method takes the first Address and Data from the patch and tests if the current values in the 
         * ROM match the values of the patch - if they match the patch is already applied. This doesn't test 
         * all values to speed things up.
         */ 
        public bool CheckIsApplied(NitroROM rom)
        {
            if (m_DecompressAllOverlays && Helper.CheckAllOverlaysDecompressed() == false)
            {
                m_IsApplied = false;
                return false;
            }

            INitroROMBlock fileToPatch = null;
            if (m_FileToPatch != null)
                fileToPatch = Program.m_ROM.GetFileFromName(m_FileToPatch);
            else if (m_OverlayID != null)
                fileToPatch = new NitroOverlay(Program.m_ROM, uint.Parse(m_OverlayID));

            AddressDataPair testAddressDataPair = null;

            switch (rom.m_Version)
            {
                case NitroROM.Version.EUR:
                    testAddressDataPair = (m_EURPatch.Count == 0) ? null : m_EURPatch.ElementAt(0);
                    break;
                case NitroROM.Version.USA_v1:
                    testAddressDataPair = (m_USv1Patch.Count == 0) ? null : m_USv1Patch.ElementAt(0);
                    break;
                case NitroROM.Version.USA_v2:
                    testAddressDataPair = (m_USv2Patch.Count == 0) ? null : m_USv2Patch.ElementAt(0);
                    break;
                case NitroROM.Version.JAP:
                    testAddressDataPair = (m_JAPPatch.Count == 0) ? null : m_JAPPatch.ElementAt(0);
                    break;
            }

            if (testAddressDataPair == null || testAddressDataPair.m_Data == null || testAddressDataPair.m_Data.Length == 0)
            {
                m_IsApplied = false;
                return false;
            }

            for (int i = 0; i < testAddressDataPair.m_Data.Length; i++)
            {
                if (fileToPatch == null && rom.Read8(testAddressDataPair.m_Address + (uint)i) != testAddressDataPair.m_Data[i])
                {
                    m_IsApplied = false;
                    return false;
                }
                else if (fileToPatch != null && fileToPatch.Read8(testAddressDataPair.m_Address + (uint)i) != testAddressDataPair.m_Data[i])
                {
                    m_IsApplied = false;
                    return false;
                }
            }

            //If it reaches here, the patch has already been applied
            m_IsApplied = true;
            return true;
        }

        public void ApplyPatch(NitroROM rom)
        {
            List<AddressDataPair> addressDataPairs = null;

            switch (rom.m_Version)
            {
                case NitroROM.Version.EUR:
                    addressDataPairs = m_EURPatch;
                    m_EURRestoreData = GenerateRestoreData(m_EURPatch);
                    break;
                case NitroROM.Version.USA_v1:
                    addressDataPairs = m_USv1Patch;
                    m_USv1RestoreData = GenerateRestoreData(m_USv1Patch);
                    break;
                case NitroROM.Version.USA_v2:
                    addressDataPairs = m_USv2Patch;
                    m_USv2RestoreData = GenerateRestoreData(m_USv2Patch);
                    break;
                case NitroROM.Version.JAP:
                    addressDataPairs = m_JAPPatch;
                    m_JAPRestoreData = GenerateRestoreData(m_JAPPatch);
                    break;
            }

            if (m_DecompressAllOverlays)
                Helper.DecompressOverlaysWithinGame();

            INitroROMBlock fileToPatch = null;
            if (m_FileToPatch != null)
                fileToPatch = Program.m_ROM.GetFileFromName(m_FileToPatch);
            else if (m_OverlayID != null)
                fileToPatch = new NitroOverlay(Program.m_ROM, uint.Parse(m_OverlayID));

            foreach (AddressDataPair addressDataPair in addressDataPairs)
            {
                for (int i = 0; i < addressDataPair.m_Data.Length; i++)
                {
                    if (fileToPatch == null)
                    {
                        rom.Write8(addressDataPair.m_Address + (uint)i, addressDataPair.m_Data[i]);
                    }
                    else
                    {
                        fileToPatch.Write8(addressDataPair.m_Address + (uint)i, addressDataPair.m_Data[i]);
                    }
                }
            }

            if (fileToPatch != null)
                fileToPatch.SaveChanges();
        }

        public void RemovePatch(NitroROM rom)
        {
            List<AddressDataPair> addressDataPairs = null;

            switch (rom.m_Version)
            {
                case NitroROM.Version.EUR:
                    addressDataPairs = m_EURRestoreData;
                    break;
                case NitroROM.Version.USA_v1:
                    addressDataPairs = m_USv1RestoreData;
                    break;
                case NitroROM.Version.USA_v2:
                    addressDataPairs = m_USv2RestoreData;
                    break;
                case NitroROM.Version.JAP:
                    addressDataPairs = m_JAPRestoreData;
                    break;
            }

            INitroROMBlock fileToPatch = null;
            if (m_FileToPatch != null)
                fileToPatch = Program.m_ROM.GetFileFromName(m_FileToPatch);
            else if (m_OverlayID != null)
                fileToPatch = new NitroOverlay(Program.m_ROM, uint.Parse(m_OverlayID));

            foreach (AddressDataPair addressDataPair in addressDataPairs)
            {
                for (int i = 0; i < addressDataPair.m_Data.Length; i++)
                {
                    if (fileToPatch == null)
                    {
                        rom.Write8(addressDataPair.m_Address + (uint)i, addressDataPair.m_Data[i]);
                    }
                    else
                    {
                        fileToPatch.Write8(addressDataPair.m_Address + (uint)i, addressDataPair.m_Data[i]);
                    }
                }
            }

            if (fileToPatch != null)
                fileToPatch.SaveChanges();
        }

        public static List<AddressDataPair> GenerateRestoreData(List<AddressDataPair> patchData, string fileName = null, string overlayID = null)
        {
            List<AddressDataPair> restoreDataList = new List<AddressDataPair>();

            INitroROMBlock fileToPatch = null;
            if (fileName != null)
                fileToPatch = Program.m_ROM.GetFileFromName(fileName);
            else if (overlayID != null)
                fileToPatch = new NitroOverlay(Program.m_ROM, uint.Parse(overlayID));

            foreach (AddressDataPair addressDataPair in patchData)
            {
                AddressDataPair restoreData = new AddressDataPair();
                restoreData.m_Address = addressDataPair.m_Address;

                List<byte> data = new List<byte>();
                for (int i = 0; i < addressDataPair.m_Data.Length; i++)
                {
                    if (fileToPatch == null)
                        data.Add(Program.m_ROM.Read8(addressDataPair.m_Address + (uint)i));
                    else
                        data.Add(fileToPatch.Read8(addressDataPair.m_Address + (uint)i));
                }
                restoreData.m_Data = data.ToArray();

                restoreDataList.Add(restoreData);
            }

            return restoreDataList;
        }

        public static void PatchToXML(List<Patch> patches)
        {
            // Write all patches to XML
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";
            settings.NewLineChars = "\r\n";
            settings.NewLineHandling = NewLineHandling.Replace;
            using (XmlWriter writer = XmlWriter.Create(Path.Combine(Application.StartupPath, "AdditionalPatches.xml"), settings))
            {
                writer.WriteStartDocument();
                writer.WriteComment(Program.AppTitle + " " + Program.AppVersion + " " + Program.AppDate);
                writer.WriteStartElement("Patches");

                foreach (Patch patch in patches)
                {
                    writer.WriteStartElement("Patch");

                    writer.WriteElementString("PatchName", patch.m_PatchName);

                    if (patch.m_Author != null)
                        writer.WriteElementString("Author", patch.m_Author);

                    if (patch.m_IsFileToPatch)
                        writer.WriteElementString("FileToPatch", patch.m_FileToPatch);

                    if (patch.m_OverlayID != null)
                        writer.WriteElementString("OverlayToPatch", patch.m_OverlayID);

                    if (patch.m_DecompressAllOverlays)
                        writer.WriteElementString("DecompressAllOverlays", (patch.m_DecompressAllOverlays == true) ? "TRUE" : "FALSE");

                    if (patch.m_EURPatch.Count != 0)
                    {
                        writer.WriteStartElement("EURPatch");
                        WriteAddressDataPairs(patch.m_EURPatch, writer);
                        writer.WriteEndElement();
                    }

                    if (patch.m_USv1Patch.Count != 0)
                    {
                        writer.WriteStartElement("USv1Patch");
                        WriteAddressDataPairs(patch.m_USv1Patch, writer);
                        writer.WriteEndElement();
                    }

                    if (patch.m_USv2Patch.Count != 0)
                    {
                        writer.WriteStartElement("USv2Patch");
                        WriteAddressDataPairs(patch.m_USv2Patch, writer);
                        writer.WriteEndElement();
                    }

                    if (patch.m_JAPPatch.Count != 0)
                    {
                        writer.WriteStartElement("JAPPatch");
                        WriteAddressDataPairs(patch.m_JAPPatch, writer);
                        writer.WriteEndElement();
                    }

                    if (patch.m_EURRestoreData.Count != 0)
                    {
                        writer.WriteStartElement("EURRestoreData");
                        WriteAddressDataPairs(patch.m_EURRestoreData, writer);
                        writer.WriteEndElement();
                    }

                    if (patch.m_USv1RestoreData.Count != 0)
                    {
                        writer.WriteStartElement("USv1RestoreData");
                        WriteAddressDataPairs(patch.m_USv1RestoreData, writer);
                        writer.WriteEndElement();
                    }

                    if (patch.m_USv2RestoreData.Count != 0)
                    {
                        writer.WriteStartElement("USv2RestoreData");
                        WriteAddressDataPairs(patch.m_USv2RestoreData, writer);
                        writer.WriteEndElement();
                    }

                    if (patch.m_JAPRestoreData.Count != 0)
                    {
                        writer.WriteStartElement("JAPRestoreData");
                        WriteAddressDataPairs(patch.m_JAPRestoreData, writer);
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public static List<Patch> XMLToPatch()
        {
            List<Patch> patches = new List<Patch>();
            string name = "";
            string author = null;
            List<AddressDataPair> eurPatch = new List<AddressDataPair>(), usv1Patch = new List<AddressDataPair>(),
                usv2Patch = new List<AddressDataPair>(), japPatch = new List<AddressDataPair>();
            List<AddressDataPair> eurRestoreData = new List<AddressDataPair>(), usv1RestoreData = new List<AddressDataPair>(),
                usv2RestoreData = new List<AddressDataPair>(), japRestoreData = new List<AddressDataPair>();
            string fileToPatch = "";
            string overlayID = null;
            bool decompressAllOverlays = false;

            // Create an XML reader for this file.
            using (XmlReader reader = XmlReader.Create(Path.Combine(Application.StartupPath, "AdditionalPatches.xml")))
            {
                reader.MoveToContent();

                while (reader.Read())
                {
                    if (reader.NodeType.Equals(XmlNodeType.Element))
                    {
                        switch (reader.LocalName)
                        {
                            case "Patch":
                                break;
                            case "PatchName":
                                reader.MoveToContent();
                                name = reader.ReadElementContentAsString();
                                break;
                            case "Author":
                                reader.MoveToContent();
                                author = reader.ReadElementContentAsString();
                                break;
                            case "FileToPatch":
                                reader.MoveToContent();
                                fileToPatch = reader.ReadElementContentAsString();
                                break;
                            case "OverlayToPatch":
                                reader.MoveToContent();
                                overlayID = reader.ReadElementContentAsString();
                                break;
                            case "DecompressAllOverlays":
                                reader.MoveToContent();
                                decompressAllOverlays = (reader.ReadElementContentAsString().ToLowerInvariant().Equals("true")) ? true : false;
                                break;
                            case "EURPatch":
                                eurPatch = ReadAddressDataPairs(reader);
                                break;
                            case "USv1Patch":
                                usv1Patch = ReadAddressDataPairs(reader);
                                break;
                            case "USv2Patch":
                                usv2Patch = ReadAddressDataPairs(reader);
                                break;
                            case "JAPPatch":
                                japPatch = ReadAddressDataPairs(reader);
                                break;
                            case "EURRestoreData":
                                eurRestoreData = ReadAddressDataPairs(reader);
                                break;
                            case "USv1RestoreData":
                                usv1RestoreData = ReadAddressDataPairs(reader);
                                break;
                            case "USv2RestoreData":
                                usv2RestoreData = ReadAddressDataPairs(reader);
                                break;
                            case "JAPRestoreData":
                                japRestoreData = ReadAddressDataPairs(reader);
                                break;
                        }
                    }
                    else if (reader.NodeType.Equals(XmlNodeType.EndElement))
                    {
                        if (reader.LocalName.Equals("Patch"))
                        {
                            patches.Add(new Patch(name, author, eurPatch.ToList(), usv1Patch.ToList(), usv2Patch.ToList(), japPatch.ToList(),
                                eurRestoreData.ToList(), usv1RestoreData.ToList(), usv2RestoreData.ToList(), japRestoreData.ToList(), 
                                fileToPatch, overlayID, decompressAllOverlays));
                            // Reset lists for next patch
                            eurPatch.Clear(); usv1Patch.Clear(); usv2Patch.Clear(); japPatch.Clear();
                            eurRestoreData.Clear(); usv1RestoreData.Clear(); usv2RestoreData.Clear(); japRestoreData.Clear();
                            fileToPatch = "";
                            overlayID = null;
                            decompressAllOverlays = false;
                            author = null;
                        }
                    }
                }
            }

            return patches;
        }

        private static List<AddressDataPair> ReadAddressDataPairs(XmlReader reader)
        {
            List<AddressDataPair> addressDataPairList = new List<AddressDataPair>();
            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.LocalName.Equals("AddressDataPair"))
                {
                    AddressDataPair addressDataPair = new AddressDataPair();
                    List<byte> byteList = new List<byte>();
                    while (reader.Read())
                    {
                        reader.MoveToContent();
                        if (reader.LocalName.Equals("Address"))
                        {
                            addressDataPair.m_Address = (uint)(reader.ReadElementContentAsLong());
                        }
                        else if (reader.LocalName.Equals("Data"))
                        {
                            while (reader.Read())
                            {
                                reader.MoveToContent();
                                if (reader.LocalName.Equals("Byte"))
                                {
                                    byteList.Add((byte)reader.ReadElementContentAsLong());
                                }
                                else
                                {
                                    addressDataPair.m_Data = byteList.ToArray();
                                    addressDataPairList.Add(addressDataPair);
                                    break;
                                }
                            }
                        }
                        else
                            break;
                    }
                }
                else
                    break;
            }
            return addressDataPairList;
        }

        private static void WriteAddressDataPairs(List<AddressDataPair> adpList, XmlWriter writer)
        {
            foreach (AddressDataPair adp in adpList)
            {
                // Start AddressDataPair
                writer.WriteStartElement("AddressDataPair");

                // Start Address
                writer.WriteElementString("Address", adp.m_Address.ToString());
                // End Address

                // Start Data
                writer.WriteStartElement("Data");
                // Start Byte(s)
                for (int i = 0; i < adp.m_Data.Length; i++)
                {
                    writer.WriteElementString("Byte", adp.m_Data[i].ToString());
                }
                // End Byte(s)
                writer.WriteEndElement();
                // End Data

                writer.WriteEndElement();
                // End AddressDataPair
            }
        }

        public class AddressDataPair
        {
            public uint m_Address;
            public byte[] m_Data;

            public AddressDataPair() { }

            public AddressDataPair(uint address)
            {
                this.m_Address = address;
            }

            public AddressDataPair(uint address, List<byte> data)
            {
                this.m_Address = address;
                this.m_Data = data.ToArray();
            }

            public AddressDataPair(uint address, byte[] data)
            {
                this.m_Address = address;
                data.CopyTo(m_Data, 0);
            }
        }
    }
}
