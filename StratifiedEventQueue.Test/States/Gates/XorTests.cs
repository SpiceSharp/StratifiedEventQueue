using StratifiedEventQueue.Events;
using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States;
using StratifiedEventQueue.States.Gates;

namespace StratifiedEventQueue.Test.States.Gates
{
    public class XorTests
    {
        [Fact]
        public void When_RegularInputs_Expect_Reference()
        {
            var scheduler = new Scheduler();
            var a = new Variable<Signal>("a");
            var b = new Variable<Signal>("b");

            // Add the AND gate
            var q = new Xor("xor1", "q", a, b);

            var vA = "00001111XXXXZZZZ".ToLogic();
            var vB = "01XZ01XZ01XZ01XZ".ToLogic();
            var vQ = "01XX10XXXXXXXXXX".ToLogic();
            for (int i = 0; i < vA.Length; i++)
            {
                a.Update(scheduler, vA[i]);
                b.Update(scheduler, vB[i]);
                Assert.Equal(vQ[i], q.Value);
            }
        }
    }
}
