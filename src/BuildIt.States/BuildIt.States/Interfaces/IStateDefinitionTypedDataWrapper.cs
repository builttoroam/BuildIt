using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Properties and Methods of the type specific data wrapper
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public interface IStateDefinitionTypedDataWrapper<TData>
        : IStateDefinitionDataWrapper
        where TData : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets method to initialize the data entity
        /// </summary>
        Func<TData, Task> Initialise { get; set; }

        /// <summary>
        /// Gets or sets method to invoke when about to change from the data entity
        /// </summary>
        Func<TData, CancelEventArgs, Task> AboutToChangeFrom { get; set; }

        /// <summary>
        /// Gets or sets method to invoke when changing from the data entity (eg leaving view model)
        /// </summary>
        Func<TData, Task> ChangingFrom { get; set; }

        /// <summary>
        /// Gets or sets method to invoke when change to the data entity (eg arriving at view model)
        /// </summary>
        Func<TData, Task> ChangedTo { get; set; }

        /// <summary>
        /// Gets or sets method to invoke when change to a data entity with data (eg data to be passed into view model)
        /// </summary>
        Func<TData, string, Task> ChangedToWithData { get; set; }
    }
}