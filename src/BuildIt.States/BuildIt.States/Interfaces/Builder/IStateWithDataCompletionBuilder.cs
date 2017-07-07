using System.ComponentModel;
using BuildIt.States.Completion;

namespace BuildIt.States.Interfaces.Builder
{
    /// <summary>
    /// Builder for state that accepts data and completes
    /// </summary>
    /// <typeparam name="TState">The type (enum) of the state</typeparam>
    /// <typeparam name="TStateData">The type of the state data</typeparam>
    /// <typeparam name="TCompletion">The type (enum) of completion</typeparam>
    public interface IStateWithDataCompletionBuilder<TState, TStateData, TCompletion> :
        IStateCompletionBuilder<TState, TCompletion>,
        IStateWithDataActionBuilder<TState,TStateData> 
        where TState : struct
        where TCompletion : struct
        where TStateData : INotifyPropertyChanged, ICompletion<TCompletion>
    {
    }
}