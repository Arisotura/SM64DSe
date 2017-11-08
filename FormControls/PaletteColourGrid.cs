using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace SM64DSe
{
    public class PaletteColourGrid : DataGridView
    {
        private static readonly int MAX_COLUMN_COUNT = 16;
        private static readonly int WIDTH_256_COLOURS = 260;
        private static readonly int WIDTH_OVER_256_COLOURS = 275;

        private Color[] m_Colours;
        private int m_NumColours;

        public PaletteColourGrid()
            : base()
        {
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.AllowUserToResizeColumns = false;
            this.AllowUserToResizeRows = false;
            this.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.ColumnHeadersVisible = false;
            this.EnableHeadersVisualStyles = false;
            this.MultiSelect = false;
            this.ReadOnly = true;
            this.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.RowHeadersVisible = false;
            this.RowHeadersWidth = 23;
            this.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.RowTemplate.Height = 16;
            this.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.Size = new System.Drawing.Size(WIDTH_256_COLOURS, WIDTH_256_COLOURS);

            this.CellStateChanged += new DataGridViewCellStateChangedEventHandler(Palette256ColourGrid_CellStateChanged);
        }

        protected void Palette256ColourGrid_CellStateChanged(object sender, DataGridViewCellStateChangedEventArgs e)
        {
            if (e.Cell == null || e.StateChanged != DataGridViewElementStates.Selected)
            {
                return;
            }
            else if (((ColumnCount * e.Cell.RowIndex) + e.Cell.ColumnIndex) >= m_NumColours)
            {
                e.Cell.Selected = false;
            }
        }

        public void SetColours(Color[] colours)
        {
            m_NumColours = colours.Length;
            m_Colours = new Color[m_NumColours];
            Array.Copy(colours, m_Colours, m_NumColours);

            // Clear all cells first
            RowCount = 0;
            ColumnCount = 0;
            Refresh();

            Width = (m_NumColours > 256) ? WIDTH_OVER_256_COLOURS : WIDTH_256_COLOURS;

            // The below line in theory should be fine however when running it always evaluates to 0 but
            // the expression evaluation debugger tool will return the correct value. Bug?
            //int nRows = ((m_NumColours % 16 != 0) ? ((m_NumColours + 16) & ~15) : m_NumColours) / 16;
            int paddedColourCount = m_NumColours;
            if (m_NumColours % MAX_COLUMN_COUNT != 0)
            {
                paddedColourCount = ((m_NumColours + MAX_COLUMN_COUNT) & ~15);
            }
            int nRows = paddedColourCount / MAX_COLUMN_COUNT;
            int nColumns = (nRows > 1) ? MAX_COLUMN_COUNT : m_NumColours;

            RowCount = nRows;
            ColumnCount = nColumns;

            // Set cells to size 16x16
            for (int i = 0; i < RowCount; i++)
            {
                Rows[i].Height = 16;
            }
            for (int i = 0; i < ColumnCount; i++)
            {
                Columns[i].Width = 16;
            }

            // Set cell colours
            int clr = 0;
            for (int row = 0; row < nRows; row++)
            {
                for (int column = 0; column < nColumns; column++)
                {
                    if (clr >= m_NumColours)
                    {
                        if (nRows <= 1)
                        {
                            break;
                        }
                        else
                        {
                            Rows[row].Cells[column].Style.BackColor = Color.Transparent;
                        }
                    }
                    else
                    {
                        Rows[row].Cells[column].Style.BackColor = m_Colours[clr];
                    }
                    clr++;
                }
            }

            // Select none by default
            if (IsAColourSelected())
            {
                CurrentCell.Selected = false; 
            }

            Refresh();
        }

        public void SetColourAtIndex(Color colour, int index)
        {
            if (index >= m_NumColours || index < 0) throw new IndexOutOfRangeException();

            m_Colours[index] = colour;
            int row = index / MAX_COLUMN_COUNT;
            int column = index % MAX_COLUMN_COUNT;

            Rows[row].Cells[column].Style.BackColor = colour;
            Refresh();
        }

        public void ClearColours()
        {
            SetColours(new Color[0]);
        }

        public bool IsAColourSelected()
        {
            return (IsACellSelected()) ? (GetSelectedIndex() < m_NumColours) : false;
        }

        public int GetSelectedColourIndex()
        {
            return (IsAColourSelected()) ? GetSelectedIndex() : -1;
        }

        public Color? GetSelectedColour()
        {
            int selectedColourIndex = GetSelectedColourIndex();
            return (selectedColourIndex > -1) ? m_Colours[selectedColourIndex] : ((Color?) null);
        }

        protected bool IsACellSelected()
        {
            return (CurrentCell != null);
        }

        protected int GetSelectedIndex()
        {
            return (IsACellSelected()) ? ((ColumnCount * CurrentCell.RowIndex) + CurrentCell.ColumnIndex) : -1;
        }
    }
}
