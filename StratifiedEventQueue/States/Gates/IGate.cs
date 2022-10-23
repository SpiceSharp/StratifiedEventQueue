namespace StratifiedEventQueue.States.Gates
{
    /// <summary>
    /// Describes a gate.
    /// </summary>
    public interface IGate : IState<Signal>
    {
        /// <summary>
        /// Gets the name of the gate.
        /// </summary>
        string GateName { get; }
    }
}
