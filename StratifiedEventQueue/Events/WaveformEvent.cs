using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States;
using System;
using System.Collections.Generic;

namespace StratifiedEventQueue.Events
{
    /// <summary>
    /// An event that automatically keeps generating a new (inactive) event for the next point in the waveform.
    /// </summary>
    /// <typeparam name="T">The value type of the waveform.</typeparam>
    public class WaveformEvent<T> : Event
    {
        private static readonly Queue<WaveformEvent<T>> _eventPool = new Queue<WaveformEvent<T>>(InitialPoolSize);
        private IEnumerator<KeyValuePair<ulong, T>> _enumerator;
        private bool _isFirst;

        /// <summary>
        /// Gets the variable to be set.
        /// </summary>
        public Variable<T> Variable { get; private set; }

        /// <summary>
        /// The initial event pool size.
        /// </summary>
        public const int InitialPoolSize = 20;

        /// <summary>
        /// Creates a new <see cref="WaveformEvent{T}"/>.
        /// </summary>
        private WaveformEvent()
        {
        }

        /// <inheritdoc />
        public override void Execute(IScheduler scheduler)
        {
            if (!_isFirst)
            {
                // We have a point already, so update the variable
                Variable.Update(scheduler, _enumerator.Current.Value);
            }

            if (_enumerator.MoveNext())
            {
                // There is a next point in the enumerable, so go ahead
                // and schedule a new event
                scheduler.ScheduleInactive(_enumerator.Current.Key, Create(Variable, _enumerator));
            }
        }

        /// <inheritdoc />
        public override void Release()
        {
            _eventPool.Enqueue(this);
        }

        /// <summary>
        /// Creates a new <see cref="WaveformEvent{T}"/>, but this can only be used by other waveform events for
        /// creating the next assignment.
        /// </summary>
        /// <param name="variable">The variable.</param>
        /// <param name="enumerator">The enumerator.</param>
        /// <returns>The waveform event.</returns>
        private static WaveformEvent<T> Create(Variable<T> variable, IEnumerator<KeyValuePair<ulong, T>> enumerator)
        {
            WaveformEvent<T> result;
            if (_eventPool.Count > 0)
                result = _eventPool.Dequeue();
            else
                result = new WaveformEvent<T>();
            result.Variable = variable;
            result._enumerator = enumerator;
            result._isFirst = false;
            result.Descheduled = false;
            return result;
        }

        /// <summary>
        /// Creates a new <see cref="WaveformEvent{T}"/>.
        /// </summary>
        /// <param name="variable">The variable.</param>
        /// <param name="points">The points.</param>
        /// <returns>The waveform event.</returns>
        public static WaveformEvent<T> Create(Variable<T> variable, IEnumerable<KeyValuePair<ulong, T>> points)
        {
            WaveformEvent<T> result;
            if (_eventPool.Count > 0)
                result = _eventPool.Dequeue();
            else
                result = new WaveformEvent<T>();
            result.Variable = variable ?? throw new ArgumentNullException(nameof(variable));
            result._enumerator = points?.GetEnumerator() ?? throw new ArgumentNullException(nameof(points)); ;
            result._isFirst = true;
            result.Descheduled = false;
            return result;
        }
    }
}
