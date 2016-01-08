using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuildIt.States
{
    public class StateManager : IStateManager//, ICanRegisterDependencies
    {
        public IDictionary<Type, IStateGroup> StateGroups { get; } =
            new Dictionary<Type, IStateGroup>();

        public async Task<bool> GoToState<TState>(TState state, bool animate = true)
            where TState : struct
        {
            var group = StateGroups.SafeValue(state.GetType());
            if (group == null) return false;
            return await group.ChangeTo(state,animate);
        }

        public async  Task<bool> GoBackToState<TState>(TState state, bool animate = true) where TState : struct
        {
            var group = StateGroups.SafeValue(state.GetType());
            if (group == null) return false;
            return await group.ChangeBackTo(state, animate);
    }

        public async Task<bool> GoBackToPreviousState(bool animate = true)
        {
            foreach (var stateGroup in StateGroups)
            {
                var grp = stateGroup.Value;
                if (grp.TrackHistory) return await grp.ChangeToPrevious(animate);
            }
            return false;
        }

        public IStateBinder Bind(IStateManager managerToBindTo)
        {
            return new StateManagerBinder(this, managerToBindTo);
        }

        private class StateManagerBinder : IStateBinder
        {
            IStateManager StateManager { get; }
            IStateManager StateManagerToBindTo { get; }

            IList<IStateBinder> GroupBinders { get; }=new List<IStateBinder>(); 
            public StateManagerBinder(IStateManager sm, IStateManager manager)
            {
                StateManager = sm;
                StateManagerToBindTo = manager;

                foreach (var kvp in manager.StateGroups)
                {
                    var sg = StateManager.StateGroups.SafeValue(kvp.Key);
                    if (sg == null) continue;
                    var binder = sg.Bind(kvp.Value);
                    if (binder == null) continue;
                    GroupBinders.Add(binder);
                }
            }

            public void Unbind()
            {
                foreach (var groupBinder in GroupBinders)
                {
                    groupBinder.Unbind();
                }
            }
        }

        //public void RegisterDependencies(IContainer container)
        //{
        //    foreach (var stateGroup in StateGroups)
        //    {
        //        (stateGroup.Value as ICanRegisterDependencies)?.RegisterDependencies(container);
        //    }
        //}

        //public IUIExecutionContext UIContext { get; set; } 
        //public virtual void RegisterForUIAccess(IUIExecutionContext context)
        //{
        //    UIContext = context;

        //    foreach (var stateGroup in StateGroups)
        //    {
        //        (stateGroup.Value as IRegisterForUIAccess)?.RegisterForUIAccess(context);
        //    }
        //}
    }
}