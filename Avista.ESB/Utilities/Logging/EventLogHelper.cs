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
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;

namespace Avista.ESB.Utilities.Logging
{
    public static class EventLogHelper
    {
        private static AutoResetEvent _signal;
        private static bool _found = false;
        private static int _eventCount;
        static readonly object _lock = new object();

        /// <summary>
        /// Clears the events from the Application event log.
        /// </summary>
        public static void ClearEventLog()
        {
            EventLog eventLog = new EventLog("Application");
            eventLog.Clear();
        }

        /// <summary>
        /// Writes all the current application event log entries to the console.
        /// </summary>
        public static void WriteEventLogEntries()
        {
            EventLog eventLog = new EventLog("Application");
            foreach (EventLogEntry eventLogEntry in eventLog.Entries)
            {
                Console.WriteLine("EventId: {0}, Source: {1}, Type: {2}, Message: {3}", eventLogEntry.InstanceId,
                    eventLogEntry.Source, eventLogEntry.EntryType, eventLogEntry.Message);
            }
        }

        /// <summary>
        /// Looks for a message in the Windows event log. The event log is checked approximately every 25 ms (1/40th of a second) up to
        /// the max wait time specified. Specify null for the message to match any message.
        /// </summary>
        /// <param name="eventId">The event id to look for in the Application event log. Specify -1 for the id to match any event id.</param>
        /// <param name="eventCategory">The event category to look for in the Application event log. Specify null to match any event category.</param>
        /// <param name="eventType">The event type to look for in the Application event log. Specify null to match any event type.</param>
        /// <param name="eventSource">The event source to look for in the Application event log. Specify null to match any event source.</param>
        /// <param name="eventMessage">The message to look for in the Application event log. Use null if you don't want to match on the message text. Wildcard search can be performed by using *.  It will match for strings on either end of the *.</param>
        /// <param name="exact">Flag to indicate that the message text must be an exact match. If false is specified then the event log entry only needs to contain the message text passed in the message argument to be considered a match.</param>
        /// <param name="maxWaitTimeMs">The maximum amount of time in milliseconds to wait for the event to appear.</param>
        /// <param name="occurences">The number of occurences of the event to wait for.</param>
        /// <returns>True if the message was found in the event log. False if the message was not found in the event log.</returns>
        public static bool WaitForEvent(int eventId, string eventCategory, string eventType, string eventSource,
            string eventMessage, bool exact, int maxWaitTimeMs, int occurrences = 1)
        {
            IEnumerable<EventLogEntry> results = WaitForEvent(eventId, eventSource, maxWaitTimeMs, occurrences,
                eventCategory, eventType, exact, eventMessage);
            return results.Count() == occurrences;
        }

