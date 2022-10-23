using System.Collections;
using System.Collections.Generic;
using StratifiedEventQueue.States.Nets;

namespace StratifiedEventQueue.States
{
    /// <summary>
    /// Represents a generic net that can take multiple drivers and resolve them.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    public abstract class Combiner<TIn, TOut> : State<TOut>
    {
        private readonly Inputs _inputs = new Inputs();

        /// <summary>
        /// A class that is used to provide access to the underlying drivers of the net.
        /// </summary>
        protected class Inputs : IReadOnlyList<TIn>
        {
            private readonly List<IState<TIn>> _drivers = new List<IState<TIn>>();

            /// <inheritdoc />
            public TIn this[int index] => _drivers[index].Value;

            /// <inheritdoc />
            public int Count => _drivers.Count;

            /// <summary>
            /// Add the state.
            /// </summary>
            /// <param name="driver">The driver.</param>
            public void Add(IState<TIn> driver)
                => _drivers.Add(driver);

            /// <inheritdoc />
            public IEnumerator<TIn> GetEnumerator()
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
        protected Combiner(string name, IEqualityComparer<TOut> comparer = null)
            : base(name, comparer)
        {
            Value = default;
        }

        /// <summary>
        /// Registers a driver to the net.
        /// </summary>
        /// <param name="driver">The driver.</param>
        public void RegisterDriver(IState<TIn> driver)
        {
            _inputs.Add(driver);
            driver.Changed += Update;
        }

        /// <summary>
        /// Updates the net according to the current driver states.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The argument.</param>
        protected void Update(object sender, StateChangedEventArgs<TIn> args)
        {
            var result = Combine(_inputs);
            Change(args.Scheduler, result);
        }

        /// <summary>
        /// Combines the results of all the drivers into a single result.
        /// </summary>
        /// <param name="drivers">The drivers.</param>
        /// <returns>The result.</returns>
        protected abstract TOut Combine(IReadOnlyList<TIn> drivers);
    }
}
