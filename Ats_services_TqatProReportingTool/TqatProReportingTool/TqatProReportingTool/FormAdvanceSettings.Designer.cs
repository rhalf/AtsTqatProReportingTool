namespace TqatProReportingTool {
    partial class FormAdvanceSettings {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
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
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAdvanceSettings));
            this.tabControlAdvancedSettings = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.checkedListBoxColumnName = new System.Windows.Forms.CheckedListBox();
            this.labelReportType = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.checkBoxSelectAll = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.comboBoxReportType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonColumnApply = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.buttonSessionClear = new System.Windows.Forms.Button();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxHost = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.buttonValuesApply = new System.Windows.Forms.Button();
            this.textBoxValuesFuelToCost = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxValuesFuelToKilometers = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tabControlAdvancedSettings.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlAdvancedSettings
            // 
            this.tabControlAdvancedSettings.Controls.Add(this.tabPage1);
            this.tabControlAdvancedSettings.Controls.Add(this.tabPage2);
            this.tabControlAdvancedSettings.Controls.Add(this.tabPage3);
            this.tabControlAdvancedSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlAdvancedSettings.Location = new System.Drawing.Point(0, 0);
            this.tabControlAdvancedSettings.Name = "tabControlAdvancedSettings";
            this.tabControlAdvancedSettings.SelectedIndex = 0;
            this.tabControlAdvancedSettings.Size = new System.Drawing.Size(289, 281);
            this.tabControlAdvancedSettings.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.checkedListBoxColumnName);
            this.tabPage1.Controls.Add(this.labelReportType);
            this.tabPage1.Controls.Add(this.tableLayoutPanel2);
            this.tabPage1.Controls.Add(this.tableLayoutPanel1);
            this.tabPage1.Controls.Add(this.buttonColumnApply);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(281, 255);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Columns";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // checkedListBoxColumnName
            // 
            this.checkedListBoxColumnName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBoxColumnName.FormattingEnabled = true;
            this.checkedListBoxColumnName.Location = new System.Drawing.Point(3, 63);
            this.checkedListBoxColumnName.Name = "checkedListBoxColumnName";
            this.checkedListBoxColumnName.Size = new System.Drawing.Size(275, 145);
            this.checkedListBoxColumnName.TabIndex = 7;
            // 
            // labelReportType
            // 
            this.labelReportType.AutoSize = true;
            this.labelReportType.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.labelReportType.Location = new System.Drawing.Point(3, 208);
            this.labelReportType.Name = "labelReportType";
            this.labelReportType.Size = new System.Drawing.Size(56, 13);
            this.labelReportType.TabIndex = 6;
            this.labelReportType.Text = "Columns : ";
            this.labelReportType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.07018F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 43.85965F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.07018F));
            this.tableLayoutPanel2.Controls.Add(this.checkBoxSelectAll, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 33);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(275, 30);
            this.tableLayoutPanel2.TabIndex = 5;
            // 
            // checkBoxSelectAll
            // 
            this.checkBoxSelectAll.AutoSize = true;
            this.checkBoxSelectAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkBoxSelectAll.Location = new System.Drawing.Point(80, 3);
            this.checkBoxSelectAll.Name = "checkBoxSelectAll";
            this.checkBoxSelectAll.Size = new System.Drawing.Size(114, 24);
            this.checkBoxSelectAll.TabIndex = 0;
            this.checkBoxSelectAll.Text = "Select All";
            this.checkBoxSelectAll.UseVisualStyleBackColor = true;
            this.checkBoxSelectAll.CheckedChanged += new System.EventHandler(this.checkBoxSelectAll_CheckedChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34.23423F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65.76576F));
            this.tableLayoutPanel1.Controls.Add(this.comboBoxReportType, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(275, 30);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // comboBoxReportType
            // 
            this.comboBoxReportType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBoxReportType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxReportType.FormattingEnabled = true;
            this.comboBoxReportType.Location = new System.Drawing.Point(97, 3);
            this.comboBoxReportType.Name = "comboBoxReportType";
            this.comboBoxReportType.Size = new System.Drawing.Size(175, 21);
            this.comboBoxReportType.TabIndex = 0;
            this.comboBoxReportType.SelectedIndexChanged += new System.EventHandler(this.comboBoxReportType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 30);
            this.label1.TabIndex = 1;
            this.label1.Text = "ReportType :";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonColumnApply
            // 
            this.buttonColumnApply.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonColumnApply.Location = new System.Drawing.Point(3, 221);
            this.buttonColumnApply.Name = "buttonColumnApply";
            this.buttonColumnApply.Size = new System.Drawing.Size(275, 31);
            this.buttonColumnApply.TabIndex = 1;
            this.buttonColumnApply.Text = "Apply";
            this.buttonColumnApply.UseVisualStyleBackColor = true;
            this.buttonColumnApply.Click += new System.EventHandler(this.buttonColumnApply_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.buttonSessionClear);
            this.tabPage2.Controls.Add(this.textBoxPassword);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.textBoxUsername);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.textBoxHost);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(281, 255);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Session";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // buttonSessionClear
            // 
            this.buttonSessionClear.Location = new System.Drawing.Point(22, 169);
            this.buttonSessionClear.Name = "buttonSessionClear";
            this.buttonSessionClear.Size = new System.Drawing.Size(236, 46);
            this.buttonSessionClear.TabIndex = 2;
            this.buttonSessionClear.Text = "Clear Database Credential";
            this.buttonSessionClear.UseVisualStyleBackColor = true;
            this.buttonSessionClear.Click += new System.EventHandler(this.buttonSessionClear_Click);
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(83, 104);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.ReadOnly = true;
            this.textBoxPassword.Size = new System.Drawing.Size(175, 20);
            this.textBoxPassword.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 107);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Password :";
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.Location = new System.Drawing.Point(83, 78);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.ReadOnly = true;
            this.textBoxUsername.Size = new System.Drawing.Size(175, 20);
            this.textBoxUsername.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Username :";
            // 
            // textBoxHost
            // 
            this.textBoxHost.Location = new System.Drawing.Point(83, 52);
            this.textBoxHost.Name = "textBoxHost";
            this.textBoxHost.ReadOnly = true;
            this.textBoxHost.Size = new System.Drawing.Size(175, 20);
            this.textBoxHost.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Host";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.buttonValuesApply);
            this.tabPage3.Controls.Add(this.textBoxValuesFuelToCost);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Controls.Add(this.textBoxValuesFuelToKilometers);
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(281, 255);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Values";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // buttonValuesApply
            // 
            this.buttonValuesApply.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonValuesApply.Location = new System.Drawing.Point(3, 221);
            this.buttonValuesApply.Name = "buttonValuesApply";
            this.buttonValuesApply.Size = new System.Drawing.Size(275, 31);
            this.buttonValuesApply.TabIndex = 6;
            this.buttonValuesApply.Text = "Apply";
            this.buttonValuesApply.UseVisualStyleBackColor = true;
            this.buttonValuesApply.Click += new System.EventHandler(this.buttonValuesApply_Click);
            // 
            // textBoxValuesFuelToCost
            // 
            this.textBoxValuesFuelToCost.Location = new System.Drawing.Point(164, 54);
            this.textBoxValuesFuelToCost.Name = "textBoxValuesFuelToCost";
            this.textBoxValuesFuelToCost.Size = new System.Drawing.Size(97, 20);
            this.textBoxValuesFuelToCost.TabIndex = 4;
            this.textBoxValuesFuelToCost.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(22, 57);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(107, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Cost of 1 Liter of Fuel";
            // 
            // textBoxValuesFuelToKilometers
            // 
            this.textBoxValuesFuelToKilometers.Location = new System.Drawing.Point(164, 28);
            this.textBoxValuesFuelToKilometers.Name = "textBoxValuesFuelToKilometers";
            this.textBoxValuesFuelToKilometers.Size = new System.Drawing.Size(97, 20);
            this.textBoxValuesFuelToKilometers.TabIndex = 5;
            this.textBoxValuesFuelToKilometers.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(22, 31);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(136, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Kilometers in 1 Liter of Fuel ";
            // 
            // FormAdvanceSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(289, 281);
            this.Controls.Add(this.tabControlAdvancedSettings);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAdvanceSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Advance Settings";
            this.Load += new System.EventHandler(this.FormAdvanceSettings_Load);
            this.tabControlAdvancedSettings.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlAdvancedSettings;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.CheckedListBox checkedListBoxColumnName;
        private System.Windows.Forms.Label labelReportType;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.CheckBox checkBoxSelectAll;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox comboBoxReportType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonColumnApply;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button buttonSessionClear;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxHost;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button buttonValuesApply;
        private System.Windows.Forms.TextBox textBoxValuesFuelToCost;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxValuesFuelToKilometers;
        private System.Windows.Forms.Label label6;

    }
}