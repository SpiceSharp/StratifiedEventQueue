using StratifiedEventQueue.Simulation;

namespace StratifiedEventQueue.States.Nets
{
    /// <summary>
    /// Represents a driver for a <see cref="Net"/>.
    /// </summary>
    public abstract class Driver
    {
        /// <summary>
        /// Updates the driver.
        /// </summary>
        /// <param name="strength"></param>
        public abstract void Update(IScheduler scheduler, DriveStrengthRange strength);
    }
}
