using System.ComponentModel;
using BuildIt.States.Completion;

namespace BuildIt.States.Interfaces.Builder
{
    public interface IStateWithDataCompletionWithDataEventBuilder
        <TState, TStateData, TCompletion, TData> :
            IStateCompletionBuilder<TState, TCompletion>,
            IStateWithDataActionData<TState, TStateData,TData> 
        where TState : struct
        where TCompletion : struct
        where TStateData : INotifyPropertyChanged, ICompletionWithData<TCompletion,TData>
    {
    }
}