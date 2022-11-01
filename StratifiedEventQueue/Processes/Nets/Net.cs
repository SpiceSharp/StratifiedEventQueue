using StratifiedEventQueue.Events;
using StratifiedEventQueue.Simulation;
using System;
using System.Collections.Generic;

namespace StratifiedEventQueue.States.Nets
{
    /// <summary>
    /// A net, which can derive a state come from multiple drivers.
    /// </summary>
    public abstract class Net : State<DriveStrengthRange>, IState<Signal>
    {
        private readonly List<NetDriver> _drivers = new List<NetDriver>();
        private readonly UpdateEvent _event;
        private Signal _signal;
        private EventNode _nextEvent;
        private ulong _nextEventTime;
        private event EventHandler<StateChangedEventArgs<Signal>> SignalChanged;
        private Action<IScheduler> _update;

        /// <summary>
        /// The implementation of <see cref="Driver"/> for a <see cref="Net"/>.
        /// </summary>
        protected class NetDriver : Driver
        {
            public Net Parent { get; set; }

            /// <summary>
            /// Gets the value of the driver.
            /// </summary>
            public DriveStrengthRange Value { get; private set; }

            /// <inheritdoc />
            public override void Update(IScheduler scheduler, DriveStrengthRange strength)
            {
                Value = strength;
                Parent?._update(scheduler);
            }
        }

        /// <summary>
        /// An update event dedicated to a <see cref="Net"/>.
        /// </summary>
        protected class UpdateEvent : Event
        {
            private readonly Net _parent;
            public DriveStrengthRange Value { get; set; }
            public UpdateEvent(Net parent)
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
        public Func<uint> RiseDelay { get; }

        /// <summary>
        /// Gets the delay for low signals.
        /// </summary>
        public Func<uint> FallDelay { get; }

        /// <summary>
        /// Gets the delay for turning off.
        /// </summary>
        public Func<uint> TurnOffDelay { get; }

        /// <inheritdoc />
        Signal IState<Signal>.Value => _signal;

        /// <inheritdoc />
        event EventHandler<StateChangedEventArgs<Signal>> IState<Signal>.Changed
        {
            add { SignalChanged += value; }
            remove { SignalChanged -= value; }
        }

        /// <summary>
        /// Creates a new <see cref="Net"/>.
        /// </summary>
        /// <param name="name">The name of the net.</param>
        /// <param name="riseDelay">The net delay for rising signals.</param>
        /// <param name="fallDelay">The net delay for falling signals.</param>
        /// <param name="turnOffDelay">The net delay for high-impedant signals.</param>
        protected Net(string name,
            Func<uint> riseDelay = null, Func<uint> fallDelay = null, Func<uint> turnOffDelay = null)
            : base(name)
        {
            RiseDelay = riseDelay;
            FallDelay = fallDelay;
            TurnOffDelay = turnOffDelay;
            _event = new UpdateEvent(this);
            if (RiseDelay == null && FallDelay == null && TurnOffDelay == null)
                _update = UpdateZeroDelay;
            else
                _update = Update;
        }

        /// <summary>
        /// Assigns a driver to the net.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="driver"/> is <c>null</c>.</exception>
        public Driver Assign(IScheduler scheduler)
        {
            var driver = new NetDriver()
            {
                Parent = this
            };
            _drivers.Add(driver);
            _update(scheduler);
            return driver;
        }

        /// <summary>
        /// Deassigns a driver from the net.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="driver"/> is <c>null</c>.</exception>
        public void Deassign(IScheduler scheduler, Driver driver)
        {
            if (driver is NetDriver netDriver)
            {
                if (!ReferenceEquals(netDriver.Parent, this))
                    throw new ArgumentException();
                netDriver.Parent = null;
                _drivers.Remove(netDriver);
                _update(scheduler);
            }
        }

        /// <summary>
        /// Updates the wired net value.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        protected void UpdateZeroDelay(IScheduler scheduler)
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

            // Schedule the update event in the active queue
            _event.Value = result;
            scheduler.Schedule(_event);
        }

        /// <summary>
        /// Updates the wired net value.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        protected void Update(IScheduler scheduler)
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
                    delay = RiseDelay?.Invoke() ?? 0;
                    break;

                case Signal.L:
                    delay = FallDelay?.Invoke() ?? 0;
                    break;

                case Signal.Z:
                    delay = TurnOffDelay?.Invoke() ?? 0;
                    break;

                default:
                    if (result.Low >= Strength.HiZ0 && result.Low <= Strength.HiZ1)
                        // Signal is at least high-impedant, else driven to a high value
                        delay = Math.Min(RiseDelay?.Invoke() ?? 0, TurnOffDelay?.Invoke() ?? 0);
                    else if (result.High >= Strength.HiZ0 && result.High <= Strength.HiZ1)
                        // Signal is at most high-impedant, else driven to a low value
                        delay = Math.Min(FallDelay?.Invoke() ?? 0, TurnOffDelay?.Invoke() ?? 0);
                    else
                        // Signal can be both low, high or high-impedant...
                        delay = Math.Min(FallDelay?.Invoke() ?? 0, Math.Min(TurnOffDelay?.Invoke() ?? 0, RiseDelay?.Invoke() ?? 0));
                    break;
            }
            ulong nextTime = scheduler.CurrentTime + delay;

            // If the next event happens before this one, deschedule the next event
            if (nextTime <= _nextEventTime && _nextEvent != null)
                _nextEvent.Deschedule();

            // If the result is different from the current value, schedule the new event
            if (result != Value)
            {
                _event.Value = result;
                _nextEvent = scheduler.ScheduleInactive(delay, _event);
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
