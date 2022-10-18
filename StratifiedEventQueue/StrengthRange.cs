using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace StratifiedEventQueue
{
    /// <summary>
    /// A range of strengths, possibly ambiguous.
    /// </summary>
    public struct StrengthRange : IEquatable<StrengthRange>
    {
        /// <summary>
        /// Gets the low end of the strength range.
        /// </summary>
        public Strength Low { get; }

        /// <summary>
        /// Gets the high end of the strength range.
        /// </summary>
        public Strength High { get; }

        /// <summary>
        /// Gets the equivalent logic value, based on the strength levels.
        /// </summary>
        public Logic Logic
        {
            get
            {
                // Both strengths are in the value 1 range
                if (Low > Strength.HiZ1)
                    return Logic.H;

                // Both strengths are in the value 0 range
                if (High < Strength.HiZ0)
                    return Logic.L;

                // Both values are in the high-Z range
                if (Low >= Strength.HiZ0 && High <= Strength.HiZ1)
                    return Logic.Z;

                // We don't know...
                return Logic.X;
            }
        }

        /// <summary>
        /// Creates a new <see cref="StrengthRange"/>.
        /// </summary>
        /// <remarks>Allow ambiguous strengths.</remarks>
        /// <param name="low">The low end.</param>
        /// <param name="high">The high end.</param>
        public StrengthRange(Strength low, Strength high)
        {
            if (low < high)
            {
                Low = low;
                High = high;
            }
            else
            {
                Low = high;
                High = low;
            }
        }

        /// <summary>
        /// Creates a new <see cref="StrengthRange"/>.
        /// </summary>
        /// <remarks>A nonambiguous strength.</remarks>
        /// <param name="strength">The strength.</param>
        public StrengthRange(Strength strength)
        {
            Low = strength;
            High = strength;
        }

        /// <summary>
        /// Creates a hash code for the drive strength range.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode() => ((int)High << 8) | (int)Low;

        /// <summary>
        /// Checks equality.
        /// </summary>
        /// <param name="obj">The object to check with.</param>
        /// <returns>Returns <c>true</c> if the object is equal; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is StrengthRange range)
                return Equals(range);
            return false;
        }

        /// <summary>
        /// Checks equality.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>Returns <c>true</c> if the range is equal to the other range; otherwise, <c>false</c>.</returns>
        public bool Equals(StrengthRange other) => other.High == High && other.Low == Low;

        /// <summary>
        /// Converts the range to a string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            if (Low == 0 && High == 0)
                return "0";

            // Unambiguous, simple strength
            if (Low == High)
                return Low.ToString();

            if (Low >= Strength.HiZ0 && Low <= Strength.HiZ1)
            {
                // Low range is high-impedant
                switch (High)
                {
                    case Strength.HiZ1: return "HiZH";
                    case Strength.Sm1: return "SmH";
                    case Strength.Me1: return "MeH";
                    case Strength.La1: return "LaH";
                    case Strength.We1: return "WeH";
                    case Strength.St1: return "StH";
                    case Strength.Pu1: return "PuH";
                    case Strength.Su1: return "SuH";
                    default: return "?";
                };
            }

            if (High >= Strength.HiZ0 && High <= Strength.HiZ1)
            {
                // High range is high-impedant
                switch (Low)
                {
                    case Strength.HiZ0: return "HiZL";
                    case Strength.Sm0: return "SmL";
                    case Strength.Me0: return "MeL";
                    case Strength.La0: return "LaL";
                    case Strength.We0: return "WeL";
                    case Strength.St0: return "StL";
                    case Strength.Pu0: return "PuL";
                    case Strength.Su0: return "SuL";
                    default: return "?";
                };
            }

            var sb = new StringBuilder(3);
            if (Low < 0 && High < 0)
            {
                // Both are on the '0' side
                sb.Append(-(int)Low - 1);
                sb.Append(-(int)High - 1);
                sb.Append('0');
                return sb.ToString();
            }
            if (Low > 0 && High > 0)
            {
                // Both are on the '1' side
                sb.Append((int)High - 1);
                sb.Append((int)Low - 1);
                sb.Append('1');
                return sb.ToString();
            }

            // Opposite sides, low is on the '0' side and high is on the '1' side
            int high = (int)High;
            int low = -(int)Low;
            if (high == low)
            {
                // Same strength result
                switch (Low)
                {
                    case Strength.Sm0: return "SmX";
                    case Strength.Me0: return "MeX";
                    case Strength.La0: return "LaX";
                    case Strength.We0: return "WeX";
                    case Strength.St0: return "StX";
                    case Strength.Pu0: return "PuX";
                    case Strength.Su0: return "SuX";
                    default: return "?";
                }
            }
            else
            {
                // Generic result
                sb.Append(low - 1);
                sb.Append(high - 1);
                sb.Append('X');
            }
            return sb.ToString();
        }

        /// <summary>
        /// Checks equality between two strength ranges.
        /// </summary>
        /// <param name="a">The left argument.</param>
        /// <param name="b">The right argument.</param>
        /// <returns>Returns <c>true</c> if both ranges are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(StrengthRange a, StrengthRange b) => a.Low == b.Low && a.High == b.High;

        /// <summary>
        /// Checks inequality between two strength ranges.
        /// </summary>
        /// <param name="a">The left argument.</param>
        /// <param name="b">The right argument.</param>
        /// <returns>Returns <c>true</c> if both ranges are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(StrengthRange a, StrengthRange b) => a.Low != b.Low || a.High != b.High;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static sbyte Abs(Strength a) => a > Strength.None ? (sbyte)a : (Strength.None - a);

        /// <summary>
        /// Combines two driving strengths.
        /// </summary>
        /// <param name="a">The first strength.</param>
        /// <param name="b">The second strength.</param>
        /// <returns>The result.</returns>
        public static StrengthRange Combine(Strength a, Strength b)
        {
            var sa = Abs(a);
            var sb = Abs(b);

            // If one strength is higher, use that
            if (sa > sb)
                return new StrengthRange(a);
            if (sb > sa)
                return new StrengthRange(b);

            // Same strength, combine both
            return new StrengthRange(a, b);
        }

        /// <summary>
        /// Combines two driving strength ranges.
        /// </summary>
        /// <param name="a">The first strength.</param>
        /// <param name="b">The second strength.</param>
        /// <returns>The result.</returns>
        public static StrengthRange Wired(StrengthRange a, StrengthRange b)
        {
            Strength low = Strength.Su1, high = Strength.Su0;
            for (var i = a.Low; i <= a.High; i++)
            {
                for (var j = b.Low; j <= b.High; j++)
                {
                    var r = Combine(i, j);
                    if (r.Low < low)
                        low = r.Low;
                    if (r.High > high)
                        high = r.High;
                }
            }
            return new StrengthRange(low, high);
        }

        /// <summary>
        /// Combines two driving strength according to wired and rules.
        /// </summary>
        /// <param name="a">The first strength.</param>
        /// <param name="b">The second strength.</param>
        /// <returns>The result.</returns>
        public static Strength WiredAnd(Strength a, Strength b)
        {
            var sa = Abs(a);
            var sb = Abs(b);

            // If one strength is higher, use that
            if (sa > sb)
                return a;
            if (sb > sa)
                return b;

            // They are same strength, so return the lowest of the two
            return a < b ? a : b;
        }

        /// <summary>
        /// Combines two driving strengths according to wired or rules.
        /// </summary>
        /// <param name="a">The first strength.</param>
        /// <param name="b">The second strength.</param>
        /// <returns>The result.</returns>
        public static Strength WiredOr(Strength a, Strength b)
        {
            var sa = Abs(a);
            var sb = Abs(b);

            // If one strength is higher, use that
            if (sa > sb)
                return a;
            if (sb > sa)
                return b;

            // They are same strength, so return the higher of the two
            return a > b ? a : b;
        }

        /// <summary>
        /// Reduces a drive strength according for resistive devices.
        /// </summary>
        /// <param name="a">The original strength.</param>
        /// <returns>The reduced strength.</returns>
        public static Strength Reduce(Strength a)
        {
            switch (a)
            {
                case Strength.Su0: return Strength.Pu0;
                case Strength.St0: return Strength.Pu0;
                case Strength.Pu0: return Strength.We0;
                case Strength.La0: return Strength.Me0;
                case Strength.We0: return Strength.Me0;
                case Strength.Me0: return Strength.Sm0;
                case Strength.Sm0: return Strength.Sm0;
                case Strength.HiZ0: return Strength.HiZ0;

                case Strength.None: return Strength.None;

                case Strength.HiZ1: return Strength.HiZ1;
                case Strength.Sm1: return Strength.Sm1;
                case Strength.Me1: return Strength.Sm1;
                case Strength.We1: return Strength.Me1;
                case Strength.La1: return Strength.Me1;
                case Strength.Pu1: return Strength.We1;
                case Strength.St1: return Strength.Pu1;
                case Strength.Su1: return Strength.Pu1;
            }
            return Strength.None;
        }

        /// <summary>
        /// Combines two strength ranges according to wired and rules.
        /// </summary>
        /// <param name="a">The first strength range.</param>
        /// <param name="b">The second strength range.</param>
        /// <returns>The result.</returns>
        public static StrengthRange WiredAnd(StrengthRange a, StrengthRange b)
        {
            Strength low = Strength.Su1, high = Strength.Su0;
            for (var i = a.Low; i <= a.High; i++)
            {
                for (var j = b.Low; j <= b.High; j++)
                {
                    var r = WiredAnd(i, j);
                    if (r < low)
                        low = r;
                    if (r > high)
                        high = r;
                }
            }
            return new StrengthRange(low, high);
        }

        /// <summary>
        /// Combines two strength ranges according to wired or rules.
        /// </summary>
        /// <param name="a">The first strength range.</param>
        /// <param name="b">The second strength range.</param>
        /// <returns>The result.</returns>
        public static StrengthRange WiredOr(StrengthRange a, StrengthRange b)
        {
            Strength low = Strength.Su1, high = Strength.Su0;
            for (var i = a.Low; i <= a.High; i++)
            {
                for (var j = b.Low; j <= b.High; j++)
                {
                    var r = WiredOr(i, j);
                    if (r < low)
                        low = r;
                    if (r > high)
                        high = r;
                }
            }
            return new StrengthRange(low, high);
        }
    }
}
