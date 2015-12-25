using System;

namespace BuildIt.States
{
    public class StateEventArgs<TState> : EventArgs
    {
        public TState State { get; set; }
        public bool UseTransitions { get; set; }

        public StateEventArgs(TState state, bool useTransitions = false)
        {
            State = state;
            UseTransitions = useTransitions;
        }
    }
}