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

namespace TqatProReportingTool {
    public partial class FormCellInformation : Form {
        string[,] itemCollection = null;
        string formName = "";
        ReportType reportType;
        double latitude = 0;
        double longitude = 0;

        public FormCellInformation(String formName, string[,] itemCollection) {
            //this.reportType = reportType;
            this.itemCollection = itemCollection;
            this.formName = formName;
            InitializeComponent();
        }

        private void FormCellInformation_Load(object sender, EventArgs e) {
            // for (int index = 0; index < hashtableInformation.Count; index++ ) {
            this.Text = formName;

            this.reportType = (ReportType)Enum.Parse(typeof(ReportType), itemCollection[0, 1], true);



            if (reportType == ReportType.HISTORICAL) {
                buttonViewOnMap.Visible = true;
                this.latitude = double.Parse(itemCollection[2, 1]);
                this.longitude = double.Parse(itemCollection[3, 1]);
            }else if (reportType == ReportType.OVERSPEED) {
                buttonViewOnMap.Visible = true;
                this.latitude = double.Parse(itemCollection[3, 1]);
                this.longitude = double.Parse(itemCollection[4, 1]);
            }
            for (int index = 0; index < 32; index++) {
                string attribute = itemCollection[index, 0];
                string data = itemCollection[index, 1];

                if (String.IsNullOrEmpty(attribute))
                    return;

                ListViewItem listViewItem = new ListViewItem(new string[] { attribute, data });
                listViewInformation.Items.Add(listViewItem);
            }

        }

        private void buttonViewOnMap_Click(object sender, EventArgs e) {
            if (reportType == ReportType.HISTORICAL || reportType == ReportType.OVERSPEED) {
                FormMap formMap = new FormMap(this.latitude, this.longitude, this.Text);
                formMap.Show();
                this.Close();
            }
        }

        private void FormCellInformation_Leave(object sender, EventArgs e) {
            this.Close();
        }
    }
}
