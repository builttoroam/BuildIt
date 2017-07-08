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
    public class EnumStateGroup<TState> : StateGroup, IEnumStateGroup<TState>
        where TState : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumStateGroup{TState}"/> class.
        /// </summary>
        public EnumStateGroup()
        {
            if (!typeof(TState).GetTypeInfo().IsEnum)
            {
                throw new ArgumentException("Type argument should be enum", nameof(TState));
            }
        }

#pragma warning disable CS0067 // See TODO
        // TODO: Raise events at correct point when changing state

        /// <summary>
        /// Typed state changed event
        /// </summary>
        public event EventHandler<EnumStateEventArgs<TState>> EnumStateChanged;

        /// <summary>
        /// Typed state changing event
        /// </summary>
        public event EventHandler<EnumStateCancelEventArgs<TState>> EnumStateChanging;
#pragma warning restore CS0067

        /// <summary>
        /// Gets the name of the state group (based on enum type)
        /// </summary>
        public override string GroupName => typeof(TState).Name;

        /// <summary>
        /// Gets the typed state definitions
        /// </summary>
        public IReadOnlyDictionary<TState, IEnumStateDefinition<TState>> EnumStates => (from s in States
                                                                                        let ekey = Utilities.EnumParse<TState>(s.Key)
                                                                                        let eval = s.Value as IEnumStateDefinition<TState>
                                                                                        select new { Key = ekey, Value = eval })
            .ToDictionary(x => x.Key, y => y.Value);

        /// <summary>
        /// Gets the current typed state
        /// </summary>
        public TState CurrentEnumState { get; private set; }

        /// <summary>
        /// Gets or sets the current state name
        /// </summary>
        public override string CurrentStateName
        {
            get => (!default(TState).Equals(CurrentEnumState)) ? CurrentEnumState + string.Empty : null;
            protected set => CurrentEnumState = value.EnumParse<TState>();
        }

        /// <summary>
        /// Gets returns the state definition for the current state
        /// </summary>
        public IStateDefinition CurrentEnumStateDefinition
            => States.SafeValue(CurrentStateName) as IEnumStateDefinition<TState>;

        /// <summary>
        /// Defines all states for the enum type
        /// </summary>
        public void DefineAllStates()
        {
            var vals = Enum.GetValues(typeof(TState));
            foreach (var enumVal in vals)
            {
                $"Defining state {enumVal}".Log();
                DefineEnumState((TState)enumVal);
            }
        }

        /// <summary>
        /// Defines a new typed state definition
        /// </summary>
        /// <param name="stateDefinition">The typed state definition to define</param>
        /// <returns>The defined state definition (maybe existing definition if already defined)</returns>
        public virtual IEnumStateDefinition<TState> DefineEnumState(IEnumStateDefinition<TState> stateDefinition)
        {
            return DefineState(stateDefinition) as IEnumStateDefinition<TState>;
        }

        /// <summary>
        /// Define a new typed state defintion based on an enum value
        /// </summary>
        /// <param name="state">The typed state</param>
        /// <returns>The defined state definition (maybe existing definition if already defined)</returns>
        public virtual IEnumStateDefinition<TState> DefineEnumState(TState state)
        {
            // Don't ever add the default value (eg Base) state
            if (default(TState).Equals(state))
            {
                "Attempted to add the default state definition".Log();
                return null;
            }

            var stateDefinition = new EnumStateDefinition<TState>(state);
            $"Defined state for {state}".Log();
            return DefineEnumState(stateDefinition);
        }

        /// <summary>
        /// Defines a new typed state definition based on an enum value, with data
        /// </summary>
        /// <typeparam name="TStateData">The type of the data</typeparam>
        /// <param name="state">The state</param>
        /// <returns>The defined state definition (maybe existing definition if already defined)</returns>
        public virtual IEnumStateDefinitionWithData<TState, TStateData> DefineEnumStateWithData<TStateData>(TState state)
            where TStateData : INotifyPropertyChanged
        {
            // Don't ever add the default value (eg Base) state
            if (default(TState).Equals(state))
            {
                "Attempted to add the default state definition".Log();
                return null;
            }

            $"Defining state for {typeof(TState).Name} with data type {typeof(TStateData)}".Log();
            var stateDefinition = new EnumStateDefinition<TState>(state)
            {
                UntypedStateDataWrapper = new StateDefinitionTypedDataWrapper<TStateData>()
            };
            var stateDef = DefineEnumState(stateDefinition);
            return new EnumStateDefinitionWithDataWrapper<TState, TStateData> { State = stateDef };
        }

        /// <summary>
        /// Change to typed state, with data
        /// </summary>
        /// <typeparam name="TData">The type of data to pass to new state</typeparam>
        /// <param name="findState">The new state</param>
        /// <param name="data">The data to pass to the new state</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <returns>Success if change is completed</returns>
        public async Task<bool> ChangeToWithData<TData>(TState findState, TData data, bool useTransitions = true)
        {
            return await ChangeToWithData(findState + string.Empty, data, useTransitions);
        }

        /// <summary>
        /// Change to typed state
        /// </summary>
        /// <param name="findState">The new state</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <returns>Success if change is completed</returns>
        public async Task<bool> ChangeTo(TState findState, bool useTransitions = true)
        {
            return await ChangeTo(findState + string.Empty, useTransitions);
        }

        /// <summary>
        /// Changes back to a typed state
        /// </summary>
        /// <param name="findState">The state to change back to</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <returns>Success if change is completed</returns>
        public async Task<bool> ChangeBackTo(TState findState, bool useTransitions = true)
        {
            if (TrackHistory == false)
            {
                throw new Exception("History tracking not enabled");
            }

            return await ChangeBackTo(findState + string.Empty, useTransitions);
        }

        /// <summary>
        /// Adds a state definition into the group
        /// </summary>
        /// <param name="stateDefinition">The state definition to add</param>
        /// <returns>The added (or existing) state definition</returns>
        public override IStateDefinition DefineState(IStateDefinition stateDefinition)
        {
            if (!(stateDefinition is IEnumStateDefinition<TState>))
            {
                throw new ArgumentException("State definition should be IEnumStateDefinition<TState> for typed state group", nameof(stateDefinition));
            }

            return base.DefineState(stateDefinition);
        }

        /// <summary>
        /// Defines a state definition
        /// </summary>
        /// <param name="state">The state name - should be an enum value</param>
        /// <returns>New state definition</returns>
        public override IStateDefinition DefineState(string state)
        {
            if (state.EnumParse<TState>().Equals(default(TState)))
            {
                throw new ArgumentException("State name must match an enum value", nameof(state));
            }

            return base.DefineState(state);
        }

        /// <summary>
        /// Overridden as not supported - use <see cref="M:DefineEnumStateWithData"/> instead
        /// </summary>
        /// <typeparam name="TStateData">The type of the state data</typeparam>
        /// <param name="state">The state</param>
        /// <returns>New state definition</returns>
        /// <exception cref="NotSupportedException">Raised in all cases as method not supported</exception>
        public override IStateDefinitionWithData<TStateData> DefineStateWithData<TStateData>(string state)
        {
            throw new NotSupportedException("Don't use for EnumStateGroup. Use DefineEnumStateWithData instead");
        }

        /// <summary>
        /// Whether the supplied enum value is the default state
        /// </summary>
        /// <param name="enumDef">The type (enum) of the state group</param>
        /// <returns>True if it is the default state</returns>
        protected bool IsDefaultState(TState enumDef)
        {
            return default(TState).Equals(enumDef);
        }
    }
}