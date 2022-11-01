using StratifiedEventQueue.States;
using StratifiedEventQueue.States.Nets;
using System;

namespace StratifiedEventQueue.Processes.Gates
{
    /// <summary>
    /// A not gate.
    /// </summary>
    public class Not : Gate
    {
        /// <summary>
        /// Gets the input of the buffer.
        /// </summary>
        public IState<Signal> Input { get; }

        /// <summary>
        /// Creates a new <see cref="Not"/>.
        /// </summary>
        /// <param name="name">The name of the gate.</param>
        /// <param name="input">The input.</param>
        /// <param name="output">The output driver.</param>
        /// <param name="risingDelay">The delay for signals going to the high state.</param>
        /// <param name="fallingDelay">The delay for signals going to the low state.</param>
        /// <param name="strength0">The strength of low signals.</param>
        /// <param name="strength1">the strength for high signals.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="input"/>, <paramref name="name"/> or <paramref name="output"/> is <c>null</c>.</exception>
        public Not(string name, Driver output, IState<Signal> input,
            Func<uint> risingDelay = null, Func<uint> fallingDelay = null,
            Strength strength0 = Strength.St0, Strength strength1 = Strength.St1)
            : base(name, output, risingDelay, fallingDelay, null, strength0, strength1)
        {
            Input = input ?? throw new ArgumentNullException(nameof(input));
            Input.Changed += Trigger;
        }

        /// <inheritdoc />
        protected override Signal Compute()
        {
            switch (Input.Value)
            {
                case Signal.L: return Signal.H;
                case Signal.H: return Signal.L;
                default: return Signal.X;
            }
        }

        /// <summary>
        /// Converts the gate to a string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString() => $"not {Name}";
    }
}
