using StratifiedEventQueue.Simulation;
using System;
using System.Collections.Concurrent;

namespace StratifiedEventQueue.Procedures
{
    /// <summary>
    /// Event arguments for when a procedural statement has executed.
    /// </summary>
    public class ProceduralStatementEventArgs : EventArgs
    {
        private static readonly ConcurrentQueue<ProceduralStatementEventArgs> _pool
            = new ConcurrentQueue<ProceduralStatementEventArgs>();

        /// <summary>
        /// Gets the scheduler.
        /// </summary>
        public IScheduler Scheduler { get; private set; }

        /// <summary>
        /// Creates a new <see cref="ProceduralStatementEventArgs"/>.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="scheduler"/> is <c>null</c>.</exception>
        private ProceduralStatementEventArgs()
        {
        }

        /// <summary>
        /// Releases the event arguments, allowing it to be reused at a later time.
        /// </summary>
        public void Release()
        {
            _pool.Enqueue(this);
        }

        /// <summary>
        /// Creates a new <see cref="ProceduralStatementEventArgs"/>, trying to use a pool of reusable
        /// objects.
        /// </summary>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ProceduralStatementEventArgs Create(IScheduler scheduler)
        {
            _pool.TryDequeue(out var result);
            if (result == null)
                result = new ProceduralStatementEventArgs();
            result.Scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
            return result;
        }
    }
}
