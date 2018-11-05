using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;

namespace Avista.ESB.Testing.Integration
{
    /// <summary>
    /// EventLogMonitor
    /// </summary>
    public class EventLogMonitor
    {
        
        private static EventLogMonitor _instance;

        public static EventLogMonitor Instance
        {
            get
            {
                return _instance ?? (_instance = new EventLogMonitor());
            }
        }

        /// <summary>
        /// Wrapper for log entry including observed flag
        /// </summary>
        protected class LoggedEvent
        {
            public bool Observed { get; set; }

            public EventLogEntry LogEntry { get; set; }

            public LoggedEvent(EventLogEntry logEntry, bool observed = false)
            {
                LogEntry = logEntry;
                Observed = observed;
            }

            public LoggedEvent(EventLogEntry logEntry)
            {
                LogEntry = logEntry;
                Observed = false;
            }
        }

        protected EventLog EventLog;
        protected SynchronizedCollection<LoggedEvent> EventLogEntries;
        public static readonly int DefaultEventTimeout = 30;

        /// <summary>
        /// Constructor, optional log name, default value is "Application"
        /// </summary>
        /// <param name="logName"></param>
        private EventLogMonitor(string logName="Application")
        {
            EventLog = new EventLog(logName);
            EventLogEntries = new SynchronizedCollection<LoggedEvent>();
        }

