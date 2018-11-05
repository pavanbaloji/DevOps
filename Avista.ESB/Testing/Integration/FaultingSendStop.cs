using System.Net;

namespace Avista.ESB.Testing.Integration
{
    public class FaultingSendStop : WcfBasicSendStop, IFaultingSendPort
    {
        public FaultingSendStop(string sendPortName, string soapAction) : base(sendPortName, soapAction)
        {
        }

        public void SetErrorHttpResponse(HttpListenerResponse response)
        {
            response.StatusCode = 503; // Service Unavailable
            response.StatusDescription = "Moxy is simulating a \"down service\" condition.";
        }
    }
}