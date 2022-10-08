using StratifiedEventQueue.Events;
using StratifiedEventQueue.Simulation;

namespace StratifiedEventQueue.Test.Simulation
{
    /// <summary>
    /// Tests involving the scheduler.
    /// </summary>
    public class SchedulerTests
    {
        [Fact]
        public void When_InactiveScheduled_Expect_ReferenceTime()
        {
            int index = 0;
            void CheckTime(Scheduler scheduler)
            {
                Assert.Equal((ulong)(index * 10), scheduler.CurrentTime);
                index++;
            }

            var scheduler = new Scheduler();
            for (int i = 0; i < 10; i++)
            {
                ulong time = (ulong)(i * 10);
                scheduler.ScheduleInactive(time, new CallbackEvent(CheckTime));
            }

            scheduler.Process();
            Assert.Equal(10, index);
        }

        [Fact]
        public void When_VariableClock_Expect_Reference()
        {
            var scheduler = new Scheduler();
            var clk = new Variable<bool>("clock");
            clk.Changed += (sender, args) =>
            {
                scheduler.ScheduleInactive(50, AssignmentEvent<bool>.Create(clk, !clk.Value));
            };
            scheduler.MaxTime = 1000;
            scheduler.ScheduleInactive(50, AssignmentEvent<bool>.Create(clk, true));

            bool expected = false;
            void CheckValue(Scheduler scheduler)
            {
                if (clk != null)
                {
                    Assert.Equal(expected, clk.Value);
                    expected = !expected;
                }
            }

            ulong t = 0;
            while (t < 1000)
            {
                // We just want to check 
                scheduler.ScheduleNonBlocking(t, new CallbackEvent(CheckValue));
                t += 50;
            }

            scheduler.Process();
            Assert.True(scheduler.CurrentTime >= 1000);
        }
    }
}