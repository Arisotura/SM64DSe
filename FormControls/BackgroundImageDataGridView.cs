using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace SM64DSe.FormControls
{
    public class BackgroundImageDataGridView : DataGridView
    {
        Image m_Image;
        public Image Img
        {
            get { return m_Image; }
            set { m_Image = value; }
        }

        public BackgroundImageDataGridView()
            : base()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void PaintBackground(Graphics graphics, Rectangle clipBounds, Rectangle
         gridBounds)
        {
            //base.PaintBackground(graphics, clipBounds, gridBounds);
            if (m_Image != null)
            {
                graphics.DrawImage(m_Image, 2, 2, m_Image.Width * 2, m_Image.Height * 2);
            }
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            base.OnCellPainting(e);
            if (e.ColumnIndex > -1 && e.RowIndex > -1)
            {
                if (this[e.ColumnIndex, e.RowIndex].Value == null || this[e.ColumnIndex, e.RowIndex].Value.Equals(""))
                    this[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.Transparent;
                else
                    this[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.CornflowerBlue;
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            DataGridView.HitTestInfo hti = this.HitTest(e.X, e.Y);
            if (hti.Type == DataGridViewHitTestType.Cell)
            {
                DataGridViewCell c = this[hti.ColumnIndex, hti.RowIndex];
                if (c.Value == null || c.Value.Equals(""))
                {
                    c.Style.BackColor = Color.Transparent;
                    c.Style.SelectionBackColor = Color.Transparent;
                }
                else
                {
                    c.Style.BackColor = Color.CornflowerBlue;
                    c.Style.SelectionBackColor = Color.CornflowerBlue;
                }
            }
            base.OnMouseClick(e);
        }
    }
}
