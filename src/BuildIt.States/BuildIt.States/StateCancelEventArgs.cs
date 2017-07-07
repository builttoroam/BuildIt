namespace BuildIt.States
{
    public class StateCancelEventArgs : CancelEventArgs, IStateEventArgs
    {
        public virtual string StateName { get; }
        public bool UseTransitions { get; protected set; }
        public bool IsNewState { get; protected set; }

        protected StateCancelEventArgs()
        {

        }

        public StateCancelEventArgs(string state, bool useTransitions, bool isNewState)
        {
            StateName=state;
            UseTransitions = useTransitions;
            IsNewState = isNewState;
        }
    }
}