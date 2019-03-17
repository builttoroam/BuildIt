namespace BuildIt.Forms.Controls.Extensions
{
    /// <summary>
    /// Extension methods for the CameraPreviewControl
    /// </summary>
    internal static class CameraPreviewControlExtensions
    {
        /// <summary>
        /// Extension method that safely sets status
        /// </summary>
        /// <param name="control">CameraPreviewControl control</param>
        /// <param name="status">Camera status</param>
        internal static void SetStatus(this CameraPreviewControl control, CameraStatus status)
        {
            if (control == null)
            {
                return;
            }

            control.Status = status;
        }
    }
}
