using System.Collections.Generic;

namespace StratifiedEventQueue.States.Nets
{
    /// <summary>
    /// A wire or tri that performs wire logic in case of conflicting drivers.
    /// </summary>
    public class Wire : Net
    {
        /// <summary>
        /// Creates a new <see cref="Wire"/>.
        /// </summary>
        /// <param name="name">The name of the wire.</param>
        public Wire(string name)
            : base(name)
        {
        }

        /// <inheritdoc />
        protected override DriveStrengthRange Combine(DriveStrengthRange a, DriveStrengthRange b) => DriveStrengthRange.Wired(a, b);

        /// <summary>
        /// Converts the wire to a string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString() => $"wire {Name}";
    }
}
