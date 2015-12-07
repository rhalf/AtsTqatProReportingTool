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

        static void Main () {

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

            int appMode = 2;

            if (appMode == 0) {
                Settings.Default.Reset();
            } else if (appMode == 1) {
                //Do nothing
            }

            #endregion
            #region -loop
            while (true) {
                Account account = new Account();

                if (login(ref account) == DialogResult.Cancel) {
                    Environment.Exit(0);
                } else {
                    if (mainApplication(ref account) == DialogResult.Cancel) {
                        Environment.Exit(0);
                    } else {
                        continue;
                    }
                }
            }
            #endregion
        }

        public static DialogResult login (ref Account account) {
            DialogLogin dialogLogin = new DialogLogin(ref account);
            return dialogLogin.ShowDialog();
        }

        public static DialogResult mainApplication (ref Account account) {

            FormMain formMain = new FormMain(ref account);
            return formMain.ShowDialog();
        }
    }
}
