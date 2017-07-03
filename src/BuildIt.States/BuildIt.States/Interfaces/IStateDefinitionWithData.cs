using System.ComponentModel;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Wrapper for state definition coupled with data entity
    /// </summary>
    /// <typeparam name="TData">The type of data entity</typeparam>
    public interface IStateDefinitionWithData<TData> where TData : INotifyPropertyChanged
    {
        /// <summary>
        /// The state definition
        /// </summary>
        IStateDefinition State { get; }

        /// <summary>
        /// The associated state data entity
        /// </summary>
        IStateDefinitionTypedDataWrapper<TData> StateData { get; } 
    }
}