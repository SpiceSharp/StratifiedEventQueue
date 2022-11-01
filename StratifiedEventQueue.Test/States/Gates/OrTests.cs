using StratifiedEventQueue.Events;
using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.Processes.Gates;
using StratifiedEventQueue.States;
using StratifiedEventQueue.States.Nets;

namespace StratifiedEventQueue.Test.States.Gates
{
    public class OrTests
    {
        [Fact]
        public void When_RegularInputs_Expect_Reference()
        {
            var scheduler = new Scheduler();
            var a = new Variable<Signal>("a");
            var b = new Variable<Signal>("b");
            var q = new Wire("q");

            // Add the OR gate
            var gate = new Or("and1", q.Assign(scheduler), a, b);

            var vA = "00001111XXXXZZZZ".ToLogic();
            var vB = "01XZ01XZ01XZ01XZ".ToLogic();
            var vQ = "01XX1111X1XXX1XX".ToLogic();
            for (int i = 0; i < vA.Length; i++)
            {
                a.Update(scheduler, vA[i]);
                b.Update(scheduler, vB[i]);
                scheduler.Process();
                Assert.Equal(vQ[i], q.Value.Logic);
            }
        }
    }
}
