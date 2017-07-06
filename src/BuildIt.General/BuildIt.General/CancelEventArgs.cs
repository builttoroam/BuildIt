using System;

namespace BuildIt
{
    /// <summary>
    /// Implementation of CancelEventArgs as not in all runtimes
    /// </summary>
    public class CancelEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether whether the action should be cancelled or not
        /// </summary>
        public bool Cancel { get; set; }
    }
}