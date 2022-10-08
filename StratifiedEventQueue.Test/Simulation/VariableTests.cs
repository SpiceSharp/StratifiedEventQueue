using StratifiedEventQueue.Simulation;

namespace StratifiedEventQueue.Test.Simulation
{
    public class VariableTests
    {
        [Fact]
        public void When_ValueChange_Expect_Event()
        {
            var v = new Variable<bool>("A");
            bool changed = false;
            v.Changed += (sender, args) => changed = true;
            v.Update(null, true);

            Assert.True(changed);
        }
    }
}
