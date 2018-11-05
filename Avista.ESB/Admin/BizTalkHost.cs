using System;
using Microsoft.BizTalk.ExplorerOM;
using Microsoft.BizTalk.Management;
using Avista.ESB.Admin.Settings;
using Avista.ESB.Admin.Enums;
namespace Avista.ESB.Admin
{
      public class BizTalkHost : BizTalkArtifact
      {
            private readonly Host host;
            private HostSetting hostSetting;

            private BizTalkHost (BizTalkCatalog catalog, Host paramHost)
                  : base( catalog )
            {
                  host = paramHost;
                  RetrieveHostSetting( paramHost.Name );
            }

            public static BizTalkHost FromItem (BizTalkCatalog catalog, object item)
            {
                  try
                  {
                        return new BizTalkHost( catalog, item as Host );
                  }
                  catch ( System.Exception )
                  {
                        throw;
                  }
            }


            public HostType BtsHostType
            {
                  get
                  {
                        return HostTypeConverter.FromNativeType( host.Type );
                  }
            }

          

            public override string Name
            {
                  get
                  {
                        return hostSetting.Name;
                  }
            }

            public string NtGroupName
            {
                  get
                  {
                        return hostSetting.NTGroupName;
                  }
            }

            public static HostType FromNativeType (HostType value)
            {
                  switch ( value )
                  {
                        case HostType.Invalid:
                              return HostType.Invalid;
                        case HostType.InProcess:
                              return HostType.InProcess;
                        case HostType.Isolated:
                              return HostType.Isolated;
                  }

                  return HostType.Invalid;
            }

