using StratifiedEventQueue.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace StratifiedEventQueue.Simulation
{
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
        void Schedule(Event @event);

        /// <summary>
        /// Schedules a new event in the inactive event queue.
        /// </summary>
        /// <param name="delay">The delay when the event needs to be activated.</param>
        /// <param name="event">The event.</param>
        void ScheduleInactive(ulong delay, Event @event);

        /// <summary>
        /// Schedules a new event in the non-blocking event queue.
        /// </summary>
        /// <param name="delay">The delay when the event needs to be activated.</param>
        /// <param name="event">The event.</param>
        void ScheduleNonBlocking(ulong delay, Event @event);

        /// <summary>
        /// Schedules a new event in the monitor event queue.
        /// </summary>
        /// <param name="event">The event.</param>
        void ScheduleMonitor(Event @event);

        /// <summary>
        /// Processes all items in the event queue.
        /// </summary>
        void Process();
    }
}
