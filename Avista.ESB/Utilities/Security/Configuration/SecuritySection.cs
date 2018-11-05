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
using System.Collections.Generic;
using System.Text;
using Avista.ESB.Utilities.Configuration;

namespace Avista.ESB.Utilities.Security.Configuration
{
    /// <summary>
    /// The SecuritySection class represents a section element in the application
    /// configuration file used to configure HP.Practices.Security components.
    /// </summary>
    public class SecuritySection : ConfigurationSection
    {
        private static readonly ConfigurationProperty s_propCredentialsProvider = new ConfigurationProperty(
            "credentialsProvider",
            typeof(ClassSpecificationElement),
            null,
            ConfigurationPropertyOptions.None
        );

        private static readonly ConfigurationProperty s_propTicketProvider = new ConfigurationProperty(
            "ticketProvider",
            typeof(ClassSpecificationElement),
            null,
            ConfigurationPropertyOptions.None
        );

        private static ConfigurationPropertyCollection s_properties;

        /// <summary>
        /// Static constructor. Prepares the property collection.
        /// </summary>
        static SecuritySection()
        {
            s_properties = new ConfigurationPropertyCollection();
            s_properties.Add(s_propCredentialsProvider);
            s_properties.Add(s_propTicketProvider);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SecuritySection()
        {
        }

        /// <summary>
        /// Gets the configuration section using the default element name.
        /// </summary>
        public static SecuritySection GetSection()
        {
              return GetSection( "Avista.ESB.Utilities.security" );
        }

        /// <summary>
        /// Gets the configuration section using the specified element name.
        /// </summary>
        /// <param name="strName">Name of the section to be retrieved</param>
        /// <returns>The requested section as an SsoSection type.</returns>
        public static SecuritySection GetSection(string strName)
        {
            SecuritySection section = ConfigurationManager.GetSection(strName) as SecuritySection;
            if (section == null)
            {
                throw new ConfigurationErrorsException("The <" + strName + "> section is not defined in your .config file.");
            }
            return section;
        }

        /// <summary>
        /// Gets the credentialsProvider setting.
        /// </summary>
        [ConfigurationProperty("credentialsProvider", IsRequired = false)]
        public ClassSpecificationElement CredentialsProvider
        {
            get { return (ClassSpecificationElement)base[s_propCredentialsProvider]; }
        }

        /// <summary>
        /// Gets the ticketProvider setting.
        /// </summary>
        [ConfigurationProperty("ticketProvider", IsRequired = false)]
        public ClassSpecificationElement TicketProvider
        {
            get { return (ClassSpecificationElement)base[s_propTicketProvider]; }
        }
    }
}
