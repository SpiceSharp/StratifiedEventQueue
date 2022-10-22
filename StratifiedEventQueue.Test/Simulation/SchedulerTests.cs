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
            var input = new Variable<Signal>("IN");
            var output = new Variable<Signal>("OUT");
            var scheduler = new Scheduler();

            // We first create a waveform to test our inertial delay
            var waveform = WaveformEvent<Signal>.Create(input, new ulong[] { 0, 1, 1, 2, 2, 1, 1, 3 }.Zip("01010101".ToLogic()).Select(a => KeyValuePair.Create(a.First, a.Second)));
            scheduler.ScheduleInactive(0, waveform);

            // Now let us create an inertial delay inverter
            EventNode? nextEvent = null;
            ulong nextEventTime = 0;
            void InertialDelayInverter(object? sender, StateChangedEventArgs<Signal> args)
            {
                var value = LogicHelper.Not(args.State.Value);
                ulong delay = args.State.Value switch
                {
                    Signal.L => 2,
                    Signal.H => 1,
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
                var @event = AssignmentEvent<Signal>.Create(output, value);
                nextEventTime = nextTime;
                nextEvent = scheduler.ScheduleInactive(delay, @event);
            }
            input.Changed += InertialDelayInverter;

            // We will then test for a specific waveform
            int index = 0;
            var expectedTime = new ulong[] { 2, 4, 5, 10, 12 };
            var expectedValues = "01010".ToLogic();
            void CheckWaveform(object? sender, StateChangedEventArgs<Signal> args)
            {
                Assert.Equal(expectedTime[index], args.Scheduler.CurrentTime);
                Assert.Equal(expectedValues[index], args.State.Value);
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
            var clk = new Variable<Signal>("clk", Signal.L);
            var output = new Variable<Signal>("out", Signal.L);

            // Starts the clock
            scheduler.ScheduleInactive(5, AssignmentEvent<Signal>.Create(clk, Signal.H));

            // Keeps the clock going
            clk.Changed += (sender, args) =>
            {
                // Invert the clock
                args.Scheduler.ScheduleInactive(5, AssignmentEvent<Signal>.Create(clk, LogicHelper.Not(clk.Value)));
            };

            // The T-flip-flop definition, with only sensitivity to the clock
            clk.Changed += (sender, args) =>
            {
                if (args.OldValue == Signal.L && args.State.Value == Signal.H)
                {
                    // Posedge reached, toggle the output
                    args.Scheduler.ScheduleInactive(0, AssignmentEvent<Signal>.Create(output, LogicHelper.Not(output.Value)));
                }
            };

            // Checking the output
            int index = 0;
            void Check(object? sender, StateChangedEventArgs<Signal> args)
            {
                Assert.Equal((ulong)(5 + index * 10), args.Scheduler.CurrentTime);
                if ((index % 2) == 0)
                    Assert.Equal(Signal.H, output.Value);
                else
                    Assert.Equal(Signal.L, output.Value);
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
            var d = new Variable<Signal>("d");
            var clk = new Variable<Signal>("clk");
            var rst = new Variable<Signal>("rst");
            var q = new Variable<Signal>("q");

            // Define the data
            scheduler.ScheduleInactive(0, WaveformEvent<Signal>.Create(d, 10, new Signal[] { Signal.L, Signal.H }));

            // Define the clock
            scheduler.ScheduleInactive(0, new TikTokEvent<Signal>(clk, 5, Signal.L, Signal.H));

            // Define the reset
            scheduler.ScheduleInactive(0, WaveformEvent<Signal>.Create(rst, 22, new Signal[] { Signal.L, Signal.H }));

            // Define the working of the flip-flop
            clk.Changed += (sender, args) =>
            {
                // The part sensitive to the clock
                if (rst.Value == Signal.H)
                    q.Update(args.Scheduler, Signal.L);
                else if (args.State.Value == Signal.H && args.OldValue == Signal.L)
                    q.Update(args.Scheduler, d.Value);
            };
            rst.Changed += (sender, args) =>
            {
                // The part sensitive to the reset
                if (rst.Value == Signal.H)
                    q.Update(args.Scheduler, Signal.L);
            };

            // Check the output
            var times = new ulong[] { 5, 15, 22  };
            var values = new Signal[] { Signal.L, Signal.H, Signal.L };
            int index = 0;
            q.Changed += (sender, args) =>
            {
                Assert.Equal(times[index], args.Scheduler.CurrentTime);
                Assert.Equal(values[index], args.State.Value);
                index++;
            };

            // Execute
            scheduler.MaxTime = 50;
            scheduler.Process();

            // Make sure we have finished
            Assert.Equal(times.Length, index);
        }
    }
}