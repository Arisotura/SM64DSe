namespace SM64DSe
{
    partial class LevelDisplaySettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.checkTextures = new System.Windows.Forms.CheckBox();
            this.checkVtxColors = new System.Windows.Forms.CheckBox();
            this.checkWireframe = new System.Windows.Forms.CheckBox();
            this.checkPolylistTypes = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // checkTextures
            // 
            this.checkTextures.AutoSize = true;
            this.checkTextures.Location = new System.Drawing.Point(12, 12);
            this.checkTextures.Name = "checkTextures";
            this.checkTextures.Size = new System.Drawing.Size(97, 17);
            this.checkTextures.TabIndex = 7;
            this.checkTextures.Text = "Show Textures";
            this.checkTextures.UseVisualStyleBackColor = true;
            this.checkTextures.Click += new System.EventHandler(this.checkTextures_Click);
            // 
            // checkVtxColors
            // 
            this.checkVtxColors.AutoSize = true;
            this.checkVtxColors.Location = new System.Drawing.Point(12, 35);
            this.checkVtxColors.Name = "checkVtxColors";
            this.checkVtxColors.Size = new System.Drawing.Size(124, 17);
            this.checkVtxColors.TabIndex = 8;
            this.checkVtxColors.Text = "Show Vertex Colours";
            this.checkVtxColors.UseVisualStyleBackColor = true;
            this.checkVtxColors.Click += new System.EventHandler(this.checkVtxColors_Click);
            // 
            // checkWireframe
            // 
            this.checkWireframe.AutoSize = true;
            this.checkWireframe.Location = new System.Drawing.Point(12, 58);
            this.checkWireframe.Name = "checkWireframe";
            this.checkWireframe.Size = new System.Drawing.Size(104, 17);
            this.checkWireframe.TabIndex = 9;
            this.checkWireframe.Text = "Show Wireframe";
            this.checkWireframe.UseVisualStyleBackColor = true;
            this.checkWireframe.Click += new System.EventHandler(this.checkWireframe_Click);
            // 
            // checkPolylistTypes
            // 
            this.checkPolylistTypes.AutoSize = true;
            this.checkPolylistTypes.Location = new System.Drawing.Point(12, 81);
            this.checkPolylistTypes.Name = "checkPolylistTypes";
            this.checkPolylistTypes.Size = new System.Drawing.Size(139, 17);
            this.checkPolylistTypes.TabIndex = 10;
            this.checkPolylistTypes.Text = "Indicate Draw Call Type";
            this.checkPolylistTypes.UseVisualStyleBackColor = true;
            this.checkPolylistTypes.Click += new System.EventHandler(this.checkPolylistTypes_Click);
            // 
            // LevelDisplaySettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(233, 116);
            this.Controls.Add(this.checkPolylistTypes);
            this.Controls.Add(this.checkWireframe);
            this.Controls.Add(this.checkVtxColors);
            this.Controls.Add(this.checkTextures);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LevelDisplaySettingsForm";
            this.Text = "Level Display Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox checkTextures;
        private System.Windows.Forms.CheckBox checkVtxColors;
        private System.Windows.Forms.CheckBox checkWireframe;
        private System.Windows.Forms.CheckBox checkPolylistTypes;
    }
}