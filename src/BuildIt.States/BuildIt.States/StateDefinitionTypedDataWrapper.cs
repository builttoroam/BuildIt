using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using BuildIt.ServiceLocation;
using BuildIt.States.Interfaces;

namespace BuildIt.States
{
    /// <summary>
    /// Wrapper class for defining a state, and associated data
    /// </summary>
    /// <typeparam name="TData">The type (enum) of the state being defined</typeparam>
    public class StateDefinitionTypedDataWrapper<TData> : IStateDefinitionTypedDataWrapper<TData>
        where TData : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the method to call when initializing the state
        /// </summary>
        public Func<TData, CancellationToken, Task> Initialise { get; set; }

        /// <summary>
        /// Gets or sets the method to call when about to change from a state
        /// </summary>
        public Func<TData, StateCancelEventArgs, Task> AboutToChangeFrom { get; set; }

        /// <summary>
        /// Gets or sets the method to call when changing from a state
        /// </summary>
        public Func<TData, CancellationToken, Task> ChangingFrom { get; set; }

        /// <summary>
        /// Gets or sets the method to call when changed from a state
        /// </summary>
        public Func<TData, CancellationToken, Task> ChangedFrom { get; set; }

        /// <summary>
        /// Gets or sets the method to call when changed to a state
        /// </summary>
        public Func<TData, CancellationToken, Task> ChangedTo { get; set; }

        /// <summary>
        /// Gets or sets the method to call when change to a state, passing in data
        /// </summary>
        public Func<TData, string, CancellationToken, Task> ChangedToWithData { get; set; }

        /// <summary>
        /// Gets the type of the state data
        /// </summary>
        public Type StateDataType => typeof(TData);

        /// <summary>
        /// Generates a new instance of the state data
        /// </summary>
        /// <returns>The generated instance of state data</returns>
        public INotifyPropertyChanged Generate()
        {
            $"Creating instance of {typeof(TData).Name}".Log();
            var vm = ServiceLocator.Current.GetInstance<TData>();

            "State Data generated".Log();
            return vm;
        }

        /// <summary>
        /// Invokes the Initialize method
        /// </summary>
        /// <param name="dataEntity">The state data to pass into the method</param>
        /// <param name="cancelToken">Cancels the state transition</param>
        /// <returns>Task to await</returns>
        public async Task InvokeInitialise(INotifyPropertyChanged dataEntity, CancellationToken cancelToken)
        {
            if (Initialise == null)
            {
                return;
            }

            if (dataEntity is TData entityData)
            {
                "Invoking Initialise".Log();
                await Initialise(entityData, cancelToken);
            }
        }

        /// <summary>
        /// Invokes the AboutToChange method, passing in data and cancel args
        /// </summary>
        /// <param name="dataEntity">The state data to pass into the method</param>
        /// <param name="cancel">Cancels the state transition</param>
        /// <returns>Task to await</returns>
        public async Task InvokeAboutToChangeFrom(INotifyPropertyChanged dataEntity, StateCancelEventArgs cancel)
        {
            if (AboutToChangeFrom == null)
            {
                return;
            }

            if (dataEntity is TData entityData)
            {
                "Invoking AboutToChangeFrom".Log();
                await AboutToChangeFrom(entityData, cancel);
            }
        }

        /// <summary>
        /// Invokes the changingfrom method
        /// </summary>
        /// <param name="dataEntity">The state data to pass into the method</param>
        /// <param name="cancelToken">Cancels the state transition</param>
        /// <returns>Task to await</returns>
        public async Task InvokeChangingFrom(INotifyPropertyChanged dataEntity, CancellationToken cancelToken)
        {
            if (ChangingFrom == null)
            {
                return;
            }

            if (dataEntity is TData entityData)
            {
                "Invoking ChangingFrom".Log();
                await ChangingFrom(entityData, cancelToken);
            }
        }

        /// <summary>
        /// Invokes the changed from method
        /// </summary>
        /// <param name="dataEntity">The state data to pass into the method</param>
        /// <param name="cancelToken">Cancels the state transition</param>
        /// <returns>Task to await</returns>
        public async Task InvokeChangedFrom(INotifyPropertyChanged dataEntity, CancellationToken cancelToken)
        {
            if (ChangedFrom == null)
            {
                return;
            }

            if (dataEntity is TData entityData)
            {
                "Invoking ChangedFrom".Log();
                await ChangedFrom(entityData, cancelToken);
            }
        }

        /// <summary>
        /// Invokes the changed to method
        /// </summary>
        /// <param name="dataEntity">The state data to pass into the method</param>
        /// <param name="cancelToken">Cancels the state transition</param>
        /// <returns>Task to await</returns>
        public async Task InvokeChangedTo(INotifyPropertyChanged dataEntity, CancellationToken cancelToken)
        {
            if (ChangedTo == null)
            {
                return;
            }

            if (dataEntity is TData entityData)
            {
                "Invoking ChangedTo".Log();
                await ChangedTo(entityData, cancelToken);
            }
        }

        /// <summary>
        /// Invokes the changed to with data method
        /// </summary>
        /// <param name="dataEntity">The state data to pass into the method</param>
        /// <param name="data">The json data to pass into the method</param>
        /// <param name="cancelToken">Cancels the state transition</param>
        /// <returns>Task to await</returns>
        public async Task InvokeChangedToWithData(INotifyPropertyChanged dataEntity, string data, CancellationToken cancelToken)
        {
            if (ChangedToWithData == null)
            {
                return;
            }

            if (dataEntity is TData entityData)
            {
                "Invoking ChangedToWithData".Log();
                await ChangedToWithData(entityData, data, cancelToken);
            }
        }
    }
}