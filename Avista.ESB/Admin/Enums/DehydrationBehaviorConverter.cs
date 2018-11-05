using Microsoft.BizTalk.Management;

namespace Avista.ESB.Admin.Enums
{
      internal static class DehydrationBehaviorConverter
      {
            public static BizTalkDehydrationBehavior FromManagementType (HostSetting.DehydrationBehaviorValues value)
            {
                  switch ( value )
                  {
                        case HostSetting.DehydrationBehaviorValues.NULL_ENUM_VALUE:
                              return BizTalkDehydrationBehavior.Invalid;
                        case HostSetting.DehydrationBehaviorValues.Always:
                              return BizTalkDehydrationBehavior.Always;
                        case HostSetting.DehydrationBehaviorValues.Custom:
                              return BizTalkDehydrationBehavior.Custom;
                        case HostSetting.DehydrationBehaviorValues.Never:
                              return BizTalkDehydrationBehavior.Never;
                  }

                  return BizTalkDehydrationBehavior.Invalid;
            }

            public static HostSetting.DehydrationBehaviorValues ToManagementType (BizTalkDehydrationBehavior value)
            {
                  switch ( value )
                  {
                        case BizTalkDehydrationBehavior.Invalid:
                              return HostSetting.DehydrationBehaviorValues.NULL_ENUM_VALUE;
                        case BizTalkDehydrationBehavior.Always:
                              return HostSetting.DehydrationBehaviorValues.Always;
                        case BizTalkDehydrationBehavior.Custom:
                              return HostSetting.DehydrationBehaviorValues.Custom;
                        case BizTalkDehydrationBehavior.Never:
                              return HostSetting.DehydrationBehaviorValues.Never;
                  }
                  return HostSetting.DehydrationBehaviorValues.NULL_ENUM_VALUE;
            }
      }
}
