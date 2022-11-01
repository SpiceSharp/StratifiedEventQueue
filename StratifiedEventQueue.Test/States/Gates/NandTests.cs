using StratifiedEventQueue.Events;
using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States;
using StratifiedEventQueue.Processes.Gates;
using StratifiedEventQueue.States.Nets;

namespace StratifiedEventQueue.Test.States.Gates
{
    public class NandTests
    {
        [Fact]
        public void When_RegularInputs_Expect_Reference()
        {
            var scheduler = new Scheduler();
            var a = new Variable<Signal>("a");
            var b = new Variable<Signal>("b");
            var q = new Wire("q");

            // Add the NAND gate
            var gate = new Nand("and1", q.Assign(scheduler), a, b);

            var vA = "00001111XXXXZZZZ".ToLogic();
            var vB = "01XZ01XZ01XZ01XZ".ToLogic();
            var vQ = "111110XX1XXX1XXX".ToLogic();
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
