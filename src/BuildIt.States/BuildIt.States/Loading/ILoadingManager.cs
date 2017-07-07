using System;

namespace BuildIt.States.Loading
{
    /// <summary>
    /// Entity that indicates if it is loading
    /// </summary>
    public interface ILoadingManager
    {
        /// <summary>
        /// Indicates when loading status changes
        /// </summary>
        event EventHandler LoadingChanged;

        /// <summary>
        /// Gets or sets a value indicating whether loading status
        /// </summary>
        bool IsLoading { get; set; }
    }
}