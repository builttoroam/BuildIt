using System;
using BuildIt.States.Completion;

namespace BuildIt.Lifecycle.States.ViewModel
{
    /// <summary>
    /// Base view model that implements ICompletion
    /// </summary>
    /// <typeparam name="TCompletion">The type (enum) of completion event</typeparam>
    /// <typeparam name="TData">The type of the completion data</typeparam>
    public class BaseViewModelWithCompletionData<TCompletion, TData>
        : BaseViewModelWithCompletion<TCompletion>,
            ICompletionWithData<TCompletion, TData>
        where TCompletion : struct
    {
        /// <summary>
        /// The complete with data event
        /// </summary>
        public event EventHandler<CompletionWithDataEventArgs<TCompletion, TData>> CompleteWithData;

        /// <summary>
        /// Raises the complete with data event
        /// </summary>
        /// <param name="completion">The completion status</param>
        /// <param name="data">The data to complete with</param>
        protected virtual void OnCompleteWithData(TCompletion completion, TData data)
        {
            var completionArgs =
                new CompletionWithDataEventArgs<TCompletion, TData> { Completion = completion, Data = data };
            OnComplete(completionArgs);

            CompleteWithData?.Invoke(this, completionArgs);
        }
    }
}