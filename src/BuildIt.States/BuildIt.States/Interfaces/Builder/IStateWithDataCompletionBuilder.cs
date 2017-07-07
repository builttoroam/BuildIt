using System.ComponentModel;
using BuildIt.States.Completion;

namespace BuildIt.States.Interfaces.Builder
{
    public interface IStateWithDataCompletionBuilder<TState, TStateData, TCompletion> :
        IStateCompletionBuilder<TState, TCompletion>,
        IStateWithDataAction<TState,TStateData> 
        where TState : struct
        where TCompletion : struct
        where TStateData : INotifyPropertyChanged, ICompletion<TCompletion>
    {
    }
}