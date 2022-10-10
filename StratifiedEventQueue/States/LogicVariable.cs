using System.Collections.Generic;

namespace StratifiedEventQueue.States
{
    /// <summary>
    /// A variable specific to logic.
    /// </summary>
    public class LogicVariable : Variable<byte>
    {
        /// <summary>
        /// Creates a variable for logic.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="initialValue">The initial value (unknown by default).</param>
        /// <param name="comparer">The equality comparer (logic comparison by default).</param>
        public LogicVariable(string name, byte initialValue = Logic.X, IEqualityComparer<byte> comparer = null)
            : base(name, initialValue, comparer ?? Logic.Comparer)
        {
        }
    }
}
