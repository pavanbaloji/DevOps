using System;
using System.Configuration;

namespace Avista.ESB.Utilities.Configuration
{
    /// <summary>
    /// The EmailSettingElement class represents an element in the application configuration
    /// file that is used to configure the email address of an email notification. 
    /// </summary>
    public class EmailNotificationSettingElement : ConfigurationElement
    {
        private static readonly ConfigurationProperty propGroupName = new ConfigurationProperty(
            "groupName",
            typeof(string),
            null,
            ConfigurationPropertyOptions.IsRequired
        );

        private static readonly ConfigurationProperty propEmailId = new ConfigurationProperty(
            "emailId",
            typeof(string),
            null,
            ConfigurationPropertyOptions.IsRequired
        );

        private static ConfigurationPropertyCollection properties;

        /// <summary>
        /// Static constructor prepares the property collection.
        /// </summary>
        static EmailNotificationSettingElement()
        {
            properties = new ConfigurationPropertyCollection();
            properties.Add(propGroupName);
            properties.Add(propEmailId);
        }

        /// <summary>
        /// Default constructor does nothing.
        /// </summary>
        public EmailNotificationSettingElement()
        {
        }

        /// <summary>
        /// Gets the GroupName setting.
        /// </summary>
        [ConfigurationProperty("groupName", IsRequired = true)]
        public string GroupName
        {
            get { return (string)base[propGroupName]; }
        }

        /// <summary>
        /// Gets the EmailId setting.
        /// </summary>
        [ConfigurationProperty("emailId", IsRequired = true)]
        public string EmailId
        {
            get { return (string)base[propEmailId]; }
        }

        /// <summary>
        /// Override the Properties collection and return our custom one.
        /// </summary>
        protected override ConfigurationPropertyCollection Properties
        {
            get { return properties; }
        }
    }
}


