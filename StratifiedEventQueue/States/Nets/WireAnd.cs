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
        protected override DriveStrengthRange Combine(IReadOnlyList<DriveStrengthRange> inputs)
        {
            if (inputs.Count == 0)
                return default;

            // Compute the combined result using wire AND logic
            var result = inputs[0];
            for (int i = 1; i < inputs.Count; i++)
                result = DriveStrengthRange.WiredAnd(result, inputs[i]);
            return result;
        }
    }
}
