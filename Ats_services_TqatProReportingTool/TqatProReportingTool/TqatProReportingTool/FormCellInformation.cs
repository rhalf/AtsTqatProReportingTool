using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

using Ats;
using Ats.Database;
using Ats.Helper;

namespace TqatProReportingTool {
    public partial class FormCellInformation : Form {
        Hashtable hashtable = null;
        string formName = "";
        ReportType reportType;
        double latitude = 0;
        double longitude = 0;

        double latitudeFrom = 0;
        double longitudeFrom = 0;
        double latitudeTo = 0;
        double longitudeTo = 0;
        List<string> keys;

        bool isApoint = false;
        bool isAline = false;

        public FormCellInformation(String formName, Hashtable hashtable) {
            //this.reportType = reportType;
            this.hashtable = hashtable;
            this.formName = formName;
            InitializeComponent();
        }

        private void FormCellInformation_Load(object sender, EventArgs e) {
            try {

                this.Text = formName;

                this.reportType = (ReportType)hashtable["ReportType"];
                this.keys = (List<string>)hashtable["Keys"];


                if (reportType == ReportType.HISTORICAL || reportType == ReportType.TRACKERS_HISTORICAL || reportType == ReportType.OVERSPEED) {
                    buttonViewOnMap.Visible = true;
                    this.latitude = (double)hashtable["Latitude"];
                    this.longitude = (double)hashtable["Longitude"];
                    this.isApoint = true;
                } else if (reportType == ReportType.RUNNING || reportType == ReportType.IDLING || reportType == ReportType.EXTERNAL_POWER_CUT || reportType == ReportType.ACC || reportType == ReportType.GEOFENCE) {
                    this.latitudeFrom = (double)hashtable["LatitudeFrom"];
                    this.longitudeFrom = (double)hashtable["LongitudeFrom"];
                    this.latitudeTo = (double)hashtable["LatitudeTo"];
                    this.longitudeTo = (double)hashtable["LongitudeTo"];
                    buttonViewOnMap.Visible = true;
                    this.isAline = true;

                }


                for (int index = 0; index < keys.Count; index++) {
                    string key = keys[index];
                    if (String.IsNullOrEmpty(key))
                        return;

                    object obj = hashtable[key];

                    if (obj.GetType() == typeof(double)) {
                        if (!
                            (key == "Latitude" &&
                            key == "Longitude" &&
                            key == "LatitudeFrom" &&
                            key == "LongitudeFrom" &&
                            key == "LatitudeTo" &&
                            key == "LongitudeTo")) {
                            obj = (object)Converter.round((double)obj);
                        }
                    }

                    string data = (string)obj.ToString();


                    ListViewItem listViewItem = new ListViewItem(new string[] { key, data });
                    listViewInformation.Items.Add(listViewItem);
                }
            } catch {
                this.Close();
            }

        }

        private void buttonViewOnMap_Click(object sender, EventArgs e) {
            if (isApoint) {
                FormMap formMap = new FormMap(this.latitude, this.longitude, this.Text);
                formMap.Show();
                this.Close();
            } else if (isAline) {
                FormMap formMap = new FormMap(this.latitudeFrom, this.longitudeFrom,this.latitudeTo,this.longitudeTo, this.Text);
                formMap.Show();
                this.Close();
            }
        }

        private void FormCellInformation_Leave(object sender, EventArgs e) {
            this.Close();
        }
    }
}
