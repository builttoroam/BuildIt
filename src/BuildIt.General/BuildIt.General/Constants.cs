namespace BuildIt
{
    /// <summary>
    /// Constant values
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Error message - activation exception (all instances of a type)
        /// </summary>
        public const string ActivateAllExceptionMessage =
            "Activation error occurred while trying to get all instances of type {0}";

        /// <summary>
        /// Error message - activation exception
        /// </summary>
        public const string ActivationExceptionMessage = "Activation error occurred while trying to get instance of type {0}, key \"{1}\"";

        /// <summary>
        /// Error message - Service location provider not set
        /// </summary>
        public const string ServiceLocationProviderNotSetMessage = ">Message shown if ServiceLocator.Current called before ServiceLocationProvider is set.";
    }
}
