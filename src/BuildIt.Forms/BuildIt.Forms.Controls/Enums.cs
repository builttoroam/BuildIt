namespace BuildIt.Forms.Controls
{
    /// <summary>
    ///  The available focus modes
    /// </summary>
    public enum CameraFocusMode
    {
        /// <summary>
        /// An unspecified focus mode
        /// </summary>
        Unspecified,

        /// <summary>
        /// The camera will automatically focus once
        /// </summary>
        Auto,

        /// <summary>
        /// The camera will continuously try to focus
        /// </summary>
        Continuous,

        /// <summary>
        /// The camera will manually focus on a point/region
        /// </summary>
        Manual
    }

    /// <summary>
    /// Defines the camera preview is displayed
    /// </summary>
    public enum CameraPreviewAspect
    {
        /// <summary>
        /// Scale the preview to fit the view. Some parts may be left empty (letter boxing)
        /// </summary>
        // Note: aspect fit is the default (as opposed to none) to be consistent with how the
        // Aspect enum is defined in Xamarin Forms to reduce cognitive load of devs
        AspectFit,

        /// <summary>
        /// Scale the preview so it exactly fills the view. Scaling may not be in uniform in X and Y
        /// </summary>
        Fill
    }
}