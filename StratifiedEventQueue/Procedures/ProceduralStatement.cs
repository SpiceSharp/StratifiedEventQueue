using StratifiedEventQueue.Processes;
using System;

namespace StratifiedEventQueue.Procedures
{
    /// <summary>
    /// Represents a procedural statement.
    /// </summary>
    public abstract class ProceduralStatement : Process
    {
        /// <summary>
        /// Occurs when the event has executed (and the next statement can be executed).
        /// </summary>
        public event EventHandler<ProceduralStatementEventArgs> Executed;

        /// <summary>
        /// Triggers the procedural statement on a procedural statement event (sequence).
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The arguments.</param>
        public virtual void Trigger(object sender, ProceduralStatementEventArgs args) => Execute(args.Scheduler);

        /// <summary>
        /// Called when the procedural statement has executed and the next statement can be executed.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        protected virtual void OnExecuted(ProceduralStatementEventArgs args)
            => Executed?.Invoke(this, args);
    }
}
