using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States;
using System;

namespace StratifiedEventQueue.Events
{
    /// <summary>
    /// An event that keeps bouncing between tik and tok events.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    public class TikTokEvent<T> : Event
    {
        private bool _isTok = false;

        /// <summary>
        /// Gets the variable that will receive the tik tok assignments.
        /// </summary>
        public Variable<T> Variable { get; private set; }

        /// <summary>
        /// Gets the tik value.
        /// </summary>
        public T Tik { get; private set; }

        /// <summary>
        /// Gets the tok value.
        /// </summary>
        public T Tok { get; private set; }

        /// <summary>
        /// Gets the period of the toggling.
        /// </summary>
        public ulong Period { get; private set; }

        /// <summary>
        /// Creates a new <see cref="TikTokEvent{T}"/>.
        /// </summary>
        public TikTokEvent(Variable<T> variable, ulong period, T tik, T tok)
        {
            Variable = variable ?? throw new ArgumentNullException(nameof(variable));
            Period = period;
            Tik = tik;
            Tok = tok;
        }

        /// <inheritdoc />
        public override void Execute(IScheduler scheduler)
        {
            if (_isTok)
                Variable.Update(scheduler, Tok);
            else
                Variable.Update(scheduler, Tik);

            // Schedule ourself in the future
            _isTok = !_isTok;
            scheduler.ScheduleInactive(Period, this);
        }
    }
}
