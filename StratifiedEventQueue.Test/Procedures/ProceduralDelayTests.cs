using StratifiedEventQueue.Procedures;
using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States;

namespace StratifiedEventQueue.Test.Procedures
{
    public class ProceduralDelayTests
    {
        [Fact]
        public void When_SimpleDelay_Expect_Reference()
        {
            var scheduler = new Scheduler();
            var a = new Variable<int>("a");

            var delay = new ProceduralDelay(() => 5);
            a.Changed += delay.Trigger;
            delay.Executed += (sender, args) =>
            {
                Assert.Equal((ulong)5, scheduler.CurrentTime);
            };
            a.Update(scheduler, 1);
            scheduler.Process();
            Assert.True(scheduler.CurrentTime > 0);
        }
    }
}
