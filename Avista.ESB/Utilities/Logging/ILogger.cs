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
using Avista.ESB.Utilities.Components;

namespace Avista.ESB.Utilities.Logging
{
    public interface ILogger : IComponent
    {
        /// <summary>
        /// The folder that is being used for log output.
        /// </summary>
        string LogFolder
        {
            get;
        }

        /// <summary>
        /// Writes an error message to the log.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        void WriteError(string message);

        /// <summary>
        /// Writes an error message to the log.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        void WriteError(string message, int eventId);

        /// <summary>
        /// Writes a warning message to the log.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        void WriteWarning(string message);

        /// <summary>
        /// Writes a warning message to the log.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        void WriteWarning(string message, int eventId);

        /// <summary>
        /// Writes an information message to the log.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        void WriteInformation(string message);

        /// <summary>
        /// Writes an information message to the log.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        void WriteInformation(string message, int eventId);

        /// <summary>
        /// Writes a trace message to the log.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        void WriteTrace(string message);

        /// <summary>
        /// Writes a trace message to the log.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        void WriteTrace(string message, int eventId);

        /// <summary>
        /// Writes an event message to the log.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        void WriteEvent(int eventId, EventLogEntryType eventType, string message);

        /// <summary>
        /// Writes an event message to the log.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        void WriteEvent(string eventSource, string message, EventLogEntryType eventType, int eventId);

        /// <summary>
        /// Gets the event source associated with a given eventId.
        /// </summary>
        /// <param name="eventId">The eventId to look up.</param>
        /// <returns>The event source associated with the eventId.</returns>
        string GetEventSource(int eventId);

        /// <summary>
        /// Indicates if an event id will be filtered based on configured filter settings.
        /// </summary>
        /// <param name="eventId">The event id to check.</param>
        /// <returns>True if the given event id is filtered.</returns>
        bool IsFiltered(int eventId);
    }
}
