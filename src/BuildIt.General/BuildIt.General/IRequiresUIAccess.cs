namespace BuildIt
{
    /// <summary>
    /// Interface that exposes the UI context
    /// </summary>
    public interface IRequiresUIAccess
    {
        /// <summary>
        /// The UI context registered with the entity
        /// </summary>
        IUIExecutionContext UIContext { get; }
    }
}