        /// <summary>
        /// Looks for a message in the Windows event log. The event log is checked approximately every 25 ms (1/40th of a second) up to
        /// the max wait time specified. Specify null for the message to match any message.
        /// </summary>
        /// <param name="eventId">The event id to look for in the Application event log. Specify -1 for the id to match any event id.</param>
        /// <param name="eventSource">The event source to look for in the Application event log. Specify null to match any event source.</param>
        /// <param name="maxWaitTimeMs">The maximum amount of time in milliseconds to wait for the event to appear.</param>
        /// <param name="occurences">The number of occurences of the event to wait for.</param>
        /// <param name="eventCategory">The event category to look for in the Application event log. Specify null to match any event category.</param>
        /// <param name="eventType">The event type to look for in the Application event log. Specify null to match any event type.</param>
        /// <param name="exact">Flag to indicate that the message text must be an exact match. If false is specified then the event log entry only needs to contain the message text passed in the message argument to be considered a match.</param>
        /// <param name="eventMessages">The messages to look for in the Application event log. Use null if you don't want to match on the message text. Wildcard search can be performed by using *.  It will match for strings on either end of the *.</param>
        /// <returns>IEnumerable<EventLogEntry> for matching entries in the event log.</returns>
        public static IEnumerable<EventLogEntry> WaitForEvent(int eventId, string eventSource, int maxWaitTimeMs,
            int occurences, string eventCategory = null, string eventType = null, bool exact = false,
            params string[] eventMessages)
        {
            int timerMs = 0;
            int waitIntervalMs = 25;
            int waitTimeMs = 0;
            int waitTimeThreshold = 3;
            int waitTimeThresholdCounter = 0;
            List<EventLogEntry> foundEventLogEntries = new List<EventLogEntry>();
            EventLog eventLog = new EventLog("Application");
            while (timerMs < maxWaitTimeMs && (foundEventLogEntries.Count < occurences))
            {
                foundEventLogEntries = new List<EventLogEntry>();
                DateTime start = DateTime.Now;
                foreach (EventLogEntry entry in eventLog.Entries)
                {
                    if ((eventId == -1) || (entry.InstanceId == eventId))
                    {
                        if (eventCategory == null || entry.Category == eventCategory)
                        {
                            if (eventType == null || entry.EntryType.ToString() == eventType)
                            {
                                if (eventSource == null || entry.Source == eventSource)
                                {
                                    if ((eventMessages == null || eventMessages.Length == 0) ||
                                        (exact && (eventMessages.Any(item => entry.Message == item)) ||
                                         (eventMessages.ToList().TrueForAll(item => entry.Message.Contains(item)))))
                                    {
                                        foundEventLogEntries.Add(entry);
                                    }
                                }
                            }
                        }
                    }
                }
                // If we haven't found what we're looking for then maybe wait a few milliseconds.
                if (foundEventLogEntries.Count < occurences)
                {
                    DateTime end = DateTime.Now;
                    TimeSpan duration = end - start;
                    int searchTimeMs = duration.Milliseconds + duration.Seconds*1000 + duration.Minutes*60000;
                    // This is the time we spent searching in this iteration.
                    timerMs = timerMs + searchTimeMs; // This is the total time we've been searching so far.
                    if (timerMs < maxWaitTimeMs)
                    {
                        // If the search was fast then we can sleep for the remainder of the wait interval.
                        if (searchTimeMs < waitIntervalMs)
                        {
                            waitTimeMs = waitIntervalMs - searchTimeMs;
                            Thread.Sleep(waitTimeMs);
                            timerMs = timerMs + waitTimeMs;
                        }
                        else
                        {
                            // If the search was slow then we'll increment a counter to track how many successive slow searches have occurred.
                            waitTimeThresholdCounter = waitTimeThresholdCounter + 1;
                        }
                        // If we have had 3 successive slow searches then we will wait for a full second before continuing.
                        // This prevents our searching from becoming a CPU hog when there are a large number of events in the event log.
                        if (waitTimeThresholdCounter == waitTimeThreshold)
                        {
                            Thread.Sleep(1000);
                            timerMs = timerMs + 1000;
                            waitTimeThresholdCounter = 0;
                        }
                    }
                }
            }
            return foundEventLogEntries;
        }

        /// <summary>
        /// Waits a given number of milliseconds and then checks for error events.
        /// </summary>
        /// <param name="minWaitTimeMs">The number of milliseconds to wait before checking for events.</param>
        /// <returns>The list of error events that have been detected.</returns>
        public static IEnumerable<EventLogEntry> WaitForErrors(int waitTimeMs)
        {
            Thread.Sleep(waitTimeMs);
            return WaitForEvent(-1, null, 1, 1, null, "Error", false, null);
        }


        private static int CheckEventCount(int eventId, string eventMessage, EventLogEntryType entryType)
        {
            int eventsExist = 0;
            EventLog eventLog = new EventLog("Application");
            if (string.IsNullOrWhiteSpace(eventMessage))
            {
                eventsExist =
                    eventLog.Entries.Cast<EventLogEntry>()
                        .Count(x => (x.InstanceId == eventId) && (x.EntryType == entryType));
            }
            else
            {
                eventsExist =
                    eventLog.Entries.Cast<EventLogEntry>()
                        .Count(
                            x =>
                                (x.InstanceId == eventId) && x.Message.Contains(eventMessage) &&
                                (x.EntryType == entryType));
            }
            eventLog.Close();
            return eventsExist;
        }

