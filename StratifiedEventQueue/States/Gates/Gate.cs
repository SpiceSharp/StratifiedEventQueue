﻿using StratifiedEventQueue.Events;
using StratifiedEventQueue.Simulation;
using System;

namespace StratifiedEventQueue.States.Gates
{
    /// <summary>
    /// Describes a gate that can be either high or low at the output.
    /// </summary>
    public abstract class Gate : State<Signal>, IGate
    {
        private EventNode _nextEvent = null;
        private ulong _nextEventTime = 0;
        private readonly UpdateEvent _event;

        /// <summary>
        /// Gets the name of the gate.
        /// </summary>
        public string GateName { get; }

        /// <summary>
        /// Gets the delay for rising signals.
        /// </summary>
        public Func<uint> RiseDelay { get; }

        /// <summary>
        /// Gets the delay for falling signals.
        /// </summary>
        public Func<uint> FallDelay { get; }

        protected class UpdateEvent : Event
        {
            private readonly Gate _parent;
            public Signal Value { get; set; }
            public UpdateEvent(Gate parent)
            {
                _parent = parent;
            }
            /// <inheritdoc />
            public override void Execute(IScheduler scheduler) => _parent.Update(scheduler, Value);
        }

        /// <summary>
        /// Creates a new <see cref="Gate"/>.
        /// </summary>
        /// <param name="gateName">The name of the gate itself.</param>
        /// <param name="outputName">The name of the output state.</param>
        /// <param name="fallDelay">The delay for signals going to the low state.</param>
        /// <param name="riseDelay">The delay for signals going to the high state.</param>
        /// <exception cref="ArgumentNullException">Thrown if any argument is <c>null</c>.</exception>
        public Gate(string gateName, string outputName, Func<uint> riseDelay, Func<uint> fallDelay)
            : base(outputName)
        {
            GateName = gateName ?? throw new ArgumentNullException(nameof(gateName));
            _event = new UpdateEvent(this);
            RiseDelay = riseDelay;
            FallDelay = fallDelay;
        }

        /// <summary>
        /// The update event listener that assumes zero delays for all.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The event arguments.</param>
        protected void UpdateZeroDelay(object sender, StateChangedEventArgs<Signal> args)
        {
            var result = ComputeSignal();
            if (result == Value)
                return;
            else
                Update(args.Scheduler, result);
        }

        /// <summary>
        /// The update event listener that assumes delays are non-zero.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The event arguments.</param>
        protected void Update(object sender, StateChangedEventArgs<Signal> args)
        {
            var result = ComputeSignal();
            if (result == _event.Value)
            {
                // Already equal to whatever is being scheduled or current value
                return;
            }

            // Compute the delay of the gate
            uint delay;
            switch (result)
            {
                case Signal.L: delay = FallDelay(); break;
                case Signal.H: delay = RiseDelay(); break;
                default: delay = Math.Min(FallDelay(), RiseDelay()); break;
            }
            ulong nextTime = args.Scheduler.CurrentTime + delay;

            // If the next event happens after this one, we will deschedule the event
            if (nextTime <= _nextEventTime && _nextEvent != null)
                _nextEvent.Deschedule();

            // Schedule the next event
            if (result != Value)
            {
                _event.Value = result;
                _nextEvent = args.Scheduler.ScheduleInactive(delay, _event);
                _nextEventTime = nextTime;
            }
            else
            {
                _nextEvent = null;
            }
        }

        /// <summary>
        /// Computes the result of the gate.
        /// </summary>
        /// <returns>The result.</returns>
        protected abstract Signal ComputeSignal();
    }
}
