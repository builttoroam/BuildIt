using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Typed state group
    /// </summary>
    /// <typeparam name="TState">The type (enum) of the state group</typeparam>
    public interface IEnumStateGroup<TState> :
        IStateGroup,
        INotifyEnumStateChanged<TState>,
        INotifyEnumStateChanging<TState>
        where TState : struct
    {
        /// <summary>
        /// Gets the current typed state
        /// </summary>
        TState CurrentEnumState { get; }

        /// <summary>
        /// Gets returns the state definition for the current state
        /// </summary>
        IReadOnlyDictionary<TState, IEnumStateDefinition<TState>> EnumStates { get; }

        /// <summary>
        /// Defines a new typed state definition
        /// </summary>
        /// <param name="stateDefinition">The typed state definition to define</param>
        /// <returns>The defined state definition (maybe existing definition if already defined)</returns>
        IEnumStateDefinition<TState> DefineEnumState(IEnumStateDefinition<TState> stateDefinition);

        /// <summary>
        /// Define a new typed state defintion based on an enum value
        /// </summary>
        /// <param name="state">The typed state</param>
        /// <returns>The defined state definition (maybe existing definition if already defined)</returns>
        IEnumStateDefinition<TState> DefineEnumState(TState state);

        /// <summary>
        /// Defines a new typed state definition based on an enum value, with data
        /// </summary>
        /// <typeparam name="TStateData">The type of the data</typeparam>
        /// <param name="state">The state</param>
        /// <returns>The defined state definition (maybe existing definition if already defined)</returns>
        IEnumStateDefinitionWithData<TState, TStateData> DefineEnumStateWithData<TStateData>(TState state)
            where TStateData : INotifyPropertyChanged;

        /// <summary>
        /// Defines all states for the enum type
        /// </summary>
        void DefineAllStates();

        /// <summary>
        /// Change to typed state
        /// </summary>
        /// <param name="newState">The new state</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <returns>Success if change is completed</returns>
        Task<bool> ChangeTo(TState newState, bool useTransitions = true);

        /// <summary>
        /// Change to typed state, with data
        /// </summary>
        /// <typeparam name="TData">The type of data to pass to new state</typeparam>
        /// <param name="newState">The new state</param>
        /// <param name="data">The data to pass to the new state</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <returns>Success if change is completed</returns>
        Task<bool> ChangeToWithData<TData>(TState newState, TData data, bool useTransitions = true);

        /// <summary>
        /// Changes back to a typed state
        /// </summary>
        /// <param name="newState">The state to change back to</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <returns>Success if change is completed</returns>
        Task<bool> ChangeBackTo(TState newState, bool useTransitions = true);
    }
}