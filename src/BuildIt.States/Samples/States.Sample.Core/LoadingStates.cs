namespace States.Sample.Core
{
    /// <summary>
    /// The loading states
    /// </summary>
    public enum LoadingStates
    {
        /// <summary>
        /// Default state
        /// </summary>
        Base,

        /// <summary>
        /// Loading data
        /// </summary>
        UILoading,

        /// <summary>
        /// Data loaded
        /// </summary>
        UILoaded,

        /// <summary>
        /// Data failed to load
        /// </summary>
        UILoadingFailed
    }
}