using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States;
using System;
using System.Collections.Generic;

namespace StratifiedEventQueue.Events
{
    /// <summary>
    /// An event that describes an assignment to a variable when executed. The
    /// value is calculated at the moment of execution.
    /// </summary>
    /// <typeparam name="T">The value type of the variable.</typeparam>
    public class DeferredAssignmentEvent<T> : Event where T : IEquatable<T>
    {
        private static readonly Queue<DeferredAssignmentEvent<T>> _eventPool = new Queue<DeferredAssignmentEvent<T>>();

        /// <summary>
        /// Gets the variable that needs to be assigned.
        /// </summary>
        public Variable<T> Variable { get; private set; }

        /// <summary>
        /// Gets the function that is used to evaluate the function at the moment of execution.
        /// </summary>
        public Func<T> Func { get; private set; }

        /// <summary>
        /// Creates a new <see cref="DeferredAssignmentEvent{T}"/>.
        /// </summary>
        private DeferredAssignmentEvent()
        {
        }

        /// <inheritdoc />
        public override void Execute(Scheduler scheduler)
        {
            T value = Func();
            Variable.Update(scheduler, value);
        }

        /// <inheritdoc />
        public override void Release()
        {
            // Allow object reuse
            _eventPool.Enqueue(this);
        }

        /// <summary>
        /// Creates a new <see cref="DeferredAssignmentEvent{T}"/>, but uses a pool of objects.
        /// </summary>
        /// <param name="variable">The variable to assign to.</param>
        /// <param name="function">The function</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static DeferredAssignmentEvent<T> Create(Variable<T> variable, Func<T> function)
        {
            DeferredAssignmentEvent<T> @event;
            if (_eventPool.Count > 0)
                @event = _eventPool.Dequeue();
            else
                @event = new DeferredAssignmentEvent<T>();
            @event.Variable = variable ?? throw new ArgumentNullException(nameof(variable));
            @event.Func = function ?? throw new ArgumentNullException(nameof(function));
            return @event;
        }
    }
}
