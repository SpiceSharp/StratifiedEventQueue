using System;
using System.Collections.Generic;
using StratifiedEventQueue.Simulation;

namespace StratifiedEventQueue.States
{
    /// <summary>
    /// Event arguments for a variable that changed.
    /// </summary>
    public class VariableValueChangedEventArgs<T> : EventArgs
    {
        private static readonly Queue<VariableValueChangedEventArgs<T>> _pool = new Queue<VariableValueChangedEventArgs<T>>(InitialPoolSize);

        /// <summary>
        /// The initial pool size.
        /// </summary>
        public const int InitialPoolSize = 20;

        /// <summary>
        /// Gets the scheduler the variable is associated with.
        /// </summary>
        /// <remarks>
        /// Making this settable allows variables to only create a single class.
        /// </remarks>
        public IScheduler Scheduler { get; private set; }

        /// <summary>
        /// Gets the variable that has changed.
        /// </summary>
        public Variable<T> Variable { get; private set; }

        /// <summary>
        /// Creates a new <see cref="VariableValueChangedEventArgs{T}"/>.
        /// </summary>
        /// <param name="variable">The variable.</param>
        private VariableValueChangedEventArgs()
        {
        }

        /// <summary>
        /// Releases the event arguments, allowing it to be reused at a later time.
        /// </summary>
        public void Release()
        {
            _pool.Enqueue(this);
        }

        /// <summary>
        /// Creates a new <see cref="VariableValueChangedEventArgs{T}"/>, trying to use a pool of reusable
        /// objects.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="variable">The variable.</param>
        /// <returns>The event arguments.</returns>
        public static VariableValueChangedEventArgs<T> Create(IScheduler scheduler, Variable<T> variable)
        {
            VariableValueChangedEventArgs<T> result;
            if (_pool.Count > 0)
                result = _pool.Dequeue();
            else
                result = new VariableValueChangedEventArgs<T>();
            result.Variable = variable;
            result.Scheduler = scheduler;
            return result;
        }
    }
}
