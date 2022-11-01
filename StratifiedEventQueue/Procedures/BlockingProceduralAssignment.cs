using StratifiedEventQueue.Events;
using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States;
using System;
using System.Collections.Concurrent;

namespace StratifiedEventQueue.Procedures
{
    /// <summary>
    /// Represents a blocking procedural assignment.
    /// </summary>
    public class BlockingProceduralAssignment<T> : ProceduralStatement
    {
        private static readonly ConcurrentQueue<UpdateEvent> _pool = new ConcurrentQueue<UpdateEvent>();

        protected class UpdateEvent : Event
        {
            public BlockingProceduralAssignment<T> Parent { get; set; }
            public T Value { get; set; }
            public override void Execute(IScheduler scheduler)
            {
                Parent.Variable.Update(scheduler, Value);

                // Pass control to the next event
                var args = ProceduralStatementEventArgs.Create(scheduler);
                Parent.OnExecuted(args);
                args.Release();
                _pool.Enqueue(this);
            }
        }

        /// <summary>
        /// Gets the variable that is being assigned to.
        /// </summary>
        public Variable<T> Variable { get; }

        /// <summary>
        /// Gets the delay until the block procedural assignment can stop.
        /// </summary>
        public Func<uint> IntraAssignmentDelay { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public Func<T> Value { get; }

        /// <summary>
        /// Creates a new <see cref="BlockingProceduralAssignment{T}"/>.
        /// </summary>
        /// <param name="variable">The variable.</param>
        /// <param name="intraAssignmentDelay">The intra-assignment delay.</param>
        /// <param name="value">The value.</param>
        public BlockingProceduralAssignment(Variable<T> variable, Func<T> value, Func<uint> intraAssignmentDelay = null)
        {
            Variable = variable ?? throw new ArgumentNullException(nameof(variable));
            IntraAssignmentDelay = intraAssignmentDelay;
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <inheritdoc />
        protected override void Execute(IScheduler scheduler)
        {
            // The value should be evaluated immediately
            var value = Value();

            if (IntraAssignmentDelay == null)
            {
                // Just assign the variable immediately
                Variable.Update(scheduler, value);

                // Notify having finished
                var args = ProceduralStatementEventArgs.Create(scheduler);
                OnExecuted(args);
                args.Release();
            }
            else
            {
                uint delay = IntraAssignmentDelay();

                // Schedule an event as an inactive event
                var @event = CreateEvent();
                @event.Value = value;
                scheduler.ScheduleInactive(delay, @event);
            }
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
