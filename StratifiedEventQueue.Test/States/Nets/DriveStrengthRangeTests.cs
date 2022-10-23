using StratifiedEventQueue.States;
using StratifiedEventQueue.States.Nets;

namespace StratifiedEventQueue.Test.States.Nets
{
    public class DriveStrengthRangeTests
    {
        [Theory]
        [MemberData(nameof(Tests))]
        public void When_DriveStrengthRange_Expect_Reference(DriveStrengthRange range, string result)
        {
            Assert.Equal(result, range.ToString());
        }

        [Theory]
        [MemberData(nameof(WiredAndTests))]
        public void When_WiredAnd_Expect_Reference(Strength a, Strength b, Strength expected)
        {
            Assert.Equal(expected, DriveStrengthRange.WiredAnd(a, b));
        }

        [Theory]
        [MemberData(nameof(WiredOrTests))]
        public void When_WiredOr_Expect_Reference(Strength a, Strength b, Strength expected)
        {
            Assert.Equal(expected, DriveStrengthRange.WiredOr(a, b));
        }

        [Theory]
        [MemberData(nameof(ReduceTests))]
        public void When_Reduce_Expect_Reference(Strength a, Strength expected)
        {
            // Emulates resistive devices
            Assert.Equal(expected, DriveStrengthRange.Reduce(a));
        }

        [Theory]
        [MemberData(nameof(AmbiguousWiredAndTests))]
        public void When_AmbiguousWiredAndTests_Expect_Reference(DriveStrengthRange a, DriveStrengthRange b, DriveStrengthRange expected)
        {
            Assert.Equal(expected, DriveStrengthRange.WiredAnd(a, b));
        }

        [Theory]
        [MemberData(nameof(AmbiguousWiredOrTests))]
        public void When_AmbiguousWiredOrTests_Expect_Reference(DriveStrengthRange a, DriveStrengthRange b, DriveStrengthRange expected)
        {
            Assert.Equal(expected, DriveStrengthRange.WiredOr(a, b));
        }

        [Theory]
        [MemberData(nameof(AmbiguousWiredTests))]
        public void When_AmbigousWiredTests_Expect_Reference(DriveStrengthRange a, DriveStrengthRange b, DriveStrengthRange expected)
        {
            Assert.Equal(expected, DriveStrengthRange.Wired(a, b));
        }

        [Theory]
        [MemberData(nameof(StrengthRangeLogic))]
        public void When_StrengthRangeLogic_Expect_Reference(DriveStrengthRange a, Signal expected)
        {
            Assert.Equal(expected, a.Logic);
        }

        public static IEnumerable<object[]> Tests
        {
            get
            {
                yield return new object[] { new DriveStrengthRange(Strength.We0, Strength.We1), "WeX" };
                yield return new object[] { new DriveStrengthRange(Strength.None, Strength.St1), "StH" };
                yield return new object[] { new DriveStrengthRange(Strength.None, Strength.St0), "StL" };
                yield return new object[] { new DriveStrengthRange(Strength.We0, Strength.Pu1), "35X" };
                yield return new object[] { new DriveStrengthRange(Strength.Pu1, Strength.St1), "651" };
                yield return new object[] { new DriveStrengthRange(Strength.Pu0, Strength.We0), "530" };
                yield return new object[] { new DriveStrengthRange(Strength.Pu0, Strength.St1), "56X" };
                yield return new object[] { new DriveStrengthRange(Strength.HiZ0, Strength.St1), "StH" };
                yield return new object[] { new DriveStrengthRange(Strength.We0, Strength.We0), "We0" };
                yield return new object[] { new DriveStrengthRange(Strength.We0, Strength.St1), "36X" };
            }
        }

        public static IEnumerable<object[]> AmbiguousWiredTests
        {
            get
            {
                yield return new object[]
                {
                    new DriveStrengthRange(Strength.St0, Strength.Pu0),
                    new DriveStrengthRange(Strength.Pu1),
                    new DriveStrengthRange(Strength.St0, Strength.Pu1)
                };
            }
        }

        public static IEnumerable<object[]> WiredAndTests
        {
            get
            {
                yield return new object[] { Strength.St0, Strength.St1, Strength.St0 };
                yield return new object[] { Strength.We0, Strength.St1, Strength.St1 };
                yield return new object[] { Strength.Sm0, Strength.St0, Strength.St0 };
                yield return new object[] { Strength.HiZ0, Strength.HiZ1, Strength.HiZ0 };
            }
        }

        public static IEnumerable<object[]> WiredOrTests
        {
            get
            {
                yield return new object[] { Strength.St0, Strength.St1, Strength.St1 };
                yield return new object[] { Strength.We0, Strength.St1, Strength.St1 };
                yield return new object[] { Strength.Sm0, Strength.St0, Strength.St0 };
                yield return new object[] { Strength.HiZ0, Strength.HiZ1, Strength.HiZ1 };
            }
        }

        public static IEnumerable<object[]> ReduceTests
        {
            get
            {
                yield return new object[] { Strength.Su1, Strength.Pu1 };
                yield return new object[] { Strength.Su0, Strength.Pu0 };

                yield return new object[] { Strength.St1, Strength.Pu1 };
                yield return new object[] { Strength.St0, Strength.Pu0 };

                yield return new object[] { Strength.Pu1, Strength.We1 };
                yield return new object[] { Strength.Pu0, Strength.We0 };

                yield return new object[] { Strength.La1, Strength.Me1 };
                yield return new object[] { Strength.La0, Strength.Me0 };

                yield return new object[] { Strength.We1, Strength.Me1 };
                yield return new object[] { Strength.We0, Strength.Me0 };

                yield return new object[] { Strength.Me1, Strength.Sm1 };
                yield return new object[] { Strength.Me0, Strength.Sm0 };

                yield return new object[] { Strength.Sm1, Strength.Sm1 };
                yield return new object[] { Strength.Sm0, Strength.Sm0 };

                yield return new object[] { Strength.HiZ0, Strength.HiZ0 };
                yield return new object[] { Strength.HiZ1, Strength.HiZ1 };

                yield return new object[] { Strength.None, Strength.None };
            }
        }

        public static IEnumerable<object[]> AmbiguousWiredAndTests
        {
            get
            {
                yield return new object[] {
                    new DriveStrengthRange(Strength.St0, Strength.Pu0),
                    new DriveStrengthRange(Strength.Pu1),
                    new DriveStrengthRange(Strength.St0, Strength.Pu0)
                };
            }
        }

        public static IEnumerable<object[]> AmbiguousWiredOrTests
        {
            get
            {
                yield return new object[]
                {
                    new DriveStrengthRange(Strength.St0, Strength.Pu0),
                    new DriveStrengthRange(Strength.Pu1),
                    new DriveStrengthRange(Strength.St0, Strength.Pu1)
                };
            }
        }

        public static IEnumerable<object[]> StrengthRangeLogic
        {
            get
            {
                yield return new object[] { new DriveStrengthRange(Strength.We0, Strength.We0), Signal.L };
                yield return new object[] { new DriveStrengthRange(Strength.Sm1, Strength.St1), Signal.H };
                yield return new object[] { new DriveStrengthRange(Strength.We0, Strength.We1), Signal.X };
                yield return new object[] { new DriveStrengthRange(Strength.HiZ0, Strength.HiZ1), Signal.Z };
            }
        }
    }
}
