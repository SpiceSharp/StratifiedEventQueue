using StratifiedEventQueue.Simulation;
using System;
using System.Collections.Generic;

namespace StratifiedEventQueue.States.Nets
{
    /// <summary>
    /// A wire.
    /// </summary>
    public class Wire : Net<DriveStrengthRange>, IState<Signal>
    {
        private Signal _signal;
        private event EventHandler<StateChangedEventArgs<Signal>> _signalChanged;

        /// <inheritdoc />
        Signal IState<Signal>.Value => _signal;

        /// <inheritdoc />
        event EventHandler<StateChangedEventArgs<Signal>> IState<Signal>.Changed
        {
            add { _signalChanged += value; }
            remove { _signalChanged -= value; }
        }

        /// <inheritdoc />
        IEqualityComparer<Signal> IState<Signal>.Comparer => EqualityComparer<Signal>.Default;

        /// <summary>
        /// Creates a new <see cref="Wire"/>.
        /// </summary>
        /// <param name="name">The name of the wire.</param>
        public Wire(string name)
            : base(name)
        {
        }

        /// <inheritdoc />
        protected override void Change(IScheduler scheduler, DriveStrengthRange newValue)
        {
            base.Change(scheduler, newValue);

            var n = Value.Logic;
            if (n != _signal)
            {
                var args = StateChangedEventArgs<Signal>.Create(scheduler, this, _signal);
                _signal = n;
                _signalChanged?.Invoke(this, args);
                args.Release();
            }
        }

        /// <inheritdoc />
        protected override DriveStrengthRange Combine(IReadOnlyList<DriveStrengthRange> inputs)
        {
            if (inputs.Count == 0)
                return default;

            // Compute the combined result
            var result = inputs[0];
            for (int i = 1; i < inputs.Count; i++)
                result = DriveStrengthRange.Wired(result, inputs[i]);
            return result;
        }

        /// <summary>
        /// Called when the drivestrength range changes.
        /// </summary>
        /// <param name="args">The argument.</param>
        protected virtual void OnChanged(StateChangedEventArgs<Signal> args)
            => _signalChanged?.Invoke(this, args);
    }
}
