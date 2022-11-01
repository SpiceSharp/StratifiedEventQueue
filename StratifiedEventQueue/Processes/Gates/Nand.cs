using StratifiedEventQueue.States;
using StratifiedEventQueue.States.Nets;
using System;

namespace StratifiedEventQueue.Processes.Gates
{
    /// <summary>
    /// An Nand gate.
    /// </summary>
    public class Nand : Gate
    {
        /// <summary>
        /// Gets the first input of the Nand-gate.
        /// </summary>
        public IState<Signal> A { get; }

        /// <summary>
        /// Gets the second input of the Nand-gate.
        /// </summary>
        public IState<Signal> B { get; }

        /// <summary>
        /// Creates a new <see cref="Nand"/> gate.
        /// </summary>
        /// <param name="name">The name of the gate.</param>
        /// <param name="a">The first input.</param>
        /// <param name="b">The second input.</param>
        /// <param name="output">The output driver.</param>
        /// <param name="risingDelay">The delay for signals going to the high state.</param>
        /// <param name="fallingDelay">The delay for signals going to the low state.</param>
        /// <param name="strength0">The strength of low signals.</param>
        /// <param name="strength1">the strength for high signals.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="a"/>, <paramref name="b"/>, <paramref name="name"/> or <paramref name="output"/> is <c>null</c>.</exception>
        public Nand(string name, Driver output, IState<Signal> a, IState<Signal> b,
            Func<uint> risingDelay = null, Func<uint> fallingDelay = null,
            Strength strength0 = Strength.St0, Strength strength1 = Strength.St1)
            : base(name, output, risingDelay, fallingDelay, null, strength0, strength1)
        {
            A = a ?? throw new ArgumentNullException(nameof(a));
            B = b ?? throw new ArgumentNullException(nameof(b));
            A.Changed += Trigger;
            B.Changed += Trigger;
        }

        /// <inheritdoc />
        protected override Signal Compute()
            => LogicHelper.Nand(A.Value, B.Value);

        /// <summary>
        /// Converts the gate to a string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString() => $"nand {Name}";
    }
}
