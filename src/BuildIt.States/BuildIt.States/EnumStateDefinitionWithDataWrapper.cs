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
        TypedStateDefinitionWithDataWrapper<TState, TData>
        where TData : INotifyPropertyChanged
        where TState : struct
    {
    }

    public class TypedStateDefinitionWithDataWrapper<TState, TData> :
        StateDefinitionWithDataWrapper<TData>, IStateDefinitionWithData<TState, TData>
        where TData : INotifyPropertyChanged
        where TState : struct
    {
        /// <summary>
        /// Gets or sets the enum state definition
        /// </summary>
        public IStateDefinition<TState> TypedState
        {
            get => State as IStateDefinition<TState>;
            set => State = value;
        }
    }
}