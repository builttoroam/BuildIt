using BuildIt.States.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuildIt.States
{
    /// <summary>
    /// Binds all the state groups within the statemanger to the state groups in another statemanager
    /// </summary>
    public class StateManagerBinder : BaseBinder<IStateManager>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateManagerBinder"/> class.
        /// Creates instance and binds the state groups
        /// </summary>
        /// <param name="target">The target (to be updated from source)</param>
        /// <param name="source">The source</param>
        /// <param name="bothDirections">Whether updates should go both directions (ie source updated from target)</param>
        public StateManagerBinder(IStateManager target, IStateManager source, bool bothDirections = true)
            : base(target, source, bothDirections)
        {
        }

        private IList<IStateBinder> GroupBinders { get; } = new List<IStateBinder>();

        /// <summary>
        /// Binds each of the state groups
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected override async Task InternalBind()
        {
            foreach (var kvp in Source.StateGroups)
            {
                var sg = Target.StateGroups.SafeValue(kvp.Key);
                var binderTask = sg?.Bind(kvp.Value, BothDirections);
                if (binderTask == null)
                {
                    continue;
                }

                var binder = await binderTask;
                GroupBinders.Add(binder);
            }
        }

        /// <summary>
        /// Unbinds each of the stage groups
        /// </summary>
        protected override void InternalUnbind()
        {
            foreach (var groupBinder in GroupBinders)
            {
                groupBinder.Unbind();
            }

            GroupBinders.Clear();
        }
    }
}