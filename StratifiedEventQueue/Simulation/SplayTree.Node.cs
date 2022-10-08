using StratifiedEventQueue.Simulation;

namespace StratifiedEventQueue
{
    public partial class SplayTree
    {
        /// <summary>
        /// A node that will be part of the SplayTree
        /// </summary>
        private class Node
        {
            /// <summary>
            /// Gets the key of the node.
            /// </summary>
            public ulong Key { get; }

            /// <summary>
            /// Gets the value of the node.
            /// </summary>
            public EventQueue Value { get; }

            /// <summary>
            /// Gets or sets the left child node.
            /// </summary>
            public Node Left { get; set; }

            /// <summary>
            /// Gets or sets the right child node.
            /// </summary>
            public Node Right { get; set; }

            /// <summary>
            /// Creates a new <see cref="Node"/>.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public Node(ulong key, EventQueue value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}
