using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace Avista.ESB.WcfExtensions.WebHttpHeader
{
    public class FixHeaderInspector : IDispatchMessageInspector
    {

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            // get current "To" prop
            var toUri = request.Headers.To;

            if (toUri.ToString().Length > 255)   
            {
                // truncate all param values
                var trucatedUri = string.Format("http://{0}:{1}{2}", toUri.Host, toUri.Port, toUri.AbsolutePath);
                // set trucated val back to prop
                request.Headers.To = new Uri(trucatedUri);
            }

            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
        }
    
    }
}
