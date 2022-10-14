using StratifiedEventQueue.Events;
using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratifiedEventQueue.Test.Events
{
    public class WaveformEventTests
    {
        [Fact]
        public void When_CreateEvent_Expect_Reference()
        {
            var v = new Variable<int>("A");
            var @event = WaveformEvent<int>.Create(v, new KeyValuePair<ulong, int>[]
            {
                KeyValuePair.Create<ulong, int>(0, 1)
            });

            Assert.Equal(v, @event.Variable);
        }

        [Fact]
        public void When_SimulateWaveform_Expect_Reference()
        {
            var deltas = new ulong[] { 0, 10, 25, 35 };
            var values = new int[] { 4, 3, 2, 1 };
            var v = new Variable<int>("A");
            var points = deltas.Zip(values).Select(a => KeyValuePair.Create(a.First, a.Second));

            var scheduler = new Scheduler();
            scheduler.ScheduleInactive(0, WaveformEvent<int>.Create(v, points));

            ulong time = 0;
            int index = 0;
            void Check(object? sender, ValueChangedEventArgs<int> args)
            {
                if (deltas == null || values == null)
                    throw new ArgumentNullException(nameof(deltas));
                time += deltas[index];
                Assert.Equal(time, args.Scheduler.CurrentTime);
                Assert.Equal(values[index], args.Variable.Value);
                index++;
            }
            v.Changed += Check;

            scheduler.Process();

            Assert.Equal(deltas.Length, index);
        }

        [Fact]
        public void When_SimulateWaveform2_Expect_Reference()
        {
            var values = new int[] { 4, 3, 2, 1 };
            var v = new Variable<int>("A");

            var scheduler = new Scheduler();
            scheduler.ScheduleInactive(0, WaveformEvent<int>.Create(v, 10, values));

            ulong time = 0;
            int index = 0;
            void Check(object? sender, ValueChangedEventArgs<int> args)
            {
                if (values == null)
                    throw new ArgumentNullException(nameof(values));
                Assert.Equal(time, args.Scheduler.CurrentTime);
                Assert.Equal(values[index], args.Variable.Value);
                index++;
                time += 10;
            }
            v.Changed += Check;

            scheduler.Process();
            Assert.Equal(values.Length, index);
        }
    }
}
