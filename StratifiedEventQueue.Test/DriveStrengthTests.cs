using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ObjectiveC;
using System.Text;
using System.Threading.Tasks;

namespace StratifiedEventQueue.Test
{
    public class DriveStrengthTests
    {
        [Theory]
        [MemberData(nameof(Tests))]
        public void When_DriveStrengthRange_Expect_Reference(StrengthRange range, string result)
        {
            Assert.Equal(result, range.ToString());
        }

        [Theory]
        [MemberData(nameof(WiredAndTests))]
        public void When_WiredAnd_Expect_Reference(Strength a, Strength b, Strength expected)
        {
            Assert.Equal(expected, StrengthRange.WiredAnd(a, b));
        }

        [Theory]
        [MemberData(nameof(WiredOrTests))]
        public void When_WiredOr_Expect_Reference(Strength a, Strength b, Strength expected)
        {
            Assert.Equal(expected, StrengthRange.WiredOr(a, b));
        }

        [Theory]
        [MemberData(nameof(ReduceTests))]
        public void When_Reduce_Expect_Reference(Strength a, Strength expected)
        {
            Assert.Equal(expected, StrengthRange.Reduce(a));
        }

        [Theory]
        [MemberData(nameof(AmbiguousWiredAndTests))]
        public void When_AmbiguousWiredAndTests_Expect_Reference(StrengthRange a, StrengthRange b, StrengthRange expected)
        {
            Assert.Equal(expected, StrengthRange.WiredAnd(a, b));
        }

        [Theory]
        [MemberData(nameof(AmbiguousWiredOrTests))]
        public void When_AmbiguousWiredOrTests_Expect_Reference(StrengthRange a, StrengthRange b, StrengthRange expected)
        {
            Assert.Equal(expected, StrengthRange.WiredOr(a, b));
        }

        [Theory]
        [MemberData(nameof(AmbiguousWiredTests))]
        public void When_AmbigousWiredTests_Expect_Reference(StrengthRange a, StrengthRange b, StrengthRange expected)
        {
            Assert.Equal(expected, StrengthRange.Wired(a, b));
        }

        public static IEnumerable<object[]> Tests
        {
            get
            {
                yield return new object[] { new StrengthRange(Strength.We0, Strength.We1), "WeX" };
                yield return new object[] { new StrengthRange(Strength.None, Strength.St1), "StH" };
                yield return new object[] { new StrengthRange(Strength.None, Strength.St0), "StL" };
                yield return new object[] { new StrengthRange(Strength.We0, Strength.Pu1), "35X" };
                yield return new object[] { new StrengthRange(Strength.Pu1, Strength.St1), "651" };
                yield return new object[] { new StrengthRange(Strength.Pu0, Strength.We0), "530" };
                yield return new object[] { new StrengthRange(Strength.Pu0, Strength.St1), "56X" };
                yield return new object[] { new StrengthRange(Strength.HiZ0, Strength.St1), "StH" };
                yield return new object[] { new StrengthRange(Strength.We0, Strength.We0), "We0" };
                yield return new object[] { new StrengthRange(Strength.We0, Strength.St1), "36X" };
            }
        }

        public static IEnumerable<object[]> AmbiguousWiredTests
        {
            get
            {
                yield return new object[]
                {
                    new StrengthRange(Strength.St0, Strength.Pu0),
                    new StrengthRange(Strength.Pu1),
                    new StrengthRange(Strength.St0, Strength.Pu1)
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
                    new StrengthRange(Strength.St0, Strength.Pu0),
                    new StrengthRange(Strength.Pu1),
                    new StrengthRange(Strength.St0, Strength.Pu0)
                };
            }
        }

        public static IEnumerable<object[]> AmbiguousWiredOrTests
        {
            get
            {
                yield return new object[]
                {
                    new StrengthRange(Strength.St0, Strength.Pu0),
                    new StrengthRange(Strength.Pu1),
                    new StrengthRange(Strength.St0, Strength.Pu1)
                };
            }
        }
    }
}
