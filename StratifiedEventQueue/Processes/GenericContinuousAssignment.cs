using StratifiedEventQueue.States;
using StratifiedEventQueue.States.Nets;
using System;

namespace StratifiedEventQueue.Processes
{
    /// <summary>
    /// A generic implementation of <see cref="ContinuousAssignment"/> that
    /// allows specifying the right-hand side using a <see cref="Func{TResult}"/>.
    /// </summary>
    public class GenericContinuousAssignment : ContinuousAssignment
    {
        /// <summary>
        /// Gets the function used to evaluate the right-hand side.
        /// </summary>
        public Func<Signal> RightHandSide { get; }

        /// <summary>
        /// Creates a  new <see cref="GenericContinuousAssignment"/>.
        /// </summary>
        /// <param name="output">The output driver.</param>
        /// <param name="rhs">The right-hand side of the assignment.</param>
        /// <param name="fallingDelay">The falling delay, or <c>null</c>.</param>
        /// <param name="turnOffDelay">The turn-off delay, or <c>null</c>.</param>
        /// <param name="risingDelay">The rising delay, or <c>null</c>.</param>
        /// <param name="strength0">The strength for low signals.</param>
        /// <param name="strength1">The strength for high signals.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="output"/> or <paramref name="rhs"/> is <c>null</c>.</exception>
        public GenericContinuousAssignment(Driver output, Func<Signal> rhs,
            Func<uint> fallingDelay = null, Func<uint> turnOffDelay = null, Func<uint> risingDelay = null,
            Strength strength0 = Strength.St0, Strength strength1 = Strength.St1)
            : base(output, fallingDelay, turnOffDelay, risingDelay, strength0, strength1)
        {
            RightHandSide = rhs ?? throw new ArgumentNullException(nameof(rhs));
        }

        /// <inheritdoc />
        protected override Signal Compute() => RightHandSide();
    }
}
