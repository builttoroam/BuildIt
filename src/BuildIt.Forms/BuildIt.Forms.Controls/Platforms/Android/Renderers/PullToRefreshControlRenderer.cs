using System;
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
    public class PullToRefreshControlRenderer : Xamarin.Forms.Platform.Android.AppCompat.ViewRenderer<ContentView, View>
    {
        private const float MaxSwipeDistanceFactor = .6f;
        private const int RefreshTriggerDistance = 120;

        private MotionEvent downEvent;
        private int currentPercentage;
        private double pullToRefreshContentTemplateConatinerHeight;
        private float previousY;
        private readonly Context context;
        private readonly int touchSlop;
        private int? distanceToTriggerRefresh;
        private bool isAnimatingAfterSuccessfulPulToRefresh;
        private VisualElement pullToRefreshControlContentTemplateContainer;
        private Grid pullToRefreshContentTemplateContainer;
        private Controls.PullToRefreshControl pullToRefreshControl;

        public PullToRefreshControlRenderer(Context context)
            : base(context)
        {
            this.context = context;

            touchSlop = ViewConfiguration.Get(context).ScaledPagingTouchSlop;
        }

        public Controls.PullToRefreshControl PullToRefreshControl => pullToRefreshControl ?? (pullToRefreshControl = (Element as StatefulControl)?.Children?.FirstOrDefault() as Controls.PullToRefreshControl);

        // TODO MK Use FindByName method instead of visual tree
        public VisualElement PullToRefreshControlContentTemplateContainer => pullToRefreshControlContentTemplateContainer ?? (pullToRefreshControlContentTemplateContainer = (PullToRefreshControl?.Children?.FirstOrDefault() as Grid)?.Children?.ElementAt(1) as Grid);

        public Grid PullToRefreshContentTemplateContainer => pullToRefreshContentTemplateContainer ?? (pullToRefreshContentTemplateContainer = ((PullToRefreshControl?.Children?.FirstOrDefault() as Grid)?.Children?.FirstOrDefault() as Grid)?.Children?.FirstOrDefault() as Grid);

        protected override void OnElementChanged(ElementChangedEventArgs<ContentView> e)
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
            if (isAnimatingAfterSuccessfulPulToRefresh)
            {
                return true;
            }

            return base.OnInterceptTouchEvent(e);
        }

        // https://android.googlesource.com/platform/frameworks/support/+/f25dedc/v4/java/android/support/v4/widget/SwipeRefreshLayout.java
        public override bool OnTouchEvent(MotionEvent e)
        {
            base.OnTouchEvent(e);

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

                        // update content
                        if (PullToRefreshControlContentTemplateContainer != null)
                        {
                            PullToRefreshControlContentTemplateContainer.TranslationY = offsetTop;
                        }

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
                await PullToRefreshControlContentTemplateContainer.TranslateTo(0, PullToRefreshContentTemplateContainer.Height, easing: Easing.SpringIn);
                isAnimatingAfterSuccessfulPulToRefresh = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}