using StratifiedEventQueue.Events;
using System;

namespace StratifiedEventQueue.Simulation
{
    /// <summary>
    /// A scheduler for tracking time.
    /// </summary>
    public class Scheduler : IScheduler
    {
        private readonly EventRegion _active = new EventRegion(), _monitor = new EventRegion();
        private readonly SplayTree _tree = new SplayTree();

        /// <inheritdoc />
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

        /// <inheritdoc />
        public void Schedule(Event @event) => _active.Add(@event ?? throw new ArgumentNullException(nameof(@event)));

        /// <inheritdoc />
        public void ScheduleInactive(ulong delay, Event @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));
            var eventQueue = _tree.GetOrAdd(CurrentTime + delay);
            eventQueue.Inactive.Add(@event);
        }

        /// <inheritdoc />
        public void ScheduleNonBlocking(ulong delay, Event @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));
            var eventQueue = _tree.GetOrAdd(CurrentTime + delay);
            eventQueue.NonBlocking.Add(@event);
        }

        /// <inheritdoc />
        public void ScheduleMonitor(Event @event) => _monitor.Add(@event);

        /// <inheritdoc />
        public void Process()
        {
            Event @event;
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
                            while ((@event = events.Inactive.Extract()) != null)
                                _active.Add(@event);
                        }
                        else if (events.NonBlocking.Count > 0)
                        {
                            while ((@event = events.NonBlocking.Extract()) != null)
                                _active.Add(@event);
                        }
                        else if (_monitor.Count > 0)
                        {
                            while ((@event = _monitor.Extract()) != null)
                                _active.Add(@event);
                        }
                        else
                            break;
                    }

                    // Get the next active event and execute it
                    while ((@event = _active.Extract()) != null)
                    {
                        @event.Execute(this);
                        @event.Release();
                    }
                }
            }
        }
    }
}
