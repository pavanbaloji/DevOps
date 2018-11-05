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
    /// The EventLoggingSourceElement class represents an element in the application configuration file
    /// that is used to configure the ranges of events that map to an event source. This is useful
    /// for processes that have multiple instances in which each instance may need to log events for a
    /// unique event id range but the instances share a common configuratino file.
    /// </summary>
    public class LoggingSourceElement : ConfigurationElement
    {
        private static readonly ConfigurationProperty s_propName = new ConfigurationProperty(
            "name",
            typeof(string),
            null,
            ConfigurationPropertyOptions.IsRequired
        );

        private static readonly ConfigurationProperty s_propRange = new ConfigurationProperty(
            "range",
            typeof(string),
            null,
            ConfigurationPropertyOptions.IsRequired
        );

        private static ConfigurationPropertyCollection s_properties;

        /// <summary>
        /// Static constructor prepares the property collection.
        /// </summary>
        static LoggingSourceElement()
        {
            s_properties = new ConfigurationPropertyCollection();
            s_properties.Add(s_propName);
            s_properties.Add(s_propRange);
        }

        /// <summary>
        /// Default constructor does nothing.
        /// </summary>
        public LoggingSourceElement()
        {
        }

        /// <summary>
        /// Gets the Name setting.
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)base[s_propName]; }
        }

        /// <summary>
        /// Gets the Range setting.
        /// </summary>
        [ConfigurationProperty("range", IsRequired = true)]
        public string Range
        {
            get { return (string)base[s_propRange]; }
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
