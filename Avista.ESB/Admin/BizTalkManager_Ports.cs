using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.BizTalk.ExplorerOM;
using portStatus = Microsoft.BizTalk.ExplorerOM.PortStatus;
using System.Collections.Specialized;


namespace Avista.ESB.Admin
{
    /// <summary>
    /// </summary>
    public partial class BizTalkManager
    {

        private void CreateBasicHttpSendPort(string sendPortName,string httpUri)
        {

            try
            {
                // create a new static one-way SendPort
                SendPort newPort = catalog.AddNewSendPort(false, true);
                newPort.Name = sendPortName;
                newPort.PrimaryTransport.TransportType =
                    catalog.ProtocolTypes.Cast<ProtocolType>().AsEnumerable().Single(x => x.Name == "WCF-BasicHttp");

                newPort.PrimaryTransport.Address = httpUri;
                newPort.SendPipeline = catalog.Pipelines["Microsoft.BizTalk.DefaultPipelines.XMLTransmit"];

                // create a new dynamic two-way sendPort
                SendPort myDynamicTwowaySendPort = catalog.AddNewSendPort(true, true);
                myDynamicTwowaySendPort.Name = "myDynamicTwowaySendPort1";
                myDynamicTwowaySendPort.SendPipeline = catalog.Pipelines["Microsoft.BizTalk.DefaultPipelines.XMLTransmit"];
                myDynamicTwowaySendPort.ReceivePipeline = catalog.Pipelines["Microsoft.BizTalk.DefaultPipelines.XMLReceive"];

                // persist changes to BizTalk Management database
                catalog.SaveChanges();
            }
            catch 
            {
                catalog.DiscardChanges();
                throw;
            }
        }

        public SendPort GetSendPort(string sendPortName)
        {
            SendPort sendPort = catalog.SendPorts[sendPortName];
            return sendPort;
        }

        /// <summary>
        /// </summary>
        /// <param name="sendPortName"></param>
        /// <param name="xName"></param>
        /// <param name="value"></param>
        public void SetPortTransportProperty(string sendPortName, NameValueCollection nvc)
        {
            //todo: other methods in this class could probably be refactored to leverage this pattern
            SendPort sendPort = GetSendPort(sendPortName);
            SetPortTransportProperty(sendPort, nvc);
        }

        /// <summary>
        /// </summary>
        /// <param name="sendPortName"></param>
        /// <param name="xName"></param>
        /// <param name="value"></param>
        public void SetPortTransportProperty(SendPort sendPort, NameValueCollection nvc)
        {
            //todo: other methods in this class could probably be refactored to leverage this pattern
            try
            {
                foreach (var xName in nvc.AllKeys)
                {
                    SetXPersistTransportProperty(sendPort, xName, nvc[xName]);
                }
                catalog.SaveChanges();
            }
            catch
            {
                catalog.DiscardChanges();
                throw;
            }
        }

        /// <summary>
        ///     ModifySendPrimaryTransport method to modify the SendPort's URI.
        /// </summary>
        /// <param name="sendPort">send port</param>
        /// <param name="proxyUri">Proxy uri (e.g. http://localhost:8080)</param>
        /// <returns></returns>
        public void SetSendPortProxy(SendPort sendPort, string proxyUri)
        {
            SetSendPortProxy(sendPort.Name, proxyUri);
        }

        /// <summary>
        ///     ModifySendPrimaryTransport method to modify the SendPort's URI.
        /// </summary>
        /// <param name="sendPortName">Name of the send port</param>
        /// <param name="proxyUri">Proxy uri (e.g. http://localhost:8080)</param>
        /// <returns></returns>
        public void SetSendPortProxy(string sendPortName, string proxyUri)
        {
            SendPort sendPort = GetSendPort(sendPortName);
            if (sendPort == null) throw new ArgumentException(string.Format("Send port \"{0}\" was not found in the BizTalk catalog.", sendPortName));

            NameValueCollection nvc = new NameValueCollection
            {
                {TransportPropertyNames.ProxyAddress, proxyUri},
                {TransportPropertyNames.ProxyToUse, "UserSpecified"}
            };
            SetPortTransportProperty(sendPortName, nvc);
        }

        public void ClearSendPortProxy(string sendPortName)
        {
            SendPort sendPort = GetSendPort(sendPortName);
            if (sendPort == null) throw new ArgumentException(string.Format("Send port \"{0}\" was not found in the BizTalk catalog.", sendPortName));

            NameValueCollection nvc = new NameValueCollection();
            nvc.Add(TransportPropertyNames.ProxyAddress, string.Empty);
            nvc.Add(TransportPropertyNames.ProxyToUse, "Default");
            SetPortTransportProperty(sendPort, nvc);
        }


        /// <summary>
        ///     only sets - catalog.SaveChanges() must be called later at the caller's disgression
        /// </summary>
        /// <param name="port"></param>
        /// <param name="xName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void SetXPersistTransportProperty(SendPort port, string xName, string value)
        {
            TransportInfo transportInfo = port.PrimaryTransport;
            string transportTypeData = transportInfo.TransportTypeData;
            XDocument transportSettings = XDocument.Parse(transportTypeData);
            transportSettings.Descendants(xName).Single().Value = value;
            transportInfo.TransportTypeData = transportSettings.ToString();
        }

        /// <summary>
        ///     only sets - catalog.SaveChanges() must be called later at the caller's disgression
        /// </summary>
        /// <param name="port"></param>
        /// <param name="xName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public XElement GetTransportProperty(string sendPortName, string xName)
        {
            SendPort port = GetSendPort(sendPortName);
            return GetTransportProperty(port, xName);
        }

        public XElement GetTransportProperty(SendPort port, string xName)
        {
            TransportInfo transportInfo = port.PrimaryTransport;
            string transportTypeData = transportInfo.TransportTypeData;
            XDocument transportSettings = XDocument.Parse(transportTypeData);
            var ret = transportSettings.Descendants(xName).SingleOrDefault();
            return ret;
        }
    }
}

