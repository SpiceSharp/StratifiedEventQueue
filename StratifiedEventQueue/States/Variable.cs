using System;
using System.Collections.Generic;
using StratifiedEventQueue.Simulation;

namespace StratifiedEventQueue.States
{
    /// <summary>
    /// A variable that can be changed and emits events when it does.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Variable<T> : State<T>
    {
        /// <summary>
        /// Gets the comparer for the variable.
        /// </summary>
        public IEqualityComparer<T> Comparer { get; }

        /// <summary>
        /// Creates a new <see cref="Variable{T}"/>.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is <c>null</c>.</exception>
        public Variable(string name, IEqualityComparer<T> comparer = null)
            : base(name)
        {
            Comparer = comparer ?? EqualityComparer<T>.Default;
        }

        /// <summary>
        /// Updates the variable.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="value">The value.</param>
        public new void Update(IScheduler scheduler, T value)
        {
            if (Comparer.Equals(Value, value))
                return; // No change

            base.Update(scheduler, value);
        }
    }
}
