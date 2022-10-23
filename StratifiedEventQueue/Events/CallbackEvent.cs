using StratifiedEventQueue.Simulation;
using System;
using System.Collections.Concurrent;

namespace StratifiedEventQueue.Events
{
    /// <summary>
    /// An event that simply calls a method when executed.
    /// </summary>
    public class CallbackEvent : Event
    {
        private readonly static ConcurrentQueue<CallbackEvent> _pool = new ConcurrentQueue<CallbackEvent>();

        /// <summary>
        /// Gets the action that will be executed for the event.
        /// </summary>
        public Action<IScheduler> Action { get; private set; }

        /// <summary>
        /// Creates a new <see cref="CallbackEvent"/>.
        /// </summary>
        private CallbackEvent()
        {
        }

        /// <inheritdoc />
        public override void Execute(IScheduler scheduler)
        {
            Action.Invoke(scheduler);

            // It is now ok to reuse this event again
            _pool.Enqueue(this);
        }

        /// <summary>
        /// Creates a new <see cref="CallbackEvent"/>.
        /// </summary>
        /// <param name="action">The action that should be called when the event is executed.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is <c>null</c>.</exception>
        public static CallbackEvent Create(Action<IScheduler> action)
        {
            if (!_pool.TryDequeue(out var result))
                result = new CallbackEvent();
            result.Action = action ?? throw new ArgumentNullException(nameof(action));
            return result;
        }
    }
}
