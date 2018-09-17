namespace BuildIt.Forms.Parameters
{
    /// <summary>
    /// Provides information about the camera preview error
    /// </summary>
    /// <typeparam name="TData">Data object passed with the error</typeparam>
    public class CameraPreviewControlErrorParameters<TData>
    {
        public CameraPreviewControlErrorParameters(string error, TData data, bool handled = false)
        {
            Error = error;
            Data = data;
            Handled = Handled;
        }

        /// <summary>
        /// Gets an error message
        /// </summary>
        public string Error { get; }

        /// <summary>
        /// Gets error data
        /// </summary>
        public TData Data { get; }

        /// <summary>
        /// Gets a value indicating whether an error was handled
        /// </summary>
        public bool Handled { get; }
    }
}
