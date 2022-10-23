using StratifiedEventQueue.Events;
using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States.Nets;
using System;

namespace StratifiedEventQueue.States.Gates
{
    /// <summary>
    /// Describes a gate that can have a tri-state output.
    /// </summary>
    public abstract class TriGate : State<DriveStrengthRange>, IState<Signal>, IGate
    {
        private EventNode _nextEvent = null;
        private ulong _nextEventTime = 0;
        private readonly AssignmentEvent _event;
        private event EventHandler<StateChangedEventArgs<Signal>> SignalChanged;

        /// <inheritdoc />
        event EventHandler<StateChangedEventArgs<Signal>> IState<Signal>.Changed
        {
            add { SignalChanged += value; }
            remove { SignalChanged -= value; }
        }

        /// <inheritdoc />
        Signal IState<Signal>.Value => Value.Logic;

        /// <summary>
        /// Gets the name of the gate.
        /// </summary>
        public string GateName { get; }

        /// <summary>
        /// Gets the delay for rising signals.
        /// </summary>
        public uint RiseDelay { get; }

        /// <summary>
        /// Gets the delay for falling signals.
        /// </summary>
        public uint FallDelay { get; }

        /// <summary>
        /// Gets the delay for turning off.
        /// </summary>
        public uint TurnOffDelay { get; }

        /// <summary>
        /// Gets the delay for unknown transitions.
        /// </summary>
        public uint UnknownDelay { get; }

        protected class AssignmentEvent : Event
        {
            private readonly TriGate _parent;
            public DriveStrengthRange Value { get; set; }
            public AssignmentEvent(TriGate parent)
            {
                _parent = parent;
            }
            public override void Execute(IScheduler scheduler)
            {
                _parent.Change(scheduler, Value);
            }
        }

        /// <summary>
        /// Creates a new <see cref="TriGate"/>.
        /// </summary>
        /// <param name="gateName">The name of the gate itself.</param>
        /// <param name="outputName">The name of the output state.</param>
        /// <param name="riseDelay">The delay for signals going high.</param>
        /// <param name="fallDelay">The delay for signals going low.</param>
        /// <param name="turnOffDelay">The delay for signals turning off.</param>
        /// <exception cref="ArgumentNullException">Thrown if any argument is <c>null</c>.</exception>
        public TriGate(string gateName, string outputName,
            uint riseDelay, uint fallDelay, uint turnOffDelay)
            : base(outputName)
        {
            GateName = gateName ?? throw new ArgumentNullException(nameof(gateName));
            RiseDelay = riseDelay;
            FallDelay = fallDelay;
            TurnOffDelay = turnOffDelay;
            UnknownDelay = Math.Min(RiseDelay, Math.Min(fallDelay, turnOffDelay));
            _event = new AssignmentEvent(this);
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
                Change(args.Scheduler, result);
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
                // Already equal
                return;
            }
            var oldSignal = result.Logic;

            ulong delay;
            switch (result.Logic)
            {
                case Signal.L: delay = FallDelay; break;
                case Signal.H: delay = RiseDelay; break;
                case Signal.Z: delay = TurnOffDelay; break;
                default: delay = UnknownDelay; break;
            }
            ulong nextTime = args.Scheduler.CurrentTime + delay;

            // If the next event happens after this one, we will deschedule the next event
            if (nextTime <= _nextEventTime && _nextEvent != null)
                _nextEvent.Deschedule();

            // Schedule the next event
            _event.Value = result;
            _nextEvent = args.Scheduler.ScheduleInactive(delay, _event);
            _nextEventTime = nextTime;

            // Deal with the resulting logic level
            if (oldSignal != Value.Logic)
            {
                var nargs = StateChangedEventArgs<Signal>.Create(args.Scheduler, this, oldSignal);
                OnChanged(nargs);
                nargs.Release();
            }
        }

        /// <summary>
        /// Called when the signal changed.
        /// </summary>
        /// <param name="args">The argument.</param>
        protected virtual void OnChanged(StateChangedEventArgs<Signal> args)
            => SignalChanged?.Invoke(this, args);

        /// <summary>
        /// Computes a signal.
        /// </summary>
        /// <returns></returns>
        protected abstract DriveStrengthRange ComputeSignal();
    }
}
