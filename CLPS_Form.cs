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
using SM64DSe.SM64DSFormats;

namespace SM64DSe
{
    public partial class CLPS_Form : Form
    {
        enum Columns
        {
            TEXTURE = 0,
            WATER,
            VIEW_ID,
            BEHAV_1,
            CAM_BEHAV,
            BEHAV_2,
            CAM_THROUGH,
            TOXIC,
            UNK_26,
            PAD_1,
            WIND_ID,
            PAD_2
        }
        struct ColStruct
        {
            public readonly string m_Name;
            public readonly int m_Shift;
            public readonly ulong m_And;
            public readonly string[] m_TypeNames;
            public ColStruct(string name, int shift, ulong and, string[] typeNames)
            {
                m_Name = name;
                m_Shift = shift;
                m_And = and;
                m_TypeNames = typeNames;
            }

            public ulong GetFlag(ulong flags) { return flags >> m_Shift & m_And; }
            public void SetFlag(ref ulong flags, ulong val)
                { flags = flags & ~(m_And << m_Shift) | (val & m_And) << m_Shift; }
        }
        ColStruct[] columns = new ColStruct[12]
        {
            new ColStruct ("Texture"     ,  0, 0x1fuL, new string[]{"",
                                                                    "Path-Textured ",
                                                                    "Grassy ",
                                                                    "Weirdly Textured ",
                                                                    "Rocky ",
                                                                    "Wooden ",
                                                                    "Snowy ",
                                                                    "Icy ",
                                                                    "Sandy ",
                                                                    "Flowery ",
                                                                    "Weirdly Textured ",
                                                                    "Weirdly Textured ",
                                                                    "Grate-Meshed ",
                                                                    "Weirdly Textured ",
                                                                    "Weirdly Textured ",
                                                                    "Weirdly Textured ",
                                                                    "Weirdly Textured ",
                                                                    "Weirdly Textured ",
                                                                    "Weirdly Textured ",
                                                                    "Weirdly Textured ",
                                                                    "Weirdly Textured ",
                                                                    "Weirdly Textured ",
                                                                    "Weirdly Textured ",
                                                                    "Weirdly Textured ",
                                                                    "Weirdly Textured ",
                                                                    "Weirdly Textured ",
                                                                    "Weirdly Textured ",
                                                                    "Weirdly Textured ",
                                                                    "Weirdly Textured ",
                                                                    "Weirdly Textured ",
                                                                    "Weirdly Textured ",
                                                                    "Weirdly Textured ",
                                                                    "Weirdly Textured ",}),
            new ColStruct ("Water"       ,  5, 0x01uL, new string[]{"",
                                                                    "Water-Defining "}),
            new ColStruct ("View ID"     ,  6, 0x3fuL, new string[]{}),
            new ColStruct ("Traction"    , 12, 0x07uL, new string[]{"",
                                                                    "Unslippable ",
                                                                    "Unslippable ",
                                                                    "Slippery-Sloped ",
                                                                    "Slippery ",
                                                                    "Slippery Unwalljumpable ",
                                                                    "Weird-Traction ",
                                                                    "Weird-Traction "}),
            new ColStruct ("Camera Type" , 15, 0x0fuL, new string[]{"Normal ",
                                                                    "Go-Behind ",
                                                                    "Zoom-Out-And-Go-Behind ",
                                                                    "Go-Behind ",
                                                                    "Go-Behind ",
                                                                    "Weird ",
                                                                    "Normal ",
                                                                    "Go-Behind ",
                                                                    "Go-Behind ",
                                                                    "8-Directional ",
                                                                    "Non-Rotating ",
                                                                    "Close-Up-And-Personal ",
                                                                    "Go-Behind ",
                                                                    "Go-Behind ",
                                                                    "Go-Behind ",
                                                                    "Go-Behind "}),
            new ColStruct ("Behavior"    , 19, 0x1fuL, new string[]{"Surface ",
                                                                    "Lava ",
                                                                    "Weird Surface ",
                                                                    "Hanging Mesh ",
                                                                    "Death Plane ",
                                                                    "Death Plane ",
                                                                    "Jump-Limiting Surface ",
                                                                    "Slow Quicksand ",
                                                                    "Slow Quicksand ",
                                                                    "Instant Quicksand ",
                                                                    "Weird Surface ",
                                                                    "Weird Surface ",
                                                                    "Weird Surface ",
                                                                    "Weird Surface ",
                                                                    "Weird Surface ",
                                                                    "Start Line ",
                                                                    "Finish Line ",
                                                                    "Vanish-Luigi-Transparent Surface ",
                                                                    "Get-Off-Of-Me Surface ",
                                                                    "Gust Plane ",
                                                                    "Weird Surface ",
                                                                    "Weird Surface ",
                                                                    "Weird Surface ",
                                                                    "Weird Surface ",
                                                                    "Weird Surface ",
                                                                    "Weird Surface ",
                                                                    "Weird Surface ",
                                                                    "Weird Surface ",
                                                                    "Weird Surface ",
                                                                    "Weird Surface ",
                                                                    "Weird Surface ",
                                                                    "Weird Surface "}),
            new ColStruct ("Camera Through"        , 24, 0x01uL, new string[]{"",
                                                                    "Go-Through " }),
            new ColStruct ("Toxic"       , 25, 0x01uL, new string[]{"",
                                                                    "Toxic "}),
            new ColStruct ("Unk << 26"   , 26, 0x01uL, new string[]{}),
            new ColStruct ("Pad << 27"   , 27, 0x1fuL, new string[]{}),
            new ColStruct ("Wind Path ID", 32, 0xffuL, new string[]{}),
            new ColStruct ("Pad << 40"   , 40, 0xffffffuL, new string[]{}),
        };

