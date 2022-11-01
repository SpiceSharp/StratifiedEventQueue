using StratifiedEventQueue.Events;
using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States;
using StratifiedEventQueue.States.Nets;
using System;

namespace StratifiedEventQueue.Processes
{
    /// <summary>
    /// Defines a continuous assignment.
    /// </summary>
    public abstract class ContinuousAssignment : Process
    {
        private DriveStrengthRange _current;
        private EventNode _nextEvent;
        private ulong _nextEventTime;
        private readonly UpdateEvent _event;
        private readonly Action<IScheduler> _update;

        /// <summary>
        /// An update event dedicated to <see cref="ContinuousAssignment"/>.
        /// </summary>
        private class UpdateEvent : Event
        {
            private readonly ContinuousAssignment _parent;
            public DriveStrengthRange Value { get; set; }
            public UpdateEvent(ContinuousAssignment parent)
            {
                _parent = parent;
            }
            public override void Execute(IScheduler scheduler)
            {
                _parent.Output.Update(scheduler, Value);
                _parent._current = Value;
            }
        }

        /// <summary>
        /// Gets the delay for a transition from non-zero to zero.
        /// </summary>
        public Func<uint> FallingDelay { get; }

        /// <summary>
        /// Gets the delay for making a transition to Z.
        /// </summary>
        public Func<uint> TurnOffDelay { get; }

        /// <summary>
        /// Gets the delay for transitions to non-zero and non-Z.
        /// </summary>
        public Func<uint> RisingDelay { get; }

        /// <summary>
        /// Gets the driver of the output.
        /// </summary>
        public Driver Output { get; }

        /// <summary>
        /// Gets the strength for low signals.
        /// </summary>
        public Strength Strength0 { get; }

        /// <summary>
        /// Gets the strength level for high signals.
        /// </summary>
        public Strength Strength1 { get; }

        /// <summary>
        /// Creates a new <see cref="ContinuousAssignment"/>.
        /// </summary>
        /// <remarks>
        /// Continuous assignments shall drive values onto nets, both vector and scalar.
        /// </remarks>
        /// <param name="output">The driver that will drive a net.</param>
        /// <param name="risingDelay">The rising delay.</param>
        /// <param name="fallingDelay">The falling delay.</param>
        /// <param name="turnOffDelay">The turn-off delay.</param>
        /// <param name="strength0">The strength 0.</param>
        /// <param name="strength1">The strength 1.</param>
        protected ContinuousAssignment(Driver output,
            Func<uint> risingDelay, Func<uint> fallingDelay, Func<uint> turnOffDelay,
            Strength strength0, Strength strength1)
        {
            Output = output ?? throw new ArgumentNullException(nameof(output));
            if (risingDelay == null)
            {
                // No delay specification
                _update = UpdateZeroDelay;
            }
            else if (fallingDelay == null)
            {
                // When one delay value is given, then this value shall be used for all propagation delays associated with the
                // gate or the net.
                RisingDelay = risingDelay;
                _update = UpdateSingleDelay;
            }
            else if (turnOffDelay == null)
            {
                // When two delays are given, the first delay shall specify the rise delay, and the second delay shall specify the
                // fall delay. The delay when the signal changes to high impedance or to unknown shall be the lesser of the two
                // delay values.
                RisingDelay = risingDelay;
                FallingDelay = fallingDelay;
                _update = UpdateTwoDelays;
            }
            else
            {
                FallingDelay = fallingDelay;
                TurnOffDelay = turnOffDelay;
                RisingDelay = risingDelay;
                _update = UpdateThreeDelays;
            }
            Strength0 = strength0;
            Strength1 = strength1;
            _event = new UpdateEvent(this);
        }

        /// <inheritdoc />
        protected override void Execute(IScheduler scheduler)
            => _update(scheduler);

