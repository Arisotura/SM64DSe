namespace SM64DSe
{
    partial class ObjectListForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ObjectListForm));
            this.lbxObjectList = new System.Windows.Forms.ListBox();
            this.spcMainContainer = new System.Windows.Forms.SplitContainer();
            this.rtbObjectDesc = new System.Windows.Forms.RichTextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSelect = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbxObjSearch = new System.Windows.Forms.TextBox();
            //((System.ComponentModel.ISupportInitialize)(this.spcMainContainer)).BeginInit();
            this.spcMainContainer.Panel1.SuspendLayout();
            this.spcMainContainer.Panel2.SuspendLayout();
            this.spcMainContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbxObjectList
            // 
            this.lbxObjectList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbxObjectList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lbxObjectList.FormattingEnabled = true;
            this.lbxObjectList.IntegralHeight = false;
            this.lbxObjectList.Location = new System.Drawing.Point(0, 0);
            this.lbxObjectList.Margin = new System.Windows.Forms.Padding(0);
            this.lbxObjectList.Name = "lbxObjectList";
            this.lbxObjectList.Size = new System.Drawing.Size(344, 265);
            this.lbxObjectList.TabIndex = 1;
            this.lbxObjectList.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lbxObjectList_DrawItem);
            this.lbxObjectList.SelectedIndexChanged += new System.EventHandler(this.lbxObjectList_SelectedIndexChanged);
            this.lbxObjectList.DoubleClick += new System.EventHandler(this.lbxObjectList_DoubleClick);
            // 
            // spcMainContainer
            // 
            this.spcMainContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.spcMainContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.spcMainContainer.Location = new System.Drawing.Point(0, 22);
            this.spcMainContainer.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.spcMainContainer.Name = "spcMainContainer";
            this.spcMainContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // spcMainContainer.Panel1
            // 
            this.spcMainContainer.Panel1.Controls.Add(this.lbxObjectList);
            // 
            // spcMainContainer.Panel2
            // 
            this.spcMainContainer.Panel2.Controls.Add(this.rtbObjectDesc);
            this.spcMainContainer.Size = new System.Drawing.Size(344, 390);
            this.spcMainContainer.SplitterDistance = 265;
            this.spcMainContainer.TabIndex = 2;
            // 
            // rtbObjectDesc
            // 
            this.rtbObjectDesc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbObjectDesc.Location = new System.Drawing.Point(0, 0);
            this.rtbObjectDesc.Name = "rtbObjectDesc";
            this.rtbObjectDesc.ReadOnly = true;
            this.rtbObjectDesc.Size = new System.Drawing.Size(344, 121);
            this.rtbObjectDesc.TabIndex = 0;
            this.rtbObjectDesc.Text = "";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(269, 412);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSelect.Location = new System.Drawing.Point(188, 412);
            this.btnSelect.Margin = new System.Windows.Forms.Padding(0);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 4;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Search:  ";
            // 
            // tbxObjSearch
            // 
            this.tbxObjSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxObjSearch.Location = new System.Drawing.Point(46, 0);
            this.tbxObjSearch.Margin = new System.Windows.Forms.Padding(0);
            this.tbxObjSearch.Name = "tbxObjSearch";
            this.tbxObjSearch.ShortcutsEnabled = false;
            this.tbxObjSearch.Size = new System.Drawing.Size(298, 20);
            this.tbxObjSearch.TabIndex = 6;
            this.tbxObjSearch.TextChanged += new System.EventHandler(this.tbxObjSearch_TextChanged);
            // 
            // ObjectListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 435);
            this.Controls.Add(this.tbxObjSearch);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.spcMainContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ObjectListForm";
            this.Text = "Select object";
            this.Load += new System.EventHandler(this.ObjectListForm_Load);
            this.spcMainContainer.Panel1.ResumeLayout(false);
            this.spcMainContainer.Panel2.ResumeLayout(false);
            //((System.ComponentModel.ISupportInitialize)(this.spcMainContainer)).EndInit();
            this.spcMainContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbxObjectList;
        private System.Windows.Forms.SplitContainer spcMainContainer;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbxObjSearch;
        private System.Windows.Forms.RichTextBox rtbObjectDesc;

    }
}