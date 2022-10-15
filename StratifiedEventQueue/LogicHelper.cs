using System;

namespace StratifiedEventQueue
{
    /// <summary>
    /// A comparer that can be used for signal values as defined by the verilog-A standard.
    /// </summary>
    public static class LogicHelper
    {
        /// <summary>
        /// And gate.
        /// </summary>
        /// <param name="a">The first argument.</param>
        /// <param name="b">The second argument.</param>
        /// <returns>The result.</returns>
        public static Logic And(Logic a, Logic b)
        {
            if (a == Logic.L || b == Logic.L)
                return Logic.L;
            if (a == Logic.H && b == Logic.H)
                return Logic.H;
            return Logic.X;
        }

        /// <summary>
        /// Nand gate.
        /// </summary>
        /// <param name="a">The first argument.</param>
        /// <param name="b">The second argument.</param>
        /// <returns>The result.</returns>
        public static Logic Nand(Logic a, Logic b)
        {
            if (a == Logic.L || b == Logic.L)
                return Logic.H;
            if (a == Logic.H && b == Logic.H)
                return Logic.L;
            return Logic.X;
        }

        /// <summary>
        /// Or gate.
        /// </summary>
        /// <param name="a">The first argument.</param>
        /// <param name="b">The second argument.</param>
        /// <returns>The result.</returns>
        public static Logic Or(Logic a, Logic b)
        {
            if (a == Logic.H || b == Logic.H)
                return Logic.H;
            if (a == Logic.L && b == Logic.L)
                return Logic.L;
            return Logic.X;
        }

        /// <summary>
        /// Nor gate.
        /// </summary>
        /// <param name="a">The first argument.</param>
        /// <param name="b">The second argument.</param>
        /// <returns>The result.</returns>
        public static Logic Nor(Logic a, Logic b)
        {
            if (a == Logic.H || b == Logic.H)
                return Logic.L;
            if (a == Logic.L && b == Logic.L)
                return Logic.H;
            return Logic.X;
        }

        /// <summary>
        /// Xor gate.
        /// </summary>
        /// <param name="a">The first argument.</param>
        /// <param name="b">The second argument.</param>
        /// <returns>The result.</returns>
        public static Logic Xor(Logic a, Logic b)
        {
            if (a == Logic.L && b == Logic.H || a == Logic.H && b == Logic.L)
                return Logic.H;
            if (a == Logic.L && b == Logic.L || a == Logic.H && b == Logic.H)
                return Logic.L;
            return Logic.X;
        }

        /// <summary>
        /// Xnor gate.
        /// </summary>
        /// <param name="a">The first argument.</param>
        /// <param name="b">The second argument.</param>
        /// <returns>The result.</returns>
        public static Logic Xnor(Logic a, Logic b)
        {
            if (a == Logic.L && b == Logic.H || a == Logic.H && b == Logic.L)
                return Logic.L;
            if (a == Logic.L && b == Logic.L || a == Logic.H && b == Logic.H)
                return Logic.H;
            return Logic.X;
        }

        /// <summary>
        /// Buffer.
        /// </summary>
        /// <param name="a">The argument.</param>
        /// <returns>The second argument.</returns>
        public static Logic Buf(Logic a)
        {
            if (a == Logic.Z)
                return Logic.X;
            return a;
        }

        /// <summary>
        /// Not gate.
        /// </summary>
        /// <param name="a">The argument.</param>
        /// <returns>The result.</returns>
        public static Logic Not(Logic a)
        {
            switch (a)
            {
                case Logic.L:
                    return Logic.H;
                case Logic.H:
                    return Logic.L;
                default:
                    return Logic.X;
            }
        }

        /// <summary>
        /// Converts a character to a signal value.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns>The signal value.</returns>
        /// <exception cref="ArgumentException">Thrown if the character is an invalid character.</exception>
        public static Logic ToLogic(this char c)
        {
            switch (c)
            {
                case 'x':
                case 'X':
                    return Logic.X;

                case '0':
                case 'L':
                case 'l':
                    return Logic.L;

                case '1':
                case 'H':
                case 'h':
                    return Logic.H;

                case 'z':
                case 'Z':
                case '?':
                    return Logic.Z;

                default:
                    throw new ArgumentException(string.Format("Invalid signal value '{0}'", c));
            }
        }

