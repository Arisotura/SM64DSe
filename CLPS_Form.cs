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
    public partial class CLPS_Form : Form
    {

        LevelEditorForm _owner;
        uint clps_addr = 0;
        ushort clps_num = 0;
        uint clps_size = 0;
        byte[][] entries;
        BiDictionaryOneToOne<string, byte[]> GLOBAL_CLPS_TYPES;


        public CLPS_Form(LevelEditorForm _owner)
        {
            InitializeComponent();

            this._owner = _owner;

            for (int i = 0; i < 52; i++)
                cbxLevels.Items.Add(i + " - " + Strings.LevelNames[i]);

            loadGlobalCLPSTypes();

            loadCLPSData();
        }

        private void loadCLPSData()
        {
            clps_addr = _owner.m_Overlay.ReadPointer(0x60);
            clps_num = _owner.m_Overlay.Read16(clps_addr + 0x06);
            clps_size = (uint)(8 + (clps_num * 8));
            txtNumEntries.Text = "" + clps_num;
            entries = new byte[clps_num][];

            gridCLPSData.ColumnCount = 8;
            gridCLPSData.RowCount = clps_num;
            gridCLPSData.Columns[0].Width = 32;
            uint entry = clps_addr + 0x08;

            // Set column widths
            for (int i = 0; i < 8; i++)
                gridCLPSData.Columns[i].Width = 32;

            DataGridViewComboBoxColumn cmb = new DataGridViewComboBoxColumn();
            cmb.HeaderText = "Type/Description";
            cmb.Items.Add("Other/Unknown");
            cmb.Width = 160;
            foreach (string name in GLOBAL_CLPS_TYPES.GetFirstToSecond().Keys)
                cmb.Items.Add(name);
            gridCLPSData.Columns.Add(cmb);

            for (int i = 0; i < clps_num; i++)
            {
                gridCLPSData.Rows[i].HeaderCell.Value = "" + i;
                entries[i] = new byte[8];
                for (int j = 0; j < 8; j++)
                {
                    entries[i][j] = _owner.m_Overlay.Read8((uint)(entry + (j)));
                    gridCLPSData.Rows[i].Cells[j].Value = entries[i][j];
                }

                // Fill in Type/Description column
                if (GLOBAL_CLPS_TYPES.GetSecondToFirst().ContainsKey(entries[i]))
                    gridCLPSData.Rows[i].Cells[8].Value = GLOBAL_CLPS_TYPES.GetSecondToFirst()[entries[i]];
                else
                    gridCLPSData.Rows[i].Cells[8].Value = cmb.Items[0];// Other/Unknown

                entry += 8;
            }

        }

        /* Reads in the list of known Global Collision Type values
         */
        private void loadGlobalCLPSTypes()
        {
            GLOBAL_CLPS_TYPES = new BiDictionaryOneToOne<string, byte[]>(new ByteArrayComparer());
            string entryName = "";
            byte[] entryValue = new byte[8];
            int valueCount = 0;

            // Create an XML reader for this file.
            using (XmlReader reader = XmlReader.Create(Path.Combine(Application.StartupPath, "CLPS_Types.xml")))
            {
                reader.MoveToContent();

                while (reader.Read())
                {
                    if (reader.NodeType.Equals(XmlNodeType.Element))
                    {
                        switch (reader.LocalName)
                        {
                            case "Entry":
                                entryName = reader.GetAttribute("name");
                                break;
                            case "Value":
                                entryValue = new byte[8];
                                valueCount = 0;
                                break;
                            case "Byte":
                                entryValue[valueCount] = Byte.Parse(reader.ReadElementContentAsString());
                                valueCount++;
                                break;
                        }
                    }
                    else if (reader.NodeType.Equals(XmlNodeType.EndElement))
                    {
                        switch (reader.LocalName)
                        {
                            case "Entry":
                                GLOBAL_CLPS_TYPES.Add(entryName, entryValue);
                                break;
                        }
                    }
                }
            }

        }

        void gridCLPSData_CellEndEdit(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex != 8)
                {
                    _owner.m_Overlay.Write8((uint)((clps_addr + 8) + (8 * e.RowIndex) + e.ColumnIndex),
                        (byte)int.Parse(gridCLPSData.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()));
                }
                else
                {
                    string entryName = gridCLPSData.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    if (entryName.Equals("Other/Unknown"))
                        return;
                    byte[] entryValue = GLOBAL_CLPS_TYPES.GetFirstToSecond()[entryName];
                    int rowIndex = e.RowIndex;
                    for (int i = 0; i < 8; i++)
                    {
                        // Update entry in overlay CLPS table
                        _owner.m_Overlay.Write8((uint)((clps_addr + 8) + (8 * rowIndex) + i),
                        (byte)entryValue[i]);
                    }
                }

                loadCLPSData();
            }
            catch
            {
                MessageBox.Show("Please enter a valid number between 0 - 255.");
            }
        }

        private void copyCLPS(int sourceLevel)
        {
            NitroOverlay otherOVL = new NitroOverlay(Program.m_ROM, Program.m_ROM.GetLevelOverlayID(sourceLevel));

            uint other_clps_addr = otherOVL.ReadPointer(0x60);
            ushort other_clps_num = otherOVL.Read16(other_clps_addr + 0x06);
            uint other_clps_size = (uint)(8 + (other_clps_num * 8));

            uint clps_addr = _owner.m_Overlay.ReadPointer(0x60);
            ushort clps_num = _owner.m_Overlay.Read16(clps_addr + 0x06);
            uint clps_size = (uint)(8 + (clps_num * 8));

            byte[] CLPS_data = otherOVL.ReadBlock(other_clps_addr, other_clps_size);

            // Make or remove room for other CLPS table
            if (clps_size < other_clps_size)
            {
                AddSpace(clps_addr + clps_size, other_clps_size - clps_size);
            }
            else if (clps_size > other_clps_size)
            {
                RemoveSpace(clps_addr + clps_size - (clps_size - other_clps_size), clps_size - other_clps_size);
            }

            // Write CLPS table from overlay
            _owner.m_Overlay.WriteBlock(clps_addr, CLPS_data);

            loadCLPSData();
        }

        private void AddSpace(uint offset, uint amount)
        {
            if ((_owner.m_Overlay.GetSize() + amount) > NitroROM.LEVEL_OVERLAY_SIZE)
                throw new Exception("This level has reached the level size limit. Cannot add more data.");

            // move the data
            byte[] block = _owner.m_Overlay.ReadBlock(offset, (uint)(_owner.m_Overlay.GetSize() - offset));
            _owner.m_Overlay.WriteBlock(offset + amount, block);

            // write zeroes in the newly created space
            for (int i = 0; i < amount; i++)
                _owner.m_Overlay.Write8((uint)(offset + i), 0);

            // update the pointers
            for (int i = 0; i < _owner.m_PointerList.Count; i++)
            {
                LevelEditorForm.PointerReference ptrref = _owner.m_PointerList[i];
                if (ptrref.m_ReferenceAddr >= offset)
                    ptrref.m_ReferenceAddr += amount;
                if (ptrref.m_PointerAddr >= offset)
                {
                    ptrref.m_PointerAddr += amount;
                    _owner.m_Overlay.WritePointer(ptrref.m_ReferenceAddr, ptrref.m_PointerAddr);
                }
                _owner.m_PointerList[i] = ptrref;
            }

            // update the objects 'n' all
            UpdateObjectOffsets(offset, amount);
        }

        private void RemoveSpace(uint offset, uint amount)
        {
            // move the data
            byte[] block = _owner.m_Overlay.ReadBlock(offset + amount, (uint)(_owner.m_Overlay.GetSize() - offset - amount));
            _owner.m_Overlay.WriteBlock(offset, block);
            _owner.m_Overlay.SetSize(_owner.m_Overlay.GetSize() - amount);

            // update the pointers
            for (int i = 0; i < _owner.m_PointerList.Count; i++)
            {
                LevelEditorForm.PointerReference ptrref = _owner.m_PointerList[i];
                if (ptrref.m_ReferenceAddr >= (offset + amount))
                    ptrref.m_ReferenceAddr -= amount;
                if (ptrref.m_PointerAddr >= (offset + amount))
                {
                    ptrref.m_PointerAddr -= amount;
                    _owner.m_Overlay.WritePointer(ptrref.m_ReferenceAddr, ptrref.m_PointerAddr);
                }
                _owner.m_PointerList[i] = ptrref;
            }

            // update the objects 'n' all
            UpdateObjectOffsets(offset + amount, (uint)-amount);
        }

        public void UpdateObjectOffsets(uint start, uint delta)
        {
            foreach (LevelObject obj in _owner.m_LevelObjects.Values)
                if (obj.m_Offset >= start) obj.m_Offset += delta;

            for (int a = 0; a < _owner.m_TexAnims.Length; a++)
            {
                foreach (LevelTexAnim anim in _owner.m_TexAnims[a])
                {
                    if (anim.m_Offset >= start) anim.m_Offset += delta;
                    if (anim.m_ScaleTblOffset >= start) anim.m_ScaleTblOffset += delta;
                    if (anim.m_RotTblOffset >= start) anim.m_RotTblOffset += delta;
                    if (anim.m_TransTblOffset >= start) anim.m_TransTblOffset += delta;
                    if (anim.m_MatNameOffset >= start) anim.m_MatNameOffset += delta;
                }
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (cbxLevels.SelectedIndex != -1)
                copyCLPS(cbxLevels.SelectedIndex);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (gridCLPSData.SelectedRows.Count == 0)
            {
                // Make room for a new entry at end
                AddSpace(clps_addr + clps_size, 8);
            }
            else
            {
                // Make room after selected row
                AddSpace((uint)(clps_addr + 8 + (8 * gridCLPSData.SelectedRows[0].Index) + 8), 8);
            }
            // Update the number of entries
            _owner.m_Overlay.Write16(clps_addr + 0x06, (ushort)(_owner.m_Overlay.Read16(clps_addr + 0x06) + 1));
            // Reload data
            loadCLPSData();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (gridCLPSData.SelectedRows.Count == 0)
                MessageBox.Show("Please select a row to delete.");
            else
            {
                // Remove selected row
                RemoveSpace((uint)(clps_addr + 8 + (8 * gridCLPSData.SelectedRows[0].Index)), 8);
                // Update the number of entries
                _owner.m_Overlay.Write16(clps_addr + 0x06, (ushort)(_owner.m_Overlay.Read16(clps_addr + 0x06) - 1));
                // Reload data
                loadCLPSData();
            }
        }

        private void btnShiftUp_Click(object sender, EventArgs e)
        {
            if (gridCLPSData.SelectedRows.Count == 0)
                MessageBox.Show("Please select a row to move.");
            else if (gridCLPSData.SelectedRows[0].Index != 0)
            {
                byte[] rowA = _owner.m_Overlay.ReadBlock((uint)(clps_addr + 8 + (8 * gridCLPSData.SelectedRows[0].Index)), 8);
                byte[] rowB = _owner.m_Overlay.ReadBlock((uint)(clps_addr + 8 + (8 * (gridCLPSData.SelectedRows[0].Index - 1))), 8);
                _owner.m_Overlay.WriteBlock((uint)(clps_addr + 8 + (8 * gridCLPSData.SelectedRows[0].Index)), rowB);
                _owner.m_Overlay.WriteBlock((uint)(clps_addr + 8 + (8 * (gridCLPSData.SelectedRows[0].Index - 1))), rowA);
            }
            loadCLPSData();
        }

        private void btnShiftDown_Click(object sender, EventArgs e)
        {
            if (gridCLPSData.SelectedRows.Count == 0)
                MessageBox.Show("Please select a row to move.");
            else if (gridCLPSData.SelectedRows[0].Index != gridCLPSData.Rows.Count - 1)
            {
                byte[] rowA = _owner.m_Overlay.ReadBlock((uint)(clps_addr + 8 + (8 * gridCLPSData.SelectedRows[0].Index)), 8);
                byte[] rowB = _owner.m_Overlay.ReadBlock((uint)(clps_addr + 8 + (8 * (gridCLPSData.SelectedRows[0].Index + 1))), 8);
                _owner.m_Overlay.WriteBlock((uint)(clps_addr + 8 + (8 * gridCLPSData.SelectedRows[0].Index)), rowB);
                _owner.m_Overlay.WriteBlock((uint)(clps_addr + 8 + (8 * (gridCLPSData.SelectedRows[0].Index + 1))), rowA);
            }
            loadCLPSData();
        }
    }
}
