using System;
using System.Configuration;

namespace Avista.ESB.Utilities.Configuration
{
    /// <summary>
    /// The EmailSettingsCollection class represents an element in the application
    /// configuration file used to configure a collection of email notification settings.
    /// </summary>
    [ConfigurationCollection(typeof(EmailNotificationSettingElement), AddItemName = "emailNotificationSetting",
      CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class EmailNotificationSettingsCollection : ConfigurationElementCollection
    {

        private static readonly ConfigurationProperty propSmtpServerName = new ConfigurationProperty(
            "smtpServerName",
            typeof(string),
            null,
            ConfigurationPropertyOptions.None
        );

        private static readonly ConfigurationProperty propDefaultFromAddress = new ConfigurationProperty(
           "defaultFromAddress",
           typeof(string),
           null,
           ConfigurationPropertyOptions.None
       );

        private static readonly ConfigurationProperty propEmailSubject = new ConfigurationProperty(
            "emailSubject",
            typeof(string),
            null,
            ConfigurationPropertyOptions.None
);

        private static ConfigurationPropertyCollection properties;

        /// <summary>
        /// Static constructor. Prepares the property collection.
        /// </summary>
        static EmailNotificationSettingsCollection()
        {
            properties = new ConfigurationPropertyCollection();
            properties.Add(propSmtpServerName);
            properties.Add(propDefaultFromAddress);
            properties.Add(propEmailSubject);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public EmailNotificationSettingsCollection()
        {
        }

        /// <summary>
        /// Gets the SmtpServerName setting.
        /// </summary>
        [ConfigurationProperty("smtpServerName", IsRequired = false)]
        public string SmtpServerName
        {
            get { return (string)base[propSmtpServerName]; }
        }

        /// <summary>
        /// Gets the DefaultFromAddress setting.
        /// </summary>
        [ConfigurationProperty("defaultFromAddress", IsRequired = false)]
        public string DefaultFromAddress
        {
            get { return (string)base[propDefaultFromAddress]; }
        }

        /// <summary>
        /// Gets the SmtpServerName setting.
        /// </summary>
        [ConfigurationProperty("emailSubject", IsRequired = false)]
        public string EmailSubject
        {
            get { return (string)base[propEmailSubject]; }
        }

        /// <summary>
        /// Returns a member of the collection using an integer indexer.
        /// </summary>
        /// <param name="index">The index of the EmailNotificationSettingElement to be returned.</param>
        /// <returns>The EmailNotificationSettingElement found at the given index.</returns>
        public EmailNotificationSettingElement this[int index]
        {
            get { return (EmailNotificationSettingElement)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        /// <summary>
        /// Returns a member of the collection using a name indexer.
        /// </summary>
        /// <param name="index">The portName of the EmailNotificationSettingElement to be returned.</param>
        /// <returns>The EmailNotificationSettingElement with the given portName.</returns>
        public new EmailNotificationSettingElement this[string name]
        {
            get { return (EmailNotificationSettingElement)base.BaseGet(name); }
        }

        /// <summary>
        /// Override the properties collection and return our custom one.
        /// </summary>
        protected override ConfigurationPropertyCollection Properties
        {
            get { return properties; }
        }

        /// <summary>
        /// Identifies the collection type.
        /// </summary>
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        /// <summary>
        /// Provides the tag name of the contained element.
        /// </summary>
        protected override string ElementName
        {
            get { return "emailNotificationSetting"; }
        }

        /// <summary>
        /// Creates a new contained element.
        /// </summary>
        protected override ConfigurationElement CreateNewElement()
        {
            return new EmailNotificationSettingElement();
        }

        /// <summary>
        /// Gets the key value of a contained element.
        /// </summary>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as EmailNotificationSettingElement).GroupName;
        }
    }
}
