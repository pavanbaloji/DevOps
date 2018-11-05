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
using System.Diagnostics;
using Avista.ESB.Utilities.Components;
using Avista.ESB.Utilities.Logging.Configuration;
using NLog;

namespace Avista.ESB.Utilities.Logging
{
    /// <summary>
    /// Performs logging using NLog. This class should not be used directly. Instead, the Logger class should be used
    /// and it will redirect calls to this class if this is the configured logging provider.
    /// </summary>
    public class NlogLogger : ComponentBase, ILogger
    {
        /// <summary>
        /// The NLog Logger object used to perform the logging.
        /// </summary>
        private NLog.Logger _logger;

        /// <summary>
        /// The folder used for logging.
        /// </summary>
        private string _logFolder = "C:\\Windows\\Temp";

        /// <summary>
        /// A flag to indicate whether or not logged messages should be forwarded to the EventLogger.
        /// </summary>
        private bool _forwardToEventLogger = true;

        /// <summary>
        /// SortedList of {min, EventRange} pairs. We use the minimum of the range as the key for sorting.
        /// </summary>
        private SortedList<int, EventRange> _eventSources;

        /// <summary>
        /// The event logging source is related to the process using the event logger and should be defined in the
        /// application configuration file. The default value is "Avista.ESB.Utilities".
        /// </summary>
        private const string _defaultEventSource = "Avista.ESB.Utilities";

        /// <summary>
        /// The event id filter is used to filter the display of specific event.
        /// The value is read from the configuration file. It should be a comma delimited list of event ids with no whitespace.
        /// </summary>
        private string _eventIdFilter = null;

        /// <summary>
        /// Constructor for the NLogLogger.
        /// </summary>
        /// <remarks>
        /// Client code should not construct and use this class directly. Instead, they should
        /// use the <see cref="HP.Practices.Logging.Logger"/> class. It provides static
        /// methods to access a singleton instance of whichever ILogger provider has been
        /// configured. That provider may or may not be an instance of the NlogLogger.
        /// </remarks>
        /// <param name="name">The name of the logger instance.</param>
        public NlogLogger(string name)
            : base(name)
        {
            _logger = NLog.LogManager.GetLogger(name);
        }

        /// <summary>
        /// Refreshes configuration settings for the NlogLogger from the configuration file.
        /// </summary>
        public override void RefreshConfiguration()
        {
            try
            {
                base.RefreshConfiguration();
                LoggingSection loggingSection = LoggingSection.GetSection();
                LoggingSettingsElement loggingSettingsElement = loggingSection.LoggingSettings;
                _logFolder = loggingSettingsElement.LogFolder;
                _forwardToEventLogger = loggingSettingsElement.ForwardToEventLogger;                
                // Set eventSources according to the configuration.
                _eventSources = new SortedList<int, EventRange>();
                LoggingSourceCollection eventSourcesCollection = loggingSection.LoggingSettings.Sources;
                foreach (LoggingSourceElement eventSourceElement in eventSourcesCollection)
                {
                    string source = eventSourceElement.Name;
                    int min = 0;
                    int max = 0;
                    string ranges = eventSourceElement.Range;
                    string[] rangeList = ranges.Split(',');
                    foreach (string range in rangeList)
                    {
                        int pos = range.IndexOf('-');
                        if ((pos <= 0) || (pos >= range.Length - 1))
                        {
                            throw new Exception("Invalid format for event range. Expected hyphen between min and max values.");
                        }
                        else
                        {
                            try
                            {
                                min = Int32.Parse(range.Substring(0, pos));
                                max = Int32.Parse(range.Substring(pos + 1));
                            }
                            catch (Exception exception)
                            {
                                throw new Exception("Invalid format for event range. Could not parse min and max values.", exception);
                            }
                            EventRange eventRange = new EventRange(min, max, source);
                            _eventSources.Add(min, eventRange);
                        }
                    }
                }
                // Set the eventIdFilter according to the configuration.
                _eventIdFilter = loggingSection.LoggingSettings.EventIdFilter;
                if (_eventIdFilter != null)
                {
                    _eventIdFilter = "," + _eventIdFilter + ",";
                }
            }
            catch (Exception exception)
            {                
                Logger.WriteWarning("The configuration for the NlogLogger could not be loaded from the application configuration file. Default values will be used.", 151);
                throw exception;
            }            
        }

