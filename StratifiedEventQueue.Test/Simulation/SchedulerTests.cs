using StratifiedEventQueue.Events;
using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States;

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
            // This example tests regular, inactive event scheduling at various delays before simulation

            int index = 0;
            void CheckTime(IScheduler scheduler)
            {
                Assert.Equal((ulong)(index * 10), scheduler.CurrentTime);
                index++;
            }

            var scheduler = new Scheduler();
            for (int i = 0; i < 10; i++)
            {
                ulong time = (ulong)(i * 10);
                scheduler.ScheduleInactive(time, CallbackEvent.Create(CheckTime));
            }

            scheduler.Process();

            // Make sure the test ran
            Assert.Equal(10, index);
        }

        [Fact]
        public void When_VariableClock_Expect_Reference()
        {
            // This test emulates a clock that keeps running indefinitely.

            var scheduler = new Scheduler();
            var clk = new Variable<bool>("clock");
            clk.Changed += (sender, args) =>
            {
                scheduler.ScheduleInactive(50, AssignmentEvent<bool>.Create(clk, !clk.Value));
            };
            scheduler.MaxTime = 1000;
            scheduler.ScheduleInactive(50, AssignmentEvent<bool>.Create(clk, true));

            bool expected = false;
            void CheckValue(IScheduler scheduler)
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
                scheduler.ScheduleNonBlocking(t, CallbackEvent.Create(CheckValue));
                t += 50;
            }

            scheduler.Process();
            Assert.True(scheduler.CurrentTime >= 1000);
        }

        [Fact]
        public void When_InertialDelay_Expect_Reference()
        {
            // This test demonstrates descheduling, which is necessary when simulating
            // inertial delays
            // We emulate an inverter with a delay in rise and fall time
            var input = new LogicVariable("IN");
            var output = new LogicVariable("OUT");
            var scheduler = new Scheduler();

            // We first create a waveform to test our inertial delay
            CreateWaveform(scheduler, input, new ulong[] { 0, 1, 2, 4, 6, 7, 8, 11 }, Logic.FromStringBinary("01010101"));

            // Now let us create an inertial delay inverter
            EventNode? nextEvent = null;
            ulong nextEventTime = 0;
            void InertialDelayInverter(object? sender, ValueChangedEventArgs<byte> args)
            {
                byte value = Logic.Not(args.Variable.Value);
                ulong delay = args.Variable.Value switch
                {
                    Logic.L => 2,
                    Logic.H => 1,
                    _ => 0
                };

                ulong nextTime = args.Scheduler.CurrentTime + delay;
                if (nextTime <= nextEventTime && nextEvent != null)
                {
                    // This event happens before/at the same time as the next
                    // This means that this event will happen instead of the
                    // already scheduled event
                    nextEvent.Deschedule();
                }
                var @event = AssignmentEvent<byte>.Create(output, value);
                nextEventTime = nextTime;
                nextEvent = scheduler.ScheduleInactive(delay, @event);
            }
            input.Changed += InertialDelayInverter;

            // We will then test for a specific waveform
            int index = 0;
            var expectedTime = new ulong[] { 2, 4, 5, 10, 12 };
            var expectedValues = Logic.FromStringBinary("01010");
            void CheckWaveform(object? sender, ValueChangedEventArgs<byte> args)
            {
                Assert.Equal(expectedTime[index], args.Scheduler.CurrentTime);
                Assert.Equal(expectedValues[index], args.Variable.Value);
                index++;
            }
            output.Changed += CheckWaveform;
            scheduler.Process();

            // This double-checks that the whole waveform is processed
            Assert.Equal(index, expectedTime.Length);
        }

        [Fact]
        public void When_EnabledTflipflop_Expect_Reference()
        {
            // This example demonstrates an enabled T-flip-flop
            var scheduler = new Scheduler();
            var clk = new LogicVariable("clk", Logic.L);
            var output = new LogicVariable("out", Logic.L);

            // Starts the clock
            scheduler.ScheduleInactive(5, AssignmentEvent<byte>.Create(clk, Logic.H));

            // Keeps the clock going
            clk.Changed += (sender, args) =>
            {
                // Invert the clock
                args.Scheduler.ScheduleInactive(5, AssignmentEvent<byte>.Create(clk, Logic.Not(clk.Value)));
            };

            // The T-flip-flop definition, with only sensitivity to the clock
            clk.Changed += (sender, args) =>
            {
                if (clk.OldValue == Logic.L && clk.Value == Logic.H)
                {
                    // Posedge reached, toggle the output
                    args.Scheduler.ScheduleInactive(0, AssignmentEvent<byte>.Create(output, Logic.Not(output.Value)));
                }
            };

            // Checking the output
            int index = 0;
            void Check(object? sender, ValueChangedEventArgs<byte> args)
            {
                Assert.Equal((ulong)(5 + index * 10), args.Scheduler.CurrentTime);
                if ((index % 2) == 0)
                    Assert.Equal(Logic.H, output.Value);
                else
                    Assert.Equal(Logic.L, output.Value);
                index++;
            }
            output.Changed += Check;

            scheduler.MaxTime = 1000;
            scheduler.Process();
            Assert.Equal(100, index); // There should be 100 toggles in total
        }

        [Fact]
        public void When_FlipFlopAsyncReset_Expect_Reference()
        {
            // This example emulates a flip-flop with asynchronous reset
            var scheduler = new Scheduler();
            var d = new LogicVariable("d");
            var clk = new LogicVariable("clk");
            var rst = new LogicVariable("rst");
            var q = new LogicVariable("q");

            // Define the data
            scheduler.ScheduleInactive(0, WaveformEvent<byte>.Create(d, 10, new byte[] { Logic.L, Logic.H }));

            // Define the clock
            scheduler.ScheduleInactive(0, new TikTokEvent<byte>(clk, 5, Logic.L, Logic.H));

            // Define the reset
            scheduler.ScheduleInactive(0, WaveformEvent<byte>.Create(rst, 22, new byte[] { Logic.L, Logic.H }));

            // Define the working of the flip-flop
            void Dflipflop(IScheduler scheduler)
            {
                if (rst.Value == Logic.H)
                    scheduler.Schedule(AssignmentEvent<byte>.Create(q, Logic.L));
                else if (clk.Value == Logic.H && clk.OldValue == Logic.L && clk.ChangeTime == scheduler.CurrentTime)
                    scheduler.Schedule(AssignmentEvent<byte>.Create(q, d.Value));
            }
            clk.Changed += (sender, args) => Dflipflop(args.Scheduler);
            rst.Changed += (sender, args) => Dflipflop(args.Scheduler);

            // Check the output
            var times = new ulong[] { 5, 15, 22  };
            var values = new byte[] { Logic.L, Logic.H, Logic.L };
            int index = 0;
            q.Changed += (sender, args) =>
            {
                Assert.Equal(times[index], args.Scheduler.CurrentTime);
                Assert.Equal(values[index], args.Variable.Value);
                index++;
            };

            // Execute
            scheduler.MaxTime = 50;
            scheduler.Process();

            // Make sure we have finished
            Assert.Equal(times.Length, index);
        }

        private void CreateWaveform<T>(Scheduler scheduler, Variable<T> variable, ulong[] time, T[] values)
        {
            for (int i = 0; i < time.Length; i++)
            {
                scheduler.ScheduleInactive(time[i], AssignmentEvent<T>.Create(variable, values[i]));
            }
        }
    }
}