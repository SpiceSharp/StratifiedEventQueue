using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States.Gates;
using StratifiedEventQueue.States;

namespace StratifiedEventQueue.Test.States.Gates
{
    public class BufTests
    {
        [Fact]
        public void When_RegularInputs_Expect_Reference()
        {
            var scheduler = new Scheduler();
            var a = new Variable<Signal>("a");

            // Add the AND gate
            var q = new Buf("buf1", "q", a);

            var vA = "01XZ".ToLogic();
            var vQ = "01XX".ToLogic();
            for (int i = 0; i < vA.Length; i++)
            {
                a.Update(scheduler, vA[i]);
                Assert.Equal(vQ[i], q.Value);
            }
        }
    }
}
