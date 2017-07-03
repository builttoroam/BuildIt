using System.ComponentModel;
using BuildIt.States.Interfaces;

namespace BuildIt.States
{
    /// <summary>
    /// Wrapper for state definition coupled with data entity
    /// </summary>
    /// <typeparam name="TData">The type of data entity</typeparam>
    public class StateDefinitionWithDataWrapper<TData> : IStateDefinitionWithData<TData>
        where TData : INotifyPropertyChanged
    {
        /// <summary>
        /// The state definition
        /// </summary>
        public IStateDefinition State { get; set; }

        /// <summary>
        /// The associated state data entity
        /// </summary>
        public IStateDefinitionTypedDataWrapper<TData> StateData
            => State.UntypedStateDataWrapper as IStateDefinitionTypedDataWrapper<TData>;
    }
}