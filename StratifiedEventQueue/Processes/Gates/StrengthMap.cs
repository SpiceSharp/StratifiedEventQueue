using StratifiedEventQueue.States;
using StratifiedEventQueue.States.Nets;

namespace StratifiedEventQueue.Gates
{
    /// <summary>
    /// Defines a map adding a strength to high/low logic levels.
    /// </summary>
    public class StrengthMap : Map<Signal, DriveStrengthRange>
    {
        /// <summary>
        /// Gets the strength for high values.
        /// </summary>
        public Strength High { get; }

        /// <summary>
        /// Gets the strength for low values.
        /// </summary>
        public Strength Low { get; }

        /// <summary>
        /// Creates a new <see cref="StrengthMap"/>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="input">The input.</param>
        /// <param name="high">The high strength.</param>
        /// <param name="low">The low strength.</param>
        public StrengthMap(string name, IState<Signal> input, Strength high = Strength.St1, Strength low = Strength.St0)
            : base(name, input)
        {
            High = high;
            Low = low;
        }

        /// <inheritdoc />
        protected override DriveStrengthRange MapInput(Signal oldInput, Signal newInput)
        {
            switch (newInput)
            {
                case Signal.X: return new DriveStrengthRange(Low, High);
                case Signal.L: return new DriveStrengthRange(Low, Low);
                case Signal.H: return new DriveStrengthRange(High, High);
                default: return default;
            }
        }
    }
}
