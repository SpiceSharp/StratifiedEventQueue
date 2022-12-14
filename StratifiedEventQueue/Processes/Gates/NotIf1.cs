using StratifiedEventQueue.Processes.Gates;
using StratifiedEventQueue.States.Nets;
using System;

namespace StratifiedEventQueue.States.Gates
{
    /// <summary>
    /// An notif1 gate.
    /// </summary>
    public class NotIf1 : Gate
    {
        /// <summary>
        /// Gets the data input of the NotIf1-gate.
        /// </summary>
        public IState<Signal> Data { get; }

        /// <summary>
        /// Gets the control input of the NotIf1-gate.
        /// </summary>
        public IState<Signal> Control { get; }

        /// <summary>
        /// Creates a new <see cref="NotIf1"/> gate.
        /// </summary>
        /// <param name="name">The name of the gate itself.</param>
        /// <param name="output">The output driver.</param>
        /// <param name="risingDelay">The delay for signals going to the high state.</param>
        /// <param name="fallingDelay">The delay for signals going to the low state.</param>
        /// <param name="turnOffDelay">The delay for signals going to the high-Z state.</param>
        /// <param name="strength0">The strength of low signals.</param>
        /// <param name="strength1">the strength for high signals.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="data"/>, <paramref name="control"/>, <paramref name="name"/> or <paramref name="outputName"/> is <c>null</c>.</exception>
        public NotIf1(string name, Driver output, IState<Signal> data, IState<Signal> control,
            Func<uint> risingDelay = null, Func<uint> fallingDelay = null, Func<uint> turnOffDelay = null,
            Strength strength0 = Strength.St0, Strength strength1 = Strength.St1)
            : base(name, output, risingDelay, fallingDelay, turnOffDelay, strength0, strength1)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Control = control ?? throw new ArgumentNullException(nameof(control));
            Data.Changed += Trigger;
            Control.Changed += Trigger;
        }

        /// <inheritdoc />
        protected override DriveStrengthRange DetermineStrength(Signal signal)
        {
            switch (signal)
            {
                case Signal.H: return new DriveStrengthRange(Strength1);
                case Signal.L: return new DriveStrengthRange(Strength0);
                case Signal.Z: return new DriveStrengthRange(Strength.None);
                default:
                    if (Control.Value == Signal.H)
                        // Inverter mode, data is X
                        return new DriveStrengthRange(Strength0, Strength1);
                    else
                    {
                        switch (Data.Value)
                        {
                            case Signal.L: return new DriveStrengthRange(Strength.None, Strength1);
                            case Signal.H: return new DriveStrengthRange(Strength0, Strength.None);
                            default: return new DriveStrengthRange(Strength0, Strength1);
                        }
                    }
            }
        }

        /// <inheritdoc />
        protected override Signal Compute()
        {
            switch (Control.Value)
            {
                case Signal.H:
                    switch (Data.Value)
                    {
                        case Signal.L: return Signal.H;
                        case Signal.H: return Signal.L;
                        default: return Signal.X;
                    }

                case Signal.L:
                    return Signal.Z;

                default:
                    return Signal.X;
            }
        }

        /// <summary>
        /// Converts the gate to a string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString() => $"notif1 {Name}";
    }
}
