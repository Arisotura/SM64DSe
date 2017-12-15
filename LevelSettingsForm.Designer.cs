namespace SM64DSe
{
    partial class LevelSettingsForm
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
            this.cbxBank0 = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.nudGeneralMinimumNumberOfAreas = new System.Windows.Forms.NumericUpDown();
            this.lblGeneralMinimumNumberOfAreas = new System.Windows.Forms.Label();
            this.txtActSelectorID = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.cbxBackground = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cbxBank7 = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cbxBank6 = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cbxBank5 = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cbxBank4 = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbxBank3 = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbxBank2 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbxBank1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtMusicByte03 = new System.Windows.Forms.TextBox();
            this.txtMusicByte02 = new System.Windows.Forms.TextBox();
            this.txtMusicByte01 = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudGeneralMinimumNumberOfAreas)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbxBank0
            // 
            this.cbxBank0.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cbxBank0.DropDownHeight = 200;
            this.cbxBank0.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxBank0.FormattingEnabled = true;
            this.cbxBank0.IntegralHeight = false;
            this.cbxBank0.Location = new System.Drawing.Point(65, 23);
            this.cbxBank0.Name = "cbxBank0";
            this.cbxBank0.Size = new System.Drawing.Size(301, 21);
            this.cbxBank0.TabIndex = 0;
            this.cbxBank0.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cbxBankX_DrawItem);
            this.cbxBank0.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.cbxBankX_MeasureItem);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.nudGeneralMinimumNumberOfAreas);
            this.groupBox1.Controls.Add(this.lblGeneralMinimumNumberOfAreas);
            this.groupBox1.Controls.Add(this.txtActSelectorID);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.cbxBackground);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(381, 105);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General settings";
            // 
            // nudGeneralMinimumNumberOfAreas
            // 
            this.nudGeneralMinimumNumberOfAreas.Location = new System.Drawing.Point(225, 77);
            this.nudGeneralMinimumNumberOfAreas.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.nudGeneralMinimumNumberOfAreas.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudGeneralMinimumNumberOfAreas.Name = "nudGeneralMinimumNumberOfAreas";
            this.nudGeneralMinimumNumberOfAreas.Size = new System.Drawing.Size(141, 20);
            this.nudGeneralMinimumNumberOfAreas.TabIndex = 5;
            this.nudGeneralMinimumNumberOfAreas.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblGeneralMinimumNumberOfAreas
            // 
            this.lblGeneralMinimumNumberOfAreas.AutoSize = true;
            this.lblGeneralMinimumNumberOfAreas.Location = new System.Drawing.Point(15, 79);
            this.lblGeneralMinimumNumberOfAreas.Name = "lblGeneralMinimumNumberOfAreas";
            this.lblGeneralMinimumNumberOfAreas.Size = new System.Drawing.Size(110, 13);
            this.lblGeneralMinimumNumberOfAreas.TabIndex = 4;
            this.lblGeneralMinimumNumberOfAreas.Text = "Minimum No. of Areas";
            // 
            // txtActSelectorID
            // 
            this.txtActSelectorID.Location = new System.Drawing.Point(225, 51);
            this.txtActSelectorID.Name = "txtActSelectorID";
            this.txtActSelectorID.Size = new System.Drawing.Size(141, 20);
            this.txtActSelectorID.TabIndex = 3;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(15, 54);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(82, 13);
            this.label10.TabIndex = 2;
            this.label10.Text = "Act Selector ID:";
            // 
            // cbxBackground
            // 
            this.cbxBackground.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxBackground.FormattingEnabled = true;
            this.cbxBackground.Items.AddRange(new object[] {
            "[0] None",
            "[1] Sea",
            "[2] Sky",
            "[3] Cloudy sky",
            "[4] Snowy mountains",
            "[5] Haunted forest",
            "[6] Desert",
            "[7] Flaming sky",
            "[8] Underwater city",
            "[9] Foggy forest",
            "[10] Bowser in the Dark World",
            "[11] Bowser in the Sky"});
            this.cbxBackground.Location = new System.Drawing.Point(225, 24);
            this.cbxBackground.Name = "cbxBackground";
            this.cbxBackground.Size = new System.Drawing.Size(141, 21);
            this.cbxBackground.TabIndex = 1;
            this.cbxBackground.SelectedIndexChanged += new System.EventHandler(this.cbxBackground_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Background:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.cbxBank7);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.cbxBank6);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.cbxBank5);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.cbxBank4);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.cbxBank3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.cbxBank2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.cbxBank1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.cbxBank0);
            this.groupBox2.Location = new System.Drawing.Point(12, 123);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(381, 230);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Object banks";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(15, 191);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(44, 13);
            this.label9.TabIndex = 14;
            this.label9.Text = "Bank 7:";
            // 
            // cbxBank7
            // 
            this.cbxBank7.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cbxBank7.DropDownHeight = 200;
            this.cbxBank7.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxBank7.FormattingEnabled = true;
            this.cbxBank7.IntegralHeight = false;
            this.cbxBank7.Location = new System.Drawing.Point(65, 188);
            this.cbxBank7.Name = "cbxBank7";
            this.cbxBank7.Size = new System.Drawing.Size(301, 21);
            this.cbxBank7.TabIndex = 13;
            this.cbxBank7.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cbxBankX_DrawItem);
            this.cbxBank7.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.cbxBankX_MeasureItem);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 168);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(44, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Bank 6:";
            // 
            // cbxBank6
            // 
            this.cbxBank6.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cbxBank6.DropDownHeight = 200;
            this.cbxBank6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxBank6.FormattingEnabled = true;
            this.cbxBank6.IntegralHeight = false;
            this.cbxBank6.Location = new System.Drawing.Point(65, 165);
            this.cbxBank6.Name = "cbxBank6";
            this.cbxBank6.Size = new System.Drawing.Size(301, 21);
            this.cbxBank6.TabIndex = 11;
            this.cbxBank6.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cbxBankX_DrawItem);
            this.cbxBank6.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.cbxBankX_MeasureItem);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 144);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Bank 5:";
            // 
            // cbxBank5
            // 
            this.cbxBank5.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cbxBank5.DropDownHeight = 200;
            this.cbxBank5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxBank5.FormattingEnabled = true;
            this.cbxBank5.IntegralHeight = false;
            this.cbxBank5.Location = new System.Drawing.Point(65, 141);
            this.cbxBank5.Name = "cbxBank5";
            this.cbxBank5.Size = new System.Drawing.Size(301, 21);
            this.cbxBank5.TabIndex = 9;
            this.cbxBank5.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cbxBankX_DrawItem);
            this.cbxBank5.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.cbxBankX_MeasureItem);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 121);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Bank 4:";
            // 
            // cbxBank4
            // 
            this.cbxBank4.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cbxBank4.DropDownHeight = 200;
            this.cbxBank4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxBank4.FormattingEnabled = true;
            this.cbxBank4.IntegralHeight = false;
            this.cbxBank4.Location = new System.Drawing.Point(65, 118);
            this.cbxBank4.Name = "cbxBank4";
            this.cbxBank4.Size = new System.Drawing.Size(301, 21);
            this.cbxBank4.TabIndex = 7;
            this.cbxBank4.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cbxBankX_DrawItem);
            this.cbxBank4.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.cbxBankX_MeasureItem);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 97);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Bank 3:";
            // 
            // cbxBank3
            // 
            this.cbxBank3.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cbxBank3.DropDownHeight = 200;
            this.cbxBank3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxBank3.FormattingEnabled = true;
            this.cbxBank3.IntegralHeight = false;
            this.cbxBank3.Location = new System.Drawing.Point(65, 94);
            this.cbxBank3.Name = "cbxBank3";
            this.cbxBank3.Size = new System.Drawing.Size(301, 21);
            this.cbxBank3.TabIndex = 5;
            this.cbxBank3.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cbxBankX_DrawItem);
            this.cbxBank3.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.cbxBankX_MeasureItem);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Bank 2:";
            // 
            // cbxBank2
            // 
            this.cbxBank2.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cbxBank2.DropDownHeight = 200;
            this.cbxBank2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxBank2.FormattingEnabled = true;
            this.cbxBank2.IntegralHeight = false;
            this.cbxBank2.Location = new System.Drawing.Point(65, 70);
            this.cbxBank2.Name = "cbxBank2";
            this.cbxBank2.Size = new System.Drawing.Size(301, 21);
            this.cbxBank2.TabIndex = 3;
            this.cbxBank2.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cbxBankX_DrawItem);
            this.cbxBank2.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.cbxBankX_MeasureItem);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Bank 1:";
            // 
            // cbxBank1
            // 
            this.cbxBank1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cbxBank1.DropDownHeight = 200;
            this.cbxBank1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxBank1.FormattingEnabled = true;
            this.cbxBank1.IntegralHeight = false;
            this.cbxBank1.Location = new System.Drawing.Point(65, 46);
            this.cbxBank1.Name = "cbxBank1";
            this.cbxBank1.Size = new System.Drawing.Size(301, 21);
            this.cbxBank1.TabIndex = 1;
            this.cbxBank1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cbxBankX_DrawItem);
            this.cbxBank1.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.cbxBankX_MeasureItem);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Bank 0:";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(318, 465);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 26);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(237, 465);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 26);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtMusicByte03);
            this.groupBox3.Controls.Add(this.txtMusicByte02);
            this.groupBox3.Controls.Add(this.txtMusicByte01);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Location = new System.Drawing.Point(12, 360);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(381, 99);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Level Music";
            // 
            // txtMusicByte03
            // 
            this.txtMusicByte03.Location = new System.Drawing.Point(266, 66);
            this.txtMusicByte03.Name = "txtMusicByte03";
            this.txtMusicByte03.Size = new System.Drawing.Size(100, 20);
            this.txtMusicByte03.TabIndex = 6;
            // 
            // txtMusicByte02
            // 
            this.txtMusicByte02.Location = new System.Drawing.Point(266, 43);
            this.txtMusicByte02.Name = "txtMusicByte02";
            this.txtMusicByte02.Size = new System.Drawing.Size(100, 20);
            this.txtMusicByte02.TabIndex = 5;
            // 
            // txtMusicByte01
            // 
            this.txtMusicByte01.Location = new System.Drawing.Point(266, 19);
            this.txtMusicByte01.Name = "txtMusicByte01";
            this.txtMusicByte01.Size = new System.Drawing.Size(100, 20);
            this.txtMusicByte01.TabIndex = 4;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(15, 69);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(92, 13);
            this.label13.TabIndex = 3;
            this.label13.Text = "Byte 03: SSEQ ID";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(15, 46);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(92, 13);
            this.label12.TabIndex = 2;
            this.label12.Text = "Byte 02: SBNK ID";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(15, 22);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(92, 13);
            this.label11.TabIndex = 1;
            this.label11.Text = "Byte 01: Group ID";
            // 
            // LevelSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(405, 500);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LevelSettingsForm";
            this.Text = "Level settings";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LevelSettingsForm_FormClosed);
            this.Load += new System.EventHandler(this.LevelSettingsForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudGeneralMinimumNumberOfAreas)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbxBank0;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbxBackground;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cbxBank7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cbxBank6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cbxBank5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbxBank4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbxBank3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbxBank2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbxBank1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtMusicByte03;
        private System.Windows.Forms.TextBox txtMusicByte02;
        private System.Windows.Forms.TextBox txtMusicByte01;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtActSelectorID;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown nudGeneralMinimumNumberOfAreas;
        private System.Windows.Forms.Label lblGeneralMinimumNumberOfAreas;
    }
}