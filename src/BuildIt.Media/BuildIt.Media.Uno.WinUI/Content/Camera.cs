namespace BuildIt.Media.Uno.WinUI
{
    /// <summary>
    /// Information about a camera device.
    /// </summary>
    public class Camera : ICamera
    {
        /// <summary>
        /// Gets or sets the unique identifier for the camera.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the direction the camera is facing.
        /// </summary>
        public CameraFacing CameraFacing { get; set; }
    }


    /// <summary>
    /// Enum indicating result of <see cref="CameraHelper"/> initialization.
    /// </summary>
    public enum CameraResult
    {
        /// <summary>
        /// Initialization was successful.
        /// </summary>
        Success,

        /// <summary>
        /// Camera preview isn't supported on the current platform
        /// </summary>
        NotSupportedOnPlatform,

        /// <summary>
        /// Initialization failed; Frame Reader Creation failed.
        /// </summary>
        CreateFrameReaderFailed,

        /// <summary>
        /// Initialization failed; Unable to start Frame Reader.
        /// </summary>
        StartFrameReaderFailed,

        /// <summary>
        /// Initialization failed; Frame Source Group is null.
        /// </summary>
        NoFrameSourceGroupAvailable,

        /// <summary>
        /// Initialization failed; Frame Source is null.
        /// </summary>
        NoFrameSourceAvailable,

        /// <summary>
        /// Access to the camera is denied.
        /// </summary>
        CameraAccessDenied,

        /// <summary>
        /// Initialization failed due to an exception.
        /// </summary>
        InitializationFailed_UnknownError,

        /// <summary>
        /// Initialization failed; No compatible frame format exposed by the frame source.
        /// </summary>
        NoCompatibleFrameFormatAvailable
    }
}