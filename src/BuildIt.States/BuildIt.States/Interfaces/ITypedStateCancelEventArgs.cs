namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Typed state event args with cancel support.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    // ReSharper disable once TypeParameterCanBeVariant - Ignore recommentation
    public interface ITypedStateCancelEventArgs<TState> : ITypedStateEventArgs<TState>, ICancellable
    {
    }
}