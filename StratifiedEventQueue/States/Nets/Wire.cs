﻿using System.Collections.Generic;

namespace StratifiedEventQueue.States.Nets
{
    /// <summary>
    /// A wire or tri that performs wire logic in case of conflicting drivers.
    /// </summary>
    public class Wire : WiredNet
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
        protected override DriveStrengthRange Combine(IReadOnlyList<DriveStrengthRange> inputs)
        {
            if (inputs.Count == 0)
                return default;

            // Compute the combined result using wire logic
            var result = inputs[0];
            for (int i = 1; i < inputs.Count; i++)
                result = DriveStrengthRange.Wired(result, inputs[i]);
            return result;
        }
    }
}
