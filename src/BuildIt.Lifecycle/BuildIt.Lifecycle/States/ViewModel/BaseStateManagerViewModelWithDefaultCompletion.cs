using BuildIt.States;
using BuildIt.States.Completion;

namespace BuildIt.Lifecycle.States.ViewModel
{
    public class BaseStateManagerViewModelWithDefaultCompletion : BaseStateManagerViewModelWithCompletion<DefaultCompletion>
    {
        protected void OnComplete()
        {
            OnComplete(DefaultCompletion.Complete);
        }
        protected void OnCompleteWithData<TData>(TData data)
        {
            OnCompleteWithData(DefaultCompletion.Complete, data);
        }
    }
}