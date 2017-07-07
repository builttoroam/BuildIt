using System.ComponentModel;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Defines properties and methods for a state definition based on an enum
    /// </summary>
    /// <typeparam name="TState">The enum type</typeparam>
    /// <typeparam name="TData">The type of data to be associated with the state</typeparam>
    public interface IEnumStateDefinitionWithData<TState, TData>: IStateDefinitionWithData<TData>
        where TData : INotifyPropertyChanged
        where TState : struct
    {
        /// <summary>
        /// Gets the enum state definition
        /// </summary>
        IEnumStateDefinition<TState> EnumState { get; }
    }
}