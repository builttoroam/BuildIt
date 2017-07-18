using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BuildIt.States.Interfaces;

namespace BuildIt.States
{

    /// <summary>
    /// A typed state group
    /// </summary>
    /// <typeparam name="TState">The type (enum) fo the state group</typeparam>
    public class TypedStateGroup<TState> : BaseStateGroup, ITypedStateGroup<TState>
    {
#pragma warning disable CS0067 // See TODO
        // TODO: Raise events at correct point when changing state

        /// <summary>
        /// Typed state changed event
        /// </summary>
        public event EventHandler<TypedStateEventArgs<TState>> TypedStateChanged;

        /// <summary>
        /// Typed state changing event
        /// </summary>
        public event EventHandler<TypedStateCancelEventArgs<TState>> TypedStateChanging;
#pragma warning restore CS0067

        /// <summary>
        /// Gets the name of the state group (based on enum type)
        /// </summary>
        public override string GroupName => typeof(TState).Name;

        /// <summary>
        /// Gets the typed state definitions
        /// </summary>
        public IReadOnlyDictionary<TState, ITypedStateDefinition<TState>> TypedStates => (from s in States
                                                                                         let ekey = FromString(s.Key)
                                                                                         let eval = s.Value as ITypedStateDefinition<TState>
                                                                                         select new { Key = ekey, Value = eval })
            .ToDictionary(x => x.Key, y => y.Value);

        /// <summary>
        /// Gets or sets the current typed state
        /// </summary>
        public TState CurrentState { get; protected set; }

        /// <summary>
        /// Gets or sets the current state name
        /// </summary>
        public override string CurrentStateName
        {
            get => (!default(TState).Equals(CurrentState)) ? CurrentState + string.Empty : null;
            protected set => CurrentState = FromString(value);
        }

        /// <summary>
        /// Gets returns the state definition for the current state
        /// </summary>
        public ITypedStateDefinition<TState> CurrentTypedStateDefinition
            => States.SafeValue(CurrentStateName) as ITypedStateDefinition<TState>;

        /// <summary>
        /// Retrieves an instance of the typed state definition, if it exists
        /// </summary>
        /// <param name="state">The state name to look up</param>
        /// <returns>The existing state definition</returns>
        public ITypedStateDefinition<TState> TypedStateDefinition(string state)
        {
            return (string.IsNullOrWhiteSpace(state) ? null : States.SafeValue(state)) as ITypedStateDefinition<TState>;
        }

        /// <summary>
        /// Defines a new typed state definition
        /// </summary>
        /// <param name="stateDefinition">The typed state definition to define</param>
        /// <returns>The defined state definition (maybe existing definition if already defined)</returns>
        public ITypedStateDefinition<TState> DefineTypedState(ITypedStateDefinition<TState> stateDefinition)
        {
            if (stateDefinition == null)
            {
                "Can't define null state definition".Log();
                return null;
            }

            var existing = TypedStateDefinition(stateDefinition.StateName);
            if (existing != null)
            {
                $"State definition already defined, returning existing instance - {existing.GetType().Name}".Log();
                return existing;
            }

            $"Defining state of type {stateDefinition.GetType().Name}".Log();
            States[stateDefinition.StateName] = stateDefinition;
            return stateDefinition;
        }

        /// <summary>
        /// Define a new typed state defintion based on an enum value
        /// </summary>
        /// <param name="state">The typed state</param>
        /// <returns>The defined state definition (maybe existing definition if already defined)</returns>
        public ITypedStateDefinition<TState> DefineTypedState(TState state)
        {
            // Don't ever add the default value (eg Base) state
            if (default(TState).Equals(state))
            {
                "Attempted to add the default state definition".Log();
                return null;
            }

            var stateDefinition = new TypedStateDefinition<TState>(state);
            $"Defined state for {state}".Log();
            return DefineTypedState(stateDefinition);
        }

