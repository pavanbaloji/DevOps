using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avista.ESB.Utilities.DataAccess;
using Avista.ESB.Utilities.Logging;
using System.Reflection;
using Microsoft.BizTalk.BaseFunctoids;

namespace Avista.ESB.Functoids
{
      public class LookupDbValueFunctoid: BaseFunctoid
      { 
            /// <summary>
            /// LookupDbValueFunctoid
            /// </summary>
            public LookupDbValueFunctoid ()
                  : base()
            {
                  this.ID = 10001;

                  SetupResourceAssembly( "Avista.ESB.Functoids.Resources", Assembly.GetExecutingAssembly() );
                  SetName( "LOOKUPDBVALUE_NAME" );
                  SetTooltip( "LOOKUPDBVALUE_TOOLTIP" );
                  SetDescription( "LOOKUPDBVALUE_DESCRIPTION" );
                  SetBitmap( "LOOKUPDBVALUE_BITMAP" );

                  this.Category = FunctoidCategory.DatabaseLookup;
                  this.SetMinParams( 6 );
                  this.SetMaxParams( 6 );

                  AddInputConnectionType( ConnectionType.AllExceptRecord );
                  AddInputConnectionType( ConnectionType.AllExceptRecord );
                  AddInputConnectionType( ConnectionType.AllExceptRecord );
                  AddInputConnectionType( ConnectionType.AllExceptRecord );

                  this.OutputConnectionType = ConnectionType.AllExceptRecord;

                  SetExternalFunctionName( GetType().Assembly.FullName, "Avista.ESB.Functoids.LookupDbValueFunctoid", "LookupDbValue" );
            }

            /// <summary>
            /// LookupDbValue implementation.
            /// </summary>
            /// <param name="fetchColumnName">Column name of value to be returned</param>
            /// <param name="schemaName">Table schema name</param>
            /// <param name="tableName">Table name</param>
            /// <param name="columnName">Filter column name</param>
            /// <param name="columnValue">Filter column value</param>
            /// <param name="connectionName">Connection string identifier name</param>
            /// <returns></returns>
            public string LookupDbValue (string fetchColumnName, string schemaName, string tableName, string columnName, string columnValue, string connectionName)
            {
                  string value = string.Empty;
                  string sql = string.Empty;
                  DatabaseConnection connection = null;
                  try
                  {
                        sql = "SELECT " + fetchColumnName + " FROM " + schemaName + "." + tableName + " WHERE " + columnName + " = '" + columnValue + "'";
                        connection = new DatabaseConnection( connectionName );
                        connection.RefreshConfiguration();
                        connection.Open();
                        value = connection.ExecuteScalar( sql );
                  }
                  catch ( Exception exception )
                  {
                        Logger.WriteError( "Error in LookUpDbValue functoid. SQL = " + sql + "\r\n" + exception.StackTrace,123);
                        throw;
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
