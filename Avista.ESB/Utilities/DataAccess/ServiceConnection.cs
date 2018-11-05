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
using System.Threading;
using Avista.ESB.Utilities.Components;
using Avista.ESB.Utilities.DataAccess.Configuration;
using Avista.ESB.Utilities.Security;
using Avista.ESB.Utilities.Configuration;
using Avista.ESB.Utilities.Sso;
using System.Net;

namespace Avista.ESB.Utilities.DataAccess
{
    public class ServiceConnection : ComponentBase
    {
        /// <summary>
        /// Flag to indicate if the object has been disposed.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Name of affiliate application used to obtain connection credentials.
        /// </summary>
        private string _affiliateApplication = "";

        /// <summary>
        /// Userid used to establish connection.
        /// </summary>
        private string _userId = "";

        /// <summary>
        /// Password used to establish connection.no 
        /// </summary>
        private string _password = "";

        /// <summary>
        /// Connection template string used to establish connection.
        /// </summary>
        private string _connectionTemplate = "";

        /// <summary>
        /// Connection string derived from connection information and connection template.
        /// </summary>
        private string _connectionString = "";

        /// <summary>
        /// The number of times an attempt is made to re-connect to the service
        /// </summary>
        private int _retryConnectionCount = 0;

        /// <summary>
        /// Number of milliseconds to wait between attemps to re-connect to the service
        /// </summary>
        private int _retryIntervalInMilliseconds = 0;

        /// <summary>
        /// The name of the connection provider.
        /// </summary>
        private string _connectionProviderName = "";

        /// <summary>
        /// The name of the assembly that implements the connection provider.
        /// </summary>
        private string _connectionProviderAssembly = "";

        /// <summary>
        /// The name of the class that implements the connection provider.
        /// </summary>
        private string _connectionProviderClass = "";

        /// <summary>
        /// Constructor for ServiceConnection. The name provided should be the name of
        /// a connection configured in the application configuration file.
        /// </summary>
        /// <param name="name">The name of the connection.</param>
        public ServiceConnection(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Refreshes the configuration of the ServiceConnection.
        /// </summary>
        public override void RefreshConfiguration()
        {
            base.RefreshConfiguration();
            DataAccessSection section = DataAccessSection.GetSection();
            ConnectionElement connectionElement = section.ConnectionList[this.Name];
            if (connectionElement == null)
            {
                throw new Exception("Could not find connection element '" + this.Name + "' in the configuration file.");
            }
            else
            {
                _affiliateApplication = connectionElement.Affiliate;
                _userId = connectionElement.UserId;
                _password = connectionElement.Password;
                _connectionTemplate = connectionElement.ConnectionTemplate;
                _retryConnectionCount = connectionElement.RetryConnectionCount;
                _retryIntervalInMilliseconds = connectionElement.RetryIntervalInMilliseconds;
                ClassSpecificationElement connectionProvider = connectionElement.DbConnectionProvider;
                _connectionProviderName = connectionProvider.Name;
                _connectionProviderAssembly = connectionProvider.Assembly;
                _connectionProviderClass = connectionProvider.Class;
                GetConnectionString();
            }
        }

        /// <summary>
        /// Gets the connection string that will be used to connect to the service. The affiliate application is looked
        /// up in SSO to get a set of login credentials and the credentials are substituted into the connection
        /// string template.
        /// </summary>
        private void GetConnectionString()
        {
            if (!String.IsNullOrEmpty(_affiliateApplication))
            {
                string userId = "";
                string password = SsoTicketProvider.LookupCredentials( _affiliateApplication, out userId );
                _userId = userId;      
                _password = password; 
            }
            _connectionString = String.Format(_connectionTemplate, _userId, _password);

        }

        /// <summary>
        /// The connection string associated with this service connection.
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
        }

        /// <summary>
        /// Returns the connection provider name.
        /// </summary>
        protected string ConnectionProviderName
        {
            get
            {
                return _connectionProviderName;
            }
        }

        /// <summary>
        /// Returns the connection provider assembly name.
        /// </summary>
        protected string ConnectionProviderAssembly
        {
            get
            {
                return _connectionProviderAssembly;
            }
        }

        /// <summary>
        /// Returns the connection provider class name.
        /// </summary>
        protected string ConnectionProviderClass
        {
            get
            {
                return _connectionProviderClass;
            }
        }

        /// <summary>
        /// Opens a connection to the service.
        /// </summary>
        public void Open()
        {
            int connectionAttempt = 1;
            bool connected = false;
            while (true)
            {
                // Try to connect.
                try
                {
                    connected = Connect();
                }
                catch (Exception)
                {
                }
                // If connection failed then we may need to wait before trying again.
                if (!connected && (connectionAttempt < _retryConnectionCount))
                {
                    // Display a warning and sleep.
                    Thread.Sleep(_retryIntervalInMilliseconds);
                    connectionAttempt++;
                }
                else
                {
                    break;
                }
            }
            // Raise an exception if last retry attempt failed.
            if (!connected)
            {
                throw new Exception("Connection to the " + Name + " service could not be opened.");
            }
        }

        /// <summary>
        /// Connects to the service. Descendants should override this method.
        /// </summary>
        protected virtual bool Connect()
        {
            return true;
        }

        /// <summary>
        /// Close the connection. Descendants should override this method.
        /// </summary>
        public virtual void Close()
        {
        }

        /// <summary>
        /// Releases the unmanaged resources used by the object and
        /// optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        ///   true to release both managed and unmanaged resources;
        ///   false to release only unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                // Dispose managed resources
                if (disposing)
                {
                }
                // Dispose unmanaged resources
            }
            _disposed = true;
            base.Dispose(disposing);
        }
    }
}
