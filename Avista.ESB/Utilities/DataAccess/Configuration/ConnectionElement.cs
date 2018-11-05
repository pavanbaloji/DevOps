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
    /// The ConnectionElement class represents an element in the application configuration file.
    /// This element is used to configure connnection details for data access to the DataIntake database.
    /// </summary>
    public class ConnectionElement : ConfigurationElement
    {
        private static readonly ConfigurationProperty s_propName = new ConfigurationProperty(
            "name",
            typeof(string),
            null,
            ConfigurationPropertyOptions.IsRequired
        );

        private static readonly ConfigurationProperty s_propAffiliate = new ConfigurationProperty(
            "affiliate",
            typeof(string),
            null,
            ConfigurationPropertyOptions.None
        );

        private static readonly ConfigurationProperty s_propUserId = new ConfigurationProperty(
            "userId",
            typeof(string),
            null,
            ConfigurationPropertyOptions.None
        );

        private static readonly ConfigurationProperty s_propPassword = new ConfigurationProperty(
            "password",
            typeof(string),
            null,
            ConfigurationPropertyOptions.None
        );

        private static readonly ConfigurationProperty s_propConnectionTemplate = new ConfigurationProperty(
            "connectionTemplate",
            typeof(string),
            null,
            ConfigurationPropertyOptions.IsRequired
        );

        private static readonly ConfigurationProperty s_propRetryConnectionCount = new ConfigurationProperty(
            "retryConnectionCount",
            typeof(int),
            0,
            ConfigurationPropertyOptions.None
        );

        private static readonly ConfigurationProperty s_propRetryIntervalInMilliseconds = new ConfigurationProperty(
            "retryIntervalInMilliseconds",
            typeof(int),
            0,
            ConfigurationPropertyOptions.None
        );

        private static readonly ConfigurationProperty s_propDbConnectionProvider = new ConfigurationProperty(
            "dbConnectionProvider",
            typeof(ClassSpecificationElement),
            null,
            ConfigurationPropertyOptions.None
        );

        private static ConfigurationPropertyCollection s_properties;

        /// <summary>
        /// Static constructor prepares the property collection.
        /// </summary>
        static ConnectionElement()
        {
            s_properties = new ConfigurationPropertyCollection();
            s_properties.Add(s_propName);
            s_properties.Add(s_propAffiliate);
            s_properties.Add(s_propUserId);
            s_properties.Add(s_propPassword);
            s_properties.Add(s_propConnectionTemplate);
            s_properties.Add(s_propRetryConnectionCount);
            s_properties.Add(s_propRetryIntervalInMilliseconds);
            s_properties.Add(s_propDbConnectionProvider);
        }

        /// <summary>
        /// Default constructor does nothing.
        /// </summary>
        public ConnectionElement()
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
        /// Gets the Affiliate setting.
        /// </summary>
        [ConfigurationProperty("affiliate", IsRequired = false)]
        public string Affiliate
        {
            get { return (string)base[s_propAffiliate]; }
        }

        /// <summary>
        /// Gets the UserId setting.
        /// </summary>
        [ConfigurationProperty("userId", IsRequired = false)]
        public string UserId
        {
            get { return (string)base[s_propUserId]; }
        }

        /// <summary>
        /// Gets the Password setting.
        /// </summary>
        [ConfigurationProperty("password", IsRequired = false)]
        public string Password
        {
            get { return (string)base[s_propPassword]; }
        }

        /// <summary>
        /// Gets the ConnectionTemplate setting.
        /// </summary>
        [ConfigurationProperty("connectionTemplate", IsRequired = true)]
        public string ConnectionTemplate
        {
            get { return (string)base[s_propConnectionTemplate]; }
        }

        /// <summary>
        /// Gets the RetryConnectionCount setting.
        /// </summary>
        [ConfigurationProperty("retryConnectionCount", IsRequired = false)]
        public int RetryConnectionCount
        {
            get { return (int)base[s_propRetryConnectionCount]; }
        }

        /// <summary>
        /// Gets the RetryIntervalInMilliseconds setting.
        /// </summary>
        [ConfigurationProperty("retryIntervalInMilliseconds", IsRequired = false)]
        public int RetryIntervalInMilliseconds
        {
            get { return (int)base[s_propRetryIntervalInMilliseconds]; }
        }

        /// <summary>
        /// Gets the DbConnectionProvider setting.
        /// </summary>
        [ConfigurationProperty("dbConnectionProvider", IsRequired = false)]
        public ClassSpecificationElement DbConnectionProvider
        {
            get { return (ClassSpecificationElement)base[s_propDbConnectionProvider]; }
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
