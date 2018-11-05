using System;
using Avista.ESB.Admin;


namespace Avista.ESB.Testing.Integration
{
    public class FileSendStop : BizTalkSendStop
    {
        public FileSendStop(string sendPortName, string soapAction) :
            base(sendPortName, soapAction)
        {
            //UNTIL WE HAVE A NEED TO MOVE THE SoapAction out we won't.....

            throw new NotImplementedException("This type of BizTalk Send Stop has not been implemented yet.");
        }

        public override void Setup(BizTalkManager manager, Uri moxyUri)
        {
            throw new NotImplementedException("This type of BizTalk Send Stop has not been implemented yet.");
        }

        public override void Cleanup(BizTalkManager manager)
        {
            throw new NotImplementedException("This type of BizTalk Send Stop has not been implemented yet.");
        }
    }
}