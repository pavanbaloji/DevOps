using System;
using Avista.ESB.Admin;


namespace Avista.ESB.Testing.Integration
{
    public abstract class BizTalkSendStop : IBizTalkSendStop
    {
        protected BizTalkSendStop(string sendPortName, string soapAction)
        {
            if (string.IsNullOrEmpty(sendPortName)) throw new ArgumentNullException("sendPortName");
            SendPortName = sendPortName;
            //UNTIL WE HAVE A NEED TO MOVE THE SoapAction out we won't.....
            SoapAction = soapAction;
            RequestCaught = false;
        }

        public object[] ExtraIncomingArgs { get; set; }

        /// <summary>
        ///     If set, the proxy will check this value
        /// </summary>
        public string ExpectedIncomingPayload { get; set; }


        public string SendPortName { get; private set; }

        /// <summary>
        ///     If this is set to something, the proxy will return this instead of going on to whatever was requested
        ///     This can theoretically be used to phase out all the mock services
        /// </summary>
        public Func<string, object[], string> SimulatedOutput { get; set; }

        public virtual void Setup(BizTalkManager manager, Uri moxyUri)
        {
            SendPortUri = new Uri(manager.GetSendPortPrimaryTransportAddress(SendPortName));
        }
        public abstract void Cleanup(BizTalkManager manager);

        public string SoapAction { get; private set; }

        public static string WashInSoap(string thingToWash)
        {
            string ret =
                string.Format(
                    @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/""><s:Body xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">{0}</s:Body></s:Envelope>",
                    thingToWash);
            return ret;
        }

        public Uri SendPortUri { get; protected set; }
        public bool RequestCaught { get; set; }
    }
}