using System;
using BuildIt.States.Completion;

namespace BuildIt.Lifecycle.States.ViewModel
{
    /// <summary>
    /// Base view model that has support for both state manager and completion events
    /// </summary>
    /// <typeparam name="TCompletion">The completion type (enum)</typeparam>
    public class BaseStateManagerViewModelWithCompletion<TCompletion> : BaseStateManagerViewModel, 
        ICompletion<TCompletion>
        where TCompletion : struct
    {
        /// <summary>
        /// The Complete event
        /// </summary>
        public event EventHandler<CompletionEventArgs<TCompletion>> Complete;

        /// <summary>
        /// Raises the Complete event
        /// </summary>
        /// <param name="completion">The completion status</param>
        protected virtual void OnComplete(TCompletion completion)
        {
            if (default(TCompletion).Equals(completion))
            {
                throw new ArgumentException("Can't complete using the default enum value", nameof(completion));
            }

            OnComplete(new CompletionEventArgs<TCompletion> { Completion = completion });
        }

        /// <summary>
        /// Raises the Complete event
        /// </summary>
        /// <param name="completionArgs">The completion args</param>
        protected virtual void OnComplete(CompletionEventArgs<TCompletion> completionArgs)
        {
            Complete?.Invoke(this, completionArgs);
        }
    }
}