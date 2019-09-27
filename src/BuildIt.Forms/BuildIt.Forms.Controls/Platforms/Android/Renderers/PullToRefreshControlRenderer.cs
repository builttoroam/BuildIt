using System;
using System.ComponentModel;
using System.Linq;
using Android.Content;
using Android.Views;
using BuildIt.Forms.Controls;
using BuildIt.Forms.Controls.Platforms.Android.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;

[assembly: ExportRenderer(typeof(StatefulControl), typeof(PullToRefreshControlRenderer))]
namespace BuildIt.Forms.Controls.Platforms.Android.Renderers
{
    public class PullToRefreshControlRenderer : Xamarin.Forms.Platform.Android.AppCompat.ViewRenderer<StatefulControl, View>
    {
        private const float MaxSwipeDistanceFactor = .6f;
        private const int RefreshTriggerDistance = 120;

        private MotionEvent downEvent;
        private int currentPercentage;
        private float previousY;
        private readonly int touchSlop;
        private int? distanceToTriggerRefresh;
        private bool isAnimatingAfterSuccessfulPulToRefresh;

        public PullToRefreshControlRenderer(Context context)
            : base(context)
        {
            touchSlop = ViewConfiguration.Get(context).ScaledPagingTouchSlop;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<StatefulControl> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // Unsubscribe
            }

            if (e.NewElement != null)
            {
                if (!distanceToTriggerRefresh.HasValue)
                {
                    if (Element.Height > 0)
                    {
                        distanceToTriggerRefresh = (int)Math.Min(Element.Height * MaxSwipeDistanceFactor, RefreshTriggerDistance * Resources.DisplayMetrics.Density);
                    }
                }

                //var pullToRefreshControl = new PullToRefreshControl(context);
                //SetNativeControl(pullToRefreshControl);
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

            if (!distanceToTriggerRefresh.HasValue && Element.Height > 0)
            {
                distanceToTriggerRefresh = (int)Math.Min(Element.Height * MaxSwipeDistanceFactor, RefreshTriggerDistance * Resources.DisplayMetrics.Density);
            }
        }

        public override bool OnInterceptTouchEvent(MotionEvent e)
        {
            var canPullToRefresh = Element?.CanPullToRefresh() ?? true;
            System.Diagnostics.Debug.WriteLine($"[{nameof(OnInterceptTouchEvent)}] {e.Action} | {nameof(canPullToRefresh)}? {canPullToRefresh}");
            if (isAnimatingAfterSuccessfulPulToRefresh || !canPullToRefresh)
            {
                return true;
            }

            return base.OnInterceptTouchEvent(e);
        }

        // https://android.googlesource.com/platform/frameworks/support/+/f25dedc/v4/java/android/support/v4/widget/SwipeRefreshLayout.java
        public override bool OnTouchEvent(MotionEvent e)
        {
            base.OnTouchEvent(e);

            var canPullToRefresh = Element?.CanPullToRefresh() ?? true;
            System.Diagnostics.Debug.WriteLine($"[{nameof(OnTouchEvent)}] {e.Action} | {nameof(canPullToRefresh)}? {canPullToRefresh}");
            if (!canPullToRefresh)
            {
                return true;
            }

            var handled = false;
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    currentPercentage = 0;
                    downEvent = MotionEvent.Obtain(e);
                    previousY = downEvent.GetY();
                    handled = true;

                    break;

                case MotionEventActions.Move:
                    if (downEvent == null)
                    {
                        break;
                    }

                    var currentPositionY = e.GetY();
                    var initialPositionY = downEvent.GetY();
                    var positionYDiff = currentPositionY - initialPositionY;
                    if (positionYDiff > touchSlop)
                    {
                        // User velocity passed min velocity; trigger a refresh
                        if (positionYDiff > distanceToTriggerRefresh)
                        {
                            StartRefresh();

                            handled = true;
                            break;
                        }

                        var offsetTop = positionYDiff;
                        if (previousY > currentPositionY)
                        {
                            offsetTop = positionYDiff - touchSlop;
                        }

                        Element.HandlePullToRefreshDragGesture(offsetTop);

                        previousY = e.GetY();
                        handled = true;
                    }

                    break;

                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                    downEvent?.Recycle();
                    downEvent = null;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return handled;
        }

        private async void StartRefresh()
        {
            isAnimatingAfterSuccessfulPulToRefresh = true;

            try
            {
                await Element.StartPullToRefresh();
                isAnimatingAfterSuccessfulPulToRefresh = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}