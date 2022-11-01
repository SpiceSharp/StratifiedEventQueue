using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States.Nets;
using System;
using System.Collections.Concurrent;

namespace StratifiedEventQueue.Events
{
    /// <summary>
    /// An update event for a net driver.
    /// </summary>
    public class DriverUpdateEvent : Event
    {
        private readonly static ConcurrentQueue<DriverUpdateEvent> _pool
            = new ConcurrentQueue<DriverUpdateEvent>();

        /// <summary>
        /// Gets the driver handle.
        /// </summary>
        public Driver Driver { get; private set; }

        /// <summary>
        /// Gets the value to update the driver to.
        /// </summary>
        public DriveStrengthRange Value { get; private set; }

        /// <inheritdoc />
        public override void Execute(IScheduler scheduler)
        {
            Driver.Update(scheduler, Value);
            _pool.Enqueue(this);
        }

        /// <summary>
        /// Creates a new <see cref="DriverUpdateEvent"/>.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="value">The value.</param>
        /// <returns>The driver update event.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="driver"/> is <c>null</c>.</exception>
        public static DriverUpdateEvent Create(Driver driver, DriveStrengthRange value)
        {
            _pool.TryDequeue(out var result);
            if (result == null)
                result = new DriverUpdateEvent();
            result.Driver = driver ?? throw new ArgumentNullException(nameof(driver));
            result.Value = value;
            return result;
        }
    }
}
