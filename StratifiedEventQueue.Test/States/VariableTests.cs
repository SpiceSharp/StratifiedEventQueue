using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States;

namespace StratifiedEventQueue.Test.States
{
    public class VariableTests
    {
        [Fact]
        public void When_ValueChange_Expect_Event()
        {
            var scheduler = new Scheduler();
            var v = new Variable<bool>("A");
            bool changed = false;
            v.Changed += (sender, args) => changed = true;
            v.Update(scheduler, true);

            Assert.True(changed);
        }
    }
}
