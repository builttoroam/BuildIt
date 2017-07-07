using System;

namespace BuildIt.States.Interfaces.Builder
{
    public interface IStateCompletionWithDataBuilder<TState, TCompletion, TData> 
        : IStateCompletionBuilder<TState, TCompletion>
        where TState : struct
        where TCompletion : struct
    {
        Func<TData> Data { get; }
    }
}