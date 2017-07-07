namespace BuildIt.States.Completion
{
    /// <summary>
    /// Completion information for a state which includes completion enum and out data
    /// </summary>
    /// <typeparam name="TCompletion">The completion state type</typeparam>
    /// <typeparam name="TData">The type of data to return</typeparam>
    public class CompletionWithDataEventArgs<TCompletion, TData> : CompletionEventArgs<TCompletion>
        where TCompletion : struct
    {
        /// <summary>
        /// The data to pass out of the state
        /// </summary>
        public TData Data { get; set; }
    }
}