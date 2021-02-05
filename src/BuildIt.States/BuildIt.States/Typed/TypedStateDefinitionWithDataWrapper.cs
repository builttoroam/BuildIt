using BuildIt.States.Interfaces;
using System.ComponentModel;

namespace BuildIt.States.Typed
{
    /// <summary>
    /// Defines properties and methods for a state definition based on an enum.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TTypedStateDefinition">The type of the state definition.</typeparam>
    /// <typeparam name="TData">The type of data to be associated with the state.</typeparam>
    public class TypedStateDefinitionWithDataWrapper<TState, TTypedStateDefinition, TData> :
        StateDefinitionWithDataWrapper<TData>, ITypedStateDefinitionWithData<TState, TTypedStateDefinition, TData>
        where TData : INotifyPropertyChanged
        where TTypedStateDefinition : class, ITypedStateDefinition<TState>, new()
    {
        /// <summary>
        /// Gets or sets the enum state definition.
        /// </summary>
        public TTypedStateDefinition TypedState
        {
            get => State as TTypedStateDefinition;
            set => State = value;
        }
    }
}