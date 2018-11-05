//-----------------------------------------------------------------------------
// This file is part of the HP.Practices Application Framework
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
using Avista.ESB.Utilities.Configuration;

namespace Avista.ESB.Utilities.DataAccess.Configuration
{
    /// <summary>
    /// The ConnectionCollectionElement class represents an element in the
    /// application configuration file used to configure a collection of connections.
    /// </summary>
    [ConfigurationCollection(typeof(ConnectionElement), AddItemName = "connection",
      CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class ConnectionCollectionElement : ConfigurationElementCollection
    {
        private static ConfigurationPropertyCollection s_properties;

        /// <summary>
        /// Static constructor. Prepares the property collection.
        /// </summary>
        static ConnectionCollectionElement()
        {
            s_properties = new ConfigurationPropertyCollection();
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConnectionCollectionElement()
        {
        }

        /// <summary>
        /// Returns a member of the collection using an integer indexer.
        /// </summary>
        /// <param name="index">The index of the ConnectionElement to be returned.</param>
        /// <returns>The ConnectionElement found at the given index.</returns>
        public ConnectionElement this[int index]
        {
            get { return (ConnectionElement)base.BaseGet(index); }
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
        /// <param name="index">The name of the ConnectionElement to be returned.</param>
        /// <returns>The ConnectionElement with the given name.</returns>
        public new ConnectionElement this[string name]
        {
            get { return (ConnectionElement)base.BaseGet(name); }
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
            get { return "connection"; }
        }

        /// <summary>
        /// Creates a new contained element.
        /// The contained element of a ConnectionElement must be a PortElement.
        /// </summary>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ConnectionElement();
        }

        /// <summary>
        /// Gets the key value of a contained element.
        /// The contained element of a ConnectionCollectionElement must be a ConnectionElement.
        /// It's key will be the value of the 'name' attribute.
        /// </summary>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as ConnectionElement).Name;
        }
    }
}
