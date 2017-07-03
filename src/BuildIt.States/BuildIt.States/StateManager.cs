using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildIt.States.Interfaces;

namespace BuildIt.States
{
    /// <summary>
    /// 
    /// </summary>
    public class StateManager : IStateManager//, ICanRegisterDependencies
    {
        public event EventHandler GoToPreviousStateIsBlockedChanged;

        private readonly Dictionary<string, IStateGroup> stateGroups =
            new Dictionary<string, IStateGroup>();

        public IReadOnlyDictionary<string, IStateGroup> StateGroups => stateGroups;

        public IEnumStateGroup<TState> EnumStateGroup<TState>() where TState : struct
        {
            return StateGroup(typeof(TState).Name) as IEnumStateGroup<TState>;
        }

        public IStateGroup StateGroup(string groupName)
        {
            return StateGroups.SafeValue(groupName);
        }

        public void AddStateGroup(IStateGroup group)
        {
            stateGroups[group.GroupName] = group;
            group.GoToPreviousStateIsBlockedChanged += Group_IsBlockedChanged;
        }

        private void Group_IsBlockedChanged(object sender, EventArgs e)
        {
            GoToPreviousStateIsBlockedChanged.SafeRaise(this);
        }

        public TState CurrentState<TState>()
            where TState : struct
        {
            var group = EnumStateGroup<TState>();
            if (group == null) return default(TState);
            return group.CurrentEnumState;
        }

        public string CurrentState(string groupName)
        {
            var group = StateGroup(groupName);
            return @group?.CurrentStateName;
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


        public async Task<bool> GoToState(string groupName, string stateName, bool animate = true)
        {
            var group = StateGroup(groupName);
            if (group == null) return false;
            return await group.ChangeTo(stateName, animate);
        }

        public async Task<bool> GoToStateWithData<TData>(string groupName, string stateName, TData data, bool animate = true)
        {
            var group = StateGroup(groupName);
            if (group == null) return false;
            return await group.ChangeToWithData(stateName, data, animate);
        }
        public async Task<bool> GoBackToState(string groupName, string stateName, bool animate = true)
        {
            var group = StateGroup(groupName);
            if (group == null) return false;
            return await group.ChangeBackTo(stateName, animate);
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

        

       
    }
}