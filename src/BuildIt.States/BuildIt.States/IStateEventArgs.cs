namespace BuildIt.States
{
    public interface IStateEventArgs
    {
        string StateName { get;  }
        bool UseTransitions { get;  }
        bool IsNewState { get;  }
    }
}