using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Collections;
using System.Threading;
using System.IO;

using Ats;
using Ats.Session;
using Ats.Extension;
using Ats.Database;
using Ats.Helper;
using Ats.Report;

using MySql.Data.MySqlClient;
using TqatProReportingTool.Properties;
using System.Windows.Forms.DataVisualization.Charting;
using System.Reflection;
using System.Globalization;

namespace TqatProReportingTool {

    public partial class FormMain : Form {

        #region Global Reference

        User user;
        Company company;

        Database database;
        TabControl tabControl;

        double workerThreadCount = 0;
        double workerThreadFinished = 0;

        //Delegates


        #endregion

        #region Initialization

        public FormMain(Company company, User user) {
            InitializeComponent();

            this.DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            this.user = user;
            this.company = company;

            database = new Database(Settings.Default.DatabaseHost, Settings.Default.DatabaseUsername, Settings.Default.DatabasePassword, 30);
        }

        private void FormMain_Load(object sender, EventArgs e) {

            //Search Category
            comboBoxUser.DataSource = company.Users;
            comboBoxUser.DisplayMember = "Username";
            comboBoxUser.ValueMember = "Id";
            comboBoxUser.SelectedItem = comboBoxUser.Items[0];

            comboBoxTrackersDisplayMember.Items.Clear();
            comboBoxTrackersDisplayMember.Items.AddRange(new string[] { "VehicleRegistration", "DriverName", "OwnerName", "VehicleModel", "SimNumber", "SimImei", "TrackerImei" });

            comboBoxReportType.Items.Clear();
            comboBoxReportType.Items.AddRange(Enum.GetNames(typeof(ReportType)));
            if (user.AccessLevel > 1) {
                comboBoxReportType.Items.Remove(Enum.GetName(typeof(ReportType), ReportType.ALL_COMPANIES));
                comboBoxReportType.Items.Remove(Enum.GetName(typeof(ReportType), ReportType.ALL_TRACKERS));
            }

            comboBoxTrackersDisplayMember.SelectedItem = comboBoxTrackersDisplayMember.Items[0];
            comboBoxReportType.SelectedItem = comboBoxReportType.Items[0];

            //Date and Time
            dateTimePickerDateTo.Format = DateTimePickerFormat.Custom;
            dateTimePickerDateTo.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            dateTimePickerDateFrom.Format = DateTimePickerFormat.Custom;
            dateTimePickerDateFrom.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            dateTimePickerDateTo.Value = (DateTime.Today.AddDays(-1));
            dateTimePickerDateFrom.Value = (DateTime.Today);


            comboBoxUser.SelectedIndexChanged += comboBoxUser_SelectedIndexChanged;
            comboBoxTrackersDisplayMember.SelectedIndexChanged += comboBoxTrackersBound_SelectedIndexChanged;
            comboBoxUser_SelectedIndexChanged(this, new EventArgs());

            comboBoxDateShortCut.Items.Clear();
            comboBoxDateShortCut.Items.AddRange(new string[] { 
                "Today","Yesterday", "3 days ago", "5 days ago", "1 week ago", "2 weeks ago", "3 weeks ago", "1 month ago"
            //,"1 day later", "2 days later", "3 days later", "5 days later", "1 week later", "2 weeks later", "3 weeks later", "1 month later",
            });
            comboBoxDateShortCut.SelectedItem = comboBoxDateShortCut.Items[0];

            //Limit
            comboBoxLimit.Items.Clear();
            comboBoxLimit.Items.AddRange(new string[] { "10", "50", "100", "200", "300", "400" });
            comboBoxLimit.SelectedItem = comboBoxLimit.Items[1];


            //Chart
            comboBoxChartType.Items.Clear();
            comboBoxChartType.Items.AddRange(Enum.GetNames(typeof(SeriesChartType)));
            comboBoxBackground.Items.Clear();
            comboBoxBackground.Items.AddRange(Enum.GetNames(typeof(KnownColor)));
            comboBoxSeries.Items.Clear();
            comboBoxSeries.Items.AddRange(Enum.GetNames(typeof(KnownColor)));


            comboBoxChartType.SelectedItem = comboBoxChartType.Items[13];
            comboBoxBackground.SelectedItem = comboBoxBackground.Items[comboBoxBackground.FindString("AliceBlue")];
            comboBoxSeries.SelectedItem = comboBoxSeries.Items[comboBoxSeries.FindString("Blue")];

            //Export
            comboBoxExportFileType.Items.Clear();
            comboBoxExportFileType.Items.AddRange(Enum.GetNames(typeof(ExportFileType)));
            comboBoxExportFilePath.Items.Clear();
            comboBoxExportFilePath.Text = Settings.Default.UserExportPath;

            //
            if (!String.IsNullOrEmpty(Settings.Default.UserLogoPath)) {
                pictureBoxCompanyLogo.Image = Image.FromFile(Settings.Default.UserLogoPath);
                pictureBoxCompanyLogo.Refresh();
            }

            textBoxCompanyName.Text = company.DisplayName;
            textBoxCompanyDateCreated.Text = user.DateTimeCreated.ToString();
            textBoxCompanyDateExpired.Text = user.DateTimeExpired.ToString();
            radioButtonSelectedTab.Checked = true;


            labelAtsInformation.Text =
            "is a full IT service provider, that has been helping businesses," +
            "organizations and individuals to achieve their potential.\n" +
            "We are offering the latest technology and innovative solutions. " +
            "Therefore, our clients are having the most efficient and professional" +
            "services with reasonable prices.";

            ToolTip toolTip = new ToolTip();
            toolTip.IsBalloon = true;
            toolTip.AutoPopDelay = 3;
            toolTip.SetToolTip(buttonSearch, "Search Data from Database");
        }

        #endregion

        #region Threads