        /// <summary>
        /// WaitForEvent2 where "2" designates alternate event subscription paradigm of waiting for event
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="source"></param>
        /// <param name="entryType"></param>
        /// <param name="timeOutSeconds"></param>
        /// <param name="action"></param>
        /// <param name="expectedEventCount"></param>
        /// <returns></returns>
        public static bool WaitForEvent2(int eventId, string eventMessage, string source, EventLogEntryType entryType,
            int timeOutSeconds,
            Action action, int expectedEventCount)
        {
            lock (_lock)
            {
                //really intended for single threaded tests - locking this region just in case
                _found = false;
                _signal = new AutoResetEvent(false);
                _eventCount = expectedEventCount;
                EventLogWatcher watcher = null;
                //try
                //{
                int level = -1;
                switch (entryType)
                {
                    case EventLogEntryType.Information:
                        level = 4;
                        break;
                    case EventLogEntryType.Warning:
                        level = 3;
                        break;
                    case EventLogEntryType.Error:
                        level = 2;
                        break;
                }
                //The following if check is for use when you check for multiple events in subsequent, separate calls -e.g.
                //WaitForEvent2(1000, ....);  --> Here the event subscription will kick in looking for 1000...alll the while 2000 events may be logged
                //WaitForEvent2(2000, ....);  --> 2000 events already logged won't be caught
                //cff: there is a theoretical threading issue if you do the above
                int eventsExist = CheckEventCount(eventId, eventMessage, entryType);
                _eventCount -= eventsExist;
                if (_eventCount == 0)
                {
                    _found = true;
                }

                else
                {
                    //here's where the theoretical threading issue occurs ONLY if you are doing two subsequent WaitForEvent2's
                    //inbetween if the if statement above and the subscription below

                    EventLogQuery subscriptionQuery = new EventLogQuery(
                        "Application", PathType.LogName,
                        string.Format("*[System[Provider[@Name='{0}'] and (EventID={1}) and (Level={2})]]", source,
                            eventId,
                            level));

                    using (watcher = new EventLogWatcher(subscriptionQuery))
                    {
                        // Make the watcher listen to the EventRecordWritten
                        // events.  When this event happens, the callback method
                        // (EventLogEventRead) is called.
                        watcher.EventRecordWritten +=
                            EventLogEventRead;

                        // Activate the subscription
                        watcher.Enabled = true;
                        action();
                        _signal.WaitOne(timeOutSeconds*1000);
                        watcher.Enabled = false;
                    }
                    //if (_eventCount > 0)
                    //{
                    eventsExist = CheckEventCount(eventId, eventMessage, entryType);
                    if (eventsExist == expectedEventCount)
                    {
                        _found = true;
                    }
                    else
                    {
                        _found = false;
                    }
                    //}
                }
            }
            return _found;
        }


        /// <summary>
        ///Callback method that gets executed when an event is
        // reported to the subscription. 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="arg"></param>
        private static void EventLogEventRead(object obj,
            EventRecordWrittenEventArgs arg)
        {
            // Make sure there was no error reading the event.
            if (arg.EventRecord != null)
            {
                _eventCount--;
                if (_eventCount == 0)
                {
                    _signal.Set();
                }
            }
            else
            {
                throw new ApplicationException("Error reading event");
            }
        }


        /// <summary>
        /// Check current event list for any of 'Error' type.
        /// </summary>
        public static bool AreErrorEventsPresent()
        {
            var entries = new EventLog("Application").Entries;
            var errorEntries = entries.Cast<EventLogEntry>().Where(e => e.EntryType == EventLogEntryType.Error);
            return errorEntries.Any();
        }
    }
}