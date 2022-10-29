using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States;
using StratifiedEventQueue.States.Nets;

namespace StratifiedEventQueue.Test.States.Nets
{
    public class WireTests
    {
        [Fact]
        public void When_SimpleWire_Expect_Reference()
        {
            var scheduler = new Scheduler();
            var a = new Variable<DriveStrengthRange>("a");
            var b = new Variable<DriveStrengthRange>("b");
            var wire = new Wire("wire");
            wire.Assign(a);
            wire.Assign(b);

            Assert.Equal(new DriveStrengthRange(), wire.Value);
            a.Update(scheduler, new DriveStrengthRange(Strength.St1));
            Assert.Equal(new DriveStrengthRange(Strength.St1), wire.Value);
            b.Update(scheduler, new DriveStrengthRange(Strength.St0));
            Assert.Equal(new DriveStrengthRange(Strength.St0, Strength.St1), wire.Value);

            a.Update(scheduler, new DriveStrengthRange(Strength.We1));
            Assert.Equal(new DriveStrengthRange(Strength.St0), wire.Value);
        }
    }
}
