using StratifiedEventQueue.Simulation;
using System;

namespace StratifiedEventQueue.Events
{
    /// <summary>
    /// An event that simply calls a method when executed.
    /// </summary>
    public class CallbackEvent : Event
    {
        private readonly Action<Scheduler> _action;

        /// <summary>
        /// Creates a new <see cref="CallbackEvent"/>.
        /// </summary>
        /// <param name="action">The action to be executed.</param>
        public CallbackEvent(Action<Scheduler> action)
        {
            _action = action;
        }

        /// <inheritdoc />
        public override void Execute(Scheduler scheduler)
        {
            _action?.Invoke(scheduler);
        }
    }
}
