using Avista.ESB.Utilities.Logging;
using Avista.ESB.Utilities.Sso;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.Practices.ESB.Itinerary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Avista.ESB.MessagingServices.AttachSsoTicket
{
    public class AttachSsoTicketService : IMessagingService
    {
        [Browsable(true), ReadOnly(true)]
        public string Name
        {
            get
            {
                return "Avista.ESB.Utilities.AttachSsoTicket";
            }
        }

        public bool SupportsDisassemble
        {
            get
            {
                return true;
            }
        }

        public bool ShouldAdvanceStep(IItineraryStep step, IBaseMessage msg)
        {
            return true;
        }

        public IBaseMessage Execute(IPipelineContext context, IBaseMessage msg, string resolverString, IItineraryStep step)
        {
            Logger.WriteTrace(string.Format("******{0} Started******",this.GetType().Name));
            try
            {

                string ticket = new SsoTicketProvider().IssueTicket();
                msg.Context.Write("SSOTicket", "http://schemas.microsoft.com/BizTalk/2003/system-properties", ticket);
            }
            catch (Exception ex)
            {
                Logger.WriteTrace(string.Format("Error occured in {0} \r\n Details: {1}", this.GetType().Name, ex.ToString()));
                throw ex;
            }
            Logger.WriteTrace(string.Format("******{0} Completed******", this.GetType().Name));
            return msg;
        }
    }
}