        /// <summary>
        /// The folder that is being used for log output.
        /// </summary>
        public string LogFolder
        {
            get
            {
                return _logFolder;
            }
        }

        /// <summary>
        /// Indicates whether or not logged messages should be forwarded to the EventLogger.
        /// </summary>
        public bool ForwardToEventLogger
        {
            get
            {
                return _forwardToEventLogger;
            }
            set
            {
                _forwardToEventLogger = value;
            }
        }

        /// <summary>
        /// Writes a message to NLog at the Error level.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public void WriteError(string message)
        {
            try
            {
                _logger.Error(message);
            }
            catch (Exception exception)
            {
                string error = "An exception occurred while trying to write an error message to Nlog. " + Environment.NewLine +
                               "The original message was as follows: " + Environment.NewLine +
                               message;
                Logger.WriteError(error, 150);
                throw exception;
            }
        }

        /// <summary>
        /// Writes a message to NLog at the Error level.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public void WriteError(string message, int eventId)
        {
            try
            {
                NLog.MappedDiagnosticsContext.Set("EventSource", GetEventSource(eventId));
                NLog.MappedDiagnosticsContext.Set("EventId", eventId.ToString());
                _logger.Error(message);
            }
            catch (Exception exception)
            {
                string error = "An exception occurred while trying to write an error message to Nlog. " + Environment.NewLine +
                               "The original message was as follows: " + Environment.NewLine +
                               message;
                Logger.WriteError(error, 150);
                throw exception;
            }
        }

        /// <summary>
        /// Writes a message to NLog at the Warning level.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public void WriteWarning(string message)
        {
            try
            {
                _logger.Warn(message);
            }
            catch (Exception exception)
            {
                string error = "An exception occurred while trying to write a warning message to Nlog. " + Environment.NewLine +
                               "The original message was as follows: " + Environment.NewLine +
                               message;
                Logger.WriteError(error, 150);
                throw exception;
            }
        }

        /// <summary>
        /// Writes a message to NLog at the Warning level.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public void WriteWarning(string message, int eventId)
        {
            try
            {
                NLog.MappedDiagnosticsContext.Set("EventSource", GetEventSource(eventId));
                NLog.MappedDiagnosticsContext.Set("EventId", eventId.ToString());
                _logger.Warn(message);
            }
            catch (Exception exception)
            {
                string error = "An exception occurred while trying to write a warning message to Nlog. " + Environment.NewLine +
                               "The original message was as follows: " + Environment.NewLine +
                               message;
                Logger.WriteError(error, 150);
                throw exception;
            }
        }

        /// <summary>
        /// Writes a message to NLog at the Info level.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public void WriteInformation(string message)
        {
            try
            {
                _logger.Info(message);
            }
            catch (Exception exception)
            {
                string error = "An exception occurred while trying to write an info message to Nlog. " + Environment.NewLine +
                               "The original message was as follows: " + Environment.NewLine +
                               message;
                Logger.WriteError(error, 150);
                throw exception;
            }
        }

        /// <summary>
        /// Writes a message to NLog at the Info level.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public void WriteInformation(string message, int eventId)
        {
            try
            {
                NLog.MappedDiagnosticsContext.Set("EventSource", GetEventSource(eventId));
                NLog.MappedDiagnosticsContext.Set("EventId", eventId.ToString());
                _logger.Info(message);
            }
            catch (Exception exception)
            {
                string error = "An exception occurred while trying to write an info message to Nlog. " + Environment.NewLine +
                               "The original message was as follows: " + Environment.NewLine +
                               message;
                Logger.WriteError(error, 150);
                throw exception;
            }
        }

        /// <summary>
        /// Writes a message to NLog at the Trace level.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public void WriteTrace(string message)
        {
            try
            {
                _logger.Trace(message);
            }
            catch (Exception exception)
            {
                string error = "An exception occurred while trying to write a trace message to Nlog. " + Environment.NewLine +
                               "The original message was as follows: " + Environment.NewLine +
                               message;
                Logger.WriteError(error, 150);
                throw exception;
            }
        }

