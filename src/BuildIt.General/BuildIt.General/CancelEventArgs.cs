using System;

namespace BuildIt
{
    /// <summary>
    /// Implementation of CancelEventArgs as not in all runtimes
    /// </summary>
    public class CancelEventArgs : EventArgs
    {
        /// <summary>
        /// Whether the action should be cancelled or not
        /// </summary>
        public bool Cancel { get; set; }
    }
}