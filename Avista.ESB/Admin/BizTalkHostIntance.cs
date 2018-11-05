using System;
using System.Net;
using Microsoft.BizTalk.ExplorerOM;
using Microsoft.BizTalk.Management;
using Avista.ESB.Admin.Enums;
namespace Avista.ESB.Admin
{
      public class BizTalkHostInstance : BizTalkArtifact
      {
            private readonly HostInstance hostInstance;
            private HostInstanceSetting bizTalkHostInstancesetting;

            private BizTalkHostInstance (BizTalkCatalog catalog, HostInstance instance)
                  : base( catalog )
            {
                  hostInstance = instance;

                  RetrieveHostInstanceSetting( instance.Name );
            }

            public static BizTalkHostInstance FromItem (BizTalkCatalog catalog, HostInstance item)
            {
                  return new BizTalkHostInstance(catalog, item);  // return new BtsHostInstance(catalog, item as HostInstance);
                        
      
            }

            #region Attributes

            public bool IsDisabled
            {
                  get
                  {
                        return hostInstance.IsDisabled;
                  }
                  set
                  {
                        try
                        {
                              hostInstance.IsDisabled = value;
                        }
                        catch ( System.Exception)
                        {
                              throw;
                        }
                  }
            }

            public override string Name
            {
                  get
                  {
                        return hostInstance.Name;
                  }
            }

            public string HostName
            {
                  get
                  {
                        return hostInstance.HostName;
                  }
            }

            public string NtGroupName
            {
                  get
                  {
                        return hostInstance.NTGroupName;
                  }
            }

            public string NtUserName
            {
                  get
                  {
                        return hostInstance.Logon;
                  }
            }

            public string RunningServer
            {
                  get
                  {
                        return hostInstance.RunningServer;
                  }
            }

            public HostType HostType
            {
                  get
                  {
                        return HostTypeConverter.FromManagementType( (HostSetting.HostTypeValues) hostInstance.HostType );
                  }
            }

            public BizTalkServiceState ServiceState
            {
                  get
                  {
                        return ServiceStateConverter.FromManagementType( hostInstance.ServiceState );
                  }
            }

            public UInt32 CLRMaxIOThreads
            {
                  get
                  {
                        return bizTalkHostInstancesetting.CLRMaxIOThreads;
                  }
                  set
                  {
                        try
                        {
                              bizTalkHostInstancesetting.CLRMaxIOThreads = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }

            public UInt32 CLRMaxWorkerThreads
            {
                  get
                  {
                        return bizTalkHostInstancesetting.CLRMaxWorkerThreads;
                  }
                  set
                  {
                        try
                        {
                              bizTalkHostInstancesetting.CLRMaxWorkerThreads = value;
                        }
                        catch ( System.Exception)
                        {
                              throw;
                        }
                  }
            }

            public UInt32 CLRMinIOThreads
            {
                  get
                  {
                        return bizTalkHostInstancesetting.CLRMinIOThreads;
                  }
                  set
                  {
                        try
                        {
                              bizTalkHostInstancesetting.CLRMinIOThreads = value;
                        }
                        catch ( System.Exception)
                        {
                              throw;
                        }
                  }
            }

            public UInt32 CLRMinWorkerThreads
            {
                  get
                  {
                        return bizTalkHostInstancesetting.CLRMinWorkerThreads;
                  }
                  set
                  {
                        try
                        {
                              bizTalkHostInstancesetting.CLRMinWorkerThreads = value;
                        }
                        catch ( System.Exception)
                        {
                              throw;
                        }
                  }
            }

            public UInt32 PhysicalMemoryMaximalUsage
            {
                  get
                  {
                        return bizTalkHostInstancesetting.PhysicalMemoryMaximalUsage;
                  }
                  set
                  {
                        try
                        {
                              bizTalkHostInstancesetting.PhysicalMemoryMaximalUsage = value;
                        }
                        catch ( System.Exception)
                        {
                              throw;
                        }
                  }
            }

            public UInt32 PhysicalMemoryOptimalUsage
            {
                  get
                  {
                        return bizTalkHostInstancesetting.PhysicalMemoryOptimalUsage;
                  }
                  set
                  {
                        try
                        {
                              bizTalkHostInstancesetting.PhysicalMemoryOptimalUsage = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }


            public UInt32 VirtualMemoryMaximalUsage
            {
                  get
                  {
                        return bizTalkHostInstancesetting.VirtualMemoryMaximalUsage;
                  }
                  set
                  {
                        try
                        {
                              bizTalkHostInstancesetting.VirtualMemoryMaximalUsage = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }

            public UInt32 VirtualMemoryOptimalUsage
            {
                  get
                  {
                        return bizTalkHostInstancesetting.VirtualMemoryOptimalUsage;
                  }
                  set
                  {
                        try
                        {
                              bizTalkHostInstancesetting.VirtualMemoryOptimalUsage = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }

            #endregion

            #region Operations

            public void Configure (NetworkCredential credential)
            {
                  try
                  {
                        // will throw if not previously stopped (WinMgmt)

                        hostInstance.Uninstall();
                        hostInstance.Install( true, credential.UserName, credential.Password );
                  }
                  catch ( System.Exception )
                  {
                        throw;
                  }
            }

            public void Start ()
            {
                  if ( HostType == HostType.Isolated )
                        return;

                  try
                  {
                        hostInstance.Start();
                  }
                  catch ( System.Exception )
                  {
                        throw;
                  }
            }

            public void Stop ()
            {
                  if ( HostType == HostType.Isolated )
                        return;

                  try
                  {
                        hostInstance.Stop();
                  }
                  catch ( System.Exception )
                  {
                        throw;
                  }
            }

            public void Restart ()
            {
                  Stop();
                  Start();
            }

            public static string GetNameFromHostName (string HostName, string RunningServer)
            {
                  return "Microsoft BizTalk Server " + HostName + " " + RunningServer;
            }


            public void ImportHostInstanceSettings (string path)
            {
                  var settingsWorker = new SettingsWorker(Catalog.Instance, Catalog.Database );
                  var hostInstanceSettings = Helper.BizTalkSettingsHelper.ParseHostInstancesSettings( path );

                  ImportHostInstanceSettings( settingsWorker, hostInstanceSettings );

                  RetrieveHostInstanceSetting( Name );
            }


            public void ImportHostInstanceSettings (SettingsWorker settingsWorker, HostInstanceSettings settings)
            {
                  var exportedSettings = new ExportedSettings();
                  exportedSettings.ExportedGroup = String.Format( "{0}:{1}", Catalog.Instance.ToUpper(), Catalog.Database.ToUpper() );
                  exportedSettings.HostInstanceSettings = settings;

                  try
                  {
                        var hostInstanceName = String.Format( "{0}:{1}", HostName, RunningServer );
                        settingsWorker.ImportHostInstanceSettings( hostInstanceName, hostInstanceName, exportedSettings );
                  }
                  catch ( System.Exception )
                  {
                        throw;
                  }
            }


            #endregion

            #region Implementation

            private void RetrieveHostInstanceSetting (string name)
            {
                  try
                  {
                        HostInstanceSetting.StaticScope = ManagementHelper.GetScope( typeof( HostInstanceSetting ), Catalog.Instance, Catalog.Database );
                        foreach ( HostInstanceSetting setting in HostInstanceSetting.GetInstances() )
                              if ( name == setting.Name )
                              {
                                    bizTalkHostInstancesetting = setting;
                                    break;
                              }
                  }

                  finally
                  {
                        HostInstanceSetting.StaticScope = null;
                  }
            }

            #endregion
      }
}
