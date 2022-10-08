using System;
using System.Collections.Generic;
using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States;

namespace StratifiedEventQueue.Events
{
    /// <summary>
    /// An event that describes an assignment to a variable when executed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AssignmentEvent<T> : Event where T : IEquatable<T>
    {
        private static readonly Queue<AssignmentEvent<T>> _eventPool = new Queue<AssignmentEvent<T>>(20);

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
        public override void Execute(Scheduler scheduler)
        {
            Variable.Update(scheduler, Value);

            // Allow object reuse
            _eventPool.Enqueue(this);
        }

        /// <summary>
        /// Creates a new <see cref="AssignmentEvent{T}"/>, but uses a pool of objects.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static AssignmentEvent<T> Create(Variable<T> variable, T value)
        {
            AssignmentEvent<T> @event;
            if (_eventPool.Count > 0)
                @event = _eventPool.Dequeue();
            else
                @event = new AssignmentEvent<T>();
            @event.Variable = variable ?? throw new ArgumentNullException(nameof(variable));
            @event.Value = value;
            return @event;
        }
    }
}
