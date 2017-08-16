namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Generic version of ITypedStateDefinition
    /// </summary>
    /// <typeparam name="TState">The type of states</typeparam>
    public interface ITypedStateDefinition<TState> : IStateDefinition
    {
        /// <summary>
        /// Gets or sets gets the name of the state
        /// </summary>
        TState State { get; set;  }
    }
}