using StratifiedEventQueue.Events;
using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States;
using System;
using System.Collections.Concurrent;

namespace StratifiedEventQueue.Procedures
{
    public class NonBlockingProceduralAssignment<T> : ProceduralStatement
    {
        private static readonly ConcurrentQueue<UpdateEvent> _pool =
            new ConcurrentQueue<UpdateEvent>();

        protected class UpdateEvent : Event
        {
            public NonBlockingProceduralAssignment<T> Parent { get; set; }
            public T Value { get; set; }
            public override void Execute(IScheduler scheduler)
            {
                Parent.Variable.Update(scheduler, Value);
                _pool.Enqueue(this);
            }
        }

        /// <summary>
        /// Gets the variable.
        /// </summary>
        public Variable<T> Variable { get; }

        /// <summary>
        /// Gets the delay until the non-blocking procedural can stop.
        /// </summary>
        public Func<uint> IntraAssignmentDelay { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public Func<T> Value { get; }

        /// <summary>
        /// Creates a new <see cref="NonBlockingProceduralAssignment{T}"/>.
        /// </summary>
        /// <param name="variable">The variable.</param>
        /// <param name="value">The value.</param>
        /// <param name="intraAssignmentDelay">The intra-assignment delay.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="variable"/> or <paramref name="value"/> is <c>null</c>.</exception>
        public NonBlockingProceduralAssignment(Variable<T> variable, Func<T> value, Func<uint> intraAssignmentDelay = null)
        {
            Variable = variable ?? throw new ArgumentNullException(nameof(variable));
            IntraAssignmentDelay = intraAssignmentDelay;
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <inheritdoc />
        public override void Execute(IScheduler scheduler)
        {
            var delay = IntraAssignmentDelay?.Invoke() ?? 0;
            var @event = CreateEvent();
            @event.Value = Value();
            scheduler.ScheduleNonBlocking(delay, @event);

            // The next procedural statement can already start
            var args = ProceduralStatementEventArgs.Create(scheduler);
            OnExecuted(args);
            args.Release();
        }

        /// <summary>
        /// Creates a new update event.
        /// </summary>
        /// <returns>The update event.</returns>
        protected UpdateEvent CreateEvent()
        {
            _pool.TryDequeue(out var result);
            if (result == null)
                result = new UpdateEvent();
            result.Parent = this;
            return result;
        }
    }
}
