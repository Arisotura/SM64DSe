namespace SM64DSe
{
    partial class SoundViewForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SoundViewForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabSWAR = new System.Windows.Forms.TabPage();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnPlay = new System.Windows.Forms.ToolStripButton();
            this.btnStop = new System.Windows.Forms.ToolStripButton();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.tvSWAR = new System.Windows.Forms.TreeView();
            this.tabSBNK = new System.Windows.Forms.TabPage();
            this.tabSSEQ = new System.Windows.Forms.TabPage();
            this.tabSSAR = new System.Windows.Forms.TabPage();
            this.tabGroup = new System.Windows.Forms.TabPage();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tabControl1.SuspendLayout();
            this.tabSWAR.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabSWAR);
            this.tabControl1.Controls.Add(this.tabSBNK);
            this.tabControl1.Controls.Add(this.tabSSEQ);
            this.tabControl1.Controls.Add(this.tabSSAR);
            this.tabControl1.Controls.Add(this.tabGroup);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(736, 425);
            this.tabControl1.TabIndex = 0;
            // 
            // tabSWAR
            // 
            this.tabSWAR.Controls.Add(this.toolStrip1);
            this.tabSWAR.Controls.Add(this.splitter1);
            this.tabSWAR.Controls.Add(this.tvSWAR);
            this.tabSWAR.Location = new System.Drawing.Point(4, 22);
            this.tabSWAR.Name = "tabSWAR";
            this.tabSWAR.Padding = new System.Windows.Forms.Padding(3);
            this.tabSWAR.Size = new System.Drawing.Size(728, 399);
            this.tabSWAR.TabIndex = 0;
            this.tabSWAR.Text = "Wave Archive";
            this.tabSWAR.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnPlay,
            this.btnStop});
            this.toolStrip1.Location = new System.Drawing.Point(271, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(454, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnPlay
            // 
            this.btnPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPlay.Enabled = false;
            this.btnPlay.Image = global::SM64DSe.Properties.Resources.Play;
            this.btnPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(23, 22);
            this.btnPlay.Text = "Play";
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnStop
            // 
            this.btnStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnStop.Image = global::SM64DSe.Properties.Resources.Stop;
            this.btnStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(23, 22);
            this.btnStop.Text = "Stop";
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(268, 3);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 393);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // tvSWAR
            // 
            this.tvSWAR.Dock = System.Windows.Forms.DockStyle.Left;
            this.tvSWAR.Location = new System.Drawing.Point(3, 3);
            this.tvSWAR.Name = "tvSWAR";
            this.tvSWAR.Size = new System.Drawing.Size(265, 393);
            this.tvSWAR.TabIndex = 0;
            this.tvSWAR.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvSWAR_AfterSelect);
            this.tvSWAR.DoubleClick += new System.EventHandler(this.tvSWAR_DoubleClick);
            // 
            // tabSBNK
            // 
            this.tabSBNK.Location = new System.Drawing.Point(4, 22);
            this.tabSBNK.Name = "tabSBNK";
            this.tabSBNK.Padding = new System.Windows.Forms.Padding(3);
            this.tabSBNK.Size = new System.Drawing.Size(728, 399);
            this.tabSBNK.TabIndex = 1;
            this.tabSBNK.Text = "Instrument Bank";
            this.tabSBNK.UseVisualStyleBackColor = true;
            // 
            // tabSSEQ
            // 
            this.tabSSEQ.Location = new System.Drawing.Point(4, 22);
            this.tabSSEQ.Name = "tabSSEQ";
            this.tabSSEQ.Padding = new System.Windows.Forms.Padding(3);
            this.tabSSEQ.Size = new System.Drawing.Size(728, 399);
            this.tabSSEQ.TabIndex = 2;
            this.tabSSEQ.Text = "Sequence";
            this.tabSSEQ.UseVisualStyleBackColor = true;
            // 
            // tabSSAR
            // 
            this.tabSSAR.Location = new System.Drawing.Point(4, 22);
            this.tabSSAR.Name = "tabSSAR";
            this.tabSSAR.Padding = new System.Windows.Forms.Padding(3);
            this.tabSSAR.Size = new System.Drawing.Size(728, 399);
            this.tabSSAR.TabIndex = 3;
            this.tabSSAR.Text = "Sequence Archive";
            this.tabSSAR.UseVisualStyleBackColor = true;
            // 
            // tabGroup
            // 
            this.tabGroup.Location = new System.Drawing.Point(4, 22);
            this.tabGroup.Name = "tabGroup";
            this.tabGroup.Padding = new System.Windows.Forms.Padding(3);
            this.tabGroup.Size = new System.Drawing.Size(728, 399);
            this.tabGroup.TabIndex = 4;
            this.tabGroup.Text = "Group";
            this.tabGroup.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(736, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // SoundViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(736, 449);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SoundViewForm";
            this.Text = "Sound Browser";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SoundViewForm_FormClosed);
            this.tabControl1.ResumeLayout(false);
            this.tabSWAR.ResumeLayout(false);
            this.tabSWAR.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabSWAR;
        private System.Windows.Forms.TabPage tabSBNK;
        private System.Windows.Forms.TabPage tabSSEQ;
        private System.Windows.Forms.TabPage tabSSAR;
        private System.Windows.Forms.TabPage tabGroup;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.TreeView tvSWAR;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnPlay;
        private System.Windows.Forms.ToolStripButton btnStop;
    }
}