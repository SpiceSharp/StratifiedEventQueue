using StratifiedEventQueue.Procedures;
using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States;

namespace StratifiedEventQueue.Test.Procedures
{
    public class BlockingProceduralAssignmentTests
    {
        [Fact]
        public void When_SimpleBlockingProcedural_Expect_Reference()
        {
            // a = b, triggered by b
            var scheduler = new Scheduler();
            var a = new Variable<int>("a");
            var b = new Variable<int>("b");
            var value = new Func<int>(() => b.Value);
            var assignment = new BlockingProceduralAssignment<int>(a, value);
            b.Changed += assignment.Trigger;

            b.Update(scheduler, 0);
            Assert.Equal(0, a.Value);
            b.Update(scheduler, 1);
            Assert.Equal(1, b.Value);
        }

        [Fact]
        public void When_IntraAssignmentDelay_Expect_Reference()
        {
            // b = #5 a, triggered by b
            // c = b
            // Since this is a blocking assignment, c should become a after 5 cycles
            var scheduler = new Scheduler();
            var a = new Variable<int>("a");
            var b = new Variable<int>("b");
            var c = new Variable<int>("c");
            var assignment = new BlockingProceduralAssignment<int>(b, () => a.Value, () => 5);
            var assignment2 = new BlockingProceduralAssignment<int>(c, () => b.Value);
            assignment.Executed += assignment2.Trigger;
            a.Changed += assignment.Trigger;

            a.Update(scheduler, 1);
            Assert.Equal(0, b.Value); // b should not have changed yet because the assignment has intra-assignment delay
            Assert.Equal(0, c.Value); // c should not have changed yet because the assignment is blocking

            // Do checks
            b.Changed += (sender, args) =>
            {
                Assert.Equal((ulong)5, scheduler.CurrentTime); // b should change on time step 5
                Assert.Equal(1, b.Value);
                Assert.Equal(0, c.Value); // c should not have changed yet because the assignment is blocking
            };
            c.Changed += (sender, args) =>
            {
                Assert.Equal((ulong)5, scheduler.CurrentTime);
                Assert.Equal(1, c.Value);
            };
            scheduler.Process();
            Assert.True(scheduler.CurrentTime > 0);
        }
    }
}