        /// <summary>
        /// Writes a message to NLog at the Trace level.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public void WriteTrace(string message, int eventId)
        {
            try
            {
                NLog.MappedDiagnosticsContext.Set("EventSource", GetEventSource(eventId));
                NLog.MappedDiagnosticsContext.Set("EventId", eventId.ToString());
                _logger.Trace(message);
            }
            catch (Exception exception)
            {
                string error = "An exception occurred while trying to write a trace message to Nlog. " + Environment.NewLine +
                               "The original message was as follows: " + Environment.NewLine +
                               message;
                Logger.WriteError(error, 150);
                throw exception;
            }
        }

        /// <summary>
        /// Gets the event source associated with a given eventId.
        /// </summary>
        /// <param name="eventId">The eventId to look up.</param>
        /// <returns>The event source associated with the eventId.</returns>
        public string GetEventSource(int eventId)
        {
            string eventSource = _defaultEventSource;
            if (_eventSources != null && _eventSources.Count > 0)
            {
                IEnumerator<KeyValuePair<int, EventRange>> enumerator = _eventSources.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    EventRange eventRange = enumerator.Current.Value;
                    if (eventRange.Includes(eventId))
                    {
                        eventSource = eventRange.Source;
                        break;
                    }
                }
            }
            return eventSource;
        }

        /// <summary>
        /// Indicates if an event id will be filtered based on configured filter settings.
        /// </summary>
        /// <param name="eventId">The event id to check.</param>
        /// <returns>True if the given event id is filtered.</returns>
        public bool IsFiltered(int eventId)
        {
            bool filtered = false;
            try
            {
                if (_eventIdFilter != null && _eventIdFilter.Contains("," + eventId.ToString() + ","))
                {
                    filtered = true;
                }
            }
            catch (Exception)
            {
                // Ignore exceptions in the filtering process.
            }
            return filtered;
        }

        /// <summary>
        /// Writes an event message to NLog.
        /// </summary>
        /// <param name="eventId">The event id to assocuiate with the message.</param>
        /// <param name="eventType">The event type to associate with the message.</param>
        /// <param name="message">The message.</param>
        public void WriteEvent(int eventId, EventLogEntryType eventType, string message)
        {
            string eventSource = "unknown";
            try
            {
                if (!IsFiltered(eventId))
                {
                    eventSource = GetEventSource(eventId);
                    NLog.LogEventInfo logEventInfo = new NLog.LogEventInfo(NLog.LogLevel.Error, _logger.Name, message);
                    switch (eventType)
                    {
                        case EventLogEntryType.Error:
                            {
                                logEventInfo.Level = NLog.LogLevel.Error;
                                break;
                            }
                        case EventLogEntryType.Warning:
                            {
                                logEventInfo.Level = NLog.LogLevel.Warn;
                                break;
                            }
                        case EventLogEntryType.Information:
                            {
                                logEventInfo.Level = NLog.LogLevel.Info;
                                break;
                            }
                        case EventLogEntryType.FailureAudit:
                            {
                                logEventInfo.Level = NLog.LogLevel.Error;
                                break;
                            }
                        case EventLogEntryType.SuccessAudit:
                            {
                                logEventInfo.Level = NLog.LogLevel.Info;
                                break;
                            }
                        default:
                            {
                                logEventInfo.Level = NLog.LogLevel.Error;
                                break;
                            }
                    }
                    logEventInfo.Properties.Add("EventID", eventId);
                    _logger.Log(typeof(NlogLogger), logEventInfo);
                    if (_forwardToEventLogger)
                    {                        
                        int maxLength = 30000;
                        if (message.Length > maxLength)
                        {
                            int keepLength = maxLength - 200; // We reserve 200 bytes for the bytes removed notice.
                            int midLength = keepLength / 2;
                            int removed = message.Length - keepLength;
                            message = message.Substring(0, midLength) + Environment.NewLine +
                                           "-----------------------------------------------------------" + Environment.NewLine +
                                           removed + " bytes removed due to length restrictions." + Environment.NewLine +
                                           "-----------------------------------------------------------" + Environment.NewLine +
                                           message.Substring(midLength + removed);
                        }
                        EventLog.WriteEntry(eventSource, message, eventType, eventId);
                    }
                }
            }
            catch (Exception e)
            {
                string eventMessage = "An exception occurred while the WindowsEventLogger was trying to write an event to the Windows event log. " + Environment.NewLine +
                                 e.Message + " " + Environment.NewLine +
                                 "The original event was as follows: " + Environment.NewLine +
                                 "   Event Source: " + eventSource + Environment.NewLine +
                                 "   Event Type: " + eventType.ToString() + Environment.NewLine +
                                 "   Event Id: " + eventId.ToString() + Environment.NewLine +
                                 "   Event Message: " + message;
                Logger.WriteError(eventMessage, 140);
                throw new Exception(eventMessage, e);
            }
        }

