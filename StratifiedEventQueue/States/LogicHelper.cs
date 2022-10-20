using System;

namespace StratifiedEventQueue.States
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
        public static Signal And(Signal a, Signal b)
        {
            if (a == Signal.L || b == Signal.L)
                return Signal.L;
            if (a == Signal.H && b == Signal.H)
                return Signal.H;
            return Signal.X;
        }

        /// <summary>
        /// Nand gate.
        /// </summary>
        /// <param name="a">The first argument.</param>
        /// <param name="b">The second argument.</param>
        /// <returns>The result.</returns>
        public static Signal Nand(Signal a, Signal b)
        {
            if (a == Signal.L || b == Signal.L)
                return Signal.H;
            if (a == Signal.H && b == Signal.H)
                return Signal.L;
            return Signal.X;
        }

        /// <summary>
        /// Or gate.
        /// </summary>
        /// <param name="a">The first argument.</param>
        /// <param name="b">The second argument.</param>
        /// <returns>The result.</returns>
        public static Signal Or(Signal a, Signal b)
        {
            if (a == Signal.H || b == Signal.H)
                return Signal.H;
            if (a == Signal.L && b == Signal.L)
                return Signal.L;
            return Signal.X;
        }

        /// <summary>
        /// Nor gate.
        /// </summary>
        /// <param name="a">The first argument.</param>
        /// <param name="b">The second argument.</param>
        /// <returns>The result.</returns>
        public static Signal Nor(Signal a, Signal b)
        {
            if (a == Signal.H || b == Signal.H)
                return Signal.L;
            if (a == Signal.L && b == Signal.L)
                return Signal.H;
            return Signal.X;
        }

        /// <summary>
        /// Xor gate.
        /// </summary>
        /// <param name="a">The first argument.</param>
        /// <param name="b">The second argument.</param>
        /// <returns>The result.</returns>
        public static Signal Xor(Signal a, Signal b)
        {
            if (a == Signal.L && b == Signal.H || a == Signal.H && b == Signal.L)
                return Signal.H;
            if (a == Signal.L && b == Signal.L || a == Signal.H && b == Signal.H)
                return Signal.L;
            return Signal.X;
        }

        /// <summary>
        /// Xnor gate.
        /// </summary>
        /// <param name="a">The first argument.</param>
        /// <param name="b">The second argument.</param>
        /// <returns>The result.</returns>
        public static Signal Xnor(Signal a, Signal b)
        {
            if (a == Signal.L && b == Signal.H || a == Signal.H && b == Signal.L)
                return Signal.L;
            if (a == Signal.L && b == Signal.L || a == Signal.H && b == Signal.H)
                return Signal.H;
            return Signal.X;
        }

        /// <summary>
        /// Buffer.
        /// </summary>
        /// <param name="a">The argument.</param>
        /// <returns>The second argument.</returns>
        public static Signal Buf(Signal a)
        {
            if (a == Signal.Z)
                return Signal.X;
            return a;
        }

        /// <summary>
        /// Not gate.
        /// </summary>
        /// <param name="a">The argument.</param>
        /// <returns>The result.</returns>
        public static Signal Not(Signal a)
        {
            switch (a)
            {
                case Signal.L:
                    return Signal.H;
                case Signal.H:
                    return Signal.L;
                default:
                    return Signal.X;
            }
        }

        /// <summary>
        /// Converts a character to a signal value.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns>The signal value.</returns>
        /// <exception cref="ArgumentException">Thrown if the character is an invalid character.</exception>
        public static Signal ToLogic(this char c)
        {
            switch (c)
            {
                case 'x':
                case 'X':
                    return Signal.X;

                case '0':
                case 'L':
                case 'l':
                    return Signal.L;

                case '1':
                case 'H':
                case 'h':
                    return Signal.H;

                case 'z':
                case 'Z':
                case '?':
                    return Signal.Z;

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
        public static Signal[] ToLogic(this string s)
        {
            var result = new Signal[s.Length];
            for (int i = 0; i < s.Length; i++)
                result[i] = s[i].ToLogic();
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
        public static Signal[] ToLogicOctal(string s)
        {
            var result = new Signal[s.Length * 3];
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
                            result[index++] = Signal.H;
                        else
                            result[index++] = Signal.L;
                        mask <<= 1;
                    }
                }
                else if (c == 'x' || c == 'X')
                {
                    for (int j = 0; j < 2; j++)
                        result[index++] = Signal.X;
                }
                else if (c == 'z' || c == 'Z' || c == '?')
                {
                    for (int j = 0; j < 2; j++)
                        result[index++] = Signal.Z;
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
        public static Signal[] ToLogicHex(this string s)
        {
            var result = new Signal[s.Length * 4];
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
                            result[index++] = Signal.H;
                        else
                            result[index++] = Signal.L;
                        mask <<= 1;
                    }
                }
                else if (c == 'x' || c == 'X')
                {
                    for (int j = 0; j < 3; j++)
                        result[index++] = Signal.X;
                }
                else if (c == 'z' || c == 'Z' || c == '?')
                {
                    for (int j = 0; j < 3; j++)
                        result[index++] = Signal.Z;
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
        public static Signal[] ToLogicDec(string s)
        {
            var result = new Signal[32];
            if (s == "X" || s == "x")
            {
                for (int i = 0; i < 32; i++)
                    result[i] = Signal.X;
            }
            else if (s == "Z" || s == "z" || s == "?")
            {
                for (int i = 0; i < 32; i++)
                    result[i] = Signal.Z;
            }
            else
            {
                uint value = Convert.ToUInt32(s);
                uint mask = 1;
                for (int i = 0; i < 32; i++)
                {
                    if ((value & mask) != 0)
                        result[i] = Signal.H;
                    else
                        result[i] = Signal.L;
                    mask <<= 1;
                }
            }
            return result;
        }
    }
}
