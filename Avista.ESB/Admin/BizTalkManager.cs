
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;
using Avista.ESB.Admin.Utility;
using Microsoft.BizTalk.ExplorerOM;
using portStatus = Microsoft.BizTalk.ExplorerOM.PortStatus;


namespace Avista.ESB.Admin
{
    /// <summary>
    /// Provides management functions for BizTalk Server. Some of the functions are executed using WMI and some are executed using the BtsCatalogExplorer.
    /// </summary>
    public partial class BizTalkManager
    {

        /// <summary>
        /// The computer name of the BizTalk Server being managed.
        /// </summary>
        private string bizTalkServer;

        /// <summary>
        /// The computer name of the SQL Server hosting the BizTalk Server management database.
        /// </summary>
        private string mgmtDbServer;

        /// <summary>
        /// The name of the BizTalk Server management database.
        /// </summary>
        private string mgmtDbName;

        /// <summary>
        /// The BtsCatalogExplorer used for various functions. Most operations are performed using WMI but BizTalk Application related operations are
        /// performed using the BtsCatalogExplorer because there is no WMI support for Biztalk Applications.
        /// </summary>
        private BtsCatalogExplorer catalog = null;

        /// <summary>
        /// Boolean flag that specifies whether warning conditions should be rethrown from methods or simply logged.
        /// </summary>
        private bool throwWarnings = false;

        /// <summary>
        /// Deafult constructor for a BizTalkManager. Assumes that the local machine is a BizTalk server.
        /// The BizTalkManager will operate on this BizTalk Server and the associated BizTalk group when operations are invoked.
        /// </summary>

        public BizTalkManager()
        {
            bizTalkServer = Environment.MachineName;
            LoadDatabaseInfo();
            catalog = new BtsCatalogExplorer();
            catalog.ConnectionString = String.Format("Integrated Security=SSPI;database={0};server={1}", mgmtDbName, mgmtDbServer);
        }

        /// <summary>
        /// Constructor for a BizTalkManager associated with the given BizTalk Server. The BizTalk Server could be a remote machine.
        /// The BizTalkManager will operate on this BizTalk Server and the associated BizTalk group when operations are invoked.
        /// </summary>
        /// <param name="bizTalkServer">The BizTalk Server that the BizTalkManager will operate on.</param>
        public BizTalkManager(string bizTalkServer)
        {
            this.bizTalkServer = bizTalkServer;
            LoadDatabaseInfo();
            catalog = new BtsCatalogExplorer();
            catalog.ConnectionString = String.Format("Integrated Security=SSPI;database={0};server={1}", mgmtDbName, mgmtDbServer);
        }



        /// <summary>
        /// The BizTalk server that will be managed by this BizTalkManager.
        /// </summary>
        public string BizTalkServer
        {
            get
            {
                return bizTalkServer;
            }
            set
            {
                bizTalkServer = value;
                LoadDatabaseInfo();
            }
        }


        /// <summary>
        /// The SQL Server hosting the BizTalk management database that is associated with this BizTalkManager.
        /// </summary>
        public string MgmtDbServer
        {
            get
            {
                return mgmtDbServer;
            }
        }

        /// <summary>
        /// The Name of the BizTalk Management Database.
        /// </summary>
        public string MgmtDbName
        {
            get
            {
                return mgmtDbName;
            }
        }
        public BtsCatalogExplorer Catalog
        {
            get { return catalog; }
        }


        /// <summary>
        /// Flag that indicates whether or not the BizTalkManager will throw exceptions for warning conditions. The default value is False which
        /// indicates that warning condirions will not cause exceptions to be thrown. However, the Warning exceptions will still be generated and
        /// sent to the registered exception handlers.
        /// </summary>
        public bool ThrowWarnings
        {
            get
            {
                return throwWarnings;
            }
            set
            {
                throwWarnings = value;
            }
        }

