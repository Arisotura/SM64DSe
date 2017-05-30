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
    public partial class ImportImage4BPP : Form
    {
        private Bitmap m_Image;
        private string m_ImageName;

        private int m_CurrentPaletteRow = 1;
        private int m_NumTilesX;
        private int m_NumTilesY;

        private bool m_ImageLoaded = false;

        MinimapEditor _owner;

        public ImportImage4BPP()
        {
            InitializeComponent();
        }

        private void ImportImage4BPP_Load(object sender, EventArgs e)
        {
            this._owner = (MinimapEditor)Owner;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select an Indexed Bitmap Image 4BPP";
            ofd.Filter = "Bitmap (.bmp)|*.bmp";
            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.OK)
            {
                try
                {
                    m_ImageName = ofd.FileName;
                    m_Image = new Bitmap(m_ImageName);

                    gridImage.Img = m_Image;

                    LoadImageInGrid();

                    m_ImageLoaded = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("There was an error opeing the image:\n\n" +
                        ofd.FileName + "\n\n" + ex.Message);
                    this.Close();
                }
            }
        }

        private void LoadImageInGrid()
        {
            int width = m_Image.Width;
            int height = m_Image.Height;
            m_NumTilesX = width / 8;
            m_NumTilesY = height / 8;

            gridImage.ColumnCount = m_NumTilesX;
            gridImage.RowCount = m_NumTilesY;

            gridImage.Width = width * 2 + (m_NumTilesX * 2);
            gridImage.Height = height * 2 + (m_NumTilesY * 2);

            if (this.Width <= gridImage.Width) this.Width = gridImage.Width + 16;
            if (this.Height <= gridImage.Height) this.Height = gridImage.Height + 16;

            for (int row = 0; row < gridImage.RowCount; row++)
            {
                gridImage.Rows[row].Height = 16; 
            }
            for (int col = 0; col < gridImage.ColumnCount; col++)
            {
                gridImage.Columns[col].Width = 16;
            }

            gridImage.CurrentCell.Selected = false;
        }

        private void AllRowRadioButtons_CheckedChanged(Object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                RadioButton rb = (RadioButton)sender;
                m_CurrentPaletteRow = int.Parse(rb.Text);
            }
        }

        private void gridImage_SelectionChanged(object sender, EventArgs e)
        {
            if (gridImage.SelectedCells.Count <= 0 || !m_ImageLoaded) return;

            for (int i = 0; i < gridImage.SelectedCells.Count; i++)
            {
                int row = gridImage.SelectedCells[i].RowIndex;
                int col = gridImage.SelectedCells[i].ColumnIndex;

                if (row < 0 || col < 0) return;

                if (gridImage.Rows[row].Cells[col].Value != null && 
                    gridImage.Rows[row].Cells[col].Value.Equals(m_CurrentPaletteRow.ToString()))
                {
                    gridImage.Rows[row].Cells[col].Value = null;
                }
                else
                {
                    gridImage.Rows[row].Cells[col].Value = m_CurrentPaletteRow.ToString();
                }
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            byte[] tilePaletteRows = new byte[m_NumTilesX * m_NumTilesY];

            int cell = 0;
            for (int row = 0; row < m_NumTilesY; row++)
            {
                for (int col = 0; col < m_NumTilesX; col++)
                {
                    byte value = (gridImage.Rows[row].Cells[col].Value == null || gridImage.Rows[row].Cells[col].Value.Equals("")) ?
                        (byte)0 : (byte)(byte.Parse(gridImage.Rows[row].Cells[col].Value.ToString()) - 1);
                    tilePaletteRows[cell++] = value;
                }
            }

            _owner.ImportBMP_4BPP(m_ImageName, m_Image.Width, m_Image.Height, m_NumTilesX, m_NumTilesY, tilePaletteRows);
        }
    }
}
