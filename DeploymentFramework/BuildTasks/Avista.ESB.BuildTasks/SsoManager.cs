using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EnterpriseSingleSignOn.Interop;

namespace Avista.ESB.BuildTasks
{
    public enum AffiliateApplicationType
    {
        None = 0,
        Individual = 1,
        Group = 2,
        ConfigStore = 4,
        HostGroup = 8,
        PasswordSyncAdapter = 16,
        PasswordSyncGroupAdapter = 32,
        All = 268435455,
    }

    public class SsoManager
    {
        public SsoManager()
        {
        }

        /// <summary>
        /// Create an SSO application.
        /// </summary>
        /// <param name="application">The name of the affiliate application.</param>
        /// <param name="type">The type of affiliate application.</param>
        /// <param name="description">A description for the affiliate application.</param>
        /// <param name="contact">Contact information for administering the affiliate application.</param>
        /// <param name="userGroup">The windows group for users that will use the affiliate application.</param>
        /// <param name="adminGroup">The windows group for administrators of the affiliate application.</param>
        /// <param name="tickets">A flag indicating if the affiliate application can be used to create SSO tickets.</param>
        /// <param name="local">A flag indicating whether or not local user accounts can use the affiliate application.</param>
        public void Create(string application, AffiliateApplicationType type, string description, string contact, string userGroup, string adminGroup, bool tickets, bool local)
        {
            try
            {
                int flags = 0;
                int fields = 2;
                // Set flags.
                if (type == AffiliateApplicationType.Individual)
                {
                    flags = SSOFlag.SSO_FLAG_SSO_WINDOWS_TO_EXTERNAL;
                }
                else if (type == AffiliateApplicationType.Group)
                {
                    flags = SSOFlag.SSO_WINDOWS_TO_EXTERNAL;
                    flags = flags | SSOFlag.SSO_FLAG_APP_GROUP;
                    flags = flags | SSOFlag.SSO_FLAG_APP_USES_GROUP_MAPPING;
                }
                else if (type == AffiliateApplicationType.HostGroup)
                {
                    flags = SSOFlag.SSO_EXTERNAL_TO_WINDOWS;
                    flags = flags | SSOFlag.SSO_FLAG_APP_GROUP;
                    flags = flags | SSOFlag.SSO_FLAG_APP_USES_GROUP_MAPPING;
                }
                else
                {
                    throw new Exception("Unsupported affiliate application type: " + type.ToString() + ".");
                }
                if (tickets)
                {
                    flags = flags | SSOFlag.SSO_FLAG_ALLOW_TICKETS | SSOFlag.SSO_FLAG_VALIDATE_TICKETS;
                }
                if (local)
                {
                    flags = flags | SSOFlag.SSO_FLAG_APP_ALLOW_LOCAL;
                }
                // Create the application
                ISSOAdmin ssoAdmin = new ISSOAdmin();
                ssoAdmin.CreateApplication(application, description, contact, userGroup, adminGroup, flags, fields);
                // Create fields used by the application
                flags = 0;
                ssoAdmin.CreateFieldInfo(application, "User ID", flags);
                flags = SSOFlag.SSO_FLAG_FIELD_INFO_MASK | SSOFlag.SSO_FLAG_FIELD_INFO_SYNC;
                ssoAdmin.CreateFieldInfo(application, "Password", flags);
                // Enable the application
                ssoAdmin.UpdateApplication(application, null, null, null, null, SSOFlag.SSO_FLAG_ENABLED, SSOFlag.SSO_FLAG_ENABLED);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to create affiliate application " + application + ". " + e.Message);
            }
        }

        /// <summary>
        /// Deletes an SSO affiliate application.
        /// </summary>
        /// <param name="application">The name of the affiliate application to be deleted.</param>
        public void Delete(string application)
        {
            try
            {
                if (!Exists(application))
                {
                    throw new Exception("The application does not exist.");
                }
                else
                {
                    ISSOAdmin ssoAdmin = new ISSOAdmin();
                    ssoAdmin.DeleteApplication(application);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Failed to delete application " + application + ". " + e.Message);
            }
        }

        /// <summary>
        /// Determines whether or not an affiliate application exists.
        /// </summary>
        /// <param name="application">The name of the affiliate application to check for existence.</param>
        public bool Exists(string application)
        {
            bool exists = false;
            string description = "";
            string contact = "";
            string userGroup = "";
            string adminGroup = "";
            int flags = 0;
            int fields = 0;
            try
            {
                ISSOAdmin ssoAdmin = new ISSOAdmin();
                ssoAdmin.GetApplicationInfo(application, out description, out contact, out userGroup, out adminGroup, out flags, out fields);
                exists = true;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("The application does not exist."))
                {
                    exists = false;
                }
                else
                {
                    throw new Exception("Failed to load information for affiliate application " + application + ". " + e.Message);
                }
            }
            return exists;
        }

        /// <summary>
        /// Adds an account mapping to an affiliate application.
        /// </summary>
        /// <param name="application">The affiliate application to which the account mapping will be added.</param>
        /// <param name="accounts">The account(s) that will use the mapping.</param>
        /// <param name="externalUserid">The external user id to map the accounts to.</param>
        /// <param name="externalPassword">The external password to map the accounts to.</param>
        public void Map(string application, string accounts, string externalUserid, string externalPassword)
        {
            try
            {
                string[] accountList = accounts.Split(';');
                foreach (string account in accountList)
                {
                    string accountDomain = account.Substring(0, account.IndexOf('\\'));
                    string accountName = account.Substring(account.IndexOf('\\') + 1);
                    try
                    {
                        // Create mapping.
                        ISSOMapping mapping = new ISSOMapping();
                        mapping.ApplicationName = application;
                        mapping.WindowsDomainName = accountDomain;
                        mapping.WindowsUserName = accountName;
                        mapping.ExternalUserName = externalUserid;
                        mapping.Create(SSOFlag.SSO_FLAG_ENABLED);
                        // Set credentials.
                        ISSOMapper mapper = new ISSOMapper();
                        string[] credentials = new string[] { externalPassword };
                        mapper.SetExternalCredentials(application, externalUserid, ref credentials);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to map account " + accountDomain + "\\" + accountName + " to external user " + externalUserid + " for application " + application + ". " + e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Failed to map account(s) " + accounts + " to application " + application + ". " + e.Message);
            }
        }
    }
}
