using System.Collections;
using System.Collections.Generic;

namespace StratifiedEventQueue.States.Nets
{
    /// <summary>
    /// A net is continuously driven and possibly from multiple sources.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    public abstract class Net<T> : State<T>
    {
        private readonly Arguments _arguments = new Arguments();

        /// <summary>
        /// A class that is used to provide access to the underlying drivers of the net.
        /// </summary>
        protected class Arguments : IReadOnlyList<T>
        {
            private readonly List<State<T>> _drivers = new List<State<T>>();

            /// <summary>
            /// Gets the value at the specified index.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <returns>The value.</returns>
            public T this[int index] => _drivers[index].Value;

            /// <summary>
            /// Gets the number of 
            /// </summary>
            public int Count => _drivers.Count;

            /// <summary>
            /// Add the state.
            /// </summary>
            /// <param name="driver">The driver.</param>
            public void Add(State<T> driver)
                => _drivers.Add(driver);

            /// <summary>
            /// Gets an enumerator.
            /// </summary>
            /// <returns>The enumerator.</returns>
            public IEnumerator<T> GetEnumerator()
            {
                foreach (var driver in _drivers)
                    yield return driver.Value;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        /// <summary>
        /// Creates a new <see cref="Net{T}"/>.
        /// </summary>
        /// <param name="name">The name of the net.</param>
        /// <param name="initialValue">The initial value of the net.</param>
        /// <param name="comparer">The comparer.</param>
        protected Net(string name, T initialValue = default, IEqualityComparer<T> comparer = null)
            : base(name, initialValue, comparer)
        {
        }

        /// <summary>
        /// Registers a driver to the net.
        /// </summary>
        /// <param name="driver">The driver.</param>
        public void RegisterDriver(State<T> driver)
        {
            _arguments.Add(driver);
            driver.Changed += Update;
        }

        /// <summary>
        /// Updates the net according to the current driver states.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The argument.</param>
        protected void Update(object sender, StateChangedEventArgs<T> args)
        {
            var result = Combine(_arguments);
            Change(args.Scheduler, result);
        }

        /// <summary>
        /// Combines the driver outputs into a result.
        /// </summary>
        /// <param name="inputs">The inputs.</param>
        /// <returns>The output.</returns>
        protected abstract T Combine(IReadOnlyList<T> inputs);
    }
}
