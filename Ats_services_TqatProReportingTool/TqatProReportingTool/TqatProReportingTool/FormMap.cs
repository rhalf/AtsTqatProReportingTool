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

        double latitudeFrom = 0;
        double longitudeFrom = 0;
        double latitudeTo = 0;
        double longitudeTo = 0;

        Uri uri;

        public FormMap(double latitude, double longitude, string formName) {
            InitializeComponent();
            this.Text = formName;
            this.longitude = longitude;
            this.latitude = latitude;
            uri = new Uri(Directory.GetCurrentDirectory() + "\\html\\locate.html?command=point&latitude=" + this.latitude.ToString() + "&longitude=" + this.longitude.ToString());
        }
        public FormMap(double latitudeFrom, double longitudeFrom, double latitudeTo, double longitudeTo, string formName) {
            InitializeComponent();
            this.Text = formName;
            this.latitudeFrom = latitudeFrom;
            this.longitudeFrom = longitudeFrom;
            this.latitudeTo = latitudeTo;
            this.longitudeTo = longitudeTo;
            uri = new Uri(Directory.GetCurrentDirectory() + "\\html\\locate.html?command=polyline&latitudeFrom=" + this.latitudeFrom.ToString() + 
                                                                                                "&longitudeFrom=" + this.longitudeFrom.ToString() +
                                                                                                "&latitudeTo=" + this.latitudeTo.ToString() +
                                                                                                "&longitudeTo=" + this.longitudeTo.ToString());


        }

        private void FormMap_Load(object sender, EventArgs e) {
            Debug.Print(uri.AbsoluteUri);
            webBrowserMap.Url = uri;
            webBrowserMap.Update();
        }

        private void FormMap_Leave(object sender, EventArgs e) {
            this.Close();
        }

        private void FormMap_Resize(object sender, EventArgs e) {
            webBrowserMap.Refresh();
        }
    }
}
