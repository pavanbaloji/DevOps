using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Avista.ESB.Admin;

namespace Avista.ESB.Testing.Integration
{
    public class OracleEbsSendStop : BizTalkSendStop
    {
        public static Dictionary<string, string> OrginalSendPortUriDict = new Dictionary<string, string>();

        protected virtual string PortKey
        {
            get
            {
                return String.Format("{0}:{1}", SendPortName, SoapAction);
            }
        }

        public OracleEbsSendStop(string sendPortName, string soapAction, string simulatedOutput)
            : base(sendPortName, soapAction)
        {
            SimulatedOutput = (s, objects) => WashInSoap(simulatedOutput);
        }

        public override void Setup(BizTalkManager manager, Uri moxyUri)
        {

            var uri = manager.GetSendPortPrimaryTransportAddress(SendPortName);

            if (!OrginalSendPortUriDict.ContainsKey(PortKey))
            {
                OrginalSendPortUriDict.Add(PortKey, uri);
            }

            
            NameValueCollection nvc = new NameValueCollection
            {
                {"BindingType", "basicHttpBinding"},
                {"BindingConfiguration", @"<binding name=""basicHttpBinding""/>"}
            };

            manager.ModifySendPortPrimaryTransportAddress(SendPortName, moxyUri.AbsoluteUri);
            manager.SetPortTransportProperty(SendPortName, nvc);
            base.Setup(manager, moxyUri);
        }

        public override void Cleanup(BizTalkManager manager)
        {
            NameValueCollection nvc = new NameValueCollection
            {
                {"BindingType", "oracleEBSBinding"},
                {
                    "BindingConfiguration",
                    @"<binding openTimeout=""00:05:00"" name=""OracleEBSBinding"" closeTimeout=""00:05:00"" sendTimeout=""00:05:00"" useSchemaInNameSpace=""false"" />"
                }
            };

            manager.SetPortTransportProperty(SendPortName, nvc);

            var origSendPortUri = OrginalSendPortUriDict[PortKey];
            if (!string.IsNullOrWhiteSpace(origSendPortUri))
            {
                manager.ModifySendPortPrimaryTransportAddress(SendPortName, origSendPortUri);
            }
        }

    }
}