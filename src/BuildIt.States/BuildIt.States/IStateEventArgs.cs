namespace BuildIt.States
{
    /// <summary>
    /// Event args for when state changes
    /// </summary>
    public interface IStateEventArgs
    {
        /// <summary>
        /// Gets the state name
        /// </summary>
        string StateName { get;  }

        /// <summary>
        /// Gets a value indicating whether whether to use transitions
        /// </summary>
        bool UseTransitions { get;  }

        /// <summary>
        /// Gets a value indicating whether whether it's a new state
        /// </summary>
        bool IsNewState { get;  }
    }
}