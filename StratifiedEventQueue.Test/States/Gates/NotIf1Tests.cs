using StratifiedEventQueue.Simulation;
using StratifiedEventQueue.States;
using StratifiedEventQueue.States.Gates;
using StratifiedEventQueue.States.Nets;

namespace StratifiedEventQueue.Test.States.Gates
{
    public class NotIf1Tests
    {
        [Fact]
        public void When_RegularInputs_Expect_Reference()
        {
            var scheduler = new Scheduler();
            var a = new Variable<Signal>("a");
            var b = new Variable<Signal>("b");
            var q = new Wire("q");

            // Add the bufif0 gate
            var gate = new NotIf1("and1", q.Assign(scheduler), a, b);

            var vA = "00001111XXXXZZZZ".ToLogic();
            var vB = "01XZ01XZ01XZ01XZ".ToLogic();
            var vQ = new[]
            {
                new DriveStrengthRange(Strength.None, Strength.None),
                new DriveStrengthRange(Strength.St1),
                new DriveStrengthRange(Strength.St1, Strength.None),
                new DriveStrengthRange(Strength.St1, Strength.None),

                new DriveStrengthRange(Strength.None, Strength.None),
                new DriveStrengthRange(Strength.St0),
                new DriveStrengthRange(Strength.None, Strength.St0),
                new DriveStrengthRange(Strength.None, Strength.St0),

                new DriveStrengthRange(Strength.None, Strength.None),
                new DriveStrengthRange(Strength.St0, Strength.St1),
                new DriveStrengthRange(Strength.St0, Strength.St1),
                new DriveStrengthRange(Strength.St0, Strength.St1),

                new DriveStrengthRange(Strength.None, Strength.None),
                new DriveStrengthRange(Strength.St0, Strength.St1),
                new DriveStrengthRange(Strength.St0, Strength.St1),
                new DriveStrengthRange(Strength.St0, Strength.St1),
            };
            for (int i = 0; i < vA.Length; i++)
            {
                a.Update(scheduler, vA[i]);
                b.Update(scheduler, vB[i]);
                scheduler.Process();
                Assert.Equal(vQ[i], q.Value);
            }
        }
    }
}
