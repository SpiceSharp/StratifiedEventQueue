using System;
using System.Collections.Generic;

namespace StratifiedEventQueue.States
{
    /// <summary>
    /// Describes the drive strenghts that a driver can pass to a <see cref="Net"/>.
    /// </summary>
    public enum DriveStrength : sbyte
    {
        Supply0 = -8,
        Strong0 = -7,
        Pull0 = -6,
        Large0 = -5,
        Weak0 = -4,
        Medium0 = -3,
        Small0 = -2,
        HighZ0 = -1,
        HighZ1 = 1,
        Small1 = 2,
        Medium1 = 3,
        Weak1 = 4,
        Large1 = 5,
        Pull1 = 6,
        Strong1 = 7,
        Supply1 = 8
    }

    /// <summary>
    /// An entity that is driving a <see cref="Net"/>.
    /// </summary>
    public abstract class NetDriver
    {
        /// <summary>
        /// Occurs when the drive strength of the driver changes.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<DriveStrength>> StrengthChanged;

        /// <summary>
        /// Gets the old strength of the net driver.
        /// </summary>
        public DriveStrength OldStrength { get; protected set; }

        /// <summary>
        /// Gets the strength of the net driver.
        /// </summary>
        public DriveStrength Strength { get; protected set; }

        /// <summary>
        /// Gets the time when the drive strength changed.
        /// </summary>
        public ulong ChangeTime { get; protected set; }

        /// <summary>
        /// Called when the drive strength changes.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        protected virtual void OnStrengthChanged(ValueChangedEventArgs<DriveStrength> args)
        {
            StrengthChanged?.Invoke(this, args);
        }
    }

    /// <summary>
    /// A net that can be driven from multiple sources.
    /// </summary>
    public abstract class Net : NetDriver
    {
        private readonly Dictionary<object, DriveStrength> _drivers = new Dictionary<object, DriveStrength>();

        /// <summary>
        /// Occurs when the result of the net changes.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<byte>> Changed;

        /// <summary>
        /// Gets the name of the variable.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the previous (old) value of the net.
        /// </summary>
        public byte OldValue { get; protected set; }

        /// <summary>
        /// Gets the current value of the net.
        /// </summary>
        public byte Value { get; protected set; }

        /// <summary>
        /// Creates a new <see cref="Net"/>.
        /// </summary>
        /// <param name="name">The name of the net.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is <c>null</c>.</exception>
        protected Net(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            OldValue = Logic.Z;
            Value = Logic.Z;
        }

        protected abstract void OnStrengthChanged(object sender, ValueChangedEventArgs<DriveStrength> args);
    }
}
