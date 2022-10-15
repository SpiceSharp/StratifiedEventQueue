namespace StratifiedEventQueue
{
    /// <summary>
    /// An enumeration of Logic.Logic values.
    /// </summary>
    public enum Logic : byte
    {
        /// <summary>
        /// Represents an unknown Logic.Logic value.
        /// </summary>
        X = 0,

        /// <summary>
        /// Represents a Logic.Low Logic.Logic value.
        /// </summary>
        L = 1,

        /// <summary>
        /// Represents a high Logic.Logic value.
        /// </summary>
        H = 2,

        /// <summary>
        /// Represents a high-impedance Logic.Logic value.
        /// </summary>
        Z = 3
    }
}
