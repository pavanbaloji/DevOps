using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;

namespace Avista.ESB.BuildTasks
{
    internal static class SettingsFileReader
    {
        private const int HeaderRowsCount = 5;
        /// <summary>
        /// Read the contents of the Settings worksheet in the specified Excel file into a DataTable.
        /// Excel files up to Excel 2000 and SpreadsheetML XML 2000 files are both supported.
        /// </summary>
        /// <param name="inputFile">Path to the XLS or XML file</param>
        /// <returns></returns>
        internal static DataTable ReadSettingsFromExcelFile(string settingsSpreadSheet)
        {
            DataTable dt = null;
            string fileExtension = Path.GetExtension(settingsSpreadSheet);

            if (string.Compare(fileExtension, ".xls", true) == 0 || string.Compare(fileExtension, ".xlsx", true) == 0)
            {
                dt = ExcelBinarySettingsFileReader.ReadSettingsFromXls(settingsSpreadSheet);
            }
            else
            {
                dt = SpreadsheetMlSettingsFileReader.ReadSettingsFromSpreadsheetMl(settingsSpreadSheet);
            }

            return dt;
        }
    }
}
