namespace SM64DSe
{
    partial class ExceptionMessageBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExceptionMessageBox));
            this.lbxMessage = new System.Windows.Forms.Label();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.lblStackTrace = new System.Windows.Forms.Label();
            this.txtStackTrace = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbxMessage
            // 
            this.lbxMessage.AutoSize = true;
            this.lbxMessage.Location = new System.Drawing.Point(13, 13);
            this.lbxMessage.Name = "lbxMessage";
            this.lbxMessage.Size = new System.Drawing.Size(53, 13);
            this.lbxMessage.TabIndex = 0;
            this.lbxMessage.Text = "Message:";
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(16, 30);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ReadOnly = true;
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessage.Size = new System.Drawing.Size(507, 64);
            this.txtMessage.TabIndex = 1;
            // 
            // lblStackTrace
            // 
            this.lblStackTrace.AutoSize = true;
            this.lblStackTrace.Location = new System.Drawing.Point(16, 101);
            this.lblStackTrace.Name = "lblStackTrace";
            this.lblStackTrace.Size = new System.Drawing.Size(65, 13);
            this.lblStackTrace.TabIndex = 2;
            this.lblStackTrace.Text = "Stack trace:";
            // 
            // txtStackTrace
            // 
            this.txtStackTrace.Location = new System.Drawing.Point(16, 118);
            this.txtStackTrace.Multiline = true;
            this.txtStackTrace.Name = "txtStackTrace";
            this.txtStackTrace.ReadOnly = true;
            this.txtStackTrace.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStackTrace.Size = new System.Drawing.Size(507, 225);
            this.txtStackTrace.TabIndex = 3;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(448, 349);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // ExceptionMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(535, 384);
            this.ControlBox = false;
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.txtStackTrace);
            this.Controls.Add(this.lblStackTrace);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.lbxMessage);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExceptionMessageBox";
            this.Text = "Error!";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbxMessage;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Label lblStackTrace;
        private System.Windows.Forms.TextBox txtStackTrace;
        private System.Windows.Forms.Button btnClose;
    }
}