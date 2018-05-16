namespace BuildIt.UI
{
    /// <summary>
    /// Indicates whether the element has a view model accessed from the code behind
    /// </summary>
    /// <typeparam name="TViewModel">The type of view model</typeparam>
    public interface ICodeBehindViewModel<TViewModel>
        where TViewModel : class
    {
        /// <summary>
        /// Gets the wrapper instance
        /// </summary>
        ContextWrapper<TViewModel> Data { get; }
    }
}