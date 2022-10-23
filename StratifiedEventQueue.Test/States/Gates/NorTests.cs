using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States.Gates;
using StratifiedEventQueue.States;

namespace StratifiedEventQueue.Test.States.Gates
{
    public class NorTests
    {
        [Fact]
        public void When_RegularInputs_Expect_Reference()
        {
            var scheduler = new Scheduler();
            var a = new Variable<Signal>("a");
            var b = new Variable<Signal>("b");

            // Add the NOR gate
            var q = new Nor("and1", "q", a, b);

            var vA = "00001111XXXXZZZZ".ToLogic();
            var vB = "01XZ01XZ01XZ01XZ".ToLogic();
            var vQ = "10XX0000X0XXX0XX".ToLogic();
            for (int i = 0; i < vA.Length; i++)
            {
                a.Update(scheduler, vA[i]);
                b.Update(scheduler, vB[i]);
                Assert.Equal(vQ[i], q.Value);
            }
        }
    }
}
