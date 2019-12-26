using BuildIt.States.Interfaces;
using System.ComponentModel;

namespace BuildIt.States
{
    /// <summary>
    /// Wrapper for state definition coupled with data entity.
    /// </summary>
    /// <typeparam name="TData">The type of data entity.</typeparam>
    public class StateDefinitionWithDataWrapper<TData> : IStateDefinitionWithData<TData>
        where TData : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the state definition.
        /// </summary>
        public IStateDefinition State { get; set; }

        /// <summary>
        /// Gets the associated state data entity.
        /// </summary>
        public IStateDefinitionTypedDataWrapper<TData> StateData
            => State.UntypedStateDataWrapper as IStateDefinitionTypedDataWrapper<TData>;
    }
}