
using System;
using System.Data;
using System.Diagnostics;
using Avista.ESB.Utilities.Components;
using Avista.ESB.Utilities.Configuration;
using Avista.ESB.Utilities.DataAccess.Configuration;
using Avista.ESB.Utilities.Logging;
using Avista.ESB.Utilities.Security;
using System.Data.Common;

namespace Avista.ESB.Utilities.DataAccess
{
    public class DatabaseConnection : ServiceConnection
    {
        /// <summary>
        /// Flag to indicate if the object has been disposed.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Connection to the database. Set to null when there is no connection open.
        /// </summary>
        protected DbConnection _connection = null;

        /// <summary>
        /// Currently open transaction. Set to null when there is no transaction open.
        /// </summary>
        protected DbTransaction _transaction = null;

        /// <summary>
        /// Constructor for SqlServerConnection. The name provided should be the name of
        /// a connection configured in the application configuration file.
        /// </summary>
        /// <param name="name">The name of the connection.</param>
        public DatabaseConnection(string name)
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
                    try
                    {
                        _connection = (DbConnection)AssemblyHelper.CreateInstance(ConnectionProviderName, null, ConnectionProviderClass, ConnectionProviderAssembly);
                    }
                    catch (Exception exception)
                    {
                        throw new Exception("Error constructing connection provider " + ConnectionProviderName + ".", exception);
                    }
                    _connection.ConnectionString = ConnectionString;
                    _connection.Open();
                }
                connected = (_connection.State == ConnectionState.Open);
            }
            catch (Exception exception)
            {
                throw new Exception("Unable to open database connection.", exception);
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
            catch (Exception exception)
            {
                throw new Exception("Unable to close database connection.", exception);
            }
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
        /// Returns a flag which indicates whether or not the connection is open.
        /// </summary>
        public bool IsOpen()
        {
            return ((_connection != null) && (_connection.State == ConnectionState.Open));
        }

        /// <summary>
        /// Returns a flag which indicates whether or not the connection is currently in a transaction.
        /// </summary>
        public bool IsInTransaction()
        {
            return (_transaction != null);
        }

        /// <summary>
        /// Executes a SQL statement that returns a scalar value. The result is returned as a string.
        /// </summary>
        /// <param name="sql">Sql query</param>
        /// <returns>The scalar result.</returns>
        public string ExecuteScalar(string sql)
        {
            string returnValue = "";
            try
            {
                DbCommand command = CreateCommand();
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    returnValue = result.ToString();
                }
            }
            catch (Exception exception)
            {
                Logger.WriteError( "Failed to perform ExecuteScalar method using sql statement :  " + string.Concat(sql, "\r\n", exception.StackTrace),323);
                throw exception;
            }
            return returnValue;
        }


        /// <summary>
        /// Returns a new instance of DbCommand.
        /// </summary>
        /// <returns></returns>
        private DbCommand CreateCommand()
        {
            DbCommand command = null;
            try
            {
                if (IsOpen())
                {
                    command = _connection.CreateCommand();
                }
                else
                {
                    throw new Exception("Database connection is not open.");
                }
            }
            catch (Exception exception)
            {
                throw new Exception("Unable to create a DbCommand.", exception);
            }
            return command;
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