       /// <summary>
       /// Import BizTalk group settings for an instance.
       /// </summary>
       /// <param name="settingFilePath"></param>
//        public void ImportGroupSettings (string settingFilePath, string hostName,string serverName, string bizTalkMgmtDb)
//        {
//              try
//              {
//                    BizTalkCatalog biztalkCatalog = new BizTalkCatalog( serverName,bizTalkMgmtDb, hostName);
//                    biztalkCatalog.ImportSettings( settingFilePath,hostName);

//              }
//              catch ( Exception exception )
//              {
//                    string message = String.Format( "Could not BizTalk import the group settings '{0}'.", settingFilePath );
//                    ContextualException contextualException = new ContextualException( message, 413, EventLogEntryType.Error, exception );
//                    ExceptionManager.HandleException( contextualException, PolicyName.SystemException );
//                    throw contextualException;
//              }
//        }
        /// <summary>
        /// Obtains SQL server name and management database name from WMI using MSBTS_GroupSetting object.
        /// </summary>
        private void LoadDatabaseInfo ()
        {
              try
              {
                    string path = String.Format( @"\\{0}\root\MicrosoftBizTalkServer", bizTalkServer );
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher( path, "SELECT * FROM MSBTS_GroupSetting" );
                    foreach ( ManagementObject mgmtObject in searcher.Get() )
                    {
                          mgmtDbName = (string) mgmtObject[ "MgmtDbName" ];
                          mgmtDbServer = (string) mgmtObject[ "MgmtDbServerName" ];
                    }
              }
              catch ( Exception exception )
              {
                    //string message = String.Format( "Unable to obtain SQL Server and BizTalk management database name from WMI on '{0}'.", bizTalkServer );
                    //ContextualException contextualException = new ContextualException( message, 214, EventLogEntryType.Error, exception );
                    //ExceptionManager.HandleException( contextualException, PolicyName.SystemException );
                    throw exception;
              }
        }

//        /// <summary>
//        /// Checks for the existance of a BizTalk application.
//        /// </summary>
//        /// <param name="applicationName">The name of the BizTalk application.</param>
//        /// <returns>True if the named BizTalk application exists, false otherwise.</returns>
//        /// <exception cref="OperationFailedException">Thrown with EventId 490 if the operation cannot be completed due to an unexpected error condition.</exception>
//        public bool ApplicationExists(string applicationName)
//        {
//            bool exists = false;
//            try
//            {
//                Application application = catalog.Applications[applicationName];
//                if (application != null)
//                {
//                    exists = true;
//                }
//            }
//            catch (Exception exception)
//            {
//                string message = String.Format("Failed to confirm existence of application '{0}'.", applicationName);
               
////log exception
//            }
//            return exists;
//        }

//        /// <summary>
//        /// Adds a BizTalk application.
//        /// </summary>
//        /// <param name="applicationName">The name of the BizTalk application.</param>
//        /// <param name="description">A description for the BizTalk application.</param>
//        public void AddApplication(string applicationName, string description)
//        {
//            try
//            {
//                if (ApplicationExists(applicationName))
//                {
//                    string message = String.Format("The BizTalk application '{0}' already exists in BizTalk group ‘{1}’. It will not be added.", applicationName, mgmtDbServer);
//                    //NoActionWarning noActionWarning = new NoActionWarning(message, 420, EventLogEntryType.Warning);
//                    //ExceptionManager.HandleException(noActionWarning, PolicyName.SystemException);
//                    //throw noActionWarning;
//                }
//                else
//                {
//                    Application application = catalog.AddNewApplication();
//                    application.Name = applicationName;
//                    application.Description = description;
//                    catalog.SaveChanges();
//                    catalog.Refresh();
//                }
//            }
//            catch (Exception exception)
//            {
//                //string message = String.Format("Failed to add BizTalk application '{0}' to BizTalk group ‘{1}’.", applicationName, mgmtDbServer);
//                //OperationFailedException operationFailedException = new OperationFailedException(message, 421, EventLogEntryType.Error, exception);
//                //ExceptionManager.HandleException(operationFailedException, PolicyName.SystemException);
//                //throw operationFailedException;
//            }
//        }

//        /// <summary>
//        /// Removes a BizTalk application.
//        /// </summary>
//        /// <param name="applicationName">The name of the BizTalk application.</param>
//        public void RemoveApplication(string applicationName)
//        {
//            try
//            {
//                if (ApplicationExists(applicationName))
//                {
//                    Application application = catalog.Applications[applicationName];
//                    catalog.RemoveApplication(application);
//                    catalog.SaveChanges();
//                    catalog.Refresh();
//                }
//                else
//                {
//                    //string message = String.Format("The BizTalk application '{0}' does not exist in BizTalk group ‘{1}’. It cannot be removed.", applicationName, mgmtDbServer);
//                    //NoActionWarning noActionWarning = new NoActionWarning(message, 422, EventLogEntryType.Warning);
//                    //ExceptionManager.HandleException(noActionWarning, PolicyName.SystemException);
//                    //throw noActionWarning;
//                }
//            }
//            catch (Exception exception)
//            {
//                //string message = String.Format("Failed to delete BizTalk application '{0}' from BizTalk group ‘{1}’.", applicationName, mgmtDbServer);
//                //OperationFailedException operationFailedException = new OperationFailedException(message, 423, EventLogEntryType.Error, exception);
//                //ExceptionManager.HandleException(operationFailedException, PolicyName.SystemException);
//                  throw exception;
//            }
//        }

//        /// <summary>
//        /// Adds an application reference to a BizTalk application.
//        /// </summary>
//        /// <param name="applicationName">The name of the application having a reference added.</param>
//        /// <param name="referenceName">The name of the application being referenced.</param>
//        public void AddApplicationReference(string applicationName, string referenceName)
//        {
//            try
//            {
//                Application application = catalog.Applications[applicationName];
//                if (application != null)
//                {
//                    Application referencedApplication = catalog.Applications[referenceName];
//                    if (referencedApplication != null)
//                    {
//                        application.AddReference(referencedApplication);
//                        catalog.SaveChanges();
//                        catalog.Refresh();
//                        ColorConsole.Info("A reference to " + referenceName + " has been added to " + applicationName + ".");
//                    }
//                    else
//                    {
//                        ColorConsole.Warning("The " + referenceName + " BizTalk application could not be found, therefore it cannot be referenced.");
//                    }
//                }
//                else
//                {
//                    ColorConsole.Warning("The " + applicationName + " BizTalk application could not be found, therefore a reference cannot be added to it.");
//                }
//            }
//            catch (Exception e)
//            {
//                throw new Exception("Failed to add BizTalk application reference to " + referenceName + " for " + applicationName + ". " + e.Message);
//            }
//        }

//        /// <summary>
//        /// Removes an application reference from a BizTalk application.
//        /// </summary>
//        /// <param name="applicationName">The name of the application having a reference removed.</param>
//        /// <param name="referenceName">The name of the application reference being removed.</param>
//        public void RemoveApplicationReference(string applicationName, string referenceName)
//        {
//            try
//            {
//                Application application = catalog.Applications[applicationName];
//                if (application != null)
//                {
//                    Application referencedApplication = catalog.Applications[referenceName];
//                    if (referencedApplication != null)
//                    {
//                        application.RemoveReference(referencedApplication);
//                        catalog.SaveChanges();
//                        catalog.Refresh();
//                        ColorConsole.Info("A reference to " + referenceName + " has been remove from " + applicationName + ".");
//                    }
//                    else
//                    {
//                        ColorConsole.Warning("The " + referenceName + " BizTalk application could not be found, therefore it cannot be dereferenced.");
//                    }
//                }
//                else
//                {
//                    ColorConsole.Warning("The " + applicationName + " BizTalk application could not be found, therefore a reference cannot be removed from it.");
//                }
//            }
//            catch (Exception e)
//            {
//                throw new Exception("Failed to remove BizTalk application reference to " + referenceName + " from " + applicationName + ". " + e.Message);
//            }
//        }

//        /// <summary>
//        /// Starts a BizTalk Application.
//        /// </summary>
//        /// <param name="applicationName">The name of the BizTalk application to be started.</param>
//        public void StartApplication(string applicationName)
//        {
//            try
//            {
//                Application application = catalog.Applications[applicationName];
//                if (application != null)
//                {
//                    try
//                    {
//                        application.Start(ApplicationStartOption.StartAll);
//                        catalog.SaveChanges();
//                        catalog.Refresh();
//                    }
//                    catch (Exception e)
//                    {
//                        ColorConsole.Warning("The " + applicationName + " BizTalk application could not be started.");
//                        ColorConsole.Warning("Exception: " + e.Message);
//                        ColorConsole.Warning("Attempting to start the application ports and orchestrations individually...");
//                        foreach (SendPort sendPort in application.SendPorts)
//                        {
//                            EnlistSendPort(sendPort.Name);
//                            StartSendPort(sendPort.Name);
//                        }
//                        foreach (ReceivePort receivePort in application.ReceivePorts)
//                        {
//                            foreach (ReceiveLocation receiveLocation in receivePort.ReceiveLocations)
//                            {
//                                EnableReceiveLocation(receiveLocation.Name);
//                            }
//                        }
//                        foreach (BtsOrchestration orchestration in application.Orchestrations)
//                        {
//                            EnlistOrchestration(orchestration.FullName);
//                            StartOrchestration(orchestration.FullName);
//                        }
//                    }
//                    ColorConsole.Info("The " + applicationName + " BizTalk application has been started.");
//                }
//                else
//                {
//                    ColorConsole.Warning("The " + applicationName + " BizTalk application could not be found, therefore it cannot be started.");
//                }
//            }
//            catch (Exception e)
//            {
//                throw new Exception("Failed to start the " + applicationName + " BizTalk application. " + e.Message);
//            }
//        }

//        /// <summary>
//        /// This will stop an application
//        /// </summary>
//        /// <param name="applicationName">The name of the application</param>
//        public void StopApplication(string applicationName)
//        {
//            Application app = catalog.Applications[applicationName];
//            app.Stop(ApplicationStopOption.StopAll);
//            catalog.SaveChanges();
//            catalog.Refresh();
//        }

//        /// <summary>
//        /// Adds a BizTalk host.
//        /// </summary>
//        /// <param name="hostName">The name of the BizTalk host.</param>
//        /// <param name="ntGroupName">The Windows security group to be associated with the host.</param>
//        /// <param name="isDefault">Flag indicating if this is to be the default host for the BizTalk group.</param>
//        /// <param name="hostTracking">Flag indicating if this host will perform tracking.</param>
//        /// <param name="authTrusted">Flag indicating if the host will be trusted.</param>
//        /// <param name="hostType">The type of host. 1=In Process, 2=Isolated</param>
//        /// <param name="isHost32BitOnly">Flag indicating if the host is to have only 32-bit host instances.</param>
//        public void AddHost(
//            string hostName,
//            string ntGroupName,
//            bool isDefault,
//            bool hostTracking,
//            bool authTrusted,
//            Avista.ESB.Admin.HostType hostType,
//            bool hostIs32BitOnly
//            )
//        {
//            try
//            {
//                ManagementClass hostSetting = new ManagementClass("root\\MicrosoftBizTalkServer", "MSBTS_HostSetting", new ObjectGetOptions());
//                ManagementObject host = hostSetting.CreateInstance();
//                host["Name"] = hostName;
//                host["NTGroupName"] = ntGroupName;
//                host["IsDefault"] = isDefault;
//                host["HostTracking"] = hostTracking;
//                host["AuthTrusted"] = authTrusted;
//                host["HostType"] = (int)hostType;
//                host["IsHost32BitOnly"] = hostIs32BitOnly;
//                PutOptions options = new PutOptions();
//                options.Type = PutType.CreateOnly;
//                host.Put(options);
//            }
//            catch (Exception e)
//            {
//                throw new Exception("Failed to add BizTalk host " + hostName + ". " + e.Message);
//            }
//        }

        /// <summary>
        /// Adds a BizTalk host.
        /// </summary>
        /// <param name="hostName">The name of the BizTalk host.</param>
        /// <param name="ntGroupName">The Windows security group to be associated with the host.</param>
        /// <param name="isDefault">Flag indicating if this is to be the default host for the BizTalk group.</param>
        /// <param name="hostTracking">Flag indicating if this host will perform tracking.</param>
        /// <param name="authTrusted">Flag indicating if the host will be trusted.</param>
        /// <param name="hostType">The type of host. 1=In Process, 2=Isolated</param>
        /// <param name="isHost32BitOnly">Flag indicating if the host is to have only 32-bit host instances.</param>
        /// <param name="dbQueueSizeThreshold">Total number of messages published by the host instance to the work, state, and suspended queues of the subscribing hosts.</param>
        /// <param name="processMemoryThreshold">Maximum process memory (in percent) allowed before throttling begins (in percent or megabytes).</param>
        /// <param name="throttlingSpoolMultiplier">Factor by which the Message count in DB threshold is multiplied and then compared against the current record count in the spool table.</param>
        //public void AddHost(
        //    string hostName, 
        //    string ntGroupName, 
        //    bool isDefault, 
        //    bool hostTracking, 
        //    bool authTrusted, 
        //    Avista.ESB.Admin.HostType hostType, 
        //    bool hostIs32BitOnly,
        //    Int32 dbQueueSizeThreshold,
        //    Int32 processMemoryThreshold,
        //    Int16 throttlingSpoolMultiplier
        //    )
        //{
        //    try
        //    {
        //        ManagementClass hostSetting = new ManagementClass("root\\MicrosoftBizTalkServer", "MSBTS_HostSetting", new ObjectGetOptions());
        //        ManagementObject host = hostSetting.CreateInstance();
        //        host["Name"] = hostName;
        //        host["NTGroupName"] = ntGroupName;
        //        host["IsDefault"] = isDefault;
        //        host["HostTracking"] = hostTracking;
        //        host["AuthTrusted"] = authTrusted;
        //        host["HostType"] = (int)hostType;
        //        host["IsHost32BitOnly"] = hostIs32BitOnly;
        //        host["DBQueueSizeThreshold"] = dbQueueSizeThreshold;
        //        host["ProcessMemoryThreshold"] = processMemoryThreshold;
        //        host["ThrottlingSpoolMultiplier"] = throttlingSpoolMultiplier;
        //        PutOptions options = new PutOptions();
        //        options.Type = PutType.CreateOnly;
        //        host.Put(options);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("Failed to add BizTalk host " + hostName + ". " + e.Message);
        //    }
        //}

