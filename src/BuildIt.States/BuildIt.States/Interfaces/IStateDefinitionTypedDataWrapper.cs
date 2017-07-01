using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BuildIt.States.Interfaces
{
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
}