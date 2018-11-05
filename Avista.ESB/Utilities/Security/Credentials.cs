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
using System.Collections.Generic;
using System.Text;
using Avista.ESB.Utilities.Components;
using Avista.ESB.Utilities.Configuration;
using Avista.ESB.Utilities.Security.Configuration;

namespace Avista.ESB.Utilities.Security
{
    public class Credentials
    {
        /// <summary>
        /// A dictionary list used to keep track of Credentials instances.
        /// </summary>
        private static Dictionary<String,ICredentials> credentialsList = null;

        /// <summary>
        /// Credentials instance lock.
        /// </summary>
        private static object credentialsLock = new Object();

        /// <summary>
        /// Creates ICredentials for an affiliate application.
        /// </summary>
        /// <param name="affiliateName">The name of the affiliate application for the credentials.</param>
        /// <returns>A reference to the ICredentials implementation.</returns>
        public static ICredentials GetCredentials(string affiliateName)
        {
            string instanceName = "";
            string className = "";
            string assemblyName = "";
            ICredentials credentials = null;
            try
            {
                lock (credentialsLock)
                {
                    // Construct ist if needed.
                    if (credentialsList == null)
                    {
                        credentialsList = new Dictionary<string, ICredentials>();
                    }
                    // Check list for the requested affiliate.
                    if (credentialsList.ContainsKey(affiliateName))
                    {
                        credentials = credentialsList[affiliateName];
                    }
                    else
                    {
                        SecuritySection section = SecuritySection.GetSection();
                        ClassSpecificationElement spec = section.CredentialsProvider;
                        instanceName = affiliateName;
                        className = spec.Class;
                        assemblyName = spec.Assembly;
                        credentials = (ICredentials)Factory.CreateComponent(instanceName, className, assemblyName);
                        credentialsList[affiliateName] = credentials;
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to create ICredentials implementation.", exception);
            }
            return credentials;
        }
    }
}
