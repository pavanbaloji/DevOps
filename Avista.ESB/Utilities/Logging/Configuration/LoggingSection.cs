//-----------------------------------------------------------------------------
// This file is part of the Avista.ESB.Utilities Application Framework
//
// Copyright (c) HP Enterprise Services. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//-----------------------------------------------------------------------------
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using Avista.ESB.Utilities.Configuration;

namespace Avista.ESB.Utilities.Logging.Configuration
{
    /// <summary>
    /// The LoggingSection class represents a section element in the application
    /// configuration file used to configure HP.Practices.Logging components.
    /// </summary>
    public class LoggingSection : ConfigurationSection
    {
        private static readonly ConfigurationProperty s_propLoggingProvider = new ConfigurationProperty(
            "loggingProvider",
            typeof(ClassSpecificationElement),
            null,
            ConfigurationPropertyOptions.None
        );

        private static readonly ConfigurationProperty s_propLoggingSettings = new ConfigurationProperty(
            "loggingSettings",
            typeof(LoggingSettingsElement),
            null,
            ConfigurationPropertyOptions.None
        );

        private static ConfigurationPropertyCollection s_properties;

        /// <summary>
        /// Static constructor. Prepares the property collection.
        /// </summary>
        static LoggingSection()
        {
            s_properties = new ConfigurationPropertyCollection();
            s_properties.Add(s_propLoggingProvider);
            s_properties.Add(s_propLoggingSettings);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LoggingSection()
        {
        }

        /// <summary>
        /// Gets the configuration section using the default element name.
        /// </summary>
        public static LoggingSection GetSection()
        {
            return GetSection("Avista.ESB.Utilities.logging");
        }

        /// <summary>
        /// Gets the configuration section using the specified element name.
        /// </summary>
        /// <param name="strName">Name of the section to be retrieved</param>
        /// <returns>The requested section as a LoggingSection type.</returns>
        public static LoggingSection GetSection(string strName)
        {
            LoggingSection section = ConfigurationManager.GetSection(strName) as LoggingSection;
            if (section == null)
            {
                throw new ConfigurationErrorsException("The <" + strName + "> section is not defined in your .config file.");
            }
            return section;
        }

        /// <summary>
        /// Gets the LoggingProvider setting.
        /// </summary>
        [ConfigurationProperty("loggingProvider", IsRequired = false)]
        public ClassSpecificationElement LoggingProvider
        {
            get { return (ClassSpecificationElement)base[s_propLoggingProvider]; }
        }

        /// <summary>
        /// Gets the LoggingSettings setting.
        /// </summary>
        [ConfigurationProperty("loggingSettings", IsRequired = false)]
        public LoggingSettingsElement LoggingSettings
        {
            get { return (LoggingSettingsElement)base[s_propLoggingSettings]; }
        }
    }

}
