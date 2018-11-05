using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.BizTalk.ExplorerOM;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Avista.ESB.BuildTasks
{
    public class GetBizTalkAppHasResource : Task
    {
        private bool _hasResources;
        private string _applicationName;

        public GetBizTalkAppHasResource()
        {
        }

        [Required]
        public string ApplicationName
        {
            get { return _applicationName; }
            set { _applicationName = value; }
        }

        [Output]
        public bool HasResources
        {
            get { return _hasResources; }
            set { _hasResources = value; }
        }

        public override bool Execute()
        {
            this.Log.LogMessage("Checking for existence of BizTalk application '{0}'...", _applicationName);

            using (BtsCatalogExplorer catalog = BizTalkCatalogExplorerFactory.GetCatalogExplorer())
            {
                Application application = catalog.Applications[_applicationName];
                if (application != null)
                {
                    if (application.Assemblies.Count > 0
                        || application.ReceivePorts.Count > 0
                        || application.SendPorts.Count > 0
                        || application.SendPortGroups.Count > 0)
                    {
                        _hasResources = true;
                    }
                    else
                    {
                        _hasResources = false;
                    }
                }
                else
                {
                    _hasResources = false;
                }

            }

            if (_hasResources)
            {
                this.Log.LogMessage("Found Resources in BizTalk application '{0}'.", _applicationName);
            }
            else
            {
                this.Log.LogMessage("Did not find Resources in BizTalk application '{0}'.", _applicationName);
            }

            return true;
        }
    }
}
