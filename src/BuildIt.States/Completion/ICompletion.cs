using System;

namespace BuildIt.States.Completion
{
    public interface ICompletion<TCompletion> where TCompletion : struct
    {
        event EventHandler<CompletionEventArgs<TCompletion>> Complete;
    }

    public interface ICompletionWithData<TCompletion,TData> 
        where TCompletion : struct
    {
        event EventHandler<CompletionWithDataEventArgs<TCompletion,TData>> CompleteWithData;
    }
}