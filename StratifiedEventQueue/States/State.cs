using StratifiedEventQueue.Simulation;
using System;

namespace StratifiedEventQueue.States
{
    /// <summary>
    /// A state.
    /// </summary>
    /// <typeparam name="T">The base type.</typeparam>
    public abstract class State<T> : IState<T>
    {
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
        protected State(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Called when the value of the variable changes.
        /// </summary>
        /// <param name="args">The argument.</param>
        protected virtual void OnChanged(StateChangedEventArgs<T> args)
            => Changed?.Invoke(this, args);

        /// <summary>
        /// Notifies everyone that the state changes.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="newValue">The new value.</param>
        protected void Update(IScheduler scheduler, T newValue)
        {
            var args = StateChangedEventArgs<T>.Create(scheduler, this, Value);
            Value = newValue;
            OnChanged(args);
            args.Release();
        }

        /// <summary>
        /// Updates the value without notifying anyone.
        /// </summary>
        /// <param name="value">The value.</param>
        protected void Update(T value)
        {
            Value = value;
        }

        /// <summary>
        /// Converts the state to a string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString() => Name;
    }
}
