using Ats.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TqatProReportingTool.Properties;
using MySql.Data.MySqlClient;
using System.Threading;

namespace TqatProReportingTool {
    public partial class DialogDatabase : Form {

        DialogLogin dialogLogin = null;

        public DialogDatabase (DialogLogin dialogLogin) {
            InitializeComponent();
            this.dialogLogin = dialogLogin;
        }

        private void buttonDone_Click (object sender, EventArgs e) {
            if (groupBoxOther.Enabled == true) {
                try {
                    Settings.Default.DatabaseHost = textBoxIp.Text;
                    Settings.Default.DatabaseUsername = textBoxUsername.Text;
                    Settings.Default.DatabasePort = Int32.Parse(textBoxPort.Text);
                    Settings.Default.DatabasePassword = textBoxPassword.Text;
                    Settings.Default.Save();
                    this.Close();
                } catch (Exception exception) {
                    MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } else {
                this.Close();
            }
        }

        private void buttonTest_Click (object sender, EventArgs e) {
            try {
                Settings.Default.DatabaseHost = textBoxIp.Text;
                Settings.Default.DatabaseUsername = textBoxUsername.Text;
                Settings.Default.DatabasePort = Int32.Parse(textBoxPort.Text);
                Settings.Default.DatabasePassword = textBoxPassword.Text;
                Settings.Default.DatabaseOther = textBoxIp.Text + "," + textBoxPort.Text + "," + textBoxUsername.Text + "," + textBoxPassword.Text;
                Settings.Default.Save();

                Thread threadMysqlTest = new Thread(threadMysqlTestFunc);
                threadMysqlTest.Start();
            } catch (Exception exception) {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void threadMysqlTestFunc () {
            try {
                Database database = new Database(Settings.Default.DatabaseHost, Settings.Default.DatabaseUsername, Settings.Default.DatabasePassword, Settings.Default.DatabasePort, "");

                database.checkConnection();
                this.Invoke(new Action(() => {
                    MessageBox.Show("Successful", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            } catch (DatabaseException databaseException) {
                this.Invoke(new Action(() => {
                    MessageBox.Show(databaseException.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
            } catch (Exception exception) {
                this.Invoke(new Action(() => {
                    MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
            }
        }

        private void radioButtonServerX_CheckedChanged (object sender, EventArgs e) {

            RadioButton radioButton = (RadioButton)sender;
            groupBoxOther.Enabled = false;

            if (radioButton.Name.Equals("radioButtonServerX")) {
                groupBoxOther.Enabled = true;
                Settings.Default.DatabaseIndex = 3;
                Settings.Default.Save();

            } else if (radioButton.Name.Equals("radioButtonServer1")) {
                //Ats Database Server
                Settings.Default.DatabaseHost = "108.163.190.202";
                Settings.Default.DatabaseUsername = "atstqatpro";
                Settings.Default.DatabasePort = 3306;
                Settings.Default.DatabasePassword = "@t5tq@pr0!@#";
                Settings.Default.DatabaseIndex = 1;
                Settings.Default.Save();
            } else if (radioButton.Name.Equals("radioButtonServer2")) {
                //Ats Mowasalat Server
                Settings.Default.DatabaseHost = "184.107.175.154";
                Settings.Default.DatabaseUsername = "reportapp";
                Settings.Default.DatabasePort = 3306;
                Settings.Default.DatabasePassword = "my5q1r3p0rt@pp!@#";
                Settings.Default.DatabaseIndex = 2;
                Settings.Default.Save();
            } else {
                groupBoxOther.Enabled = false;
                Settings.Default.DatabaseIndex = 0;
                Settings.Default.Save();
            }
        }

        private void DialogDatabase_Load (object sender, EventArgs e) {
            this.textBoxPassword.PasswordChar = '\u25CF';
            groupBoxOther.Enabled = false;

            if (Settings.Default.DatabaseIndex == 1) {
                radioButtonServer1.Checked = true;
            } else if (Settings.Default.DatabaseIndex == 2) {
                radioButtonServer2.Checked = true;
            } else if (Settings.Default.DatabaseIndex == 3) {
                radioButtonServerX.Checked = true;
            }

            try {
                String[] db = Settings.Default.DatabaseOther.Split(',');
                textBoxIp.Text = db[0];
                textBoxPort.Text = db[1];
                textBoxUsername.Text = db[2];
                textBoxPassword.Text = db[3];
            } catch {

            }
        }

        private void DialogDatabase_FormClosed (object sender, FormClosedEventArgs e) {
            dialogLogin.Show();
        }
    }
}
