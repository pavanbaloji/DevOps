using System.Diagnostics;

namespace Avista.ESB.Testing.Integration
{
    public class EventInfo
    {
        public EventInfo(int eventId, string eventSource)
        {
            EventId = eventId;
            EventSource = eventSource;
            EventCount = 1;
            EventType = EventLogEntryType.Error;
            MaxWaitTime = null;
            EventMessage = null;
        }

        public int EventId;
        public string EventMessage;
        public string EventSource;
        public EventLogEntryType EventType;
        public int EventCount;
        public int? MaxWaitTime;

        public EventInfo()
        {
        }
    }
}