            #region Host settings Attributes
            /// <summary>
            /// AllowMultipleResponses
            /// </summary>
            public bool AllowMultipleResponses
            {
                  get
                  {
                        return hostSetting.AllowMultipleResponses;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.AllowMultipleResponses = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// DBQueueSizeThreshold
            /// </summary>
            public UInt32 DBQueueSizeThreshold
            {
                  get
                  {
                        return hostSetting.DBQueueSizeThreshold;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.DBQueueSizeThreshold = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// DBSessionThreshold
            /// </summary>
            public UInt32 DBSessionThreshold
            {
                  get
                  {
                        return hostSetting.DBSessionThreshold;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.DBSessionThreshold = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            ///  AuthTrusted attribute
            /// </summary>
            public bool AuthTrusted
            {
                  get
                  {
                        return hostSetting.AuthTrusted;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.AuthTrusted = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// Clustered attribute
            /// </summary>
            public bool Clustered
            {
                  get
                  {
                        return !String.IsNullOrEmpty( hostSetting.ClusterResourceGroupName );
                  }
            }
            /// <summary>
            /// HostTracking
            /// </summary>
            public bool HostTracking
            {
                  get
                  {
                        return hostSetting.HostTracking;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.HostTracking = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// IsDefault
            /// </summary>
            public bool IsDefault
            {
                  get
                  {
                        return hostSetting.IsDefault;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.IsDefault = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// Is32BitOnly
            /// </summary>
            public bool Is32BitOnly
            {
                  get
                  {
                        return hostSetting.IsHost32BitOnly;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.IsHost32BitOnly = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }


            /// <summary>
            /// DehydrationBehavior
            /// </summary>

            public BizTalkDehydrationBehavior DehydrationBehavior
            {
                  get
                  {
                        return DehydrationBehaviorConverter.FromManagementType( hostSetting.DehydrationBehavior );
                  }
                  set
                  {
                        try
                        {
                              hostSetting.DehydrationBehavior = DehydrationBehaviorConverter.ToManagementType( value );
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }

            /// <summary>
            /// DeliveryQyueueSize
            /// </summary>
            public UInt32 DeliveryQueueSize
            {
                  get
                  {
                        return hostSetting.DeliveryQueueSize;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.DeliveryQueueSize = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// GlobalMemoryThreshold
            /// </summary>
            public UInt32 GlobalMemoryThreshold
            {
                  get
                  {
                        return hostSetting.GlobalMemoryThreshold;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.GlobalMemoryThreshold = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// InflightMessageThreshold
            /// </summary>
            public UInt32 InflightMessageThreshold
            {
                  get
                  {
                        return hostSetting.InflightMessageThreshold;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.InflightMessageThreshold = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// IsHost32BitOnly
            /// </summary>
            public bool IsHost32BitOnly
            {
                  get
                  {
                        return hostSetting.IsHost32BitOnly;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.IsHost32BitOnly = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// LegacyWhitespace
            /// </summary>
            public bool LegacyWhitespace
            {
                  get
                  {
                        return hostSetting.LegacyWhitespace;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.LegacyWhitespace = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// MessageDeliveryMaximumDelay
            /// </summary>
            public UInt32 MessageDeliveryMaximumDelay
            {
                  get
                  {
                        return hostSetting.MessageDeliveryMaximumDelay;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.MessageDeliveryMaximumDelay = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// MessageDeliveryOverdriveFactor
            /// </summary>
            public UInt32 MessageDeliveryOverdriveFactor
            {
                  get
                  {
                        return hostSetting.MessageDeliveryOverdriveFactor;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.MessageDeliveryOverdriveFactor = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// MessageDeliverySampleSpaceSize
            /// </summary>
            public UInt32 MessageDeliverySampleSpaceSize
            {
                  get
                  {
                        return hostSetting.MessageDeliverySampleSpaceSize;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.MessageDeliverySampleSpaceSize = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// MessageDeliverySampleSpaceWindow
            /// </summary>
            public UInt32 MessageDeliverySampleSpaceWindow
            {
                  get
                  {
                        return hostSetting.MessageDeliverySampleSpaceWindow;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.MessageDeliverySampleSpaceWindow = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// MessagePublishMaximumDelay
            /// </summary>
            public UInt32 MessagePublishMaximumDelay
            {
                  get
                  {
                        return hostSetting.MessagePublishMaximumDelay;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.MessagePublishMaximumDelay = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// MessagePublishOverdriveFactor
            /// </summary>
            public UInt32 MessagePublishOverdriveFactor
            {
                  get
                  {
                        return hostSetting.MessagePublishOverdriveFactor;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.MessagePublishOverdriveFactor = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// MessagePublishSampleSpaceSize
            /// </summary>
            public UInt32 MessagePublishSampleSpaceSize
            {
                  get
                  {
                        return hostSetting.MessagePublishSampleSpaceSize;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.MessagePublishSampleSpaceSize = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// MessagePublishSampleSpaceWindow
            /// </summary>
            public UInt32 MessagePublishSampleSpaceWindow
            {
                  get
                  {
                        return hostSetting.MessagePublishSampleSpaceWindow;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.MessagePublishSampleSpaceWindow = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// MessagingMaxReceiveInterval
            /// </summary>
            public UInt32 MessagingMaxReceiveInterval
            {
                  get
                  {
                        return hostSetting.MessagingMaxReceiveInterval;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.MessagingMaxReceiveInterval = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// MessagingReqRespTTL
            /// </summary>
            public UInt32 MessagingReqRespTTL
            {
                  get
                  {
                        return hostSetting.MessagingReqRespTTL;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.MessagingReqRespTTL = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }

            /// <summary>
            /// MsgAgentPerfCounterServiceClassID
            /// </summary>
            public Guid MsgAgentPerfCounterServiceClassID
            {
                  get
                  {
                        return PerfCounterServiceClassConverter.FromManagementType( hostSetting.MsgAgentPerfCounterServiceClassID );
                  }
                  set
                  {
                        try
                        {
                              hostSetting.MsgAgentPerfCounterServiceClassID = PerfCounterServiceClassConverter.ToManagementType( value );
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// ProcessMemoryThreshold
            /// </summary>
            public UInt32 ProcessMemoryThreshold
            {
                  get
                  {
                        return hostSetting.ProcessMemoryThreshold;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.ProcessMemoryThreshold = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// SubscriptionPauseAt
            /// </summary>
            public UInt32 SubscriptionPauseAt
            {
                  get
                  {
                        return hostSetting.SubscriptionPauseAt;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.SubscriptionPauseAt = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// SubscriptionResumeAt
            /// </summary>
            public UInt32 SubscriptionResumeAt
            {
                  get
                  {
                        return hostSetting.SubscriptionResumeAt;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.SubscriptionResumeAt = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// ThreadPoolSize
            /// </summary>
            public UInt32 ThreadPoolSize
            {
                  get
                  {
                        return hostSetting.ThreadPoolSize;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.ThreadPoolSize = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// ThreadThreshold
            /// </summary>
            public UInt32 ThreadThreshold
            {
                  get
                  {
                        return hostSetting.ThreadThreshold;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.ThreadThreshold = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// ThrottlingBatchMemoryThresholdPercent
            /// </summary>
            public UInt32 ThrottlingBatchMemoryThresholdPercent
            {
                  get
                  {
                        return hostSetting.ThrottlingBatchMemoryThresholdPercent;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.ThrottlingBatchMemoryThresholdPercent = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }

            /// <summary>
            /// ThrottlingDeliveryOverride
            /// </summary>
            public UInt32 ThrottlingDeliveryOverride
            {
                  get
                  {
                        return Convert.ToUInt32( hostSetting.ThrottlingDeliveryOverride );
                  }
                  set
                  {
                        try
                        {
                              hostSetting.ThrottlingDeliveryOverride = (HostSetting.ThrottlingDeliveryOverrideValues)
                                  Enum.Parse( typeof( HostSetting.ThrottlingDeliveryOverrideValues ), value.ToString() );
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            //ThrottlingDeliveryOverrideSeverity
            public UInt32 ThrottlingDeliveryOverrideSeverity
            {
                  get
                  {
                        return hostSetting.ThrottlingDeliveryOverrideSeverity;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.ThrottlingDeliveryOverrideSeverity = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// ThrottlingLimitToTriggerGC
            /// </summary>
            public UInt32 ThrottlingLimitToTriggerGC
            {
                  get
                  {
                        return hostSetting.ThrottlingLimitToTriggerGC;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.ThrottlingLimitToTriggerGC = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }

            /// <summary>
            /// ThrottlingPublishOverride
            /// </summary>
            public UInt32 ThrottlingPublishOverride
            {
                  get
                  {
                        return Convert.ToUInt32( hostSetting.ThrottlingPublishOverride );
                  }
                  set
                  {
                        try
                        {
                              hostSetting.ThrottlingPublishOverride = (HostSetting.ThrottlingPublishOverrideValues)
                                  Enum.Parse( typeof( HostSetting.ThrottlingPublishOverrideValues ), value.ToString() );
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// ThrottlingPublishOverrideSeverity
            /// </summary>
            public UInt32 ThrottlingPublishOverrideSeverity
            {
                  get
                  {
                        return hostSetting.ThrottlingPublishOverrideSeverity;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.ThrottlingPublishOverrideSeverity = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// ThrottlingSeverityDatabaseSize
            /// </summary>
            public UInt32 ThrottlingSeverityDatabaseSize
            {
                  get
                  {
                        return hostSetting.ThrottlingSeverityDatabaseSize;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.ThrottlingSeverityDatabaseSize = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// ThrottlingSeverityInflightMessage
            /// </summary>
            public UInt32 ThrottlingSeverityInflightMessage
            {
                  get
                  {
                        return hostSetting.ThrottlingSeverityInflightMessage;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.ThrottlingSeverityInflightMessage = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// ThrottlingSeverityProcessMemory
            /// </summary>
            public UInt32 ThrottlingSeverityProcessMemory
            {
                  get
                  {
                        return hostSetting.ThrottlingSeverityProcessMemory;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.ThrottlingSeverityProcessMemory = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// ThrottlingSpoolMultiplier
            /// </summary>
            public UInt32 ThrottlingSpoolMultiplier
            {
                  get
                  {
                        return hostSetting.ThrottlingSpoolMultiplier;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.ThrottlingSpoolMultiplier = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// ThrottlingTrackingDataMultiplier
            /// </summary>
            public UInt32 ThrottlingTrackingDataMultiplier
            {
                  get
                  {
                        return hostSetting.ThrottlingTrackingDataMultiplier;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.ThrottlingTrackingDataMultiplier = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// TimeBasedMaxThreshold
            /// </summary>
            public UInt32 TimeBasedMaxThreshold
            {
                  get
                  {
                        return hostSetting.TimeBasedMaxThreshold;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.TimeBasedMaxThreshold = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// TimeBasedMinThreshold
            /// </summary>
            public UInt32 TimeBasedMinThreshold
            {
                  get
                  {
                        return hostSetting.TimeBasedMinThreshold;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.TimeBasedMinThreshold = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// UseDefaultAppDomainForIsolatedAdapter
            /// </summary>
            public bool UseDefaultAppDomainForIsolatedAdapter
            {
                  get
                  {
                        return hostSetting.UseDefaultAppDomainForIsolatedAdapter;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.UseDefaultAppDomainForIsolatedAdapter = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }
            /// <summary>
            /// XlangMaxReceiveInterval
            /// </summary>
            public UInt32 XlangMaxReceiveInterval
            {
                  get
                  {
                        return hostSetting.XlangMaxReceiveInterval;
                  }
                  set
                  {
                        try
                        {
                              hostSetting.XlangMaxReceiveInterval = value;
                        }
                        catch ( System.Exception )
                        {
                              throw;
                        }
                  }
            }

            #endregion

            #region Host  Operations

            public void ExportHostSettings (string path)
            {
            }

            public void ImportHostSettings (string path)
            {
                  SettingsWorker settingsWorker = new SettingsWorker( Catalog.Instance, Catalog.Database );
                  HostSettings hostSettings = Helper.BizTalkSettingsHelper.ParseHostsSettings( path );

                  ImportHostSettings( settingsWorker, hostSettings );

                  RetrieveHostSetting( Name );
            }

            internal void ImportHostSettings (SettingsWorker settingsWorker, HostSettings settings)
            {
                  ExportedSettings exportedSettings = new ExportedSettings();
                  exportedSettings.ExportedGroup = String.Format( "{0}:{1}", Catalog.Instance.ToUpper(), Catalog.Database.ToUpper() );
                  exportedSettings.HostSettings = settings;

                  try
                  {
                        settingsWorker.ImportHostSettings(Name, Name, exportedSettings );
                  }
                  catch ( Exception e )
                  {
                        throw new BtsException( settingsWorker.GetManagementExceptionDescription( e ), e );
                  }
            }

            #endregion

            #region Host Implementation

            private void RetrieveHostSetting (string hostName)
            {
                  try
                  {
                        HostSetting.StaticScope = ManagementHelper.GetScope( typeof( HostSetting ), Catalog.Instance, Catalog.Database );
                        foreach ( HostSetting setting in HostSetting.GetInstances() )
                              if ( hostName == setting.Name )
                                    hostSetting = setting;
                  }

                  finally
                  {
                        HostSetting.StaticScope = null;
                  }
            }

            #endregion
      }
}
