using System;
using Avista.ESB.Admin.Enums;
using Microsoft.BizTalk.Management;


namespace Avista.ESB.Admin.Settings
{

      internal static class PerfCounterServiceClassConverter
      {
            public static Guid FromManagementType (HostSetting.MsgAgentPerfCounterServiceClassIDValues value)
            {
                  switch ( value )
                  {
                        case HostSetting.MsgAgentPerfCounterServiceClassIDValues.NULL_ENUM_VALUE:
                              return Guid.Empty;
                        case HostSetting.MsgAgentPerfCounterServiceClassIDValues.Val__59F295B0_3123_416E_966B_A2C6D65FF8E6_:
                              return BizTalkPerfCounterServiceClass.MessagingInProcess;
                        case HostSetting.MsgAgentPerfCounterServiceClassIDValues.Val__683AEDF1_027D_4006_AE9A_443D1A5746FC_:
                              return BizTalkPerfCounterServiceClass.MessagingIsolated;
                  }

                  return Guid.Empty;
            }

            public static HostSetting.MsgAgentPerfCounterServiceClassIDValues ToManagementType (Guid value)
            {
                  if ( value.CompareTo( BizTalkPerfCounterServiceClass.MessagingInProcess ) == 0 )
                        return HostSetting.MsgAgentPerfCounterServiceClassIDValues.Val__59F295B0_3123_416E_966B_A2C6D65FF8E6_;
                  else if ( value.CompareTo( BizTalkPerfCounterServiceClass.MessagingIsolated ) == 0 )
                        return HostSetting.MsgAgentPerfCounterServiceClassIDValues.Val__683AEDF1_027D_4006_AE9A_443D1A5746FC_;

                  return HostSetting.MsgAgentPerfCounterServiceClassIDValues.NULL_ENUM_VALUE;
            }
      }
}
