using System;

namespace BuildIt.States
{
    public class StateEventArgs<TState> : EventArgs
    {
        public TState State { get; set; }
        public bool UseTransitions { get; set; }

        public bool IsNewState { get; set; }
        public StateEventArgs(TState state, bool useTransitions, bool isNewState)
        {
            State = state;
            UseTransitions = useTransitions;
            IsNewState = isNewState;
        }
    }
}