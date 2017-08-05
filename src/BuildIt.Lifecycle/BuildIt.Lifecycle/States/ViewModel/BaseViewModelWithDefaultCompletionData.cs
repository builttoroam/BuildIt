using BuildIt.States.Completion;

namespace BuildIt.Lifecycle.States.ViewModel
{
    /// <summary>
    /// Base view model that implements ICompletion using the DefaultCompletion enum, with data
    /// </summary>
    /// <typeparam name="TData">The type of data to complete with</typeparam>
    public class BaseViewModelWithDefaultCompletionData<TData> : BaseViewModelWithCompletionData<DefaultCompletion, TData>
    {
        /// <summary>
        /// Completes the state with data
        /// </summary>
        /// <param name="data">The data to complete with</param>
        protected virtual void OnCompleteWithData(TData data)
        {
            OnCompleteWithData(DefaultCompletion.Complete, data);
        }
    }
}