using System;

namespace BuildIt.Lifecycle.States.ViewModel
{
    public class BaseViewModelWithCompletion<TCompletion> : BaseViewModel, IViewModelCompletion<TCompletion>
        where TCompletion : struct
    {
        public event EventHandler<CompletionEventArgs<TCompletion>> Complete;

        protected void OnComplete(TCompletion completion)
        {
            Complete?.Invoke(this, new CompletionEventArgs<TCompletion> {Completion = completion});
        }
    }
}