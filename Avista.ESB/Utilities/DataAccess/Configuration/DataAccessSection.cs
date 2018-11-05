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

namespace Avista.ESB.Utilities.DataAccess.Configuration
{
    /// <summary>
    /// The DataAccessSection class represents an element in the application
    /// configuration file used as a high level container for configuration
    /// of the data access layer for the DataIntake database.
    /// </summary>
    public class DataAccessSection : ConfigurationSection
    {
        private static readonly ConfigurationProperty s_propConnectionList = new ConfigurationProperty(
            "connectionList",
            typeof(ConnectionCollectionElement),
            null,
            ConfigurationPropertyOptions.IsRequired
        );

        private static ConfigurationPropertyCollection s_properties;

        /// <summary>
        /// Static constructor. Prepares the property collection.
        /// </summary>
        static DataAccessSection()
        {
            s_properties = new ConfigurationPropertyCollection();
            s_properties.Add(s_propConnectionList);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataAccessSection()
        {
        }

        /// <summary>
        /// Gets the connection settings.
        /// </summary>
        [ConfigurationProperty("connectionList", IsRequired = true)]
        public ConnectionCollectionElement ConnectionList
        {
            get { return (ConnectionCollectionElement)base[s_propConnectionList]; }
        }

        /// <summary>
        /// Override the Properties collection and return our custom one.
        /// </summary>
        protected override ConfigurationPropertyCollection Properties
        {
            get { return s_properties; }
        }

        /// <summary>
        /// Gets the configuration section using the default element name.
        /// </summary>
        public static DataAccessSection GetSection()
        {
            return GetSection("avista.esb.common.dataAccess");
        }

        /// <summary>
        /// Gets the configuration section using the specified element name.
        /// </summary>
        public static DataAccessSection GetSection(string strName)
        {
            DataAccessSection section = ConfigurationManager.GetSection(strName) as DataAccessSection;
            if (section == null)
            {
                throw new ConfigurationErrorsException("The <" + strName + "> section is not defined in the configuration file.");
            }
            return section;
        }
    }
}
