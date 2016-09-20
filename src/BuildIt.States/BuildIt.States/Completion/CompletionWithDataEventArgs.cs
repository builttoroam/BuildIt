namespace BuildIt.States.Completion
{
    public class CompletionWithDataEventArgs<TCompletion, TData> : CompletionEventArgs<TCompletion>
        where TCompletion : struct
    {
        public TData Data { get; set; }
    }
}