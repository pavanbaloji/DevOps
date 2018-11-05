using System;
using System.Runtime.Serialization;
using Avista.ESB.Utilities.Components;
using Microsoft.EnterpriseSingleSignOn.Interop;
using Avista.ESB.Utilities.Logging;
using Avista.ESB.Utilities.Security;
using MSMQT;

namespace Avista.ESB.Utilities.Sso
{
    [Serializable]
    public class SsoTicketProvider : ComponentBase, ITicketProvider
    {


        public SsoTicketProvider()
            : base(typeof(SsoTicketProvider).Name)
        {
            
        }
        /// <summary>
        /// Constructor for SsoTicketProvider.
        /// </summary>
        /// <param name="name">The name for the SsoTicketProvider.</param>
        public SsoTicketProvider(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Initializes a new instance of SsoTicketProvider with serialized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected SsoTicketProvider(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            RefreshConfiguration();
        }


        /// <summary>
        /// Method which issues and returns a ticket.
        /// </summary>
        /// <returns>The issued ticket.</returns>
        public string IssueTicket()
        {
            ISSOTicket sso = new ISSOTicket();
            return sso.IssueTicket(0);
        }

        /// <summary>
        /// Method which issues and returns ticket with the given flags.
        /// </summary>
        /// <returns>The issued ticket.</returns>
        public string IssueTicket(int flags)
        {
            ISSOTicket sso = new ISSOTicket();
            return sso.IssueTicket(flags);
        }

        /// <summary>
        /// Method which redeems a ticket.
        /// </summary>
        /// <returns>The redeemed ticket.</returns>
        public string[] RedeemTicket(string applicationName, string sender, string ticket, int flags, out string externalUserName)
        {
            ISSOTicket sso = new ISSOTicket();
            string[] results = sso.RedeemTicket(applicationName, sender, ticket, flags, out externalUserName);
            return results;
        }

        /// <summary>
        /// Looks up a set of credentials for an affiliate application in SSO.
        /// </summary>
        /// <param name="affiliateApplication">The affiliate application for which credentials should be looked up in SSO.</param>
        /// <returns>The requested credentials.</returns>
        public static string LookupCredentials(string affiliateApplication, out string userId)
        {
            try
            {
                ISSOLookup1 ssoLookup = new ISSOLookup1();
                string[] results = ssoLookup.GetCredentials(affiliateApplication, SSOFlag.SSO_FLAG_REFRESH, out userId);

                //return password
                return results[0];
            }
            catch (Exception ex)
            {
                Logger.WriteWarning(ex.ToString());
                throw ex;
            }

        }
    }
}
