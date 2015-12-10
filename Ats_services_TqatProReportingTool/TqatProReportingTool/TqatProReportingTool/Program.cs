﻿using System;
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
            } 

            DialogLogin dialogLogin = new DialogLogin();
            dialogLogin.Show();

            Application.Run();
        }
    }
}
