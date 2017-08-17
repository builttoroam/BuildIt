using System.Threading.Tasks;
using BuildIt.States.Interfaces;

namespace BuildIt.States
{
    /// <summary>
    /// Entity that keeps two state groups in sync
    /// </summary>
    public class StateGroupBinder : BaseBinder<IStateGroup>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateGroupBinder"/> class.
        /// Creates instance of binder and wires up link between state groups
        /// </summary>
        /// <param name="target">The target group (to be updated when source changes)</param>
        /// <param name="source">The source group</param>
        /// <param name="bothDirections">If true, source will be updated by changes to target</param>
        public StateGroupBinder(IStateGroup target, IStateGroup source, bool bothDirections)
            : base(target, source, bothDirections)
        {
        }

        /// <summary>
        /// Disconnects the two state groups
        /// </summary>
        protected override void InternalUnbind()
        {
            Source.StateChanged -= Source_StateChanged;

            if (!BothDirections)
            {
                return;
            }

            var dest = Target;
            if (dest == null)
            {
                return;
            }

            dest.StateChanged += Dest_StateChanged;
        }

        /// <summary>
        /// Binds the source and target state groups
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected override async Task InternalBind()
        {
            Source.StateChanged += Source_StateChanged;

            if (Source.CurrentStateName != Target.CurrentStateName)
            {
                await UpdateState(Target, Source.CurrentStateName, true, false);
            }

            if (!BothDirections)
            {
                return;
            }

            var dest = Target;
            if (dest == null)
            {
                return;
            }

            dest.StateChanged += Dest_StateChanged;
        }

        private async void Source_StateChanged(object sender, IStateEventArgs e)
        {
            await UpdateState(Target, e.StateName, e.IsNewState, e.UseTransitions);
        }

        private async void Dest_StateChanged(object sender, IStateEventArgs e)
        {
            await UpdateState(Source, e.StateName, e.IsNewState, e.UseTransitions);
        }

        private async Task UpdateState(IStateGroup stateGroup, string stateName, bool isNewState, bool useTransitions)
        {
            if (isNewState)
            {
                await stateGroup.ChangeToStateByName(stateName, useTransitions);
            }
            else
            {
                await stateGroup.ChangeBackToStateByName(stateName, useTransitions);
            }
        }
    }
}