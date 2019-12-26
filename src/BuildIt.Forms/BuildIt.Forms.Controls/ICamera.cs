namespace BuildIt.Forms.Controls
{
    /// <summary>
    /// Information about a camera device.
    /// </summary>
    public interface ICamera
    {
        /// <summary>
        /// Gets the Unique identifier for the camera.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the direction the camera is facing.
        /// </summary>
        CameraFacing CameraFacing { get; }
    }
}