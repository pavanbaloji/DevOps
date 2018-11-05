using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.BizTalk.BaseFunctoids;
using Avista.ESB.Utilities.DataAccess;
using Avista.ESB.Utilities.Logging;
using System.Threading;

namespace Avista.ESB.Functoids
{

      /// <summary>
      /// ExecuteSqlFunctoid
      /// </summary>
      public class ExecuteSqlFunctoid:BaseFunctoid
      {
            const int _MaxConnectionAttempts = 3;
            public int ConnectionAttempts
            {
                  get;
                  set;
            }
            public ExecuteSqlFunctoid ()
                  : base()
            {
                  this.ID = 10004;
                  ConnectionAttempts = 0;

                  SetupResourceAssembly( "Avista.ESB.Functoids.Resources", Assembly.GetExecutingAssembly() );
                  SetName( "EXECUTESQL_NAME" );
                  SetTooltip( "EXECUTESQL_TOOLTIP" );
                  SetDescription( "EXECUTESQL_DESCRIPTION" );
                  SetBitmap( "EXECUTESQL_BITMAP" );

                  this.Category = FunctoidCategory.DatabaseLookup;
                  this.SetMinParams( 6 );
                  this.SetMaxParams( 6 );

                  AddInputConnectionType( ConnectionType.AllExceptRecord ); // sql
                  AddInputConnectionType( ConnectionType.AllExceptRecord ); // affiliate name
                  AddInputConnectionType( ConnectionType.AllExceptRecord );
                  AddInputConnectionType( ConnectionType.AllExceptRecord );
                  AddInputConnectionType( ConnectionType.AllExceptRecord );
                  AddInputConnectionType( ConnectionType.AllExceptRecord );

                  this.OutputConnectionType = ConnectionType.AllExceptRecord;

                  SetExternalFunctionName( GetType().Assembly.FullName, "Avista.ESB.Functoids.ExecuteSqlFunctoid", "ExecuteSql" );
            }

            /// <summary>
            /// ExecuteSqlFunctoid implementation.
            /// </summary>
            public string ExecuteSql (string template, string connectionName, string arg1, string arg2, string arg3, string arg4)
            {
                  string sql = "";
                  string value = string.Empty;
                  DatabaseConnection connection = null;
                  try
                  {
                        sql = String.Format( template, arg1, arg2, arg3, arg4 );
                        connection = new DatabaseConnection( connectionName );
                        connection.RefreshConfiguration();
                        connection.Open();
                        value = connection.ExecuteScalar( sql );

                        if ( string.IsNullOrWhiteSpace( value ) )
                        {
                              Logger.WriteWarning( "No value was return by the ExecuteSql statement : " + sql );
                        }

                  }
                  catch ( Exception exception )
                  {
                        ConnectionAttempts++;
                        if ( ConnectionAttempts <= _MaxConnectionAttempts )
                        {
                              Logger.WriteInformation( string.Format( "The following exception occurred, but the statement will be attempted at least {0} times.  Current number of attempts: {1}; Error Message: {2}", _MaxConnectionAttempts, ConnectionAttempts, exception.Message ) );
                              Thread.Sleep( 30000 ); // Wait for 30 seconds and try again.
                              ExecuteSql( template, connectionName, arg1, arg2, arg3, arg4 );
                        }
                        else
                        {
                              Logger.WriteError( "Error in ExecuteSqlFunctoid functoid. SQL = " + sql + "\r\n" + exception.StackTrace,126);
                              throw;
                        }
                  }
                  finally
                  {
                        if ( connection != null )
                        {
                              connection.Close();
                        }
                  }
                  return value;
            }
      }
}
