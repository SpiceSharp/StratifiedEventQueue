﻿using System;

namespace StratifiedEventQueue.States.Gates
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
        /// <param name="riseDelay">The rise delay.</param>
        /// <param name="fallDelay">The fall delay.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="a"/>, <paramref name="b"/>, <paramref name="name"/> or <paramref name="outputName"/> is <c>null</c>.</exception>
        public Nand(string name, string outputName, IState<Signal> a, IState<Signal> b,
            ulong riseDelay = 0, ulong fallDelay = 0)
            : base(name, outputName, riseDelay, fallDelay)
        {
            A = a ?? throw new ArgumentNullException(nameof(a));
            B = b ?? throw new ArgumentNullException(nameof(b));
            Value = ComputeSignal();
            if (riseDelay == 0 && fallDelay == 0)
            {
                A.Changed += UpdateZeroDelay;
                B.Changed += UpdateZeroDelay;
            }
            else
            {
                A.Changed += Update;
                B.Changed += Update;
            }
        }

        /// <inheritdoc />
        protected override Signal ComputeSignal()
            => LogicHelper.Nand(A.Value, B.Value);

        /// <summary>
        /// Converts the gate to a string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString() => $"nand {Name}";
    }
}