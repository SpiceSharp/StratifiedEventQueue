using System;

namespace StratifiedEventQueue.States.Gates
{
    /// <summary>
    /// A buffer.
    /// </summary>
    public class Buf : Gate
    {
        /// <summary>
        /// Gets the input of the buffer.
        /// </summary>
        public IState<Signal> Input { get; }

        /// <summary>
        /// Creates a new <see cref="Buf"/>.
        /// </summary>
        /// <param name="name">The name of the gate.</param>
        /// <param name="outputName">The name of the state.</param>
        /// <param name="input">The input.</param>
        /// <param name="riseDelay">The delay for high states.</param>
        /// <param name="fallDelay">The delay for low states.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>, <paramref name="outputName"/> or <paramref name="input"/> is <c>null</c>.</exception>
        public Buf(string name, string outputName, IState<Signal> input,
            uint riseDelay = 0, uint fallDelay = 0)
            : base(name, outputName, riseDelay, fallDelay)
        {
            Input = input ?? throw new ArgumentNullException(nameof(input));
            if (riseDelay == 0 && fallDelay == 0)
                input.Changed += UpdateZeroDelay;
            else
                input.Changed += Update;
        }

        /// <inheritdoc />
        protected override Signal ComputeSignal()
        {
            switch (Input.Value)
            {
                case Signal.L: return Signal.L;
                case Signal.H: return Signal.H;
                default: return Signal.X;
            }
        }

        /// <summary>
        /// Converts the gate to a string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString() => $"buf {Name}";
    }
}
