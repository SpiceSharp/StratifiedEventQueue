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
        /// Executes and releases the event from the given scheduler.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        public abstract void Execute(IScheduler scheduler);
    }
}
