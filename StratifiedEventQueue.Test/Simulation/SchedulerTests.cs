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
            void CheckTime(Scheduler scheduler)
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
            var input = new Variable<byte>("IN");
            var output = new Variable<byte>("OUT");
            var scheduler = new Scheduler();

            // We first create a waveform to test our inertial delay
            CreateWaveform(scheduler, input, new ulong[] { 0, 1, 2, 4, 6, 7, 8, 11 }, SignalValue.FromStringBinary("01010101"));

            // Now let us create an inertial delay inverter
            Event? nextEvent = null;
            ulong nextEventTime = 0;
            void InertialDelayInverter(object? sender, VariableValueChangedEventArgs<byte> args)
            {
                ulong delay;
                byte value;
                if (args.Variable.Value == SignalValue.L)
                {
                    // Rise time, 2 ticks
                    delay = 2;
                    value = SignalValue.H;
                }
                else if (args.Variable.Value == SignalValue.H)
                {
                    // Fall time, 1 tick
                    delay = 1;
                    value = SignalValue.L;
                }
                else
                {
                    delay = 0;
                    value = SignalValue.X;
                }

                ulong nextTime = args.Scheduler.CurrentTime + delay;
                if (nextTime <= nextEventTime && nextEvent != null)
                {
                    // This event happens before/at the same time as the next
                    // This means that this event will happen instead of the
                    // already scheduled event
                    nextEvent.Descheduled = true;
                }
                nextEvent = AssignmentEvent<byte>.Create(output, value);
                nextEventTime = nextTime;
                scheduler.ScheduleInactive(delay, nextEvent);
            }
            input.Changed += InertialDelayInverter;

            // We will then test for a specific waveform
            int index = 0;
            var expectedTime = new ulong[] { 2, 4, 5, 10, 12 };
            var expectedValues = SignalValue.FromStringBinary("01010");
            void CheckWaveform(object? sender, VariableValueChangedEventArgs<byte> args)
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

        private void CreateWaveform<T>(Scheduler scheduler, Variable<T> variable, ulong[] time, T[] values)
        {
            for (int i = 0; i < time.Length; i++)
            {
                scheduler.ScheduleInactive(time[i], AssignmentEvent<T>.Create(variable, values[i]));
            }
        }
    }
}