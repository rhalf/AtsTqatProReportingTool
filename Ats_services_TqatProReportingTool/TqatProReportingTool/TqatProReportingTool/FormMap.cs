using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace TqatProReportingTool {
    public partial class FormMap : Form {
        double latitude = 0;
        double longitude = 0;

        public FormMap(double latitude, double longitude, string formName) {
            InitializeComponent();
            this.Text = formName;
            this.longitude = longitude;
            this.latitude = latitude;
        }

        private void FormMap_Load(object sender, EventArgs e) {
            Uri uri = new Uri(Directory.GetCurrentDirectory() + "\\html\\locate.html?latitude=" + this.latitude.ToString() + "&longitude=" + this.longitude.ToString());
            Debug.Print(uri.AbsoluteUri);
            webBrowserMap.Url = uri;
            webBrowserMap.Update();
        }

        private void FormMap_Leave(object sender, EventArgs e) {
            this.Close();
        }
    }
}
