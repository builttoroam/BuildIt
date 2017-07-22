using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildIt.States.Interfaces;
using BuildIt.States.Typed.Enum;

namespace BuildIt.States
{
    /// <summary>
    /// Manager class for interacting with states
    /// </summary>
    public class StateManager : IStateManager// , ICanRegisterDependencies
    {
        private readonly Dictionary<string, IStateGroup> stateGroups =
            new Dictionary<string, IStateGroup>();

        /// <summary>
        /// Event to indicate that going to previous state has been blocked by one of the active states
        /// </summary>
        public event EventHandler GoToPreviousStateIsBlockedChanged;

        /// <summary>
        /// Gets the currently defined state groups
        /// </summary>
        public IReadOnlyDictionary<string, IStateGroup> StateGroups => stateGroups;

        /// <summary>
        /// Gets a value indicating whether indicates if there is a previous state (in any state group)
        /// </summary>
        public bool PreviousStateExists => StateGroups.Select(stateGroup => stateGroup.Value).Any(grp => grp.TrackHistory && grp.HasHistory);

        /// <summary>
        /// Gets a value indicating whether whether going to previous state is currently blocked
        /// </summary>
        public bool GoToPreviousStateIsBlocked
        {
            get
            {
                return PreviousStateExists &&
                    StateGroups.Select(stateGroup => stateGroup.Value).Any(grp => grp.GoToPreviousStateIsBlocked);
            }
        }

        /// <summary>
        /// Retrieves state group based on type
        /// </summary>
        /// <typeparam name="TState">The type (enum) to look up the state group</typeparam>
        /// <returns>The typed state group</returns>
        public ITypedStateGroup<TState, EnumStateDefinition<TState>, EnumStateGroupDefinition<TState>> TypedStateGroup<TState>()
            where TState : struct
        {
            return StateGroup(typeof(TState).Name) as ITypedStateGroup<TState, EnumStateDefinition<TState>, EnumStateGroupDefinition<TState>>;
        }

        /// <summary>
        /// Retrieves a state group by name
        /// </summary>
        /// <param name="groupName">The name of the state group to retrieve</param>
        /// <returns>The state group (or null)</returns>
        public IStateGroup StateGroup(string groupName)
        {
            return StateGroups.SafeValue(groupName);
        }

        /// <summary>
        /// Add a state group
        /// </summary>
        /// <param name="group">The state group to add</param>
        public void AddStateGroup(IStateGroup group)
        {
            stateGroups[group.GroupName] = group;
            group.GoToPreviousStateIsBlockedChanged += Group_IsBlockedChanged;
        }

        /// <summary>
        /// The current state for a particular state group
        /// </summary>
        /// <typeparam name="TState">the type (enum) to look up the state group</typeparam>
        /// <returns>The current state</returns>
        public TState CurrentState<TState>()
            where TState : struct
        {
            var group = TypedStateGroup<TState>();
            if (group == null)
            {
                return default(TState);
            }

            return group.CurrentState;
        }

        /// <summary>
        /// The current state for a particular state group
        /// </summary>
        /// <param name="groupName">The name of the state group to retrieve</param>
        /// <returns>The current state</returns>
        public string CurrentState(string groupName)
        {
            var group = StateGroup(groupName);
            return @group?.CurrentStateName;
        }

        /// <summary>
        /// Go to a new state
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state to go to</typeparam>
        /// <param name="state">The state to go to</param>
        /// <param name="animate">Whether to animate the transition</param>
        /// <returns>Whether the transition was successful</returns>
        public async Task<bool> GoToState<TState>(TState state, bool animate = true)
            where TState : struct
        {
            var group = TypedStateGroup<TState>(); // StateGroups.SafeValue(typeof(TState));
            if (group == null)
            {
                return false;
            }

            return await group.ChangeToState(state, animate);
        }

