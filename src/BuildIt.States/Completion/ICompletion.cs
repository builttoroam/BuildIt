using System;

namespace BuildIt.States.Completion
{
    public interface ICompletion<TCompletion> where TCompletion : struct
    {
        event EventHandler<CompletionEventArgs<TCompletion>> Complete;
    }
}