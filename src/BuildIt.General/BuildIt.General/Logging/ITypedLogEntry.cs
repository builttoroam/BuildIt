namespace BuildIt.Logging
{
    /// <summary>
    /// Typed log entry
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to be logged</typeparam>
    // ReSharper disable once TypeParameterCanBeVariant - ignoring this!
    public interface ITypedLogEntry<TEntity> : ILogEntry
    {
        /// <summary>
        /// Gets the entity to be logged
        /// </summary>
        TEntity Entity { get; }
    }
}