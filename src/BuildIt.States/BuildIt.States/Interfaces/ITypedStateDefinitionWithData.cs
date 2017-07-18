using System.ComponentModel;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Wrapper for state definition coupled with data entity
    /// </summary>
    /// <typeparam name="TState">The type of state definition</typeparam>
    /// <typeparam name="TData">The type of data entity</typeparam>
    public interface ITypedStateDefinitionWithData<TState, TData> : IStateDefinitionWithData<TData>
    where TData : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the enum state definition
        /// </summary>
        ITypedStateDefinition<TState> TypedState { get; }
    }
}