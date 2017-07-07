namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Defines an entity that can be reset to its default value
    /// </summary>
    public interface IDefaultValue
    {
        /// <summary>
        /// Resets entity to default value
        /// </summary>
        void RevertToDefault();
    }
}