        public void threadLoadFilters(object uncast) {
            try {

                this.BeginInvoke(new MethodInvoker(delegate() {

                    comboBoxCollection.Items.Clear();

                    User user = (User)comboBoxUser.SelectedItem;


                    foreach (Collection collection in user.Collections) {
                        comboBoxCollection.Items.Add(collection);
                    }

                    comboBoxCollection.DisplayMember = "Name";

                    comboBoxCollection.SelectedItem = comboBoxCollection.Items[0];


                    ContextMenuStrip contextMenuStripCheckedListBoxTrackers = new ContextMenuStrip();
                    ToolStripItem toolStripItemCheckedListBoxTrackersCheckAll = contextMenuStripCheckedListBoxTrackers.Items.Add("Check all");
                    toolStripItemCheckedListBoxTrackersCheckAll.Click += toolStripItemCheckedListBoxTrackersCheckAll_Click;
                    ToolStripItem toolStripItemCheckedListBoxTrackersUncheckAll = contextMenuStripCheckedListBoxTrackers.Items.Add("Uncheck all");
                    toolStripItemCheckedListBoxTrackersUncheckAll.Click += toolStripItemCheckedListBoxTrackersUncheckAll_Click;
                    ToolStripItem toolStripItemCheckedNextGroup = contextMenuStripCheckedListBoxTrackers.Items.Add("Check Next 10 items");
                    toolStripItemCheckedNextGroup.Click += toolStripItemCheckedNextGroup_Click;
                    ToolStripItem toolStripItemCheckedPrevGroup = contextMenuStripCheckedListBoxTrackers.Items.Add("Check Prev 10 items");
                    toolStripItemCheckedPrevGroup.Click += toolStripItemCheckedPrevGroup_Click;

                    ToolStripItem toolStripItemSortAscending = contextMenuStripCheckedListBoxTrackers.Items.Add("Sort/Unsort");
                    toolStripItemSortAscending.Click += toolStripItemSortAscending_Click;

                    checkedListBoxTrackers.ContextMenuStrip = contextMenuStripCheckedListBoxTrackers;
                    checkedListBoxTrackers.ItemCheck += checkedListBoxTrackers_ItemCheck;
                    checkedListBoxTrackers.ResumeLayout();


                }));
            } catch (QueryException queryException) {
                this.Invoke(new MethodInvoker(delegate() {
                    MessageBox.Show(this, queryException.Message, "Query Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
                Log log = new Log(LogFileType.TXT, LogType.EXCEPTION);
                string logData = DateTime.Now.ToString() + "\t\t queryException \t\t" + queryException.Message;
                log.write(logData);
            } finally {
                updateProgressBarStatus(100);
            }

        }

        private void toolStripItemSortAscending_Click(object sender, EventArgs e) {
            checkedListBoxTrackers.Sorted = !checkedListBoxTrackers.Sorted;
        }
        public void threadFunctionQuery(object uncast) {
            #region -instances

            DataTable dataTableDetails = new DataTable();
            int speedLimit;

            Hashtable hashTable = (Hashtable)uncast;
            Query query = new Query(database);

            Tracker tracker = (Tracker)hashTable["tracker"];
            if (tracker != null)
                speedLimit = tracker.SpeedLimit;

            TabControl tabControl = (TabControl)hashTable["tabControl"];


            DateTime dateTimeFrom = (DateTime)hashTable["dateTimeFrom"];
            DateTime dateTimeTo = (DateTime)hashTable["dateTimeTo"];
            List<Tracker> trackerList = (List<Tracker>)hashTable["trackerList"];


            int limit = (int)hashTable["limit"];
            int offset = (int)hashTable["offset"];

            int userId = (int)hashTable["comboBoxAccountId"];

            int dataCount = 0;

            string comboBoxReportTypeText = (string)hashTable["comboBoxReportTypeItem"];
            ReportType reportType = (ReportType)Enum.Parse(typeof(ReportType), comboBoxReportTypeText);

            bool tabPageExist = false;
            string tabPageName = "";
            string reportItemName = (string)hashTable["reportItemName"];
            string reportTypeName = Enum.GetName(typeof(ReportType), reportType);


            bool pagingStatus = false;
            #endregion
            #region -loop
            //If timeout we will query again up to 3 times
            for (int count = 0; count < 3; count++) {
                try {
                    #region checkTabPage
                    tabPageName = reportTypeName + " : " + reportItemName;
                    this.Invoke(new MethodInvoker(delegate {
                        foreach (TabPage tabPageItem in tabControl.TabPages) {
                            if (tabPageItem.Text == tabPageName) {
                                tabControl.SelectedTab = tabPageItem;
                                tabPageExist = true;
                                break;
                            }
                        }
                    }));

                    if (tabPageExist == true) {
                        break;
                    }
                    #endregion
                    #region querySelection
                    switch (reportType) {
                        case ReportType.HISTORICAL:
                            dataTableDetails = query.getTrackerHistoricalData(this.company, this.user, dateTimeFrom, dateTimeTo, limit, offset, tracker);
                            dataCount = query.getTrackerHistoricalDataCount(this.user, dateTimeFrom, dateTimeTo, tracker);
                            //dataTableDetails.writeToCsvFile(Directory.GetCurrentDirectory());
                            pagingStatus = true;
                            break;
                        case ReportType.IDLING:
                            dataTableDetails = query.getTrackerIdlingData(this.company, this.user, dateTimeFrom, dateTimeTo, 1000000, 0, tracker);
                            dataCount = dataTableDetails.Rows.Count;
                            break;
                        case ReportType.RUNNING:
                            dataTableDetails = query.getTrackerRunningData(this.company, this.user, dateTimeFrom, dateTimeTo, 1000000, 0, tracker);
                            dataCount = dataTableDetails.Rows.Count;
                            break;
                        case ReportType.GEOFENCE:
                            dataTableDetails = query.getTrackerGeofence(this.company, this.user, dateTimeFrom, dateTimeTo, 1000000, 0, tracker);
                            dataCount = dataTableDetails.Rows.Count;
                            break;
                        case ReportType.TRACKERS:
                            dataTableDetails = query.getTrackers(this.company);
                            dataCount = dataTableDetails.Rows.Count;
                            break;
                        case ReportType.ACC:
                            dataTableDetails = query.getTrackerAccData(this.company, this.user, dateTimeFrom, dateTimeTo, 1000000, 0, tracker);
                            dataCount = dataTableDetails.Rows.Count;
                            break;
                        case ReportType.EXTERNAL_POWER_CUT:
                            dataTableDetails = query.getTrackerExternalPowerCutData(this.company, this.user, dateTimeFrom, dateTimeTo, 1000000, 0, tracker);
                            dataCount = dataTableDetails.Rows.Count;
                            break;
                        case ReportType.OVERSPEED:
                            dataTableDetails = query.getTrackerOverSpeedData(this.company, this.user, dateTimeFrom, dateTimeTo, 1000000, 0, tracker);
                            dataCount = dataTableDetails.Rows.Count;
                            break;
                        case ReportType.TRACKERS_GEOFENCE:
                            dataTableDetails = query.getTrackersGeofence(this.company, this.user, dateTimeFrom, dateTimeTo, 1000000, 0, trackerList);
                            dataCount = dataTableDetails.Rows.Count;
                            break;
                        case ReportType.TRACKERS_HISTORICAL:
                            dataTableDetails = query.getTrackersHistorical(this.company, this.user, dateTimeFrom, dateTimeTo, 500, 0, trackerList);
                            dataCount = dataTableDetails.Rows.Count;
                            break;
                        case ReportType.ALL_COMPANIES:
                            //dataTableDetails = query.getAllCompanies();
                            dataCount = dataTableDetails.Rows.Count;
                            break;
                        case ReportType.ALL_TRACKERS:
                            //dataTableDetails = query.getAllTrackers();
                            dataCount = dataTableDetails.Rows.Count;
                            break;
                    }
                    #endregion
                    #region dataGridViewInformation
                    DataGridView dataGridView = new DataGridView();
                    dataGridView.Tag = dataTableDetails;
                    dataGridView.VirtualMode = true;
                    dataGridView.BackgroundColor = Color.White;
                    dataGridView.Name = reportTypeName;
                    dataGridView.RowHeadersVisible = false;
                    dataGridView.AllowUserToAddRows = false;
                    dataGridView.AllowUserToDeleteRows = false;
                    dataGridView.ReadOnly = true;

                    //DataView dataView = new DataView();
                    //dataView.Table = (DataTable)dataGridView.Tag;
                    //dataView.RowFilter = ("No > 0 AND No <= 100");

                    dataGridView.DataSource = dataTableDetails;
                    dataGridView.Dock = DockStyle.Fill;
                    dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    dataGridView.DataBindingComplete += dataGridView_DataBindingComplete;
                    dataGridView.CellDoubleClick += dataGridView_CellDoubleClick;
                    dataGridView.MouseHover += dataGridView_MouseHover;


                    ContextMenuStrip contextMenuStripDataGridView = new ContextMenuStrip();
                    contextMenuStripDataGridView.Name = "dataGridView";
                    contextMenuStripDataGridView.Opening += contextMenuStripDataGridView_Opening;


                    dataGridView.ContextMenuStrip = contextMenuStripDataGridView;
                    #endregion
                    #region tableLayoutPanelRight
                    ListViewItem[] listViewItemsDetail = new ListViewItem[11];
                    for (int index = 0; index < listViewItemsDetail.Count(); index++) {
                        listViewItemsDetail[index] = new ListViewItem();
                    }


                    listViewItemsDetail[0].Text = "Company Name";
                    listViewItemsDetail[0].SubItems.Add(company.DisplayName);

                    listViewItemsDetail[1].Text = "User Name";
                    listViewItemsDetail[1].SubItems.Add(user.Username);

                    if (tracker != null) {
                        listViewItemsDetail[2].Text = "Vehicle Reg";
                        listViewItemsDetail[2].SubItems.Add(tracker.VehicleRegistration);

                        listViewItemsDetail[3].Text = "Vehicle Model";
                        listViewItemsDetail[3].SubItems.Add(tracker.VehicleModel);

                        listViewItemsDetail[4].Text = "Owner Name";
                        listViewItemsDetail[4].SubItems.Add(tracker.OwnerName);

                        listViewItemsDetail[5].Text = "Driver Name";
                        listViewItemsDetail[5].SubItems.Add(tracker.DriverName);

                        listViewItemsDetail[6].Text = "Device Imei";
                        listViewItemsDetail[6].SubItems.Add(tracker.TrackerImei);

                        listViewItemsDetail[7].Text = "Sim Number";
                        listViewItemsDetail[7].SubItems.Add(tracker.SimNumber);

                        listViewItemsDetail[8].Text = "Vehicle Created";
                        listViewItemsDetail[8].SubItems.Add(tracker.DateTimeCreated.ToString("yyyy/MM/dd HH:mm:ss"));

                        listViewItemsDetail[9].Text = "Vehicle Expiry";
                        listViewItemsDetail[9].SubItems.Add(tracker.VehicleRegistrationExpiry.ToString("yyyy/MM/dd HH:mm:ss"));

                        listViewItemsDetail[10].Text = "Device Expiry";
                        listViewItemsDetail[10].SubItems.Add(tracker.DateTimeExpired.ToString("yyyy/MM/dd HH:mm:ss"));
                    }

                    ListView listViewDetails = new ListView();
                    listViewDetails.Dock = DockStyle.Fill;
                    listViewDetails.View = View.Details;
                    listViewDetails.Scrollable = false;
                    listViewDetails.FullRowSelect = true;

                    ColumnHeader columnHeaderLabelDetail = listViewDetails.Columns.Add("Details");
                    columnHeaderLabelDetail.Width = 90;

                    ColumnHeader columnHeaderValueDetail = listViewDetails.Columns.Add("Value");
                    columnHeaderValueDetail.Width = 150;


                    listViewDetails.Items.AddRange(listViewItemsDetail);


                    ListViewItem[] listViewItemsSummary = new ListViewItem[10];
                    for (int index = 0; index < listViewItemsSummary.Count(); index++) {
                        listViewItemsSummary[index] = new ListViewItem();
                    }


                    listViewItemsSummary[0].Text = "DateTime From";
                    listViewItemsSummary[0].SubItems.Add(dateTimeFrom.ToString("yyyy/MM/dd HH:mm:ss"));

                    listViewItemsSummary[1].Text = "DateTime To";
                    listViewItemsSummary[1].SubItems.Add(dateTimeTo.ToString("yyyy/MM/dd HH:mm:ss"));

                    if (reportType == ReportType.RUNNING ||
                        reportType == ReportType.IDLING ||
                        reportType == ReportType.ACC ||
                        reportType == ReportType.GEOFENCE ||
                        reportType == ReportType.EXTERNAL_POWER_CUT) {
                        listViewItemsSummary[2].Text = "Total Distance";
                        listViewItemsSummary[2].SubItems.Add(Converter.dataTableColumnSumValueIfTrue(dataTableDetails, "Distance").ToString() + " Km");
                        listViewItemsSummary[3].Text = "Total Fuel";
                        listViewItemsSummary[3].SubItems.Add(Converter.dataTableColumnSumValueIfTrue(dataTableDetails, "Fuel").ToString() + " L");
                        listViewItemsSummary[4].Text = "Total Cost";
                        listViewItemsSummary[4].SubItems.Add(Converter.dataTableColumnSumValueIfTrue(dataTableDetails, "Cost").ToString() + " Qr");
                    }
                    if (reportType == ReportType.TRACKERS_GEOFENCE) {
                        listViewItemsSummary[2].Text = "Total Distance";
                        listViewItemsSummary[2].SubItems.Add(Converter.dataTableColumnSumValue(dataTableDetails, "Distance").ToString() + " Km");
                        listViewItemsSummary[3].Text = "Total Fuel";
                        listViewItemsSummary[3].SubItems.Add(Converter.dataTableColumnSumValue(dataTableDetails, "Fuel").ToString() + " L");
                        listViewItemsSummary[4].Text = "Total Cost";
                        listViewItemsSummary[4].SubItems.Add(Converter.dataTableColumnSumValue(dataTableDetails, "Cost").ToString() + " Qr");
                    }

                    if (reportType == ReportType.RUNNING ||
                        reportType == ReportType.IDLING ||
                        reportType == ReportType.ACC ||
                        reportType == ReportType.GEOFENCE ||
                        reportType == ReportType.EXTERNAL_POWER_CUT) {

                        if (reportType == ReportType.RUNNING) {
                            listViewItemsSummary[5].Text = "Total Running Time";
                        } else if (reportType == ReportType.IDLING) {
                            listViewItemsSummary[5].Text = "Total Idling Time";
                        } else if (reportType == ReportType.GEOFENCE) {
                            listViewItemsSummary[5].Text = "Total Geofence Active Time";
                        } else if (reportType == ReportType.ACC) {
                            listViewItemsSummary[5].Text = "Total ACC Active Time";
                        } else if (reportType == ReportType.EXTERNAL_POWER_CUT) {
                            listViewItemsSummary[5].Text = "Total ExternalPower Cut Time";
                        }
                        listViewItemsSummary[5].SubItems.Add(Converter.dataTableColumnSumTimeSpanIfTrue(dataTableDetails, "Time").ToString(@"dd\.hh\:mm\:ss"));
                    }

                    ListView listViewSummary = new ListView();
                    listViewSummary.Dock = DockStyle.Fill;
                    listViewSummary.View = View.Details;
                    listViewSummary.Scrollable = false;
                    listViewSummary.FullRowSelect = true;

                    ColumnHeader columnHeaderLabelSummary = listViewSummary.Columns.Add("Summary");
                    columnHeaderLabelSummary.Width = 90;

                    ColumnHeader columnHeaderValueSummary = listViewSummary.Columns.Add("Value");
                    columnHeaderValueSummary.Width = 150;

                    listViewSummary.Items.AddRange(listViewItemsSummary);


                    TableLayoutPanel tableLayoutPanelRight = new TableLayoutPanel();
                    tableLayoutPanelRight.Dock = DockStyle.Fill;
                    tableLayoutPanelRight.RowStyles.Clear();
                    tableLayoutPanelRight.ColumnStyles.Clear();
                    tableLayoutPanelRight.Margin = new Padding(0, 0, 0, 0);
                    tableLayoutPanelRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                    tableLayoutPanelRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 220));
                    tableLayoutPanelRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 220));
                    tableLayoutPanelRight.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

                    tableLayoutPanelRight.Controls.Add(listViewDetails, 0, 0);
                    tableLayoutPanelRight.Controls.Add(listViewSummary, 0, 1);
                    #endregion
                    #region tableLayoutPanelBottom
                    Button buttonNext = new Button();
                    buttonNext.Dock = DockStyle.Fill;
                    //buttonNext.Text = "Next";
                    buttonNext.Click += buttonNext_Click;
                    buttonNext.Visible = pagingStatus;
                    buttonNext.BackgroundImage = Resources.icon_stepr_001;
                    buttonNext.BackgroundImageLayout = ImageLayout.Zoom;

                    Button buttonPrev = new Button();
                    buttonPrev.Dock = DockStyle.Fill;
                    //buttonPrev.Text = "Prev";
                    buttonPrev.Click += buttonPrev_Click;
                    buttonPrev.Visible = pagingStatus;
                    buttonPrev.BackgroundImage = Resources.icon_stepl_001;
                    buttonPrev.BackgroundImageLayout = ImageLayout.Zoom;

                    Button buttonFirst = new Button();
                    buttonFirst.Dock = DockStyle.Fill;
                    //buttonFirst.Text = "First";
                    buttonFirst.Click += buttonFirst_Click;
                    buttonFirst.Visible = pagingStatus;
                    buttonFirst.BackgroundImage = Resources.icon_stepll_001;
                    buttonFirst.BackgroundImageLayout = ImageLayout.Zoom;

                    Button buttonLast = new Button();
                    buttonLast.Dock = DockStyle.Fill;
                    //buttonLast.Text = "Last";
                    buttonLast.Click += buttonLast_Click;
                    buttonLast.Visible = pagingStatus;
                    buttonLast.BackgroundImage = Resources.icon_steprl_001;
                    buttonLast.BackgroundImageLayout = ImageLayout.Zoom;

                    Label LabelPageNumber = new Label();
                    LabelPageNumber.Dock = DockStyle.Fill;
                    LabelPageNumber.Text = "1";
                    LabelPageNumber.TextAlign = ContentAlignment.MiddleRight;
                    LabelPageNumber.Visible = pagingStatus;

                    Label LabelTotalPageNumber = new Label();
                    LabelTotalPageNumber.Dock = DockStyle.Fill;
                    int totalPageNumber = (dataCount / (int)hashTable["limit"]) + 1;
                    LabelTotalPageNumber.Text = "/    " + totalPageNumber.ToString() + " Page(s)";
                    LabelTotalPageNumber.TextAlign = ContentAlignment.MiddleLeft;
                    LabelTotalPageNumber.Visible = pagingStatus;

                    Label LabelDataCount = new Label();
                    LabelDataCount.Dock = DockStyle.Fill;
                    LabelDataCount.Text = dataCount.ToString();
                    LabelDataCount.TextAlign = ContentAlignment.MiddleRight;

                    Label labelDetails = new Label();
                    labelDetails.Dock = DockStyle.Fill;
                    labelDetails.TextAlign = ContentAlignment.MiddleLeft;
                    labelDetails.Text = "Record(s) found";


                    TableLayoutPanel tableLayoutPanelBottom = new TableLayoutPanel();
                    tableLayoutPanelBottom.Dock = DockStyle.Fill;
                    tableLayoutPanelBottom.RowStyles.Clear();
                    tableLayoutPanelBottom.ColumnStyles.Clear();
                    tableLayoutPanelBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50));
                    tableLayoutPanelBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50));
                    tableLayoutPanelBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50));
                    tableLayoutPanelBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50));
                    tableLayoutPanelBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50));
                    tableLayoutPanelBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
                    tableLayoutPanelBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                    tableLayoutPanelBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));

                    tableLayoutPanelBottom.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                    tableLayoutPanelBottom.Controls.Add(buttonFirst, 0, 0);
                    tableLayoutPanelBottom.Controls.Add(buttonPrev, 1, 0);
                    tableLayoutPanelBottom.Controls.Add(buttonNext, 2, 0);
                    tableLayoutPanelBottom.Controls.Add(buttonLast, 3, 0);
                    tableLayoutPanelBottom.Controls.Add(LabelPageNumber, 4, 0);
                    tableLayoutPanelBottom.Controls.Add(LabelTotalPageNumber, 5, 0);
                    tableLayoutPanelBottom.Controls.Add(LabelDataCount, 6, 0);
                    tableLayoutPanelBottom.Controls.Add(labelDetails, 7, 0);
                    #endregion
                    #region tableLayoutInformationParent
                    TableLayoutPanel tableLayoutInformationParent = new TableLayoutPanel();
                    tableLayoutInformationParent.RowStyles.Clear();
                    tableLayoutInformationParent.ColumnStyles.Clear();
                    tableLayoutInformationParent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                    tableLayoutInformationParent.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250));
                    tableLayoutInformationParent.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                    tableLayoutInformationParent.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
                    tableLayoutInformationParent.Controls.Add(dataGridView, 0, 0);
                    tableLayoutInformationParent.Controls.Add(tableLayoutPanelBottom, 0, 1);
                    tableLayoutInformationParent.Controls.Add(tableLayoutPanelRight, 1, 0);
                    //tableLayoutInformationParent.Controls.Add(labelDetails, 1, 1);
                    tableLayoutInformationParent.Dock = DockStyle.Fill;
                    #endregion
                    #region tabPageAndTabControls
                    TabPage tabPage = new TabPage(tabPageName);
                    tabPage.Name = reportItemName;


                    tabPage.Tag = hashTable;
                    tabPage.Controls.Add(tableLayoutInformationParent);

                    hashTable["dataGridViewInformation"] = dataGridView;

                    if (tabControl != null) {
                        this.Invoke(new MethodInvoker(delegate {
                            tabControl.TabPages.Add(tabPage);
                            tabControl.SelectTab(tabPage);
                            tabControl.Click += tabControl_Click;
                            tabControl.Refresh();
                            Application.DoEvents();
                        }));
                    }
                    #endregion
                    break;
                } catch (QueryException queryException) {
                    Log log = new Log(LogFileType.TXT, LogType.EXCEPTION);
                    string logData = "";
                    if (tracker != null)
                        logData = DateTime.Now.ToString() + "\t\t queryException \t\t" + tracker.VehicleRegistration + " : " + queryException.Message;
                    else
                        logData = DateTime.Now.ToString() + "\t\t queryException \t\t" + queryException.Message;

                    log.write(logData);
                    break;
                } catch (MySqlException mySqlException) {
                    Log log = new Log(LogFileType.TXT, LogType.EXCEPTION);
                    string logData = "";
                    if (tracker != null)
                        logData = DateTime.Now.ToString() + "\t\t mySqlException \t\t" + tracker.VehicleRegistration + " : " + mySqlException.Message;
                    else
                        logData = DateTime.Now.ToString() + "\t\t mySqlException \t\t" + mySqlException.Message;
                    log.write(logData);
                    if (mySqlException.ErrorCode == -2147467259) {
                        //requery
                        if (count == 2) {
                            break;
                        }
                    } else {
                        break;
                    }
                } catch (Exception exception) {
                    Log log = new Log(LogFileType.TXT, LogType.EXCEPTION);
                    string logData = "";
                    if (tracker != null) {
                        logData = DateTime.Now.ToString() + "\t\t exception \t\t" + tracker.VehicleRegistration + " : " + exception.Message;
                    } else {
                        logData = DateTime.Now.ToString() + "\t\t exception \t\t" + exception.Message;
                    }
                    log.write(logData);
                    break;
                }
            }
            #endregion
            #region -updateProgressBarStatus
            lock (this.progressBarStatus) {
                workerThreadFinished++;
                double result = workerThreadFinished / workerThreadCount;
                double progressValue = result * (double)progressBarStatus.Maximum;
                updateProgressBarStatus((int)progressValue);
            }
            #endregion
        }



        void tabControl_Click(object sender, EventArgs e) {
            TabPage tabPage = (TabPage)tabControl.SelectedTab;
            for (int index = 0; index < checkedListBoxTrackers.Items.Count; index++) {
                Tracker tracker = (Tracker)checkedListBoxTrackers.Items[index];
                string trackerItem = (string)checkedListBoxTrackers.GetItemText(tracker);
                string[] trackerName = tabPage.Text.Split(' ');
                if (trackerName[2] == trackerItem) {
                    checkedListBoxTrackers.SetSelected(index, true);
                    break;
                }

            }

        }


        public void threadShortQuery(object uncast) {
            #region -instances
            DataTable dataTableDetails = new DataTable();
            Hashtable hashTable = (Hashtable)uncast;
            Query query = new Query(database);

            Tracker tracker = (Tracker)hashTable["tracker"];

            DateTime dateTimeFrom = (DateTime)hashTable["dateTimeFrom"];
            DateTime dateTimeTo = (DateTime)hashTable["dateTimeTo"];

            DataGridView dataGridViewInformation = (DataGridView)hashTable["dataGridViewInformation"];


            string comboBoxReportTypeText = (string)hashTable["comboBoxReportTypeItem"];
            ReportType reportType = (ReportType)Enum.Parse(typeof(ReportType), comboBoxReportTypeText);

            int queryLimit = ((int)hashTable["limit"]);
            int offset = (int)hashTable["offset"];
            #endregion
            #region -loop

            try {
                #region querySelection
                switch (reportType) {
                    case ReportType.HISTORICAL:
                        dataTableDetails = query.getTrackerHistoricalData(this.company, this.user, dateTimeFrom, dateTimeTo, queryLimit, offset, tracker);
                        break;
                    case ReportType.IDLING:
                        dataTableDetails = query.getTrackerIdlingData(this.company, this.user, dateTimeFrom, dateTimeTo, queryLimit, offset, tracker);
                        break;
                    case ReportType.RUNNING:
                        dataTableDetails = query.getTrackerRunningData(this.company, this.user, dateTimeFrom, dateTimeTo, queryLimit, offset, tracker);
                        break;
                    case ReportType.GEOFENCE:
                        dataTableDetails = query.getTrackerGeofence(this.company, this.user, dateTimeFrom, dateTimeTo, queryLimit, offset, tracker);
                        break;
                    case ReportType.TRACKERS:
                        //dataTableDetails = query.getTrackers(this.account, userId);
                        break;
                    case ReportType.ALL_COMPANIES:
                        //dataTableDetails = query.getAllCompanies();
                        break;
                    case ReportType.ALL_TRACKERS:
                        //dataTableDetails = query.getAllTrackers();
                        break;

                }
                #endregion
                #region updateDataGridView
                this.Invoke(new MethodInvoker(delegate {
                    dataGridViewInformation.DataSource = dataTableDetails;
                }));
                #endregion
            } catch (QueryException queryException) {
                Log log = new Log(LogFileType.TXT, LogType.EXCEPTION);
                string logData = DateTime.Now.ToString() + "\t\t queryException \t\t" + tracker.VehicleRegistration + " : " + queryException.Message;
                log.write(logData);

            } catch (MySqlException mySqlException) {
                Log log = new Log(LogFileType.TXT, LogType.EXCEPTION);
                string logData = DateTime.Now.ToString() + "\t\t mySqlException \t\t" + tracker.VehicleRegistration + " : " + mySqlException.Message;
                log.write(logData);
                if (mySqlException.ErrorCode == -2147467259) {

                } else {

                }
            } catch (Exception exception) {
                Log log = new Log(LogFileType.TXT, LogType.EXCEPTION);
                string logData = DateTime.Now.ToString() + "\t\t exception \t\t" + tracker.VehicleRegistration + " : " + exception.Message;
                log.write(logData);

            }

            #endregion
            updateProgressBarStatus(100);
        }


        #region pagination
        void buttonFirst_Click(object sender, EventArgs e) {
            updateProgressBarStatus(0);
            Button button = (Button)sender;
            TableLayoutPanel TableLayoutPanelBottom = (TableLayoutPanel)button.Parent;
            Label labelPageNumber = (Label)TableLayoutPanelBottom.Controls[4];
            Label labelDataCount = (Label)TableLayoutPanelBottom.Controls[5];
            TableLayoutPanel TableLayoutPanelMain = (TableLayoutPanel)TableLayoutPanelBottom.Parent;
            DataGridView dataGridViewInformation = (DataGridView)TableLayoutPanelMain.Controls[0];
            TabPage tabPage = (TabPage)TableLayoutPanelMain.Parent;
            Hashtable hashTable = (Hashtable)tabPage.Tag;


            hashTable["offset"] = 0;

            ThreadPool.QueueUserWorkItem(new WaitCallback(threadShortQuery), hashTable);


            labelPageNumber.Text = "1";

        }
        void buttonLast_Click(object sender, EventArgs e) {
            updateProgressBarStatus(0);
            Button button = (Button)sender;
            TableLayoutPanel TableLayoutPanelBottom = (TableLayoutPanel)button.Parent;
            Label labelPageNumber = (Label)TableLayoutPanelBottom.Controls[4];
            Label labelTotalPageNumber = (Label)TableLayoutPanelBottom.Controls[5];
            TableLayoutPanel TableLayoutPanelMain = (TableLayoutPanel)TableLayoutPanelBottom.Parent;
            DataGridView dataGridViewInformation = (DataGridView)TableLayoutPanelMain.Controls[0];
            TabPage tabPage = (TabPage)TableLayoutPanelMain.Parent;
            Hashtable hashTable = (Hashtable)tabPage.Tag;

            string[] data = labelTotalPageNumber.Text.Split(' ');

            hashTable["offset"] = ((int.Parse(data[4]) - 1) * (int)hashTable["limit"]);
            labelPageNumber.Text = data[4];

            ThreadPool.QueueUserWorkItem(new WaitCallback(threadShortQuery), hashTable);

        }
        void buttonPrev_Click(object sender, EventArgs e) {
            updateProgressBarStatus(0);
            Button button = (Button)sender;
            TableLayoutPanel TableLayoutPanelBottom = (TableLayoutPanel)button.Parent;
            Label labelPageNumber = (Label)TableLayoutPanelBottom.Controls[4];
            Label labelDataCount = (Label)TableLayoutPanelBottom.Controls[6];
            TableLayoutPanel TableLayoutPanelMain = (TableLayoutPanel)TableLayoutPanelBottom.Parent;
            DataGridView dataGridViewInformation = (DataGridView)TableLayoutPanelMain.Controls[0];
            TabPage tabPage = (TabPage)TableLayoutPanelMain.Parent;
            Hashtable hashTable = (Hashtable)tabPage.Tag;

            int dataCount = int.Parse(labelDataCount.Text);
            int pageCount = (int.Parse(labelPageNumber.Text) - 1);

            if (pageCount <= 0)
                return;

            int limit = (int)hashTable["limit"];
            int noFrom = (int.Parse(labelPageNumber.Text) - 2) * limit;

            hashTable["offset"] = noFrom;

            ThreadPool.QueueUserWorkItem(new WaitCallback(threadShortQuery), hashTable);

            int pageNumber = int.Parse(labelPageNumber.Text) - 1;
            labelPageNumber.Text = pageNumber.ToString();

        }
        void buttonNext_Click(object sender, EventArgs e) {
            updateProgressBarStatus(0);
            Button button = (Button)sender;
            TableLayoutPanel TableLayoutPanelBottom = (TableLayoutPanel)button.Parent;
            Label labelPageNumber = (Label)TableLayoutPanelBottom.Controls[4];
            Label labelDataCount = (Label)TableLayoutPanelBottom.Controls[6];
            TableLayoutPanel TableLayoutPanelMain = (TableLayoutPanel)TableLayoutPanelBottom.Parent;
            DataGridView dataGridViewInformation = (DataGridView)TableLayoutPanelMain.Controls[0];
            TabPage tabPage = (TabPage)TableLayoutPanelMain.Parent;
            Hashtable hashTable = (Hashtable)tabPage.Tag;

            int dataCount = int.Parse(labelDataCount.Text);
            int pageCount = (int.Parse(labelPageNumber.Text) + 1) * (int)hashTable["limit"];

            if ((dataCount + (int)hashTable["limit"]) > pageCount) {


                int limit = (int)hashTable["limit"];
                int noFrom = int.Parse(labelPageNumber.Text) * limit;

                hashTable["offset"] = noFrom;

                ThreadPool.QueueUserWorkItem(new WaitCallback(threadShortQuery), hashTable);
                int pageNumber = int.Parse(labelPageNumber.Text) + 1;
                labelPageNumber.Text = pageNumber.ToString();
            }
        }
        #endregion
        #endregion
        #region Events

        #region dataGridViewData
        void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            DataGridView dataGridView = (DataGridView)sender;
            if (dataGridView == null) {
                return;
            }

            TableLayoutPanel tableLayoutPanel = (TableLayoutPanel)dataGridView.Parent;
            TabPage tabPage = (TabPage)tableLayoutPanel.Parent;

            Hashtable hashtableInformation = new Hashtable();
            List<string> keys = new List<string>();

            ReportType reportType = (ReportType)Enum.Parse(typeof(ReportType), dataGridView.Name, true);

            hashtableInformation.Add("ReportType", reportType);
            keys.Add("ReportType");

            foreach (DataGridViewColumn dataGridViewColumn in dataGridView.Columns) {
                DataGridViewRow dataGridViewRow = dataGridView.CurrentRow;
                hashtableInformation.Add(dataGridViewColumn.Name, dataGridViewRow.Cells[dataGridViewColumn.Name].Value);
                keys.Add(dataGridViewColumn.Name);
            }
            hashtableInformation.Add("Keys", keys);

            FormCellInformation formCellInformation = new FormCellInformation(tabPage.Text, hashtableInformation);
            formCellInformation.Show();
        }
        void dataGridView_MouseHover(object sender, EventArgs e) {
            DataGridView dataGridView = (DataGridView)sender;
            if (dataGridView == null) {
                return;
            }

            GC.Collect();
            //dataGridView.Refresh();
        }
        void dataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e) {
            DataGridView dataGridViewInformation = (DataGridView)sender;
            if (dataGridViewInformation == null) {
                return;
            }

            GC.Collect();

            dataGridViewInformation.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
            dataGridViewInformation.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewInformation.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView.ColumnHeadersDefaultCellStyle.Font, FontStyle.Bold);
            dataGridViewInformation.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridViewInformation.ColumnHeadersDefaultCellStyle.Font.FontFamily, 9.0f);

            dataGridViewInformation.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            ReportType reportType = (ReportType)Enum.Parse(typeof(ReportType), dataGridViewInformation.Name, true);


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


            foreach (DataGridViewRow dataGridViewRow in dataGridViewInformation.Rows) {
                if ((dataGridViewRow.Index % 2) == 0) {
                    dataGridViewRow.DefaultCellStyle.BackColor = Color.AliceBlue;//Color.Lavender;
                }
            }

            foreach (DataGridViewColumn dataGridViewColumn in dataGridViewInformation.Columns) {
                if (dataGridViewColumn.ValueType == typeof(DateTime)) {
                    DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
                    dataGridViewCellStyle.Format = ("yyyy/MM/dd HH:mm:ss");
                    dataGridViewColumn.DefaultCellStyle = dataGridViewCellStyle;
                }
                if (dataGridViewColumn.ValueType == typeof(TimeSpan)) {
                    DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
                    dataGridViewCellStyle.Format = (@"dd\.hh\:mm\:ss");
                    dataGridViewColumn.DefaultCellStyle = dataGridViewCellStyle;
                }
                if ((dataGridViewColumn.Name != "Latitude" &&
                    dataGridViewColumn.Name != "Longitude" &&
                    dataGridViewColumn.Name != "LatitudeTo" &&
                    dataGridViewColumn.Name != "LongitudeTo" &&
                    dataGridViewColumn.Name != "LatitudeFrom" &&
                    dataGridViewColumn.Name != "LongitudeFrom") && dataGridViewColumn.ValueType == typeof(double)) {
                    DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
                    dataGridViewCellStyle.Format = ("0.00");
                    dataGridViewCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dataGridViewColumn.DefaultCellStyle = dataGridViewCellStyle;
                }
                if (dataGridViewColumn.ValueType == typeof(int)) {
                    DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
                    dataGridViewCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dataGridViewColumn.DefaultCellStyle = dataGridViewCellStyle;
                }
                dataGridViewColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

        }

        void contextMenuStripDataGridView_Opening(object sender, CancelEventArgs e) {
            ContextMenuStrip contextMenuStrip = (ContextMenuStrip)sender;
            contextMenuStrip.Items.Clear();
            DataGridView dataGridView = (DataGridView)contextMenuStrip.SourceControl;
            foreach (DataGridViewColumn dataGridViewColumn in dataGridView.Columns) {
                ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
                toolStripMenuItem.Name = dataGridViewColumn.Name;
                toolStripMenuItem.Text = dataGridViewColumn.Name;
                toolStripMenuItem.Checked = dataGridViewColumn.Visible;
                toolStripMenuItem.Click += toolStripMenuItem_Click;
                contextMenuStrip.Items.Add(toolStripMenuItem);
            }
        }

        private void toolStripMenuItem_Click(object sender, EventArgs e) {
            ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)sender;
            ContextMenuStrip contextMenuStrip = (ContextMenuStrip)toolStripMenuItem.Owner;
            if (contextMenuStrip.Name == "dataGridView") {
                toolStripMenuItem.Checked = !toolStripMenuItem.Checked;
            }

            DataGridView dataGridView = (DataGridView)contextMenuStrip.SourceControl;
            foreach (ToolStripMenuItem toolStripMenuItemNewState in contextMenuStrip.Items) {
                foreach (DataGridViewColumn dataGridViewColumn in dataGridView.Columns) {
                    if (toolStripMenuItemNewState.Name == dataGridViewColumn.Name) {
                        dataGridViewColumn.Visible = toolStripMenuItemNewState.Checked;
                    }
                }
            }
        }


        #endregion
        #region checkedListBoxTrackers
        private void checkedListBoxTrackers_Click(object sender, EventArgs e) {
            if (tabControl != null) {
                string stringSelectedItem = comboBoxReportType.Text + " : " + checkedListBoxTrackers.GetItemText(checkedListBoxTrackers.SelectedItem);

                foreach (TabPage tabPage in tabControl.TabPages) {
                    if (tabPage.Text == stringSelectedItem) {
                        tabControl.SelectTab(tabPage);
                    }
                }
            }

        }
        void toolStripItemCheckedPrevGroup_Click(object sender, EventArgs e) {
            int itemsSelected = 10;
            int checkedListBoxTrackersItemsLength = checkedListBoxTrackers.Items.Count;
            if (checkedListBoxTrackersItemsLength > itemsSelected) {
                int firstIndex = checkedListBoxTrackersItemsLength;
                foreach (int index in checkedListBoxTrackers.CheckedIndices)
                    if (index < firstIndex) {
                        firstIndex = index;
                    }


                if (((checkedListBoxTrackersItemsLength + 1) > firstIndex) && (firstIndex != 0)) {
                    firstIndex--;
                }

                toolStripItemCheckedListBoxTrackersUncheckAll_Click(this, new EventArgs());

                int difference = (firstIndex - 1);


                if (difference >= itemsSelected) {
                    int totalItemBefore = firstIndex - itemsSelected;
                    for (; firstIndex > totalItemBefore; firstIndex--)
                        checkedListBoxTrackers.SetItemChecked(firstIndex, true);

                } else if (difference < itemsSelected && difference > -1) {
                    for (; firstIndex > -1; firstIndex--)
                        checkedListBoxTrackers.SetItemChecked(firstIndex, true);
                } else if (difference == -1) {
                    foreach (int index in checkedListBoxTrackers.CheckedIndices)
                        checkedListBoxTrackers.SetItemChecked(index, false);
                }
            }
        }
        void toolStripItemCheckedNextGroup_Click(object sender, EventArgs e) {
            int itemsSelected = 10;
            int checkedListBoxTrackersItemsLength = checkedListBoxTrackers.Items.Count;
            if (checkedListBoxTrackersItemsLength > itemsSelected) {
                int lastIndex = 0;
                foreach (int index in checkedListBoxTrackers.CheckedIndices)
                    if (lastIndex < index) {
                        lastIndex = index;
                    }
                if ((checkedListBoxTrackersItemsLength - 1) > lastIndex && lastIndex != 0) {
                    lastIndex++;
                }

                toolStripItemCheckedListBoxTrackersUncheckAll_Click(this, new EventArgs());

                int difference = checkedListBoxTrackersItemsLength - lastIndex;
                if (difference >= itemsSelected) {
                    int totalItemAfter = lastIndex + itemsSelected;
                    for (; lastIndex < totalItemAfter; lastIndex++)
                        checkedListBoxTrackers.SetItemChecked(lastIndex, true);

                } else if (difference < itemsSelected && difference > 1) {
                    for (; lastIndex < checkedListBoxTrackers.Items.Count; lastIndex++)
                        checkedListBoxTrackers.SetItemChecked(lastIndex, true);
                } else if (difference == 1) {
                    foreach (int index in checkedListBoxTrackers.CheckedIndices)
                        checkedListBoxTrackers.SetItemChecked(index, false);
                }
            }
        }

        void toolStripItemCheckedListBoxTrackersUncheckAll_Click(object sender, EventArgs e) {

            for (int index = 0; index < checkedListBoxTrackers.Items.Count; index++) {
                checkedListBoxTrackers.SetItemCheckState(index, CheckState.Unchecked);
            }

        }

        void toolStripItemCheckedListBoxTrackersCheckAll_Click(object sender, EventArgs e) {

            for (int index = 0; index < checkedListBoxTrackers.Items.Count; index++) {
                checkedListBoxTrackers.SetItemCheckState(index, CheckState.Checked);
            }

        }



        void checkedListBoxTrackers_ItemCheck(object sender, ItemCheckEventArgs e) {
            int count = this.checkedListBoxTrackers.CheckedItems.Count;
            if (e.CurrentValue != CheckState.Checked && e.NewValue == CheckState.Checked) {
                count += 1;
            } else if (e.CurrentValue == CheckState.Checked && e.NewValue != CheckState.Checked) {
                count -= 1;
            }
            labelTotalCheckedTrackers.Text = "Checked : " + count.ToString();
        }
        #endregion
        #region tabControlData
        void toolStripItemCloseThisTab_Click(object sender, EventArgs e) {
            if (tabControl.TabPages.Count > 0) {
                if (tabControl.TabPages.Count == 1) {
                    panelData.Controls.Remove(tabControl);
                    tabControl.Dispose();
                    tabControl = null;
                } else if (tabControl.TabPages.Count > 1) {
                    tabControl.TabPages.RemoveAt(tabControl.SelectedIndex);
                }
            }
        }

        void toolStripItemCloseOtherTabs_Click(object sender, EventArgs e) {
            if (tabControl.TabPages.Count > 0) {
                if (tabControl.TabPages.Count > 0) {
                    foreach (TabPage tabPage in tabControl.TabPages) {
                        if (tabPage != tabControl.SelectedTab)
                            tabControl.TabPages.Remove(tabPage);
                    }
                }
            }
        }

        void toolStripItemCloseAllTabs_Click(object sender, EventArgs e) {

            if (tabControl.TabPages.Count > 0) {
                panelData.Controls.Remove(tabControl);
                tabControl.Dispose();
                tabControl = null;
            }

        }

        void tabControl_Disposed(object sender, EventArgs e) {
            updateProgressBarStatus(100);
        }
        #endregion


        #region buttonSearch
        private void buttonSearch_Click(object sender, EventArgs e) {
            /*
             * Start Validate Inputs
             * 
             */
            ReportType reportType = (ReportType)Enum.Parse(typeof(ReportType), comboBoxReportType.Text);


            TimeSpan timeSpan = dateTimePickerDateTo.Value.Subtract(dateTimePickerDateFrom.Value);
            if (reportType == ReportType.TRACKERS_HISTORICAL) {
                if (timeSpan.Hours > 1 || timeSpan.Days > 0) {
                    MessageBox.Show(this, "For TRACKERS_HISTORICAL Report, \"DateTime From\" should be maximum of 1 hour difference from the \"Date To\".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            } else {
                if (timeSpan.Days > 31) {
                    MessageBox.Show(this, "For this Report, \"Date From\" should be maximum of one month difference from the \"Date To\".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (reportType == ReportType.TRACKERS || reportType == ReportType.ALL_COMPANIES || reportType == ReportType.ALL_TRACKERS) {

            } else {
                CheckedListBox.CheckedItemCollection checkedItemCollection = checkedListBoxTrackers.CheckedItems;
                if (checkedItemCollection.Count <= 0) {
                    MessageBox.Show(this, "Check the item(s) to be search.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (tabControl == null) {
                tabControl = new TabControl();
                labelTotalTabPages.Text = "TabPages : 0";
                ContextMenuStrip contextMenuStripTabControl = new ContextMenuStrip();
                ToolStripItem toolStripItemCloseThisTab = contextMenuStripTabControl.Items.Add("Close this tab", null);
                ToolStripItem toolStripItemCloseOtherTabs = contextMenuStripTabControl.Items.Add("Close other tabs", null);
                ToolStripItem toolStripItemCloseAllTabs = contextMenuStripTabControl.Items.Add("Close all tabs", null);
                toolStripItemCloseThisTab.Click += toolStripItemCloseThisTab_Click;
                toolStripItemCloseOtherTabs.Click += toolStripItemCloseOtherTabs_Click;
                toolStripItemCloseAllTabs.Click += toolStripItemCloseAllTabs_Click;

                tabControl.ContextMenuStrip = contextMenuStripTabControl;
                tabControl.Dock = DockStyle.Fill;

                tabControl.Disposed += tabControl_Disposed;
                panelData.Controls.Add(tabControl);
            } else {
                panelData.Controls.Add(tabControl);
            }

            workerThreadCount = 0;
            workerThreadFinished = 0;
            progressBarStatus.Value = 1;
            labelTotalTabPages.Text = "TabPages : 0";

            if (reportType == ReportType.TRACKERS || reportType == ReportType.ALL_COMPANIES || reportType == ReportType.ALL_TRACKERS || reportType == ReportType.TRACKERS_GEOFENCE || reportType == ReportType.TRACKERS_HISTORICAL) {
                workerThreadCount = 0;
                workerThreadFinished = 0;
                progressBarStatus.Value = 10;
                Hashtable hashTable = new Hashtable();


                List<Tracker> trackerList = new List<Tracker>();

                foreach (Tracker tracker in checkedListBoxTrackers.CheckedItems) {
                    trackerList.Add(tracker);
                }

                hashTable.Add("reportItemName", comboBoxUser.Text);
                hashTable.Add("trackerList", trackerList);
                hashTable.Add("tabControl", tabControl);
                hashTable.Add("dateTimeFrom", dateTimePickerDateFrom.Value);
                hashTable.Add("dateTimeTo", dateTimePickerDateTo.Value);
                hashTable.Add("comboBoxReportTypeItem", comboBoxReportType.Text);
                hashTable.Add("limit", int.Parse(comboBoxLimit.Text));
                hashTable.Add("dataGridViewInformation", null);
                hashTable.Add("offset", 0);

                int userId = (int)comboBoxUser.SelectedValue;
                hashTable.Add("comboBoxAccountId", userId);
                ThreadPool.QueueUserWorkItem(new WaitCallback(threadFunctionQuery), hashTable);
                workerThreadCount++;

            } else {
                foreach (Tracker tracker in checkedListBoxTrackers.CheckedItems) {
                    Hashtable hashTable = new Hashtable();
                    hashTable.Add("tracker", tracker);
                    hashTable.Add("reportItemName", checkedListBoxTrackers.GetItemText(tracker));
                    hashTable.Add("checkedListBoxTrackers", checkedListBoxTrackers);
                    hashTable.Add("tabControl", tabControl);
                    hashTable.Add("dateTimeFrom", dateTimePickerDateFrom.Value);
                    hashTable.Add("dateTimeTo", dateTimePickerDateTo.Value);
                    hashTable.Add("comboBoxReportTypeItem", comboBoxReportType.Text);
                    hashTable.Add("limit", int.Parse(comboBoxLimit.Text));
                    hashTable.Add("dataGridViewInformation", null);
                    hashTable.Add("offset", 0);
                    int userId = (int)comboBoxUser.SelectedValue;
                    hashTable.Add("comboBoxAccountId", userId);

                    ThreadPool.QueueUserWorkItem(new WaitCallback(threadFunctionQuery), hashTable);
                    workerThreadCount++;
                }
            }
        }
        #endregion

        #region buttonFilter

        private void buttonFilter_Click(object sender, EventArgs e) {
            if (tabControl == null)
                return;

            if (tabControl.TabPages.Count == 0)
                return;

            TabPage tabPage = tabControl.SelectedTab;
            TableLayoutPanel TableLayoutInformation = (TableLayoutPanel)tabPage.Controls[0];
            DataGridView dataGridViewInformation = (DataGridView)TableLayoutInformation.Controls[0];




            try {
                if (comboBoxFilterContains.Enabled == true) {
                    string rowFilter = "";
                    if (!String.IsNullOrEmpty(comboBoxFilterContains.Text)) {
                        rowFilter = string.Format("[{0}] = '{1}'", comboBoxColumn.SelectedItem.ToString(), comboBoxFilterContains.Text);
                    }
                    (dataGridViewInformation.DataSource as DataTable).DefaultView.RowFilter = rowFilter;
                } else if (comboBoxFilterValueFrom.Enabled == true && comboBoxFilterValueTo.Enabled == true) {

                    double valueFrom = double.Parse(comboBoxFilterValueFrom.Text);
                    double valueTo = double.Parse(comboBoxFilterValueTo.Text);
                    string rowFilter = "";

                    if (valueFrom < valueTo) {
                        rowFilter = comboBoxColumn.Text + " >= " + valueFrom.ToString() + " and " + comboBoxColumn.Text + " <= " + valueTo.ToString();
                    } else if (valueFrom > valueTo) {
                        rowFilter = comboBoxColumn.Text + " <= " + valueFrom.ToString() + " and " + comboBoxColumn.Text + " >= " + valueTo.ToString();
                    } else if (valueFrom == valueTo) {
                        rowFilter = comboBoxColumn.Text + " = " + valueFrom.ToString();
                    }

                    (dataGridViewInformation.DataSource as DataTable).DefaultView.RowFilter = rowFilter;
                }




            } catch (Exception exception) {
                MessageBox.Show(this, exception.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log log = new Log(LogFileType.TXT, LogType.EXCEPTION);
                string logData = DateTime.Now.ToString() + "\t\tException\t\t" + exception.Message;
                log.write(logData);
            }

        }

        #endregion

        #region buttonGenerateExport
        private void buttonGenerateExport_Click(object sender, EventArgs e) {
            if (String.IsNullOrEmpty(comboBoxExportFilePath.Text))
                return;

            if (tabControl == null)
                return;

            if (tabControl.TabPages.Count == 0)
                return;

            if (radioButtonSelectedTab.Checked == true) {
                TabPage tabPage = tabControl.SelectedTab;
                prepareExport(tabPage, true);
            } else {
                int count = 0;
                foreach (TabPage tabPage in tabControl.TabPages) {
                    prepareExport(tabPage, false);
                    count++;
                }
                MessageBox.Show(this, "Done, " + count.ToString() + " files has been exported.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        #region buttonGenerateChart
        private void buttonGenerateChart_Click(object sender, EventArgs e) {

            if (tabControl == null)
                return;

            if (tabControl.TabPages.Count == 0)
                return;

            TabPage tabPage = tabControl.SelectedTab;
            if (tabPage == null)
                return;

            TableLayoutPanel tableLayoutInformation = (TableLayoutPanel)tabPage.Controls[0];
            if (tableLayoutInformation == null)
                return;

            DataGridView dataGridViewInformation = (DataGridView)tableLayoutInformation.Controls[0];
            if (dataGridViewInformation == null)
                return;

            DataTable dataTable = (DataTable)dataGridViewInformation.DataSource;
            if (dataTable == null)
                return;

            if (string.IsNullOrEmpty(comboBoxChartColumn.Text) || string.IsNullOrEmpty(comboBoxChartType.Text))
                return;



            string columnName = "";
            foreach (DataColumn dataColumn in dataTable.Columns) {
                if (dataColumn.DataType == typeof(DateTime))
                    columnName = dataColumn.ColumnName;
            }

            //Titles
            Title titleAts = new Title();
            titleAts.Alignment = ContentAlignment.TopCenter;
            titleAts.Name = "chartTitleAts";
            titleAts.Text = "Advanced Technologies and Solutions";

            Title titleChart = new Title();
            titleChart.Alignment = ContentAlignment.TopCenter;
            titleChart.Name = "chartTitle";
            titleChart.Text = tabControl.TabPages[0].Name + " (" + comboBoxChartType.Text + " Chart)";
            titleChart.TextStyle = TextStyle.Shadow;
            titleChart.Font = new Font("Calibri", 16, FontStyle.Bold);

            //Chart
            Chart chart = new Chart();
            chart.Titles.Add(titleAts);
            chart.Titles.Add(titleChart);
            chart.Dock = DockStyle.Fill;
            chart.BackColor = Color.FromName(comboBoxBackground.Text);
            chart.DataSource = dataTable;
            chart.AntiAliasing = AntiAliasingStyles.All;
            chart.TextAntiAliasingQuality = TextAntiAliasingQuality.High;
            chart.Text = dataGridViewInformation.Name;

            //ContextMenuStrip
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Name = "Chart";
            ToolStripItem toolStripItemChartSaveAsImage = contextMenuStrip.Items.Add("Save as image");
            toolStripItemChartSaveAsImage.Click += toolStripItemChartSaveAsImage_Click;
            chart.ContextMenuStrip = contextMenuStrip;

            //ChartLegends
            chart.Legends.Clear();
            chart.Legends.Add("Legend");
            chart.Legends["Legend"].Name = comboBoxChartColumn.Text;

            //ChartArea
            ChartArea chartArea = new ChartArea();
            chartArea.Name = columnName;
            chartArea.BackGradientStyle = GradientStyle.VerticalCenter;
            chartArea.BackColor = Color.FromName(comboBoxBackground.Text);

            //ChartAreaX
            chartArea.AxisX.Title = columnName;
            chartArea.AxisX.TitleFont = new Font("calibri", 16, FontStyle.Bold);
            chartArea.AxisX.LabelStyle.Format = "yyyy/MM/dd HH:mm:ss";
            chartArea.AxisX.TitleAlignment = StringAlignment.Center;
            chartArea.AxisX.MajorGrid.LineColor = Color.Transparent;
            chartArea.AxisX.Name = columnName;
            chartArea.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.LabelsAngleStep90;
            chartArea.AxisX.ScaleView.Zoomable = true;

            //ChartAreaY
            chartArea.AxisY.Title = comboBoxChartColumn.Text;
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisY.TitleFont = new Font("calibri", 16, FontStyle.Bold);
            chartArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.DashDotDot;
            chartArea.AxisY.Name = comboBoxChartColumn.Text;
            chartArea.AxisY.ScaleView.Zoomable = true;


            chart.ChartAreas.Add(chartArea);
            chart.MouseWheel += chart_MouseWheel;



            Series series = new Series(comboBoxChartColumn.Text);
            series.ChartType = (SeriesChartType)Enum.Parse(typeof(SeriesChartType), comboBoxChartType.Text, true);
            series.XValueMember = columnName;
            series.YValueMembers = comboBoxChartColumn.Text;
            series.Color = Color.FromName(comboBoxSeries.Text);
            series.BorderWidth = 5;


            chart.Series.Add(series);

            chart.DataBind();

            FormChart formChart = new FormChart();
            formChart.Controls.Add(chart);
            formChart.Text = tabControl.TabPages[0].Name;
            formChart.StartPosition = FormStartPosition.CenterParent;
            formChart.Show();

        }
        void toolStripItemChartSaveAsImage_Click(object sender, EventArgs e) {
            ToolStripItem toolStripItem = (ToolStripItem)sender;
            ContextMenuStrip contextMenuStrip = (ContextMenuStrip)toolStripItem.Owner;
            Chart chart = (Chart)contextMenuStrip.SourceControl;
            Form form = (Form)chart.Parent;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".jpg";
            saveFileDialog.FileName = form.Text;
            saveFileDialog.InitialDirectory = Convert.ToString(Environment.SpecialFolder.MyDocuments);
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                chart.SaveImage(saveFileDialog.FileName, ChartImageFormat.Jpeg);
                MessageBox.Show(this, "Chart has been saved successfully!", "Chart", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        #region buttonRevert
        private void buttonRevert_Click(object sender, EventArgs e) {
            if (tabControl == null)
                return;

            if (tabControl.TabPages.Count == 0)
                return;

            TabPage tabPage = tabControl.SelectedTab;
            TableLayoutPanel TableLayoutInformation = (TableLayoutPanel)tabPage.Controls[0];
            DataGridView dataGridViewInformation = (DataGridView)TableLayoutInformation.Controls[0];

            (dataGridViewInformation.DataSource as DataTable).DefaultView.RowFilter = "1=1";

        }
        #endregion

        #region buttonSettingsAdvance
        private void buttonSettingsAdvance_Click(object sender, EventArgs e) {
            FormAdvanceSettings formAdvanceSettings = new FormAdvanceSettings(user, tabControl);
            formAdvanceSettings.Show();
        }
        #endregion

        #region buttonRefresh
        private void buttonRefresh_Click(object sender, EventArgs e) {
            FormMain_Load(this, new EventArgs());
            Thread.Sleep(50);
        }
        #endregion


        #region comboBoxTrackers

        private void comboBoxTrackers_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                int selectedIndex = (int)comboBoxTrackers.SelectedIndex;

                if (selectedIndex != -1) {
                    if (checkedListBoxTrackers.GetItemCheckState(selectedIndex) == CheckState.Checked) {
                        checkedListBoxTrackers.SetItemCheckState(selectedIndex, CheckState.Unchecked);
                    } else {
                        checkedListBoxTrackers.SetItemCheckState(selectedIndex, CheckState.Checked);
                    }
                    labelTotalCheckedTrackers.Text = "Checked : " + checkedListBoxTrackers.CheckedItems.Count.ToString();
                }
            }
        }

        private void comboBoxTrackers_SelectedIndexChanged(object sender, EventArgs e) {
            //string selectedText = (string)comboBoxTrackers.SelectedText;
            //int index = checkedListBoxTrackers.FindString(selectedText, -1);
            //if (index != -1) {
            //    if (checkedListBoxTrackers.GetItemCheckState(index) == CheckState.Checked) {
            //        checkedListBoxTrackers.SetItemCheckState(index, CheckState.Unchecked);
            //    } else {
            //        checkedListBoxTrackers.SetItemCheckState(index, CheckState.Checked);
            //    }
            //}

        }

        #endregion

        #region comboBoxAccount
        private void comboBoxUser_SelectedIndexChanged(object sender, EventArgs e) {
            ThreadPool.QueueUserWorkItem(new WaitCallback(threadLoadFilters));
        }

        #endregion

        #region comboBoxTrackersBound
        private void comboBoxTrackersBound_SelectedIndexChanged(object sender, EventArgs e) {

            checkedListBoxTrackers.DisplayMember = comboBoxTrackersDisplayMember.Text;
        }
        #endregion

        #region comboBoxColumns
        private void comboBoxColumns_Click(object sender, EventArgs e) {

            if (tabControl == null)
                return;

            TabPage tabPage = tabControl.SelectedTab;
            if (tabPage == null)
                return;

            TableLayoutPanel tableLayoutInformation = (TableLayoutPanel)tabPage.Controls[0];
            if (tableLayoutInformation == null)
                return;

            DataGridView dataGridViewInformation = (DataGridView)tableLayoutInformation.Controls[0];
            if (dataGridViewInformation == null)
                return;


            comboBoxColumn.Items.Clear();
            foreach (DataGridViewColumn dataGridViewColumn in dataGridViewInformation.Columns) {
                if (dataGridViewColumn.Visible)
                    comboBoxColumn.Items.Add(dataGridViewColumn.Name);
            }

            if (comboBoxReportType.Items.IndexOf(dataGridViewInformation.Name) == 0) {

            }
        }

        private void comboBoxColumn_SelectedIndexChanged(object sender, EventArgs e) {
            TabPage tabPage = tabControl.SelectedTab;
            TableLayoutPanel TableLayoutInformation = (TableLayoutPanel)tabPage.Controls[0];
            DataGridView dataGridViewInformation = (DataGridView)TableLayoutInformation.Controls[0];
            DataTable dataTable = (DataTable)dataGridViewInformation.DataSource;

            Type dataType = dataTable.Columns[comboBoxColumn.Text].DataType;

            if (dataType == typeof(Int16) ||
                dataType == typeof(Int32) ||
                dataType == typeof(Int64) ||
                dataType == typeof(double) ||
                dataType == typeof(float)) {

                comboBoxFilterValueFrom.Enabled = true;
                comboBoxFilterValueTo.Enabled = true;
                comboBoxFilterContains.Enabled = false;

            } else if (dataType == typeof(string) || dataType == typeof(bool)) {
                comboBoxFilterValueFrom.Enabled = false;
                comboBoxFilterValueTo.Enabled = false;
                comboBoxFilterContains.Enabled = true;
            } else if (dataType == typeof(DateTime)) {
                comboBoxFilterValueFrom.Enabled = false;
                comboBoxFilterValueTo.Enabled = false;
                comboBoxFilterContains.Enabled = false;
            }

            comboBoxFilterContains.Items.Clear();
            foreach (DataRow dataRow in dataTable.Rows) {
                string data = dataRow[comboBoxColumn.Text].ToString();
                if (comboBoxFilterContains.Items.IndexOf(data) == -1) {
                    comboBoxFilterContains.Items.Add(data);
                }
            }



        }

        #endregion

        #region comboBoxContains
        private void comboBoxContains_KeyDown(object sender, KeyEventArgs e) {

            comboBoxFilterContains.Text = comboBoxFilterContains.Text;

            if (tabControl == null)
                return;

            TabPage tabPage = tabControl.SelectedTab;
            TableLayoutPanel TableLayoutInformation = (TableLayoutPanel)tabPage.Controls[0];
            DataGridView dataGridViewInformation = (DataGridView)TableLayoutInformation.Controls[0];

            //comboBoxContains.DataSource = (DataTable)dataGridViewInformation.DataSource;
            //comboBoxContains.DisplayMember = comboBoxColumn.SelectedItem.ToString();

        }

        #endregion

        #region comboBoxChartColumn
        private void comboBoxChartColumn_Click(object sender, EventArgs e) {

            if (tabControl == null)
                return;

            TabPage tabPage = tabControl.SelectedTab;
            if (tabPage == null)
                return;

            TableLayoutPanel tableLayoutInformation = (TableLayoutPanel)tabPage.Controls[0];
            if (tableLayoutInformation == null)
                return;

            DataGridView dataGridViewInformation = (DataGridView)tableLayoutInformation.Controls[0];
            if (dataGridViewInformation == null)
                return;

            DataTable dataTable = (DataTable)dataGridViewInformation.DataSource;
            comboBoxChartColumn.Items.Clear();
            foreach (DataColumn dataColumn in dataTable.Columns) {
                if (comboBoxChartColumn.Visible) {
                    if (dataColumn.DataType == typeof(int) ||
                        dataColumn.DataType == typeof(float) ||
                        dataColumn.DataType == typeof(double) ||
                        dataColumn.DataType == typeof(Int16) ||
                        dataColumn.DataType == typeof(Int32) ||
                        dataColumn.DataType == typeof(Int64)) {
                        comboBoxChartColumn.Items.Add(dataColumn.ColumnName);
                    }

                }
            }
        }
        #endregion

        #region comboBoxExportPath
        private void comboBoxExportPath_Click(object sender, EventArgs e) {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                Settings.Default.UserExportPath = folderBrowserDialog.SelectedPath;
                Settings.Default.Save();
                comboBoxExportFilePath.Text = folderBrowserDialog.SelectedPath;
            }

        }
        #endregion

        #region comboBoxDateShortCut
        private void comboBoxDateShortCut_SelectedIndexChanged(object sender, EventArgs e) {


            dateTimePickerDateTo.Value = DateTime.Today;                    

            switch (comboBoxDateShortCut.SelectedIndex) {
                case 0:
                    dateTimePickerDateFrom.Value = DateTime.Today.AddDays(1);
                    break;
                case 1:
                    dateTimePickerDateFrom.Value = dateTimePickerDateTo.Value.AddDays(-1);
                    break;
                case 2:
                    dateTimePickerDateFrom.Value = dateTimePickerDateTo.Value.AddDays(-3);
                    break;
                case 3:
                    dateTimePickerDateFrom.Value = dateTimePickerDateTo.Value.AddDays(-5);
                    break;
                case 4:
                    dateTimePickerDateFrom.Value = dateTimePickerDateTo.Value.AddDays(-7);
                    break;
                case 5:
                    dateTimePickerDateFrom.Value = dateTimePickerDateTo.Value.AddDays(-14);
                    break;
                case 6:
                    dateTimePickerDateFrom.Value = dateTimePickerDateTo.Value.AddDays(-21);
                    break;
                case 7:
                    dateTimePickerDateFrom.Value = dateTimePickerDateTo.Value.AddDays(-30);
                    break;
            }

        }
        #endregion

        #endregion
        #region Gui Methods
        private void updateProgressBarStatus(int value) {
            lock (progressBarStatus) {
                this.Invoke(new Action(() => {
                    if (value > progressBarStatus.Maximum)
                        value = progressBarStatus.Maximum;

                    progressBarStatus.Value = value;
                    progressBarStatus.Refresh();

                    if (tabControl != null) {
                        labelTotalTabPages.Text = "TabPages : " + tabControl.TabPages.Count.ToString();
                    } else {
                        labelTotalTabPages.Text = "TabPages : 0";
                    }
                    progressBarStatus.Refresh();
                }));
            }
        }
        #endregion
        #region chart
        void chart_MouseWheel(object sender, MouseEventArgs e) {
            Chart chart = (Chart)sender;

            try {
                if (e.Delta < 0) {
                    chart.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    //chart.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                }

                if (e.Delta > 0) {
                    double xMin = chart.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
                    double xMax = chart.ChartAreas[0].AxisX.ScaleView.ViewMaximum;
                    //double yMin = chart.ChartAreas[0].AxisY.ScaleView.ViewMinimum;
                    //double yMax = chart.ChartAreas[0].AxisY.ScaleView.ViewMaximum;

                    double posXStart = chart.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 4;
                    double posXFinish = chart.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 4;
                    //double posYStart = chart.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y) - (yMax - yMin) / 4;
                    //double posYFinish = chart.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y) + (yMax - yMin) / 4;

                    chart.ChartAreas[0].AxisX.ScaleView.Zoom(posXStart, posXFinish);
                    //chart.ChartAreas[0].AxisY.ScaleView.Zoom(posYStart, posYFinish);
                }
            } catch {
            }
        }
        #endregion
        #region link
        private void pictureBoxAtsWebsite_Click(object sender, EventArgs e) {
            System.Diagnostics.Process.Start("http://www.ats-qatar.com/");
        }

        private void pictureBoxTwitter_Click(object sender, EventArgs e) {
            System.Diagnostics.Process.Start("https://twitter.com/ATSQATAR2014");
        }

        private void pictureBoxGooglePlus_Click(object sender, EventArgs e) {
            System.Diagnostics.Process.Start("https://plus.google.com/108001371032978819108/posts?hl=en");
        }

        private void pictureBoxInstagram_Click(object sender, EventArgs e) {
            System.Diagnostics.Process.Start("https://instagram.com/atsqatar2014/");
        }

        private void pictureBoxFacebook_Click(object sender, EventArgs e) {
            System.Diagnostics.Process.Start("https://www.facebook.com/pages/Advanced-Technologies-and-Solutions/618588578251068");
        }

        #endregion
        #region prepareExport
        public void prepareExport(TabPage tabPage, bool openTheFile) {

            if (String.IsNullOrEmpty(comboBoxExportFileType.Text))
                return;

            if (tabPage == null)
                return;

            TableLayoutPanel tableLayoutInformation = (TableLayoutPanel)tabPage.Controls[0];
            if (tableLayoutInformation == null)
                return;

            DataGridView dataGridViewInformation = (DataGridView)tableLayoutInformation.Controls[0];
            if (dataGridViewInformation == null)
                return;

            DataTable dataTable = new DataTable();

            foreach (DataGridViewColumn dataGridViewColumn in dataGridViewInformation.Columns) {
                if (dataGridViewColumn.Visible)
                    dataTable.Columns.Add(dataGridViewColumn.Name, dataGridViewColumn.ValueType);
            }

            foreach (DataGridViewRow dataGridViewRow in dataGridViewInformation.Rows) {
                DataRow dataRow = dataTable.NewRow();
                foreach (DataColumn dataColumn in dataTable.Columns) {
                    dataRow[dataColumn.ColumnName] = dataGridViewRow.Cells[dataColumn.ColumnName].Value;
                }
                dataTable.Rows.Add(dataRow);
            }


            ExportFileType exportFileType = (ExportFileType)(Enum.Parse(typeof(ExportFileType), comboBoxExportFileType.Text));
            ReportType reportType = (ReportType)(Enum.Parse(typeof(ReportType), dataGridViewInformation.Name));

            ReportInformation reportInformation = new ReportInformation();

            TableLayoutPanel tableLayoutPanelParent = (TableLayoutPanel)tabPage.Controls[0];
            TableLayoutPanel tableLayoutPanelRight = (TableLayoutPanel)tableLayoutPanelParent.Controls[2];

            ListView listViewDetails = (ListView)tableLayoutPanelRight.Controls[0];

            reportInformation.companyName = listViewDetails.Items[0].SubItems[1].Text;
            reportInformation.userName = listViewDetails.Items[1].SubItems[1].Text;

            if (reportType == ReportType.HISTORICAL || reportType == ReportType.RUNNING || reportType == ReportType.IDLING || reportType == ReportType.ACC || reportType == ReportType.GEOFENCE || reportType == ReportType.OVERSPEED || reportType == ReportType.ACC) {
                reportInformation.trackerVehicleReg = listViewDetails.Items[2].SubItems[1].Text;
                reportInformation.trackerVehicleModel = listViewDetails.Items[3].SubItems[1].Text;
                reportInformation.trackerOwnerName = listViewDetails.Items[4].SubItems[1].Text;
                reportInformation.trackerDriverName = listViewDetails.Items[5].SubItems[1].Text;

                reportInformation.trackerDeviceImei = listViewDetails.Items[6].SubItems[1].Text;
                reportInformation.trackerSimNumber = listViewDetails.Items[7].SubItems[1].Text;
                reportInformation.trackerVehicleCreated = listViewDetails.Items[8].SubItems[1].Text;
                reportInformation.trackerDeviceExpiry = listViewDetails.Items[9].SubItems[1].Text;

            }

            ListView listViewSummary = (ListView)tableLayoutPanelRight.Controls[1];
            reportInformation.summaryDateTimeFrom = listViewSummary.Items[0].SubItems[1].Text;
            reportInformation.summaryDateTimeTo = listViewSummary.Items[1].SubItems[1].Text;



            if (reportType == ReportType.RUNNING) {
                reportInformation.summaryTotalDistance = listViewSummary.Items[2].SubItems[1].Text;
                reportInformation.summaryTotalFuel = listViewSummary.Items[3].SubItems[1].Text;
                reportInformation.summaryTotalCost = listViewSummary.Items[4].SubItems[1].Text;
                reportInformation.summaryTotalRunningTime = listViewSummary.Items[5].SubItems[1].Text;
            } else if (reportType == ReportType.IDLING) {
                reportInformation.summaryTotalDistance = listViewSummary.Items[2].SubItems[1].Text;
                reportInformation.summaryTotalFuel = listViewSummary.Items[3].SubItems[1].Text;
                reportInformation.summaryTotalCost = listViewSummary.Items[4].SubItems[1].Text;
                reportInformation.summaryTotalIdlingTime = listViewSummary.Items[5].SubItems[1].Text;
            } else if (reportType == ReportType.GEOFENCE) {
                reportInformation.summaryTotalDistance = listViewSummary.Items[2].SubItems[1].Text;
                reportInformation.summaryTotalFuel = listViewSummary.Items[3].SubItems[1].Text;
                reportInformation.summaryTotalCost = listViewSummary.Items[4].SubItems[1].Text;
                reportInformation.summaryTotalGeofenceActiveTime = listViewSummary.Items[5].SubItems[1].Text;
            } else if (reportType == ReportType.ACC) {
                reportInformation.summaryTotalDistance = listViewSummary.Items[2].SubItems[1].Text;
                reportInformation.summaryTotalFuel = listViewSummary.Items[3].SubItems[1].Text;
                reportInformation.summaryTotalCost = listViewSummary.Items[4].SubItems[1].Text;
                reportInformation.summaryTotalAccActiveTime = listViewSummary.Items[5].SubItems[1].Text;
            } else if (reportType == ReportType.TRACKERS_GEOFENCE) {
                reportInformation.summaryTotalDistance = listViewSummary.Items[2].SubItems[1].Text;
                reportInformation.summaryTotalFuel = listViewSummary.Items[3].SubItems[1].Text;
                reportInformation.summaryTotalCost = listViewSummary.Items[4].SubItems[1].Text;
            }

            if (string.IsNullOrEmpty(comboBoxExportFileType.Text))
                return;

            try {
                Export export = new Export(exportFileType, reportType);
                export.parentDirectory = "TqatExportFiles";
                export.path = comboBoxExportFilePath.Text;
                export.title = reportInformation.trackerVehicleReg;//tabControl.TabPages[0].Name;

                if (!File.Exists(Settings.Default.UserLogoPath)) {
                    export.logoPath = Directory.GetCurrentDirectory() + "\\logo.png";
                } else {
                    export.logoPath = Settings.Default.UserLogoPath;
                }
                export.companyName = company.DisplayName;
                export.reportInformation = reportInformation;
                if (!export.dataTable(dataTable, openTheFile)) {
                    throw new Exception();
                }
            } catch (Exception exception) {
                MessageBox.Show(this, exception.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log log = new Log(LogFileType.TXT, LogType.EXCEPTION);
                string logData = DateTime.Now.ToString() + "\t\tException\t\t" + exception.Message;
                log.write(logData);
            }
        }
        #endregion
        #region pictureBoxCompanyLogo
        private void pictureBoxCompanyLogo_Click(object sender, EventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                Settings.Default.UserLogoPath = openFileDialog.FileName;
                Settings.Default.Save();
            }

            if (!String.IsNullOrEmpty(Settings.Default.UserLogoPath)) {
                pictureBoxCompanyLogo.Image = Image.FromFile(Settings.Default.UserLogoPath);
                pictureBoxCompanyLogo.Refresh();
            }
        }
        #endregion
        #region logout
        private void pictureBoxLogout_Click(object sender, EventArgs e) {
            Settings.Default.accountRememberMe = false;
            Settings.Default.Save();

            DialogLogin dialogLogin = new DialogLogin();
            dialogLogin.Show();
            this.Close();
        }
        #endregion

        private void tableLayoutTrackers_Paint(object sender, PaintEventArgs e) {

        }

        private void comboBoxReportType_SelectedIndexChanged(object sender, EventArgs e) {
            ReportType reportType = (ReportType)Enum.Parse(typeof(ReportType), comboBoxReportType.Text);
            if (reportType == ReportType.HISTORICAL) {
                comboBoxLimit.Enabled = true;
            } else {
                comboBoxLimit.Enabled = false;
            }
        }

        private void comboBoxCollection_SelectedIndexChanged(object sender, EventArgs e) {
            comboBoxTrackers.Items.Clear();
            checkedListBoxTrackers.Items.Clear();
            foreach (Tracker tracker in company.Trackers) {
                foreach (Collection collection in tracker.Collections) {
                    Collection collectionSelected = (Collection)comboBoxCollection.SelectedItem;
                    if (collectionSelected.Id == collection.Id) {
                        checkedListBoxTrackers.Items.Add(tracker);
                        comboBoxTrackers.Items.Add(tracker);
                    }
                }

            }
            comboBoxTrackers.DisplayMember = comboBoxTrackersDisplayMember.Text;
            checkedListBoxTrackers.DisplayMember = comboBoxTrackersDisplayMember.Text;
            groupBoxTrackers.Text = "Trackers " + checkedListBoxTrackers.Items.Count.ToString();
        }


    }
}
