using System;
using Avista.ESB.Admin;


namespace Avista.ESB.Testing.Integration
{
    public class WcfWsSendStop : BizTalkSendStop
    {
        public WcfWsSendStop(string sendPortName, string soapAction)
            : base(sendPortName, soapAction)
        {
        }

        public override void Setup(BizTalkManager manager, Uri moxyUri)
        {
            manager.SetSendPortProxy(SendPortName, moxyUri.AbsoluteUri);
            base.Setup(manager, moxyUri);
        }

        public override void Cleanup(BizTalkManager manager)
        {
            manager.ClearSendPortProxy(SendPortName);
        }
    }
}