        /// <summary>
        /// Start monitoring session, win event log is cleared, and internal store is cleared.
        /// </summary>
        public void StartCapture()
        {
            Trace.AutoFlush = true;
            // clear all events in log before monitoring
            EventLog.Clear();
            // clear internal session events
            ClearCaptureEntries();
            // set event handler
            EventLog.EntryWritten += OnEntryWritten;
            EventLog.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Clear entries saved for session
        /// </summary>
        private void ClearCaptureEntries()
        {
            StopCapture();
            EventLogEntries.Clear();
        }

        /// <summary>
        /// Stop monitoring session
        /// </summary>
        public void StopCapture()
        {
            // set event handler
            EventLog.EntryWritten -= OnEntryWritten;
            EventLog.EnableRaisingEvents = false;
        }

        /// <summary>
        ///  Method called on new log entry, entry is stored in session
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnEntryWritten(object source, EntryWrittenEventArgs e)
        {
            EventLogEntry entry = e.Entry;
            var loggedEvent = new LoggedEvent(entry);

            //TraceWriteLn("Captured event: " + entry.EventID);

            EventLogEntries.Add(loggedEvent);
        }

        /// <summary>
        /// Return count of Unobserved Events
        /// </summary>
        /// <returns></returns>
        public int UnobservedEventCount()
        {
            return EventLogEntries.Count(e => e.Observed == false);
        }

        /// <summary>
        /// Return count of Observed Events
        /// </summary>
        /// <returns></returns>
        public int ObservedEventCount()
        {
            return EventLogEntries.Count(e => e.Observed);
        }

        /// <summary>
        ///  Check if multiple events from param are all present
        /// </summary>
        /// <param name="eventInfo"></param>
        /// <returns></returns>
        public bool AreAllEventsObserved(EventInfo[] eventInfo)
        {
            if (eventInfo == null || eventInfo.Length <= 0)
            {
                // return true if no events are passed, means test doesn't depend on event processing. 
                return true;
            }

            bool retVal = true;
            foreach (EventInfo eInfo in eventInfo)
            {
                for (int i = 0; i < eInfo.EventCount; i++)
                {
                    if (!IsEventObserved(eInfo))
                    {
                        retVal = false;
                        break;
                    }
                }
            }
            return retVal;
        }

        /// <summary>
        /// Check if single event is present
        /// </summary>
        /// <param name="eventInfo"></param>
        /// <returns></returns>
        public bool IsEventObserved(EventInfo eventInfo)
        {
            if (eventInfo == null)
            {
                return false;
            }

            int maxWaitTime = eventInfo.MaxWaitTime.GetValueOrDefault(DefaultEventTimeout);
            bool retVal = false;

            for (int x = 1; x < maxWaitTime + 1 ; x++)
            {
                if (EventLogEntries.Count > 0)
                {
                    // select loggedEvent with appropriate criteria
                    var loggedEvent = RunQuery(eventInfo);

                    // if we still have an event mark it as observed
                    if (loggedEvent != null)
                    {
                        loggedEvent.Observed = true;
                        TraceWriteLn("Observed Event: " + loggedEvent.LogEntry.EventID);
                        retVal = true;
                        break;
                    }
                }
                // sleep 1 sec
                if ((x % 5) == 0) TraceWriteLn(" (" + x + ") Sleeping.");
                Thread.Sleep(1000);
            }
            return retVal;
        }

        /// <summary>
        /// Run query for logged event based on criteria in EventInfo
        /// </summary>
        /// <param name="eventInfo"></param>
        /// <returns></returns>
        private LoggedEvent RunQuery(EventInfo eventInfo)
        {
            LoggedEvent loggedEvent = null;
            IEnumerable <LoggedEvent> results = null;

            if (eventInfo.EventId >= 0) // kind of silly, but...
            {
                // select with minimun info
                results = EventLogEntries.Where(e => e.Observed == false &&
                                            e.LogEntry.EventID == eventInfo.EventId &&
                                            e.LogEntry.EntryType == eventInfo.EventType);

                // Event source populated?
                if (!string.IsNullOrWhiteSpace(eventInfo.EventSource)) 
                {
                    results = results.Where(e => e.LogEntry.Source == eventInfo.EventSource);
                }


                // EventMEssage populated?
                if (!string.IsNullOrWhiteSpace(eventInfo.EventMessage))
                {
                    results = results.Where(e => e.LogEntry.Message.Contains(eventInfo.EventMessage));
                }

                loggedEvent = results.DefaultIfEmpty(null).FirstOrDefault();
            }                    
            else
            {
                TraceWriteLn("Event lacks basic criteria for query (min: EventId >= 0)");
                // set as never found
                loggedEvent = null;
            }

            return loggedEvent;
        }

        /// <summary>
        /// Return a formatted version of all unobserved events in session
        /// </summary>
        /// <returns></returns>
        public string UnobservedEventsFormatted()
        {
            if (UnobservedEventCount() > 0)
            {
                IEnumerable<LoggedEvent> loggedEvents = EventLogEntries.Where(e => e.Observed == false);

                StringBuilder sb = new StringBuilder();
                sb.Append(String.Format("Unobserved Events (count={0}):", UnobservedEventCount()));
                sb.Append("\r\n");
                foreach (LoggedEvent entry in loggedEvents)
                {
                    // GUID surrounded by braces is being confused as "format" param
                    var msg = entry.LogEntry.Message.Replace("{", "{{").Replace("}", "}}");
                    // trucate msg if needed
                    msg = (msg.Length <= 40) ? msg : msg.Substring(0, 40);
                    sb.Append(String.Format(@"   EventID={0}, Source={1}, Type={2}, Message={3}, Observed={4}",
                        entry.LogEntry.EventID, entry.LogEntry.Source,
                        entry.LogEntry.EntryType, msg,
                        entry.Observed));
                    sb.Append("\r\n");
                }
                return sb.ToString();
            }
            return "Unobserved Events - none present.\r\n";
        }

        /// <summary>
        /// Return a formatted version of all observed events in session
        /// </summary>
        /// <returns></returns>
        public string ObservedEventsFormatted()
        {
            if (ObservedEventCount() > 0)
            {
                IEnumerable<LoggedEvent> loggedEvents = EventLogEntries.Where(e => e.Observed);

                StringBuilder sb = new StringBuilder();
                sb.Append(String.Format("Observed Events (count={0}):", ObservedEventCount()));
                sb.Append("\r\n");
                foreach (LoggedEvent entry in loggedEvents)
                {
                    // GUID surrounded by braces is being confused as "format" param
                    var msg = entry.LogEntry.Message.Replace("{", "{{").Replace("}", "}}");
                    // trucate msg if needed
                    msg = (msg.Length <= 40) ? msg : msg.Substring(0, 40);
                    sb.Append(String.Format(@"   EventID={0}, Source={1}, Type={2}, Message={3}, Observed={4}",
                        entry.LogEntry.EventID, entry.LogEntry.Source,
                        entry.LogEntry.EntryType, msg,
                        entry.Observed));
                    sb.Append("\r\n");
                }
                return sb.ToString();
            }
            return "Observed Events - none present.\r\n";
        }

        /// <summary>
        /// Writeln Trace message (with prefix applied).
        /// </summary>
        /// <param name="message"></param>
        private void TraceWriteLn(string message)
        {
            Trace.WriteLine(string.Format(" EVENT MONITOR: {0}",message) );
        }

    }
}
