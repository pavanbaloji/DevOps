using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.BizTalk.Management;

namespace Avista.ESB.Admin
{
      public class BtsHostInstanceCollection : BizTalkCollection<BizTalkHostInstance>
      {
            public BtsHostInstanceCollection (BizTalkCatalog catalog)
                  : base(
                        catalog
                      , HostInstance.GetInstances(
                          ManagementHelper.GetScope(typeof( HostInstance ), catalog.Instance, catalog.Database ), null, null ) )
            {
            }
      }
}