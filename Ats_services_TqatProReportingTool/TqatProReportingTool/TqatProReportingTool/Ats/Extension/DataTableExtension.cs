using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Ats.Extension {

    public static class DataTableExtensions {
        public static void writeToCsvFile(this DataTable dataTable, string filePath) {

            StringBuilder fileContent = new StringBuilder();

            foreach (var col in dataTable.Columns) {
                fileContent.Append(col.ToString() + ",");
            }

            fileContent.Replace(",", System.Environment.NewLine, fileContent.Length - 1, 1);
            foreach (DataRow datarow in dataTable.Rows) {

                foreach (var column in datarow.ItemArray) {
                    fileContent.Append("\"" + column.ToString() + "\",");
                }

                fileContent.Replace(",", System.Environment.NewLine, fileContent.Length - 1, 1);
            }

            string filename = filePath + "\\" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".csv";

            System.IO.File.WriteAllText(filename, fileContent.ToString());
        }
    }

}
