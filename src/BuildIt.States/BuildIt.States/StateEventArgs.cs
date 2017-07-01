using System;

namespace BuildIt.States
{
    public interface IStateEventArgs
    {
        string StateName { get;  }
        bool UseTransitions { get;  }
        bool IsNewState { get;  }
    }



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

    public class StateEventArgs : EventArgs, IStateEventArgs
    {
        public virtual string StateName { get; }
        public bool UseTransitions { get; protected set; }
        public bool IsNewState { get; protected set; }

        protected StateEventArgs()
        {
            
        }

        public StateEventArgs(string state, bool useTransitions, bool isNewState)
        {
            StateName = state;
            UseTransitions = useTransitions;
            IsNewState = isNewState;
        }
    }

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