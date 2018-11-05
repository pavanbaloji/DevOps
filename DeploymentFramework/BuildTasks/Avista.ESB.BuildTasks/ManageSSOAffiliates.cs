using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.BizTalk.ExplorerOM;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.Xml;

namespace Avista.ESB.BuildTasks
{
    public class ManageSSOAffiliates : Task
    {
        private string _action;
        private string _ssoAffiliateConfiguration;

        public ManageSSOAffiliates()
        { }

        [Required]
        public string Action
        {
            get { return _action; }
            set { _action = value; }
        }

        [Required]
        public string SsoAffiliateConfiguration
        {
            get { return _ssoAffiliateConfiguration; }
            set { _ssoAffiliateConfiguration = value; }
        }

        public override bool Execute()
        {
            this.Log.LogMessage("Updating SSO Affiliate.\n SSO Affiliate Config : {0} \n Action: {1}", _ssoAffiliateConfiguration, _action);
            try
            {
                Action action;
                Enum.TryParse<Action>(this._action, out action);
                ManageSSO(action, _ssoAffiliateConfiguration);

            }
            catch (Exception ex)
            {
                this.Log.LogMessage("Error while ManageSSOAffiliate. " + ex.ToString());
            }

            return true;
        }

        private void ManageSSO(Action action, string inputFile)
        {
            try
            {
                SsoManager ssoManager = new SsoManager();
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(inputFile);
                XmlNodeList xnlNodes = xmldoc.SelectNodes("SSOAffiliates/Affiliate");
                foreach (XmlNode xndNode in xnlNodes)
                {
                    XmlElement element = (XmlElement)xndNode;
                    string affiliateName = xndNode.Attributes["name"].Value;
                    
                    if (action == BuildTasks.Action.Remove)
                    {
                        if (ssoManager.Exists(affiliateName))
                        {
                            ssoManager.Delete(affiliateName);
                        }
                        else
                        {
                            this.Log.LogMessage("SSO Affiliate does not exists '{0}'.", affiliateName);
                        }
                    }
                    else if (action == BuildTasks.Action.Add)
                    {
                        if (ssoManager.Exists(affiliateName))
                        {
                            this.Log.LogMessage("Application already exists '{0}'.", affiliateName);

                            if (xndNode.ChildNodes.Count > 0)
                                MapAccounts(xndNode.ChildNodes, ssoManager, affiliateName);
                        }
                        else
                        {
                            string type = xndNode.Attributes["type"].Value;
                            AffiliateApplicationType affiliateApplicationType = (AffiliateApplicationType)Enum.Parse(typeof(AffiliateApplicationType), type, true);
                            string description = xndNode.Attributes["description"].Value;
                            string contact = xndNode.Attributes["contact"].Value;
                            string userGroup = xndNode.Attributes["adminGroup"].Value;
                            string adminGroup = xndNode.Attributes["userGroup"].Value;

                            bool allowLocal = true;
                            Boolean.TryParse(xndNode.Attributes["allowLocal"].Value, out allowLocal);
                            bool allowTickets = true;
                            Boolean.TryParse(xndNode.Attributes["allowTickets"].Value, out allowTickets);

                            ssoManager.Create(affiliateName, affiliateApplicationType,
                                                          description,
                                                          contact,
                                                          userGroup,
                                                          adminGroup,
                                                          allowTickets,
                                                          allowLocal);

                            this.Log.LogMessage("SSO Affiliate '{0}' Created.", affiliateName);

                            if (xndNode.ChildNodes.Count > 0)
                                  MapAccounts(xndNode.ChildNodes, ssoManager, affiliateName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Log.LogMessage("Error while ManageSSOAffiliate. " + ex.ToString());
            }

           
        }

        private void MapAccounts(XmlNodeList xmlNodeList, SsoManager ssoManager, string affiliateName)
        {
            foreach (XmlNode childNode in xmlNodeList)
            {
                string accounts = childNode.Attributes["accounts"].Value;
                string externalUserid = childNode.Attributes["remoteUserId"].Value;
                string externalPassword = childNode.Attributes["remotePassword"].Value;

                ssoManager.Map(affiliateName, accounts, externalUserid, externalPassword);
                this.Log.LogMessage("Mapping created for accounts '{0}'.", accounts);

            }
        }
    }
}