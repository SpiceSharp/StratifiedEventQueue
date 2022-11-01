using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States;
using System.Collections.Concurrent;

namespace StratifiedEventQueue.Events
{
    /// <summary>
    /// An event that simply updates a variable.
    /// </summary>
    /// <typeparam name="T">The variable type.</typeparam>
    public class UpdateEvent<T> : Event
    {
        private static readonly ConcurrentQueue<UpdateEvent<T>> _pool = new ConcurrentQueue<UpdateEvent<T>>();

        /// <summary>
        /// Gets the variable to update.
        /// </summary>
        public Variable<T> Variable { get; private set; }

        /// <summary>
        /// Gets the value to apply to the variable.
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// Creates a new <see cref="UpdateEvent"/>.
        /// </summary>
        private UpdateEvent()
        {
        }

        /// <inheritdoc />
        public override void Execute(IScheduler scheduler)
        {
            Variable.Update(scheduler, Value);
            _pool.Enqueue(this);
        }

        /// <summary>
        /// Creates a new update event.
        /// </summary>
        /// <param name="variable">The variable.</param>
        /// <param name="value">The value.</param>
        /// <returns>The event.</returns>
        public static UpdateEvent<T> Create(Variable<T> variable, T value)
        {
            _pool.TryDequeue(out var result);
            if (result == null)
                result = new UpdateEvent<T>();
            result.Variable = variable;
            result.Value = value;
            return result;
        }
    }
}
