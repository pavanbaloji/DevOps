using System;
using System.Management;
using System.Reflection;

namespace  Microsoft.BizTalk.Management
{
      internal static class ManagementHelper
      {
            private static string SCOPE_TEMPLATE_GROUPSETTING = @"\\.\root\MicrosoftBizTalkServer:{0}.MgmtDbServerName='{1}',MgmtDbName='{2}'";
            private static string SCOPE_TEMPLATE = @"\\.\root\MicrosoftBizTalkServer:{0}.MgmtDbServerOverride='{1}',MgmtDbNameOverride='{2}'";

            public static ManagementScope GetScope (Type type, string instance, string database)
            {
                  var classname = GetCreatedClassName( type );
                  var scope = String.Format( SCOPE_TEMPLATE, classname, instance, database );

                  if ( type == typeof( GroupSetting ) )
                        scope = String.Format( SCOPE_TEMPLATE_GROUPSETTING, classname, instance, database );

                  return new ManagementScope( scope );
            }

            private static string GetCreatedClassName (Type type)
            {
                  return (string)
                      type.GetField(
                          "CreatedClassName"
                              , BindingFlags.NonPublic | BindingFlags.Static )
                                  .GetValue( null );
            }
      }
}
