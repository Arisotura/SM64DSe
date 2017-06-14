/*
    Copyright 2012 Kuribo64

    This file is part of SM64DSe.

    SM64DSe is free software: you can redistribute it and/or modify it under
    the terms of the GNU General Public License as published by the Free
    Software Foundation, either version 3 of the License, or (at your option)
    any later version.

    SM64DSe is distributed in the hope that it will be useful, but WITHOUT ANY 
    WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS 
    FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along 
    with SM64DSe. If not, see http://www.gnu.org/licenses/.
*/

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
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            cbAutoUpdateODB.Checked = Properties.Settings.Default.AutoUpdateODB;
            
            chkUseSimpleModelAndCollisionMapImporters.Checked = Properties.Settings.Default.UseSimpleModelAndCollisionMapImporters;
            chkRememberLastUsedModelImportationSettings.Enabled = Properties.Settings.Default.UseSimpleModelAndCollisionMapImporters;
            chkRememberLastUsedCollisionTypeAssignments.Enabled = Properties.Settings.Default.UseSimpleModelAndCollisionMapImporters;
            chkRememberLastUsedModelImportationSettings.Checked = Properties.Settings.Default.RememberLastUsedModelImportationSettings;
            chkRememberLastUsedCollisionTypeAssignments.Checked = Properties.Settings.Default.RememberMaterialCollisionTypeAssignments;
            chkDisableTextureSizeWarning.Checked = Properties.Settings.Default.DisableTextureSizeWarning;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutoUpdateODB = cbAutoUpdateODB.Checked;
            
            Properties.Settings.Default.UseSimpleModelAndCollisionMapImporters = chkUseSimpleModelAndCollisionMapImporters.Checked;
            Properties.Settings.Default.RememberLastUsedModelImportationSettings = chkRememberLastUsedModelImportationSettings.Checked;
            Properties.Settings.Default.RememberMaterialCollisionTypeAssignments = chkRememberLastUsedCollisionTypeAssignments.Checked;
            Properties.Settings.Default.DisableTextureSizeWarning = chkDisableTextureSizeWarning.Checked;
            Properties.Settings.Default.Save();
        }
    }
}
