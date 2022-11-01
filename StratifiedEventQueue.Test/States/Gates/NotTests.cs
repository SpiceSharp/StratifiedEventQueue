using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.Processes.Gates;
using StratifiedEventQueue.States;
using StratifiedEventQueue.States.Nets;

namespace StratifiedEventQueue.Test.States.Gates
{
    public class NotTests
    {
        [Fact]
        public void When_RegularInputs_Expect_Reference()
        {
            var scheduler = new Scheduler();
            var a = new Variable<Signal>("a");
            var q = new Wire("q");

            // Add the AND gate
            var gate = new Not("not1", q.Assign(scheduler), a);

            var vA = "01XZ".ToLogic();
            var vQ = "10XX".ToLogic();
            for (int i = 0; i < vA.Length; i++)
            {
                a.Update(scheduler, vA[i]);
                scheduler.Process();
                Assert.Equal(vQ[i], q.Value.Logic);
            }
        }
    }
}
