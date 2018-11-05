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
      public class LookUpDbValueUsingJoin: BaseFunctoid
      {
             /// <summary>
        /// GetOrgId
        /// </summary>
        public LookUpDbValueUsingJoin()
            : base()
        {
              this.ID = 10002;

            SetupResourceAssembly("Avista.ESB.Functoids.Resources", Assembly.GetExecutingAssembly());
            SetName("LOOKUPDBVALUEUSINGJOIN_NAME");
            SetTooltip("LOOKUPDBVALUEUSINGJOIN_TOOLTIP");
            SetDescription("LOOKUPDBVALUEUSINGJOIN_DESCRIPTION");
            SetBitmap("LOOKUPDBVALUEUSINGJOIN_BITMAP");

            this.Category = FunctoidCategory.DatabaseLookup;
            this.SetMinParams(11);
            this.SetMaxParams(11);

            AddInputConnectionType(ConnectionType.AllExceptRecord);
            AddInputConnectionType(ConnectionType.AllExceptRecord);
            AddInputConnectionType(ConnectionType.AllExceptRecord);
            AddInputConnectionType(ConnectionType.AllExceptRecord);
            AddInputConnectionType(ConnectionType.AllExceptRecord);
            AddInputConnectionType(ConnectionType.AllExceptRecord);
            AddInputConnectionType(ConnectionType.AllExceptRecord);
            AddInputConnectionType(ConnectionType.AllExceptRecord);
            AddInputConnectionType(ConnectionType.AllExceptRecord);
            AddInputConnectionType(ConnectionType.AllExceptRecord);
            AddInputConnectionType(ConnectionType.AllExceptRecord);

            this.OutputConnectionType = ConnectionType.AllExceptRecord;

            SetExternalFunctionName(GetType().Assembly.FullName, "Avista.ESB.Functoids.LookUpDbValueUsingJoin", "GetLookUpDbValueUsingJoin");
        }

        /// <summary>
        /// Get organization id implementation.
        /// </summary>
        /// <param name="fetchColumnName">Column name of value to be returned</param>
        /// <param name="firstSchemaName">Schema name for the first table in a join query.</param>
        /// <param name="firstTableName">First table name in the join query.</param>
        /// <param name="firstColumnName">First filter column name.</param>
        /// <param name="firstColumnValue">First Filter column value.</param>
        /// <param name="secondSchemaName">Schema name for the second table.</param>
        /// <param name="secondTableName">Second table name.</param>
        /// <param name="secondColumnName">Second filter column name.</param>
        /// <param name="secondColumnValue">Second Filter column value</param>
        /// <param name="sqlOperator">Sql operator =, like, <=, != ,>= etc.</param>
        /// <param name="connectionName">Connection string identifier name.</param>
        /// <returns></returns>
        public string GetLookUpDbValueUsingJoin(string fetchColumnName,
                                           string firstSchemaName,
                                           string firstTableName,
                                           string firstColumnName,
                                           string firstColumnValue,
                                           string secondSchemaName,
                                           string secondTableName,
                                           string secondColumnName,
                                           string secondColumnValue,
                                           string sqlOperator,
                                           string connectionName)
        {
            string value = string.Empty;
            string sql = string.Empty;
            DatabaseConnection connection = null;
            try
            {
                sql = "SELECT T1." + fetchColumnName + " FROM " + firstSchemaName + "." + firstTableName + " T1," + secondSchemaName + "." + secondTableName + " T2 WHERE T1." + firstColumnName + " " + sqlOperator + "'" + firstColumnValue + "%' AND T1.DATE_TO IS NULL AND T1.ORGANIZATION_ID=T2.ORGANIZATION_ID AND T2." + secondColumnName + "='" + secondColumnValue + "' AND T2.INACTIVE_DATE IS NULL AND ROWNUM=1";
                connection = new DatabaseConnection(connectionName);
                connection.RefreshConfiguration();
                connection.Open();
                value = connection.ExecuteScalar(sql);
            }
            catch (Exception exception)
            {
                Logger.WriteError(string.Concat("Error in GetLookUpDbValueUsingJoin functoid. SQL = " + sql, "\r\n", exception.StackTrace),124);
                  throw;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return value;
        }
    }
}
