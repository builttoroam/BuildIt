namespace BuildIt.States
{
    public class EnumStateEventArgs<TState> : StateEventArgs
        where TState:struct
    {
        public override string StateName => EnumState + "";

        public TState EnumState { get;  }

        public EnumStateEventArgs(TState state, bool useTransitions, bool isNewState)
        {
            EnumState = state;
            UseTransitions = useTransitions;
            IsNewState = isNewState;
        }
    }
}