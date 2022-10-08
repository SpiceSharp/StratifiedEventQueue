using System.Collections.Generic;
using StratifiedEventQueue.Events;

namespace StratifiedEventQueue.Simulation
{
    /// <summary>
    /// Describes an event region.
    /// </summary>
    /// <remarks>
    /// Events can be added and retrieved from an event region with no particular order.
    /// </remarks>
    public class EventRegion
    {
        private readonly Queue<Event> _events = new Queue<Event>();

        /// <summary>
        /// Gets the number of events in the region.
        /// </summary>
        public int Count => _events.Count;

        /// <summary>
        /// Pops an event from the region.
        /// </summary>
        /// <returns>One event from the region, or <c>null</c> if no events are in the queue.</returns>
        public Event Extract()
        {
            Event @event;
            do
            {
                if (_events.Count > 0)
                    @event = _events.Dequeue();
                else
                    @event = null;
            }
            while (@event != null && @event.Descheduled);

            // This should be an event that is not descheduled, or null
            return @event;
        }

        /// <summary>
        /// Adds an event to the event region.
        /// </summary>
        /// <param name="event">The event to be added.</param>
        public void Add(Event @event) => _events.Enqueue(@event);
    }
}
