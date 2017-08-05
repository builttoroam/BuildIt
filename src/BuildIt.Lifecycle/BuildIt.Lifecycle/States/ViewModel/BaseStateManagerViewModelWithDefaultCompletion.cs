using BuildIt.States;
using BuildIt.States.Completion;

namespace BuildIt.Lifecycle.States.ViewModel
{
    /// <summary>
    /// Base view model that has state manager and implements ICompletion using the DefaultCompletion enum
    /// </summary>
    public class BaseStateManagerViewModelWithDefaultCompletion : BaseStateManagerViewModelWithCompletion<DefaultCompletion>
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