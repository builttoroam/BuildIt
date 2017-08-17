namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Entity that raises a typed state changing event
    /// </summary>
    /// <typeparam name="TState">The type of the state</typeparam>
    // ReSharper disable once TypeParameterCanBeVariant - Ignore recommentation
    public interface ITypedStateEventArgs<TState> : IStateEventArgs
    {
        /// <summary>
        /// Gets the enum value of the state
        /// </summary>
        TState TypedState { get; }
    }
}