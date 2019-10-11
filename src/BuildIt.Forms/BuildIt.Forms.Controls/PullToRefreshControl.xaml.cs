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

        public Func<bool> CanPullToRefreshCallback { get; set; }

        public Action<float> HandlePullToRefreshDragGestureCallback { get; set; }

        public Func<Task> StartPullToRefreshCallback { get; set; }

        internal bool CanPullToRefresh()
        {
            return CanPullToRefreshCallback?.Invoke() ?? false;
        }

        internal void HandlePullToRefreshDragGesture(float offsetTop)
        {
            HandlePullToRefreshDragGestureCallback?.Invoke(offsetTop);
        }

        internal async Task StartPullToRefresh()
        {
            if (StartPullToRefreshCallback == null)
            {
                return;
            }

            await StartPullToRefreshCallback.Invoke();
        }
    }
}