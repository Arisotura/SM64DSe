namespace SM64DSe
{
    partial class RawEditorForm
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
            this.valueInput = new System.Windows.Forms.NumericUpDown();
            this.panControls = new System.Windows.Forms.Panel();
            this.btnToogleBinary = new System.Windows.Forms.Button();
            this.btnToogleHex = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.valueInput)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // valueInput
            // 
            this.valueInput.Location = new System.Drawing.Point(3, 4);
            this.valueInput.Name = "valueInput";
            this.valueInput.Size = new System.Drawing.Size(120, 20);
            this.valueInput.TabIndex = 2;
            this.valueInput.Tag = "";
            // 
            // panControls
            // 
            this.panControls.BackColor = System.Drawing.SystemColors.Control;
            this.panControls.Location = new System.Drawing.Point(12, 38);
            this.panControls.Name = "panControls";
            this.panControls.Size = new System.Drawing.Size(320, 165);
            this.panControls.TabIndex = 3;
            this.panControls.Tag = "";
            // 
            // btnToogleBinary
            // 
            this.btnToogleBinary.Location = new System.Drawing.Point(232, 9);
            this.btnToogleBinary.Name = "btnToogleBinary";
            this.btnToogleBinary.Size = new System.Drawing.Size(100, 23);
            this.btnToogleBinary.TabIndex = 4;
            this.btnToogleBinary.Text = "Display in Binary";
            this.btnToogleBinary.UseVisualStyleBackColor = true;
            this.btnToogleBinary.Click += new System.EventHandler(this.btnToogleBinary_Click);
            // 
            // btnToogleHex
            // 
            this.btnToogleHex.Location = new System.Drawing.Point(124, 2);
            this.btnToogleHex.Name = "btnToogleHex";
            this.btnToogleHex.Size = new System.Drawing.Size(37, 23);
            this.btnToogleHex.TabIndex = 5;
            this.btnToogleHex.Text = "Hex";
            this.btnToogleHex.UseVisualStyleBackColor = true;
            this.btnToogleHex.Click += new System.EventHandler(this.btnToogleHex_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnToogleHex);
            this.panel1.Controls.Add(this.valueInput);
            this.panel1.Location = new System.Drawing.Point(12, 9);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(166, 31);
            this.panel1.TabIndex = 6;
            // 
            // RawEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 204);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnToogleBinary);
            this.Controls.Add(this.panControls);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(368, 243);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(368, 243);
            this.Name = "RawEditorForm";
            this.Text = "Raw Parameter Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RawEditorForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.valueInput)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.NumericUpDown valueInput;
        private System.Windows.Forms.Panel panControls;
        private System.Windows.Forms.Button btnToogleBinary;
        private System.Windows.Forms.Button btnToogleHex;
        private System.Windows.Forms.Panel panel1;
    }
}