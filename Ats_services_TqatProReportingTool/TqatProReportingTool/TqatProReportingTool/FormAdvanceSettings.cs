using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Ats;
using Ats.Helper;
using Ats.Session;
using TqatProReportingTool.Properties;

namespace TqatProReportingTool {
    public partial class FormAdvanceSettings : Form {
        TabControl tabControl;
        Account account;

        public FormAdvanceSettings(Account acount, TabControl tabControl) {
            this.tabControl = tabControl;
            this.account = acount;
            InitializeComponent();

        }

        private void FormAdvanceSettings_Load(object sender, EventArgs e) {
            this.textBoxPassword.PasswordChar = '\u25CF';
            textBoxHost.Text = Settings.Default.DatabaseHost;
            textBoxUsername.Text = Settings.Default.DatabaseUsername;
            textBoxPassword.Text = Settings.Default.DatabasePassword;

            textBoxValuesFuelToKilometers.Text = Settings.Default.fuelLiterToKilometer.ToString("0.00");
            textBoxValuesFuelToCost.Text = Settings.Default.fuelLiterToCost.ToString("0.00");

            comboBoxReportType.Items.AddRange(Enum.GetNames(typeof(ReportType)));
            if (account.accessLevel > 1) {
                comboBoxReportType.Items.Remove("ALLCOMPANIES");
                comboBoxReportType.Items.Remove("ALLTRACKERS");
                tabControlAdvancedSettings.TabPages.RemoveAt(tabControlAdvancedSettings.TabPages.IndexOfKey("tabPage2"));
            }

        }

        private void comboBoxReportType_SelectedIndexChanged(object sender, EventArgs e) {

            if (comboBoxReportType.SelectedIndex == -1)
                return;

            checkedListBoxColumnName.Items.Clear();

            ReportType reportType = (ReportType)Enum.Parse(typeof(ReportType), comboBoxReportType.Text);


            switch (reportType) {
                case ReportType.HISTORICAL:
                    historical();
                    break;
                case ReportType.RUNNING:
                    running();
                    break;
                case ReportType.IDLING:
                    idle();
                    break;
                case ReportType.GEOFENCE:
                    geofence();
                    break;
                case ReportType.ACC:
                    acc();
                    break;
                case ReportType.OVERSPEED:
                    overspeed();
                    break;
                case ReportType.TRACKERS:
                    trackers();
                    break;
                case ReportType.ALLCOMPANIES:
                    break;
                case ReportType.ALLTRACKERS:
                    break;
            }

            checkedListBoxColumnNameUpdate();
        }

        private void checkedListBoxColumnNameUpdate() {
            int checkListBoxColumnCount = checkedListBoxColumnName.Items.Count;

            //for (int index = 0; index < checkListBoxColumnCount; index++)
            //    checkedListBoxColumnName.SetItemChecked(index, checkBoxSelectAll.Checked);

            labelReportType.Text = "Columns : " + checkListBoxColumnCount;
        }

