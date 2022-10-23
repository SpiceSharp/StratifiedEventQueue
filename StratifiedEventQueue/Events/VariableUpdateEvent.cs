using System;
using System.Collections.Concurrent;
using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States;

namespace StratifiedEventQueue.Events
{
    /// <summary>
    /// An event that describes an assignment to a variable when executed.
    /// </summary>
    /// <typeparam name="T">The value type of the variable.</typeparam>
    public class VariableUpdateEvent<T> : Event
    {
        private static readonly ConcurrentQueue<VariableUpdateEvent<T>> _pool
            = new ConcurrentQueue<VariableUpdateEvent<T>>();

        /// <summary>
        /// Gets the variable that needs to be assigned.
        /// </summary>
        public Variable<T> Variable { get; private set; }

        /// <summary>
        /// Gets the value that will be assigned.
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// Creates a new <see cref="VariableUpdateEvent{T}"/>.
        /// </summary>
        private VariableUpdateEvent()
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
        /// Creates a new <see cref="VariableUpdateEvent{T}"/>, but uses a pool of objects.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static VariableUpdateEvent<T> Create(Variable<T> variable, T value)
        {
            if (!_pool.TryDequeue(out var result))
                result = new VariableUpdateEvent<T>();
            result.Variable = variable ?? throw new ArgumentNullException(nameof(variable));
            result.Value = value;
            return result;
        }
    }
}
