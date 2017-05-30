using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace SM64DSe
{
    public partial class OffsetAllObjectCoordsForm : Form
    {
        LevelEditorForm _owner;

        public OffsetAllObjectCoordsForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this._owner = (LevelEditorForm)Owner;

            int[] movableTypes = new int[] { 0, 5, 1, 10, 9, 2, 4, 6, 7 };
            IEnumerable<LevelObject> objects = _owner.m_LevelObjects.Values.Where(obj => movableTypes.Contains(obj.m_Type));
            try
            {
                foreach (LevelObject obj in objects)
                {
                    if (chkOffsetAll.Checked)
                    {
                        obj.Position.X += Helper.ParseFloat(txtOffsetX.Text);
                        obj.Position.Y += Helper.ParseFloat(txtOffsetY.Text);
                        obj.Position.Z += Helper.ParseFloat(txtOffsetZ.Text);
                    }
                    if (chkScaleAll.Checked)
                    {
                        obj.Position.X *= Helper.ParseFloat(txtScaleX.Text);
                        obj.Position.Y *= Helper.ParseFloat(txtScaleY.Text);
                        obj.Position.Z *= Helper.ParseFloat(txtScaleZ.Text);
                    }
                    obj.GenerateProperties();
                }
                for (int i = 0; i < 8; i++)
                    _owner.RefreshObjects(i);
            }
            catch { MessageBox.Show("Please enter a valid value in the format x.xxxxxx"); }
        }
    }
}
