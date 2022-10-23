using System;
using System.Collections.Concurrent;
using StratifiedEventQueue.Simulation;

namespace StratifiedEventQueue.States
{
    /// <summary>
    /// Event arguments for a variable that changed.
    /// </summary>
    public class StateChangedEventArgs<T> : EventArgs
    {
        private static readonly ConcurrentQueue<StateChangedEventArgs<T>> _pool 
            = new ConcurrentQueue<StateChangedEventArgs<T>>();

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
        public IState<T> State { get; private set; }

        /// <summary>
        /// Gets the old (previous) value of the state.
        /// </summary>
        public T OldValue { get; private set; }

        /// <summary>
        /// Creates a new <see cref="StateChangedEventArgs{T}"/>.
        /// </summary>
        /// <param name="variable">The variable.</param>
        private StateChangedEventArgs()
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
        /// Creates a new <see cref="StateChangedEventArgs{T}"/>, trying to use a pool of reusable
        /// objects.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="state">The state.</param>
        /// <returns>The event arguments.</returns>
        public static StateChangedEventArgs<T> Create(IScheduler scheduler, IState<T> state, T oldValue)
        {
            _pool.TryDequeue(out var result);
            if (result == null)
                result = new StateChangedEventArgs<T>();
            result.State = state;
            result.Scheduler = scheduler;
            result.OldValue = oldValue;
            return result;
        }
    }
}
