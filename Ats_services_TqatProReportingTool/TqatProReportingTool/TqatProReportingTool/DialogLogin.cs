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

        User user = new User();
        Company company = new Company();


        Database database = new Database(Settings.Default.DatabaseHost, Settings.Default.DatabaseUsername, Settings.Default.DatabasePassword);

        public DialogLogin() {
            InitializeComponent();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void buttonLogin_Click(object sender, EventArgs e) {
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

            try {

                company.Username = textBoxCompany.Text;
                user.Username = textBoxUsername.Text;
                user.Password = textBoxPassword.Text;
                user.RememberMe = checkBoxRememberMe.Checked;

                ThreadPool.QueueUserWorkItem(new WaitCallback(run), null);

            } catch (DatabaseException databaseException) {
                MessageBox.Show(this, databaseException.Message, "Database Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log log = new Log(LogFileType.TXT, LogType.EXCEPTION);
                string logData = DateTime.Now.ToString() + "\t\t databaseException \t\t" + databaseException.Message;
                log.write(logData);
            } finally {
            }
        }

        void run(object state) {
            pictureBoxLoading.Invoke(new MethodInvoker(delegate {
                pictureBoxLoading.Visible = true;
            }));

            try {
                Query query = new Query(database);

                query.getCompany(company);
                query.getUser(company, user);

                if (user.AccessLevel != 1) {

                    int isExpired = company.DateTimeExpired.CompareTo(DateTime.Now);

                    if (isExpired == -1)
                        throw new QueryException(1, "This company is expired.");
                    if (!company.IsActive)
                        throw new QueryException(1, "Company is deactivated.");

                    isExpired = user.DateTimeExpired.CompareTo(DateTime.Now);

                    if (isExpired == -1)
                        throw new QueryException(1, "This user is expired.");
                    if (!user.IsActive)
                        throw new QueryException(1, "User is deactivated.");
                }
                //=============================Login successful

                query.fillGeofences(company);

                query.fillUsers(company, user);

                query.fillCollection(company);

                query.fillTrackers(company);

                query.fillPois(company);




                this.Invoke(new MethodInvoker(delegate {
                    Settings.Default.accountRememberMe = (bool)user.RememberMe;
                    Settings.Default.accountCompanyUsername = company.Username;
                    Settings.Default.accountUsername = user.Username;
                    Settings.Default.accountPassword = user.Password;
                    Settings.Default.Save();

                    FormMain formMain = new FormMain(company, user);
                    formMain.Show();
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

        private void DialogLogin_Load(object sender, EventArgs e) {
            labelApp.Text = Application.ProductVersion;
            this.textBoxPassword.PasswordChar = '\u25CF';
            if (Settings.Default.accountRememberMe) {
                company.Username = Settings.Default.accountCompanyUsername;
                user.Username = Settings.Default.accountUsername;
                user.Password = Settings.Default.accountPassword;
                user.RememberMe = Settings.Default.accountRememberMe;

                textBoxCompany.Text = Settings.Default.accountCompanyUsername;
                textBoxPassword.Text = Settings.Default.accountPassword;
                textBoxUsername.Text = Settings.Default.accountUsername;
                checkBoxRememberMe.Checked = Settings.Default.accountRememberMe;
            }
        }

        private void textBoxPassword_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                buttonLogin.PerformClick();
            }
        }

        private void pictureBoxSetDatabase_Click (object sender, EventArgs e) {
            this.Hide();
            DialogDatabase dialogDatabase = new DialogDatabase(this);
            dialogDatabase.ShowDialog();
        }
    }
}
