namespace BuildIt.Media.Uno.WinUI
{
    ///// <summary>
    /////  The available focus modes.
    ///// </summary>
    //public enum CameraFocusMode
    //{
    //    /// <summary>
    //    /// An unspecified focus mode
    //    /// </summary>
    //    Unspecified = 0,

    //    /// <summary>
    //    /// The camera will manually focus on a point/region
    //    /// </summary>
    //    Manual = 1,

    //    /// <summary>
    //    /// The camera will automatically focus once
    //    /// </summary>
    //    Auto = 2,

    //    /// <summary>
    //    /// The camera will continuously try to focus
    //    /// </summary>
    //    Continuous = 3,
    //}

    /// <summary>
    /// Enumeration the available camera facings/positions.
    /// </summary>
    public enum CameraFacing
    {
        /// <summary>
        /// An unspecified camera facing
        /// </summary>
        Unspecified,

        /// <summary>
        /// The camera located on the back of the device enclosure
        /// </summary>
        Back,

        /// <summary>
        /// The front-facing camera
        /// </summary>
        Front,
    }

    /// <summary>
    /// Enumeration of camera statuses.
    /// </summary>
    public enum CameraStatus
    {
        /// <summary>
        /// Default state of the camera. Camera hasn't been interacted with
        /// </summary>
        None,

        /// <summary>
        /// Camera preview about to be started
        /// </summary>
        Starting,

        /// <summary>
        /// Camera preview has been started
        /// </summary>
        Running,

        /// <summary>
        /// Camera preview has been paused
        /// </summary>
        Paused,

        /// <summary>
        /// Camera preview has been stopped
        /// </summary>
        Stopped,

        /// <summary>
        /// Camera preview is in an error state
        /// </summary>
        Error,
    }
}