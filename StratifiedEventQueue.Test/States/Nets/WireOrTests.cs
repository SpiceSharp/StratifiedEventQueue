using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States.Nets;
using StratifiedEventQueue.States;

namespace StratifiedEventQueue.Test.States.Nets
{
    public class WireOrTests
    {
        [Fact]
        public void When_SimpleWire_Expect_Reference()
        {
            var scheduler = new Scheduler();
            var a = new Variable<DriveStrengthRange>("a");
            var b = new Variable<DriveStrengthRange>("b");
            var wire = new WireOr("wire");
            wire.Assign(a);
            wire.Assign(b);

            Assert.Equal(new DriveStrengthRange(), wire.Value);
            a.Update(scheduler, new DriveStrengthRange(Strength.St1));
            Assert.Equal(new DriveStrengthRange(Strength.St1), wire.Value);
            b.Update(scheduler, new DriveStrengthRange(Strength.St0));
            Assert.Equal(new DriveStrengthRange(Strength.St1), wire.Value);

            a.Update(scheduler, new DriveStrengthRange(Strength.We1));
            Assert.Equal(new DriveStrengthRange(Strength.St0), wire.Value);
        }
    }
}
