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
using System.Diagnostics;
using Avista.ESB.Utilities.Components;

namespace Avista.ESB.Utilities.Logging
{
    /// <summary>
    /// A logger implementation which ignores logging requests.
    /// </summary>
    public class NullLogger : ComponentBase, ILogger
    {
        /// <summary>
        /// Constructor for the NullLogger.
        /// </summary>
        /// <param name="name">The name of the logger instance.</param>
        public NullLogger(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Refreshes configuration from the configuration file. The NullLogger requires no configuration.
        /// </summary>
        public override void RefreshConfiguration()
        {
            base.RefreshConfiguration();
        }

        /// <summary>
        /// The folder that is being used for log output. The NullLogger simply returns the empty string.
        /// </summary>
        public string LogFolder
        {
            get
            {
                return "";
            }
        }

        /// <summary>
        /// Ignores the Error message.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public void WriteError(string message)
        {
        }

        /// <summary>
        /// Ignores the Error message.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public void WriteError(string message, int eventId)
        {
        }

        /// <summary>
        /// Ignores the Warning message.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public void WriteWarning(string message)
        {
        }

        /// <summary>
        /// Ignores the Info message.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public void WriteWarning(string message, int eventId)
        {
        }

        /// <summary>
        /// Ignores the Info message.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public void WriteInformation(string message)
        {
        }

        /// <summary>
        /// Ignores the Info message.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public void WriteInformation(string message, int eventId)
        {
        }

        /// <summary>
        /// Ignoes the Trace message.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public void WriteTrace(string message)
        {
        }

        /// <summary>
        /// Ignoes the Trace message.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public void WriteTrace(string message, int eventId)
        {
        }

        /// <summary>
        /// Gets the event source associated with a given eventId.
        /// </summary>
        /// <param name="eventId">The eventId to look up.</param>
        /// <returns>The event source associated with the eventId.</returns>
        public string GetEventSource(int eventId)
        {              
                return "";            
        }

        /// <summary>
        /// Ignoes the Event message.
        /// </summary>
        /// <param name="eventId">The event id to assocuiate with the message.</param>
        /// <param name="eventType">The event type to associate with the message.</param>
        /// <param name="message">The message.</param>
        public void WriteEvent(int eventId, EventLogEntryType eventType, string message)
        {
        }

        public void WriteEvent(string eventSource, string message, EventLogEntryType eventType, int eventId)
        {
        }

        /// <summary>
        /// Indicates if an event id will be filtered based on configured filter settings.
        /// </summary>
        /// <param name="eventId">The event id to check.</param>
        /// <returns>True if the given event id is filtered.</returns>
        public bool IsFiltered(int eventId)
        {
            return false;
        }
    }
}
