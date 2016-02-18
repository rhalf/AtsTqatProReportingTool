using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using System.Globalization;

namespace Ats.Helper {
    class Converter {

        public static DateTime unixTimeStampToDateTime(double unixTimeStamp) {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
        public static double dateTimeToUnixTimestamp(DateTime dateTime) {
            return (dateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
        }
        public static string degreesToCardinal(double degrees) {
            string[] caridnals = { "N", "NE", "E", "SE", "S", "SW", "W", "NW", "N" };
            return caridnals[(int)Converter.round(((double)degrees % 360) / 45)];
        }
        public static string degreesToCardinalDetailed(double degrees) {
            degrees *= 10;

            string[] caridnals = { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW", "N" };
            return caridnals[(int)Converter.round(((double)degrees % 3600) / 225)];
        }
        public static DateTime subStandardDateTimeToDateTime(string dateTime) {
            DateTime parsedDateTime;

            if (!String.IsNullOrEmpty(dateTime)) {
                int dateTimeLength = dateTime.Length;

                if (dateTimeLength == 10) {
                    parsedDateTime = DateTime.ParseExact(dateTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                } else if (dateTimeLength == 8) {
                    parsedDateTime = DateTime.ParseExact(dateTime, "d/M/yyyy", CultureInfo.InvariantCulture);
                } else if (dateTimeLength == 9) {
                    if (dateTime.IndexOf('/') == 1) {
                        parsedDateTime = DateTime.ParseExact(dateTime, "d/MM/yyyy", CultureInfo.InvariantCulture);
                    } else {//if (dateTime.IndexOf('/') == 2) {
                        parsedDateTime = DateTime.ParseExact(dateTime, "dd/M/yyyy", CultureInfo.InvariantCulture);
                    }

                } else if (dateTimeLength == 18) {
                    if (dateTime.IndexOf('/') == 1) {
                        parsedDateTime = DateTime.ParseExact(dateTime, "d/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    } else { //if (dateTime.IndexOf('/') == 2) {
                        parsedDateTime = DateTime.ParseExact(dateTime, "dd/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    }
                } else { //(dateTimeLength == 19) {
                    parsedDateTime = DateTime.ParseExact(dateTime, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                }
            } else {
                parsedDateTime = new DateTime(2000, 01, 01);
            }

            return parsedDateTime;
        }
        public static uint getBit(uint value, int index) {
            return (value >> index) & 1;
        }
        public static uint setBit(uint value, int index, bool bit) {
            uint flag = (uint)(bit ? 1 : 0);
            uint constBit = 1;
            if (bit) {
                value |= (constBit << index);
            } else {
                value &= ~(constBit << index);
            }
            return value;
        }
        public static double dataTableColumnSumValue(DataTable dataTable, DataColumn dataColumn) {
            double summation = 0;
            foreach (DataRow dataRow in dataTable.Rows) {
                summation += (double)dataRow[dataColumn.ColumnName];
            }
            return Converter.round(summation);
        }
        public static double dataTableColumnSumValue(DataTable dataTable, string column) {
            DataColumn dataColumn = new DataColumn(column);
            double summation = 0;
            foreach (DataRow dataRow in dataTable.Rows) {
                summation += (double)dataRow[dataColumn.ColumnName];
            }
            return Converter.round(summation);
        }
        public static double dataTableColumnSumValueIfTrue(DataTable dataTable, DataColumn dataColumn) {
            double summation = 0;
            foreach (DataRow dataRow in dataTable.Rows) {
                if ((bool)dataRow["Status"] == true) {
                    summation += (double)dataRow[dataColumn.ColumnName];
                }
            }
            return Converter.round(summation);
        }
        public static double dataTableColumnSumValueIfTrue(DataTable dataTable, string column) {
            DataColumn dataColumn = new DataColumn(column);
            double summation = 0;
            foreach (DataRow dataRow in dataTable.Rows) {
                if ((bool)dataRow["Status"] == true) {
                    summation += (double)dataRow[dataColumn.ColumnName];
                }
            }
            return Converter.round(summation);
        }
        public static TimeSpan dataTableColumnSumTimeSpanIfTrue(DataTable dataTable, DataColumn dataColumn) {
            TimeSpan summationTimespan = new TimeSpan();
            foreach (DataRow dataRow in dataTable.Rows) {
                if ((bool)dataRow["Status"] == true) {
                    summationTimespan.Add((TimeSpan)dataRow[dataColumn.ColumnName]);
                }
            }
            return summationTimespan;
        }
        public static TimeSpan dataTableColumnSumTimeSpanIfTrue(DataTable dataTable, string column) {
            TimeSpan summationTimespan = new TimeSpan();
            foreach (DataRow dataRow in dataTable.Rows) {
                if ((bool)dataRow["Status"] == true) {
                    TimeSpan timeSpan = (TimeSpan)dataRow[column];
                    summationTimespan += timeSpan;
                }
            }
            return summationTimespan;
        }
        public static string dataTableColumnAppend(DataTable dataTable, DataColumn dataColumn, string delimeter) {
            string data = "";
            foreach (DataRow dataRow in dataTable.Rows) {
                string geofence = "";

                if (!Convert.IsDBNull(dataRow[dataColumn.ColumnName])) {
                    geofence = (string)dataRow[dataColumn.ColumnName];
                }

                string rowString = geofence;
                if (!String.IsNullOrEmpty(rowString))
                    data += rowString + " " + delimeter + " ";
            }
            if (data.Length > 0)
                data = data.Substring(0, data.Length - 2);
            return data;
        }
        public static string dataTableColumnAppend(DataTable dataTable, DataColumn dataColumn) {
            string delimeter = " , ";
            string data = "";
            foreach (DataRow dataRow in dataTable.Rows) {
                string geofence = "";

                if (!Convert.IsDBNull(dataRow[dataColumn.ColumnName])) {
                    geofence = (string)dataRow[dataColumn.ColumnName];
                }

                string rowString = geofence;
                if (!String.IsNullOrEmpty(rowString))
                    data += rowString + " " + delimeter + " ";
            }
            if (data.Length > 0)
                data = data.Substring(0, data.Length - 2);
            return data;
        }
        public static double dataTableColumnMaxValue(DataTable dataTable, DataColumn dataColumn) {
            double maxValue = 0;
            foreach (DataRow dataRow in dataTable.Rows) {
                double value = (double)dataRow[dataColumn.ColumnName];
                if (value > maxValue) {
                    maxValue = value;
                }
            }
            return Converter.round(maxValue);
        }
        public static double round(double number) {
            double result = Math.Round(number, 4);
            if (Double.IsNaN(result)) {
                return 0;
            }
            return result;
        }
    }
}
