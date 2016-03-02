using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildIt.States
{
    public class StateManager : IStateManager//, ICanRegisterDependencies
    {
        public event EventHandler GoToPreviousStateIsBlockedChanged;

        private readonly Dictionary<Type, IStateGroup> stateGroups =
            new Dictionary<Type, IStateGroup>();

        public IReadOnlyDictionary<Type, IStateGroup> StateGroups => stateGroups;

        public void AddStateGroup<TState>(IStateGroup<TState> group)
            where TState : struct
        {
            AddStateGroup(typeof(TState), group);
        }

        public void AddStateGroup(Type state, IStateGroup group)
        {
            stateGroups[state] = group;
            group.GoToPreviousStateIsBlockedChanged += Group_IsBlockedChanged;
        }

        private void Group_IsBlockedChanged(object sender, EventArgs e)
        {
            GoToPreviousStateIsBlockedChanged.SafeRaise(this);
        }

        public TState CurrentState<TState>()
            where TState : struct
        {
            var group = StateGroups.SafeValue(typeof(TState)) as IStateGroup<TState>;
            if (group == null) return default(TState);
            return group.CurrentState;
        }

        public async Task<bool> GoToState<TState>(TState state, bool animate = true)
            where TState : struct
        {
            var group = StateGroups.SafeValue(typeof(TState));
            if (group == null) return false;
            return await group.ChangeTo(state, animate);
        }

        public async Task<bool> GoToStateWithData<TState, TData>(TState state, TData data, bool animate = true)
           where TState : struct
        {
            var group = StateGroups.SafeValue(typeof(TState));
            if (group == null) return false;
            return await group.ChangeToWithData(state, data, animate);
        }
        public async Task<bool> GoBackToState<TState>(TState state, bool animate = true) where TState : struct
        {
            var group = StateGroups.SafeValue(typeof(TState));
            if (group == null) return false;
            return await group.ChangeBackTo(state, animate);
        }

        public async Task<bool> GoBackToPreviousState(bool animate = true)
        {
            foreach (var stateGroup in StateGroups)
            {
                var grp = stateGroup.Value;
                if (grp.TrackHistory && grp.HasHistory) return await grp.ChangeToPrevious(animate);
            }
            return false;
        }

        public bool PreviousStateExists
        {
            get
            {
                return StateGroups.Select(stateGroup => stateGroup.Value).Any(grp => grp.TrackHistory && grp.HasHistory);
            }
        }

        public bool GoToPreviousStateIsBlocked
        {
            get
            {
                return PreviousStateExists &&
                    StateGroups.Select(stateGroup => stateGroup.Value).Any(grp => grp.GoToPreviousStateIsBlocked);
            }
        }

        public IStateBinder Bind(IStateManager managerToBindTo)
        {
            return new StateManagerBinder(this, managerToBindTo);
        }

        private class StateManagerBinder : IStateBinder
        {
            IStateManager StateManager { get; }
            IStateManager StateManagerToBindTo { get; }

            IList<IStateBinder> GroupBinders { get; } = new List<IStateBinder>();
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