using System.Collections;
using System.Collections.Generic;

namespace StratifiedEventQueue.States.Nets
{
    /// <summary>
    /// Represents a generic net that can take multiple drivers and resolve them.
    /// </summary>
    /// <typeparam name="R">The driver type.</typeparam>
    /// <typeparam name="T">The result type.</typeparam>
    public abstract class InvariantNet<T> : State<T>
    {
        private T _result;
        private readonly Drivers _drivers = new Drivers();

        /// <summary>
        /// A class that is used to provide access to the underlying drivers of the net.
        /// </summary>
        protected class Drivers : IReadOnlyList<T>
        {
            private readonly List<IState<T>> _drivers = new List<IState<T>>();

            /// <inheritdoc />
            public T this[int index] => _drivers[index].Value;

            /// <inheritdoc />
            public int Count => _drivers.Count;

            /// <summary>
            /// Add the state.
            /// </summary>
            /// <param name="driver">The driver.</param>
            public void Add(IState<T> driver)
                => _drivers.Add(driver);

            /// <inheritdoc />
            public IEnumerator<T> GetEnumerator()
            {
                foreach (var driver in _drivers)
                    yield return driver.Value;
            }

            /// <inheritdoc />
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        /// <summary>
        /// Creates a new <see cref="WiredNet"/>.
        /// </summary>
        /// <param name="name">The name of the net.</param>
        /// <param name="initialValue">The initial value.</param>
        /// <param name="comparer">The comparer.</param>
        protected InvariantNet(string name, T initialValue = default, IEqualityComparer<T> comparer = null)
            : base(name, initialValue, comparer)
        {
        }

        /// <summary>
        /// Registers a driver to the net.
        /// </summary>
        /// <param name="driver">The driver.</param>
        public void RegisterDriver(IState<T> driver)
        {
            _drivers.Add(driver);
            driver.Changed += Update;
        }

        /// <summary>
        /// Updates the net according to the current driver states.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The argument.</param>
        protected void Update(object sender, StateChangedEventArgs<T> args)
        {
            var result = Combine(_drivers);
            Change(args.Scheduler, result);
        }

        /// <summary>
        /// Combines the results of all the drivers into a single result.
        /// </summary>
        /// <param name="drivers">The drivers.</param>
        /// <returns>The result.</returns>
        protected abstract T Combine(IReadOnlyList<T> drivers);
    }
}
