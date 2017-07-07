namespace BuildIt.States
{
    /// <summary>
    /// Event args for when state changes
    /// </summary>
    public interface IStateEventArgs
    {
        /// <summary>
        /// The state name
        /// </summary>
        string StateName { get;  }
        
        /// <summary>
        /// Whether to use transitions
        /// </summary>
        bool UseTransitions { get;  }
        
        /// <summary>
        /// Whether it's a new state
        /// </summary>
        bool IsNewState { get;  }
    }
}