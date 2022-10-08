namespace StratifiedEventQueue.Simulation
{
    /// <summary>
    /// Describes the stratified event queue.
    /// </summary>
    public class EventQueue
    {
        /// <summary>
        /// All inactive events that will be activated once all active events have finished.
        /// </summary>
        public EventRegion Inactive { get; }

        /// <summary>
        /// All non-blocking events that will be activated once all active and inactive events have finished.
        /// </summary>
        public EventRegion NonBlocking { get; }

        /// <summary>
        /// Creates a new <see cref="EventQueue"/>.
        /// </summary>
        public EventQueue()
        {
            Inactive = new EventRegion();
            NonBlocking = new EventRegion();
        }
    }
}
