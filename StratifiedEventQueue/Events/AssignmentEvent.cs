using System;
using System.Collections.Generic;
using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States;

namespace StratifiedEventQueue.Events
{
    /// <summary>
    /// An event that describes an assignment to a variable when executed.
    /// </summary>
    /// <typeparam name="T">The value type of the variable.</typeparam>
    public class AssignmentEvent<T> : Event
    {
        private static readonly System.Collections.Generic.Queue<AssignmentEvent<T>> _pool
            = new System.Collections.Generic.Queue<AssignmentEvent<T>>();

        /// <summary>
        /// Gets the variable that needs to be assigned.
        /// </summary>
        public Variable<T> Variable { get; private set; }

        /// <summary>
        /// Gets the value that will be assigned.
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// Creates a new <see cref="AssignmentEvent{T}"/>.
        /// </summary>
        private AssignmentEvent()
        {
        }

        /// <inheritdoc />
        public override void Execute(IScheduler scheduler)
        {
            Variable.Update(scheduler, Value);

            // It is now ok to reuse this event again
            _pool.Enqueue(this);
        }

        /// <summary>
        /// Creates a new <see cref="AssignmentEvent{T}"/>, but uses a pool of objects.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static AssignmentEvent<T> Create(Variable<T> variable, T value)
        {
            AssignmentEvent<T> @event = _pool.Count > 0 ? _pool.Dequeue() : new AssignmentEvent<T>();
            @event.Variable = variable ?? throw new ArgumentNullException(nameof(variable));
            @event.Value = value;
            return @event;
        }
    }
}
