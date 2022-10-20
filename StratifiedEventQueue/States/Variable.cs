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
        /// Creates a new <see cref="Variable{T}"/>.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="initialValue">The initial value.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is <c>null</c>.</exception>
        public Variable(string name, T initialValue = default, IEqualityComparer<T> comparer = null)
            : base(name, initialValue, comparer)
        {
        }

        /// <summary>
        /// Updates the variable.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="value">The value.</param>
        public void Update(IScheduler scheduler, T value)
            => Change(scheduler, value);
    }
}
