using Microsoft.BizTalk.Management;


namespace Avista.ESB.Admin
{
      internal static class ServiceStateConverter
      {
            public static BizTalkServiceState FromManagementType (HostInstance.ServiceStateValues value)
            {
                  switch ( value )
                  {
                        case HostInstance.ServiceStateValues.NULL_ENUM_VALUE:
                              return BizTalkServiceState.NotApplicable;
                        case HostInstance.ServiceStateValues.Stopped:
                              return BizTalkServiceState.Stopped;
                        case HostInstance.ServiceStateValues.Start_pending:
                              return BizTalkServiceState.StartPending;
                        case HostInstance.ServiceStateValues.Stop_pending:
                              return BizTalkServiceState.StopPending;
                        case HostInstance.ServiceStateValues.Running:
                              return BizTalkServiceState.Running;
                        case HostInstance.ServiceStateValues.Continue_pending:
                              return BizTalkServiceState.ContinuePending;
                        case HostInstance.ServiceStateValues.Pause_pending:
                              return BizTalkServiceState.PausePending;
                        case HostInstance.ServiceStateValues.Paused:
                              return BizTalkServiceState.Paused;
                  }

                  return BizTalkServiceState.NotApplicable;
            }
      }
}
