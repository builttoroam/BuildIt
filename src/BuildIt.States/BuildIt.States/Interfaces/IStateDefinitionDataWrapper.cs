using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Properties and methods for the entity that manages data associated with a state.
    /// </summary>
    public interface IStateDefinitionDataWrapper
    {
        /// <summary>
        /// Gets the type of data associated with the state (ie that an instance will be created from).
        /// </summary>
        Type StateDataType { get; }

        /// <summary>
        /// Generates an instance of the state data entity.
        /// </summary>
        /// <returns>The data entity.</returns>
        INotifyPropertyChanged Generate();

        /// <summary>
        /// Invokes the initialize method.
        /// </summary>
        /// <param name="dataEntity">The data entity to invoke the method on.</param>
        /// <param name="cancelToken">Cancellation token allowing change to be cancelled.</param>
        /// <returns>Task to await.</returns>
        Task InvokeInitialise(INotifyPropertyChanged dataEntity, CancellationToken cancelToken);

        /// <summary>
        /// Invoked the AboutToChangeFrom method.
        /// </summary>
        /// <param name="dataEntity">The data entity to invoke the method on.</param>
        /// <param name="cancel">Whether the change should be cancelled.</param>
        /// <returns>Task to await.</returns>
        Task InvokeAboutToChangeFrom(INotifyPropertyChanged dataEntity, StateCancelEventArgs cancel);

        /// <summary>
        /// Invokes the ChangingFrom method.
        /// </summary>
        /// <param name="dataEntity">The data entity to invoke the method on.</param>
        /// <param name="cancelToken">Cancellation token allowing change to be cancelled.</param>
        /// <returns>Task to await.</returns>
        Task InvokeChangingFrom(INotifyPropertyChanged dataEntity, CancellationToken cancelToken);

        /// <summary>
        /// Invokes the ChangedFrom method.
        /// </summary>
        /// <param name="dataEntity">The data entity to invoke the method on.</param>
        /// <param name="cancelToken">Cancellation token allowing change to be cancelled.</param>
        /// <returns>Task to await.</returns>
        Task InvokeChangedFrom(INotifyPropertyChanged dataEntity, CancellationToken cancelToken);

        /// <summary>
        /// Invokes the ChangedTo method.
        /// </summary>
        /// <param name="dataEntity">The data entity to invoke the method on.</param>
        /// <param name="cancelToken">Cancellation token allowing change to be cancelled.</param>
        /// <returns>Task to await.</returns>
        Task InvokeChangedTo(INotifyPropertyChanged dataEntity, CancellationToken cancelToken);

        /// <summary>
        /// Invokes the ChangedToWithData method.
        /// </summary>
        /// <param name="dataEntity">The data entity to invoke the method on.</param>
        /// <param name="data">The json data to pass into the ChangedToWithData method.</param>
        /// <param name="cancelToken">Cancellation token allowing change to be cancelled.</param>
        /// <returns>Task to await.</returns>
        Task InvokeChangedToWithData(INotifyPropertyChanged dataEntity, string data, CancellationToken cancelToken);
    }
}