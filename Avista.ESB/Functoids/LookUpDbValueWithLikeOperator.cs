using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.BizTalk.BaseFunctoids;
using Avista.ESB.Utilities.Logging;
using Avista.ESB.Utilities.DataAccess;
namespace Avista.ESB.Functoids
{
      public class LookUpDbValueWithLikeOperator:BaseFunctoid
      { /// <summary>
            /// LookUpDbValueWithLikeOperator
            /// </summary>
            public LookUpDbValueWithLikeOperator ()
                  : base()
            {
                  this.ID = 10003;

                  SetupResourceAssembly( "Avista.ESB.Functoids.Resources", Assembly.GetExecutingAssembly() );
                  SetName( "LOOKUPDBVALUEWITHLIKEOPERATOR_NAME" );
                  SetTooltip( "LOOKUPDBVALUEWITHLIKEOPERATOR_TOOLTIP" );
                  SetDescription( "LOOKUPDBVALUEWITHLIKEOPERATOR_DESCRIPTION" );
                  SetBitmap( "LOOKUPDBVALUE_BITMAP" );

                  this.Category = FunctoidCategory.DatabaseLookup;
                  this.SetMinParams( 6 );
                  this.SetMaxParams( 6 );

                  AddInputConnectionType( ConnectionType.AllExceptRecord );
                  AddInputConnectionType( ConnectionType.AllExceptRecord );
                  AddInputConnectionType( ConnectionType.AllExceptRecord );
                  AddInputConnectionType( ConnectionType.AllExceptRecord );
                  this.OutputConnectionType = ConnectionType.AllExceptRecord;

                  SetExternalFunctionName( GetType().Assembly.FullName, "Avista.ESB.Functoids.LookUpDbValueWithLikeOperator", "LookupDbValueWithLike" );
            }

            /// <summary>
            /// LookUpDbValueWithLikeOperator implementation.
            /// </summary>
            /// <param name="fetchColumnName">Column name of value to be returned</param>
            /// <param name="schemaName">Table schema name</param>
            /// <param name="tableName">Table name</param>
            /// <param name="columnName">Filter column name</param>
            /// <param name="columnValue">Filter column value</param>
            /// <param name="connectionName">Connection string identifier name</param>
            /// <returns></returns>
            public string LookupDbValueWithLike (string fetchColumnName, string schemaName, string tableName, string columnName, string columnValue, string connectionName)
            {
                  string value = string.Empty;
                  string sql = string.Empty;
                  DatabaseConnection connection = null;
                  try
                  {
                        if ( !string.IsNullOrEmpty( columnValue ) )
                        {
                              sql = "SELECT " + fetchColumnName + " FROM " + schemaName + "." + tableName + " WHERE " + columnName + " Like '" + columnValue + "%' AND END_DATE_ACTIVE IS NULL AND ROWNUM=1";
                              connection = new DatabaseConnection( connectionName );
                              connection.RefreshConfiguration();
                              connection.Open();
                              value = connection.ExecuteScalar( sql );
                        }
                  }
                  catch ( Exception exception )
                  {
                        Logger.WriteError( string.Concat( "Error in LookUpDbValueWithLikeOperator functoid. SQL = " + sql, "\r\n", exception.StackTrace ), 125 );
                        throw;
                  }
                  return value;
            }
      }
}

