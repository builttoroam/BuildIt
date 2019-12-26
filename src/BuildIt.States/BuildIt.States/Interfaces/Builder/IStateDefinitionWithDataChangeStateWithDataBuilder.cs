using System.ComponentModel;

namespace BuildIt.States.Interfaces.Builder
{
    /// <summary>
    /// Builder for state definition with data to be passed to state data.
    /// </summary>
    /// <typeparam name="TState">The type (enum) of the state.</typeparam>
    /// <typeparam name="TStateData">The type of the state data.</typeparam>
    /// <typeparam name="TNewStateData">The data to be passed to the state.</typeparam>
    public interface IStateDefinitionWithDataChangeStateWithDataBuilder<TState, TStateData, TNewStateData> :
        IStateDefinitionWithDataBuilder<TState, TStateData>
        where TStateData : INotifyPropertyChanged
        where TState : struct
    {
        /// <summary>
        /// Gets the new state to transition to.
        /// </summary>
        TState NewState { get; }
    }
}