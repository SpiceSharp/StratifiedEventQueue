using StratifiedEventQueue.Simulation;
using System;
using System.Collections.Generic;

namespace StratifiedEventQueue.Events
{
    /// <summary>
    /// An event that simply calls a method when executed.
    /// </summary>
    public class CallbackEvent : Event
    {
        private readonly static Queue<CallbackEvent> _eventPool = new Queue<CallbackEvent>(InitialPoolSize);

        /// <summary>
        /// The initial size of the pool
        /// </summary>
        public const int InitialPoolSize = 20;

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
            Action?.Invoke(scheduler);
        }

        /// <inheritdoc />
        public override void Release()
        {
            _eventPool.Enqueue(this);
        }

        /// <summary>
        /// Creates a new <see cref="CallbackEvent"/>.
        /// </summary>
        /// <param name="action">The action that should be called when the event is executed.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is <c>null</c>.</exception>
        public static CallbackEvent Create(Action<IScheduler> action)
        {
            CallbackEvent result;
            if (_eventPool.Count > 0)
                result = _eventPool.Dequeue();
            else
                result = new CallbackEvent();
            result.Action = action ?? throw new ArgumentNullException(nameof(action));
            result.Descheduled = false;
            return result;
        }
    }
}
