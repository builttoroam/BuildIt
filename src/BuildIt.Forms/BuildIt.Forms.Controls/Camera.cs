using static BuildIt.Forms.Controls.CameraPreviewControl;

namespace BuildIt.Forms.Controls
{
    /// <summary>
    /// Information about a camera device
    /// </summary>
    public class Camera : ICamera
    {
        /// <summary>
        /// Gets the unique identifier for the camera
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets the direction the camera is facing
        /// </summary>
        public CameraFacing CameraFacing { get; set; }
    }
}