        CLPS m_CLPS;

        public CLPS_Form(CLPS CLPS)
        {
            InitializeComponent();

            this.m_CLPS = CLPS;

            for (int i = 0; i < 52; i++)
                cbxLevels.Items.Add(i + " - " + Strings.LevelNames[i]);

            LoadCLPSData();
        }
        
        private void LoadCLPSData()
        {
            txtNumEntries.Text = "" + m_CLPS.Count;

            gridCLPSData.RowCount = m_CLPS.Count;

            if (gridCLPSData.ColumnCount != columns.Length + 1)
            {
                gridCLPSData.ColumnCount = columns.Length;
                
                // Set column widths
                gridCLPSData.RowHeadersWidth = 54;
                for (int i = 0; i < columns.Length; i++)
                {
                    gridCLPSData.Columns[i].Width = 54;
                    gridCLPSData.Columns[i].HeaderText = columns[i].m_Name;
                }

                DataGridViewTextBoxColumn cmb = new DataGridViewTextBoxColumn();
                cmb.HeaderText = "Type/Description";
                cmb.ReadOnly = true;
                cmb.Width = 500;
                gridCLPSData.Columns.Add(cmb);
            }

            for (int i = 0; i < m_CLPS.Count; i++)
            {
                gridCLPSData.Rows[i].HeaderCell.Value = "" + i;
                for (int j = 0; j < columns.Length; j++)
                {
                    gridCLPSData.Rows[i].Cells[j].Value = columns[j].GetFlag(m_CLPS[i].flags);
                }

                // Fill in Type/Description column
                gridCLPSData.Rows[i].Cells[columns.Length].Value =
                    columns[(int)Columns.WATER].m_TypeNames[m_CLPS[i].m_Water] +
                    columns[(int)Columns.TOXIC].m_TypeNames[m_CLPS[i].m_Toxic] +
                    (m_CLPS[i].m_WindID != 0xff ? "Windy " : "") +
                    columns[(int)Columns.TEXTURE].m_TypeNames[m_CLPS[i].m_Texture] +
                    columns[(int)Columns.BEHAV_1].m_TypeNames[m_CLPS[i].m_Traction] +
                    columns[(int)Columns.BEHAV_2].m_TypeNames[m_CLPS[i].m_Behav] +
                    "w/ " +
                    columns[(int)Columns.CAM_THROUGH].m_TypeNames[m_CLPS[i].m_CamThrough] +
                    columns[(int)Columns.CAM_BEHAV].m_TypeNames[m_CLPS[i].m_CamBehav] +
                    "Camera" +
                    (m_CLPS[i].m_ViewID != 0x3f ? ", View ID " + m_CLPS[i].m_ViewID.ToString() : "") +
                    (m_CLPS[i].m_WindID != 0xff ? ", Wind Path ID " + m_CLPS[i].m_WindID.ToString() : "");
            }

        }

