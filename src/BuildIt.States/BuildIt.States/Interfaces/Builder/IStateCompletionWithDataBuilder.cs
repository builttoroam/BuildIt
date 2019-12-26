using System;

namespace BuildIt.States.Interfaces.Builder
{
    /// <summary>
    /// Builder for state that completes with data.
    /// </summary>
    /// <typeparam name="TState">The type (enum) of the state.</typeparam>
    /// <typeparam name="TCompletion">The type (enum) of completion.</typeparam>
    /// <typeparam name="TData">The type of data.</typeparam>
    public interface IStateCompletionWithDataBuilder<TState, TCompletion, TData>
        : IStateCompletionBuilder<TState, TCompletion>
        where TState : struct
        where TCompletion : struct
    {
        /// <summary>
        /// Gets the data to complete with.
        /// </summary>
        Func<TData> Data { get; }
    }
}