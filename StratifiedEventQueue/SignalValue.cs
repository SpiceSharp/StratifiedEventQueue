namespace StratifiedEventQueue
{
    /// <summary>
    /// The supported signal values.
    /// </summary>
    public enum SignalValue : byte
    {
        /// <summary>
        /// Represents an unknown state. This is usually the default intial value
        /// so we assign 0 to it.
        /// </summary>
        Unknown = 0x00,

        /// <summary>
        /// Represents a low state.
        /// </summary>
        Low = 0x01,

        /// <summary>
        /// Represents a high state.
        /// </summary>
        High = 0x02,

        /// <summary>
        /// Represents a high-impedant state.
        /// </summary>
        HighZ = 0x03,
    }
}
