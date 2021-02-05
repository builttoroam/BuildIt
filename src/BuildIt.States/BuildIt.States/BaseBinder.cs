using BuildIt.States.Interfaces;
using System;
using System.Threading.Tasks;

namespace BuildIt.States
{
    /// <summary>
    /// Entity that keeps two state groups in sync.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to bind.</typeparam>
    public abstract class BaseBinder<TEntity> : IStateBinder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseBinder{TEntity}"/> class.
        /// Creates instance of binder and wires up link between state groups.
        /// </summary>
        /// <param name="target">The target group (to be updated when source changes).</param>
        /// <param name="source">The source group.</param>
        /// <param name="bothDirections">If true, source will be updated by changes to target.</param>
        protected BaseBinder(TEntity target, TEntity source, bool bothDirections)
        {
            if (target == null)
            {
#pragma warning disable IDE0016 // Use 'throw' expression - the recommended simplification is incorrect
                throw new ArgumentNullException(nameof(target));
#pragma warning restore IDE0016 // Use 'throw' expression
            }

            if (source == null)
            {
#pragma warning disable IDE0016 // Use 'throw' expression - the recommended simplification is incorrect
                throw new ArgumentNullException(nameof(source));
#pragma warning restore IDE0016 // Use 'throw' expression
            }

            BothDirections = bothDirections;
            Target = target;
            Source = source;
        }

        /// <summary>
        /// Gets the target for binding (to be updated when source changes).
        /// </summary>
        protected TEntity Target { get; private set; }

        /// <summary>
        /// Gets the source to be monitored.
        /// </summary>
        protected TEntity Source { get; private set; }

        /// <summary>
        /// Gets a value indicating whether whether the target is monitored for changes too.
        /// </summary>
        protected bool BothDirections { get; }

        private bool IsBound { get; set; }

        /// <summary>
        /// Binds the source and target state groups.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task Bind()
        {
            if (IsBound)
            {
                return;
            }

            if (Target == null || Source == null)
            {
                throw new InvalidOperationException("Binder has been disposed; create a new binder to keep entities synchronized");
            }

            await InternalBind();
            IsBound = true;
        }

        /// <summary>
        /// Disconnects the two state groups.
        /// </summary>
        public void Unbind()
        {
            if (!IsBound)
            {
                return;
            }

            InternalUnbind();
            IsBound = false;
        }

        /// <summary>
        /// Disposes (and unbinds) the binder.
        /// </summary>
        public void Dispose()
        {
            Unbind();

            Source = default(TEntity);
            Target = default(TEntity);
        }

        /// <summary>
        /// Binds source and target.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected abstract Task InternalBind();

        /// <summary>
        /// Unbinds source and target.
        /// </summary>
        protected abstract void InternalUnbind();
    }
}