using BuildIt.States.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BuildIt.States
{
    /// <summary>
    /// Definition of a typed state group
    /// </summary>
    /// <typeparam name="TState">The state type</typeparam>
    public class TypedStateGroupDefinition<TState> : BaseStateGroupDefinition, ITypedStateGroupDefinition<TState>
    {
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
            if (default(TState)?.Equals(state) ?? false)
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
        /// Allow all states for a group to be defined (only works for Enum groups atm)
        /// </summary>
        public virtual void DefineAllStates()
        {
        }

        /// <summary>
        /// Whether the supplied enum value is the default state
        /// </summary>
        /// <param name="enumDef">The type (enum) of the state group</param>
        /// <returns>True if it is the default state</returns>
        public bool IsDefaultState(TState enumDef)
        {
            return default(TState).Equals(enumDef);
        }

        /// <summary>
        /// Converts string statename to the type of state
        /// </summary>
        /// <param name="stateName">The state name</param>
        /// <returns>The parsed state</returns>
        public virtual TState FromString(string stateName) => default(TState);
    }
}