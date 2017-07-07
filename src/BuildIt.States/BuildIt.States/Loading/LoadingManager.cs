using System;

namespace BuildIt.States.Loading
{
    /// <summary>
    /// Manager to track loading state
    /// </summary>
    /// <typeparam name="TLoadingState">The type (enum) that defines loading/loaded values</typeparam>
    public class LoadingManager<TLoadingState> : ILoadingManager
    {
        private bool isLoading;

        /// <summary>
        /// Event indicating loading has changed
        /// </summary>
        public event EventHandler LoadingChanged;

        /// <summary>
        /// Gets or sets loading state
        /// </summary>
        public TLoadingState LoadingState { get; set; }

        /// <summary>
        /// Gets or sets the loaded state
        /// </summary>
        public TLoadingState LoadedState { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether whether loading is in progress
        /// </summary>
        public bool IsLoading
        {
            get => isLoading;
            set
            {
                isLoading = value;

                LoadingChanged.SafeRaise(this);
            }
        }

        /// <summary>
        /// Gets the current loading enum value
        /// </summary>
        public TLoadingState IsLoadingState => IsLoading ? LoadingState : LoadedState;

        /// <summary>
        /// Trigger loading - dispose object to end loading
        /// </summary>
        /// <returns>Disposable to end loading</returns>
        public IDisposable Load()
        {
            return new Loader(this);
        }

        private class Loader : IDisposable
        {
            public Loader(ILoadingManager manager)
            {
                Manager = manager;
                Manager.IsLoading = true;
            }

            private ILoadingManager Manager { get; }

            public void Dispose()
            {
                Manager.IsLoading = false;
            }
        }
    }
}
