using System;

namespace BuildIt.States.Completion
{
    /// <summary>
    /// Entity that raises an event when it is complete, with data
    /// </summary>
    /// <typeparam name="TCompletion">The type(enum) of the completion</typeparam>
    /// <typeparam name="TData">The type of the data</typeparam>
    public interface ICompletionWithData<TCompletion, TData>
        where TCompletion : struct
    {
        /// <summary>
        /// The completion event
        /// </summary>
        event EventHandler<CompletionWithDataEventArgs<TCompletion, TData>> CompleteWithData;
    }
}