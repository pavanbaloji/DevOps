using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.BizTalk.ExplorerOM;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Avista.ESB.BuildTasks
{
    public class ManageConfigurations : Task
    {
        private string _configSource;
        private string _configRules;
        private string _action;

        public ManageConfigurations()
        {
        }

        [Required]
        public string ConfigSource
        {
            get { return _configSource; }
            set { _configSource = value; }
        }

        [Required]
        public string ConfigRules
        {
            get { return _configRules; }
            set { _configRules = value; }
        }
        [Required]
        public string Action
        {
            get { return _action; }
            set { _action = value; }
        }

        public override bool Execute()
        {
            this.Log.LogMessage("Updating configuration.\n Confi Source: '{0}' \n Config Rules: {1} \n Action: {2}", _configSource, _configRules,_action);

            try
            {
                ConfigEditor editor = new ConfigEditor(this._configSource, this._configRules);
                Action action;
                Enum.TryParse<Action>(this._action, out action);
                editor.ProcessRules(action, this.Log);
            }
            catch(Exception ex)
            {
                this.Log.LogMessage("Error while updating configuration. " + ex.ToString());
            }

            return true;
        }
    }
}
