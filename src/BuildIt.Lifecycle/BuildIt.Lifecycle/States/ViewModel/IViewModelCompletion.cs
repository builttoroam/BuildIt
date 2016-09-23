using System;
using BuildIt.States.Completion;

namespace BuildIt.Lifecycle.States.ViewModel
{
    public interface IViewModelCompletion<TCompletion> where TCompletion:struct
    {
        event EventHandler<CompletionEventArgs<TCompletion>> Complete;
    }
}