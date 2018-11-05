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
    /// The EventLoggingSourceCollection class represents an element in the application configuration file
    /// that is used to configure the event log sources used for event logging within the process.
    /// </summary>
    [ConfigurationCollection(typeof(LoggingSourceElement), AddItemName = "source",
      CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class LoggingSourceCollection : ConfigurationElementCollection
    {
        private static ConfigurationPropertyCollection s_properties;

        /// <summary>
        /// Static constructor. Prepares the property collection.
        /// </summary>
        static LoggingSourceCollection()
        {
            s_properties = new ConfigurationPropertyCollection();
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LoggingSourceCollection()
        {
        }

        /// <summary>
        /// Returns a member of the collection using an integer indexer.
        /// </summary>
        /// <param name="index">The index of the EventLoggingSourceElement to be returned.</param>
        /// <returns>The EventLoggingSourceElement found at the given index.</returns>
        public LoggingSourceElement this[int index]
        {
            get { return (LoggingSourceElement)base.BaseGet(index); }
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
        /// <param name="index">The name of the EventLoggingSourceElement to be returned.</param>
        /// <returns>The EventLoggingSourceElement with the given name.</returns>
        public new LoggingSourceElement this[string name]
        {
            get { return (LoggingSourceElement)base.BaseGet(name); }
        }

        /// <summary>
        /// Override the Properties collection and return our custom one.
        /// </summary>
        protected override ConfigurationPropertyCollection Properties
        {
            get { return s_properties; }
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
            get { return "source"; }
        }

        /// <summary>
        /// Creates a new contained element.
        /// The contained element of an EventLoggingSourceCollection must be an EventLoggingSourceElement.
        /// </summary>
        protected override ConfigurationElement CreateNewElement()
        {
            return new LoggingSourceElement();
        }

        /// <summary>
        /// Gets the key value of a contained element.
        /// The contained element of an EventLoggingSourceCollection must be an EventLoggingSourceElement.
        /// It's key will be the value of the 'name' attribute.
        /// </summary>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as LoggingSourceElement).Name;
        }
    }
}
