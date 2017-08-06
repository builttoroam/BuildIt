using BuildIt.States;
using BuildIt.States.Completion;

namespace BuildIt.Lifecycle.States.ViewModel
{
    /// <summary>
    /// Base view model that implements ICompletion using the DefaultCompletion enum
    /// </summary>
    public class BaseViewModelWithDefaultCompletion: BaseViewModelWithCompletion<DefaultCompletion>
    {
        /// <summary>
        /// Completes the state
        /// </summary>
        protected virtual void OnComplete()
        {
            OnComplete(DefaultCompletion.Complete);
        }
    }
}