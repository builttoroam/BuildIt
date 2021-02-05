using System;

namespace BuildIt.States.Completion
{
    /// <summary>
    /// Exposes an event to indicate that a state has been completed.
    /// </summary>
    /// <typeparam name="TCompletion">The type (enum) of the completion.</typeparam>
    public interface ICompletion<TCompletion>
        where TCompletion : struct
    {
        /// <summary>
        /// The complete event, indicating the completion value
        /// </summary>
        event EventHandler<CompletionEventArgs<TCompletion>> Complete;
    }
}