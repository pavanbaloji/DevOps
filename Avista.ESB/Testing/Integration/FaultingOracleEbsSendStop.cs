using System;
using System.Net;

namespace Avista.ESB.Testing.Integration
{
    public class FaultingOracleEbsSendStop : OracleEbsSendStop, IFaultingSendPort
    {
        public FaultingOracleEbsSendStop(string sendPortName, string soapAction)
            : base(sendPortName, soapAction, null)
        {
        }

        protected override string PortKey
        {
            get
            {
                return String.Format("{0}:{1}", SendPortName, "FaultingSendStop");
            }
        }

        public void SetErrorHttpResponse(HttpListenerResponse response)
        {
            response.StatusCode = 503; // Service Unavailable
            response.StatusDescription = "Moxy is simulating a \"down OracelEBS\" condition.";
        }
    }
}