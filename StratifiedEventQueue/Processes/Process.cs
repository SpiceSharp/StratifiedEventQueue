using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States;

namespace StratifiedEventQueue.Processes
{
    /// <summary>
    /// Describes a process.
    /// </summary>
    public abstract class Process
    {
        /// <summary>
        /// The method that can be used to trigger the process.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The event arguments.</param>
        public virtual void Trigger<T>(object sender, StateChangedEventArgs<T> args)
            => Execute(args.Scheduler);

        /// <summary>
        /// Executes the process.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        protected abstract void Execute(IScheduler scheduler);
    }
}
