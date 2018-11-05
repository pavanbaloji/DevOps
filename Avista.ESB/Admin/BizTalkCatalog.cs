using System;
using System.Collections.Generic;
//using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml;
using Microsoft.BizTalk.Deployment;
using Microsoft.BizTalk.Deployment.Binding;
using Microsoft.BizTalk.ExplorerOM;
using Microsoft.BizTalk.Management;


namespace Avista.ESB.Admin
{
      /// <summary>
      /// BizTalk Catalog explorer class
      /// </summary>
      public class BizTalkCatalog
      {
            internal Microsoft.BizTalk.ExplorerOM.BtsCatalogExplorer catalogExplorer = new BtsCatalogExplorer();
            private GroupSetting groupSettings = null;
            private string databaseName = "BizTalkMgmtDb";
            private string sqlInstanceName = "";
            private string hostName = "";
            /// <summary>
            /// Connect to the Microsoft.BizTalk.ExplorerOM.BtsCatalogExplorer
            /// </summary>
            /// <param name="sqlInstance">Biztalk Instance name</param>
            /// <param name="database">BizTalk database name</param>
            public BizTalkCatalog (string sqlInstance, string btsDatabase,string singleHost)
            {
                  databaseName = btsDatabase;
                  sqlInstanceName = sqlInstance;
                  hostName = singleHost;
                  BtsCatalogExplorer.ConnectionString = ConnectionString;
                  RetrieveGroupSettings();

            }

            /// <summary>
            /// Instance name
            /// </summary>
            public string Instance
            {
                  get
                  {
                        return sqlInstanceName;
                  }
            }

            /// <summary>
            /// Database name
            /// </summary>
            public string Database
            {
                  get
                  {
                        return databaseName;
                  }
            }

            /// <summary>
            /// Create a connection string
            /// </summary>
            private string ConnectionString
            {
                  get
                  {
                        return String.Format( "Data Source={0};Initial Catalog={1};Integrated Security=SSPI;", sqlInstanceName, databaseName );
                  }
            }

            /// <summary>
            /// Returns BizTalk Catalog explorer
            /// </summary>
            public BtsCatalogExplorer BtsCatalogExplorer
            {
                  get
                  {
                        return catalogExplorer;
                  }
            }
            /// <summary>
            /// BizTalk Host Instances collection
            /// </summary>
            public BtsHostInstanceCollection HostInstances
            {
                  get
                  {
                        return new BtsHostInstanceCollection(this );
                  }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="instance"></param>
            /// <param name="database"></param>
            /// <param name="host"></param>
            /// <returns></returns>
            public static BizTalkCatalog Connect (string instance, string database,string host)
            {
                  try
                  {
                        return new BizTalkCatalog( database, instance, host);
                  }

                  catch ( System.Exception )
                  {
                        throw;
                  }
            }

            /// <summary>
            /// Reetrieve the first WMI MSBTS_GroupSetting object in the collection
            /// </summary>
            private void RetrieveGroupSettings ()
            {
                  try
                  {
                        GroupSetting.StaticScope = ManagementHelper.GetScope( typeof( GroupSetting ), sqlInstanceName, databaseName );
                        foreach ( GroupSetting setting in GroupSetting.GetInstances() )
                        {
                              groupSettings = setting;
                              break;
                        }
                  }

                  finally
                  {
                        GroupSetting.StaticScope = null;
                  }
            }
            /// <summary>
            /// ImportBiztalkGroupSettings
            /// </summary>
            /// <param name="settingsWorker"></param>
            /// <param name="filePath"></param>
            private void ImportBizTalkGroupSettings (SettingsWorker settingsWorker, string filePath)
            {
                  SettingsRoot groupSettings = BizTalkSettings.LoadBiztalkGroupSettings( filePath );


                  try
                  {
                        ExportedSettings exportedSettings = new ExportedSettings();
                        exportedSettings.ExportedGroup = String.Format( "{0}:{1}", databaseName, databaseName );
                        exportedSettings.GroupSettings = groupSettings;
                        settingsWorker.ImportGroupSettings( exportedSettings );
                  }

                  catch ( Exception exception )
                  {
                        throw exception;
                  }
            }
            /// <summary>
            /// Import bizTalk Hosts settings
            /// </summary>
            /// <param name="settingsWorker">SettingsWorker</param>
            /// <param name="filePath">Settings file path</param>
            private void ImportBizTalkHostsSettings (SettingsWorker settingsWorker, string filePath,string hostToImport)
            {
                  HostSettings hostSettings = BizTalkSettings.LoadBiztalkHostsSettings( filePath );
                  
                  foreach ( var host in Host )
                  {
                        if ( hostToImport.Equals( host.Name.ToString()) )
                        {
                              host.ImportHostSettings( settingsWorker, hostSettings );
                        }
                  }
                        
            }
            /// <summary>
            /// ImportBizTalkHostInstancesSettings
            /// </summary>
            /// <param name="path">Settings file path</param>
            private void ImportBizTalkHostInstancesSettings (SettingsWorker settingsWorker, string path, string hostToImport )
            {
                  HostInstanceSettings hostInstanceSettings = Helper.BizTalkSettingsHelper.ParseHostInstancesSettings( path );
                  foreach ( var hostInstance in HostInstances )
                  {
                        if(hostToImport.Equals(hostInstance.HostName.ToString()))
                        {
                              hostInstance.ImportHostInstanceSettings( settingsWorker, hostInstanceSettings );
                        }
                  }
                        
            }
            /// <summary>
            /// Host collection
            /// </summary>
            public BizTalkHostCollection Host
            {
                  get
                  {
                        return new BizTalkHostCollection( this );
                  }
            }
            /// <summary>
            /// Import settings method
            /// </summary>
            /// <param name="path">settings file</param>
            public void ImportSettings (string path,string host)
            {
                  var settingsWorker = new SettingsWorker( sqlInstanceName, databaseName );
                  try
                  {
                        ImportBizTalkGroupSettings( settingsWorker, path );
                        ImportBizTalkHostsSettings( settingsWorker, path,host);
                        RetrieveGroupSettings();
                  }
                  catch ( Exception )
                  {
                        throw;
                  }
                 
            }
      }
}
