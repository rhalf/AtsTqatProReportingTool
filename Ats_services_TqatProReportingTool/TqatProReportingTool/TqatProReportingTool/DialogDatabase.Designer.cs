namespace TqatProReportingTool {
    partial class DialogDatabase {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent () {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogDatabase));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxConfiguration = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.radioButtonServerX = new System.Windows.Forms.RadioButton();
            this.buttonDone = new System.Windows.Forms.Button();
            this.radioButtonServer2 = new System.Windows.Forms.RadioButton();
            this.radioButtonServer1 = new System.Windows.Forms.RadioButton();
            this.groupBoxOther = new System.Windows.Forms.GroupBox();
            this.buttonTest = new System.Windows.Forms.Button();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxIp = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBoxConfiguration.SuspendLayout();
            this.groupBoxOther.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.BackgroundImage = global::TqatProReportingTool.Properties.Resources.image_background_001;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.groupBoxConfiguration, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxOther, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(385, 224);
            this.tableLayoutPanel1.TabIndex = 11;
            // 
            // groupBoxConfiguration
            // 
            this.groupBoxConfiguration.BackColor = System.Drawing.Color.Transparent;
            this.groupBoxConfiguration.Controls.Add(this.label6);
            this.groupBoxConfiguration.Controls.Add(this.label3);
            this.groupBoxConfiguration.Controls.Add(this.radioButtonServerX);
            this.groupBoxConfiguration.Controls.Add(this.buttonDone);
            this.groupBoxConfiguration.Controls.Add(this.radioButtonServer2);
            this.groupBoxConfiguration.Controls.Add(this.radioButtonServer1);
            this.groupBoxConfiguration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxConfiguration.Location = new System.Drawing.Point(3, 3);
            this.groupBoxConfiguration.Name = "groupBoxConfiguration";
            this.groupBoxConfiguration.Size = new System.Drawing.Size(186, 218);
            this.groupBoxConfiguration.TabIndex = 1;
            this.groupBoxConfiguration.TabStop = false;
            this.groupBoxConfiguration.Text = "Configuration";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(38, 90);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "184.107.175.154";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(38, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "108.163.190.202";
            // 
            // radioButtonServerX
            // 
            this.radioButtonServerX.AutoSize = true;
            this.radioButtonServerX.Location = new System.Drawing.Point(6, 119);
            this.radioButtonServerX.Name = "radioButtonServerX";
            this.radioButtonServerX.Size = new System.Drawing.Size(51, 17);
            this.radioButtonServerX.TabIndex = 2;
            this.radioButtonServerX.Text = "Other";
            this.radioButtonServerX.UseVisualStyleBackColor = true;
            this.radioButtonServerX.CheckedChanged += new System.EventHandler(this.radioButtonServerX_CheckedChanged);
            // 
            // buttonDone
            // 
            this.buttonDone.Location = new System.Drawing.Point(23, 175);
            this.buttonDone.Name = "buttonDone";
            this.buttonDone.Size = new System.Drawing.Size(127, 23);
            this.buttonDone.TabIndex = 8;
            this.buttonDone.Text = "Done";
            this.buttonDone.UseVisualStyleBackColor = true;
            this.buttonDone.Click += new System.EventHandler(this.buttonDone_Click);
            // 
            // radioButtonServer2
            // 
            this.radioButtonServer2.AutoSize = true;
            this.radioButtonServer2.Location = new System.Drawing.Point(6, 70);
            this.radioButtonServer2.Name = "radioButtonServer2";
            this.radioButtonServer2.Size = new System.Drawing.Size(132, 17);
            this.radioButtonServer2.TabIndex = 1;
            this.radioButtonServer2.Text = "Ats Database Server 2";
            this.radioButtonServer2.UseVisualStyleBackColor = true;
            this.radioButtonServer2.CheckedChanged += new System.EventHandler(this.radioButtonServerX_CheckedChanged);
            // 
            // radioButtonServer1
            // 
            this.radioButtonServer1.AutoSize = true;
            this.radioButtonServer1.Checked = true;
            this.radioButtonServer1.Location = new System.Drawing.Point(6, 19);
            this.radioButtonServer1.Name = "radioButtonServer1";
            this.radioButtonServer1.Size = new System.Drawing.Size(132, 17);
            this.radioButtonServer1.TabIndex = 0;
            this.radioButtonServer1.TabStop = true;
            this.radioButtonServer1.Text = "Ats Database Server 1";
            this.radioButtonServer1.UseVisualStyleBackColor = true;
            this.radioButtonServer1.CheckedChanged += new System.EventHandler(this.radioButtonServerX_CheckedChanged);
            // 
            // groupBoxOther
            // 
            this.groupBoxOther.BackColor = System.Drawing.Color.Transparent;
            this.groupBoxOther.Controls.Add(this.buttonTest);
            this.groupBoxOther.Controls.Add(this.textBoxPort);
            this.groupBoxOther.Controls.Add(this.label4);
            this.groupBoxOther.Controls.Add(this.textBoxPassword);
            this.groupBoxOther.Controls.Add(this.label5);
            this.groupBoxOther.Controls.Add(this.textBoxUsername);
            this.groupBoxOther.Controls.Add(this.label2);
            this.groupBoxOther.Controls.Add(this.textBoxIp);
            this.groupBoxOther.Controls.Add(this.label1);
            this.groupBoxOther.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxOther.Enabled = false;
            this.groupBoxOther.Location = new System.Drawing.Point(195, 3);
            this.groupBoxOther.Name = "groupBoxOther";
            this.groupBoxOther.Size = new System.Drawing.Size(187, 218);
            this.groupBoxOther.TabIndex = 2;
            this.groupBoxOther.TabStop = false;
            this.groupBoxOther.Text = "Other";
            // 
            // buttonTest
            // 
            this.buttonTest.Location = new System.Drawing.Point(22, 175);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(127, 23);
            this.buttonTest.TabIndex = 7;
            this.buttonTest.Text = "Test";
            this.buttonTest.UseVisualStyleBackColor = true;
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(69, 49);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(100, 20);
            this.textBoxPort.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(37, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Port";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(69, 100);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(100, 20);
            this.textBoxPassword.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 103);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Password";
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.Location = new System.Drawing.Point(69, 74);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(100, 20);
            this.textBoxUsername.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Username";
            // 
            // textBoxIp
            // 
            this.textBoxIp.Location = new System.Drawing.Point(69, 23);
            this.textBoxIp.Name = "textBoxIp";
            this.textBoxIp.Size = new System.Drawing.Size(100, 20);
            this.textBoxIp.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(47, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(16, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ip";
            // 
            // DialogDatabase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(385, 224);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogDatabase";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DialogDatabase";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DialogDatabase_FormClosed);
            this.Load += new System.EventHandler(this.DialogDatabase_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBoxConfiguration.ResumeLayout(false);
            this.groupBoxConfiguration.PerformLayout();
            this.groupBoxOther.ResumeLayout(false);
            this.groupBoxOther.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxConfiguration;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radioButtonServerX;
        private System.Windows.Forms.Button buttonDone;
        private System.Windows.Forms.RadioButton radioButtonServer2;
        private System.Windows.Forms.RadioButton radioButtonServer1;
        private System.Windows.Forms.Button buttonTest;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.GroupBox groupBoxOther;
        private System.Windows.Forms.TextBox textBoxIp;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}