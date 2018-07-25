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
    /// Enumeration the available camera facings/positions
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
        Front
    }
}