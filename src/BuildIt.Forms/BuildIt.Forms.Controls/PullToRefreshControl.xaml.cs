using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BuildIt.Forms.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PullToRefreshControl
    {
        public PullToRefreshControl()
        {
            InitializeComponent();
        }

        public WeakReference<Func<bool>> CanPullToRefreshCallback { get; set; }

        public WeakReference<Action<float>> HandlePullToRefreshDragGestureCallback { get; set; }

        public WeakReference<Action> StartPullToRefreshCallback { get; set; }

        internal bool CanPullToRefresh()
        {
            if (CanPullToRefreshCallback != null &&
                CanPullToRefreshCallback.TryGetTarget(out var canPullToRefreshCallback))
            {
                return canPullToRefreshCallback.Invoke();
            }

            return false;
        }

        internal void HandlePullToRefreshDragGesture(float offsetTop)
        {
            if (HandlePullToRefreshDragGestureCallback != null &&
                HandlePullToRefreshDragGestureCallback.TryGetTarget(out var handlePullToRefreshDragGestureCallback))
            {
                handlePullToRefreshDragGestureCallback.Invoke(offsetTop);
            }
        }

        internal void StartPullToRefresh()
        {
            if (StartPullToRefreshCallback != null &&
                StartPullToRefreshCallback.TryGetTarget(out var startPullToRefreshCallback))
            {
                startPullToRefreshCallback.Invoke();
            }
        }
    }
}