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
using Ats.Database;

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
                comboBoxReportType.Items.Remove(Enum.GetName(typeof(ReportType), ReportType.ALL_COMPANIES));
                comboBoxReportType.Items.Remove(Enum.GetName(typeof(ReportType), ReportType.ALL_TRACKERS));
                tabControlAdvancedSettings.TabPages.RemoveAt(tabControlAdvancedSettings.TabPages.IndexOfKey("tabPage2"));
            }

            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            ToolStripMenuItem toolStripMenuItemCheckAll = (ToolStripMenuItem)contextMenuStrip.Items.Add("Check All");
            toolStripMenuItemCheckAll.Click += toolStripMenuItemCheckAll_Click;
            ToolStripMenuItem toolStripMenuItemUncheckAll = (ToolStripMenuItem)contextMenuStrip.Items.Add("Uncheck All");
            toolStripMenuItemUncheckAll.Click += toolStripMenuItemUncheckAll_Click;

            checkedListBoxColumnName.ContextMenuStrip = contextMenuStrip;
        }

        void toolStripMenuItemUncheckAll_Click(object sender, EventArgs e) {
            for (int index = 0; index < checkedListBoxColumnName.Items.Count; index++) {
                checkedListBoxColumnName.SetItemChecked(index, false);
            }
        }

        void toolStripMenuItemCheckAll_Click(object sender, EventArgs e) {
            for (int index = 0; index < checkedListBoxColumnName.Items.Count; index++) {
                checkedListBoxColumnName.SetItemChecked(index, true);
            }
        }

        private void comboBoxReportType_SelectedIndexChanged(object sender, EventArgs e) {

            if (comboBoxReportType.SelectedIndex == -1)
                return;

            checkedListBoxColumnName.Items.Clear();

            ReportType reportType = (ReportType)Enum.Parse(typeof(ReportType), comboBoxReportType.Text);



            Database database = new Database(Settings.Default.DatabaseHost, Settings.Default.DatabaseUsername, Settings.Default.DatabasePassword, 30);
            Query query = new Query(database);
            DataTable dataTable = query.getReportTable(reportType);
            foreach (DataColumn dataColumn in dataTable.Columns) {
                checkedListBoxColumnName.Items.Add(dataColumn.ColumnName);
            }

            uint setting = 0;
            switch (reportType) {
                case ReportType.HISTORICAL:
                    setting = Settings.Default.tableHistorical;
                    break;
                case ReportType.RUNNING:
                    setting = Settings.Default.tableRunning;
                    break;
                case ReportType.IDLING:
                    setting = Settings.Default.tableIdle;
                    break;
                case ReportType.ACC:
                    setting = Settings.Default.tableAcc;
                    break;
                case ReportType.GEOFENCE:
                    setting = Settings.Default.tableGeofence;
                    break;
                case ReportType.OVERSPEED:
                    setting = Settings.Default.tableOverspeed;
                    break;
                case ReportType.EXTERNAL_POWER_CUT:
                    setting = Settings.Default.tableExternalPowerCut;
                    break;
                case ReportType.TRACKERS:
                    setting = Settings.Default.tableTrackers;
                    break;
                case ReportType.TRACKERS_GEOFENCE:
                    setting = Settings.Default.tableTrackersGeofence;
                    break;
                case ReportType.TRACKERS_HISTORICAL:
                    setting = Settings.Default.tableTrackersHistorical;
                    break;
                case ReportType.ALL_TRACKERS:
                    setting = Settings.Default.tableAllTrackers;
                    break;
                case ReportType.ALL_COMPANIES:
                    setting = Settings.Default.tableAllCompanies;
                    break;
            }

            int totalItems = dataTable.Columns.Count;
            for (int index = 0; index < totalItems; index++) {
                uint status = Converter.getBit(setting, index);
                bool flag = (status == 1) ? true : false;
                checkedListBoxColumnName.SetItemChecked(index, flag);

            }

            checkedListBoxColumnNameUpdate();
        }

        private void checkedListBoxColumnNameUpdate() {
            int checkListBoxColumnCount = checkedListBoxColumnName.Items.Count;

            //for (int index = 0; index < checkListBoxColumnCount; index++)
            //    checkedListBoxColumnName.SetItemChecked(index, checkBoxSelectAll.Checked);

            labelReportType.Text = "Columns : " + checkListBoxColumnCount;
        }

        private void checkBoxSelectAll_CheckedChanged(object sender, EventArgs e) {

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
                case ReportType.EXTERNAL_POWER_CUT:
                    for (int index = 0; index < checkedListBoxColumnName.Items.Count; index++) {
                        bool flag = checkedListBoxColumnName.GetItemChecked(index);
                        Settings.Default.tableExternalPowerCut = Converter.setBit(Settings.Default.tableExternalPowerCut, index, flag);
                        Settings.Default.Save();
                    }
                    break;
                case ReportType.TRACKERS:
                    for (int index = 0; index < checkedListBoxColumnName.Items.Count; index++) {
                        bool flag = checkedListBoxColumnName.GetItemChecked(index);
                        Settings.Default.tableTrackers = Converter.setBit(Settings.Default.tableTrackers, index, flag);
                        Settings.Default.Save();
                    }
                    break;
                case ReportType.TRACKERS_GEOFENCE:
                    for (int index = 0; index < checkedListBoxColumnName.Items.Count; index++) {
                        bool flag = checkedListBoxColumnName.GetItemChecked(index);
                        Settings.Default.tableTrackersGeofence = Converter.setBit(Settings.Default.tableTrackersGeofence, index, flag);
                        Settings.Default.Save();
                    }
                    break;
                case ReportType.TRACKERS_HISTORICAL:
                    for (int index = 0; index < checkedListBoxColumnName.Items.Count; index++) {
                        bool flag = checkedListBoxColumnName.GetItemChecked(index);
                        Settings.Default.tableTrackersHistorical = Converter.setBit(Settings.Default.tableTrackersHistorical, index, flag);
                        Settings.Default.Save();
                    }
                    break;

                case ReportType.ALL_TRACKERS:
                    for (int index = 0; index < checkedListBoxColumnName.Items.Count; index++) {
                        bool flag = checkedListBoxColumnName.GetItemChecked(index);
                        Settings.Default.tableAllTrackers = Converter.setBit(Settings.Default.tableAllTrackers, index, flag);
                        Settings.Default.Save();
                    }
                    break;
                case ReportType.ALL_COMPANIES:
                    for (int index = 0; index < checkedListBoxColumnName.Items.Count; index++) {
                        bool flag = checkedListBoxColumnName.GetItemChecked(index);
                        Settings.Default.tableAllCompanies = Converter.setBit(Settings.Default.tableAllCompanies, index, flag);
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

                            switch (reportType) {
                                case ReportType.HISTORICAL:
                                    for (int index = 0; index < dataGridViewInformation.ColumnCount; index++) {
                                        uint flag = Converter.getBit(Settings.Default.tableHistorical, index);
                                        dataGridViewInformation.Columns[index].Visible = (flag == 1) ? true : false;
                                    }
                                    break;
                                case ReportType.RUNNING:
                                    for (int index = 0; index < dataGridViewInformation.ColumnCount; index++) {
                                        uint flag = Converter.getBit(Settings.Default.tableRunning, index);
                                        dataGridViewInformation.Columns[index].Visible = (flag == 1) ? true : false;
                                    }

                                    break;
                                case ReportType.IDLING:
                                    for (int index = 0; index < dataGridViewInformation.ColumnCount; index++) {
                                        uint flag = Converter.getBit(Settings.Default.tableIdle, index);
                                        dataGridViewInformation.Columns[index].Visible = (flag == 1) ? true : false;
                                    }
                                    break;
                                case ReportType.GEOFENCE:
                                    for (int index = 0; index < dataGridViewInformation.ColumnCount; index++) {
                                        uint flag = Converter.getBit(Settings.Default.tableGeofence, index);
                                        dataGridViewInformation.Columns[index].Visible = (flag == 1) ? true : false;
                                    }
                                    break;
                                case ReportType.ACC:
                                    for (int index = 0; index < dataGridViewInformation.ColumnCount; index++) {
                                        uint flag = Converter.getBit(Settings.Default.tableAcc, index);
                                        dataGridViewInformation.Columns[index].Visible = (flag == 1) ? true : false;
                                    }
                                    break;
                                case ReportType.OVERSPEED:
                                    for (int index = 0; index < dataGridViewInformation.ColumnCount; index++) {
                                        uint flag = Converter.getBit(Settings.Default.tableOverspeed, index);
                                        dataGridViewInformation.Columns[index].Visible = (flag == 1) ? true : false;
                                    }
                                    break;
                                case ReportType.EXTERNAL_POWER_CUT:
                                    for (int index = 0; index < dataGridViewInformation.ColumnCount; index++) {
                                        uint flag = Converter.getBit(Settings.Default.tableExternalPowerCut, index);
                                        dataGridViewInformation.Columns[index].Visible = (flag == 1) ? true : false;
                                    }
                                    break;
                                case ReportType.TRACKERS:
                                    for (int index = 0; index < dataGridViewInformation.ColumnCount; index++) {
                                        uint flag = Converter.getBit(Settings.Default.tableTrackers, index);
                                        dataGridViewInformation.Columns[index].Visible = (flag == 1) ? true : false;
                                    }
                                    break;
                                case ReportType.TRACKERS_GEOFENCE:
                                    for (int index = 0; index < dataGridViewInformation.ColumnCount; index++) {
                                        uint flag = Converter.getBit(Settings.Default.tableTrackersGeofence, index);
                                        dataGridViewInformation.Columns[index].Visible = (flag == 1) ? true : false;
                                    }
                                    break;
                                case ReportType.TRACKERS_HISTORICAL:
                                    for (int index = 0; index < dataGridViewInformation.ColumnCount; index++) {
                                        uint flag = Converter.getBit(Settings.Default.tableTrackersHistorical, index);
                                        dataGridViewInformation.Columns[index].Visible = (flag == 1) ? true : false;
                                    }
                                    break;
                                case ReportType.ALL_TRACKERS:
                                    for (int index = 0; index < dataGridViewInformation.ColumnCount; index++) {
                                        uint flag = Converter.getBit(Settings.Default.tableAllTrackers, index);
                                        dataGridViewInformation.Columns[index].Visible = (flag == 1) ? true : false;
                                    }
                                    break;
                                case ReportType.ALL_COMPANIES:
                                    for (int index = 0; index < dataGridViewInformation.ColumnCount; index++) {
                                        uint flag = Converter.getBit(Settings.Default.tableAllCompanies, index);
                                        dataGridViewInformation.Columns[index].Visible = (flag == 1) ? true : false;
                                    }
                                    break;
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

        private void checkBoxSelectAll_Click(object sender, EventArgs e) {

        }

        private void checkedListBoxColumnName_ItemCheck(object sender, ItemCheckEventArgs e) {

        }
    }
}
