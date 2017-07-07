using BuildIt.States.Interfaces;

namespace BuildIt.States
{
    /// <summary>
    /// Entity that keeps two state groups in sync
    /// </summary>
    public abstract class BaseBinder<TEntity> : IStateBinder
    {
        /// <summary>
        /// The target for binding (to be updated when source changes)
        /// </summary>
        protected TEntity Target { get; }
        
        /// <summary>
        /// The source to be monitored
        /// </summary>
        protected TEntity Source { get; }

        /// <summary>
        /// Whether the target is monitored for changes too
        /// </summary>
        protected bool BothDirections { get; }

        private bool IsBound { get; set; }

        /// <summary>
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
        /// Binds the source and target state groups
        /// </summary>
        public void Bind()
        {

            if (IsBound) return;
            InternalBind();
            IsBound = true;
        }

        /// <summary>
        /// Binds source and target
        /// </summary>
        protected abstract void InternalBind();

        /// <summary>
        /// Unbinds source and target
        /// </summary>
        protected abstract void InternalUnbind();

        /// <summary>
        /// Disconnects the two state groups
        /// </summary>
        public void Unbind()
        {
            if (!IsBound) return;
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
    }
}