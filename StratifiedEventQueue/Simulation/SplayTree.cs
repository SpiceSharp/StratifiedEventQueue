using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using StratifiedEventQueue.Simulation;

namespace StratifiedEventQueue
{
    /// <summary>
    /// A splay tree optimized for event scheduling. This implementation
    /// also allows duplicate keys.
    /// </summary>
    /// <typeparam name="TKey">The key.</typeparam>
    /// <typeparam name="TValue">The value.</typeparam>
    public partial class SplayTree
    {
        private Node _root;

        /// <summary>
        /// Gets the number of items in the dictionary.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Adds a new item in the tree.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(ulong key, EventQueue value)
        {
            var node = new Node(key, value);

            if (Count == 0)
            {
                // Simple case
                _root = node;
                Count = 1;
                return;
            }

            // Find the location to insert the node
            _root = Splay(_root, key);

            // Insert the element as the new root
            if (_root.Key < key)
            {
                node.Left = _root;
                node.Right = _root.Right;
                _root.Right = null;
            }
            else
            {
                node.Left = _root.Left;
                node.Right = _root;
                _root.Left = null;
            }
            _root = node;
            Count++;
        }

        /// <summary>
        /// Gets a value from the tree, or creates a new key-value pair if the key was not found yet.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="factory">The factory.</param>
        /// <returns>The value.</returns>
        public EventQueue GetOrAdd(ulong key)
        {
            if (_root == null)
            {
                _root = new Node(key, new EventQueue());
                Count = 1;
                return _root.Value;
            }

            // Splay the tree to the node
            _root = Splay(_root, key);

            // Insert the element as the new root
            if (_root.Key == key)
                return _root.Value;

            var node = new Node(key, new EventQueue());
            if (_root.Key < key)
            {
                node.Left = _root;
                node.Right = _root.Right;
                _root.Right = null;
            }
            else
            {
                node.Left = _root.Left;
                node.Right = _root;
                _root.Left = null;
            }
            _root = node;
            Count++;
            return node.Value;
        }

        /// <summary>
        /// Peeks the first item in the tree.
        /// </summary>
        /// <returns>The first item in the tree.</returns>
        public KeyValuePair<ulong, EventQueue> PeekFirst()
        {
            while (_root.Left != null)
                _root = RotateRight(_root);
            return new KeyValuePair<ulong, EventQueue>(_root.Key, _root.Value);
        }

        /// <summary>
        /// Gets the first item in the tree and removes it.
        /// </summary>
        /// <returns>The first item in the tree.</returns>
        public KeyValuePair<ulong, EventQueue> PopFirst()
        {
            // We can keep rotating right until our left element
            // is the minimum one
            while (_root.Left != null)
                _root = RotateRight(_root);
            var result = _root;
            _root = _root.Right;

            Count--;
            result.Left = null;
            result.Right = null;
            return new KeyValuePair<ulong, EventQueue>(result.Key, result.Value);
        }

        /// <summary>
        /// Rotates a subtree to the right rooted with x.
        /// </summary>
        /// <param name="x">The child of which the left child should become root.</param>
        /// <returns>Returns the new root (the originally left child).</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Node RotateRight(Node x)
        {
            Node y = x.Left;
            x.Left = y.Right;
            y.Right = x;
            return y;
        }

        /// <summary>
        /// Rotates a subtree to the left rooted with x.
        /// </summary>
        /// <param name="x">The child of which the right child should become root.</param>
        /// <returns>Returns the new root (the originally right child).</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Node RotateLeft(Node x)
        {
            Node y = x.Right;
            x.Right = y.Left;
            y.Left = x;
            return y;
        }

        /// <summary>
        /// Brings the element _root to the key to the root position.
        /// </summary>
        /// <param name="root">The root node.</param>
        /// <param name="key">The key.</param>
        /// <returns>The new root element.</returns>
        private Node Splay(Node root, ulong key)
        {
            if (root == null)
                return null;

            if (root.Key == key)
                return root;
            else if (root.Key > key)
            {
                // Key lies in the left subtree
                if (root.Left == null)
                    return root;

                if (root.Left.Key > key)
                {
                    // Zig-zig
                    //        R
                    //       / \
                    //      X   T2
                    //     / \
                    // HERE   T1
                    root.Left.Left = Splay(root.Left.Left, key);

                    // Do first rotation for root
                    root = RotateRight(root);
                }
                else if (root.Left.Key < key)
                {
                    // Zag-zig
                    //      R
                    //     / \
                    //    X   T2
                    //   / \
                    // T1   HERE
                    root.Left.Right = Splay(root.Left.Right, key);

                    // Do first rotation for root.Left
                    if (root.Left.Right != null)
                        root.Left = RotateLeft(root.Left);
                }

                // Do the second rotation for root
                return root.Left == null ? root : RotateRight(root);
            }
            else
            {
                // Key lies in right subtree
                if (root.Right == null)
                    return root;

                if (root.Right.Key > key)
                {
                    // Zig-zag
                    //     R
                    //    / \
                    //  T1   X
                    //      / \
                    //  HERE   T2
                    root.Right.Left = Splay(root.Right.Left, key);

                    // Do first rotation for root.right
                    if (root.Right.Left != null)
                        root.Right = RotateRight(root.Right);
                }
                else if (root.Right.Key < key)
                {
                    // Zag-zag
                    //     R
                    //    / \
                    //  T1   X
                    //      / \
                    //    T2   HERE
                    root.Right.Right = Splay(root.Right.Right, key);
                    root = RotateLeft(root);
                }

                // Second rotation
                return root.Right == null ? root : RotateLeft(root);
            }
        }
    }
}
