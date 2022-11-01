using StratifiedEventQueue.States.Nets;
using System;

namespace StratifiedEventQueue.Processes.Gates
{
    /// <summary>
    /// Describes a gate that can only result in 3 states (high, low and unknown).
    /// </summary>
    /// <remarks>
    /// A gate is very much like a <see cref="ContinuousAssignment"/>, except that
    /// the delay specification is different.
    /// </remarks>
    public abstract class Gate : ContinuousAssignment
    {
        /// <summary>
        /// Gets the name of the gate.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Creates a new <see cref="Gate"/>.
        /// </summary>
        /// <param name="name">The name of the gate itself.</param>
        /// <param name="output">The output driver.</param>
        /// <param name="risingDelay">The delay for signals going to the high state.</param>
        /// <param name="fallingDelay">The delay for signals going to the low state.</param>
        /// <param name="turnOffDelay">The delay for signals going to the high-Z state.</param>
        /// <param name="strength0">The strength of low signals.</param>
        /// <param name="strength1">the strength for high signals.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or <paramref name="output"/> is <c>null</c>.</exception>
        protected Gate(string name, Driver output,
            Func<uint> risingDelay, Func<uint> fallingDelay, Func<uint> turnOffDelay,
            Strength strength0 = Strength.St0, Strength strength1 = Strength.St1)
            : base(output, risingDelay, fallingDelay, turnOffDelay, strength0, strength1)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
