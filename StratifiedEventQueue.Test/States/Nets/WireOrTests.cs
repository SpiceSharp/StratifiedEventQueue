using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States.Nets;

namespace StratifiedEventQueue.Test.States.Nets
{
    public class WireOrTests
    {
        [Fact]
        public void When_SimpleWire_Expect_Reference()
        {
            var scheduler = new Scheduler();
            var wire = new WireOr("wire");
            var driverA = wire.Assign(scheduler);
            var driverB = wire.Assign(scheduler);

            Assert.Equal(new DriveStrengthRange(), wire.Value);

            driverA.Update(scheduler, new DriveStrengthRange(Strength.St1));
            scheduler.Process();
            Assert.Equal(new DriveStrengthRange(Strength.St1), wire.Value);
            
            driverB.Update(scheduler, new DriveStrengthRange(Strength.St0));
            scheduler.Process();
            Assert.Equal(new DriveStrengthRange(Strength.St1), wire.Value);

            driverA.Update(scheduler, new DriveStrengthRange(Strength.We1));
            scheduler.Process();
            Assert.Equal(new DriveStrengthRange(Strength.St0), wire.Value);
        }
    }
}
