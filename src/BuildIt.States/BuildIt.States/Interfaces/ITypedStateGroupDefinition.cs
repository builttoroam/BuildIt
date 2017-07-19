using System.Collections.Generic;
using System.ComponentModel;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Defines a typed state group
    /// </summary>
    /// <typeparam name="TState">The type of states</typeparam>
    public interface ITypedStateGroupDefinition<TState> : IStateGroupDefinition
    {
        /// <summary>
        /// Gets the typed state definitions
        /// </summary>
        IReadOnlyDictionary<TState, ITypedStateDefinition<TState>> TypedStates { get; }

        /// <summary>
        /// Retrieves an instance of the typed state definition, if it exists
        /// </summary>
        /// <param name="state">The state name to look up</param>
        /// <returns>The existing state definition</returns>
        ITypedStateDefinition<TState> TypedStateDefinition(string state);

        /// <summary>
        /// Defines a new typed state definition
        /// </summary>
        /// <param name="stateDefinition">The typed state definition to define</param>
        /// <returns>The defined state definition (maybe existing definition if already defined)</returns>
        ITypedStateDefinition<TState> DefineTypedState(ITypedStateDefinition<TState> stateDefinition);

        /// <summary>
        /// Define a new typed state defintion based on an enum value
        /// </summary>
        /// <param name="state">The typed state</param>
        /// <returns>The defined state definition (maybe existing definition if already defined)</returns>
        ITypedStateDefinition<TState> DefineTypedState(TState state);

        /// <summary>
        /// Defines a new typed state definition based on an enum value, with data
        /// </summary>
        /// <typeparam name="TStateData">The type of the data</typeparam>
        /// <param name="state">The state</param>
        /// <returns>The defined state definition (maybe existing definition if already defined)</returns>
        ITypedStateDefinitionWithData<TState, TStateData> DefineTypedStateWithData<TStateData>(TState state)
            where TStateData : INotifyPropertyChanged;

        /// <summary>
        /// Allow all states for a group to be defined (only works for Enum groups atm)
        /// </summary>
        void DefineAllStates();

        /// <summary>
        /// Whether the supplied enum value is the default state
        /// </summary>
        /// <param name="enumDef">The type (enum) of the state group</param>
        /// <returns>True if it is the default state</returns>
        bool IsDefaultState(TState enumDef);

        /// <summary>
        /// Converts string statename to the type of state
        /// </summary>
        /// <param name="stateName">The state name</param>
        /// <returns>The parsed state</returns>
        TState FromString(string stateName);
    }
}