        private void running() {
            string[] running = new string[]{
                "No", 
                "Status",
                "DateTimeFrom", 
                "DateTimeTo", 
                "Time", 
                "SpeedMax",
                "SpeedAve",
                "Distance",
                "Fuel",  
                "Cost",  
                "Geofence"
              };
            checkedListBoxColumnName.Items.AddRange(running);
            int totalItems = running.ToList().Count;
            for (int index = 0; index < totalItems; index++) {
                uint status = Converter.getBit(Settings.Default.tableRunning, index);
                bool flag = (status == 1) ? true : false;
                checkedListBoxColumnName.SetItemChecked(index, flag);

            }
        }
        private void geofence() {
            string[] geofence = new string[]{
                "No",
                "Status",
                "DateTimeFrom", 
                "DateTimeTo",
                "Time",
                "SpeedMax", 
                "SpeedAve", 
                "Distance",
                "Fuel",  
                "Cost",  
                "Geofence"
              };
            checkedListBoxColumnName.Items.AddRange(geofence);
            int totalItems = geofence.ToList().Count;
            for (int index = 0; index < totalItems; index++) {
                uint status = Converter.getBit(Settings.Default.tableGeofence, index);
                bool flag = (status == 1) ? true : false;
                checkedListBoxColumnName.SetItemChecked(index, flag);

            }
        }
        private void idle() {
            string[] idle = new string[]{
                "No", 
                "Status",
                "DateTimeFrom", 
                "DateTimeTo", 
                "Time", 
                "SpeedMax",
                "SpeedAve",
                "Distance",
                "Fuel",  
                "Cost",  
                "Geofence"
              };

            checkedListBoxColumnName.Items.AddRange(idle);
            int totalItems = idle.ToList().Count;
            for (int index = 0; index < totalItems; index++) {
                uint status = Converter.getBit(Settings.Default.tableIdle, index);
                bool flag = (status == 1) ? true : false;
                checkedListBoxColumnName.SetItemChecked(index, flag);

            }
        }
        private void historical() {
            string[] historical = new string[]{
                "No", 
                "DateTime", 
                "Latitude",  
                "Longitude",  
                "Speed", 
                "Mileage",  
                "Fuel",  
                "Cost",  
                "Altitude", 
                "Degrees", 
                "Direction",  
                "GpsSatellites", 
                "GsmSignal", 
                "EventCode",  
                "Geofence",  
                "ACC",  
                "SOS",  
                "OS",  
                "Battery",  
                "BatteryVolt",  
                "ExternalVolt"
              };

            checkedListBoxColumnName.Items.AddRange(historical);
            int totalItems = historical.ToList().Count;
            for (int index = 0; index < totalItems; index++) {
                uint status = Converter.getBit(Settings.Default.tableHistorical, index);
                bool flag = (status == 1) ? true : false;
                checkedListBoxColumnName.SetItemChecked(index, flag);

            }

        }

        private void acc() {
            string[] acc = new string[]{
                  "No", 
                "Status",
                "DateTimeFrom", 
                "DateTimeTo", 
                "Time", 
                "SpeedMax",
                "SpeedAve",
                "Distance",
                "Fuel",  
                "Cost",  
                "Geofence"
              };

            checkedListBoxColumnName.Items.AddRange(acc);
            int totalItems = acc.ToList().Count;
            for (int index = 0; index < totalItems; index++) {
                uint status = Converter.getBit(Settings.Default.tableAcc, index);
                bool flag = (status == 1) ? true : false;
                checkedListBoxColumnName.SetItemChecked(index, flag);

            }

        }
        private void overspeed() {
            string[] overspeed = new string[]{
                "No", 
                "Status",
                "DateTime",
                "Latitude",  
                "Longitude",  
                "Speed", 
                "Mileage",  
                "Geofence"
              };

            checkedListBoxColumnName.Items.AddRange(overspeed);
            int totalItems = overspeed.ToList().Count;
            for (int index = 0; index < totalItems; index++) {
                uint status = Converter.getBit(Settings.Default.tableOverspeed, index);
                bool flag = (status == 1) ? true : false;
                checkedListBoxColumnName.SetItemChecked(index, flag);

            }

        }
        private void trackers() {
            checkedListBoxColumnName.Items.AddRange(new string[]{
             "id",
             "vehicleRegistration",
             "vehicleModel",
             "ownerName",
             "driverName",
             "simImei", 
             "simNumber",
            "mobileDataProvider",
            "deviceImei",
            "devicePassword",
            "deviceType",
            "emails",
            "users", 
            "mileageInitial",
            "mileageLimit",
            "speedLimit", 
            "idlingTime",
            "inputs",
            "imageNumber",        
            "note", 
            "collections",
            "companyDatabaseName",
            "databaseHost",
            "dataDatabaseName",
            "httpHost",
            "dateCreated",
            "dateExpired"
            });

        }

        private void checkBoxSelectAll_CheckedChanged(object sender, EventArgs e) {
            for (int index = 0; index < checkedListBoxColumnName.Items.Count; index++) {
                checkedListBoxColumnName.SetItemChecked(index, checkBoxSelectAll.Checked);
            }
        }

