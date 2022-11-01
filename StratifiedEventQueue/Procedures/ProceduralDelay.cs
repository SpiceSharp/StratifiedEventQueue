using StratifiedEventQueue.Events;
using StratifiedEventQueue.Simulation;
using System;

namespace StratifiedEventQueue.Procedures
{
    /// <summary>
    /// A procedural statement that can be scheduled in the future to model
    /// procedural delays.
    /// </summary>
    public class ProceduralDelay : ProceduralStatement
    {
        private readonly FinishEvent _event;

        /// <summary>
        /// The event that gets activated when the delay finishes allowing the next statement to start.
        /// </summary>
        protected class FinishEvent : Event
        {
            private readonly ProceduralDelay _parent;
            public FinishEvent(ProceduralDelay parent)
            {
                _parent = parent;
            }
            public override void Execute(IScheduler scheduler)
            {
                var args = ProceduralStatementEventArgs.Create(scheduler);
                _parent.OnExecuted(args);
                args.Release();
            }
        }

        /// <summary>
        /// Gets the delay.
        /// </summary>
        public Func<uint> Delay { get; }

        /// <summary>
        /// Creates a new <see cref="ProceduralDelay"/>.
        /// </summary>
        /// <param name="delay">The delay.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="delay"/> is <c>null</c>.</exception>
        public ProceduralDelay(Func<uint> delay)
        {
            Delay = delay ?? throw new ArgumentNullException(nameof(delay));
            _event = new FinishEvent(this);
        }

        /// <inheritdoc />
        protected override void Execute(IScheduler scheduler)
        {
            uint delay = Delay();
            scheduler.ScheduleInactive(delay, _event);
        }
    }
}
