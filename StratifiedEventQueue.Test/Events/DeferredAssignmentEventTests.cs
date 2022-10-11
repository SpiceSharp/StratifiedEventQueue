using StratifiedEventQueue.Events;
using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States;

namespace StratifiedEventQueue.Test.Events
{
    public class DeferredAssignmentEventTests
    {
        [Fact]
        public void When_CreateEvent_Expect_Reference()
        {
            var f = new Func<int>(() => 4);
            var v = new Variable<int>("A");
            var @event = DeferredAssignmentEvent<int>.Create(v, f);
            Assert.Equal(f, @event.Func);
            Assert.Equal(v, @event.Variable);
        }

        [Fact]
        public void When_ExecuteEvent_Expect_Reference()
        {
            var scheduler = new Scheduler();
            var va = new Variable<int>("A");
            var vb = new Variable<int>("B");
            var fa = new Func<int>(() => 4);
            var fb = new Func<int>(() => 5);

            var event1 = DeferredAssignmentEvent<int>.Create(va, fa);
            Assert.Equal(fa, event1.Func);
            Assert.Equal(va, event1.Variable);
            event1.Execute(scheduler);

            var event2 = DeferredAssignmentEvent<int>.Create(vb, fb);
            Assert.Equal(fb, event2.Func);
            Assert.Equal(vb, event2.Variable);

            // Because we require a pool, the object should be reused
            Assert.True(ReferenceEquals(event1, event2));
        }
    }
}
