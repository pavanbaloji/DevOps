using Microsoft.Practices.ESB.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avista.ESB.Utilities
{
    [Serializable]
    public class Utilities
    {
        const string btsMgmtDbConn = "SERVER={0};DATABASE={1};Integrated Security=SSPI;Connect Timeout=120";

        public static string GetOutboundTransportCLSID(string adapterName)
        {
            string outboundTansportCLSID = string.Empty;

            try
            {
                BizTalkServerRegistry bizTalkServerRegistry = default(BizTalkServerRegistry);
                bizTalkServerRegistry = Utility.GetMgmtServerInfo();
                string commandText = "SELECT OutboundEngineCLSID FROM BizTalkMgmtDb.dbo.adm_Adapter WHERE Name = '{0}'";
                using (SqlConnection connection = new SqlConnection(string.Format(btsMgmtDbConn, bizTalkServerRegistry.BizTalkMgmtDb, bizTalkServerRegistry.BizTalkMgmtDbName)))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(string.Format(commandText,adapterName), connection))
                    {
                        connection.Open();
                        object obj = sqlCommand.ExecuteScalar();
                        connection.Close();
                        if (obj != null)
                            outboundTansportCLSID = obj.ToString();
                    }
                }
            }
            catch (Exception) { }

            return outboundTansportCLSID;
        }
    }
}
