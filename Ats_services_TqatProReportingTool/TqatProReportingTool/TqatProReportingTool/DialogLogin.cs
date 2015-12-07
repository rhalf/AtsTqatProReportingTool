using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading;
using System.Collections;

using MySql.Data.MySqlClient;
using TqatProReportingTool.Properties;

using Ats.Database;
using Ats.Session;
using Ats.Helper;

namespace TqatProReportingTool {
    public partial class DialogLogin : Form {

        Account account;
        Database database;

        //Hashtable hashTable = new Hashtable();

        public DialogLogin (ref Account account) {
            this.account = account;
            InitializeComponent();
        }

        private void buttonCancel_Click (object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonLogin_Click (object sender, EventArgs e) {
            /*
             *  Validate Inputs 
             * 
             */
            if (String.IsNullOrEmpty(textBoxCompany.Text)) {
                errorProviderDatabaseConfiguration.SetError(textBoxCompany, "Field is empty.");
            } else {
                errorProviderDatabaseConfiguration.SetError(textBoxCompany, String.Empty);
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

            if (String.IsNullOrEmpty(textBoxCompany.Text) || String.IsNullOrEmpty(textBoxUsername.Text) || String.IsNullOrEmpty(textBoxPassword.Text) || (String.IsNullOrEmpty(textBoxCompany.Text))) {
                return;
            }

            account.companyUsername = textBoxCompany.Text;
            account.username = textBoxUsername.Text;
            account.password = Cryptography.md5(textBoxPassword.Text);
            account.rememberMe = checkBoxRememberMe.Checked;

            try {
                database = new Database(Settings.Default.DatabaseHost, Settings.Default.DatabaseUsername, Settings.Default.DatabasePassword);
                BackgroundWorker backgroundWorker = new BackgroundWorker();
                backgroundWorker.DoWork += backgroundWorker_DoWork;
                if (!backgroundWorker.IsBusy) {
                    backgroundWorker.RunWorkerAsync();
                }
            } catch (DatabaseException databaseException) {
                MessageBox.Show(this, databaseException.Message, "Database Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log log = new Log(LogFileType.TXT, LogType.EXCEPTION);
                string logData = DateTime.Now.ToString() + "\t\t databaseException \t\t" + databaseException.Message;
                log.write(logData);
            } finally {
            }
        }

        void backgroundWorker_DoWork (object sender, DoWorkEventArgs e) {
            database = new Database(Settings.Default.DatabaseHost, Settings.Default.DatabaseUsername, Settings.Default.DatabasePassword);


            pictureBoxLoading.Invoke(new MethodInvoker(delegate {
                pictureBoxLoading.Visible = true;
            }));

            try {
                Query query = new Query(database);
                query.checkLogin(account);

                if (account.active == 0) {
                    throw new QueryException(1, "Can't Login! This account is deactivated.");
                }


                int result = account.dateTimeCreated.CompareTo(DateTime.Now);
                if (result != -1) {
                    throw new QueryException(1, "Can't Login! This account is expired.");
                }


                this.Invoke(new MethodInvoker(delegate {
                    Settings.Default.accountRememberMe = account.rememberMe;
                    Settings.Default.accountCompanyUsername = account.companyUsername;
                    Settings.Default.accountUsername = account.username;
                    Settings.Default.accountPassword = account.password;
                    Settings.Default.Save();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }));
            } catch (QueryException queryException) {
                this.Invoke(new MethodInvoker(delegate {
                    MessageBox.Show(this, queryException.Message, "Query Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    Log log = new Log(LogFileType.TXT, LogType.EXCEPTION);
                    string logData = DateTime.Now.ToString() + "\t\t queryException \t\t" + queryException.Message;
                    log.write(logData);
                }));
            } finally {
                try {
                    pictureBoxLoading.Invoke(new MethodInvoker(delegate {
                        pictureBoxLoading.Visible = false;
                    }));
                } catch {

                }
            }
        }

        private void DialogLogin_Load (object sender, EventArgs e) {

            this.textBoxPassword.PasswordChar = '\u25CF';
            if (Settings.Default.accountRememberMe) {
                account.companyUsername = Settings.Default.accountCompanyUsername;
                account.username = Settings.Default.accountUsername;
                account.password = Settings.Default.accountPassword;
                account.rememberMe = Settings.Default.accountRememberMe;
                try {
                    database = new Database(Settings.Default.DatabaseHost, Settings.Default.DatabaseUsername, Settings.Default.DatabasePassword);
                    BackgroundWorker backgroundWorker = new BackgroundWorker();
                    backgroundWorker.DoWork += backgroundWorker_DoWork;
                    if (!backgroundWorker.IsBusy) {
                        backgroundWorker.RunWorkerAsync();
                    }
                } catch (DatabaseException databaseException) {
                    MessageBox.Show(this, databaseException.Message, "Database Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log log = new Log(LogFileType.TXT, LogType.EXCEPTION);
                    string logData = DateTime.Now.ToString() + "\t\t databaseException \t\t" + databaseException.Message;
                    log.write(logData);
                } finally {
                }
            }
        }

        private void textBoxPassword_KeyDown (object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                buttonLogin.PerformClick();
            }
        }

        private void pictureBoxSetDatabase_Click (object sender, EventArgs e) {
            DialogDatabase dialogDatabase = new DialogDatabase();
            dialogDatabase.ShowDialog();
        }
    }
}
