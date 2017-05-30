namespace SM64DSe
{
    partial class SettingsForm
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.cbAutoUpdateODB = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.grpGeneralSettings = new System.Windows.Forms.GroupBox();
            this.grpModelAndCollisionMapImportationSettings = new System.Windows.Forms.GroupBox();
            this.chkRememberLastUsedCollisionTypeAssignments = new System.Windows.Forms.CheckBox();
            this.chkDisableTextureSizeWarning = new System.Windows.Forms.CheckBox();
            this.chkRememberLastUsedModelImportationSettings = new System.Windows.Forms.CheckBox();
            this.chkUseSimpleModelAndCollisionMapImporters = new System.Windows.Forms.CheckBox();
            this.grpGeneralSettings.SuspendLayout();
            this.grpModelAndCollisionMapImportationSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbAutoUpdateODB
            // 
            this.cbAutoUpdateODB.AutoSize = true;
            this.cbAutoUpdateODB.Location = new System.Drawing.Point(6, 19);
            this.cbAutoUpdateODB.Name = "cbAutoUpdateODB";
            this.cbAutoUpdateODB.Size = new System.Drawing.Size(163, 17);
            this.cbAutoUpdateODB.TabIndex = 0;
            this.cbAutoUpdateODB.Text = "Auto-update object database";
            this.cbAutoUpdateODB.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(199, 187);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(118, 187);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // grpGeneralSettings
            // 
            this.grpGeneralSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpGeneralSettings.Controls.Add(this.cbAutoUpdateODB);
            this.grpGeneralSettings.Location = new System.Drawing.Point(12, 12);
            this.grpGeneralSettings.Name = "grpGeneralSettings";
            this.grpGeneralSettings.Size = new System.Drawing.Size(262, 46);
            this.grpGeneralSettings.TabIndex = 3;
            this.grpGeneralSettings.TabStop = false;
            this.grpGeneralSettings.Text = "General Settings";
            // 
            // grpModelAndCollisionMapImportationSettings
            // 
            this.grpModelAndCollisionMapImportationSettings.Controls.Add(this.chkUseSimpleModelAndCollisionMapImporters);
            this.grpModelAndCollisionMapImportationSettings.Controls.Add(this.chkRememberLastUsedCollisionTypeAssignments);
            this.grpModelAndCollisionMapImportationSettings.Controls.Add(this.chkDisableTextureSizeWarning);
            this.grpModelAndCollisionMapImportationSettings.Controls.Add(this.chkRememberLastUsedModelImportationSettings);
            this.grpModelAndCollisionMapImportationSettings.Location = new System.Drawing.Point(13, 65);
            this.grpModelAndCollisionMapImportationSettings.Name = "grpModelAndCollisionMapImportationSettings";
            this.grpModelAndCollisionMapImportationSettings.Size = new System.Drawing.Size(261, 114);
            this.grpModelAndCollisionMapImportationSettings.TabIndex = 4;
            this.grpModelAndCollisionMapImportationSettings.TabStop = false;
            this.grpModelAndCollisionMapImportationSettings.Text = "Model and Collision Map Importation Settings";
            // 
            // chkRememberLastUsedCollisionTypeAssignments
            // 
            this.chkRememberLastUsedCollisionTypeAssignments.AutoSize = true;
            this.chkRememberLastUsedCollisionTypeAssignments.Location = new System.Drawing.Point(7, 67);
            this.chkRememberLastUsedCollisionTypeAssignments.Name = "chkRememberLastUsedCollisionTypeAssignments";
            this.chkRememberLastUsedCollisionTypeAssignments.Size = new System.Drawing.Size(246, 17);
            this.chkRememberLastUsedCollisionTypeAssignments.TabIndex = 2;
            this.chkRememberLastUsedCollisionTypeAssignments.Text = "Remember last used collision type assignments";
            this.chkRememberLastUsedCollisionTypeAssignments.UseVisualStyleBackColor = true;
            // 
            // chkDisableTextureSizeWarning
            // 
            this.chkDisableTextureSizeWarning.AutoSize = true;
            this.chkDisableTextureSizeWarning.Location = new System.Drawing.Point(7, 90);
            this.chkDisableTextureSizeWarning.Name = "chkDisableTextureSizeWarning";
            this.chkDisableTextureSizeWarning.Size = new System.Drawing.Size(157, 17);
            this.chkDisableTextureSizeWarning.TabIndex = 1;
            this.chkDisableTextureSizeWarning.Text = "Disable texture size warning";
            this.chkDisableTextureSizeWarning.UseVisualStyleBackColor = true;
            // 
            // chkRememberLastUsedModelImportationSettings
            // 
            this.chkRememberLastUsedModelImportationSettings.AutoSize = true;
            this.chkRememberLastUsedModelImportationSettings.Location = new System.Drawing.Point(7, 43);
            this.chkRememberLastUsedModelImportationSettings.Name = "chkRememberLastUsedModelImportationSettings";
            this.chkRememberLastUsedModelImportationSettings.Size = new System.Drawing.Size(246, 17);
            this.chkRememberLastUsedModelImportationSettings.TabIndex = 0;
            this.chkRememberLastUsedModelImportationSettings.Text = "Remember last used model importation settings";
            this.chkRememberLastUsedModelImportationSettings.UseVisualStyleBackColor = true;
            // 
            // chkUseSimpleModelAndCollisionMapImporters
            // 
            this.chkUseSimpleModelAndCollisionMapImporters.AutoSize = true;
            this.chkUseSimpleModelAndCollisionMapImporters.Location = new System.Drawing.Point(7, 20);
            this.chkUseSimpleModelAndCollisionMapImporters.Name = "chkUseSimpleModelAndCollisionMapImporters";
            this.chkUseSimpleModelAndCollisionMapImporters.Size = new System.Drawing.Size(237, 17);
            this.chkUseSimpleModelAndCollisionMapImporters.TabIndex = 3;
            this.chkUseSimpleModelAndCollisionMapImporters.Text = "Use simple model and collision map importers";
            this.chkUseSimpleModelAndCollisionMapImporters.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(286, 222);
            this.Controls.Add(this.grpModelAndCollisionMapImportationSettings);
            this.Controls.Add(this.grpGeneralSettings);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.grpGeneralSettings.ResumeLayout(false);
            this.grpGeneralSettings.PerformLayout();
            this.grpModelAndCollisionMapImportationSettings.ResumeLayout(false);
            this.grpModelAndCollisionMapImportationSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox cbAutoUpdateODB;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.GroupBox grpGeneralSettings;
        private System.Windows.Forms.GroupBox grpModelAndCollisionMapImportationSettings;
        private System.Windows.Forms.CheckBox chkRememberLastUsedModelImportationSettings;
        private System.Windows.Forms.CheckBox chkDisableTextureSizeWarning;
        private System.Windows.Forms.CheckBox chkRememberLastUsedCollisionTypeAssignments;
        private System.Windows.Forms.CheckBox chkUseSimpleModelAndCollisionMapImporters;
    }
}