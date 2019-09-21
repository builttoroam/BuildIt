using System.ComponentModel;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Wrapper for state definition coupled with data entity.
    /// </summary>
    /// <typeparam name="TState">The type of state.</typeparam>
    /// <typeparam name="TStateDefinition">The type of state definition.</typeparam>
    /// <typeparam name="TData">The type of data entity.</typeparam>
    // ReSharper disable once TypeParameterCanBeVariant - Not Required
    public interface ITypedStateDefinitionWithData<TState, TStateDefinition, TData>
        : IStateDefinitionWithData<TData>
        where TStateDefinition : class, ITypedStateDefinition<TState>, new()
        where TData : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the enum state definition.
        /// </summary>
        TStateDefinition TypedState { get; }
    }
}