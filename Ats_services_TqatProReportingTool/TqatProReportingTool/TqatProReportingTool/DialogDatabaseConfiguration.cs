using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Ats.Helper;
using Ats.Database;

using TqatProReportingTool.Properties;
using MySql.Data.MySqlClient;


namespace TqatProReportingTool {
    public partial class DialogDatabaseConfiguration : Form {

        Database database;

        public DialogDatabaseConfiguration() {
            InitializeComponent();
        }

        private void buttonLogin_Click(object sender, EventArgs e) {

            /*
                Validate Inputs
            */
            //modalLoading = new ModalLoading();
            //this.Enabled = false;
            //modalLoading.Show();


            if (String.IsNullOrEmpty(textBoxHost.Text)) {
                errorProviderDatabaseConfiguration.SetError(textBoxHost, "Field is empty.");
            } else {
                if (StringValidator.isIpAddress(textBoxHost.Text)) {
                    errorProviderDatabaseConfiguration.SetError(textBoxHost, String.Empty);
                } else {
                    errorProviderDatabaseConfiguration.SetError(textBoxHost, "Invalid Host address!");
                }
            }

            if (String.IsNullOrEmpty(textBoxUsername.Text)) {
                errorProviderDatabaseConfiguration.SetError(textBoxUsername, "Field is empty.");
            } else {
                errorProviderDatabaseConfiguration.SetError(textBoxUsername, String.Empty);
            }


            if (String.IsNullOrEmpty(textBoxPassword.Text)) {
                errorProviderDatabaseConfiguration.SetError(textBoxPassword, "Field is empty.");
            } else {
                errorProviderDatabaseConfiguration.SetError(textBoxPassword, String.Empty);
            }

            if (String.IsNullOrEmpty(textBoxHost.Text) || String.IsNullOrEmpty(textBoxUsername.Text) || String.IsNullOrEmpty(textBoxPassword.Text) || !(StringValidator.isIpAddress(textBoxHost.Text))) {
                return;
            }


            /*
              Input Validated  
            */

            Settings.Default.DatabaseHost = textBoxHost.Text;
            Settings.Default.DatabaseUsername = textBoxUsername.Text;
            Settings.Default.DatabasePassword = textBoxPassword.Text;
            Settings.Default.Save();

            try {
                database = new Database(Settings.Default.DatabaseHost, Settings.Default.DatabaseUsername, Settings.Default.DatabasePassword);
                BackgroundWorker backgroundWorker = new BackgroundWorker();
                backgroundWorker.DoWork += backgroundWorker_DoWork;
                if (!backgroundWorker.IsBusy) {
                    backgroundWorker.RunWorkerAsync();
                }
            } catch (DatabaseException databaseException) {
                Log log = new Log(LogFileType.TXT, LogType.EXCEPTION);
                string logData = DateTime.Now.ToString() + "\t\t databaseException \t\t" + databaseException.Message;
                log.write(logData);
                MessageBox.Show(this, databaseException.Message, "Database Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        void backgroundWorker_DoWork(object sender, DoWorkEventArgs e) {

            try {
                pictureBoxLoading.Invoke(new MethodInvoker(delegate() {
                    pictureBoxLoading.Visible = true;
                }));
               
                database.checkConnection();
                Invoke(new MethodInvoker(delegate {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }));
            } catch (DatabaseException databaseException) {
                Log log = new Log(LogFileType.TXT, LogType.EXCEPTION);
                string logData = DateTime.Now.ToString() + "\t\t databaseException \t\t" + databaseException.Message;
                log.write(logData);

                Invoke(new MethodInvoker(delegate {
                    MessageBox.Show(this, databaseException.Message, "Database Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    pictureBoxLoading.Visible = false;
                }));
            } 

        }


        private void FormDatabaseConfiguration_Load(object sender, EventArgs e) {
            if (Settings.Default.DatabaseHost == "67.205.85.177") {
                labelDatabaseHost.Text = "Ats Server 1";
            } else if (Settings.Default.DatabaseHost == "184.107.175.154") {
                labelDatabaseHost.Text = "Ats Server 2";
            } else if (Settings.Default.DatabaseHost == "108.163.190.202") {
                labelDatabaseHost.Text = "Ats Database Server";
            } else {
                labelDatabaseHost.Text = Settings.Default.DatabaseHost;
            }

            this.textBoxPassword.PasswordChar = '\u25CF';
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            DialogResult dialogResult = MessageBox.Show(this, "Are you sure you want to exit this application?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == System.Windows.Forms.DialogResult.Yes) {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        private void textBoxPassword_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                buttonLogin.PerformClick();
            }
        }
    }
}
