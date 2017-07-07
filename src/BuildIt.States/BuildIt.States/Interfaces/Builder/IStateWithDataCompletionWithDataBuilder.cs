using System;
using System.ComponentModel;
using BuildIt.States.Completion;

namespace BuildIt.States.Interfaces.Builder
{
    /// <summary>
    /// Builder for state the completes with data
    /// </summary>
    /// <typeparam name="TState">The type (enum) of the state</typeparam>
    /// <typeparam name="TStateData">The type of the state data</typeparam>
    /// <typeparam name="TCompletion">The type (enum) of completion</typeparam>
    /// <typeparam name="TData">The type of data</typeparam>
    public interface IStateWithDataCompletionWithDataBuilder<TState, TStateData, TCompletion, TData> :
        IStateWithDataCompletionBuilder<TState, TStateData, TCompletion>,
        IStateWithDataActionDataBuilder<TState,TStateData,TData>
        where TState : struct
        where TCompletion : struct
        where TStateData : INotifyPropertyChanged,ICompletion<TCompletion>
    {
        /// <summary>
        /// The function to retrieve data
        /// </summary>
        Func<TStateData, TData> Data { get; }
    }
}