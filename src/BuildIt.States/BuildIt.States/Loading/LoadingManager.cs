using System;

namespace BuildIt.States.Loading
{
    /// <summary>
    /// Manager to track loading state
    /// </summary>
    /// <typeparam name="TLoadingState"></typeparam>
    public class LoadingManager<TLoadingState> : ILoadingManager
    {
        private bool isLoading;

        /// <summary>
        /// Event indicating loading has changed
        /// </summary>
        public event EventHandler LoadingChanged;

        /// <summary>
        /// loading state
        /// </summary>
        public TLoadingState LoadingState { get; set; }

        /// <summary>
        /// The loaded state
        /// </summary>
        public TLoadingState LoadedState { get; set; }

        /// <summary>
        /// Whether loading is in progress
        /// </summary>
        public bool IsLoading

        {
            get { return isLoading; }
            set
            {
                isLoading = value;

                LoadingChanged.SafeRaise(this);
            }
        }

        /// <summary>
        /// The current loading enum value
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
            ILoadingManager Manager { get; }
            public Loader(ILoadingManager manager)
            {
                Manager = manager;
                Manager.IsLoading = true;
            }
            public void Dispose()
            {
                Manager.IsLoading = false;
            }
        }
    }
}
