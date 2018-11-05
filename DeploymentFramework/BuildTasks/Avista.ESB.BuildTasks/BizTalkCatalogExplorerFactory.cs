using System;
using Microsoft.Win32;
using Microsoft.BizTalk.ExplorerOM;

namespace Avista.ESB.BuildTasks
{
    public class BizTalkCatalogExplorerFactory
    {
        public static BtsCatalogExplorer GetCatalogExplorer()
        {
            string mgmtServer = null;
            string mgmtDbName = null;

            using (RegistryKey rk = Registry.LocalMachine)
            {
                using (RegistryKey rk2 = rk.OpenSubKey(@"SOFTWARE\Microsoft\BizTalk Server\3.0\Administration"))
                {
                    mgmtServer = (string)rk2.GetValue("MgmtDBServer");
                    mgmtDbName = (string)rk2.GetValue("MgmtDBName");
                }
            }

            BtsCatalogExplorer catalog = new BtsCatalogExplorer();
            catalog.ConnectionString =
                string.Format("Server={0};Initial Catalog={1};Integrated Security=SSPI;", mgmtServer, mgmtDbName);
            return catalog;
        }
    }
}
