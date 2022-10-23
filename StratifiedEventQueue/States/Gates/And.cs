using System;

namespace StratifiedEventQueue.States.Gates
{
    /// <summary>
    /// An AND gate.
    /// </summary>
    public class And : Gate
    {
        /// <summary>
        /// Gets the first input of the and-gate.
        /// </summary>
        public IState<Signal> A { get; }

        /// <summary>
        /// Gets the second input of the and-gate.
        /// </summary>
        public IState<Signal> B { get; }

        /// <summary>
        /// Creates a new <see cref="And"/> gate.
        /// </summary>
        /// <param name="name">The name of the gate.</param>
        /// <param name="a">The first input.</param>
        /// <param name="b">The second input.</param>
        /// <param name="riseDelay">The rise delay.</param>
        /// <param name="fallDelay">The fall delay.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="a"/>, <paramref name="b"/>, <paramref name="name"/> or <paramref name="outputName"/> is <c>null</c>.</exception>
        public And(string name, string outputName, IState<Signal> a, IState<Signal> b,
            ulong riseDelay = 0, ulong fallDelay = 0)
            : base(name, outputName, riseDelay, fallDelay)
        {
            A = a ?? throw new ArgumentNullException(nameof(a));
            B = b ?? throw new ArgumentNullException(nameof(b));
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
            => LogicHelper.And(A.Value, B.Value);
    }
}
