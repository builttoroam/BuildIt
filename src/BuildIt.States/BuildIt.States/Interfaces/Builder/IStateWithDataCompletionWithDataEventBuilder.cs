using System.ComponentModel;
using BuildIt.States.Completion;

namespace BuildIt.States.Interfaces.Builder
{
    /// <summary>
    /// Defines a state that has state data that exposes a
    /// completion event which returns data
    /// </summary>
    /// <typeparam name="TState">The type (enum) of the state</typeparam>
    /// <typeparam name="TStateData">The type of the state data</typeparam>
    /// <typeparam name="TCompletion">The type (enum) of the completion</typeparam>
    /// <typeparam name="TData">The type of the data to be returned on completion</typeparam>
    public interface IStateWithDataCompletionWithDataEventBuilder
        <TState, TStateData, TCompletion, TData> :
            IStateCompletionBuilder<TState, TCompletion>,
            IStateWithDataActionDataBuilder<TState, TStateData, TData>
        where TState : struct
        where TCompletion : struct
        where TStateData : INotifyPropertyChanged, ICompletionWithData<TCompletion, TData>
    {
    }
}