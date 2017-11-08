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
        public static readonly LevelObject.Type[] k_MovableLevelObjectTypes = new LevelObject.Type[] {
            LevelObject.Type.STANDARD, 
            LevelObject.Type.SIMPLE, 
            LevelObject.Type.ENTRANCE, 
            LevelObject.Type.EXIT, 
            LevelObject.Type.DOOR, 
            LevelObject.Type.PATH_NODE, 
            LevelObject.Type.VIEW, 
            LevelObject.Type.TELEPORT_SOURCE, 
            LevelObject.Type.TELEPORT_DESTINATION
        };

        protected LevelEditorForm _owner;

        public OffsetAllObjectCoordsForm()
        {
            InitializeComponent();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            this._owner = (LevelEditorForm)Owner;

            IEnumerable<LevelObject> objects =
                _owner.m_Level.m_LevelObjects.Values.Where(obj => k_MovableLevelObjectTypes.Contains(obj.m_Type));
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
                for (int area = 0; area < Level.k_MaxNumAreas; area++)
                    _owner.RefreshObjects(area);
            }
            catch { MessageBox.Show("Please enter a valid value in the format x.xxxxxx"); }
        }
    }
}
