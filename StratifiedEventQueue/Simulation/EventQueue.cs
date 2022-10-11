using System.Collections.Generic;

namespace StratifiedEventQueue.Simulation
{
    /// <summary>
    /// Describes the stratified event queue.
    /// </summary>
    public class EventQueue
    {
        private static readonly Queue<EventQueue> _pool = new Queue<EventQueue>();

        /// <summary>
        /// All inactive events that will be activated once all active events have finished.
        /// </summary>
        public Queue<EventNode> Inactive { get; }

        /// <summary>
        /// All non-blocking events that will be activated once all active and inactive events have finished.
        /// </summary>
        public Queue<EventNode> NonBlocking { get; }

        /// <summary>
        /// Creates a new <see cref="EventQueue"/>.
        /// </summary>
        private EventQueue()
        {
            Inactive = new Queue<EventNode>();
            NonBlocking = new Queue<EventNode>();
        }

        /// <summary>
        /// Releases the event queue for object reuse.
        /// </summary>
        public void Release()
        {
            Inactive.Clear();
            NonBlocking.Clear();
            _pool.Enqueue(this);
        }

        /// <summary>
        /// Creates a new event queue, possibly reusing objects.
        /// </summary>
        /// <returns>The event queue.</returns>
        public static EventQueue Create()
            => _pool.Count > 0 ? _pool.Dequeue() : new EventQueue();
    }
}
