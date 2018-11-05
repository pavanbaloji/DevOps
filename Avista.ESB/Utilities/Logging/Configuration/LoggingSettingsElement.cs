//-----------------------------------------------------------------------------
// This file is part of the Avista.ESB.Utilities Application Framework
//
// Copyright (c) Avista Corp. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//-----------------------------------------------------------------------------
using System;
using System.Configuration;

namespace Avista.ESB.Utilities.Logging.Configuration
{
    /// <summary>
    /// The LoggingSettingsElement class represents an element in the application
    /// configuration file used to specify settings for logging.
    /// </summary>
    public class LoggingSettingsElement : ConfigurationElement
    {
        private static readonly ConfigurationProperty s_propLogFolder = new ConfigurationProperty(
            "logFolder",
            typeof(string),
            "C:\\Windows\\Temp",
            ConfigurationPropertyOptions.None
        );

        private static readonly ConfigurationProperty s_propForwardToEventLogger = new ConfigurationProperty(
            "forwardToEventLogger",
            typeof(bool),
            true,
            ConfigurationPropertyOptions.None
        );

        private static readonly ConfigurationProperty s_propSources = new ConfigurationProperty(
            "sources",
            typeof(LoggingSourceCollection),
            null,
            ConfigurationPropertyOptions.IsRequired
        );

        private static readonly ConfigurationProperty s_propEventIdFilter = new ConfigurationProperty(
            "eventIdFilter",
            typeof(string),
            null,
            ConfigurationPropertyOptions.None
        );

        private static ConfigurationPropertyCollection s_properties;

        /// <summary>
        /// Static constructor prepares the property collection.
        /// </summary>
        static LoggingSettingsElement()
        {
            s_properties = new ConfigurationPropertyCollection();
            s_properties.Add(s_propLogFolder);
            s_properties.Add(s_propForwardToEventLogger);
            s_properties.Add(s_propSources);
            s_properties.Add(s_propEventIdFilter);
        }

        /// <summary>
        /// Default constructor does nothing.
        /// </summary>
        public LoggingSettingsElement()
        {
        }

        /// <summary>
        /// Gets the Source setting.
        /// </summary>
        [ConfigurationProperty("logFolder", IsRequired = false, DefaultValue = "C:\\Windows\\Temp")]
        public string LogFolder
        {
            get { return (string)base[s_propLogFolder]; }
        }

        /// <summary>
        /// Gets the ForwardToEventLogger setting.
        /// </summary>
        [ConfigurationProperty("forwardToEventLogger", IsRequired = false, DefaultValue = true)]
        public bool ForwardToEventLogger
        {
            get { return (bool)base[s_propForwardToEventLogger]; }
        }

        /// <summary>
        /// Gets the Sources setting.
        /// </summary>
        [ConfigurationProperty("sources", IsRequired = true)]
        public LoggingSourceCollection Sources
        {
            get { return (LoggingSourceCollection)base[s_propSources]; }
        }

        /// <summary>
        /// Gets the EventIdFilter setting.
        /// </summary>
        [ConfigurationProperty("eventIdFilter", IsRequired = false)]
        public string EventIdFilter
        {
            get { return (string)base[s_propEventIdFilter]; }
        }

        /// <summary>
        /// Override the Properties collection and return our custom one.
        /// </summary>
        protected override ConfigurationPropertyCollection Properties
        {
            get { return s_properties; }
        }
    }
}
