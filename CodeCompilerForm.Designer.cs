namespace SM64DSe
{
    partial class CodeCompilerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CodeCompilerForm));
            this.lblFolder = new System.Windows.Forms.Label();
            this.btnFolder = new System.Windows.Forms.Button();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.lblOutput = new System.Windows.Forms.Label();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.btnCompile = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.lblOffset = new System.Windows.Forms.Label();
            this.txtOffset = new System.Windows.Forms.TextBox();
            this.chkDynamic = new System.Windows.Forms.CheckBox();
            this.btnClean = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblFolder
            // 
            this.lblFolder.AutoSize = true;
            this.lblFolder.Location = new System.Drawing.Point(12, 9);
            this.lblFolder.Name = "lblFolder";
            this.lblFolder.Size = new System.Drawing.Size(36, 13);
            this.lblFolder.TabIndex = 0;
            this.lblFolder.Text = "Folder";
            // 
            // btnFolder
            // 
            this.btnFolder.Location = new System.Drawing.Point(165, 97);
            this.btnFolder.Name = "btnFolder";
            this.btnFolder.Size = new System.Drawing.Size(85, 23);
            this.btnFolder.TabIndex = 1;
            this.btnFolder.Text = "Select Folder";
            this.btnFolder.UseVisualStyleBackColor = true;
            this.btnFolder.Click += new System.EventHandler(this.btnFolder_Click);
            // 
            // txtFolder
            // 
            this.txtFolder.Location = new System.Drawing.Point(54, 6);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.Size = new System.Drawing.Size(361, 20);
            this.txtFolder.TabIndex = 2;
            // 
            // lblOutput
            // 
            this.lblOutput.AutoSize = true;
            this.lblOutput.Location = new System.Drawing.Point(12, 39);
            this.lblOutput.Name = "lblOutput";
            this.lblOutput.Size = new System.Drawing.Size(58, 13);
            this.lblOutput.TabIndex = 3;
            this.lblOutput.Text = "Output File";
            // 
            // txtOutput
            // 
            this.txtOutput.Location = new System.Drawing.Point(76, 36);
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.Size = new System.Drawing.Size(201, 20);
            this.txtOutput.TabIndex = 4;
            // 
            // btnCompile
            // 
            this.btnCompile.Location = new System.Drawing.Point(340, 97);
            this.btnCompile.Name = "btnCompile";
            this.btnCompile.Size = new System.Drawing.Size(75, 23);
            this.btnCompile.TabIndex = 5;
            this.btnCompile.Text = "Compile!";
            this.btnCompile.UseVisualStyleBackColor = true;
            this.btnCompile.Click += new System.EventHandler(this.btnCompile_Click);
            // 
            // lblOffset
            // 
            this.lblOffset.AutoSize = true;
            this.lblOffset.Location = new System.Drawing.Point(283, 39);
            this.lblOffset.Name = "lblOffset";
            this.lblOffset.Size = new System.Drawing.Size(35, 13);
            this.lblOffset.TabIndex = 6;
            this.lblOffset.Text = "Offset";
            // 
            // txtOffset
            // 
            this.txtOffset.Location = new System.Drawing.Point(324, 36);
            this.txtOffset.Name = "txtOffset";
            this.txtOffset.Size = new System.Drawing.Size(91, 20);
            this.txtOffset.TabIndex = 7;
            // 
            // chkDynamic
            // 
            this.chkDynamic.AutoSize = true;
            this.chkDynamic.Location = new System.Drawing.Point(286, 62);
            this.chkDynamic.Name = "chkDynamic";
            this.chkDynamic.Size = new System.Drawing.Size(97, 17);
            this.chkDynamic.TabIndex = 8;
            this.chkDynamic.Text = "Dynamic library";
            this.chkDynamic.UseVisualStyleBackColor = true;
            this.chkDynamic.CheckedChanged += new System.EventHandler(this.chkDynamic_CheckedChanged);
            // 
            // btnClean
            // 
            this.btnClean.Location = new System.Drawing.Point(12, 97);
            this.btnClean.Name = "btnClean";
            this.btnClean.Size = new System.Drawing.Size(75, 23);
            this.btnClean.TabIndex = 9;
            this.btnClean.Text = "Clean Patch";
            this.btnClean.UseVisualStyleBackColor = true;
            this.btnClean.Click += new System.EventHandler(this.btnClean_Click);
            // 
            // CodeCompilerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(427, 132);
            this.Controls.Add(this.btnClean);
            this.Controls.Add(this.chkDynamic);
            this.Controls.Add(this.txtOffset);
            this.Controls.Add(this.lblOffset);
            this.Controls.Add(this.btnCompile);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.lblOutput);
            this.Controls.Add(this.txtFolder);
            this.Controls.Add(this.btnFolder);
            this.Controls.Add(this.lblFolder);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CodeCompilerForm";
            this.Text = "Code Compiler";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFolder;
        private System.Windows.Forms.Button btnFolder;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Button btnCompile;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Label lblOffset;
        private System.Windows.Forms.TextBox txtOffset;
        private System.Windows.Forms.CheckBox chkDynamic;
        private System.Windows.Forms.Button btnClean;
    }
}