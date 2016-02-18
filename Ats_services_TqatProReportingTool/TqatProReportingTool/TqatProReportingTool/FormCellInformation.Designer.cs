namespace TqatProReportingTool {
    partial class FormCellInformation {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCellInformation));
            this.listViewInformation = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button1 = new System.Windows.Forms.Button();
            this.buttonViewOnMap = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listViewInformation
            // 
            this.listViewInformation.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listViewInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewInformation.FullRowSelect = true;
            this.listViewInformation.Location = new System.Drawing.Point(0, 0);
            this.listViewInformation.Name = "listViewInformation";
            this.listViewInformation.Size = new System.Drawing.Size(355, 354);
            this.listViewInformation.TabIndex = 0;
            this.listViewInformation.UseCompatibleStateImageBehavior = false;
            this.listViewInformation.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Attributes";
            this.columnHeader1.Width = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Values";
            this.columnHeader2.Width = 200;
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button1.Location = new System.Drawing.Point(0, 314);
            this.button1.Margin = new System.Windows.Forms.Padding(5);
            this.button1.MinimumSize = new System.Drawing.Size(0, 40);
            this.button1.Name = "button1";
            this.button1.Padding = new System.Windows.Forms.Padding(5);
            this.button1.Size = new System.Drawing.Size(355, 40);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            // 
            // buttonViewOnMap
            // 
            this.buttonViewOnMap.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonViewOnMap.Location = new System.Drawing.Point(0, 274);
            this.buttonViewOnMap.Margin = new System.Windows.Forms.Padding(5);
            this.buttonViewOnMap.MinimumSize = new System.Drawing.Size(0, 40);
            this.buttonViewOnMap.Name = "buttonViewOnMap";
            this.buttonViewOnMap.Padding = new System.Windows.Forms.Padding(5);
            this.buttonViewOnMap.Size = new System.Drawing.Size(355, 40);
            this.buttonViewOnMap.TabIndex = 2;
            this.buttonViewOnMap.Text = "View on Map";
            this.buttonViewOnMap.UseVisualStyleBackColor = true;
            this.buttonViewOnMap.Visible = false;
            this.buttonViewOnMap.Click += new System.EventHandler(this.buttonViewOnMap_Click);
            // 
            // FormCellInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(355, 354);
            this.Controls.Add(this.buttonViewOnMap);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listViewInformation);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormCellInformation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FormCellInformation_Load);
            this.Leave += new System.EventHandler(this.FormCellInformation_Leave);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewInformation;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button buttonViewOnMap;
    }
}