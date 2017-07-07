using BuildIt.States.Interfaces;

namespace BuildIt.States
{
    /// <summary>
    /// Entity that keeps two state groups in sync
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to bind</typeparam>
    public abstract class BaseBinder<TEntity> : IStateBinder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseBinder{TEntity}"/> class.
        /// Creates instance of binder and wires up link between state groups
        /// </summary>
        /// <param name="target">The target group (to be updated when source changes)</param>
        /// <param name="source">The source group</param>
        /// <param name="bothDirections">If true, source will be updated by changes to target</param>
        protected BaseBinder(TEntity target, TEntity source, bool bothDirections)
        {
            BothDirections = bothDirections;
            Target = target;
            Source = source;

            Bind();
        }

        /// <summary>
        /// Gets the target for binding (to be updated when source changes)
        /// </summary>
        protected TEntity Target { get; }

        /// <summary>
        /// Gets the source to be monitored
        /// </summary>
        protected TEntity Source { get; }

        /// <summary>
        /// Gets a value indicating whether whether the target is monitored for changes too
        /// </summary>
        protected bool BothDirections { get; }

        private bool IsBound { get; set; }

        /// <summary>
        /// Binds the source and target state groups
        /// </summary>
        public void Bind()
        {
            if (IsBound)
            {
                return;
            }

            InternalBind();
            IsBound = true;
        }

        /// <summary>
        /// Disconnects the two state groups
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
        /// Disposes (and unbinds) the binder
        /// </summary>
        public void Dispose()
        {
            Unbind();
        }

        /// <summary>
        /// Binds source and target
        /// </summary>
        protected abstract void InternalBind();

        /// <summary>
        /// Unbinds source and target
        /// </summary>
        protected abstract void InternalUnbind();
    }
}