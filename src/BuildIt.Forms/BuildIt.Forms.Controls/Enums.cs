namespace BuildIt.Forms.Controls
{
    /// <summary>
    ///  The available focus modes
    /// </summary>
    public enum CameraFocusMode
    {
        // An unspecified focus mode
        Unspecified,

        // The camera will automatically focus once
        Auto,

        // The camera will continuously try to focus
        Continuous,

        // The camera will manually focus on a point/region
        Manual
    }
}