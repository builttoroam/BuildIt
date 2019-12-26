using System.Collections.Generic;
using System.ComponentModel;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Defines a typed state group.
    /// </summary>
    /// <typeparam name="TState">The type of states.</typeparam>
    /// <typeparam name="TStateDefinition">The type of state definition.</typeparam>
    public interface ITypedStateGroupDefinition<TState, TStateDefinition>
        : IStateGroupDefinition<TStateDefinition>
        where TStateDefinition : class, ITypedStateDefinition<TState>, new()
    {
        /// <summary>
        /// Gets the typed state definitions.
        /// </summary>
        IReadOnlyDictionary<TState, ITypedStateDefinition<TState>> TypedStates { get; }

        /// <summary>
        /// Defines a new typed state definition.
        /// </summary>
        /// <param name="stateDefinition">The typed state definition to define.</param>
        /// <returns>The defined state definition (maybe existing definition if already defined).</returns>
        TStateDefinition DefineTypedState(TStateDefinition stateDefinition);

        /// <summary>
        /// Define a new typed state defintion based on an enum value.
        /// </summary>
        /// <param name="state">The typed state.</param>
        /// <returns>The defined state definition (maybe existing definition if already defined).</returns>
        TStateDefinition DefineTypedState(TState state);

        /// <summary>
        /// Defines a new typed state definition based on an enum value, with data.
        /// </summary>
        /// <typeparam name="TStateData">The type of the data.</typeparam>
        /// <param name="state">The state.</param>
        /// <returns>The defined state definition (maybe existing definition if already defined).</returns>
        ITypedStateDefinitionWithData<TState, TStateDefinition, TStateData> DefineTypedStateWithData<TStateData>(TState state)
            where TStateData : INotifyPropertyChanged;

        /// <summary>
        /// Whether the supplied enum value is the default state.
        /// </summary>
        /// <param name="enumDef">The type (enum) of the state group.</param>
        /// <returns>True if it is the default state.</returns>
        bool IsDefaultState(TState enumDef);
    }
}