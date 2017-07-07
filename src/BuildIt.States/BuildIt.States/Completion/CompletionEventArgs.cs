using System;

namespace BuildIt.States.Completion
{
    /// <summary>
    /// The output value for when a state is completed
    /// </summary>
    /// <typeparam name="TCompletion">The type (enum) of completion</typeparam>
    public class CompletionEventArgs<TCompletion>:EventArgs where TCompletion : struct
    {
        /// <summary>
        /// The completion state
        /// </summary>
        public TCompletion Completion { get; set; }
    }

}