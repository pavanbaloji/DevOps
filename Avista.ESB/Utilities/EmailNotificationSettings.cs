using Avista.ESB.Utilities.Configuration;
using Avista.ESB.Utilities.Logging;
using System;

namespace Avista.ESB.Utilities
{
    [Serializable]
    public class EmailNotificationSettings
    {
        private static EmailNotificationSettingsCollection emailNotificationSettingsCollection = null;
        private static string smtpServerName = "";
        private static string defaultFromAddress = "";
        private static string emailSubject = "";

        static EmailNotificationSettings()
        {
            try
            {
                AvistaESBCommonSection section = AvistaESBCommonSection.GetSection();
                emailNotificationSettingsCollection = section.EmailNotificationSettings;
                smtpServerName = section.EmailNotificationSettings.SmtpServerName;
                defaultFromAddress = section.EmailNotificationSettings.DefaultFromAddress;
                emailSubject = section.EmailNotificationSettings.EmailSubject;
            }
            catch (Exception)
            {
                // If the values cannot be loaded, default values will be used.
            }
        }

        public string LookupEmailAddress(string groupName)
        {
            string returnValue = "";

            foreach (EmailNotificationSettingElement emailNotificationSetting in emailNotificationSettingsCollection)
            {
                if (emailNotificationSetting.GroupName == groupName)
                {
                    returnValue = emailNotificationSetting.EmailId;
                    break;
                }
            }

            if (string.IsNullOrEmpty(returnValue))
            {
                string eventMessage = "Unable to determine the To email address based on existing configuration settings in application configuration file.";
                Logger.WriteWarning(eventMessage, 0);
            }

            return returnValue;
        }

        /// <summary>
        /// Returns default SMTP server defined in the application configuration file.
        /// </summary>
        public string SmtpServerName
        {
            get
            {
                if (string.IsNullOrEmpty(smtpServerName))
                {
                    string eventMessage = "smtpServerName property has not been initialized in <hp.practices.biztalk>\\<emailNotificationSettings> section.";
                    Logger.WriteWarning(eventMessage, 0);                    
                }
                return smtpServerName;
            }
        }

        /// <summary>
        /// Returns default 'From' address to send out email notifications defined in the application configuration file.
        /// </summary>
        public string DefaultFromAddress
        {
            get
            {
               if (string.IsNullOrEmpty(defaultFromAddress))
                {
                    string eventMessage = "defaultFromAddress property has not been initialized in <hp.practices.biztalk>\\<emailNotificationSettings> section.";
                    Logger.WriteWarning(eventMessage, 0);
                }
                return defaultFromAddress;
            }
        }

        /// <summary>
        /// Returns default 'From' address to send out email notifications defined in the application configuration file.
        /// </summary>
        public string EmailSubject
        {
            get
            {
                if (string.IsNullOrEmpty(emailSubject))
                {
                    string eventMessage = "emailSubject property has not been initialized in <hp.practices.biztalk>\\<emailNotificationSettings> section.";
                    Logger.WriteWarning(eventMessage, 0);
                }
                return emailSubject;
            }
        }

    }
}
