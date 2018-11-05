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
using System.Data;
using System.Data.SqlClient;
using Avista.ESB.Utilities.Components;
using Avista.ESB.Utilities.Configuration;
using Avista.ESB.Utilities.DataAccess.Configuration;
using Avista.ESB.Utilities.Logging;
using Avista.ESB.Utilities.Security;
using Avista.ESB.Utilities.DataAccess;

namespace Avista.ESB.Utilities.DataAccess
{
    public class SqlServerConnection : ServiceConnection
    {
        /// <summary>
        /// Flag to indicate if the object has been disposed.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Connection to the database. Set to null when there is no connection open.
        /// </summary>
        protected SqlConnection _connection = null;

        /// <summary>
        /// Currently open transaction. Set to null when there is no transaction open.
        /// </summary>
        protected SqlTransaction _transaction = null;

        /// <summary>
        /// Constructor for SqlServerConnection. The name provided should be the name of
        /// a connection configured in the application configuration file.
        /// </summary>
        /// <param name="name">The name of the connection.</param>
        public SqlServerConnection(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Refreshes the configuration of the SqlServerConnection.
        /// </summary>
        public override void RefreshConfiguration()
        {
            Close();
            base.RefreshConfiguration();
        }

        /// <summary>
        /// Connect to the database.
        /// </summary>
        protected override bool Connect()
        {
            bool connected = false;
            try
            {
                if (_connection == null)
                {
                    _connection = new SqlConnection(ConnectionString);
                    _connection.Open();
                }
                connected = (_connection.State == ConnectionState.Open);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to open database connection. " + ex.Message);
            }
            return connected;
        }

        /// <summary>
        /// Close the connection.
        /// </summary>
        public override void Close()
        {
            try
            {
                if (_connection != null)
                {
                    _connection.Close();
                    _connection.Dispose();
                    _connection = null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to close database connection. " + ex.Message);
            }
        }

        /// <summary>
        /// Returns a flag which indicates whether or not the connection is open.
        /// </summary>
        public bool IsOpen()
        {
            return ((_connection != null) && (_connection.State == ConnectionState.Open));
        }

        /// <summary>
        /// Begins a transaction with the default isolation level (repeatable read).
        /// </summary>
        public void BeginTransaction()
        {
            BeginTransaction(IsolationLevel.RepeatableRead);
        }

        /// <summary>
        /// Begins a transaction with the specified isolation level.
        /// </summary>
        /// <param name="isolationLevel">The isolation level to use for the transaction.</param>
        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            if (_connection == null)
            {
                throw new Exception("Cannot begin a transaction when the connection is null.");
            }
            if (_connection.State != ConnectionState.Open)
            {
                throw new Exception("Cannot begin a transaction when the connection is closed.");
            }
            if (_transaction != null)
            {
               throw new Exception("Cannot begin a transaction when an existing transaction is open on the same connection.");
            }
            _transaction = _connection.BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// Returns a flag which indicates whether or not the connection is currently in a transaction.
        /// </summary>
        public bool IsInTransaction()
        {
            return (_transaction != null);
        }

        /// <summary>
        /// Commit the currently open transaction.
        /// </summary>
        public void Commit()
        {
            if (_transaction == null)
            {
                throw new Exception("A transaction must be started before it can be committed.");
            }
            else
            {
                _transaction.Commit();
                _transaction = null;
            }
        }

        /// <summary>
        /// Rollback the currently open transaction.
        /// </summary>
        public void Rollback()
        {
            if (_transaction == null)
            {
                throw new Exception("A transaction must be started before it can be rolled back.");
            }
            else
            {
                _transaction.Rollback();
                _transaction = null;
            }
        }

        /// <summary>
        /// Executes a non-query command on the connection.
        /// </summary>
        /// <param name="command">The command to be executed.</param>
        /// <returns>The number of rows affected.</returns>
        public int ExecuteNonQuery(SqlCommand command)
        {
            int rows = -1;
            command.Connection = _connection;
            command.Transaction = _transaction;
            rows = command.ExecuteNonQuery();
            return rows;
        }

        /// <summary>
        /// Executes a scalar command on the connection.
        /// </summary>
        /// <param name="command">The command to be executed.</param>
        /// <returns>The scalar result.</returns>
        public object ExecuteScalar(SqlCommand command)
        {
            object result = null;
            command.Connection = _connection;
            command.Transaction = _transaction;
            result = command.ExecuteScalar();
            return result;
        }

        /// <summary>
        /// Executes a query command on the connection.
        /// </summary>
        /// <param name="command">The command to be executed.</param>
        /// <returns>A SqlDataReader object.</returns>
        public SqlDataReader ExecuteReader(SqlCommand command)
        {
            SqlDataReader reader = null;
            command.Connection = _connection;
            command.Transaction = _transaction;
            reader = command.ExecuteReader();
            return reader;
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
                    if (_transaction != null)
                    {
                        _transaction.Rollback();
                        _transaction = null;
                    }
                    if (_connection != null)
                    {
                        _connection.Close();
                        _connection.Dispose();
                    }
                }
                // Dispose unmanaged resources
            }
            _disposed = true;
            base.Dispose(disposing);
        }
    }
}
