using System;
using System.Collections.Concurrent;

namespace StratifiedEventQueue.Simulation
{
    /// <summary>
    /// Describes the stratified event queue.
    /// </summary>
    public class EventQueue
    {
        private static readonly ConcurrentQueue<EventQueue> _pool = new ConcurrentQueue<EventQueue>();

        /// <summary>
        /// All inactive events that will be activated once all active events have finished.
        /// </summary>
        public Queue<EventNode> Inactive { get; private set; }

        /// <summary>
        /// All non-blocking events that will be activated once all active and inactive events have finished.
        /// </summary>
        public Queue<EventNode> NonBlocking { get; private set; }

        /// <summary>
        /// Creates a new <see cref="EventQueue"/>.
        /// </summary>
        private EventQueue()
        {
            Inactive = new Queue<EventNode>();
            NonBlocking = new Queue<EventNode>();
        }

        /// <summary>
        /// Swaps a queue with the inactive queue.
        /// </summary>
        /// <param name="queue">The queue to swap.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="queue"/> is <c>null</c>.</exception>
        public void SwapInactive(ref Queue<EventNode> queue)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));
            (queue, Inactive) = (Inactive, queue);
        }

        /// <summary>
        /// Swaps a queue with the nonblocking queue.
        /// </summary>
        /// <param name="queue">The queue to swap.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="queue"/> is <c>null</c>.</exception>
        public void SwapNonBlocking(ref Queue<EventNode> queue)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));
            (queue, NonBlocking) = (NonBlocking, queue);
        }

        /// <summary>
        /// Releases the event queue for object reuse.
        /// </summary>
        public void Release()
        {
            // Due to the way the scheduler works, we don't need to bother with clearing the queues
            _pool.Enqueue(this);
        }

        /// <summary>
        /// Creates a new event queue, possibly reusing objects.
        /// </summary>
        /// <returns>The event queue.</returns>
        public static EventQueue Create()
        {
            if (!_pool.TryDequeue(out var result))
                result = new EventQueue();
            return result;
        }
    }
}
