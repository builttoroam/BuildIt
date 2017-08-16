using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Properties and Methods of the type specific data wrapper
    /// </summary>
    /// <typeparam name="TData">The type (enum) of the state being defined</typeparam>
    public interface IStateDefinitionTypedDataWrapper<TData>
        : IStateDefinitionDataWrapper
        where TData : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets method to initialize the data entity
        /// </summary>
        Func<TData, CancellationToken, Task> Initialise { get; set; }

        /// <summary>
        /// Gets or sets method to invoke when about to change from the data entity
        /// </summary>
        Func<TData, StateCancelEventArgs, Task> AboutToChangeFrom { get; set; }

        /// <summary>
        /// Gets or sets method to invoke when changing from the data entity (eg leaving view model)
        /// </summary>
        Func<TData, CancellationToken, Task> ChangingFrom { get; set; }

        /// <summary>
        /// Gets or sets method to invoke when change to the data entity (eg arriving at view model)
        /// </summary>
        Func<TData, CancellationToken, Task> ChangedTo { get; set; }

        /// <summary>
        /// Gets or sets method to invoke when change to a data entity with data (eg data to be passed into view model)
        /// </summary>
        Func<TData, string, CancellationToken, Task> ChangedToWithData { get; set; }
    }
}