namespace TqatProReportingTool {
    partial class DialogLogin {
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogLogin));
            this.label1 = new System.Windows.Forms.Label();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.labelPassword = new System.Windows.Forms.Label();
            this.labelUsername = new System.Windows.Forms.Label();
            this.labelHost = new System.Windows.Forms.Label();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.textBoxCompany = new System.Windows.Forms.TextBox();
            this.buttonLogin = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.errorProviderDatabaseConfiguration = new System.Windows.Forms.ErrorProvider(this.components);
            this.pictureBoxLoading = new System.Windows.Forms.PictureBox();
            this.checkBoxRememberMe = new System.Windows.Forms.CheckBox();
            this.labelDatabaseHost = new System.Windows.Forms.Label();
            this.pictureBoxSetDatabase = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderDatabaseConfiguration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLoading)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSetDatabase)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(108, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(168, 13);
            this.label1.TabIndex = 26;
            this.label1.Text = "Please enter your Account details.";
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(0, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 220);
            this.splitter1.TabIndex = 24;
            this.splitter1.TabStop = false;
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.BackColor = System.Drawing.Color.Transparent;
            this.labelPassword.Location = new System.Drawing.Point(63, 111);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(53, 13);
            this.labelPassword.TabIndex = 20;
            this.labelPassword.Text = "Password";
            // 
            // labelUsername
            // 
            this.labelUsername.AutoSize = true;
            this.labelUsername.BackColor = System.Drawing.Color.Transparent;
            this.labelUsername.Location = new System.Drawing.Point(63, 86);
            this.labelUsername.Name = "labelUsername";
            this.labelUsername.Size = new System.Drawing.Size(55, 13);
            this.labelUsername.TabIndex = 21;
            this.labelUsername.Text = "Username";
            // 
            // labelHost
            // 
            this.labelHost.AutoSize = true;
            this.labelHost.BackColor = System.Drawing.Color.Transparent;
            this.labelHost.Location = new System.Drawing.Point(63, 61);
            this.labelHost.Name = "labelHost";
            this.labelHost.Size = new System.Drawing.Size(51, 13);
            this.labelHost.TabIndex = 22;
            this.labelHost.Text = "Company";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(122, 108);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(194, 20);
            this.textBoxPassword.TabIndex = 3;
            this.textBoxPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxPassword_KeyDown);
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.Location = new System.Drawing.Point(122, 83);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(194, 20);
            this.textBoxUsername.TabIndex = 2;
            // 
            // textBoxCompany
            // 
            this.textBoxCompany.Location = new System.Drawing.Point(122, 58);
            this.textBoxCompany.Name = "textBoxCompany";
            this.textBoxCompany.Size = new System.Drawing.Size(194, 20);
            this.textBoxCompany.TabIndex = 1;
            // 
            // buttonLogin
            // 
            this.buttonLogin.Location = new System.Drawing.Point(195, 156);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new System.Drawing.Size(121, 39);
            this.buttonLogin.TabIndex = 6;
            this.buttonLogin.Text = "Login";
            this.buttonLogin.UseVisualStyleBackColor = true;
            this.buttonLogin.Click += new System.EventHandler(this.buttonLogin_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(64, 156);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(121, 39);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // errorProviderDatabaseConfiguration
            // 
            this.errorProviderDatabaseConfiguration.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProviderDatabaseConfiguration.ContainerControl = this;
            // 
            // pictureBoxLoading
            // 
            this.pictureBoxLoading.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxLoading.BackgroundImage = global::TqatProReportingTool.Properties.Resources.image_background_001;
            this.pictureBoxLoading.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBoxLoading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxLoading.Image = global::TqatProReportingTool.Properties.Resources.gif_loading_001;
            this.pictureBoxLoading.Location = new System.Drawing.Point(3, 0);
            this.pictureBoxLoading.Name = "pictureBoxLoading";
            this.pictureBoxLoading.Size = new System.Drawing.Size(377, 220);
            this.pictureBoxLoading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxLoading.TabIndex = 27;
            this.pictureBoxLoading.TabStop = false;
            this.pictureBoxLoading.Visible = false;
            // 
            // checkBoxRememberMe
            // 
            this.checkBoxRememberMe.AutoSize = true;
            this.checkBoxRememberMe.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxRememberMe.Location = new System.Drawing.Point(153, 133);
            this.checkBoxRememberMe.Name = "checkBoxRememberMe";
            this.checkBoxRememberMe.Size = new System.Drawing.Size(95, 17);
            this.checkBoxRememberMe.TabIndex = 4;
            this.checkBoxRememberMe.Text = "Remember Me";
            this.checkBoxRememberMe.UseVisualStyleBackColor = false;
            // 
            // labelDatabaseHost
            // 
            this.labelDatabaseHost.BackColor = System.Drawing.Color.Transparent;
            this.labelDatabaseHost.Location = new System.Drawing.Point(200, 194);
            this.labelDatabaseHost.Name = "labelDatabaseHost";
            this.labelDatabaseHost.Size = new System.Drawing.Size(177, 23);
            this.labelDatabaseHost.TabIndex = 30;
            this.labelDatabaseHost.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pictureBoxSetDatabase
            // 
            this.pictureBoxSetDatabase.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxSetDatabase.BackgroundImage = global::TqatProReportingTool.Properties.Resources.icon_settingsadvanced_001;
            this.pictureBoxSetDatabase.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxSetDatabase.Location = new System.Drawing.Point(345, 12);
            this.pictureBoxSetDatabase.Name = "pictureBoxSetDatabase";
            this.pictureBoxSetDatabase.Size = new System.Drawing.Size(23, 24);
            this.pictureBoxSetDatabase.TabIndex = 32;
            this.pictureBoxSetDatabase.TabStop = false;
            this.pictureBoxSetDatabase.Click += new System.EventHandler(this.pictureBoxSetDatabase_Click);
            // 
            // DialogLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TqatProReportingTool.Properties.Resources.image_background_001;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(380, 220);
            this.Controls.Add(this.pictureBoxLoading);
            this.Controls.Add(this.pictureBoxSetDatabase);
            this.Controls.Add(this.labelDatabaseHost);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.labelPassword);
            this.Controls.Add(this.labelUsername);
            this.Controls.Add(this.labelHost);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.textBoxUsername);
            this.Controls.Add(this.textBoxCompany);
            this.Controls.Add(this.buttonLogin);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.checkBoxRememberMe);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DialogLogin";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.DialogLogin_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderDatabaseConfiguration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLoading)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSetDatabase)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.Label labelUsername;
        private System.Windows.Forms.Label labelHost;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.TextBox textBoxCompany;
        private System.Windows.Forms.Button buttonLogin;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ErrorProvider errorProviderDatabaseConfiguration;
        private System.Windows.Forms.PictureBox pictureBoxLoading;
        private System.Windows.Forms.CheckBox checkBoxRememberMe;
        private System.Windows.Forms.Label labelDatabaseHost;
        private System.Windows.Forms.PictureBox pictureBoxSetDatabase;
    }
}