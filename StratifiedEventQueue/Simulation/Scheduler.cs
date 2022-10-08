using StratifiedEventQueue.Events;

namespace StratifiedEventQueue.Simulation
{
    /// <summary>
    /// A scheduler for tracking time.
    /// </summary>
    public class Scheduler
    {
        private readonly EventRegion _active = new EventRegion(), _monitor = new EventRegion();
        private readonly SplayTree _tree = new SplayTree();

        /// <summary>
        /// Gets the current time.
        /// </summary>
        public ulong CurrentTime { get; private set; }

        /// <summary>
        /// Gets or sets the maximum time to simulate.
        /// </summary>
        public ulong MaxTime { get; set; } = ulong.MaxValue;

        /// <summary>
        /// Creates a new <see cref="Scheduler"/>.
        /// </summary>
        public Scheduler()
        {
            CurrentTime = 0;
        }

        /// <summary>
        /// Schedules a new event in the active event queue.
        /// </summary>
        /// <param name="event">The event.</param>
        public void Schedule(Event @event) => _active.Add(@event);

        /// <summary>
        /// Schedules a new event in the inactive event queue.
        /// </summary>
        /// <param name="delay">The delay when the event needs to be activated.</param>
        /// <param name="event">The event.</param>
        public void ScheduleInactive(ulong delay, Event @event)
        {
            var eventQueue = _tree.GetOrAdd(CurrentTime + delay);
            eventQueue.Inactive.Add(@event);
        }

        /// <summary>
        /// Schedules a new event in the non-blocking event queue.
        /// </summary>
        /// <param name="delay">The delay when the event needs to be activated.</param>
        /// <param name="event">The event.</param>
        public void ScheduleNonBlocking(ulong delay, Event @event)
        {
            var eventQueue = _tree.GetOrAdd(CurrentTime + delay);
            eventQueue.NonBlocking.Add(@event);
        }

        /// <summary>
        /// Schedules a new event in the monitor event queue.
        /// </summary>
        /// <param name="event">The event.</param>
        public void ScheduleMonitor(Event @event) => _monitor.Add(@event);

        /// <summary>
        /// Processes all items in the event queue.
        /// </summary>
        public void Process()
        {
            while (_tree.Count > 0)
            {
                var node = _tree.PopFirst();
                if (node.Key > MaxTime)
                    return;
                var events = node.Value;
                CurrentTime = node.Key;

                // Stratified event queue
                while (true)
                {
                    if (_active.Count == 0)
                    {
                        if (events.Inactive.Count > 0)
                        {
                            while (events.Inactive.Count > 0)
                                _active.Add(events.Inactive.Extract());
                        }
                        else if (events.NonBlocking.Count > 0)
                        {
                            while (events.NonBlocking.Count > 0)
                                _active.Add(events.NonBlocking.Extract());
                        }
                        else if (_monitor.Count > 0)
                        {
                            while (_monitor.Count > 0)
                                _active.Add(_monitor.Extract());
                        }
                        else
                            break;
                    }

                    // Get the next active event and execute it
                    var @event = _active.Extract();
                    @event.Execute(this);
                }
            }
        }
    }
}
