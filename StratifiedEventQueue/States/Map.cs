using System;
using System.Collections.Generic;

namespace StratifiedEventQueue.States
{
    /// <summary>
    /// Describes a map that simply maps an input to an output.
    /// </summary>
    /// <typeparam name="TIn">The input.</typeparam>
    /// <typeparam name="TOut">The output.</typeparam>
    public abstract class Map<TIn, TOut> : IState<TOut>
    {
        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public IEqualityComparer<TOut> Comparer => throw new NotImplementedException();

        /// <inheritdoc />
        public TOut Value { get; private set; }

        /// <inheritdoc />
        public event EventHandler<StateChangedEventArgs<TOut>> Changed;

        /// <summary>
        /// Gets the input state.
        /// </summary>
        public IState<TIn> Input { get; }

        /// <summary>
        /// Creates a new <see cref="Map{TIn, TOut}"/>.
        /// </summary>
        /// <param name="name">The name of the map.</param>
        /// <param name="input">The input.</param>
        /// <exception cref="ArgumentNullException">Thrown if any argument is <c>null</c>.</exception>
        public Map(string name, IState<TIn> input)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Input = input ?? throw new ArgumentNullException(nameof(input));

            Value = MapInput(input.Value, input.Value);

            // Register this map with the input
            input.Changed += Update;
        }

        /// <summary>
        /// Updates the map input.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The arguments.</param>
        protected void Update(object sender, StateChangedEventArgs<TIn> args)
        {
            var result = MapInput(args.OldValue, args.State.Value);

            // Nothing changed...
            if (Comparer.Equals(result, Value))
                return;

            var nargs = StateChangedEventArgs<TOut>.Create(args.Scheduler, this, Value);
            Value = result;
            OnChanged(nargs);
            nargs.Release();
        }

        /// <summary>
        /// Maps the input to another value.
        /// </summary>
        /// <param name="input">the input</param>
        /// <returns></returns>
        protected abstract TOut MapInput(TIn oldInput, TIn newInput);

        /// <summary>
        /// Called when the output changed.
        /// </summary>
        /// <param name="args">The argument.</param>
        protected virtual void OnChanged(StateChangedEventArgs<TOut> args)
            => Changed?.Invoke(this, args);

        /// <inheritdoc />
        public override string ToString() => Name;
    }
}
