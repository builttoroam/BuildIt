using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;

namespace BuildIt.States
{
    public class StateDefinitionWithDataWrapper<TState, TData> : IStateDefinitionWithData<TState, TData>
          where TData : INotifyPropertyChanged
        where TState : struct
    {
        public IStateDefinition<TState> State { get; set; }

        public IStateDefinitionTypedDataWrapper<TData> StateDataWrapper
            => State.UntypedStateDataWrapper as IStateDefinitionTypedDataWrapper<TData>;
    }

    public interface IStateDefinitionWithData<TState, TData>
        where TData: INotifyPropertyChanged
        where TState : struct
    {
        IStateDefinition<TState> State { get; }
        IStateDefinitionTypedDataWrapper<TData> StateDataWrapper { get; } 
    }

    public interface IStateDefinition<TState> where TState : struct
    {
        TState State { get; }

        IList<IStateTrigger> Triggers { get; }

        Func<CancelEventArgs, Task> AboutToChangeFrom { get; set; }
        Func<Task> ChangingFrom { get; set; }
        Func<Task> ChangedTo { get; set; }
        Func<string,Task> ChangedToWithJsonData { get; set; }

        IList<IStateValue> Values { get; }

        void TransitionTo(IDictionary<Tuple<object, string>, IDefaultValue> defaultValues);

        IStateDefinitionDataWrapper UntypedStateDataWrapper { get; }
    }

    public interface IStateDefinitionTypedDataWrapper<TData>
        :IStateDefinitionDataWrapper
        where TData : INotifyPropertyChanged
    {
        Func<TData, Task> Initialise { get; set; }

        Func<TData, CancelEventArgs, Task> AboutToChangeFrom { get; set; }


        Func<TData, Task> ChangingFrom { get; set; }

        Func<TData, Task> ChangedTo { get; set; }

        Func<TData, string, Task> ChangedToWithData { get; set; }
    }

    public interface IStateDefinitionDataWrapper
    {
        Type StateDataType { get; }
        INotifyPropertyChanged Generate();
        Task InvokeInitialise(INotifyPropertyChanged dataEntity);

        Task InvokeAboutToChangeFrom(INotifyPropertyChanged dataEntity, CancelEventArgs cancel);

        Task InvokeChangingFrom(INotifyPropertyChanged dataEntity);

        Task InvokeChangedTo(INotifyPropertyChanged dataEntity);

        Task InvokeChangedToWithData(INotifyPropertyChanged dataEntity, string data);
    }

    public interface IStateDefinitionWithData<TData> where TData : INotifyPropertyChanged
    {
        IStateDefinitionTypedDataWrapper<TData> StateData { get; set; } 
    }

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