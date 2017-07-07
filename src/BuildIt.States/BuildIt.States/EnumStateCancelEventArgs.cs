namespace BuildIt.States
{
    public class EnumStateCancelEventArgs<TState> : StateCancelEventArgs
        where TState : struct
    {
        public override string StateName => EnumState + "";

        public TState EnumState { get; }

        public EnumStateCancelEventArgs(TState state, bool useTransitions, bool isNewState)
        {
            EnumState = state;
            UseTransitions = useTransitions;
            IsNewState = isNewState;
        }
    }
}