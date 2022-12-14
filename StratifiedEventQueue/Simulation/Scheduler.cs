using StratifiedEventQueue.Events;
using System;
using System.Collections.Generic;

namespace StratifiedEventQueue.Simulation
{
    /// <summary>
    /// A scheduler for tracking time.
    /// </summary>
    public class Scheduler : IScheduler
    {
        private Queue<EventNode> _active = new Queue<EventNode>(), _monitor = new Queue<EventNode>();
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
        public EventNode Schedule(Event @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));
            var node = EventNode.Create(@event);
            _active.Enqueue(node);
            return node;
        }

        /// <inheritdoc />
        public EventNode ScheduleInactive(ulong delay, Event @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));
            var eventQueue = _tree.GetOrAdd(CurrentTime + delay);
            var node = EventNode.Create(@event);
            eventQueue.Inactive.Enqueue(node);
            return node;
        }

        /// <inheritdoc />
        public EventNode ScheduleNonBlocking(ulong delay, Event @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));
            var eventQueue = _tree.GetOrAdd(CurrentTime + delay);
            var node = EventNode.Create(@event);
            eventQueue.NonBlocking.Enqueue(node);
            return node;
        }

        /// <inheritdoc />
        public EventNode ScheduleMonitor(Event @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));
            var node = EventNode.Create(@event);
            _monitor.Enqueue(node);
            return node;
        }

        /// <inheritdoc />
        public void Process()
        {
            // First empty the active queue
            while (!_active.IsEmpty)
            {
                var a = _active.Dequeue();
                if (a.IsScheduled)
                    a.Event.Execute(this);
                a.Release();
            }

            // Go to the next point
            while (_tree.Count > 0)
            {
                var node = _tree.PeekFirst();
                if (node.Key > MaxTime)
                    return;
                var events = node.Value;
                CurrentTime = node.Key;

                // Stratified event queue
                while (true)
                {
                    if (_active.IsEmpty)
                    {
                        if (!events.Inactive.IsEmpty)
                            events.SwapInactive(ref _active);
                        else if (!events.NonBlocking.IsEmpty)
                            events.SwapNonBlocking(ref _active);
                        else if (!_monitor.IsEmpty)
                            (_active, _monitor) = (_monitor, _active);
                        else
                            break;
                    }

                    // Run the active event queue
                    while (!_active.IsEmpty)
                    {
                        var a = _active.Dequeue();
                        if (a.IsScheduled)
                            a.Event.Execute(this);
                        a.Release();
                    }
                }

                // Remove the processed event queue
                _tree.PopFirst();
            }
        }
    }
}
