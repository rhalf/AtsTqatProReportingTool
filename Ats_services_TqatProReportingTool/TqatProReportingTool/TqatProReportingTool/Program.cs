using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;


using Ats.Helper;
using Ats.Session;
using TqatProReportingTool.Properties;


namespace TqatProReportingTool {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]

        static void Main() {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            #region -validation

            //DateTime dateTime = new DateTime(2015, 6, 1);
            //int result = DateTime.Compare(DateTime.Now, dateTime);
            //if (result == 1) {
            //    //d2 is ahead of d1
            //    MessageBox.Show("This application is expired.", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;

            //} else if (result == -1) {
            //} else if (result == 0) {
            //}

            #endregion
            #region -mode
            //if (Debugger.IsAttached) {
            //    Settings.Default.Reset();
            //}

            //0 = ResetMode 
            //1 = DevelopersMode 
            //2 = Static Ip
            int appMode = 2;
            int appServer = 2;

            if (appMode == 0) {
                Settings.Default.Reset();
            } else if (appMode == 1) {
                //Do nothing
            } else if (appMode == 2) {
                switch (appServer) {
                    case 1:
                        //Ats Mowasalat Server
                        Settings.Default.DatabaseHost = "184.107.175.154";
                        Settings.Default.DatabaseUsername = "reportapp";
                        Settings.Default.DatabasePassword = "my5q1r3p0rt@pp!@#";
                        break;
                    case 2:
                        //Ats Database Server
                        Settings.Default.DatabaseHost = "108.163.190.202";
                        Settings.Default.DatabaseUsername = "atstqatpro";
                        Settings.Default.DatabasePassword = "@t5tq@pr0!@#";

                        break;
                }
                Settings.Default.Save();
            }

            #endregion
            #region -loop
            while (true) {
                Account account = new Account();


                if ((string.IsNullOrEmpty(Settings.Default.DatabaseHost) || string.IsNullOrEmpty(Settings.Default.DatabaseUsername) || string.IsNullOrEmpty(Settings.Default.DatabasePassword))) {
                    DialogDatabaseConfiguration dialogDatabaseConfiguration = new DialogDatabaseConfiguration();
                    if (dialogDatabaseConfiguration.ShowDialog() == DialogResult.OK) {
                        if (login(ref account) == DialogResult.Cancel) {
                            break;
                        } else {
                            DialogResult dialogResult = mainApplication(ref account);
                            if (dialogResult == DialogResult.Cancel) {
                                break;
                            }
                        }
                    } else {
                        break;
                    }
                } else {
                    if (login(ref account) == DialogResult.Cancel) {
                        break;
                    } else {
                        DialogResult dialogResult = mainApplication(ref account);
                        if (dialogResult == DialogResult.Cancel) {
                            break;
                        }
                    }
                }
            }
            #endregion
        }

        public static DialogResult login(ref Account account) {
            DialogLogin dialogLogin = new DialogLogin(ref account);
            return dialogLogin.ShowDialog();
        }

        public static DialogResult mainApplication(ref Account account) {
            FormMain formMain = new FormMain(ref account);
            return formMain.ShowDialog();
        }
    }
}
