﻿using System;
using System.Collections.Generic;

namespace StratifiedEventQueue.States
{
    /// <summary>
    /// A state that emits events when it changes.
    /// </summary>
    /// <typeparam name="T">The base type.</typeparam>
    public interface IState<T>
    {
        /// <summary>
        /// Gets the comparer used by the variable.
        /// </summary>
        IEqualityComparer<T> Comparer { get; }

        /// <summary>
        /// Occurs when the value of the state changes.
        /// </summary>
        event EventHandler<StateChangedEventArgs<T>> Changed;

        /// <summary>
        /// Gets the name of the state.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the value of the state.
        /// </summary>
        T Value { get; }
    }
}
