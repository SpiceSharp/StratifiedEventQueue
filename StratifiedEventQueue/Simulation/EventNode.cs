using StratifiedEventQueue.Events;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace StratifiedEventQueue.Simulation
{
    /// <summary>
    /// A class that accompagnies an event such that it can be descheduled.
    /// </summary>
    public class EventNode
    {
        private static readonly ConcurrentQueue<EventNode> _pool = new ConcurrentQueue<EventNode>();

        /// <summary>
        /// Gets the event that needs to be executed.
        /// </summary>
        public Event Event { get; private set; }

        /// <inheritdoc />
        public bool IsScheduled { get; private set; }

        /// <summary>
        /// Creates a new <see cref="EventNode"/>.
        /// </summary>
        private EventNode()
        {
        }

        /// <summary>
        /// Deschedules the event from the scheduler. Make sure you only call this on events that have
        /// not been executed yet!
        /// </summary>
        public void Deschedule() => IsScheduled = false;

        /// <summary>
        /// Releases the node such that it can be reused. This should only be called by the scheduler.
        /// </summary>
        public void Release()
        {
            _pool.Enqueue(this);
        }

        /// <summary>
        /// Creates a new event node.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <returns>The event node.</returns>
        public static EventNode Create(Event @event)
        {
            if (!_pool.TryDequeue(out var result))
                result = new EventNode();
            result.IsScheduled = true;
            result.Event = @event;
            return result;
        }
    }
}