        void gridCLPSData_CellEndEdit(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex != columns.Length)
                {
                    ulong clps = m_CLPS[e.RowIndex].flags;

                    columns[e.ColumnIndex].SetFlag(ref clps,
                        ulong.Parse(gridCLPSData.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()));

                    CLPS.Entry temp = new CLPS.Entry();
                    temp.flags = clps;
                    m_CLPS[e.RowIndex] = temp;
                }
                else
                {
                    return;
                }

                LoadCLPSData();
            }
            catch (Exception ex)
            {
                new ExceptionMessageBox(ex).ShowDialog();
                return;
            }
        }

        private void CopyCLPS(int sourceLevel)
        {
            NitroOverlay otherOVL = new NitroOverlay(Program.m_ROM, Program.m_ROM.GetLevelOverlayID(sourceLevel));

            uint other_clps_addr = otherOVL.ReadPointer(0x60);
            ushort other_clps_num = otherOVL.Read16(other_clps_addr + 0x06);
            uint other_clps_size = (uint)(8 + (other_clps_num * 8));

            m_CLPS = new CLPS();
            for(int i = 0; i < other_clps_num; ++i)
            {
                ulong flags   = otherOVL.Read32((uint)(other_clps_addr + 8 + 8 * i + 0));
                flags |= (ulong)otherOVL.Read32((uint)(other_clps_addr + 8 + 8 * i + 4)) << 32;
                CLPS.Entry clps = new CLPS.Entry();
                clps.flags = flags;
                m_CLPS.Add(clps);
            }

            LoadCLPSData();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (cbxLevels.SelectedIndex != -1)
                CopyCLPS(cbxLevels.SelectedIndex);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            CLPS.Entry clps = new CLPS.Entry();
            clps.flags = 0x000000ff00000fc0;

            if (gridCLPSData.SelectedRows.Count == 0)
                m_CLPS.Add(clps);
            else
                m_CLPS.m_Entries.Insert(gridCLPSData.SelectedRows[0].Index, clps);
            
            // Reload data
            LoadCLPSData();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (gridCLPSData.SelectedRows.Count == 0)
                MessageBox.Show("Please select a row to delete.");
            else
            {
                m_CLPS.m_Entries.RemoveAt(gridCLPSData.SelectedRows[0].Index);
                // Reload data
                LoadCLPSData();
            }
        }

        private void btnShiftUp_Click(object sender, EventArgs e)
        {
            if (gridCLPSData.SelectedRows.Count == 0)
                MessageBox.Show("Please select a row to move.");
            else if (gridCLPSData.SelectedRows[0].Index != 0)
            {
                CLPS.Entry temp = m_CLPS[gridCLPSData.SelectedRows[0].Index];
                m_CLPS[gridCLPSData.SelectedRows[0].Index] = m_CLPS[gridCLPSData.SelectedRows[0].Index - 1];
                m_CLPS[gridCLPSData.SelectedRows[0].Index - 1] = temp;
            }
            LoadCLPSData();
        }

        private void btnShiftDown_Click(object sender, EventArgs e)
        {
            if (gridCLPSData.SelectedRows.Count == 0)
                MessageBox.Show("Please select a row to move.");
            else if (gridCLPSData.SelectedRows[0].Index != gridCLPSData.Rows.Count - 1)
            {
                CLPS.Entry temp = m_CLPS[gridCLPSData.SelectedRows[0].Index];
                m_CLPS[gridCLPSData.SelectedRows[0].Index] = m_CLPS[gridCLPSData.SelectedRows[0].Index + 1];
                m_CLPS[gridCLPSData.SelectedRows[0].Index + 1] = temp;
            }
            LoadCLPSData();
        }
    }
}