        ///// <summary>
        ///// Removes a BizTalk host.
        ///// </summary>
        ///// <param name="hostName">The name of the BizTalk host.</param>
        //public void RemoveHost(string hostName)
        //{
        //    try
        //    {
        //        ManagementClass hostSetting = new ManagementClass("root\\MicrosoftBizTalkServer", "MSBTS_HostSetting", new ObjectGetOptions());
        //        if (Wmi.ObjectExists(hostSetting, "Name", hostName))
        //        {
        //            ManagementObject host = hostSetting.CreateInstance();
        //            host["Name"] = hostName;
        //            host.Delete();
        //        }
        //        else
        //        {
        //            ColorConsole.Warning("The " + hostName + " BizTalk host could not be found, therefore it cannot be removed.");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("Failed to remove the " + hostName + " BizTalk host. " + e.Message);
        //    }
        //}

        //public void StartHost(string hostName, string machineName, int waitTimeSec = 30)
        //{
        //    // Find the management object representing the BizTalk Host.
        //    ManagementObject host = Wmi.LoadObject(machineName, "root\\MicrosoftBizTalkServer", "MSBTS_Host", "Name", hostName);
        //    if (host != null)
        //    {
        //        // Execute the Start operation and Wait until the operation has completed or times out.
        //        WmiOperationCompletedEventHandler handler = new WmiOperationCompletedEventHandler();
        //        host.InvokeMethod(handler.Observer, "Start", null);
        //        handler.WaitForCompletion(waitTimeSec);
        //    }
        //    else
        //    {
        //        throw new Exception("The host " + hostName + " could not be found on " + machineName + ".");
        //    }
        //}

        //public void StopHost(string hostName, string machineName, int waitTimeSec = 30)
        //{
        //    // Find the management object representing the BizTalk Host.
        //    ManagementScope mgmtScope = new ManagementScope(string.Format(@"\\{0}\root\MicrosoftBizTalkServer", machineName));
        //    mgmtScope.Connect();
        //    ManagementPath mgmtPath = new ManagementPath("MSBTS_Host");
        //    ObjectGetOptions options = new ObjectGetOptions();
        //    ManagementClass mgmtClass = new ManagementClass(mgmtScope, mgmtPath, options);
        //    if (Wmi.ObjectExists(mgmtClass, "Name", hostName))
        //    {
        //        // Get the management object instance and execute the Start method.
        //        ManagementObject host = mgmtClass.CreateInstance();
        //        host["Name"] = hostName;
        //        WmiOperationCompletedEventHandler handler = new WmiOperationCompletedEventHandler();
        //        host.InvokeMethod(handler.Observer, "Stop", null);
        //        // Wait until the operation has completed or we time out.
        //        handler.WaitForCompletion(waitTimeSec);
        //    }
        //    else
        //    {
        //        throw new Exception("The host " + hostName + " could not be found on " + machineName + ".");
        //    }
        //}

