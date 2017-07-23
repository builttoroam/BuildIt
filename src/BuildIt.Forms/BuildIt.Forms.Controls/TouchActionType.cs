namespace BuildIt.Forms.Controls
{
    /// <summary>
    /// Action states for touch interaction
    /// </summary>
    public enum TouchActionType
    {
        /// <summary>
        /// Cursor enters the control
        /// </summary>
        Entered,

        /// <summary>
        /// Item clicked/pressed
        /// </summary>
        Pressed,

        /// <summary>
        /// Cursor/Touch moved
        /// </summary>
        Moved,

        /// <summary>
        /// Cursor is released
        /// </summary>
        Released,

        /// <summary>
        /// Cursor exits
        /// </summary>
        Exited,

        /// <summary>
        /// Interaction cancelled
        /// </summary>
        Cancelled
    }
}
