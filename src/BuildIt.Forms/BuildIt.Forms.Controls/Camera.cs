namespace BuildIt.Forms.Controls
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
}