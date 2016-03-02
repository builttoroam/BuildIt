using System;

namespace BuildIt.States.Completion
{
    public class CompletionEventArgs<TCompletion>:EventArgs where TCompletion : struct
    {
        public TCompletion Completion { get; set; }
    }
}