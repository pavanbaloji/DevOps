using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.BizTalk.ExplorerOM;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.Data;
using System.IO;

namespace Avista.ESB.BuildTasks
{
    public class ExportEnvironmentSettings : Task
    {
        private string _settingsSpreadsheetPath;
        private string _masterSettingsSpreadsheetPath;
        private string _settingsFilesExportPath;
        private string _environment;
        private bool _useMasterSettings;
        public ExportEnvironmentSettings()
        {
        }

        public string SettingsSpreadsheetPath
        {
            get { return _settingsSpreadsheetPath; }
            set { _settingsSpreadsheetPath = value; }
        }

        public string MasterSettingsSpreadsheetPath
        {
            get { return _masterSettingsSpreadsheetPath; }
            set { _masterSettingsSpreadsheetPath = value; }
        }
        [Required]
        public string SettingsFilesExportPath
        {
            get { return _settingsFilesExportPath; }
            set { _settingsFilesExportPath = value; }
        }

        [Required]
        public bool UseMasterSettingsSpreadsheet
        {
            get { return _useMasterSettings; }
            set { _useMasterSettings = value; }
        }

        public string Environment
        {
            get { return _environment; }
            set { _environment = value; }
        }

        public override bool Execute()
        {
            this.Log.LogMessage("Environment Settings Spreadsheet to XML Exporter ");
            this.Log.LogMessage("Settings Spreadsheet Path: " + _settingsSpreadsheetPath);
            this.Log.LogMessage("Master Settings Spreadsheet Path: "+_masterSettingsSpreadsheetPath);
            this.Log.LogMessage("Export Path: "+_settingsFilesExportPath);
            this.Log.LogMessage("Environment: " + _environment);
            try
            {
                DataTable masterSettingsTable = null;
                DataTable settingsTable = null;
                string filename = "";

                if (!string.IsNullOrWhiteSpace(_masterSettingsSpreadsheetPath) && File.Exists(_masterSettingsSpreadsheetPath))
                {
                    this.Log.LogMessage("Importing from Master Settings Spreadsheet " + Path.GetFileName(_masterSettingsSpreadsheetPath));
                    filename += Path.GetFileName(_masterSettingsSpreadsheetPath);
                    masterSettingsTable = SettingsFileReader.ReadSettingsFromExcelFile(_masterSettingsSpreadsheetPath);
                }
                else
                {
                    if (_useMasterSettings)
                        throw new FileNotFoundException("Master Settings Spreadsheet Not Found at Path: "+_masterSettingsSpreadsheetPath);

                    this.Log.LogMessage("Has no Master Settings Spreadsheet");
                }

                if (!string.IsNullOrWhiteSpace(_settingsSpreadsheetPath) && File.Exists(_settingsSpreadsheetPath))
                {
                    this.Log.LogMessage("Importing from Settings Spreadsheet " + Path.GetFileName(_settingsSpreadsheetPath));
                    filename += Path.GetFileName(_settingsSpreadsheetPath);
                    settingsTable = SettingsFileReader.ReadSettingsFromExcelFile(_settingsSpreadsheetPath);
                }
                else
                {
                    this.Log.LogMessage("Has no Settings Spreadsheet");
                }


                if (settingsTable == null)
                {
                    settingsTable = masterSettingsTable;
                }
                else if (masterSettingsTable != null)
                {
                    for (int i = 0; i < masterSettingsTable.Rows.Count; i++)
                    {
                        if (i > 5)
                            settingsTable.ImportRow(masterSettingsTable.Rows[i]);
                    }
                }

                if (settingsTable == null)
                    throw new Exception("Settings not found in SettingsSpreadSheet or MasterSettingsSpreadSheet.");

                if (!Directory.Exists(_settingsFilesExportPath))
                {
                    Directory.CreateDirectory(_settingsFilesExportPath);
                }

                DataTableToXmlExporter exporter = new DataTableToXmlPreprocessXmlExporter();
                exporter.ExportSettings(settingsTable, _settingsFilesExportPath, filename, false, null);
            }
            catch (Exception ex)
            {
                this.Log.LogMessage("Error while exporting  Environment Settings to XML. " + ex.ToString());
            }

            return true;
        }
    }
}
