namespace BuildIt
{
    public static class Constants
    {
        public const string ActivateAllExceptionMessage =
            "Activation error occurred while trying to get all instances of type {0}";
        public const string ActivationExceptionMessage = "Activation error occurred while trying to get instance of type {0}, key \"{1}\"";
        public const string ServiceLocationProviderNotSetMessage = ">Message shown if ServiceLocator.Current called before ServiceLocationProvider is set.";
    }
}
