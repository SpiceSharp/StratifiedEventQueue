using System;

namespace StratifiedEventQueue.Simulation
{
    /// <summary>
    /// A variable that can be changed and emits events when it does.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Variable<T> where T : IEquatable<T>
    {
        /// <summary>
        /// Occurs when the value of the variable changed.
        /// </summary>
        public event EventHandler<VariableValueChangedEventArgs<T>> Changed;

        /// <summary>
        /// Gets the name of the variable.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the previous (old) value of the variable.
        /// </summary>
        public T OldValue { get; private set; }

        /// <summary>
        /// Gets the current value of the variable.
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// Creates a new <see cref="Variable{T}"/>.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="initialValue">The initial value.</param>
        public Variable(string name, T initialValue = default)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            OldValue = initialValue;
            Value = initialValue;
        }

        /// <summary>
        /// Updates the variable.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="value">The value.</param>
        public void Update(Scheduler scheduler, T value)
        {
            if (Value.Equals(value))
                return; // No change

            // Update the variable
            OldValue = Value;
            Value = value;
            OnChanged(VariableValueChangedEventArgs<T>.Create(scheduler, this));
        }

        /// <summary>
        /// Called when the value of the variable changes.
        /// </summary>
        /// <param name="args">The argument.</param>
        protected virtual void OnChanged(VariableValueChangedEventArgs<T> args)
        {
            Changed?.Invoke(this, args);
        }

        /// <summary>
        /// Converts the variable to a string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString() => Name;
    }
}