        /// <summary>
        /// Converts a string to an array of signal values.
        /// </summary>
        /// <remarks>
        /// This does not implement the real Verilog standard for literals.
        /// </remarks>
        /// <param name="s">The string.</param>
        /// <returns>The signal values.</returns>
        /// <exception cref="ArgumentException">Thrown if the input is invalid.</exception>
        public static Logic[] ToLogic(this string s)
        {
            var result = new Logic[s.Length];
            for (int i = 0; i < s.Length; i++)
                result[i] = ToLogic(s[i]);
            return result;
        }

        /// <summary>
        /// Converts a string to an array of signal values.
        /// </summary>
        /// <remarks>
        /// This does not implement the real Verilog standard for literals.
        /// </remarks>
        /// <param name="s">The string.</param>
        /// <returns>The signal values.</returns>
        /// <exception cref="ArgumentException">Thrown if the input is invalid.</exception>
        public static Logic[] ToLogicOctal(string s)
        {
            var result = new Logic[s.Length * 3];
            int index = 0;
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c >= '0' && c <= '8')
                {
                    int value = Convert.ToInt32(c.ToString(), 8);
                    int mask = 0x01;
                    for (int j = 0; j < 2; j++)
                    {
                        if ((value & mask) != 0)
                            result[index++] = Logic.H;
                        else
                            result[index++] = Logic.L;
                        mask <<= 1;
                    }
                }
                else if (c == 'x' || c == 'X')
                {
                    for (int j = 0; j < 2; j++)
                        result[index++] = Logic.X;
                }
                else if (c == 'z' || c == 'Z' || c == '?')
                {
                    for (int j = 0; j < 2; j++)
                        result[index++] = Logic.Z;
                }
                else
                    throw new ArgumentException(string.Format("Invalid signal value '{0}'", c));
            }
            return result;
        }

        /// <summary>
        /// Converts a string to an array of signal values.
        /// </summary>
        /// <remarks>
        /// This does not implement the real Verilog standard for literals.
        /// </remarks>
        /// <param name="s">The string.</param>
        /// <returns>The signal values.</returns>
        /// <exception cref="ArgumentException">Thrown if the input is invalid.</exception>
        public static Logic[] ToLogicHex(this string s)
        {
            var result = new Logic[s.Length * 4];
            int index = 0;
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c >= '0' && c <= '8')
                {
                    int value = Convert.ToInt32(c.ToString(), 16);
                    int mask = 0x01;
                    for (int j = 0; j < 3; j++)
                    {
                        if ((value & mask) != 0)
                            result[index++] = Logic.H;
                        else
                            result[index++] = Logic.L;
                        mask <<= 1;
                    }
                }
                else if (c == 'x' || c == 'X')
                {
                    for (int j = 0; j < 3; j++)
                        result[index++] = Logic.X;
                }
                else if (c == 'z' || c == 'Z' || c == '?')
                {
                    for (int j = 0; j < 3; j++)
                        result[index++] = Logic.Z;
                }
                else
                    throw new ArgumentException(string.Format("Invalid signal value '{0}'", c));
            }
            return result;
        }

        /// <summary>
        /// Converts a string to an array of signal values.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>The signal values.</returns>
        public static Logic[] ToLogicDec(string s)
        {
            var result = new Logic[32];
            if (s == "X" || s == "x")
            {
                for (int i = 0; i < 32; i++)
                    result[i] = Logic.X;
            }
            else if (s == "Z" || s == "z" || s == "?")
            {
                for (int i = 0; i < 32; i++)
                    result[i] = Logic.Z;
            }
            else
            {
                uint value = Convert.ToUInt32(s);
                uint mask = 1;
                for (int i = 0; i < 32; i++)
                {
                    if ((value & mask) != 0)
                        result[i] = Logic.H;
                    else
                        result[i] = Logic.L;
                    mask <<= 1;
                }
            }
            return result;
        }
    }
}
