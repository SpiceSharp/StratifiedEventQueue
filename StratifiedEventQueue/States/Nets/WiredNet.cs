using StratifiedEventQueue.Events;
using StratifiedEventQueue.Simulation;
using System;
using System.Collections.Generic;

namespace StratifiedEventQueue.States.Nets
{
    /// <summary>
    /// A net represents wired logic, which can come from multiple drivers.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    public abstract class WiredNet : State<DriveStrengthRange>, IState<Signal>
    {
        private readonly List<IState<DriveStrengthRange>> _drivers = new List<IState<DriveStrengthRange>>();
        private readonly UpdateEvent _event;
        private Signal _signal;
        private EventNode _nextEvent;
        private ulong _nextEventTime;
        private event EventHandler<StateChangedEventArgs<Signal>> SignalChanged;

        private class UpdateEvent : Event
        {
            private readonly WiredNet _parent;
            public DriveStrengthRange Value { get; set; }
            public UpdateEvent(WiredNet parent)
            {
                _parent = parent;
            }
            /// <inheritdoc />
            public override void Execute(IScheduler scheduler)
            {
                _parent.Update(scheduler, Value);

                // Deal with the signal
                var signal = Value.Logic;
                if (_parent._signal != signal)
                {
                    var args = StateChangedEventArgs<Signal>.Create(scheduler, _parent, _parent._signal);
                    _parent._signal = signal;
                    _parent.OnChanged(args);
                    args.Release();
                }
            }
        }

        /// <summary>
        /// Gets the delay for high signals.
        /// </summary>
        public uint RiseDelay { get; }

        /// <summary>
        /// Gets the delay for low signals.
        /// </summary>
        public uint FallDelay { get; }

        /// <summary>
        /// Gets the delay for turning off.
        /// </summary>
        public uint TurnOffDelay { get; }

        /// <inheritdoc />
        Signal IState<Signal>.Value => _signal;

        /// <inheritdoc />
        event EventHandler<StateChangedEventArgs<Signal>> IState<Signal>.Changed
        {
            add { SignalChanged += value; }
            remove { SignalChanged -= value; }
        }

        /// <summary>
        /// Creates a new <see cref="WiredNet"/>.
        /// </summary>
        /// <param name="name">The name of the net.</param>
        /// <param name="riseDelay">The net delay for rising signals.</param>
        /// <param name="fallDelay">The net delay for falling signals.</param>
        /// <param name="turnOffDelay">The net delay for high-impedant signals.</param>
        protected WiredNet(string name, uint riseDelay = 0, uint fallDelay = 0, uint turnOffDelay = 0)
            : base(name)
        {
            RiseDelay = riseDelay;
            FallDelay = fallDelay;
            TurnOffDelay = turnOffDelay;
            _event = new UpdateEvent(this);
        }

        /// <summary>
        /// Assigns a driver to the net.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="driver"/> is <c>null</c>.</exception>
        public void Assign(IState<DriveStrengthRange> driver)
        {
            _drivers.Add(driver ?? throw new ArgumentNullException(nameof(driver)));
            Update(this, null);

            // Add listener
            if (RiseDelay == 0 && FallDelay == 0 && TurnOffDelay == 0)
                driver.Changed += UpdateZeroDelay;
            else
                driver.Changed += Update;
        }

        /// <summary>
        /// Deassigns a driver from the net.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="driver"/> is <c>null</c>.</exception>
        public void Deassign(IState<DriveStrengthRange> driver)
        {
            if (_drivers.Remove(driver ?? throw new ArgumentNullException(nameof(driver))))
            {
                // The value might have changed
                Update(this, null);
                if (RiseDelay == 0 && FallDelay == 0 && TurnOffDelay == 0)
                    driver.Changed -= UpdateZeroDelay;
                else
                    driver.Changed -= Update;
            }
        }

        protected void UpdateZeroDelay(object sender, StateChangedEventArgs<DriveStrengthRange> args)
        {
            // Determine the result of the wire
            DriveStrengthRange result;
            if (_drivers.Count == 0)
                result = default;
            else
            {
                result = _drivers[0].Value;
                for (int i = 1; i < _drivers.Count; i++)
                    result = Combine(result, _drivers[i].Value);
            }
            if (result == Value)
                return; // Nothing changes
            Update(args.Scheduler, result);
        }

        /// <summary>
        /// Updates the wired net value.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The event arguments of the driver that changed.</param>
        protected void Update(object sender, StateChangedEventArgs<DriveStrengthRange> args)
        {
            // Determine the result of the wire
            DriveStrengthRange result;
            if (_drivers.Count == 0)
                result = default;
            else
            {
                result = _drivers[0].Value;
                for (int i = 1; i < _drivers.Count; i++)
                    result = Combine(result, _drivers[i].Value);
            }

            if (result == _event.Value)
                return; // Nothing changes...

            uint delay;
            switch (result.Logic)
            {
                case Signal.H:
                    delay = RiseDelay;
                    break;

                case Signal.L:
                    delay = FallDelay;
                    break;

                case Signal.Z:
                    delay = TurnOffDelay;
                    break;

                default:
                    if (result.Low >= Strength.HiZ0 && result.Low <= Strength.HiZ1)
                        // Signal is at least high-impedant, else driven to a high value
                        delay = Math.Min(RiseDelay, TurnOffDelay);
                    else if (result.High >= Strength.HiZ0 && result.High <= Strength.HiZ1)
                        // Signal is at most high-impedant, else driven to a low value
                        delay = Math.Min(FallDelay, TurnOffDelay);
                    else
                        // Signal can be both low, high or high-impedant...
                        delay = Math.Min(FallDelay, Math.Min(TurnOffDelay, RiseDelay));
                    break;
            }
            ulong nextTime = args.Scheduler.CurrentTime + delay;

            // If the next event happens before this one, deschedule the next event
            if (nextTime <= _nextEventTime && _nextEvent != null)
                _nextEvent.Deschedule();

            // If the result is different from the current value, schedule the new event
            if (result != Value)
            {
                _event.Value = result;
                _nextEvent = args.Scheduler.ScheduleInactive(delay, _event);
                _nextEventTime = nextTime;
            }
            else
            {
                // Value doesn't change, no need to schedule a new event
                _nextEvent = null;
            }
        }

        /// <summary>
        /// Combines 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected abstract DriveStrengthRange Combine(DriveStrengthRange a, DriveStrengthRange b);

        /// <summary>
        /// Called when the signal changes.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        protected virtual void OnChanged(StateChangedEventArgs<Signal> args)
            => SignalChanged?.Invoke(this, args);
    }
}
