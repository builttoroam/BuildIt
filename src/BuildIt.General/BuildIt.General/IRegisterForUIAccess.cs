namespace BuildIt
{
    /// <summary>
    /// Interface that allows an entity to register for UI access.
    /// </summary>
    public interface IRegisterForUIAccess : IRequiresUIAccess
    {
        /// <summary>
        /// Register for UI access.
        /// </summary>
        /// <param name="context">The context that can be used for invoking actions on the UI thread.</param>
        void RegisterForUIAccess(IUIExecutionContext context);
    }
}