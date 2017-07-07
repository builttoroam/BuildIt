using System.ComponentModel;

namespace BuildIt.States.Interfaces.Builder
{
    /// <summary>
    /// Builder for state definition with state data
    /// </summary>
    /// <typeparam name="TState">The type (enum) of the state</typeparam>
    /// <typeparam name="TData">The type of the state data</typeparam>
    public interface IStateDefinitionWithDataBuilder<TState, TData> : IStateDefinitionBuilder<TState>
        where TData : INotifyPropertyChanged
        where TState : struct
    {
        /// <summary>
        /// Gets the state data wrapper
        /// </summary>
        IStateDefinitionTypedDataWrapper<TData> StateDataWrapper { get; }
    }
}