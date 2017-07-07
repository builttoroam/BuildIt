using System.ComponentModel;
using BuildIt.States.Interfaces;

namespace BuildIt.States
{
    /// <summary>
    /// Defines properties and methods for a state definition based on an enum
    /// </summary>
    /// <typeparam name="TState">The enum type</typeparam>
    /// <typeparam name="TData">The type of data to be associated with the state</typeparam>
    public class EnumStateDefinitionWithDataWrapper<TState, TData> :
        StateDefinitionWithDataWrapper<TData>, IEnumStateDefinitionWithData<TState, TData>
        where TData : INotifyPropertyChanged
        where TState : struct
    {
        /// <summary>
        /// Gets or sets the enum state definition
        /// </summary>
        public IEnumStateDefinition<TState> EnumState
        {
            get => State as IEnumStateDefinition<TState>;
            set => State = value;
        }
    }
}