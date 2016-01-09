using System;

namespace BuildIt.Lifecycle.States.ViewModel
{
    public class CompletionEventArgs<TCompletion>:EventArgs where TCompletion : struct
    {
        public TCompletion Completion { get; set; }
    }

    public class CompletionWithDataEventArgs<TCompletion, TData> : CompletionEventArgs<TCompletion>
        where TCompletion : struct
    {
        public TData Data { get; set; }
    }
}