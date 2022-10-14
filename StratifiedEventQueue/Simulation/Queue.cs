namespace StratifiedEventQueue.Simulation
{
    /// <summary>
    /// A queue implementation using a linked list.
    /// </summary>
    /// <remarks>
    /// It is possible for some time points to generate a lot of events, while others (or most of them)
    /// may only generate a very small amount. When swapping around queues using a circular array in this
    /// case, we are likely to end up with many queues that will have to allocate a large array.
    /// This is not the case when using linked lists. Furthermore, we use a pool that does use an array
    /// under the hood to reuse nodes avoiding the necessity of the garbage collector to run.
    /// </remarks>
    /// <typeparam name="T">The base type.</typeparam>
    public class Queue<T>
    {
        private Node _head = null, _tail = null;

        private class Node
        {
            // It's a bit strange that we have a queue inside something that should be describing a queue
            // The reason is that we want to be able to quickly swap queues around, without having the
            // underlying arrays of System.Collections.Generic.Queue<T> blowing up too much.
            // If there is only one step every 100 that is very heavy on events, but the arrays are swapped
            // around, all arrays will end up needing to allocate a large number of events.
            // If we have a single queue for our pool, there is only one block of memory allocated for
            // node reuse.
            private static readonly System.Collections.Generic.Queue<Node> _pool = new System.Collections.Generic.Queue<Node>();

            /// <summary>
            /// Gets the value of the queue node.
            /// </summary>
            public T Value { get; private set; }

            /// <summary>
            /// Gets the next node in the queue.
            /// </summary>
            public Node Next { get; set; }

            private Node()
            {
            }

            /// <summary>
            /// Releases the node from the queue.
            /// </summary>
            public void Release()
            {
                _pool.Enqueue(this);
            }

            /// <summary>
            /// Creates a new queue node.
            /// </summary>
            /// <param name="head">The previous head of the queue</param>
            /// <param name="value"></param>
            /// <returns></returns>
            public static Node Create(T value)
            {
                var result = _pool.Count > 0 ? _pool.Dequeue() : new Node();
                result.Value = value;
                result.Next = null;
                return result;
            }
        }

        /// <summary>
        /// Checks whether there are items in the queue or not.
        /// </summary>
        public bool IsEmpty => _head == null;

        /// <summary>
        /// Enqueues an item on the queue.
        /// </summary>
        /// <param name="item">The item to enqueue.</param>
        public void Enqueue(T item)
        {
            if (_head == null)
            {
                // Empty queue, create a first item
                _head = Node.Create(item);
                _tail = _head;
            }
            else
            {
                // Append an item to the head of the queue
                _head.Next = Node.Create(item);
                _head = _head.Next;
            }
        }

        /// <summary>
        /// Dequeues an item from the queue.
        /// </summary>
        /// <returns>The item.</returns>
        public T Dequeue()
        {
            // Move to the next item in the queue
            var tail = _tail;
            _tail = tail.Next;

            if (_tail == null)
            {
                // We went through the whole queue
                _head = null;
            }

            // Allow object reuse
            tail.Release();
            return tail.Value;
        }
    }
}
