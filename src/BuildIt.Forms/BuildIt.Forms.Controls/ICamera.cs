using System.Collections.Generic;

namespace BuildIt.Forms.Controls
{
    /// <summary>
    /// Information about a camera device
    /// </summary>
    public interface ICamera
    {
        /// <summary>
        /// Unique identifier for the camera
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The direction the camera is facing
        /// </summary>
        CameraFacing CameraFacing { get; }
    }
}