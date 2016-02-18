using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.IO;
using System.Windows.Forms;


using iTextSharp.text;
using iTextSharp.text.pdf;

using Ats;
using Ats.Database;
using Ats.Report;

namespace Ats.Helper {





    class ItextPageEvent : PdfPageEventHelper {

        PdfContentByte pdfContentByte;
        PdfTemplate pdfTemplate;

        int pageNumber = 0;
        //int pageCount = 0;

        iTextSharp.text.Font font = FontFactory.GetFont("Courier", BaseFont.IDENTITY_H, 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);

        BaseFont baseFont = null;

        public override void OnOpenDocument(PdfWriter writer, Document document) {
            try {

                baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                pdfContentByte = writer.DirectContent;
                pdfTemplate = pdfContentByte.CreateTemplate(50, 50);
            } catch (DocumentException documentException) {
                Log log = new Log(LogFileType.TXT, LogType.EXCEPTION);
                string logData = DateTime.Now.ToString() + "\t\t documentException \t\t" + documentException.Message;
                log.write(logData);
            } catch (System.IO.IOException ioException) {
                Log log = new Log(LogFileType.TXT, LogType.EXCEPTION);
                string logData = DateTime.Now.ToString() + "\t\t documentException \t\t" + ioException.Message;
                log.write(logData);
            }
        }
        public override void OnEndPage(PdfWriter writer, Document document) {
            base.OnEndPage(writer, document);

            pageNumber = writer.PageNumber;
            String text = "Page " + pageNumber + " of ";
            float length = baseFont.GetWidthPoint(text, 8);
            Rectangle pageSize = document.PageSize;
            pdfContentByte.SetRGBColorFill(100, 100, 100);
            pdfContentByte.BeginText();
            pdfContentByte.SetFontAndSize(baseFont, 8);
            pdfContentByte.SetTextMatrix(pageSize.GetLeft(40), pageSize.GetBottom(20));
            pdfContentByte.ShowText(text);
            pdfContentByte.EndText();
            pdfContentByte.AddTemplate(pdfTemplate, pageSize.GetLeft(40) + length, pageSize.GetBottom(20));

            pdfContentByte.BeginText();
            pdfContentByte.SetFontAndSize(baseFont, 8);
            pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, /*DateTime.Now.ToString()*/ "Powered by Advanced Technologies and Solutions.", pageSize.GetRight(40), pageSize.GetBottom(20), 0);
            pdfContentByte.EndText();

        }
        public override void OnCloseDocument(PdfWriter writer, Document document) {
            base.OnEndPage(writer, document);
            pdfTemplate.BeginText();
            pdfTemplate.SetFontAndSize(baseFont, 8);
            pdfTemplate.SetTextMatrix(0, 0);
            pdfTemplate.ShowText("" + (writer.PageNumber - 1));
            pdfTemplate.EndText();
        }
    }

    class Export {
        public string logoPath {
            get;
            set;
        }
        public string path {
            get;
            set;
        }
        public string parentDirectory {
            get;
            set;
        }
        public string fileName {
            get;
            set;
        }
        //================================================
        public string companyName {
            get;
            set;
        }
        public string title {
            get;
            set;
        }
        //================================================
        public ReportInformation reportInformation {
            get;
            set;
        }

        ExportFileType exportFileType;
        ReportType reportType;

        public Export(ExportFileType exportFileType, ReportType reportType) {
            this.exportFileType = exportFileType;
            this.reportType = reportType;
        }
        public bool dataTable(DataTable dataTable, bool openTheFile) {

            DataTable dataTableCloned = dataTable.Copy();


            foreach (DataColumn dataColumn in dataTable.Columns) {
                foreach (DataRow dataRow in dataTableCloned.Rows) {
                    if (dataColumn.DataType == typeof(double)) {
                        if (dataColumn.ColumnName != "Latitude" &&
                            dataColumn.ColumnName != "Longitude" &&
                            dataColumn.ColumnName != "LatitudeTo" &&
                            dataColumn.ColumnName != "LongitudeTo" &&
                            dataColumn.ColumnName != "LatitudeFrom" &&
                            dataColumn.ColumnName != "LongitudeFrom") {
                            if (dataRow[dataColumn.ColumnName] != System.DBNull.Value) {
                                dataRow[dataColumn.ColumnName] = Math.Round((double)dataRow[dataColumn.ColumnName], 2);
                            }
                        }
                    }
                }
            }


            this.path = this.path + "\\" + parentDirectory;

            if (!Directory.Exists(this.path))
                Directory.CreateDirectory(this.path);

            this.fileName = this.path + "\\" + DateTime.Now.ToString("yyyyMMdd") + " " + Enum.GetName(typeof(ReportType), reportType) + " " + this.title + "." + Enum.GetName(typeof(ExportFileType), exportFileType);


            if (File.Exists(this.fileName))
                File.Delete(this.fileName);


            switch (this.exportFileType) {
                case ExportFileType.CSV:
                    dataTableToCsv(dataTableCloned, this.fileName);
                    break;
                case ExportFileType.TXT:
                    dataTableToText(dataTableCloned, this.fileName);
                    break;
                case ExportFileType.PDF:
                    dataTableToPdf(dataTableCloned, this.fileName);
                    break;
            }

            if (openTheFile)
                System.Diagnostics.Process.Start(this.fileName);
            return true;
        }
        public void dataTableToCsv(DataTable dataTable, string fileName) {
            StreamWriter streamWriter;
            streamWriter = new StreamWriter(fileName, true);
            try {
                streamWriter.Write("ReportType,,," + Enum.GetName(typeof(ReportType), reportType) + "\n");
                streamWriter.Write("CompanyName,,," + reportInformation.companyName + "\n");

                if (
                    this.reportType == ReportType.HISTORICAL ||
                    this.reportType == ReportType.RUNNING ||
                    this.reportType == ReportType.IDLING ||
                    this.reportType == ReportType.GEOFENCE ||
                    this.reportType == ReportType.ACC ||
                    this.reportType == ReportType.OVERSPEED ||
                    this.reportType == ReportType.EXTERNAL_POWER_CUT
                    ) {

                    streamWriter.Write("VehicleRegistration," + reportInformation.trackerVehicleReg + "\n");
                    streamWriter.Write("VehicleModel,,," + reportInformation.trackerVehicleModel + "\n");
                    streamWriter.Write("OwnerName,,," + reportInformation.trackerOwnerName + "\n");
                    streamWriter.Write("DriverName,,," + reportInformation.trackerDriverName + "\n");
                    streamWriter.Write("DeviceImei,,," + reportInformation.trackerDeviceImei + "\n");
                }

                switch (this.reportType) {
                    case ReportType.RUNNING:
                        streamWriter.Write("Total Distance,,," + reportInformation.summaryTotalDistance + "\n");
                        streamWriter.Write("Total Fuel,,," + reportInformation.summaryTotalFuel + "\n");
                        streamWriter.Write("Total Cost,,," + reportInformation.summaryTotalCost + "\n");
                        streamWriter.Write("Total Running Time,,," + reportInformation.summaryTotalRunningTime + "\n");
                        break;
                    case ReportType.IDLING:
                        streamWriter.Write("Total Distance,,," + reportInformation.summaryTotalDistance + "\n");
                        streamWriter.Write("Total Fuel,,," + reportInformation.summaryTotalFuel + "\n");
                        streamWriter.Write("Total Cost,,," + reportInformation.summaryTotalCost + "\n");
                        streamWriter.Write("Total Idling Time,,," + reportInformation.summaryTotalIdlingTime + "\n");
                        break;
                    case ReportType.GEOFENCE:
                        streamWriter.Write("Total Distance,,," + reportInformation.summaryTotalDistance + "\n");
                        streamWriter.Write("Total Fuel,,," + reportInformation.summaryTotalFuel + "\n");
                        streamWriter.Write("Total Cost,,," + reportInformation.summaryTotalCost + "\n");
                        streamWriter.Write("Total Geofence Active Time,,," + reportInformation.summaryTotalGeofenceActiveTime + "\n");
                        break;
                    case ReportType.ACC:
                        streamWriter.Write("Total Distance,,," + reportInformation.summaryTotalDistance + "\n");
                        streamWriter.Write("Total Fuel,,," + reportInformation.summaryTotalFuel + "\n");
                        streamWriter.Write("Total Cost,,," + reportInformation.summaryTotalCost + "\n");
                        streamWriter.Write("Total ACC Active Time,,," + reportInformation.summaryTotalAccActiveTime + "\n");
                        break;
                    case ReportType.EXTERNAL_POWER_CUT:
                        streamWriter.Write("Total Distance,,," + reportInformation.summaryTotalDistance + "\n");
                        streamWriter.Write("Total Fuel,,," + reportInformation.summaryTotalFuel + "\n");
                        streamWriter.Write("Total Cost,,," + reportInformation.summaryTotalCost + "\n");
                        streamWriter.Write("Total ExternalPower Cut Time,,," + reportInformation.summaryTotalExternalPowerCutTime + "\n");
                        break;
                    case ReportType.TRACKERS_GEOFENCE:
                        streamWriter.Write("Total Distance,,," + reportInformation.summaryTotalDistance + "\n");
                        streamWriter.Write("Total Fuel,,," + reportInformation.summaryTotalFuel + "\n");
                        streamWriter.Write("Total Cost,,," + reportInformation.summaryTotalCost + "\n");
                        break;

                }

                if (
                  this.reportType == ReportType.HISTORICAL ||
                  this.reportType == ReportType.RUNNING ||
                  this.reportType == ReportType.IDLING ||
                  this.reportType == ReportType.GEOFENCE ||
                  this.reportType == ReportType.ACC ||
                  this.reportType == ReportType.OVERSPEED ||
                  this.reportType == ReportType.EXTERNAL_POWER_CUT
                  ) {
                    streamWriter.Write("DateTime From,,," + reportInformation.summaryDateTimeFrom + "\n");
                    streamWriter.Write("DateTime To,,," + reportInformation.summaryDateTimeTo + "\n");
                }
                streamWriter.Write("Created On,,," + DateTime.Now.ToString() + "\n\n");
                //=================================================



                foreach (DataColumn dataColumn in dataTable.Columns) {
                    streamWriter.Write(dataColumn.ColumnName + ",");
                }
                streamWriter.Write("\n");


                foreach (DataRow dataRow in dataTable.Rows) {
                    for (int index = 0; index < dataTable.Columns.Count; index++) {
                        streamWriter.Write(dataRow[index] + ",");
                    }
                    streamWriter.Write("\n");
                }
            } catch (Exception exception) {
                Log log = new Log(LogFileType.TXT, LogType.EXCEPTION);
                string logData = DateTime.Now.ToString() + "\t\t Exception \t\t" + exception.Message;
                log.write(logData);
            } finally {
                streamWriter.Close();
            }

        }
        public void dataTableToText(DataTable dataTable, string fileName) {
            StreamWriter streamWriter;
            streamWriter = new StreamWriter(fileName, true);
            try {

                streamWriter.Write("ReportType\t\t\t" + Enum.GetName(typeof(ReportType), reportType) + "\r\n");
                streamWriter.Write("CompanyName\t\t\t" + reportInformation.companyName + "\r\n");

                if (
                    this.reportType == ReportType.HISTORICAL ||
                    this.reportType == ReportType.RUNNING ||
                    this.reportType == ReportType.IDLING ||
                    this.reportType == ReportType.GEOFENCE ||
                    this.reportType == ReportType.ACC ||
                    this.reportType == ReportType.OVERSPEED ||
                    this.reportType == ReportType.EXTERNAL_POWER_CUT
                    ) {

                    streamWriter.Write("VehicleRegistration\t" + reportInformation.trackerVehicleReg + "\r\n");
                    streamWriter.Write("VehicleModel\t\t\t" + reportInformation.trackerVehicleModel + "\r\n");
                    streamWriter.Write("OwnerName\t\t\t" + reportInformation.trackerOwnerName + "\r\n");
                    streamWriter.Write("DriverName\t\t\t" + reportInformation.trackerDriverName + "\r\n");
                    streamWriter.Write("DeviceImei\t\t\t" + reportInformation.trackerDeviceImei + "\r\n");
                }

                switch (this.reportType) {
                    case ReportType.RUNNING:
                        streamWriter.Write("Total Distance\t\t\t" + reportInformation.summaryTotalDistance + "\r\n");
                        streamWriter.Write("Total Fuel\t\t\t" + reportInformation.summaryTotalFuel + "\r\n");
                        streamWriter.Write("Total Cosl\t\t\t" + reportInformation.summaryTotalCost + "\r\n");
                        streamWriter.Write("Total Running Time\t\t\t" + reportInformation.summaryTotalRunningTime + "\r\n");
                        break;
                    case ReportType.IDLING:
                        streamWriter.Write("Total Distance\t\t\t" + reportInformation.summaryTotalDistance + "\r\n");
                        streamWriter.Write("Total Fuel\t\t\t" + reportInformation.summaryTotalFuel + "\r\n");
                        streamWriter.Write("Total Cosl\t\t\t" + reportInformation.summaryTotalCost + "\r\n");
                        streamWriter.Write("Total Idling Time\t\t\t" + reportInformation.summaryTotalIdlingTime + "\r\n");
                        break;
                    case ReportType.GEOFENCE:
                        streamWriter.Write("Total Distance\t\t\t" + reportInformation.summaryTotalDistance + "\r\n");
                        streamWriter.Write("Total Fuel\t\t\t" + reportInformation.summaryTotalFuel + "\r\n");
                        streamWriter.Write("Total Cosl\t\t\t" + reportInformation.summaryTotalCost + "\r\n");
                        streamWriter.Write("Total Geofence Active Time\t\t\t" + reportInformation.summaryTotalGeofenceActiveTime + "\r\n");
                        break;
                    case ReportType.ACC:
                        streamWriter.Write("Total Distance\t\t\t" + reportInformation.summaryTotalDistance + "\r\n");
                        streamWriter.Write("Total Fuel\t\t\t" + reportInformation.summaryTotalFuel + "\r\n");
                        streamWriter.Write("Total Cosl\t\t\t" + reportInformation.summaryTotalCost + "\r\n");
                        streamWriter.Write("Total ACC Active Time\t\t\t" + reportInformation.summaryTotalAccActiveTime + "\r\n");
                        break;
                    case ReportType.EXTERNAL_POWER_CUT:
                        streamWriter.Write("Total Distance\t\t\t" + reportInformation.summaryTotalDistance + "\r\n");
                        streamWriter.Write("Total Fuel\t\t\t" + reportInformation.summaryTotalFuel + "\r\n");
                        streamWriter.Write("Total Cosl\t\t\t" + reportInformation.summaryTotalCost + "\r\n");
                        streamWriter.Write("Total ExternalPower Cut Time\t\t\t" + reportInformation.summaryTotalExternalPowerCutTime + "\r\n");
                        break;
                    case ReportType.TRACKERS_GEOFENCE:
                        streamWriter.Write("Total Distance\t\t\t" + reportInformation.summaryTotalDistance + "\r\n");
                        streamWriter.Write("Total Fuel\t\t\t" + reportInformation.summaryTotalFuel + "\r\n");
                        streamWriter.Write("Total Cost\t\t\t" + reportInformation.summaryTotalCost + "\r\n");
                        break;
                }
                if (
                  this.reportType == ReportType.HISTORICAL ||
                  this.reportType == ReportType.RUNNING ||
                  this.reportType == ReportType.IDLING ||
                  this.reportType == ReportType.GEOFENCE ||
                  this.reportType == ReportType.ACC ||
                  this.reportType == ReportType.OVERSPEED ||
                  this.reportType == ReportType.EXTERNAL_POWER_CUT
                  ) {
                    streamWriter.Write("DateTime From\t\t\t" + reportInformation.summaryDateTimeFrom + "\r\n");
                    streamWriter.Write("DateTime To\t\t\t" + reportInformation.summaryDateTimeTo + "\r\n");
                }
                streamWriter.Write("Created On\t\t\t" + DateTime.Now.ToString() + "\r\n\r\n");
                //====================================================
                foreach (DataColumn dataColumn in dataTable.Columns) {
                    streamWriter.Write(dataColumn.ColumnName + "\t\t");
                }
                streamWriter.Write("\r\n");

                foreach (DataRow dataRow in dataTable.Rows) {
                    for (int index = 0; index < dataTable.Columns.Count; index++) {
                        streamWriter.Write(dataRow[index] + "\t\t");
                    }
                    streamWriter.Write("\r\n");
                }
            } catch (Exception exception) {
                Log log = new Log(LogFileType.TXT, LogType.EXCEPTION);
                string logData = DateTime.Now.ToString() + "\t\t Exception \t\t" + exception.Message;
                log.write(logData);
            } finally {
                streamWriter.Close();
            }
        }
        public void dataTableToPdf(System.Data.DataTable dataTable, string FilePath) {


            FontFactory.RegisterDirectories();
            iTextSharp.text.Font font = FontFactory.GetFont("Courier", BaseFont.IDENTITY_H, 8, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
            iTextSharp.text.Font fontMedium = FontFactory.GetFont("Courier", BaseFont.IDENTITY_H, 11, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
            Document document = new Document(PageSize.A4.Rotate(), 20f, 20f, 20f, 25f);

            PdfWriter pdfWriter = PdfWriter.GetInstance(document, new FileStream(FilePath, FileMode.Create));
            pdfWriter.PageEvent = new ItextPageEvent();

            PdfPTable pdfTable = new PdfPTable(dataTable.Columns.Count);
            pdfTable.WidthPercentage = 100f;
            pdfTable.SpacingAfter = 10;
            pdfTable.SpacingBefore = 10;

            PdfContentByte pdfContentByte = new PdfContentByte(pdfWriter);

            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath, true);
            logo.ScaleToFit(80f, 80f);

            StringBuilder sbParagraph1, sbParagraph2, sbParagraph3, sbParagraph4, sbParagraph5, sbParagraph6;
            sbParagraph1 = new StringBuilder("");
            sbParagraph2 = new StringBuilder("");
            sbParagraph3 = new StringBuilder("");
            sbParagraph4 = new StringBuilder("");
            sbParagraph5 = new StringBuilder("");
            sbParagraph6 = new StringBuilder("");


            if (
                reportType == ReportType.HISTORICAL ||
                reportType == ReportType.RUNNING ||
                reportType == ReportType.IDLING ||
                reportType == ReportType.ACC ||
                reportType == ReportType.GEOFENCE ||
                reportType == ReportType.OVERSPEED ||
                reportType == ReportType.EXTERNAL_POWER_CUT
                ) {
                sbParagraph1.Append(
                    "Vehicle Reg\n" +
                    "Vehicle Model\n" +
                    "Owner Name\n" +
                    "Driver Name\n" +
                    "Device Imei\n"
                );
                sbParagraph2.Append(
                    ": " + reportInformation.trackerVehicleReg + "\n" +
                    ": " + reportInformation.trackerVehicleModel + "\n" +
                    ": " + reportInformation.trackerOwnerName + "\n" +
                    ": " + reportInformation.trackerDriverName + "\n" +
                    ": " + reportInformation.trackerDeviceImei + "\n"
                );
            }

            if (
                reportType == ReportType.RUNNING ||
                reportType == ReportType.IDLING ||
                reportType == ReportType.ACC ||
                reportType == ReportType.GEOFENCE ||
                reportType == ReportType.OVERSPEED ||
                reportType == ReportType.TRACKERS_GEOFENCE ||
                reportType == ReportType.EXTERNAL_POWER_CUT
               ) {

                sbParagraph3.Append(
                   "Total Distance\n" +
                   "Total Fuel\n" +
                   "Total Cost\n");

                switch (reportType) {
                    case ReportType.RUNNING:
                        sbParagraph3.Append("Total Running Time\n");
                        break;
                    case ReportType.IDLING:
                        sbParagraph3.Append("Total Idling Time\n");
                        break;
                    case ReportType.GEOFENCE:
                        sbParagraph3.Append("Total Geofence Active Time\n");
                        break;
                    case ReportType.ACC:
                        sbParagraph3.Append("Total ACC Active Time\n");
                        break;
                    case ReportType.EXTERNAL_POWER_CUT:
                        sbParagraph3.Append("Total ExternalPower Cut Time\n");
                        break;
                }


                sbParagraph4.Append(": " + reportInformation.summaryTotalDistance + "\n");
                sbParagraph4.Append(": " + reportInformation.summaryTotalFuel + "\n");
                sbParagraph4.Append(": " + reportInformation.summaryTotalCost + "\n");
                switch (reportType) {
                    case ReportType.RUNNING:
                        sbParagraph4.Append(": " + reportInformation.summaryTotalRunningTime + "\n");
                        break;
                    case ReportType.IDLING:
                        sbParagraph4.Append(": " + reportInformation.summaryTotalIdlingTime + "\n");
                        break;
                    case ReportType.GEOFENCE:
                        sbParagraph4.Append(": " + reportInformation.summaryTotalGeofenceActiveTime + "\n");
                        break;
                    case ReportType.ACC:
                        sbParagraph4.Append(": " + reportInformation.summaryTotalAccActiveTime + "\n");
                        break;
                    case ReportType.EXTERNAL_POWER_CUT:
                        sbParagraph4.Append(": " + reportInformation.summaryTotalExternalPowerCutTime + "\n");
                        break;
                }
            }

            if (
               reportType == ReportType.HISTORICAL ||
               reportType == ReportType.RUNNING ||
               reportType == ReportType.IDLING ||
               reportType == ReportType.ACC ||
               reportType == ReportType.GEOFENCE ||
               reportType == ReportType.OVERSPEED ||
                reportType == ReportType.EXTERNAL_POWER_CUT
               ) {
                sbParagraph5.Append("DateTime From\n");
                sbParagraph5.Append("DateTime To\n");
            }

            sbParagraph5.Append("Generated by\n" + "Created on\n");

            if (
               reportType == ReportType.HISTORICAL ||
               reportType == ReportType.RUNNING ||
               reportType == ReportType.IDLING ||
               reportType == ReportType.ACC ||
               reportType == ReportType.GEOFENCE ||
               reportType == ReportType.OVERSPEED ||
                reportType == ReportType.EXTERNAL_POWER_CUT

               ) {
                sbParagraph6.Append(": " + reportInformation.summaryDateTimeFrom + "\n");
                sbParagraph6.Append(": " + reportInformation.summaryDateTimeTo + "\n");
            }

            sbParagraph6.Append(": " + reportInformation.userName + "\n");
            sbParagraph6.Append(": " + DateTime.Now.ToString() + "\n");

            Paragraph paragraph0 = new Paragraph(reportInformation.companyName + "\n" + Enum.GetName(typeof(ReportType), reportType) + " Report\n" + this.title, fontMedium);
            Paragraph paragraph1 = new Paragraph(sbParagraph1.ToString(), font);
            Paragraph paragraph2 = new Paragraph(sbParagraph2.ToString(), font);
            Paragraph paragraph3 = new Paragraph(sbParagraph3.ToString(), font);
            Paragraph paragraph4 = new Paragraph(sbParagraph4.ToString(), font);
            Paragraph paragraph5 = new Paragraph(sbParagraph5.ToString(), font);
            Paragraph paragraph6 = new Paragraph(sbParagraph6.ToString(), font);


            //======================================
            PdfPTable pdfTableHeader;
            PdfPCell pdfCell;

            pdfTableHeader = new PdfPTable(8);

            pdfCell = new PdfPCell(logo, true);
            pdfCell.BorderWidth = 0;
            pdfCell.PaddingLeft = 20f;
            pdfCell.PaddingRight = 20f;
            pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            pdfTableHeader.AddCell(pdfCell);

            pdfCell = new PdfPCell(paragraph0);
            pdfCell.BorderWidth = 0;
            pdfCell.Padding = 0;
            pdfCell.PaddingTop = 12;
            pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            pdfTableHeader.AddCell(pdfCell);

            pdfCell = new PdfPCell(paragraph1);
            pdfCell.BorderWidth = 0;
            pdfCell.Padding = 0;
            pdfCell.PaddingTop = 12;
            pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            pdfTableHeader.AddCell(pdfCell);

            pdfCell = new PdfPCell(paragraph2);
            pdfCell.BorderWidth = 0;
            pdfCell.Padding = 0;
            pdfCell.PaddingTop = 12;
            pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            pdfTableHeader.AddCell(pdfCell);


            pdfCell = new PdfPCell(paragraph3);
            pdfCell.BorderWidth = 0;
            pdfCell.Padding = 0;
            pdfCell.PaddingTop = 12;
            pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            pdfTableHeader.AddCell(pdfCell);

            pdfCell = new PdfPCell(paragraph4);
            pdfCell.BorderWidth = 0;
            pdfCell.Padding = 0;
            pdfCell.PaddingTop = 12;
            pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            pdfTableHeader.AddCell(pdfCell);

            pdfCell = new PdfPCell(paragraph5);
            pdfCell.BorderWidth = 0;
            pdfCell.Padding = 0;
            pdfCell.PaddingTop = 12;
            pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            pdfTableHeader.AddCell(pdfCell);

            pdfCell = new PdfPCell(paragraph6);
            pdfCell.BorderWidth = 0;
            pdfCell.Padding = 0;
            pdfCell.PaddingTop = 12;
            pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            pdfTableHeader.AddCell(pdfCell);
            pdfTableHeader.SetWidthPercentage(new float[8] { 140f, 150f, 60f, 100f, 110f, 100f, 60f, 100f }, PageSize.A4.Rotate());
            pdfTableHeader.HorizontalAlignment = Element.ALIGN_LEFT;

            try {

                document.Open();
                document.Add(pdfTableHeader);



                for (int index = 0; index < dataTable.Columns.Count; ++index) {
                    string columnName = dataTable.Columns[index].ColumnName;
                    //columnName = columnName.Replace('[', ' ');
                    //columnName = columnName.Replace(']', ' ');
                    Phrase phrase = new Phrase(columnName, font);
                    PdfPCell cell = new PdfPCell(phrase);
                    cell.BackgroundColor = BaseColor.CYAN;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    pdfTable.AddCell(cell);
                }

                for (int i = 0; i < dataTable.Rows.Count; ++i) {
                    for (int j = 0; j < dataTable.Columns.Count; ++j) {
                        string data = dataTable.Rows[i][j].ToString();

                        if (dataTable.Rows[i][j].GetType() == typeof(DateTime)) {
                            DateTime dateTime = (DateTime)dataTable.Rows[i][j];
                            data = dateTime.ToString("yyyy/MM/dd HH:mm:ss");
                        }
                        if (dataTable.Rows[i][j].GetType() == typeof(TimeSpan)) {
                            TimeSpan timeSpan = (TimeSpan)dataTable.Rows[i][j];
                            data = timeSpan.ToString(@"dd\ hh\:mm\:ss");
                        }

                        //if (dataTable.Rows[i][j].GetType() == typeof(double)) {
                        //    double number = (double)dataTable.Rows[i][j];
                        //    data = Converter.round(number).ToString();
                        //}
                        //if (dataTable.Rows[i][j].GetType() == typeof(TimeSpan)) {
                        //    TimeSpan timeSpan = (TimeSpan)dataTable.Rows[i][j];
                        //    data = timeSpan.ToString(@"dd\ hh\:mm\:ss");
                        //}

                        Phrase phrase = new Phrase(data, font);
                        PdfPCell cell = new PdfPCell(phrase);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                        pdfTable.AddCell(cell);
                    }
                }
                document.Add(pdfTable);
                document.Close();
            } catch (Exception exception) {
                document.Close();
                throw exception;
            }
        }


    }
}
