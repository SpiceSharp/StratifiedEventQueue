using StratifiedEventQueue.Simulation;

namespace StratifiedEventQueue.Events
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

        /// <summary>
        /// Releases the event from the scheduler.
        /// </summary>
        /// <remarks>
        /// This method allows events to be reused to avoid allocation of large number of objects.
        /// </remarks>
        public abstract void Release();
    }
}
