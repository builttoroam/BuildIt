using System;
using System.ComponentModel;
using System.Threading.Tasks;
using BuildIt.ServiceLocation;
using BuildIt.States.Interfaces;

namespace BuildIt.States
{
    /// <summary>
    /// Wrapper class for defining a state, and associated data
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class StateDefinitionTypedDataWrapper<TData> : IStateDefinitionTypedDataWrapper<TData>
        where TData : INotifyPropertyChanged
    {
        /// <summary>
        /// The method to call when initializing the state
        /// </summary>
        public Func<TData, Task> Initialise { get; set; }

        /// <summary>
        /// The method to call when about to change from a state
        /// </summary>
        public Func<TData, CancelEventArgs, Task> AboutToChangeFrom { get; set; }

        /// <summary>
        /// The method to call when changing from a state
        /// </summary>
        public Func<TData, Task> ChangingFrom { get; set; }

        /// <summary>
        /// The method to call when changed from a state
        /// </summary>
        public Func<TData, Task> ChangedFrom { get; set; }

        /// <summary>
        /// The method to call when changed to a state
        /// </summary>
        public Func<TData, Task> ChangedTo { get; set; }

        /// <summary>
        /// The method to call when change to a state, passing in data
        /// </summary>
        public Func<TData, string, Task> ChangedToWithData { get; set; }

        /// <summary>
        /// The type of the state data
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
        /// <returns>Task to await</returns>
        public async Task InvokeInitialise(INotifyPropertyChanged dataEntity)
        {
            if (Initialise == null) return;

            if (!(dataEntity is TData entityData)) return;

            "Invoking Initialise".Log();
            await Initialise(entityData);
        }


        /// <summary>
        /// Invokes the AboutToChange method, passing in data and cancel args
        /// </summary>
        /// <param name="dataEntity">The state data to pass into the method</param>
        /// <param name="cancel">Cancels the state transition</param>
        /// <returns>Task to await</returns>
        public async Task InvokeAboutToChangeFrom(INotifyPropertyChanged dataEntity, CancelEventArgs cancel)
        {
            if (AboutToChangeFrom == null) return;

            if (!(dataEntity is TData entityData)) return;

            "Invoking AboutToChangeFrom".Log();
            await AboutToChangeFrom(entityData, cancel);
        }


        /// <summary>
        /// Invokes the changingfrom method
        /// </summary>
        /// <param name="dataEntity">The state data to pass into the method</param>
        /// <returns>Task to await</returns>
        public async Task InvokeChangingFrom(INotifyPropertyChanged dataEntity)
        {
            if (ChangingFrom == null) return;

            if (!(dataEntity is TData entityData)) return;

            "Invoking ChangingFrom".Log();
            await ChangingFrom(entityData);
        }

        /// <summary>
        /// Invokes the changed from method
        /// </summary>
        /// <param name="dataEntity">The state data to pass into the method</param>
        /// <returns>Task to await</returns>
        public async Task InvokeChangedFrom(INotifyPropertyChanged dataEntity)
        {
            if (ChangedFrom == null) return;

            if (!(dataEntity is TData entityData)) return;

            "Invoking ChangedFrom".Log();
            await ChangedFrom(entityData);
        }


        /// <summary>
        /// Invokes the changed to method
        /// </summary>
        /// <param name="dataEntity">The state data to pass into the method</param>
        /// <returns>Task to await</returns>
        public async Task InvokeChangedTo(INotifyPropertyChanged dataEntity)
        {
            if (ChangedTo == null) return;

            if (!(dataEntity is TData entityData)) return;

            "Invoking ChangedTo".Log();
            await ChangedTo(entityData);
        }

        /// <summary>
        /// Invokes the changed to with data method
        /// </summary>
        /// <param name="dataEntity">The state data to pass into the method</param>
        /// <param name="data">The json data to pass into the method</param>
        /// <returns>Task to await</returns>
        public async Task InvokeChangedToWithData(INotifyPropertyChanged dataEntity, string data)
        {
            if (ChangedToWithData == null) return;

            if (!(dataEntity is TData entityData)) return;

            "Invoking ChangedToWithData".Log();
            await ChangedToWithData(entityData, data);
        }
    }
}