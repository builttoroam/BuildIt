using System;
using System.ComponentModel;
using System.Threading.Tasks;
using BuildIt.ServiceLocation;
using BuildIt.States.Interfaces;

namespace BuildIt.States
{
    public class StateDefinitionTypedDataWrapper<TData> : IStateDefinitionTypedDataWrapper<TData>
        where TData : INotifyPropertyChanged
    {
        public Func<TData, Task> Initialise { get; set; }

        public Type StateDataType => typeof(TData);

        public INotifyPropertyChanged Generate()
        {
            $"Creating instance of {typeof(TData).Name}".Log();
            var vm = ServiceLocator.Current.GetInstance<TData>();


            "State Data generated".Log();
            return vm;
        }
        public async Task InvokeInitialise(INotifyPropertyChanged dataEntity)
        {
            if (Initialise == null) return;

            "Invoking Initialise".Log();
            await Initialise((TData)dataEntity);
        }

        public Func<TData, CancelEventArgs, Task> AboutToChangeFrom { get; set; }

        public async Task InvokeAboutToChangeFrom(INotifyPropertyChanged dataEntity, CancelEventArgs cancel)
        {
            if (AboutToChangeFrom == null) return;

            "Invoking AboutToChangeFrom".Log();
            await AboutToChangeFrom((TData)dataEntity, cancel);
        }

        public Func<TData, Task> ChangingFrom { get; set; }

        public async Task InvokeChangingFrom(INotifyPropertyChanged dataEntity)
        {
            if (ChangingFrom == null) return;

            "Invoking ChangingFrom".Log();
            await ChangingFrom((TData)dataEntity);
        }

        public Func<TData, Task> ChangedFrom { get; set; }

        public async Task InvokeChangedFrom(INotifyPropertyChanged dataEntity)
        {
            if (ChangedFrom == null) return;

            "Invoking ChangedFrom".Log();
            await ChangedFrom((TData)dataEntity);
        }


        public Func<TData, Task> ChangedTo { get; set; }

        public async Task InvokeChangedTo(INotifyPropertyChanged dataEntity)
        {
            if (ChangedTo == null) return;

            "Invoking ChangedTo".Log();
            await ChangedTo((TData)dataEntity);
        }

        public Func<TData, string, Task> ChangedToWithData { get; set; }

        public async Task InvokeChangedToWithData(INotifyPropertyChanged dataEntity, string data)
        {
            if (ChangedToWithData == null) return;

            "Invoking ChangedToWithData".Log();
            await ChangedToWithData((TData)dataEntity, data);
        }
    }
}