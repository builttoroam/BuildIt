using System;
using BuildIt.States.Completion;

namespace BuildIt.Lifecycle.States.ViewModel
{
    public class BaseViewModelWithCompletion<TCompletion> 
        : BaseViewModel, 
        ICompletion<TCompletion>
        where TCompletion : struct
    {
        public event EventHandler<CompletionEventArgs<TCompletion>> Complete;

        protected void OnComplete(TCompletion completion)
        {
            Complete?.Invoke(this, new CompletionEventArgs<TCompletion> {Completion = completion});
        }

        protected void OnCompleteWithData<TData>(TCompletion completion, TData data)
        {
            Complete?.Invoke(this, new CompletionWithDataEventArgs<TCompletion, TData> { Completion = completion, Data = data });
        }

    }
}