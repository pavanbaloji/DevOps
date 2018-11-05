using Microsoft.BizTalk.Management;
using _hostType = Microsoft.BizTalk.ExplorerOM.HostType;

namespace Avista.ESB.Admin
{
      internal static class HostTypeConverter
      {
            public static HostType FromNativeType (_hostType value)
            {
                  switch ( value )
                  {
                        case _hostType.Invalid:
                              return HostType.Invalid;
                        case _hostType.InProcess:
                              return HostType.InProcess;
                        case _hostType.Isolated:
                              return HostType.Isolated;
                  }

                  return HostType.Invalid;
            }

            public static HostType FromManagementType (HostSetting.HostTypeValues value)
            {
                  switch ( value )
                  {
                        case HostSetting.HostTypeValues.NULL_ENUM_VALUE:
                              return HostType.Invalid;
                        case HostSetting.HostTypeValues.In_process:
                              return HostType.InProcess;
                        case HostSetting.HostTypeValues.Isolated:
                              return HostType.Isolated;
                  }

                  return HostType.Invalid;
            }
      }
}
