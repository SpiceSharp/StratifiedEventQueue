using StratifiedEventQueue.Events;
using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States;
using StratifiedEventQueue.States.Gates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratifiedEventQueue.Test.States.Gates
{
    public class AndTests
    {
        [Fact]
        public void When_RegularInputs_Expect_Reference()
        {
            var scheduler = new Scheduler();
            var a = new Variable<Signal>("a");
            a.Update(scheduler, Signal.L);
            var b = new Variable<Signal>("b");
            b.Update(scheduler, Signal.L);

            // Apply excitations
            scheduler.ScheduleInactive(0, WaveformEvent<Signal>.Create(a, 10, "01100110".ToLogic()));
            scheduler.ScheduleInactive(0, WaveformEvent<Signal>.Create(b, 10, "00110011".ToLogic()));

            // Add the AND gate
            var q = new And("and1", "q", a, b);

            int index = 0;
            var expectedChanges = new ulong[] { 20, 30, 60, 70 };
            Signal expectedResult = Signal.H;
            q.Changed += (sender, args) => {
                Assert.Equal(expectedChanges[index], scheduler.CurrentTime);
                Assert.Equal(expectedResult, args.State.Value);
                expectedResult = expectedResult == Signal.L ? Signal.H : Signal.L;
                index++;
            };

            scheduler.Process();
            Assert.Equal(expectedChanges.Length, index);
        }
    }
}
