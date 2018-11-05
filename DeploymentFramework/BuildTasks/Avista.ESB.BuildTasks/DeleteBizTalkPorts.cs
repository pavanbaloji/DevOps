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
    public class DeleteBizTalkPorts : Task
    {
        private string _portBindingsMasterFile;
        private string _applicationName;

        public DeleteBizTalkPorts()
        { }        

        [Required]
        public string PortBindingsMasterFile
        {
            get { return _portBindingsMasterFile; }
            set { _portBindingsMasterFile = value; }
        }
        [Required]
        public string ApplicationName
        {
            get { return _applicationName; }
            set { _applicationName = value; }
        }

        public override bool Execute()
        {
            this.Log.LogMessage("Removing ports from Bindingfile '{0}'...", _portBindingsMasterFile);
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(_portBindingsMasterFile);
            using (BtsCatalogExplorer catalog = BizTalkCatalogExplorerFactory.GetCatalogExplorer())
            {
                Application application = catalog.Applications[_applicationName];
                try
                {
                    //Removing Receive Ports
                    XmlNodeList Recieveport = xmldoc.SelectNodes("BindingInfo/ReceivePortCollection/ReceivePort");
                    foreach (XmlNode xndNode in Recieveport)
                    {                        
                        string name = xndNode.Attributes["Name"].Value;
                        foreach (ReceivePort receivePort in application.ReceivePorts)
                        {
                            if (receivePort.Name == name)
                            {
                                catalog.RemoveReceivePort(receivePort);
                                break;
                            }
                        }
                    }
                    
                    //Removing Send Port Groups
                    XmlNodeList SendportGroup = xmldoc.SelectNodes("BindingInfo/DistributionListCollection/DistributionList");
                    foreach (XmlNode xndNode in SendportGroup)
                    {
                        string name = xndNode.Attributes["Name"].Value;
                        foreach (SendPortGroup sendPortGroup in application.SendPortGroups)
                        {
                            if (sendPortGroup.Name == name)
                            {
                                sendPortGroup.Status = PortStatus.Bound;
                                catalog.RemoveSendPortGroup(sendPortGroup);
                                break;
                            }
                        }
                    }

                    //Removing Send Ports
                    XmlNodeList Sendport = xmldoc.SelectNodes("BindingInfo/SendPortCollection/SendPort");
                    foreach (XmlNode xndNode in Sendport)
                    {
                        string name = xndNode.Attributes["Name"].Value;
                        foreach (SendPort sendPort in application.SendPorts)
                        {
                            if (sendPort.Name == name)
                            {
                                sendPort.Status = PortStatus.Bound;
                                catalog.RemoveSendPort(sendPort);
                                break;
                            }
                        }
                    }
                    catalog.SaveChanges();                   
                }
                catch (Exception ex)
                {                    
                    this.Log.LogMessage("Error In removing ports. " + ex.ToString());
                }
            }
            return true;        
        }
    }
}
