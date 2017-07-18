using System.ComponentModel;
using BuildIt.States.Interfaces;

namespace BuildIt.States
{
    /// <summary>
    /// Defines properties and methods for a state definition based on an enum
    /// </summary>
    /// <typeparam name="TState">The state type</typeparam>
    /// <typeparam name="TData">The type of data to be associated with the state</typeparam>
    public class TypedStateDefinitionWithDataWrapper<TState, TData> :
        StateDefinitionWithDataWrapper<TData>, ITypedStateDefinitionWithData<TState, TData>
        where TData : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the enum state definition
        /// </summary>
        public ITypedStateDefinition<TState> TypedState
        {
            get => State as ITypedStateDefinition<TState>;
            set => State = value;
        }
    }
}