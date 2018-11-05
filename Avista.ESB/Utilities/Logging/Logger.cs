//-----------------------------------------------------------------------------
// This file is part of the Avista.ESB.Utilities Application Framework
//
// Copyright (c) HP Enterprise Services. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//-----------------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.Text;
using Avista.ESB.Utilities.Components;
using Avista.ESB.Utilities.Configuration;
using Avista.ESB.Utilities.Logging.Configuration;

namespace Avista.ESB.Utilities.Logging
{
    /// <summary>
    /// Provides a factory and manages a singleton instance of the ILogger interface which is used to write to the log.
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// Logger instance. Note that the instance might not be an instance of this class.
        /// </summary>
        private static ILogger _instance = null;

        /// <summary>
        /// Logger instance lock.
        /// </summary>
        private static object _lock = new Object();

        /// <summary>
        /// Creates an ILogger implementation. The instance is a singleton.
        /// </summary>
        /// <returns>A reference to the ILogger implementation.</returns>
        public static ILogger GetLogger()
        {
            string instanceName = "";
            string className = "";
            string assemblyName = "";
            ILogger logger = null;
            try
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        // Try to load the logging congiguration section.
                        LoggingSection section = null;
                        try
                        {
                            section = LoggingSection.GetSection();
                        }
                        catch (Exception exception)
                        {
                            _instance = new NullLogger("NullLogger");
                            logger.WriteWarning("Logging is not configured. No logging will be performed.", 152);                                                     
                        }
                        // If the logging configuration section was loaded, try to load the logging provider.
                        if (section != null)
                        {
                            try
                            {
                                ClassSpecificationElement spec = section.LoggingProvider;
                                instanceName = spec.Name;
                                className = spec.Class;
                                assemblyName = spec.Assembly;
                                _instance = (ILogger)Factory.CreateComponent(instanceName, className, assemblyName);
                                _instance.RefreshConfiguration();
                            }
                            catch (Exception exception)
                            {
                              
                                _instance = new NullLogger("NullLogger");
                                logger.WriteError("Failed to create ILogger implementation. No logging will be performed.", 152);                           
                            }
                        }
                    }
                    logger = _instance;
                }
            }
            catch (Exception exception)
            {
                logger.WriteError("Failed to create ILogger implementation.: " + exception.ToString(), 152);  
                throw exception;
            }
            return logger;
        }

        /// <summary>
        /// The folder that is being used for log output.
        /// </summary>
        public static string LogFolder
        {
            get
            {
                ILogger logger = GetLogger();
                return logger.LogFolder;
            }
        }

        /// <summary>
        /// Writes an error message to the log.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public static void WriteError(string message)
        {
            ILogger logger = GetLogger();
            logger.WriteError(message);
        }

        /// <summary>
        /// Writes an error message to the log.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public static void WriteError(string message, int eventId)
        {
            ILogger logger = GetLogger();
            logger.WriteError(message, eventId);
        }

        /// <summary>
        /// Writes a warning message to the log.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public static void WriteWarning(string message)
        {
            ILogger logger = GetLogger();
            logger.WriteWarning(message);
        }

        /// <summary>
        /// Writes an information message to the log.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public static void WriteWarning(string message, int eventId)
        {
            ILogger logger = GetLogger();
            logger.WriteWarning(message, eventId);
        }

        /// <summary>
        /// Writes an information message to the log.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public static void WriteInformation(string message)
        {
            ILogger logger = GetLogger();
            logger.WriteInformation(message);
        }

        /// <summary>
        /// Writes an information message to the log.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public static void WriteInformation(string message, int eventId)
        {
            ILogger logger = GetLogger();
            logger.WriteInformation(message, eventId);
        }

        /// <summary>
        /// Writes a trace message to the log.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public static void WriteTrace(string message)
        {
            try
            {
                ILogger logger = GetLogger();
                logger.WriteTrace(message);
            }
            catch (Exception)
            {
                // Ignore errors in tracing.
            }
        }

        /// <summary>
        /// Writes a trace message to the log.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public static void WriteTrace(string message, int eventId)
        {
            try
            {
                ILogger logger = GetLogger();
                logger.WriteTrace(message, eventId);
            }
            catch (Exception)
            {
                // Ignore errors in tracing.
            }
        }

        /// <summary>
        /// Writes an event message to the log.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public static void WriteEvent(int eventId, EventLogEntryType eventType, string message)
        {
            ILogger logger = GetLogger();
            logger.WriteEvent(eventId, eventType, message);
        }

        /// <summary>
        /// Writes an event message to the log.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public static void WriteEvent(string eventSource, string message, EventLogEntryType eventType, int eventId)
        {
            ILogger logger = GetLogger();
            logger.WriteEvent(eventSource, message, eventType, eventId);
        }

        /// <summary>
        ///Gets the event source associated with a given eventId.
        /// </summary>
        /// <param name="message">eventId</param>
        public static string GetEventSource(int eventId)
        {
            ILogger logger = GetLogger();
            return logger.GetEventSource(eventId);

        }

    }
}
