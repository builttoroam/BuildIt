using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildIt.States.Loading
{
    public class LoadingManager<TLoadingState> : ILoadingManager
    {
        private bool isLoading;
        public event EventHandler LoadingChanged;

        public TLoadingState LoadingState { get; set; }
        public TLoadingState LoadedState { get; set; }

        public bool IsLoading

        {
            get { return isLoading; }
            set
            {
                isLoading = value;

                LoadingChanged.SafeRaise(this);
            }
        }

        public TLoadingState IsLoadingState => IsLoading ? LoadingState : LoadedState;

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
