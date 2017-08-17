namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Indicates that something can be cancelled
    /// </summary>
    public interface ICancellable
    {
        /// <summary>
        ///  Gets or sets a value indicating whether whether the action should be cancelled or not
        /// </summary>
        bool Cancel { get; set; }
    }
}