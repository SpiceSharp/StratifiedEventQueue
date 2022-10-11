using StratifiedEventQueue.Events;

namespace StratifiedEventQueue.Simulation
{
    /// <summary>
    /// Represents a scheduler.
    /// </summary>
    public interface IScheduler
    {
        /// <summary>
        /// Gets the current time.
        /// </summary>
        ulong CurrentTime { get; }

        /// <summary>
        /// Gets the maximum time to simulate.
        /// </summary>
        ulong MaxTime { get; }

        /// <summary>
        /// Schedules a new event in the active event queue.
        /// </summary>
        /// <param name="event">The event.</param>
        EventNode Schedule(Event @event);

        /// <summary>
        /// Schedules a new event in the inactive event queue.
        /// </summary>
        /// <param name="delay">The delay when the event needs to be activated.</param>
        /// <param name="event">The event.</param>
        EventNode ScheduleInactive(ulong delay, Event @event);

        /// <summary>
        /// Schedules a new event in the non-blocking event queue.
        /// </summary>
        /// <param name="delay">The delay when the event needs to be activated.</param>
        /// <param name="event">The event.</param>
        EventNode ScheduleNonBlocking(ulong delay, Event @event);

        /// <summary>
        /// Schedules a new event in the monitor event queue.
        /// </summary>
        /// <param name="event">The event.</param>
        EventNode ScheduleMonitor(Event @event);

        /// <summary>
        /// Processes all items in the event queue.
        /// </summary>
        void Process();
    }
}
