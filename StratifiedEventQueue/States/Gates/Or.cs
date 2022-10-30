using System;

namespace StratifiedEventQueue.States.Gates
{
    /// <summary>
    /// An Or gate.
    /// </summary>
    public class Or : Gate
    {
        /// <summary>
        /// Gets the first input of the Or-gate.
        /// </summary>
        public IState<Signal> A { get; }

        /// <summary>
        /// Gets the second input of the Or-gate.
        /// </summary>
        public IState<Signal> B { get; }

        /// <summary>
        /// Creates a new <see cref="Or"/> gate.
        /// </summary>
        /// <param name="name">The name of the gate.</param>
        /// <param name="a">The first input.</param>
        /// <param name="b">The second input.</param>
        /// <param name="riseDelay">The rise delay.</param>
        /// <param name="fallDelay">The fall delay.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="a"/>, <paramref name="b"/>, <paramref name="name"/> or <paramref name="outputName"/> is <c>null</c>.</exception>
        public Or(string name, string outputName, IState<Signal> a, IState<Signal> b,
            Func<uint> riseDelay = null, Func<uint> fallDelay = null)
            : base(name, outputName, riseDelay, fallDelay)
        {
            A = a ?? throw new ArgumentNullException(nameof(a));
            B = b ?? throw new ArgumentNullException(nameof(b));
            if (riseDelay == null && fallDelay == null)
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
            => LogicHelper.Or(A.Value, B.Value);

        /// <summary>
        /// Converts the gate to a string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString() => $"or {Name}";
    }
}
