using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildIt.States.Interfaces;

namespace BuildIt.States
{
    public class StateManager : IStateManager//, ICanRegisterDependencies
    {
        public event EventHandler GoToPreviousStateIsBlockedChanged;

        private readonly Dictionary<Type, IStateGroup> stateGroups =
            new Dictionary<Type, IStateGroup>();

        public IReadOnlyDictionary<Type, IStateGroup> StateGroups => stateGroups;

        public IEnumStateGroup<TState> EnumStateGroup<TState>() where TState:struct
        {
            return StateGroups.SafeValue(typeof(TState)) as IEnumStateGroup<TState>;
        }

        public void AddStateGroup<TState>(IEnumStateGroup<TState> group)
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
            var group = StateGroups.SafeValue(typeof(TState)) as IEnumStateGroup<TState>;
            if (group == null) return default(TState);
            return group.CurrentEnumState;
        }

        public async Task<bool> GoToState<TState>(TState state, bool animate = true)
            where TState : struct
        {
            var group = EnumStateGroup<TState>();//StateGroups.SafeValue(typeof(TState));
            if (group == null) return false;
            return await group.ChangeTo(state, animate);
        }

        public async Task<bool> GoToStateWithData<TState, TData>(TState state, TData data, bool animate = true)
           where TState : struct
        {
            var group = EnumStateGroup<TState>();// StateGroups.SafeValue(typeof(TState));
            if (group == null) return false;
            return await group.ChangeToWithData(state, data, animate);
        }
        public async Task<bool> GoBackToState<TState>(TState state, bool animate = true) where TState : struct
        {
            var group = EnumStateGroup<TState>();// StateGroups.SafeValue(typeof(TState));
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

        public IStateBinder Bind(IStateManager managerToBindTo, bool bothDirections=true)
        {
            return new StateManagerBinder(this, managerToBindTo, bothDirections);
        }

        private class StateManagerBinder : IStateBinder
        {
            IStateManager StateManager { get; }
            IStateManager StateManagerToBindTo { get; }

            IList<IStateBinder> GroupBinders { get; } = new List<IStateBinder>();
            public StateManagerBinder(IStateManager sm, IStateManager manager, bool bothDirections=true)
            {
                StateManager = sm;
                StateManagerToBindTo = manager;

                foreach (var kvp in manager.StateGroups)
                {
                    var sg = StateManager.StateGroups.SafeValue(kvp.Key);
                    var binder = sg?.Bind(kvp.Value, bothDirections);
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

       
    }
}