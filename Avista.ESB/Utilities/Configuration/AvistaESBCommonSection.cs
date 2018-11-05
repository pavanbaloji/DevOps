using System;
using System.Configuration;
using Avista.ESB.Utilities.Configuration;

namespace Avista.ESB.Utilities.Configuration
{
    /// <summary>
    /// The BizTalkPracticesSection class represents a section element in the application
    /// configuration file used to configure Avista.ESB.Utilities components.
    /// </summary>
    [Serializable]
    public class AvistaESBCommonSection : ConfigurationSection
    {
        private static readonly ConfigurationProperty propContext = new ConfigurationProperty(
            "context",
            typeof(ContextElement),
            null,
            ConfigurationPropertyOptions.IsRequired
        );
        
        private static readonly ConfigurationProperty propEmailNotificationSettings = new ConfigurationProperty(
          "emailNotificationSettings",
          typeof(EmailNotificationSettingsCollection),
          null,
          ConfigurationPropertyOptions.IsRequired
        );

        private static ConfigurationPropertyCollection properties;

        /// <summary>
        /// Static constructor. Prepares the property collection.
        /// </summary>
        static AvistaESBCommonSection()
        {
            properties = new ConfigurationPropertyCollection();
            properties.Add(propContext);           
            properties.Add(propEmailNotificationSettings);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AvistaESBCommonSection()
        {
        }

        /// <summary>
        /// Gets the configuration section using the default element name.
        /// </summary>
        public static AvistaESBCommonSection GetSection()
        {
            return GetSection("Avista.ESB.Utilities");
        }

        /// <summary>
        /// Gets the configuration section using the specified element name.
        /// </summary>
        /// <param name="strName">Name of the section to be retrieved</param>
        /// <returns>The requested section as a BizTalkPracticesSection type.</returns>
        public static AvistaESBCommonSection GetSection(string strName)
        {
            AvistaESBCommonSection section = ConfigurationManager.GetSection(strName) as AvistaESBCommonSection;
            if (section == null)
            {
                throw new ConfigurationErrorsException("The <" + strName + "> section is not defined in your .config file.");
            }
            return section;
        }

        /// <summary>
        /// Gets the Context setting.
        /// </summary>
        [ConfigurationProperty("context", IsRequired = true)]
        public ContextElement Context
        {
            get { return (ContextElement)base[propContext]; }
        }

        /// <summary>
        /// Gets the EmailNotificationSettings setting.
        /// </summary>
        [ConfigurationProperty("emailNotificationSettings", IsRequired = true)]
        public EmailNotificationSettingsCollection EmailNotificationSettings
        {
            get { return (EmailNotificationSettingsCollection)base[propEmailNotificationSettings]; }
        }

    }
}
