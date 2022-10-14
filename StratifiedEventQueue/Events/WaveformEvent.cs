using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StratifiedEventQueue.Events
{
    /// <summary>
    /// An event that automatically keeps generating a new (inactive) event for the next point in the waveform.
    /// </summary>
    /// <typeparam name="T">The value type of the waveform.</typeparam>
    public class WaveformEvent<T> : Event
    {
        private static readonly System.Collections.Generic.Queue<WaveformEvent<T>> _pool 
            = new System.Collections.Generic.Queue<WaveformEvent<T>>(InitialPoolSize);
        private IEnumerator<KeyValuePair<ulong, T>> _enumerator;
        private bool _isFirst;

        /// <summary>
        /// The initial event pool size.
        /// </summary>
        public const int InitialPoolSize = 20;

        /// <summary>
        /// Gets the variable to be set.
        /// </summary>
        public Variable<T> Variable { get; private set; }

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
                // We have already started the waveform!
                Variable.Update(scheduler, _enumerator.Current.Value);
            }

            if (_enumerator.MoveNext())
            {
                // There is a next point in the enumerable, so go ahead
                // and schedule a new event
                scheduler.ScheduleInactive(_enumerator.Current.Key, this);
                _isFirst = false;
            }
            else
            {
                // It is now ok to reuse this event
                _pool.Enqueue(this);
            }
        }

        /// <summary>
        /// Creates a new <see cref="WaveformEvent{T}"/>.
        /// </summary>
        /// <param name="variable">The variable.</param>
        /// <param name="points">The points.</param>
        /// <returns>The waveform event.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="variable"/> or <paramref name="points"/> is <c>null</c>.</exception>
        public static WaveformEvent<T> Create(Variable<T> variable, IEnumerable<KeyValuePair<ulong, T>> points)
        {
            WaveformEvent<T> result;
            if (_pool.Count > 0)
                result = _pool.Dequeue();
            else
                result = new WaveformEvent<T>();
            result.Variable = variable ?? throw new ArgumentNullException(nameof(variable));
            result._enumerator = points?.GetEnumerator() ?? throw new ArgumentNullException(nameof(points)); ;
            result._isFirst = true;
            return result;
        }

        /// <summary>
        /// Creates a new <see cref="WaveformEvent{T}"/>.
        /// </summary>
        /// <param name="variable">The variable.</param>
        /// <param name="period">The period between values.</param>
        /// <param name="values">The values.</param>
        /// <returns>The waveform event.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="variable"/> or <paramref name="values" /> is <c>ull</c>.</exception>
        public static WaveformEvent<T> Create(Variable<T> variable, ulong period, IEnumerable<T> values)
        {
            WaveformEvent<T> result;
            if (_pool.Count > 0)
                result = _pool.Dequeue();
            else
                result = new WaveformEvent<T>();
            result.Variable = variable ?? throw new ArgumentNullException(nameof(variable));
            result._enumerator = values?.Select((v, index) =>
            {
                if (index == 0)
                    return new KeyValuePair<ulong, T>(0, v);
                return new KeyValuePair<ulong, T>(period, v);
            }).GetEnumerator() ?? throw new ArgumentNullException(nameof(values));
            result._isFirst = true;
            return result;
        }
    }
}
