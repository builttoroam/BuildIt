using System;

namespace BuildIt.ServiceLocation
{
    /// <summary>
    /// This class provides the ambient container for this application. If your
    /// framework defines such an ambient container, use ServiceLocator.Current
    /// to get it.
    /// </summary>
    public static class ServiceLocator
    {
        private static ServiceLocatorProvider currentProvider;

        /// <summary>
        /// Gets a value indicating whether indicates whether the service location provider has been specified (use SetLocatorProvider)
        /// </summary>
        public static bool IsLocationProviderSet => currentProvider != null;

        /// <summary>
        /// Gets the current ambient container.
        /// </summary>
        public static IServiceLocator Current
        {
            get
            {
                if (!IsLocationProviderSet)
                {
                    throw new InvalidOperationException(Constants.ServiceLocationProviderNotSetMessage);
                }

                return currentProvider();
            }
        }

        /// <summary>
        /// Set the delegate that is used to retrieve the current container.
        /// </summary>
        /// <param name="newProvider">Delegate that, when called, will return
        /// the current ambient container.</param>
        public static void SetLocatorProvider(ServiceLocatorProvider newProvider)
        {
            currentProvider = newProvider;
        }
    }
}
