namespace StratifiedEventQueue.Simulation
{
    /// <summary>
    /// Describes an event.
    /// </summary>
    /// <typeparam name="S">The state type.</typeparam>
    public abstract class Event
    {
        /// <summary>
        /// Gets or sets whether the event is descheduled. If <c>true</c>, the event is
        /// consumed without being executed.
        /// </summary>
        public bool Descheduled { get; set; }

        /// <summary>
        /// Executes and releases the event from the given scheduler.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        public abstract void Execute(Scheduler scheduler);
    }
}
