using System;

namespace BuildIt.States.Completion
{
    public interface ICompletionWithData<TCompletion,TData> 
        where TCompletion : struct
    {
        event EventHandler<CompletionWithDataEventArgs<TCompletion,TData>> CompleteWithData;
    }
}