        /// <summary>
        /// Transitions to a new state, passing in data
        /// </summary>
        /// <typeparam name="TState">The type (enum) of state to go to</typeparam>
        /// <typeparam name="TData">The type of data to be passed to new state</typeparam>
        /// <param name="state">The new state to go to</param>
        /// <param name="data">The data to pass to the new state</param>
        /// <param name="animate">Whether the transition should be animated</param>
        /// <returns>Whether the transition was successful</returns>
        public async Task<bool> GoToStateWithData<TState, TData>(TState state, TData data, bool animate = true)
            where TState : struct
        {
            var group = TypedStateGroup<TState>(); // StateGroups.SafeValue(typeof(TState));
            if (group == null)
            {
                return false;
            }

            return await group.ChangeToStateWithData(state, data, animate);
        }

        /// <summary>
        /// Go to state by going back over history of state changes
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state to go to</typeparam>
        /// <param name="state">The state to go to</param>
        /// <param name="animate">Whether the transition should be animated</param>
        /// <returns>Whether the transition was successful</returns>
        public async Task<bool> GoBackToState<TState>(TState state, bool animate = true)
            where TState : struct
        {
            var group = TypedStateGroup<TState>(); // StateGroups.SafeValue(typeof(TState));
            if (group == null)
            {
                return false;
            }

            return await group.ChangeBackToState(state, animate);
        }

        /// <summary>
        /// Go to a new state
        /// </summary>
        /// <param name="groupName">The state group name</param>
        /// <param name="stateName">The state name</param>
        /// <param name="animate">Whether to animate transition</param>
        /// <returns>Whether the transition was successful</returns>
        public async Task<bool> GoToState(string groupName, string stateName, bool animate = true)
        {
            var group = StateGroup(groupName);
            if (group == null)
            {
                return false;
            }

            return await group.ChangeToStateByName(stateName, animate);
        }

        /// <summary>
        /// Go to a new state, passing in data
        /// </summary>
        /// <typeparam name="TData">The type of data to be passed to the new state</typeparam>
        /// <param name="groupName">The state group name</param>
        /// <param name="stateName">The state name</param>
        /// <param name="data">The data to be passed in</param>
        /// <param name="animate">Whether to animate transition</param>
        /// <returns>Whether the transition was successful</returns>
        public async Task<bool> GoToStateWithData<TData>(string groupName, string stateName, TData data, bool animate = true)
        {
            var group = StateGroup(groupName);
            if (group == null)
            {
                return false;
            }

            return await group.ChangeToStateByNameWithData(stateName, data, animate);
        }

        /// <summary>
        /// Go to state by going back over history of state changes
        /// </summary>
        /// <param name="groupName">The state group name</param>
        /// <param name="stateName">The state name</param>
        /// <param name="animate">Whether the transition should be animated</param>
        /// <returns>Whether the transition was successful</returns>
        public async Task<bool> GoBackToState(string groupName, string stateName, bool animate = true)
        {
            var group = StateGroup(groupName);
            if (group == null)
            {
                return false;
            }

            return await group.ChangeBackToStateByName(stateName, animate);
        }

        /// <summary>
        /// Go back to the previous state
        /// </summary>
        /// <param name="animate">whether to animate the transition</param>
        /// <returns>Whether the transition was successful</returns>
        public async Task<bool> GoBackToPreviousState(bool animate = true)
        {
            foreach (var stateGroup in StateGroups)
            {
                var grp = stateGroup.Value;
                if (grp.TrackHistory && grp.HasHistory)
                {
                    return await grp.ChangeToPrevious(animate);
                }
            }

            return false;
        }

        /// <summary>
        /// Bind two different state managers
        /// </summary>
        /// <param name="managerToBindTo">The state manager to listen to for changes</param>
        /// <param name="bothDirections">Whether updates to states should go both ways</param>
        /// <returns>Binder that can be used to disconnect the state managers</returns>
        public async Task<IStateBinder> Bind(IStateManager managerToBindTo, bool bothDirections = true)
        {
            var binder = new StateManagerBinder(this, managerToBindTo, bothDirections);
            await binder.Bind();
            return binder;
        }

        private void Group_IsBlockedChanged(object sender, EventArgs e)
        {
            GoToPreviousStateIsBlockedChanged.SafeRaise(this);
        }
    }
}