        /// <summary>
        /// Writes an event message to NLog.
        /// </summary>
        /// <param name="eventSource">The event source to assocuiate with the message.</param>
        /// <param name="message">The message.</param>
        /// <param name="eventType">The event type to associate with the message.</param>
        /// <param name="eventId">The event id to assocuiate with the message.</param>
        public void WriteEvent(string eventSource, string message, EventLogEntryType eventType, int eventId)
        {
            try
            {
                if (!IsFiltered(eventId))
                {
                    NLog.LogEventInfo logEventInfo = new NLog.LogEventInfo(NLog.LogLevel.Error, _logger.Name, message);
                    switch (eventType)
                    {
                        case EventLogEntryType.Error:
                            {
                                logEventInfo.Level = NLog.LogLevel.Error;
                                break;
                            }
                        case EventLogEntryType.Warning:
                            {
                                logEventInfo.Level = NLog.LogLevel.Warn;
                                break;
                            }
                        case EventLogEntryType.Information:
                            {
                                logEventInfo.Level = NLog.LogLevel.Info;
                                break;
                            }
                        case EventLogEntryType.FailureAudit:
                            {
                                logEventInfo.Level = NLog.LogLevel.Error;
                                break;
                            }
                        case EventLogEntryType.SuccessAudit:
                            {
                                logEventInfo.Level = NLog.LogLevel.Info;
                                break;
                            }
                        default:
                            {
                                logEventInfo.Level = NLog.LogLevel.Error;
                                break;
                            }
                    }
                    logEventInfo.Properties.Add("EventID", eventId);
                    logEventInfo.Properties.Add("EventSource", eventSource);
                    _logger.Log(typeof(NlogLogger), logEventInfo);
                    if (_forwardToEventLogger)
                    {
                        int maxLength = 30000;
                        if (message.Length > maxLength)
                        {
                            int keepLength = maxLength - 200; // We reserve 200 bytes for the bytes removed notice.
                            int midLength = keepLength / 2;
                            int removed = message.Length - keepLength;
                            message = message.Substring(0, midLength) + Environment.NewLine +
                                           "-----------------------------------------------------------" + Environment.NewLine +
                                           removed + " bytes removed due to length restrictions." + Environment.NewLine +
                                           "-----------------------------------------------------------" + Environment.NewLine +
                                           message.Substring(midLength + removed);
                        }
                        EventLog.WriteEntry(eventSource, message, eventType, eventId);
                    }
                }
            }
            catch (Exception e)
            {
                string eventMessage = "An exception occurred while the WindowsEventLogger was trying to write an event to the Windows event log. " + Environment.NewLine +
                                 e.Message + " " + Environment.NewLine +
                                 "The original event was as follows: " + Environment.NewLine +
                                 "   Event Source: " + eventSource + Environment.NewLine +
                                 "   Event Type: " + eventType.ToString() + Environment.NewLine +
                                 "   Event Id: " + eventId.ToString() + Environment.NewLine +
                                 "   Event Message: " + message;
                Logger.WriteError(eventMessage, 140);
                throw new Exception(eventMessage, e);
            }
        }
    }
}
