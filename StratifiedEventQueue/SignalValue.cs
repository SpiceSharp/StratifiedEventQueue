using System;
using System.Collections.Generic;

namespace StratifiedEventQueue
{
    /// <summary>
    /// A comparer that can be used for signal values as defined by the verilog-A standard.
    /// </summary>
    public class SignalValue : IEqualityComparer<byte>
    {
        /// <summary>
        /// Gets the comparer for signal values.
        /// </summary>
        public static IEqualityComparer<byte> Comparer { get; } = new SignalValue();

        /// <summary>
        /// Represents an unknown value.
        /// </summary>
        public const byte X = 0;

        /// <summary>
        /// Represents a low value.
        /// </summary>
        public const byte L = 1;

        /// <summary>
        /// Represents a high value.
        /// </summary>
        public const byte H = 2;

        /// <summary>
        /// Represents a high-impedant value.
        /// </summary>
        public const byte Z = 3;

        /// <summary>
        /// Creates a new <see cref="SignalValue"/>.
        /// </summary>
        private SignalValue()
        {
        }

        /// <inheritdoc />
        public bool Equals(byte x, byte y) => x == y;

        /// <inheritdoc />
        public int GetHashCode(byte obj) => obj;

        /// <summary>
        /// Converts a character to a signal value.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns>The signal value.</returns>
        /// <exception cref="ArgumentException">Thrown if the character is an invalid character.</exception>
        public static byte FromChar(char c)
        {
            switch (c)
            {
                case 'x':
                case 'X':
                    return X;

                case '0':
                case 'L':
                case 'l':
                    return L;

                case '1':
                case 'H':
                case 'h':
                    return H;

                case 'z':
                case 'Z':
                case '?':
                    return Z;

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
        public static byte[] FromStringBinary(string s)
        {
            var result = new byte[s.Length];
            for (int i = 0; i < s.Length; i++)
                result[i] = FromChar(s[i]);
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
        public static byte[] FromStringOctal(string s)
        {
            var result = new byte[s.Length * 3];
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
                            result[index++] = H;
                        else
                            result[index++] = L;
                        mask <<= 1;
                    }
                }
                else if (c == 'x' || c == 'X')
                {
                    for (int j = 0; j < 2; j++)
                        result[index++] = X;
                }
                else if (c == 'z' || c == 'Z' || c == '?')
                {
                    for (int j = 0; j < 2; j++)
                        result[index++] = Z;
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
        public static byte[] FromStringHex(string s)
        {
            var result = new byte[s.Length * 4];
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
                            result[index++] = H;
                        else
                            result[index++] = L;
                        mask <<= 1;
                    }
                }
                else if (c == 'x' || c == 'X')
                {
                    for (int j = 0; j < 3; j++)
                        result[index++] = X;
                }
                else if (c == 'z' || c == 'Z' || c == '?')
                {
                    for (int j = 0; j < 3; j++)
                        result[index++] = Z;
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
        /// <returns></returns>
        public static byte[] FromStringDec(string s)
        {
            var result = new byte[32];
            if (s == "X" || s == "x")
            {
                for (int i = 0; i < 32; i++)
                    result[i] = X;
            }
            else if (s == "Z" || s == "z" || s == "?")
            {
                for (int i = 0; i < 32; i++)
                    result[i] = Z;
            }
            else
            {
                uint value = Convert.ToUInt32(s);
                uint mask = 1;
                for (int i = 0; i < 32; i++)
                {
                    if ((value & mask) != 0)
                        result[i] = H;
                    else
                        result[i] = L;
                    mask <<= 1;
                }
            }
            return result;
        }
    }
}