        /// <summary>
        /// Updates the continuous assignment without any delays involved.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        protected void UpdateZeroDelay(IScheduler scheduler)
        {
            // Determine the result of the continuous assignment
            var signal = Compute();
            DriveStrengthRange result = DetermineStrength(signal);

            if (result != _event.Value)
            {
                _event.Value = result;
                scheduler.Schedule(_event);
            }
        }

        /// <summary>
        /// Updates the continuous assignment assuming a single delay specification.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        protected void UpdateSingleDelay(IScheduler scheduler)
        {
            var signal = Compute();
            DriveStrengthRange result = DetermineStrength(signal);

            // The delay is simple: we use the risingDelay specification
            uint delay = RisingDelay();
            ulong nextTime = scheduler.CurrentTime + delay;

            // If the next event happens after this one, we will deschedule the event
            if (nextTime <= _nextEventTime && _nextEvent != null)
                _nextEvent.Deschedule();

            // Schedule the next event if the current value is different
            if (result != _current)
            {
                _event.Value = result;
                _nextEvent = scheduler.ScheduleInactive(delay, _event);
                _nextEventTime = nextTime;
            }
            else
                _nextEvent = null;
        }

        /// <summary>
        /// Updates the continuous assignment assuming two delays specified.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        protected void UpdateTwoDelays(IScheduler scheduler)
        {
            var signal = Compute();
            DriveStrengthRange result = DetermineStrength(signal);

            // We have only three cases
            uint delay;
            switch (signal)
            {
                case Signal.H:
                    delay = RisingDelay();
                    break;

                case Signal.L:
                    delay = FallingDelay();
                    break;

                default:
                    delay = Math.Min(RisingDelay(), FallingDelay());
                    break;
            }
            ulong nextTime = scheduler.CurrentTime + delay;

            // If the next event happens after this one, we will deschedule the event
            if (nextTime <= _nextEventTime && _nextEvent != null)
                _nextEvent.Deschedule();

            // Schedule the next event if the current value is different
            if (result != _current)
            {
                _event.Value = result;
                _nextEvent = scheduler.ScheduleInactive(delay, _event);
                _nextEventTime = nextTime;
            }
            else
                _nextEvent = null;
        }

        /// <summary>
        /// Updates the continuous assignment with possible delays.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        protected void UpdateThreeDelays(IScheduler scheduler)
        {
            // Determine the result of the continuous assignment
            var signal = Compute();
            var result = DetermineStrength(signal);
            uint delay;
            switch (signal)
            {
                case Signal.H:
                    delay = RisingDelay();
                    break;

                case Signal.L:
                    delay = FallingDelay();
                    break;

                case Signal.Z:
                    delay = TurnOffDelay();
                    break;

                default:
                    delay = RisingDelay();
                    delay = Math.Min(delay, FallingDelay());
                    delay = Math.Min(delay, TurnOffDelay());
                    break;
            }
            ulong nextTime = scheduler.CurrentTime + delay;

            // If the next event happens after this one, we will deschedule the event
            if (nextTime <= _nextEventTime && _nextEvent != null)
                _nextEvent.Deschedule();

            // Schedule the next event if the current value is different
            if (result != _current)
            {
                _event.Value = result;
                _nextEvent = scheduler.ScheduleInactive(delay, _event);
                _nextEventTime = nextTime;
            }
            else
                _nextEvent = null;
        }

        /// <summary>
        /// Determines the <see cref="DriveStrengthRange"/> associated with
        /// the signal.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <returns>The drive strength range.</returns>
        protected virtual DriveStrengthRange DetermineStrength(Signal signal)
        {
            switch (signal)
            {
                case Signal.H:
                    return new DriveStrengthRange(Strength1);

                case Signal.L:
                    return new DriveStrengthRange(Strength0);

                case Signal.Z:
                    return new DriveStrengthRange();

                default:
                    return new DriveStrengthRange(Strength0, Strength1);
            }
        }

        /// <summary>
        /// Computes the result of the continuous assignment.
        /// </summary>
        /// <returns>The result.</returns>
        protected abstract Signal Compute();
    }
}
