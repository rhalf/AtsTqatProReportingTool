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
            //if (Debugger.IsAttached) {
            //    Settings.Default.Reset();
            //}

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


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

            DialogLogin dialogLogin = new DialogLogin();
            dialogLogin.Show();

            Application.Run();
        }
    }
}
