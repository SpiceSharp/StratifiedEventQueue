using System.Collections.Generic;

namespace StratifiedEventQueue.States.Nets
{
    /// <summary>
    /// A wand or triand that performs AND logic in case of conflicting drivers.
    /// </summary>
    public class WireAnd : WiredNet
    {
        /// <summary>
        /// Creates a new <see cref="WireAnd"/>.
        /// </summary>
        /// <param name="name">The name of the wand/triand.</param>
        public WireAnd(string name)
            : base(name)
        {
        }

        /// <inheritdoc />
        protected override DriveStrengthRange Combine(DriveStrengthRange a, DriveStrengthRange b) => DriveStrengthRange.WiredAnd(a, b);

        /// <summary>
        /// Converts the wire to a string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString() => $"wand {Name}";
    }
}
