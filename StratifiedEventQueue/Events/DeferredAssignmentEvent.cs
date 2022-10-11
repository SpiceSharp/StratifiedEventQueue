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
    public class DeferredAssignmentEvent<T> : Event
    {
        private static readonly Queue<DeferredAssignmentEvent<T>> _pool = new Queue<DeferredAssignmentEvent<T>>();

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
        public override void Execute(IScheduler scheduler)
        {
            T value = Func();
            Variable.Update(scheduler, value);

            // It is now ok to reuse this event
            _pool.Enqueue(this);
        }

        /// <summary>
        /// Creates a new <see cref="DeferredAssignmentEvent{T}"/>, but uses a pool of objects.
        /// </summary>
        /// <param name="variable">The variable to assign to.</param>
        /// <param name="function">The function</param>
        /// <returns>The event.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any parameter is <c>null</c>.</exception>
        public static DeferredAssignmentEvent<T> Create(Variable<T> variable, Func<T> function)
        {
            DeferredAssignmentEvent<T> result = _pool.Count > 0 ? _pool.Dequeue() : new DeferredAssignmentEvent<T>();
            result.Variable = variable ?? throw new ArgumentNullException(nameof(variable));
            result.Func = function ?? throw new ArgumentNullException(nameof(function));
            return result;
        }
    }
}
