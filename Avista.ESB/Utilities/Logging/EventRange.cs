using System;

namespace Avista.ESB.Utilities.Logging
{
    /// <summary>
    /// Defines a range of event ids and parameters related to the range.
    /// </summary>
    public class EventRange
    {
        /// <summary>
        /// Holds the minimum event id for the range.
        /// </summary>
        private int min;

        /// <summary>
        /// Holds the maximum event id for the range.
        /// </summary>
        private int max;
                
        /// <summary>
        /// Holds the source of the event range.
        /// </summary>
        private string source;

        /// <summary>
        /// Constructs an EventRange with the given minimum and maximum event ids and event source.
        /// </summary>
        /// <param name="min">The minimum event id for the range.</param>
        /// <param name="max">The maximum event id for the range.</param>
        /// <param name="source">The source of the event range.</param>
        public EventRange(int min, int max, string source)
        {
            this.min = min;
            this.max = max;
            this.source = source;
        }

        /// <summary>
        /// The minimum event id for the range.
        /// </summary>
        public int Min
        {
            get
            {
                return min;
            }
        }

        /// <summary>
        /// The maximum event id for the range.
        /// </summary>
        public int Max
        {
            get
            {
                return max;
            }
        }

        /// <summary>
        /// The source of the event range.
        /// </summary>
        public string Source
        {
            get
            {
                return source;
            }
        }

        /// <summary>
        /// Checks whether or not a given eventId is in the range.
        /// </summary>
        /// <param name="eventId">The eventId to be checked.</param>
        /// <returns>True if the given eventId is in the range, false if it is not in the range.</returns>
        public bool Includes(int eventId)
        {
            return (eventId >= min && eventId <= max);
        }
    }
}
