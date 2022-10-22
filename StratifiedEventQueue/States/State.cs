using StratifiedEventQueue.Simulation;
using System;
using System.Collections.Generic;

namespace StratifiedEventQueue.States
{
    /// <summary>
    /// A basic implementation of <see cref="IState{T}"/>.
    /// </summary>
    /// <remarks>
    /// The state only generates events when the value changes according to the
    /// <see cref="IEqualityComparer{T}"/> passed to the state.
    /// </remarks>
    /// <typeparam name="T">The base type.</typeparam>
    public abstract class State<T> : IState<T>
    {
        /// <inheritdoc />
        public IEqualityComparer<T> Comparer { get; }

        /// <inheritdoc />
        public event EventHandler<StateChangedEventArgs<T>> Changed;

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public T Value { get; private set; }

        /// <summary>
        /// Creates a new <see cref="State{T}"/>.
        /// </summary>
        /// <param name="name">The name of the state.</param>
        /// <param name="initialValue">The initial value of the state.</param>
        /// <param name="comparer">The comparer used for detecting changes.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is <c>null</c>.</exception>
        protected State(string name, T initialValue = default, IEqualityComparer<T> comparer = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Comparer = comparer ?? EqualityComparer<T>.Default;
            Value = initialValue;
        }

        /// <summary>
        /// Changes the value of the state.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void Change(IScheduler scheduler, T newValue)
        {
            if (Comparer.Equals(newValue, Value))
                return; // Nothing changed

            // Update the variable
            var args = StateChangedEventArgs<T>.Create(scheduler, this, Value);
            Value = newValue;
            OnChanged(args);
            args.Release();
        }

        /// <summary>
        /// Called when the value of the variable changes.
        /// </summary>
        /// <param name="args">The argument.</param>
        protected virtual void OnChanged(StateChangedEventArgs<T> args)
            => Changed?.Invoke(this, args);

        /// <summary>
        /// Converts the state to a string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString() => Name;
    }
}
