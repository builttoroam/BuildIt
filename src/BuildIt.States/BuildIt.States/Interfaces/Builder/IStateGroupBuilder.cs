namespace BuildIt.States.Interfaces.Builder
{
    /// <summary>
    /// Builder for state group
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    public interface IStateGroupBuilder<TState> : IStateBuilder
        where TState : struct
    {
        /// <summary>
        /// The typed state group
        /// </summary>
        IEnumStateGroup<TState> StateGroup { get; }
    }
}