        private void buttonColumnApply_Click(object sender, EventArgs e) {

            ReportType reportType = (ReportType)Enum.Parse(typeof(ReportType), comboBoxReportType.Text);
            switch (reportType) {
                case ReportType.HISTORICAL:
                    for (int index = 0; index < checkedListBoxColumnName.Items.Count; index++) {
                        bool flag = checkedListBoxColumnName.GetItemChecked(index);
                        Settings.Default.tableHistorical = Converter.setBit(Settings.Default.tableHistorical, index, flag);
                        Settings.Default.Save();
                    }
                    break;
                case ReportType.RUNNING:
                    for (int index = 0; index < checkedListBoxColumnName.Items.Count; index++) {
                        bool flag = checkedListBoxColumnName.GetItemChecked(index);
                        Settings.Default.tableRunning = Converter.setBit(Settings.Default.tableRunning, index, flag);
                        Settings.Default.Save();
                    }
                    break;
                case ReportType.IDLING:
                    for (int index = 0; index < checkedListBoxColumnName.Items.Count; index++) {
                        bool flag = checkedListBoxColumnName.GetItemChecked(index);
                        Settings.Default.tableIdle = Converter.setBit(Settings.Default.tableIdle, index, flag);
                        Settings.Default.Save();
                    }
                    break;
                case ReportType.GEOFENCE:
                    for (int index = 0; index < checkedListBoxColumnName.Items.Count; index++) {
                        bool flag = checkedListBoxColumnName.GetItemChecked(index);
                        Settings.Default.tableGeofence = Converter.setBit(Settings.Default.tableGeofence, index, flag);
                        Settings.Default.Save();
                    }
                    break;
                case ReportType.ACC:
                    for (int index = 0; index < checkedListBoxColumnName.Items.Count; index++) {
                        bool flag = checkedListBoxColumnName.GetItemChecked(index);
                        Settings.Default.tableAcc = Converter.setBit(Settings.Default.tableAcc, index, flag);
                        Settings.Default.Save();
                    }
                    break;
                case ReportType.OVERSPEED:
                    for (int index = 0; index < checkedListBoxColumnName.Items.Count; index++) {
                        bool flag = checkedListBoxColumnName.GetItemChecked(index);
                        Settings.Default.tableOverspeed = Converter.setBit(Settings.Default.tableOverspeed, index, flag);
                        Settings.Default.Save();
                    }
                    break;

            }


            if (tabControl != null) {
                if (tabControl.TabPages.Count > 0) {
                    foreach (TabPage tabPage in tabControl.TabPages) {
                        TableLayoutPanel TableLayoutInformation = (TableLayoutPanel)tabPage.Controls[0];
                        DataGridView dataGridViewInformation = (DataGridView)TableLayoutInformation.Controls[0];

                        if (dataGridViewInformation.Name == comboBoxReportType.Text) {

                            foreach (DataGridViewColumn dataGridViewColumn in dataGridViewInformation.Columns) {

                                for (int index = 0; index < checkedListBoxColumnName.Items.Count; index++) {
                                    bool status = checkedListBoxColumnName.GetItemChecked(index);

                                    if (dataGridViewColumn.Name == checkedListBoxColumnName.GetItemText(checkedListBoxColumnName.Items[index])) {
                                        dataGridViewColumn.Visible = status;
                                    }

                                }
                            }
                        }

                    }
                }
            }

            MessageBox.Show(this, Enum.GetName(typeof(ReportType), reportType) + " has been saved!", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonSessionClear_Click(object sender, EventArgs e) {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to remove this credentials?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (dialogResult == System.Windows.Forms.DialogResult.OK) {
                Settings.Default.Reset();
                textBoxHost.Clear();
                textBoxUsername.Clear();
                textBoxPassword.Clear();
            }
        }

        private void buttonValuesApply_Click(object sender, EventArgs e) {
            try {
                Settings.Default.fuelLiterToKilometer = float.Parse(textBoxValuesFuelToKilometers.Text);
                Settings.Default.fuelLiterToCost = float.Parse(textBoxValuesFuelToCost.Text);
                Settings.Default.Save();
                MessageBox.Show(this, "Fuel and Cost has been saved!", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } catch (Exception exception) {
                Log log = new Log(LogFileType.TXT, LogType.EXCEPTION);
                string logData = DateTime.Now.ToString() + "\t\t Exception \t\t" + "Values" + " : " + exception.Message;
                log.write(logData);
                MessageBox.Show(this, "Wrong Format! Please try again. Ex. '10.00'", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
