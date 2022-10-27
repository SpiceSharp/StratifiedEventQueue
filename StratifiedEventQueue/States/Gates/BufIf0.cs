﻿using StratifiedEventQueue.States.Nets;
using System;

namespace StratifiedEventQueue.States.Gates
{
    /// <summary>
    /// An bufif0 gate.
    /// </summary>
    public class BufIf0 : TriGate
    {
        /// <summary>
        /// Gets the data input of the BufIf0-gate.
        /// </summary>
        public IState<Signal> Data { get; }

        /// <summary>
        /// Gets the control input of the BufIf0-gate.
        /// </summary>
        public IState<Signal> Control { get; }

        /// <summary>
        /// Gets the strength for high signals.
        /// </summary>
        public Strength High { get; }

        /// <summary>
        /// Gets the strength for low signals.
        /// </summary>
        public Strength Low { get; }

        /// <summary>
        /// Creates a new <see cref="BufIf0"/> gate.
        /// </summary>
        /// <param name="name">The name of the gate.</param>
        /// <param name="data">The first input.</param>
        /// <param name="control">The second input.</param>
        /// <param name="riseDelay">The rise delay.</param>
        /// <param name="fallDelay">The fall delay.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="data"/>, <paramref name="control"/>, <paramref name="name"/> or <paramref name="outputName"/> is <c>null</c>.</exception>
        public BufIf0(string name, string outputName, IState<Signal> data, IState<Signal> control,
            uint riseDelay = 0, uint fallDelay = 0, Strength low = Strength.St0, Strength high = Strength.St1)
            : base(name, outputName, riseDelay, fallDelay, riseDelay)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Control = control ?? throw new ArgumentNullException(nameof(control));
            Low = low;
            High = high;
            if (riseDelay == 0 && fallDelay == 0)
            {
                Data.Changed += UpdateZeroDelay;
                Control.Changed += UpdateZeroDelay;
            }
            else
            {
                Data.Changed += Update;
                Control.Changed += Update;
            }
        }

        /// <inheritdoc />
        protected override DriveStrengthRange ComputeSignal()
        {
            switch (Control.Value)
            {
                case Signal.L:
                    switch (Data.Value)
                    {
                        case Signal.L: return new DriveStrengthRange(Low, Low);
                        case Signal.H: return new DriveStrengthRange(High, High);
                        default: return new DriveStrengthRange(Low, High);
                    }

                case Signal.H:
                    return new DriveStrengthRange(Strength.None, Strength.None);

                default:
                    switch (Data.Value)
                    {
                        case Signal.L: return new DriveStrengthRange(Low, Strength.None);
                        case Signal.H: return new DriveStrengthRange(Strength.None, High);
                        default: return new DriveStrengthRange(Low, High);
                    }
            }
        }

        /// <summary>
        /// Converts the gate to a string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString() => $"bufif0 {GateName}";
    }
}
