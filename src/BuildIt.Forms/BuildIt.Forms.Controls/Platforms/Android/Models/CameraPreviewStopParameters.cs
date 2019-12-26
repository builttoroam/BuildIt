using BuildIt.Forms.Controls.Interfaces;

namespace BuildIt.Forms.Controls.Platforms.Android.Models
{
    /// <summary>
    /// Provides a way of passing parameters into the stop camera preview method.
    /// </summary>
    internal class CameraPreviewStopParameters : ICameraPreviewStopParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CameraPreviewStopParameters"/> class.
        /// </summary>
        /// <param name="stopBackgroundThread">Stop background thread indicator (used with camera 2 API).</param>
        /// <param name="status">Value indicating what status should be set after camera preview stops (used with camera API).</param>
        public CameraPreviewStopParameters(bool stopBackgroundThread = true, CameraStatus status = CameraStatus.Stopped)
        {
            StopBackgroundThread = stopBackgroundThread;
            Status = status;
        }

        /// <summary>
        /// Gets the default value for stop camera parameters.
        /// </summary>
        public static CameraPreviewStopParameters Default { get; } = new CameraPreviewStopParameters();

        /// <summary>
        /// Gets a value indicating whether stopping the camera we should stop the background thread - which camera 2 works with.
        /// </summary>
        public bool StopBackgroundThread { get; private set; }

        /// <summary>
        /// Gets a value indicating camera preview status that should be set after camera preview stops.
        /// </summary>
        public CameraStatus Status { get; private set; }
    }
}