using BuildIt.States.Interfaces;

namespace BuildIt.States
{
    /// <summary>
    /// Entity that keeps two state groups in sync
    /// </summary>
    public class StateGroupBinder : BaseBinder<IStateGroup>
    {
        /// <summary>
        /// Creates instance of binder and wires up link between state groups
        /// </summary>
        /// <param name="target">The target group (to be updated when source changes)</param>
        /// <param name="source">The source group</param>
        /// <param name="bothDirections">If true, source will be updated by changes to target</param>
        public StateGroupBinder(IStateGroup target, IStateGroup source, bool bothDirections):base(target,source,bothDirections)
        {
        }

        /// <summary>
        /// Binds the source and target state groups
        /// </summary>
        protected override void InternalBind()
        {
            Source.StateChanged += Source_StateChanged;
            if (!BothDirections) return;

            var dest = Target;
            if (dest == null) return;

            dest.StateChanged += Dest_StateChanged;
        }

        private void Source_StateChanged(object sender, StateEventArgs e)
        {
            if (e.IsNewState)
            {
                Target.ChangeTo(e.StateName, e.UseTransitions);
            }
            else
            {
                Target.ChangeBackTo(e.StateName, e.UseTransitions);
            }
        }

        private void Dest_StateChanged(object sender, StateEventArgs e)
        {
            if (e.IsNewState)
            {
                Source.ChangeTo(e.StateName, e.UseTransitions);
            }
            else
            {
                Source.ChangeBackTo(e.StateName, e.UseTransitions);
            }
        }

        /// <summary>
        /// Disconnects the two state groups
        /// </summary>
        protected override void InternalUnbind()
        {
            Source.StateChanged -= Source_StateChanged;

            if (!BothDirections) return;
            var dest = Target;
            if (dest == null) return;

            dest.StateChanged += Dest_StateChanged;
        }
    }
}