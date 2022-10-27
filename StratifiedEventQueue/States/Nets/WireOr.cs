using System.Collections.Generic;

namespace StratifiedEventQueue.States.Nets
{
    /// <summary>
    /// A wor or trior net that performs OR logic in case of conflicting drivers.
    /// </summary>
    public class WireOr : WiredNet
    {
        /// <summary>
        /// Creates a new <see cref="Wire"/>.
        /// </summary>
        /// <param name="name">The name of the wire.</param>
        public WireOr(string name)
            : base(name)
        {
        }

        /// <inheritdoc />
        protected override DriveStrengthRange Combine(DriveStrengthRange a, DriveStrengthRange b) => DriveStrengthRange.WiredOr(a, b);

        /// <summary>
        /// Converts the wire to a string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString() => $"wor {Name}";
    }
}
