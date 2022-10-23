using StratifiedEventQueue.Events;
using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States;

namespace StratifiedEventQueue.Test.Events
{
    public class AssignmentEventTests
    {
        [Fact]
        public void When_CreateEvent_Expect_Reference()
        {
            var v = new Variable<int>("A");
            var @event = VariableUpdateEvent<int>.Create(v, 4);
            Assert.Equal(4, @event.Value);
            Assert.Equal(v, @event.Variable);
        }

        [Fact]
        public void When_ExecuteEvent_Expect_PoolPresence()
        {
            var scheduler = new Scheduler();

            var va = new Variable<int>("A");
            var vb = new Variable<int>("B");
            var event1 = VariableUpdateEvent<int>.Create(va, 3);
            Assert.Equal(3, event1.Value);
            Assert.Equal(va, event1.Variable);
            event1.Execute(scheduler);

            var event2 = VariableUpdateEvent<int>.Create(vb, 4);
            Assert.Equal(4, event2.Value);
            Assert.Equal(vb, event2.Variable);

            // Because we require a pool, the object should be reused
            Assert.True(ReferenceEquals(event1, event2));
        }
    }
}
