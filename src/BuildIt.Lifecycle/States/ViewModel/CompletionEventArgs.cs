using System;

namespace BuildIt.Lifecycle.States.ViewModel
{
    public class CompletionEventArgs<TCompletion>:EventArgs where TCompletion : struct
    {
        public TCompletion Completion { get; set; }
    }
}