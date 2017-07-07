using System;
using System.ComponentModel;
using BuildIt.States.Completion;

namespace BuildIt.States.Interfaces.Builder
{
    public interface IStateWithDataCompletionWithDataBuilder<TState, TStateData, TCompletion, TData> :
        IStateWithDataCompletionBuilder<TState, TStateData, TCompletion>,
        IStateWithDataActionData<TState,TStateData,TData>
        where TState : struct
        where TCompletion : struct
        where TStateData : INotifyPropertyChanged,ICompletion<TCompletion>
    {
        Func<TStateData, TData> Data { get; }
    }
}