        /// <summary>
        /// Defines a new typed state definition based on an enum value, with data
        /// </summary>
        /// <typeparam name="TStateData">The type of the data</typeparam>
        /// <param name="state">The state</param>
        /// <returns>The defined state definition (maybe existing definition if already defined)</returns>
        public virtual ITypedStateDefinitionWithData<TState, TStateData> DefineTypedStateWithData<TStateData>(TState state)
            where TStateData : INotifyPropertyChanged
        {
            // Don't ever add the default value (eg Base) state
            if (default(TState).Equals(state))
            {
                "Attempted to add the default state definition".Log();
                return null;
            }

            $"Defining state for {typeof(TState).Name} with data type {typeof(TStateData)}".Log();
            var stateDefinition = new TypedStateDefinition<TState>(state)
            {
                UntypedStateDataWrapper = new StateDefinitionTypedDataWrapper<TStateData>()
            };
            var stateDef = DefineTypedState(stateDefinition);
            return new TypedStateDefinitionWithDataWrapper<TState, TStateData> { State = stateDef };
        }

        /// <summary>
        /// Change to typed state, with data
        /// </summary>
        /// <typeparam name="TData">The type of data to pass to new state</typeparam>
        /// <param name="findState">The new state</param>
        /// <param name="data">The data to pass to the new state</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <returns>Success if change is completed</returns>
        public async Task<bool> ChangeToStateWithData<TData>(TState findState, TData data, bool useTransitions = true)
        {
            return await ChangeToStateByNameWithData(findState + string.Empty, data, useTransitions);
        }

        /// <summary>
        /// Change to typed state
        /// </summary>
        /// <param name="findState">The new state</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <returns>Success if change is completed</returns>
        public async Task<bool> ChangeToState(TState findState, bool useTransitions = true)
        {
            return await ChangeToStateByName(findState + string.Empty, useTransitions);
        }

        /// <summary>
        /// Changes back to a typed state
        /// </summary>
        /// <param name="findState">The state to change back to</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <returns>Success if change is completed</returns>
        public async Task<bool> ChangeBackToState(TState findState, bool useTransitions = true)
        {
            if (TrackHistory == false)
            {
                throw new Exception("History tracking not enabled");
            }

            return await ChangeBackToStateByName(findState + string.Empty, useTransitions);
        }

        public virtual void DefineAllStates()
        {
        }

        ///// <summary>
        ///// Adds a state definition into the group
        ///// </summary>
        ///// <param name="stateDefinition">The state definition to add</param>
        ///// <returns>The added (or existing) state definition</returns>
        // public override IStateDefinition DefineState(IStateDefinition stateDefinition)
        // {
        //    if (!(stateDefinition is IEnumStateDefinition<TState>))
        //    {
        //        throw new ArgumentException("State definition should be IEnumStateDefinition<TState> for typed state group", nameof(stateDefinition));
        //    }

        // return base.DefineState(stateDefinition);
        // }

        ///// <summary>
        ///// Defines a state definition
        ///// </summary>
        ///// <param name="state">The state name - should be an enum value</param>
        ///// <returns>New state definition</returns>
        // public override IStateDefinition DefineState(string state)
        // {
        //    if (state.EnumParse<TState>().Equals(default(TState)))
        //    {
        //        throw new ArgumentException("State name must match an enum value", nameof(state));
        //    }

        // return base.DefineState(state);
        // }

        ///// <summary>
        ///// Overridden as not supported - use <see cref="M:DefineEnumStateWithData"/> instead
        ///// </summary>
        ///// <typeparam name="TStateData">The type of the state data</typeparam>
        ///// <param name="state">The state</param>
        ///// <returns>New state definition</returns>
        ///// <exception cref="NotSupportedException">Raised in all cases as method not supported</exception>
        // public override IStateDefinitionWithData<TStateData> DefineStateWithData<TStateData>(string state)
        // {
        //    throw new NotSupportedException("Don't use for EnumStateGroup. Use DefineEnumStateWithData instead");
        // }

        ///// <summary>
        ///// Returns a new instance of the typed state definition
        ///// </summary>
        ///// <param name="stateName">The state name</param>
        ///// <returns>State definition</returns>
        // protected virtual TypedStateDefinition<TState> CreateTypedStateDefinition(TState stateName)
        // {
        //    return new TypedStateDefinition<TState>(stateName);
        // }

        /// <summary>
        /// Whether the supplied enum value is the default state
        /// </summary>
        /// <param name="enumDef">The type (enum) of the state group</param>
        /// <returns>True if it is the default state</returns>
        protected bool IsDefaultState(TState enumDef)
        {
            return default(TState).Equals(enumDef);
        }

        /// <summary>
        /// Converts string statename to the type of state
        /// </summary>
        /// <param name="stateName">The state name</param>
        /// <returns>The parsed state</returns>
        protected virtual TState FromString(string stateName) => default(TState);
    }
}