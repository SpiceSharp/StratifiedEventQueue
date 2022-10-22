namespace StratifiedEventQueue.States.Nets
{
    /// <summary>
    /// Represents a strength level as defined by the Verilog standard.
    /// </summary>
    public enum Strength : sbyte
    {
        /// <summary>
        /// No drive strength.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents a HiZ0 (High-Z 0) drive strength.
        /// </summary>
        HiZ0 = -1,

        /// <summary>
        /// Represents a Sm0 (small 0) charge strength.
        /// </summary>
        Sm0 = -2,

        /// <summary>
        /// Represents a Me0 (medium 0) charge strength.
        /// </summary>
        Me0 = -3,

        /// <summary>
        /// Represents a We0 (weak 0) drive strength.
        /// </summary>
        We0 = -4,

        /// <summary>
        /// Represents a La0 (large 0) charge strength.
        /// </summary>
        La0 = -5,

        /// <summary>
        /// Represents a Pu0 (pull 0) drive strength.
        /// </summary>
        Pu0 = -6,

        /// <summary>
        /// Represents a St0 (strong 0) drive strength.
        /// </summary>
        St0 = -7,

        /// <summary>
        /// Represents a Su0 (supply 0) drive strength.
        /// </summary>
        Su0 = -8,

        /// <summary>
        /// Represents a HiZ1 (high-Z 1)drive strength.
        /// </summary>
        HiZ1 = 1,

        /// <summary>
        /// Represents a Sm1 (small 1) charge strength.
        /// </summary>
        Sm1 = 2,

        /// <summary>
        /// Represents a Me1 (medium 1) drive strength.
        /// </summary>
        Me1 = 3,

        /// <summary>
        /// Represents a We1 (weak 1) charge strength.
        /// </summary>
        We1 = 4,

        /// <summary>
        /// Represents a La1 (large 1) charge strength.
        /// </summary>
        La1 = 5,

        /// <summary>
        /// Represents a Pu1 (pull 1) drive strength.
        /// </summary>
        Pu1 = 6,

        /// <summary>
        /// Represents an St1 (strong 1) drive strength.
        /// </summary>
        St1 = 7,

        /// <summary>
        /// Represents an Su1 (supply 1) drive strength.
        /// </summary>
        Su1 = 8,
    }
}
