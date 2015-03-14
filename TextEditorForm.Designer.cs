namespace SM64DSe
{
    partial class TextEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextEditorForm));
            this.splitCVertical = new System.Windows.Forms.SplitContainer();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lbxMsgList = new System.Windows.Forms.ListBox();
            this.splitCHorizontal = new System.Windows.Forms.SplitContainer();
            this.tbxMsgPreview = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtEdit = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnCoins = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.btnUpdateString = new System.Windows.Forms.Button();
            this.btnSaveAll = new System.Windows.Forms.Button();
            this.btnStarFull = new System.Windows.Forms.Button();
            this.btnStarEmpty = new System.Windows.Forms.Button();
            this.btnR = new System.Windows.Forms.Button();
            this.btnDPad = new System.Windows.Forms.Button();
            this.btnL = new System.Windows.Forms.Button();
            this.btnA = new System.Windows.Forms.Button();
            this.btnY = new System.Windows.Forms.Button();
            this.btnB = new System.Windows.Forms.Button();
            this.btnX = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnImport = new System.Windows.Forms.ToolStripButton();
            this.btnExport = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.lblVer = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnLanguages = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnHelp = new System.Windows.Forms.ToolStripButton();
            //((System.ComponentModel.ISupportInitialize)(this.splitCVertical)).BeginInit();
            this.splitCVertical.Panel1.SuspendLayout();
            this.splitCVertical.Panel2.SuspendLayout();
            this.splitCVertical.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(this.splitCHorizontal)).BeginInit();
            this.splitCHorizontal.Panel1.SuspendLayout();
            this.splitCHorizontal.Panel2.SuspendLayout();
            this.splitCHorizontal.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitCVertical
            // 
            this.splitCVertical.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitCVertical.Location = new System.Drawing.Point(0, 0);
            this.splitCVertical.Name = "splitCVertical";
            // 
            // splitCVertical.Panel1
            // 
            this.splitCVertical.Panel1.Controls.Add(this.label2);
            this.splitCVertical.Panel1.Controls.Add(this.txtSearch);
            this.splitCVertical.Panel1.Controls.Add(this.lbxMsgList);
            // 
            // splitCVertical.Panel2
            // 
            this.splitCVertical.Panel2.Controls.Add(this.splitCHorizontal);
            this.splitCVertical.Panel2.Controls.Add(this.toolStrip1);
            this.splitCVertical.Size = new System.Drawing.Size(793, 515);
            this.splitCVertical.SplitterDistance = 223;
            this.splitCVertical.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Search:";
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Location = new System.Drawing.Point(46, 3);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(178, 20);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // lbxMsgList
            // 
            this.lbxMsgList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbxMsgList.FormattingEnabled = true;
            this.lbxMsgList.Location = new System.Drawing.Point(0, 26);
            this.lbxMsgList.Name = "lbxMsgList";
            this.lbxMsgList.Size = new System.Drawing.Size(223, 485);
            this.lbxMsgList.TabIndex = 0;
            this.lbxMsgList.SelectedIndexChanged += new System.EventHandler(this.lbxMsgList_SelectedIndexChanged);
            // 
            // splitCHorizontal
            // 
            this.splitCHorizontal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitCHorizontal.Location = new System.Drawing.Point(0, 25);
            this.splitCHorizontal.Name = "splitCHorizontal";
            this.splitCHorizontal.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitCHorizontal.Panel1
            // 
            this.splitCHorizontal.Panel1.Controls.Add(this.tbxMsgPreview);
            // 
            // splitCHorizontal.Panel2
            // 
            this.splitCHorizontal.Panel2.Controls.Add(this.label1);
            this.splitCHorizontal.Panel2.Controls.Add(this.txtEdit);
            this.splitCHorizontal.Panel2.Controls.Add(this.label5);
            this.splitCHorizontal.Panel2.Controls.Add(this.btnCoins);
            this.splitCHorizontal.Panel2.Controls.Add(this.label4);
            this.splitCHorizontal.Panel2.Controls.Add(this.btnUpdateString);
            this.splitCHorizontal.Panel2.Controls.Add(this.btnSaveAll);
            this.splitCHorizontal.Panel2.Controls.Add(this.btnStarFull);
            this.splitCHorizontal.Panel2.Controls.Add(this.btnStarEmpty);
            this.splitCHorizontal.Panel2.Controls.Add(this.btnR);
            this.splitCHorizontal.Panel2.Controls.Add(this.btnDPad);
            this.splitCHorizontal.Panel2.Controls.Add(this.btnL);
            this.splitCHorizontal.Panel2.Controls.Add(this.btnA);
            this.splitCHorizontal.Panel2.Controls.Add(this.btnY);
            this.splitCHorizontal.Panel2.Controls.Add(this.btnB);
            this.splitCHorizontal.Panel2.Controls.Add(this.btnX);
            this.splitCHorizontal.Size = new System.Drawing.Size(566, 490);
            this.splitCHorizontal.SplitterDistance = 245;
            this.splitCHorizontal.TabIndex = 25;
            // 
            // tbxMsgPreview
            // 
            this.tbxMsgPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbxMsgPreview.Location = new System.Drawing.Point(0, 0);
            this.tbxMsgPreview.Multiline = true;
            this.tbxMsgPreview.Name = "tbxMsgPreview";
            this.tbxMsgPreview.ReadOnly = true;
            this.tbxMsgPreview.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbxMsgPreview.Size = new System.Drawing.Size(566, 245);
            this.tbxMsgPreview.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Enter your edited text here:";
            // 
            // txtEdit
            // 
            this.txtEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEdit.Location = new System.Drawing.Point(2, 19);
            this.txtEdit.Multiline = true;
            this.txtEdit.Name = "txtEdit";
            this.txtEdit.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtEdit.Size = new System.Drawing.Size(564, 115);
            this.txtEdit.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(115, 216);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(207, 13);
            this.label5.TabIndex = 22;
            this.label5.Text = "Click this to permanently save all changes.";
            // 
            // btnCoins
            // 
            this.btnCoins.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCoins.Image = global::SM64DSe.Properties.Resources.Coins1;
            this.btnCoins.Location = new System.Drawing.Point(3, 140);
            this.btnCoins.Name = "btnCoins";
            this.btnCoins.Size = new System.Drawing.Size(35, 30);
            this.btnCoins.TabIndex = 14;
            this.btnCoins.UseVisualStyleBackColor = true;
            this.btnCoins.Click += new System.EventHandler(this.btnCoins_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(115, 185);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(268, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Click this when you\'ve finished editing the current entry.";
            // 
            // btnUpdateString
            // 
            this.btnUpdateString.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUpdateString.Location = new System.Drawing.Point(6, 180);
            this.btnUpdateString.Name = "btnUpdateString";
            this.btnUpdateString.Size = new System.Drawing.Size(103, 23);
            this.btnUpdateString.TabIndex = 2;
            this.btnUpdateString.Text = "Update String";
            this.btnUpdateString.UseVisualStyleBackColor = true;
            this.btnUpdateString.Click += new System.EventHandler(this.btnUpdateString_Click);
            // 
            // btnSaveAll
            // 
            this.btnSaveAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSaveAll.Location = new System.Drawing.Point(6, 211);
            this.btnSaveAll.Name = "btnSaveAll";
            this.btnSaveAll.Size = new System.Drawing.Size(103, 23);
            this.btnSaveAll.TabIndex = 20;
            this.btnSaveAll.Text = "Save Changes";
            this.btnSaveAll.UseVisualStyleBackColor = true;
            this.btnSaveAll.Click += new System.EventHandler(this.btnSaveAll_Click);
            // 
            // btnStarFull
            // 
            this.btnStarFull.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStarFull.Image = global::SM64DSe.Properties.Resources.StarFull1;
            this.btnStarFull.Location = new System.Drawing.Point(42, 140);
            this.btnStarFull.Name = "btnStarFull";
            this.btnStarFull.Size = new System.Drawing.Size(35, 30);
            this.btnStarFull.TabIndex = 5;
            this.btnStarFull.UseVisualStyleBackColor = true;
            this.btnStarFull.Click += new System.EventHandler(this.btnStarFull_Click);
            // 
            // btnStarEmpty
            // 
            this.btnStarEmpty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStarEmpty.Image = global::SM64DSe.Properties.Resources.StarEmpty1;
            this.btnStarEmpty.Location = new System.Drawing.Point(79, 140);
            this.btnStarEmpty.Name = "btnStarEmpty";
            this.btnStarEmpty.Size = new System.Drawing.Size(35, 30);
            this.btnStarEmpty.TabIndex = 6;
            this.btnStarEmpty.UseVisualStyleBackColor = true;
            this.btnStarEmpty.Click += new System.EventHandler(this.btnStarEmpty_Click);
            // 
            // btnR
            // 
            this.btnR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnR.Image = global::SM64DSe.Properties.Resources.btnR1;
            this.btnR.Location = new System.Drawing.Point(352, 140);
            this.btnR.Name = "btnR";
            this.btnR.Size = new System.Drawing.Size(35, 30);
            this.btnR.TabIndex = 13;
            this.btnR.UseVisualStyleBackColor = true;
            this.btnR.Click += new System.EventHandler(this.btnR_Click);
            // 
            // btnDPad
            // 
            this.btnDPad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDPad.Image = global::SM64DSe.Properties.Resources.DPad1;
            this.btnDPad.Location = new System.Drawing.Point(118, 140);
            this.btnDPad.Name = "btnDPad";
            this.btnDPad.Size = new System.Drawing.Size(35, 30);
            this.btnDPad.TabIndex = 7;
            this.btnDPad.UseVisualStyleBackColor = true;
            this.btnDPad.Click += new System.EventHandler(this.btnDPad_Click);
            // 
            // btnL
            // 
            this.btnL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnL.Image = global::SM64DSe.Properties.Resources.btnL1;
            this.btnL.Location = new System.Drawing.Point(313, 140);
            this.btnL.Name = "btnL";
            this.btnL.Size = new System.Drawing.Size(35, 30);
            this.btnL.TabIndex = 12;
            this.btnL.UseVisualStyleBackColor = true;
            this.btnL.Click += new System.EventHandler(this.btnL_Click);
            // 
            // btnA
            // 
            this.btnA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnA.Image = global::SM64DSe.Properties.Resources.btnA1;
            this.btnA.Location = new System.Drawing.Point(157, 140);
            this.btnA.Name = "btnA";
            this.btnA.Size = new System.Drawing.Size(35, 30);
            this.btnA.TabIndex = 8;
            this.btnA.UseVisualStyleBackColor = true;
            this.btnA.Click += new System.EventHandler(this.btnA_Click);
            // 
            // btnY
            // 
            this.btnY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnY.Image = global::SM64DSe.Properties.Resources.btnY1;
            this.btnY.Location = new System.Drawing.Point(274, 140);
            this.btnY.Name = "btnY";
            this.btnY.Size = new System.Drawing.Size(35, 30);
            this.btnY.TabIndex = 11;
            this.btnY.UseVisualStyleBackColor = true;
            this.btnY.Click += new System.EventHandler(this.btnY_Click);
            // 
            // btnB
            // 
            this.btnB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnB.Image = global::SM64DSe.Properties.Resources.btnB1;
            this.btnB.Location = new System.Drawing.Point(196, 140);
            this.btnB.Name = "btnB";
            this.btnB.Size = new System.Drawing.Size(35, 30);
            this.btnB.TabIndex = 9;
            this.btnB.UseVisualStyleBackColor = true;
            this.btnB.Click += new System.EventHandler(this.btnB_Click);
            // 
            // btnX
            // 
            this.btnX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnX.Image = global::SM64DSe.Properties.Resources.btnX1;
            this.btnX.Location = new System.Drawing.Point(235, 140);
            this.btnX.Name = "btnX";
            this.btnX.Size = new System.Drawing.Size(35, 30);
            this.btnX.TabIndex = 10;
            this.btnX.UseVisualStyleBackColor = true;
            this.btnX.Click += new System.EventHandler(this.btnX_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnImport,
            this.btnExport,
            this.toolStripSeparator1,
            this.toolStripLabel1,
            this.lblVer,
            this.toolStripSeparator2,
            this.btnLanguages,
            this.btnHelp});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(566, 25);
            this.toolStrip1.TabIndex = 24;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnImport
            // 
            this.btnImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnImport.Enabled = false;
            this.btnImport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(47, 22);
            this.btnImport.Text = "Import";
            this.btnImport.ToolTipText = "Import XML";
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnExport
            // 
            this.btnExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnExport.Enabled = false;
            this.btnExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(44, 22);
            this.btnExport.Text = "Export";
            this.btnExport.ToolTipText = "Export to XML";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(77, 22);
            this.toolStripLabel1.Text = "Rom Version:";
            // 
            // lblVer
            // 
            this.lblVer.Name = "lblVer";
            this.lblVer.Size = new System.Drawing.Size(24, 22);
            this.lblVer.Text = "Ver";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnLanguages
            // 
            this.btnLanguages.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnLanguages.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLanguages.Name = "btnLanguages";
            this.btnLanguages.Size = new System.Drawing.Size(115, 22);
            this.btnLanguages.Text = "Select a Language";
            this.btnLanguages.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.btnLanguages_DropDownItemClicked);
            // 
            // btnHelp
            // 
            this.btnHelp.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(36, 22);
            this.btnHelp.Text = "Help";
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // TextEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(793, 515);
            this.Controls.Add(this.splitCVertical);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TextEditorForm";
            this.Text = "Text editor - SM64DS Editor";
            this.Load += new System.EventHandler(this.TextEditorForm_Load);
            this.splitCVertical.Panel1.ResumeLayout(false);
            this.splitCVertical.Panel1.PerformLayout();
            this.splitCVertical.Panel2.ResumeLayout(false);
            this.splitCVertical.Panel2.PerformLayout();
            //((System.ComponentModel.ISupportInitialize)(this.splitCVertical)).EndInit();
            this.splitCVertical.ResumeLayout(false);
            this.splitCHorizontal.Panel1.ResumeLayout(false);
            this.splitCHorizontal.Panel1.PerformLayout();
            this.splitCHorizontal.Panel2.ResumeLayout(false);
            this.splitCHorizontal.Panel2.PerformLayout();
            //((System.ComponentModel.ISupportInitialize)(this.splitCHorizontal)).EndInit();
            this.splitCHorizontal.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitCVertical;
        private System.Windows.Forms.ListBox lbxMsgList;
        private System.Windows.Forms.TextBox tbxMsgPreview;
        private System.Windows.Forms.TextBox txtEdit;
        private System.Windows.Forms.Button btnUpdateString;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnStarEmpty;
        private System.Windows.Forms.Button btnStarFull;
        private System.Windows.Forms.Button btnY;
        private System.Windows.Forms.Button btnX;
        private System.Windows.Forms.Button btnB;
        private System.Windows.Forms.Button btnA;
        private System.Windows.Forms.Button btnDPad;
        private System.Windows.Forms.Button btnR;
        private System.Windows.Forms.Button btnL;
        private System.Windows.Forms.Button btnCoins;
        private System.Windows.Forms.Button btnSaveAll;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnImport;
        private System.Windows.Forms.ToolStripButton btnExport;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripLabel lblVer;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripDropDownButton btnLanguages;
        private System.Windows.Forms.ToolStripButton btnHelp;
        private System.Windows.Forms.SplitContainer splitCHorizontal;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label2;
    }
}