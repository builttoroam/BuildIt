namespace BuildIt.Forms.Controls
{
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
        Started,

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

    /// <summary>
    /// Enumeration of camera error codes.
    /// </summary>
    public enum CameraErrorCode
    {
        /// <summary>
        /// No errors
        /// </summary>
        None,

        /// <summary>
        /// Indicates that the camera failure/error relates to the lack of app camera permissions
        /// </summary>
        PermissionsNotGranted,
    }
}