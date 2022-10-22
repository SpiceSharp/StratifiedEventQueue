using StratifiedEventQueue.Simulation;
using System;
using System.Collections.Generic;

namespace StratifiedEventQueue.States.Nets
{
    /// <summary>
    /// A net represents wired logic, which can come from multiple drivers.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    public abstract class WiredNet : InvariantNet<DriveStrengthRange>, IState<Signal>
    {
        private Signal _signal;
        private event EventHandler<StateChangedEventArgs<Signal>> SignalChanged;

        /// <inheritdoc />
        Signal IState<Signal>.Value => _signal;

        /// <inheritdoc />
        event EventHandler<StateChangedEventArgs<Signal>> IState<Signal>.Changed
        {
            add { SignalChanged += value; }
            remove { SignalChanged -= value; }
        }

        /// <inheritdoc />
        IEqualityComparer<Signal> IState<Signal>.Comparer => EqualityComparer<Signal>.Default;

        /// <summary>
        /// Creates a new <see cref="WiredNet"/>.
        /// </summary>
        /// <param name="name">The name of the net.</param>
        protected WiredNet(string name)
            : base(name, default, EqualityComparer<DriveStrengthRange>.Default)
        {
        }

        /// <inheritdoc />
        protected override void Change(IScheduler scheduler, DriveStrengthRange newValue)
        {
            base.Change(scheduler, newValue);

            // Also deal with the signal value
            var n = Value.Logic;
            if (n != _signal)
            {
                // the value changed, let's notify everyone!
                var nargs = StateChangedEventArgs<Signal>.Create(scheduler, this, _signal);
                _signal = n;
                OnChanged(nargs);
            }
        }

        /// <summary>
        /// Called when the signal changes.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        protected virtual void OnChanged(StateChangedEventArgs<Signal> args)
            => SignalChanged?.Invoke(this, args);
    }
}