        public int GetHostInstanceCount(string hostName)
        {
            int hostInstanceCount = 0;
            try
            {
                string query = "Select * from MSBTS_HostInstance where HostName='" + hostName + "'";
                ManagementObjectSearcher searchObject = new ManagementObjectSearcher("root\\MicrosoftBizTalkServer", query);

                ManagementObjectCollection hostCollection = searchObject.Get();

                if (hostCollection != null && hostCollection.Count > 0)
                {
                    hostInstanceCount = hostCollection.Count;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Failed to retrieve host instance count of the " + hostName + " BizTalk host. " + e.Message);
            }

            return hostInstanceCount;
        }

        public bool HostInstanceExists(string hostName)
        {
            throw new NotImplementedException();
        }

        public ServiceStatus HostInstanceStatus(string hostName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a host instance mapping and installs the host instance service.
        /// </summary>
        /// <param name="hostName">The host name.</param>
        /// <param name="userId">The user id of the service account used for the host instance.</param>
        /// <param name="password">The password of the service account used for the host instance.</param>
        /// <param name="startInstance">A flag indicating if the service instance should be started after it is installed.</param>
        //public void AddHostInstance(string hostName, string userId, string password, bool startInstance)
        //{
        //    try
        //    {
        //        ManagementClass serverHostClass = new ManagementClass("root\\MicrosoftBizTalkServer", "MSBTS_ServerHost", new ObjectGetOptions());
        //        ManagementObject serverHost = serverHostClass.CreateInstance();
        //        serverHost["ServerName"] = bizTalkServer;
        //        serverHost["HostName"] = hostName;
        //        serverHost.InvokeMethod("Map", null);
        //        ColorConsole.Info("Mapped host instance for " + hostName + " on server " + bizTalkServer + ".");
        //        ManagementClass hostInstanceClass = new ManagementClass("root\\MicrosoftBizTalkServer", "MSBTS_HostInstance", new ObjectGetOptions());
        //        ManagementObject hostInstance = hostInstanceClass.CreateInstance();
        //        hostInstance["Name"] = "Microsoft BizTalk Server " + hostName + " " + bizTalkServer;
        //        object[] parms = new object[3];
        //        parms[0] = userId;
        //        parms[1] = password;
        //        parms[2] = true;
        //        hostInstance.InvokeMethod("Install", parms);
        //        if (startInstance)
        //        {
        //            hostInstance.InvokeMethod("Start", null);
        //            ColorConsole.Info("Started host instance for " + hostName + " on server " + bizTalkServer + ".");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("Failed to add host instance for " + hostName + " on server " + bizTalkServer + ". " + e.Message);
        //    }
        //}


        /// <summary>
        /// Removes a host instance for a host from a given server.
        /// </summary>
        /// <param name="servername">The server name.</param>
        /// <param name="hostname">The host name.</param>
        //public void RemoveHostInstance(string hostname)
        //{
        //    try
        //    {
        //        string hostInstanceName = "Microsoft BizTalk Server " + hostname + " " + bizTalkServer;
        //        ManagementClass hostInstClass = Wmi.LoadClass(bizTalkServer, "root\\MicrosoftBizTalkServer", "MSBTS_HostInstance");
        //        EnumerationOptions enumOptions = new EnumerationOptions();
        //        enumOptions.ReturnImmediately = false;
        //        ManagementObjectCollection hostInstCollection = hostInstClass.GetInstances(enumOptions);
        //        ManagementObject hostInstance = null;
        //        foreach (ManagementObject inst in hostInstCollection)
        //        {
        //            if (inst["Name"] != null)
        //            {
        //                if (inst["Name"].ToString().ToUpper() == hostInstanceName.ToUpper())
        //                {
        //                    hostInstance = inst;
        //                }
        //            }
        //        }
        //        if (hostInstance == null)
        //            return;

        //        if (hostInstance["HostType"].ToString() != "2" && hostInstance["ServiceState"].ToString() == "4")
        //        {
        //            hostInstance.InvokeMethod("Stop", null);
        //        }
        //        //Now UnInstall the HostInstance
        //        hostInstance.InvokeMethod("UnInstall", null);
        //        //Create an instance of the ServerHost class using the System.Management namespace
        //        ManagementClass svrHostClass = new ManagementClass("root\\MicrosoftBizTalkServer", "MSBTS_ServerHost", new ObjectGetOptions());
        //        ManagementObject svrHostObject = svrHostClass.CreateInstance();
        //        //Set the properties of the ServerHost instance
        //        svrHostObject["ServerName"] = bizTalkServer;
        //        svrHostObject["HostName"] = hostname;
        //        //Invoke the UnMap method of the ServerHost object
        //        svrHostObject.InvokeMethod("UnMap", null);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("" + e.Message);
        //    }
        //}

        public void StartHostInstance(string hostName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stops a host instance.
        /// </summary>
        /// <param name="hostName">The host name.</param>
        public void StopHostInstance(string hostName, int waitTime = 60)
        {
            try
            {
                ManagementObject hostInstance = Wmi.LoadObject(bizTalkServer, "root\\MicrosoftBizTalkServer", "MSBTS_HostInstance", "ServerName", bizTalkServer, "HostName", hostName);
                hostInstance.InvokeMethod("Stop", null);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to start host instance for " + hostName + " on server " + bizTalkServer + ". " + e.Message);
            }
        }

        /// <summary>
        /// Adds an adapter mapping to a BizTalk host.
        /// </summary>
        /// <param name="adapterName"></param>
        /// <param name="adapterClsid"></param>
        /// <param name="comment"></param>
        //public void AddAdapter(string adapterName, string adapterClsid, string comment)
        //{
        //    try
        //    {
        //        ManagementObject adapter = null;
        //        ManagementClass adapterClass = new ManagementClass("root\\MicrosoftBizTalkServer", "MSBTS_AdapterSetting", new ObjectGetOptions());
        //        EnumerationOptions enumOptions = new EnumerationOptions();
        //        enumOptions.ReturnImmediately = false;
        //        ManagementObjectCollection adapterCollection = adapterClass.GetInstances(enumOptions);
        //        foreach (ManagementObject adapterInstance in adapterCollection)
        //        {
        //            if (adapterInstance["Name"] != null)
        //            {
        //                if (adapterInstance["Name"].ToString().ToUpper() == adapterName.ToUpper())
        //                {
        //                    adapter = adapterInstance;
        //                }
        //            }
        //        }
        //        if (adapter == null)
        //        {
        //            adapter = adapterClass.CreateInstance();
        //            adapter.SetPropertyValue("Name", adapterName);
        //            adapter.SetPropertyValue("MgmtCLSID", adapterClsid);
        //            adapter.SetPropertyValue("Comment", comment);
        //        }
        //        try
        //        {
        //            PutOptions options = new PutOptions();
        //            options.Type = PutType.UpdateOrCreate;
        //            adapter.Put(options);
        //        }
        //        catch (Exception)
        //        {
        //            return;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("" + e.Message);
        //    }
        //}

        /// <summary>
        /// Removes an adapter mapping from a BizTalk host.
        /// </summary>
        /// <param name="name"></param>
        //public void RemoveAdapter(string name)
        //{
        //    try
        //    {
        //        ManagementObject objInstance = null;
        //        ManagementClass objClass = new ManagementClass("root\\MicrosoftBizTalkServer", "MSBTS_AdapterSetting", new ObjectGetOptions());
        //        //Leave the adapter if there are other instances depending on it.
        //        if (objClass.GetInstances().Count > 0)
        //            return;
        //        objInstance = objClass.CreateInstance();
        //        objInstance.SetPropertyValue("Name", name);
        //        if (Wmi.ObjectExists(objClass, "Name", name))
        //        {
        //            objInstance.Delete();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("" + e.Message);
        //    }
        //}

        /// <summary>
        /// Adds a receive handler for a given adapter to a BizTalk host.
        /// </summary>
        /// <param name="hostName">The name of the BizTalk host.</param>
        /// <param name="adapterName">The name of the adapter.</param>
        //public void AddReceiveHandler(string hostName, string adapterName)
        //{
        //    try
        //    {
        //        ManagementClass receiveHandlerClass = new ManagementClass("root\\MicrosoftBizTalkServer", "MSBTS_ReceiveHandler", null);
        //        if (Wmi.ObjectExists(receiveHandlerClass, "HostName", hostName, "AdapterName", adapterName))
        //        {
        //            ColorConsole.Warning("A receive handler for the " + adapterName + " adapter already exists for the " + hostName + " BizTalk host, therefore it will not be added.");
        //        }
        //        else
        //        {
        //            ManagementObject receiveHandler = receiveHandlerClass.CreateInstance();
        //            receiveHandler["AdapterName"] = adapterName;
        //            receiveHandler["HostName"] = hostName;
        //            PutOptions options = new PutOptions();
        //            options.Type = PutType.CreateOnly;
        //            receiveHandler.Put(options);
        //            ColorConsole.Info("A receive handler for the " + adapterName + " adapter has been added to the " + hostName + " BizTalk host.");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("Failed to add receive handler for the " + adapterName + " adapter to the " + hostName + " BizTalk host. " + e.Message);
        //    }
        //}

        /// <summary>
        /// Removes a receive handler for a given adapter from a BizTalk host.
        /// </summary>
        /// <param name="hostName">The name of the BizTalk host.</param>
        /// <param name="adapterName">The name of the adapter.</param>
        //public void RemoveReceiveHandler(string hostName, string adapterName)
        //{
        //    try
        //    {
        //        ManagementClass receiveHandlerClass = Wmi.LoadClass(bizTalkServer, "root\\MicrosoftBizTalkServer", "MSBTS_ReceiveHandler");
        //        if (Wmi.ObjectExists(receiveHandlerClass, "HostName", hostName, "AdapterName", adapterName))
        //        {
        //            ManagementObject receiveHandler = receiveHandlerClass.CreateInstance();
        //            receiveHandler.SetPropertyValue("AdapterName", adapterName);
        //            receiveHandler.SetPropertyValue("HostName", hostName);
        //            receiveHandler.Delete();
        //            ColorConsole.Info("The receive handler for the " + adapterName + " adapter has been removed from the " + hostName + " BizTalk host.");
        //        }
        //        else
        //        {
        //            ColorConsole.Warning("A receive handler for the " + adapterName + " adapter could not be found for the " + hostName + " BizTalk host, therefore it could not be removed");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("Failed to remove the receive handler for the " + adapterName + " adapter from the " + hostName + " BizTalk host. " + e.Message);
        //    }
        //}

        /// <summary>
        /// Adds a send handler for a given adapter to a given BizTalk host.
        /// </summary>
        /// <param name="hostName">The name of the BizTalk host.</param>
        /// <param name="adapterName">The name of the adpater.</param>
        //public void AddSendHandler(string hostName, string adapterName)
        //{
        //    try
        //    {
        //        // Create a management class object and spawn a management object instance
        //        ManagementClass sendHandlerClass = new ManagementClass("root\\MicrosoftBizTalkServer", "MSBTS_SendHandler2", null);
        //        if (Wmi.ObjectExists(sendHandlerClass, "HostName", hostName, "AdapterName", adapterName))
        //        {
        //            ColorConsole.Warning("A send handler for the " + adapterName + " adapter already exists for " + hostName + " BizTalk host, therefore it will not be added.");
        //        }
        //        else
        //        {
        //            ManagementObject sendHandler = sendHandlerClass.CreateInstance();
        //            // Set the properties for the management object
        //            sendHandler["AdapterName"] = adapterName;
        //            sendHandler["HostName"] = hostName;
        //            // Create the management object
        //            PutOptions options = new PutOptions();
        //            options.Type = PutType.CreateOnly;
        //            sendHandler.Put(options);
        //            ColorConsole.Info("A send handler for the " + adapterName + " adapter has been added for the " + hostName + " BizTalk host.");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("Failed to add send handler for the " + adapterName + " adapter to the " + hostName + " BizTalk host. " + e.Message);
        //    }
        //}

        /// <summary>
        /// Removes a send handler for a given adapter from a given BizTalk host.
        /// </summary>
        /// <param name="hostName">The name of the BizTalk host.</param>
        /// <param name="adapterName">The name of the adpater.</param>
        //public void RemoveSendHandler(string hostName, string adapterName)
        //{
        //    try
        //    {
        //        ManagementClass sendHandlerClass = new ManagementClass("root\\MicrosoftBizTalkServer", "MSBTS_SendHandler2", null);
        //        if (Wmi.ObjectExists(sendHandlerClass, "HostName", hostName, "AdapterName", adapterName))
        //        {
        //            ManagementObject sendHandler = sendHandlerClass.CreateInstance();
        //            sendHandler.SetPropertyValue("AdapterName", adapterName);
        //            sendHandler.SetPropertyValue("HostName", hostName);
        //            sendHandler.Delete();
        //            ColorConsole.Info("The send handler for the " + adapterName + " adapter has been removed from the " + hostName + " BizTalk host.");
        //        }
        //        else
        //        {
        //            ColorConsole.Warning("A send handler for the " + adapterName + " adapter could not be found for the " + hostName + " BizTalk host, therefore it could not be removed");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("Failed to remove the send handler for the " + adapterName + " adapter from the " + hostName + " BizTalk host. " + e.Message);
        //    }
        //}

        /// <summary>
        /// Checks for the existence of a receive port.
        /// </summary>
        /// <param name="receivePortName">The name of the receive port.</param>
        /// <returns>True if the receive port exists. False if the receive port does not exist.</returns>
        /// <exception cref="OperationFailedException">Thrown with EventId 493 if the operation cannot be completed due to an unexpected error condition.</exception>
        //public bool ReceivePortExists(string receivePortName)
        //{
        //    bool exists = false;
        //    try
        //    {
        //        ManagementClass mgmtClass = Wmi.LoadClass(Environment.MachineName, "root\\MicrosoftBizTalkServer", "MSBTS_ReceivePort");
        //        exists = Wmi.ObjectExists(mgmtClass, "Name", receivePortName);
        //    }
        //    catch (Exception exception)
        //    {
        //        //string message = String.Format("Failed to confirm existence of receive port '{0}'.", receivePortName);
        //        //OperationFailedException operationFailedException = new OperationFailedException(message, 494, EventLogEntryType.Error, exception);
        //        //ExceptionManager.HandleException(operationFailedException, PolicyName.SystemException);
        //        //throw operationFailedException;
        //    }
        //    return exists;
        //}

        /// <summary>
        /// Checks for the existence of a receive location.
        /// </summary>
        /// <param name="receiveLocationName">The name of the receive location.</param>
        /// <returns>True if the receive location exists. False if the receive location does not exist.</returns>
        /// <exception cref="OperationFailedException">Thrown with EventId 494 if the operation cannot be completed due to an unexpected error condition.</exception>
        //public bool ReceiveLocationExists(string receiveLocationName)
        //{
        //    bool exists = false;
        //    try
        //    {
        //        ManagementClass mgmtClass = Wmi.LoadClass(Environment.MachineName, "root\\MicrosoftBizTalkServer", "MSBTS_ReceiveLocation");
        //        exists = Wmi.ObjectExists(mgmtClass, "Name", receiveLocationName);
        //    }
        //    catch (Exception exception)
        //    {
        //        //string message = String.Format("Failed to confirm existence of receive location '{0}'.", receiveLocationName);
        //        //OperationFailedException operationFailedException = new OperationFailedException(message, 494, EventLogEntryType.Error, exception);
        //        //ExceptionManager.HandleException(operationFailedException, PolicyName.SystemException);
        //        //throw operationFailedException;
        //    }
        //    return exists;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiveLocationName">The name of the receive location.</param>
        /// <returns>True if the receive location is disabled. False if the receive location is enabled.</returns>
        /// <exception cref="OperationFailedException">Thrown with EventId 494 if the operation cannot be completed due to an unexpected error condition.</exception>
        //public bool ReceiveLocationIsDisabled(string receiveLocationName)
        //{
        //    bool isDisabled = false;
        //    try
        //    {
        //        if (ReceiveLocationExists(receiveLocationName))
        //        {
        //            ManagementObject receiveLocation = Wmi.LoadObject(bizTalkServer, "root\\MicrosoftBizTalkServer", "MSBTS_ReceiveLocation", "Name", receiveLocationName);
        //            isDisabled = (bool)(receiveLocation["IsDisabled"]);
        //        }
        //        else
        //        {
        //            throw new Exception("The receive location could not be found.");
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        //string message = String.Format("Could not get status of receive location '{0}'.", receiveLocationName);
        //        //OperationFailedException operationFailedException = new OperationFailedException(message, 412, EventLogEntryType.Error, exception);
        //        //ExceptionManager.HandleException(operationFailedException, PolicyName.SystemException);
        //        //throw operationFailedException;
        //    }
        //    return isDisabled;
        //}

        /// <summary>
        /// Enables a receive location.
        /// </summary>
        /// <param name="receiveLocationName">The name of the receive location.</param>
        /// <param name="waitTimeSec">The maximum amount of time that we will wait for the operation to complete. The default is 30 seconds.</param>
        /// <exception cref="NoActionWarning">Thrown as a Warning with EventId 458 if the receive location was already enabled.</exception>
        /// <exception cref="OperationFailedException">Thrown as an Error with EventId 459 if there was an error enabling the receive location.</exception>
        //public void EnableReceiveLocation(string receiveLocationName, int waitTimeSec = 30)
        //{
        //    try
        //    {
        //        if (!ReceiveLocationIsDisabled(receiveLocationName))
        //        {
        //            //string message = String.Format("The receive location '{0}' is already enabled.", receiveLocationName);
        //            //NoActionWarning noActionWarning = new NoActionWarning(message, 458, EventLogEntryType.Warning);
        //            //ExceptionManager.HandleException(noActionWarning, PolicyName.SystemException);
        //            //throw noActionWarning;
        //        }
        //        else
        //        {
        //            ManagementObject receiveLocation = Wmi.LoadObject(bizTalkServer, "root\\MicrosoftBizTalkServer", "MSBTS_ReceiveLocation", "Name", receiveLocationName);
        //            WmiOperationCompletedEventHandler handler = new WmiOperationCompletedEventHandler();
        //            receiveLocation.InvokeMethod(handler.Observer, "Enable", null, null);
        //            handler.WaitForCompletion(waitTimeSec);
        //        }
        //    }
        //    catch ( NoActionWarning )
        //    {
        //          if ( throwWarnings )
        //          {
        //                throw;
        //          }
        //    }
        //    catch (Exception exception)
        //    {
        //        //string message = String.Format("Failed to enable receive location '{0}'.", receiveLocationName);
        //        //OperationFailedException operationFailedException = new OperationFailedException(message, 459, EventLogEntryType.Error, exception);
        //        //ExceptionManager.HandleException(operationFailedException, PolicyName.SystemException);
        //        //throw operationFailedException;
        //    }
        //}

        /// <summary>
        /// Disables a receive location.
        /// </summary>
        /// <param name="receiveLocationName">The name of the receive location.</param>
        /// <param name="waitTimeSec">The maximum amount of time that we will wait for the operation to complete. The default is 30 seconds.</param>
        /// <exception cref="NoActionWarning">Thrown as a Warning with EventId 460 if the receive location was already disabled.</exception>
        /// <exception cref="OperationFailedException">Thrown as an Error with EventId 461 if there was an error disabling the receive location.</exception>
        //public void DisableReceiveLocation(string receiveLocationName, int waitTimeSec = 30)
        //{
        //    try
        //    {
        //        if (ReceiveLocationIsDisabled(receiveLocationName))
        //        {
        //            //string message = String.Format("The receive location '{0}' is already disabled.", receiveLocationName);
        //            //NoActionWarning noActionWarning = new NoActionWarning(message, 460, EventLogEntryType.Warning);
        //            //ExceptionManager.HandleException(noActionWarning, PolicyName.SystemException);
        //            //throw noActionWarning;
        //        }
        //        else
        //        {
        //            ManagementObject receiveLocation = Wmi.LoadObject(bizTalkServer, "root\\MicrosoftBizTalkServer", "MSBTS_ReceiveLocation", "Name", receiveLocationName);
        //            WmiOperationCompletedEventHandler handler = new WmiOperationCompletedEventHandler();
        //            receiveLocation.InvokeMethod(handler.Observer, "Disable", null, null);
        //            handler.WaitForCompletion(waitTimeSec);
        //        }
        //    }
        //    //catch (NoActionWarning)
        //    //{
        //    //    if (throwWarnings)
        //    //    {
        //    //        throw;
        //    //    }
        //    //}
        //    catch (Exception exception)
        //    {
        //        //string message = String.Format("Failed to disable receive location '{0}'.", receiveLocationName);
        //        //OperationFailedException operationFailedException = new OperationFailedException(message, 461, EventLogEntryType.Error, exception);
        //        //ExceptionManager.HandleException(operationFailedException, PolicyName.SystemException);
        //        //throw operationFailedException;
        //    }
        //}

        /// <summary>
        /// Checks for the existence of a send port.
        /// </summary>
        /// <param name="sendPortName">The name of the send port.</param>
        /// <returns>True if the send port exists. False if the send port does no exist.</returns>
        /// <exception cref="OperationFailedException">Thrown with EventId 492 if the operation cannot be completed due to an unexpected error condition.</exception>
        public bool SendPortExists(string sendPortName)
        {
            bool exists = false;
            try
            {
                ManagementClass mgmtClass = Wmi.LoadClass(Environment.MachineName, "root\\MicrosoftBizTalkServer", "MSBTS_SendPort");
                exists = Wmi.ObjectExists(mgmtClass, "Name", sendPortName);
            }
            catch (Exception exception)
            {
                //string message = String.Format("Failed to confirm existence of send port '{0}'.", sendPortName);
                //OperationFailedException operationFailedException = new OperationFailedException(message, 492, EventLogEntryType.Error, exception);
                //ExceptionManager.HandleException(operationFailedException, PolicyName.SystemException);
                //throw operationFailedException;
            }
            return exists;
        }

        /// <summary>
        /// Gets the current status of a send port.
        /// </summary>
        /// <param name="sendPortName">The name of the send port.</param>
        /// <returns>The port status.</returns>
        /// <exception cref="OperationFailedException">Thrown with EventId 411 if the operation cannot be completed due to an unexpected error condition.</exception>
        public PortStatus SendPortStatus(string sendPortName)
        {
            PortStatus status = PortStatus.Unknown;
            try
            {
                if (SendPortExists(sendPortName))
                {
                    ManagementObject sendPort = Wmi.LoadObject(bizTalkServer, "root\\MicrosoftBizTalkServer", "MSBTS_SendPort", "Name", sendPortName);
                    status = (PortStatus)(sendPort["Status"]);
                }
                else
                {
                    throw new Exception("The send port could not be found.");
                }
            }
            catch (Exception exception)
            {
                //string message = String.Format("Could not get status of send port '{0}'.", sendPortName);
                //OperationFailedException operationFailedException = new OperationFailedException(message, 411, EventLogEntryType.Error, exception);
                //ExceptionManager.HandleException(operationFailedException, PolicyName.SystemException);
                //throw operationFailedException;
            }
            return status;
        }

        /// <summary>
        /// Enlists a send port.
        /// </summary>
        /// <param name="sendPortName">The name of the send port.</param>
        /// <param name="waitTimeSec">The maximum amount of time that we will wait for the operation to complete. The default is 30 seconds.</param>
        /// <exception cref="NoActionWarning">Thrown as a Warning with EventId 466 if the send port was already enlisted.</exception>
        /// <exception cref="OperationFailedException">Thrown as an Error with EventId 467 if there was an error enlisting the send port.</exception>
        public void EnlistSendPort(string sendPortName, int waitTimeSec = 30)
        {
            try
            {
                PortStatus portStatus = SendPortStatus(sendPortName);
                if (portStatus == PortStatus.Enlisted_And_Stopped || portStatus == PortStatus.Started)
                {
                    //string message = String.Format("The send port '{0}' is already enlisted.", sendPortName);
                    //NoActionWarning noActionWarning = new NoActionWarning(message, 466, EventLogEntryType.Warning);
                    //ExceptionManager.HandleException(noActionWarning, PolicyName.SystemException);
                    //throw noActionWarning;
                }
                else
                {
                    ManagementObject sendPort = Wmi.LoadObject(bizTalkServer, "root\\MicrosoftBizTalkServer", "MSBTS_SendPort", "Name", sendPortName);
                    WmiOperationCompletedEventHandler handler = new WmiOperationCompletedEventHandler();
                    sendPort.InvokeMethod(handler.Observer, "Enlist", null, null);
                    handler.WaitForCompletion(waitTimeSec);
                }
            }
            //catch (NoActionWarning)
            //{
            //    if (throwWarnings)
            //    {
            //        throw;
            //    }
            //}
            catch (Exception exception)
            {
                //string message = String.Format("Failed to enlist send port '{0}'.", sendPortName);
                //OperationFailedException operationFailedException = new OperationFailedException(message, 467, EventLogEntryType.Error, exception);
                //ExceptionManager.HandleException(operationFailedException, PolicyName.SystemException);
                //throw operationFailedException;
            }
        }

        /// <summary>
        /// Unenlists a send port.
        /// </summary>
        /// <param name="sendPortName">The name of the send port.</param>
        /// <param name="waitTimeSec">The maximum amount of time that we will wait for the operation to complete. The default is 30 seconds.</param>
        /// <exception cref="NoActionWarning">Thrown as a Warning with EventId 468 if the send port was already enlisted.</exception>
        /// <exception cref="OperationFailedException">Thrown as an Error with EventId 469 if there was an error enlisting the send port.</exception>
        //public void UnenlistSendPort(string sendPortName, int waitTimeSec = 30)
        //{
        //    try
        //    {
        //        PortStatus portStatus = SendPortStatus(sendPortName);
        //        if (portStatus == PortStatus.Bound_And_Unenlisted)
        //        {
        //            string message = String.Format("The send port '{0}' is already unenlisted.", sendPortName);
        //            NoActionWarning noActionWarning = new NoActionWarning(message, 468, EventLogEntryType.Warning);
        //            ExceptionManager.HandleException(noActionWarning, PolicyName.SystemException);
        //            throw noActionWarning;
        //        }
        //        else
        //        {
        //            if (portStatus == PortStatus.Started)
        //            {
        //                StopSendPort(sendPortName, waitTimeSec);
        //            }
        //            ManagementObject sendPort = Wmi.LoadObject(bizTalkServer, "root\\MicrosoftBizTalkServer", "MSBTS_SendPort", "Name", sendPortName);
        //            WmiOperationCompletedEventHandler handler = new WmiOperationCompletedEventHandler();
        //            sendPort.InvokeMethod(handler.Observer, "UnEnlist", null, null);
        //            handler.WaitForCompletion(waitTimeSec);
        //        }
        //    }
        //    //catch (NoActionWarning)
        //    //{
        //    //    if (throwWarnings)
        //    //    {
        //    //        throw;
        //    //    }
        //    //}
        //    catch (Exception exception)
        //    {
        //        //string message = String.Format("Failed to unenlist send port '{0}'.", sendPortName);
        //        //OperationFailedException operationFailedException = new OperationFailedException(message, 469, EventLogEntryType.Error, exception);
        //        //ExceptionManager.HandleException(operationFailedException, PolicyName.SystemException);
        //        //throw operationFailedException;
        //    }
        //}

        ///// <summary>
        ///// Starts a send port
        ///// </summary>
        ///// <param name="sendPortName">The name of the send port.</param>
        ///// <param name="waitTimeSec">The maximum amount of time that we will wait for the operation to complete. The default is 30 seconds.</param>
        ///// <exception cref="NoActionWarning">Thrown as a Warning with EventId 470 if the send port was already started.</exception>
        ///// <exception cref="OperationFailedException">Thrown as an Error with EventId 471 if there was an error starting the send port.</exception>
        //public void StartSendPort(string sendPortName, int waitTimeSec = 30)
        //{
        //    try
        //    {
        //        PortStatus portStatus = SendPortStatus(sendPortName);
        //        if (portStatus == PortStatus.Started)
        //        {
        //            //string message = String.Format("The send port '{0}' is already started.", sendPortName);
        //            //NoActionWarning alreadyStartedException = new NoActionWarning(message, 470, EventLogEntryType.Warning);
        //            //ExceptionManager.HandleException(alreadyStartedException, PolicyName.SystemException);
        //            //throw alreadyStartedException;
        //        }
        //        else
        //        {
        //            if (portStatus == PortStatus.Bound_And_Unenlisted)
        //            {
        //                EnlistSendPort(sendPortName, waitTimeSec);
        //            }
        //            ManagementObject sendPort = Wmi.LoadObject(bizTalkServer, "root\\MicrosoftBizTalkServer", "MSBTS_SendPort", "Name", sendPortName);
        //            WmiOperationCompletedEventHandler handler = new WmiOperationCompletedEventHandler();
        //            sendPort.InvokeMethod(handler.Observer, "Start", null, null);
        //            handler.WaitForCompletion(waitTimeSec);
        //        }
        //    }
        //    //catch (NoActionWarning)
        //    //{
        //    //    if (throwWarnings)
        //    //    {
        //    //        throw;
        //    //    }
        //    //}
        //    catch (Exception exception)
        //    {
        //        //string message = String.Format("Failed to start send port '{0}'.", sendPortName);
        //        //OperationFailedException operationFailedException = new OperationFailedException(message, 471, EventLogEntryType.Error, exception);
        //        //ExceptionManager.HandleException(operationFailedException, PolicyName.SystemException);
        //        //throw operationFailedException;
        //    }
        //}

        ///// <summary>
        ///// Stops a send port.
        ///// </summary>
        ///// <param name="sendPortName">The name of the send port.</param>
        ///// <param name="waitTimeSec">The maximum amount of time that we will wait for the operation to complete. The default is 30 seconds.</param>
        ///// <exception cref="NoActionWarning">Thrown as a Warning with EventId 472 if the send port was already stopped.</exception>
        ///// <exception cref="OperationFailedException">Thrown as an Error with EventId 473 if there was an error stopping the send port.</exception>
        //public void StopSendPort(string sendPortName, int waitTimeSec = 30)
        //{
        //    try
        //    {
        //        PortStatus portStatus = SendPortStatus(sendPortName);
        //        if (portStatus != PortStatus.Started)
        //        {
        //            string message = String.Format("The send port '{0}' is already stopped.", sendPortName);
        //            NoActionWarning noActionWarning = new NoActionWarning(message, 472, EventLogEntryType.Warning);
        //            ExceptionManager.HandleException(noActionWarning, PolicyName.SystemException);
        //            throw noActionWarning;
        //        }
        //        else
        //        {
        //            ManagementObject sendPort = Wmi.LoadObject(bizTalkServer, "root\\MicrosoftBizTalkServer", "MSBTS_SendPort", "Name", sendPortName);
        //            WmiOperationCompletedEventHandler handler = new WmiOperationCompletedEventHandler();
        //            sendPort.InvokeMethod(handler.Observer, "Stop", null, null);
        //            handler.WaitForCompletion(waitTimeSec);
        //        }
        //    }
        //    //catch (NoActionWarning)
        //    //{
        //    //    if (throwWarnings)
        //    //    {
        //    //        throw;
        //    //    }
        //    //}
        //    catch (Exception exception)
        //    {
        //        //string message = String.Format("Failed to stop send port '{0}'.", sendPortName);
        //        //OperationFailedException operationFailedException = new OperationFailedException(message, 473, EventLogEntryType.Error, exception);
        //        //ExceptionManager.HandleException(operationFailedException, PolicyName.SystemException);
        //        //throw operationFailedException;
        //    }
        //}

        /// <summary>
        /// GetSendPortPrimaryTransportAddress method to retrieve the SendPort's URI.
        /// </summary>
        /// <param name="sendPortName">Name of the send port</param>
        /// <returns>string</returns>
        public string GetSendPortPrimaryTransportAddress(string sendPortName)
        {
            SendPort sendPort = catalog.SendPorts[sendPortName];
            TransportInfo transportInfo = sendPort.PrimaryTransport;

            return transportInfo.Address;
        }

        /// <summary>
        /// ModifySendPrimaryTransport method to modify the SendPort's URI.
        /// </summary>
        /// <param name="sendPortName">Name of the send port</param>
        /// <param name="transportUri">Sendport transport uri</param>
        /// <returns></returns>
        public void ModifySendPortPrimaryTransportAddress(string sendPortName, string transportUri)
        {
            try
            {
                SendPort sendPort = catalog.SendPorts[sendPortName];
                TransportInfo transportInfo = sendPort.PrimaryTransport;

                string transport = transportInfo.Address;

                sendPort.Status = portStatus.Stopped;
                catalog.SaveChanges();

                transportInfo.Address = transportUri;

                sendPort.Status = portStatus.Bound;
                sendPort.Status = portStatus.Started;
                catalog.SaveChanges();

                string newaddr = transportInfo.Address;
                if (String.Compare(newaddr, transportUri) != 0)
                {
                    throw new Exception("Failed to change address for sendport: " + sendPortName);
                }
            }
            catch (Exception exception)
            {
                catalog.DiscardChanges();
                throw exception;
            }
        }

        /// <summary>
        /// Checks for the existence of an orchestration.
        /// </summary>
        /// <param name="orchestrationName">The name of the orchestration.</param>
        /// <returns>True if the orchestration exists. False if the orchestration does no exist.</returns>
        /// <exception cref="OperationFailedException">Thrown with EventId 491 if the operation cannot be completed due to an unexpected error condition.</exception>
        public bool OrchestrationExists(string orchestrationName)
        {
            bool exists = false;
            try
            {
                ManagementClass mgmtClass = Wmi.LoadClass(Environment.MachineName, "root\\MicrosoftBizTalkServer", "MSBTS_Orchestration");
                exists = Wmi.ObjectExists(mgmtClass, "Name", orchestrationName);
            }
            catch (Exception exception)
            {
                //string message = String.Format("Failed to confirm existence of orchestration '{0}'.", orchestrationName);
                //OperationFailedException operationFailedException = new OperationFailedException(message, 491, EventLogEntryType.Error, exception);
                //ExceptionManager.HandleException(operationFailedException, PolicyName.SystemException);
                //throw operationFailedException;
            }
            return exists;
        }

        /// <summary>
        /// Checks the status of an orchestration.
        /// </summary>
        /// <param name="orchestrationName">The name of the orchestration.</param>
        /// <returns>An orchestration status.</returns>
        /// <exception cref="OperationFailedException">Thrown as an Error with EventId 410 if the status cannot be determined.</exception>
        public OrchStatus OrchestrationStatus(string orchestrationName)
        {
            OrchStatus status = OrchStatus.Unknown;
            try
            {
                if (OrchestrationExists(orchestrationName))
                {
                    ManagementObject orchestration = Wmi.LoadObject(bizTalkServer, "root\\MicrosoftBizTalkServer", "MSBTS_Orchestration", "Name", orchestrationName);
                    status = (OrchStatus)(orchestration["OrchestrationStatus"]);
                }
                else
                {
                    throw new Exception("The orchestration could not be found.");
                }
            }
            catch (Exception exception)
            {
                //string message = String.Format("Could not get status of orchestration '{0}'.", orchestrationName);
                //OperationFailedException operationFailedException = new OperationFailedException(message, 410, EventLogEntryType.Error, exception);
                //ExceptionManager.HandleException(operationFailedException, PolicyName.SystemException);
                //throw operationFailedException;
            }
            return status;
        }

        /// <summary>
        /// Enlists an orchestration.
        /// </summary>
        /// <param name="orchestrationName">The name of the orchestration.</param>
        /// <param name="waitTimeSec">The maximum amount of time that we will wait for the operation to complete. The default is 30 seconds.</param>
        /// <exception cref="NoActionWarning">Thrown as a Warning with EventId 474 if the orchestration was already enlisted.</exception>
        /// <exception cref="OperationFailedException">Thrown as an Error with EventId 475 if there was an error enlisting the orchestration.</exception>
        public void EnlistOrchestration(string orchestrationName, int waitTimeSec = 30)
        {
            try
            {
                OrchStatus orchStatus = OrchestrationStatus(orchestrationName);
                if (orchStatus == OrchStatus.Enlisted_And_Stopped || orchStatus == OrchStatus.Started)
                {
                    //string message = String.Format("The orchestration '{0}' is already enlisted.", orchestrationName);
                    //NoActionWarning noActionWarning = new NoActionWarning(message, 474, EventLogEntryType.Warning);
                    //ExceptionManager.HandleException(noActionWarning, PolicyName.SystemException);
                    //throw noActionWarning;
                }
                else if (orchStatus == OrchStatus.Unbound)
                {
                    throw new Exception("The orchestration is not bound.");
                }
                else
                {
                    ManagementObject orchestration = Wmi.LoadObject(bizTalkServer, "root\\MicrosoftBizTalkServer", "MSBTS_Orchestration", "Name", orchestrationName);
                    WmiOperationCompletedEventHandler handler = new WmiOperationCompletedEventHandler();
                    ManagementBaseObject inParams = orchestration.GetMethodParameters("Enlist");
                    inParams["HostName"] = null;
                    orchestration.InvokeMethod(handler.Observer, "Enlist", inParams, null);
                    handler.WaitForCompletion(waitTimeSec);
                }
            }
            //catch (NoActionWarning)
            //{
            //    if (throwWarnings)
            //    {
            //        throw;
            //    }
            //}
            catch (Exception exception)
            {
                //string message = String.Format("Failed to enlist orchestration '{0}'.", orchestrationName);
                //OperationFailedException operationFailedException = new OperationFailedException(message, 475, EventLogEntryType.Error, exception);
                //ExceptionManager.HandleException(operationFailedException, PolicyName.SystemException);
                //throw operationFailedException;
            }
        }

        /// <summary>
        /// Unenlist an orchestration.
        /// </summary>
        /// <param name="orchestrationName">The name of the orchestration.</param>
        /// <param name="waitTimeSec">The maximum amount of time that we will wait for the operation to complete. The default is 30 seconds.</param>
        /// <param name="autoTerminateOrchestrationInstanceFlag">An integer specifying whether instances of this orchestration type should be automatically
        /// terminated. Permissible values for this parameter are:</br>
        /// 1) Do not terminate service instances of this orchestration, or</br>
        /// 2) Terminate all service instances of this orchestration.</param>
        /// <exception cref="NoActionWarning">Thrown as a Warning with EventId 476 if the orchestration was already unenlisted.</exception>
        /// <exception cref="OperationFailedException">Thrown as an Error with EventId 477 if there was an error starting the orchestration.</exception>
        //public void UnenlistOrchestration(string orchestrationName, int waitTimeSec = 30, int autoTerminateOrchestrationInstanceFlag = 1)
        //{
        //    try
        //    {
        //        OrchStatus orchStatus = OrchestrationStatus(orchestrationName);
        //        if (orchStatus == OrchStatus.Bound_And_Unenlisted || orchStatus == OrchStatus.Unbound)
        //        {
        //            //string message = String.Format("The orchestration '{0}' is already unenlisted.", orchestrationName);
        //            //NoActionWarning noActionWarning = new NoActionWarning(message, 476, EventLogEntryType.Warning);
        //            //ExceptionManager.HandleException(noActionWarning, PolicyName.SystemException);
        //            //throw noActionWarning;
        //        }
        //        else
        //        {
        //            if (orchStatus == OrchStatus.Started)
        //            {
        //                StopOrchestration(orchestrationName, waitTimeSec);
        //            }
        //            ManagementObject orchestration = Wmi.LoadObject(bizTalkServer, "root\\MicrosoftBizTalkServer", "MSBTS_Orchestration", "Name", orchestrationName);
        //            ManagementBaseObject inParams = orchestration.GetMethodParameters("Unenlist");
        //            inParams["AutoTerminateOrchestrationInstanceFlag"] = autoTerminateOrchestrationInstanceFlag;
        //            WmiOperationCompletedEventHandler handler = new WmiOperationCompletedEventHandler();
        //            orchestration.InvokeMethod(handler.Observer, "Unenlist", inParams, null);
        //            handler.WaitForCompletion(waitTimeSec);
        //        }
        //    }
              //catch (NoActionWarning)
              //{
              //    if (throwWarnings)
              //    {
              //        throw;
              //    }
              //}
        //    catch (Exception exception)
        //    {
        //        //string message = String.Format("Failed to unenlist orchestration '{0}'.", orchestrationName);
        //        //OperationFailedException operationFailedException = new OperationFailedException(message, 477, EventLogEntryType.Error, exception);
        //        //ExceptionManager.HandleException(operationFailedException, PolicyName.SystemException);
        //        //throw operationFailedException;
        //    }
        //}

        /// <summary>
        /// Starts an orchestration.
        /// </summary>
        /// <param name="orchestrationName">The name of the orchestration.</param>
        /// <param name="waitTimeSec">The maximum amount of time that we will wait for the operation to complete. The default is 30 seconds.</param>
        /// <param name="autoEnableReceiveLocationFlag">Specifies whether receive locations associated with this orchestration should be automatically
        /// enabled. Permissible values for this parameter are:</br>
        /// 1) No auto enable of receive locations related to this orchestration, or </br>
        /// 2) Enable all receive locations related to this orchestration.</param>
        /// <param name="autoResumeOrchestrationInstanceFlag">Specifies whether service instances of this orchestration type that were manually suspended
        /// previously should be automatically resumed. Permissible values for this parameter are:</br>
        /// 1) No auto resume of service instances of this orchestration, or</br>
        /// 2) Automatically resume all suspended service instances of this orchestration.</param>
        /// <param name="autoStartSendPortsFlag">Specifies whether send ports and send port groups imported by this orchestration should be automatically
        /// started. Permissible values for this parameter are:</br>
        /// 1) No auto start of send ports and send port groups of this orchestration, or</br>
        /// 2) Start all send ports and send port groups associated with this orchestration.</br>
        /// If the value is 1 and there exists a send port or send port group that is in the Bound state, WMI will fail this orchestration start operation.</param>
        /// <exception cref="AlreadyStartedException">Thrown as a Warning with EventId 478 if the orchestration was already started.</exception>
        /// <exception cref="OperationFailedException">Thrown as an Error with EventId 479 if there was an error starting the orchestration.</exception>
        public void StartOrchestration(string orchestrationName, int waitTimeSec = 30, int autoEnableReceiveLocationFlag = 1, int autoResumeOrchestrationInstanceFlag = 2, int autoStartSendPortsFlag = 2)
        {
            try
            {
                OrchStatus orchStatus = OrchestrationStatus(orchestrationName);
                if (orchStatus == OrchStatus.Started)
                {
                    //string message = String.Format("The orchestration '{0}' is already started.", orchestrationName);
                    //NoActionWarning alreadyStartedException = new NoActionWarning(message, 478, EventLogEntryType.Warning);
                    //ExceptionManager.HandleException(alreadyStartedException, PolicyName.SystemException);
                    //throw alreadyStartedException;
                }
                else if (orchStatus == OrchStatus.Unbound)
                {
                    throw new Exception("The orchestration is not bound.");
                }
                else
                {
                    if (orchStatus == OrchStatus.Bound_And_Unenlisted)
                    {
                        EnlistOrchestration(orchestrationName, waitTimeSec);
                    }
                    ManagementObject orchestration = Wmi.LoadObject(bizTalkServer, "root\\MicrosoftBizTalkServer", "MSBTS_Orchestration", "Name", orchestrationName);
                    WmiOperationCompletedEventHandler handler = new WmiOperationCompletedEventHandler();
                    ManagementBaseObject inParams = orchestration.GetMethodParameters("Start");
                    inParams["AutoEnableReceiveLocationFlag"] = autoEnableReceiveLocationFlag;
                    inParams["AutoResumeOrchestrationInstanceFlag"] = autoResumeOrchestrationInstanceFlag;
                    inParams["AutoStartSendPortsFlag"] = autoStartSendPortsFlag;
                    orchestration.InvokeMethod(handler.Observer, "Start", inParams, null);
                    handler.WaitForCompletion(waitTimeSec);
                }
            }
            //catch (NoActionWarning)
            //{
            //    if (throwWarnings)
            //    {
            //        throw;
            //    }
            //}
            catch (Exception exception)
            {
                //string message = String.Format("Failed to start orchestration '{0}'.", orchestrationName);
                //OperationFailedException operationFailedException = new OperationFailedException(message, 479, EventLogEntryType.Error, exception);
                //ExceptionManager.HandleException(operationFailedException, PolicyName.SystemException);
                //throw operationFailedException;
            }
        }

        /// <summary>
        /// Stops an orchestration.
        /// </summary>
        /// <param name="orchestrationName">The name of the orchestration.</param>
        /// <param name="waitTimeSec">The maximum amount of time that we will wait for the operation to complete. The default is 30 seconds.</param>        
        /// <param name="autoDisableReceiveLocationFlag">Permissible values for this property are:</br>
        /// 1) No auto disable of receive locations related to this Orchestration, or </br>
        /// 2) Disable all receive locations related to this orchestration that are not shared by other orchestration(s).</param>
        /// <param name="autoSuspendOrchestrationInstanceFlag">Permissible values for this property are:</br>
        /// 1) No auto suspend of service instances of this Orchestration, or </br>
        /// 2) Suspend all running service instances of this Orchestration</param>
        /// <exception cref="NoActionWarning">Thrown as a Warning with EventId 480 if the orchestration was already stopped.</exception>
        /// <exception cref="OperationFailedException">Thrown as an Error with EventId 481 if there was an error stopping the orchestration.</exception>
        //public void StopOrchestration(string orchestrationName, int waitTimeSec = 30, int autoDisableReceiveLocationFlag = 1, int autoSuspendOrchestrationInstanceFlag = 2)
        //{
        //    try
        //    {
        //        OrchStatus orchStatus = OrchestrationStatus(orchestrationName);
        //        if (orchStatus != OrchStatus.Started)
        //        {
        //            string message = String.Format("The orchestration '{0}' is already stopped.", orchestrationName);
        //            NoActionWarning noActionWarning = new NoActionWarning(message, 480, EventLogEntryType.Warning);
        //            ExceptionManager.HandleException(noActionWarning, PolicyName.SystemException);
        //            throw exception;
        //        }
        //        else
        //        {
        //            ManagementObject orchestration = Wmi.LoadObject(bizTalkServer, "root\\MicrosoftBizTalkServer", "MSBTS_Orchestration", "Name", orchestrationName);
        //            ManagementBaseObject inParams = orchestration.GetMethodParameters("Stop");
        //            inParams["AutoDisableReceiveLocationFlag"] = autoDisableReceiveLocationFlag;
        //            inParams["AutoSuspendOrchestrationInstanceFlag"] = autoSuspendOrchestrationInstanceFlag;
        //            WmiOperationCompletedEventHandler handler = new WmiOperationCompletedEventHandler();
        //            orchestration.InvokeMethod(handler.Observer, "Stop", inParams, null);
        //            handler.WaitForCompletion(waitTimeSec);
        //        }
        //    }
        //    //catch (NoActionWarning)
        //    //{
        //    //    if (throwWarnings)
        //    //    {
        //    //        throw;
        //    //    }
        //    //}
        //    catch (Exception exception)
        //    {
        //        //string message = String.Format("Failed to stop orchestration '{0}'.", orchestrationName);
        //        //OperationFailedException operationFailedException = new OperationFailedException(message, 481, EventLogEntryType.Error, exception);
        //        //ExceptionManager.HandleException(operationFailedException, PolicyName.SystemException);
        //        //throw operationFailedException;
        //    }
        //}

        /// <summary>
        /// Method to set the Send Port Soap Action.
        /// </summary>
        /// <param name="sendPortName">SendPort name</param>
        /// <param name="transportUri">Soap Action</param>
        /// <returns></returns>
        public void SetSendPortSoapAction(string sendPortName, string soapAction)
        {
            try
            {
                SendPort sendPort = catalog.SendPorts[sendPortName];
                TransportInfo transportInfo = sendPort.PrimaryTransport;

                string transportTypeData = transportInfo.TransportTypeData;

                XDocument transportSettings = XDocument.Parse(transportTypeData);
                XElement soapActionElement = null;

                if (transportSettings.Descendants("StaticAction") != null && transportSettings.Descendants("StaticAction").Count() > 0)
                {
                    soapActionElement = transportSettings.Descendants("StaticAction").First();
                    soapActionElement.SetValue(soapAction);
                }

                transportInfo.TransportTypeData = transportSettings.ToString();

                sendPort.Status = portStatus.Bound;
                sendPort.Status = portStatus.Started;
                catalog.SaveChanges();
            }
            catch (Exception exception)
            {
                catalog.DiscardChanges();
                throw exception;
            }
        }

        /// <summary>
        /// Method to get the SendPort Soap Action.
        /// </summary>
        /// <param name="sendPortName">SendPort name</param>
        /// <returns>Soap Action value</returns>
        public string GetSendPortSoapAction(string sendPortName)
        {
            string staticActionValueCurrent = "";
            try
            {
                SendPort sendPort = catalog.SendPorts[sendPortName];
                TransportInfo transportInfo = sendPort.PrimaryTransport;

                string transportTypeData = transportInfo.TransportTypeData;

                XDocument transportSettings = XDocument.Parse(transportTypeData);

                if (transportSettings.Descendants("StaticAction") != null && transportSettings.Descendants("StaticAction").Count() > 0)
                {
                    staticActionValueCurrent = transportSettings.Descendants("StaticAction").First().Value;
                }
            }
            catch (Exception exception)
            {
                catalog.DiscardChanges();
                throw exception;
            }
            return staticActionValueCurrent;
        }


    }
}
