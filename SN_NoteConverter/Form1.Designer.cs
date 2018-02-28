namespace SN_NoteConverter
{
    partial class Form1
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgv1 = new CC.XDatagrid();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.btnLoadOldData = new System.Windows.Forms.Button();
            this.btnStartImport = new System.Windows.Forms.Button();
            this.btnStopImport = new System.Windows.Forms.Button();
            this.lblProgress = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chLock = new System.Windows.Forms.CheckBox();
            this.txtNewPassword = new System.Windows.Forms.TextBox();
            this.txtNewUid = new System.Windows.Forms.TextBox();
            this.txtNewDbName = new System.Windows.Forms.TextBox();
            this.txtNewServerName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.txtOldPwd = new System.Windows.Forms.TextBox();
            this.txtOldUid = new System.Windows.Forms.TextBox();
            this.txtOldDB = new System.Windows.Forms.TextBox();
            this.txtOldSrv = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgv1
            // 
            this.dgv1.AllowSortByColumnHeaderClicked = false;
            this.dgv1.AllowUserToAddRows = false;
            this.dgv1.AllowUserToDeleteRows = false;
            this.dgv1.AllowUserToResizeColumns = false;
            this.dgv1.AllowUserToResizeRows = false;
            this.dgv1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(207)))), ((int)(((byte)(179)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 9.75F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgv1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgv1.EnableHeadersVisualStyles = false;
            this.dgv1.FillEmptyRow = false;
            this.dgv1.FocusedRowBorderRedLine = false;
            this.dgv1.Location = new System.Drawing.Point(12, 272);
            this.dgv1.MultiSelect = false;
            this.dgv1.Name = "dgv1";
            this.dgv1.ReadOnly = true;
            this.dgv1.RowHeadersVisible = false;
            this.dgv1.RowTemplate.Height = 26;
            this.dgv1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv1.Size = new System.Drawing.Size(708, 164);
            this.dgv1.StandardTab = true;
            this.dgv1.TabIndex = 0;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(27, 20);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(214, 24);
            this.comboBox1.TabIndex = 4;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // btnLoadOldData
            // 
            this.btnLoadOldData.Enabled = false;
            this.btnLoadOldData.Location = new System.Drawing.Point(247, 20);
            this.btnLoadOldData.Name = "btnLoadOldData";
            this.btnLoadOldData.Size = new System.Drawing.Size(117, 25);
            this.btnLoadOldData.TabIndex = 5;
            this.btnLoadOldData.Text = "Load Old Data";
            this.btnLoadOldData.UseVisualStyleBackColor = true;
            this.btnLoadOldData.Click += new System.EventHandler(this.btnLoadOldData_Click);
            // 
            // btnStartImport
            // 
            this.btnStartImport.Enabled = false;
            this.btnStartImport.Location = new System.Drawing.Point(370, 20);
            this.btnStartImport.Name = "btnStartImport";
            this.btnStartImport.Size = new System.Drawing.Size(90, 25);
            this.btnStartImport.TabIndex = 5;
            this.btnStartImport.Text = "Start Import";
            this.btnStartImport.UseVisualStyleBackColor = true;
            this.btnStartImport.Click += new System.EventHandler(this.btnStartImport_Click);
            // 
            // btnStopImport
            // 
            this.btnStopImport.Enabled = false;
            this.btnStopImport.Location = new System.Drawing.Point(466, 20);
            this.btnStopImport.Name = "btnStopImport";
            this.btnStopImport.Size = new System.Drawing.Size(90, 25);
            this.btnStopImport.TabIndex = 5;
            this.btnStopImport.Text = "Stop Import";
            this.btnStopImport.UseVisualStyleBackColor = true;
            this.btnStopImport.Click += new System.EventHandler(this.btnStopImport_Click);
            // 
            // lblProgress
            // 
            this.lblProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProgress.Location = new System.Drawing.Point(511, 246);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(207, 20);
            this.lblProgress.TabIndex = 6;
            this.lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chLock);
            this.groupBox1.Controls.Add(this.txtNewPassword);
            this.groupBox1.Controls.Add(this.txtNewUid);
            this.groupBox1.Controls.Add(this.txtNewDbName);
            this.groupBox1.Controls.Add(this.txtNewServerName);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 150);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(708, 91);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "New DB Connection";
            // 
            // chLock
            // 
            this.chLock.AutoSize = true;
            this.chLock.Location = new System.Drawing.Point(15, 26);
            this.chLock.Name = "chLock";
            this.chLock.Size = new System.Drawing.Size(52, 20);
            this.chLock.TabIndex = 2;
            this.chLock.Text = "Lock";
            this.chLock.UseVisualStyleBackColor = true;
            this.chLock.CheckedChanged += new System.EventHandler(this.chLock_CheckedChanged);
            // 
            // txtNewPassword
            // 
            this.txtNewPassword.Location = new System.Drawing.Point(483, 52);
            this.txtNewPassword.Name = "txtNewPassword";
            this.txtNewPassword.Size = new System.Drawing.Size(189, 23);
            this.txtNewPassword.TabIndex = 1;
            // 
            // txtNewUid
            // 
            this.txtNewUid.Location = new System.Drawing.Point(198, 52);
            this.txtNewUid.Name = "txtNewUid";
            this.txtNewUid.Size = new System.Drawing.Size(189, 23);
            this.txtNewUid.TabIndex = 1;
            this.txtNewUid.Text = "wee";
            // 
            // txtNewDbName
            // 
            this.txtNewDbName.Location = new System.Drawing.Point(483, 25);
            this.txtNewDbName.Name = "txtNewDbName";
            this.txtNewDbName.Size = new System.Drawing.Size(189, 23);
            this.txtNewDbName.TabIndex = 1;
            this.txtNewDbName.Text = "sn";
            // 
            // txtNewServerName
            // 
            this.txtNewServerName.Location = new System.Drawing.Point(198, 25);
            this.txtNewServerName.Name = "txtNewServerName";
            this.txtNewServerName.Size = new System.Drawing.Size(189, 23);
            this.txtNewServerName.TabIndex = 1;
            this.txtNewServerName.Text = "sn-server";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(410, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "Password";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(410, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = "DB Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(103, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "User ID";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(103, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server Name";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox1);
            this.groupBox2.Controls.Add(this.txtOldPwd);
            this.groupBox2.Controls.Add(this.txtOldUid);
            this.groupBox2.Controls.Add(this.txtOldDB);
            this.groupBox2.Controls.Add(this.txtOldSrv);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Location = new System.Drawing.Point(12, 56);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(708, 91);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Old DB Connection";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(15, 26);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(52, 20);
            this.checkBox1.TabIndex = 2;
            this.checkBox1.Text = "Lock";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // txtOldPwd
            // 
            this.txtOldPwd.Location = new System.Drawing.Point(483, 52);
            this.txtOldPwd.Name = "txtOldPwd";
            this.txtOldPwd.Size = new System.Drawing.Size(189, 23);
            this.txtOldPwd.TabIndex = 1;
            // 
            // txtOldUid
            // 
            this.txtOldUid.Location = new System.Drawing.Point(198, 52);
            this.txtOldUid.Name = "txtOldUid";
            this.txtOldUid.Size = new System.Drawing.Size(189, 23);
            this.txtOldUid.TabIndex = 1;
            this.txtOldUid.Text = "wee";
            // 
            // txtOldDB
            // 
            this.txtOldDB.Location = new System.Drawing.Point(483, 25);
            this.txtOldDB.Name = "txtOldDB";
            this.txtOldDB.Size = new System.Drawing.Size(189, 23);
            this.txtOldDB.TabIndex = 1;
            this.txtOldDB.Text = "sn_net";
            // 
            // txtOldSrv
            // 
            this.txtOldSrv.Location = new System.Drawing.Point(198, 25);
            this.txtOldSrv.Name = "txtOldSrv";
            this.txtOldSrv.Size = new System.Drawing.Size(189, 23);
            this.txtOldSrv.TabIndex = 1;
            this.txtOldSrv.Text = "sn-server";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(410, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 16);
            this.label5.TabIndex = 0;
            this.label5.Text = "Password";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(410, 28);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 16);
            this.label6.TabIndex = 0;
            this.label6.Text = "DB Name";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(103, 55);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 16);
            this.label7.TabIndex = 0;
            this.label7.Text = "User ID";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(103, 28);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(83, 16);
            this.label8.TabIndex = 0;
            this.label8.Text = "Server Name";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(732, 448);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dgv1);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.btnStopImport);
            this.Controls.Add(this.btnStartImport);
            this.Controls.Add(this.btnLoadOldData);
            this.Controls.Add(this.comboBox1);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private CC.XDatagrid dgv1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button btnLoadOldData;
        private System.Windows.Forms.Button btnStartImport;
        private System.Windows.Forms.Button btnStopImport;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtNewPassword;
        private System.Windows.Forms.TextBox txtNewUid;
        private System.Windows.Forms.TextBox txtNewServerName;
        private System.Windows.Forms.TextBox txtNewDbName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chLock;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox txtOldPwd;
        private System.Windows.Forms.TextBox txtOldUid;
        private System.Windows.Forms.TextBox txtOldDB;
        private System.Windows.Forms.TextBox txtOldSrv;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
    }
}

