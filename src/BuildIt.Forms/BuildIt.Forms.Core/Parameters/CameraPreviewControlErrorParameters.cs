namespace BuildIt.Forms.Parameters
{
    /// <summary>
    /// Provides information about the camera preview error
    /// </summary>
    /// <typeparam name="TData">Data object passed with the error</typeparam>
    public class CameraPreviewControlErrorParameters<TData>
    {
        /// <summary>
        /// Gets an error message
        /// </summary>
        public string Error { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether an error was handled
        /// </summary>
        public bool Handled { get; internal set; }

        /// <summary>
        /// Gets error data
        /// </summary>
        public TData Data { get; internal set; }
    }
}
