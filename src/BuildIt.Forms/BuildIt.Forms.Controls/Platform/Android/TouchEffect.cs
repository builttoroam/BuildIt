using Android.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("BuildIt")]
[assembly: ExportEffect(typeof(BuildIt.Forms.Controls.Droid.TouchEffect), "TouchEffect")]

// ReSharper disable once CheckNamespace - keeping platform specific namespace
namespace BuildIt.Forms.Controls.Droid
{
    /// <summary>
    /// Touch effect implementation for Android
    /// </summary>
    [Preserve]
    public class TouchEffect : PlatformEffect
    {
        private class TouchInfo
        {
            public Android.Views.View View { get; set; }
            public bool EventsAttached { get; set; }
            public Element Element { get; set; }
            public Func<double,double> FromPixel { get; set; }
            public Forms.TouchEffect Effect { get; set; }
            public TouchEffect DroidEffect { get; set; }
        }

        private static IDictionary<Android.Views.View, TouchInfo> viewDictionary =
            new Dictionary<Android.Views.View, TouchInfo>();

        private static Dictionary<int, TouchEffect> idToEffectDictionary =
            new Dictionary<int, TouchEffect>();

        private TouchInfo currentView;
        // private Element formsElement;
        // private Forms.TouchEffect pclTouchEffect;
        private bool capture;
        // private Func<double, double> fromPixels;
        private int[] twoIntArray = new int[2];

        /// <inheritdoc />
        /// <summary>
        /// Attach the effect
        /// </summary>
        protected override void OnAttached()
        {
            // Get the Android View corresponding to the Element that the effect is attached to
            var newView = Control ?? Container;

            // Attach the new view
            AttachView(newView);
        }

        /// <inheritdoc/>
        /// <summary>
        /// Detach the effect
        /// </summary>
        protected override void OnDetached()
        {
            // Method must be overridden
            try
            {
                DetachView(currentView?.View);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void ViewAttachedToWindow(object sender, Android.Views.View.ViewAttachedToWindowEventArgs e)
        {
            AttachHandlers(e.AttachedView);
        }

        private void ViewDetachedFromWindow(object sender, Android.Views.View.ViewDetachedFromWindowEventArgs e)
        {
            DetachHandlers(e.DetachedView);
        }

        private void AttachView(Android.Views.View newView)
        {
            if (newView == null)
            {
                return;
            }

            if (currentView != null && currentView.View != newView)
            {
                // Clean up the existing view (and any event handlers)
                DetachView(currentView?.View);
            }

            var existing = viewDictionary.SafeValue(newView);

            if (existing != null)
            {
                currentView = existing;
                return;
            }

            var view = newView;

            // Get access to the TouchEffect class in the PCL
            Forms.TouchEffect touchEffect =
                    (Forms.TouchEffect)Element.Effects.
                        FirstOrDefault(ef => ef is Forms.TouchEffect);

            if (touchEffect == null || view == null)
            {
                return;
            }

            var touchInfo = new TouchInfo();
            touchInfo.View = view;
            touchInfo.DroidEffect = this;
            touchInfo.Effect = touchEffect;
            touchInfo.Element = Element;
            touchInfo.FromPixel = view.Context.FromPixels;
            viewDictionary.Add(view, touchInfo);

            if (Element is Xamarin.Forms.View fview)
            {
                ElementHelper.ApplyToAllNested<Xamarin.Forms.View>(fview, fv => fv.InputTransparent = true, false);
            }

            currentView = touchInfo;

            view.ViewAttachedToWindow += ViewAttachedToWindow;
            view.ViewDetachedFromWindow += ViewDetachedFromWindow;

            if (view.IsAttachedToWindow)
            {
                AttachHandlers(view);
            }
        }

        private void AttachHandlers(Android.Views.View newView)
        {
            if (currentView.View != newView)
            {
                AttachView(newView);
                return;
            }

            var info = viewDictionary.SafeValue(newView);
            if (info == null)
            {
                Debug.WriteLine("This shouldn't happen");
            }

            if (!info.EventsAttached)
            {
                newView.Touch += OnTouch;
                info.EventsAttached = true;
            }
        }

        private void DetachView(Android.Views.View viewToCleanup)
        {
            try
            {
                if (viewToCleanup == null || !viewDictionary.ContainsKey(viewToCleanup))
                {
                    return;
                }

                DetachHandlers(viewToCleanup);

                viewDictionary.Remove(viewToCleanup);

                viewToCleanup.ViewAttachedToWindow -= ViewAttachedToWindow;
                viewToCleanup.ViewDetachedFromWindow -= ViewDetachedFromWindow;

                currentView = null;
            }
            catch (Exception ex)
            {
                ex.LogError();
            }
        }

        private void DetachHandlers(Android.Views.View viewToCleanup)
        {
            try
            {
                var info = viewDictionary.SafeValue(viewToCleanup);
                if (info==null || !info.EventsAttached)
                {
                    return;
                }

                viewToCleanup.Touch -= OnTouch;
                info.EventsAttached = false;
            }
            catch (Exception ex)
            {
                ex.LogError();
            }
        }

        private void OnTouch(object sender, Android.Views.View.TouchEventArgs args)
        {
            try
            {
                // Two object common to all the events
                Android.Views.View senderView = sender as Android.Views.View;
                MotionEvent motionEvent = args.Event;
                var info = viewDictionary.SafeValue(senderView);
                if (info == null)
                {
                    return;
                }

                // Get the pointer index
                int pointerIndex = motionEvent.ActionIndex;

                // Get the id that identifies a finger over the course of its progress
                int id = motionEvent.GetPointerId(pointerIndex);

                senderView.GetLocationOnScreen(twoIntArray);
                var screenPointerCoords = new Point(
                    twoIntArray[0] + motionEvent.GetX(pointerIndex),
                    twoIntArray[1] + motionEvent.GetY(pointerIndex));

                // Use ActionMasked here rather than Action to reduce the number of possibilities
                switch (args.Event.ActionMasked)
                {
                    case MotionEventActions.Down:
                    case MotionEventActions.PointerDown:
                        FireEvent(this, id, TouchActionType.Pressed, screenPointerCoords, true);

                        idToEffectDictionary.Add(id, this);

                        capture = info.Effect.Capture;
                        if (capture)
                        {
                            info.View.RequestPointerCapture();
                        }

                        break;

                    case MotionEventActions.Move:

                        // Multiple Move events are bundled, so handle them in a loop
                        for (pointerIndex = 0; pointerIndex < motionEvent.PointerCount; pointerIndex++)
                        {
                            id = motionEvent.GetPointerId(pointerIndex);

                            if (capture)
                            {
                                senderView.GetLocationOnScreen(twoIntArray);

                                screenPointerCoords = new Point(
                                    twoIntArray[0] + motionEvent.GetX(pointerIndex),
                                    twoIntArray[1] + motionEvent.GetY(pointerIndex));

                                FireEvent(this, id, TouchActionType.Moved, screenPointerCoords, true);
                            }
                            else
                            {
                                CheckForBoundaryHop(id, screenPointerCoords);

                                if (idToEffectDictionary[id] != null)
                                {
                                    FireEvent(idToEffectDictionary[id], id, TouchActionType.Moved, screenPointerCoords, true);
                                }
                            }
                        }

                        break;

                    case MotionEventActions.Up:
                    case MotionEventActions.Pointer1Up:
                        if (capture)
                        {
                            info.View.ReleasePointerCapture();
                            FireEvent(this, id, TouchActionType.Released, screenPointerCoords, false);
                        }
                        else
                        {
                            CheckForBoundaryHop(id, screenPointerCoords);

                            if (idToEffectDictionary[id] != null)
                            {
                                FireEvent(idToEffectDictionary[id], id, TouchActionType.Released, screenPointerCoords, false);
                            }
                        }

                        idToEffectDictionary.Remove(id);
                        break;

                    case MotionEventActions.Cancel:
                        if (capture)
                        {
                            info.View.ReleasePointerCapture();
                            FireEvent(this, id, TouchActionType.Cancelled, screenPointerCoords, false);
                        }
                        else
                        {
                            if (idToEffectDictionary[id] != null)
                            {
                                FireEvent(idToEffectDictionary[id], id, TouchActionType.Cancelled, screenPointerCoords, false);
                            }
                        }

                        idToEffectDictionary.Remove(id);
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.LogError();
            }
        }

        private void CheckForBoundaryHop(int id, Point pointerLocation)
        {
            TouchEffect touchEffectHit = null;

            foreach (Android.Views.View view in viewDictionary.Keys)
            {
                // Get the view rectangle
                try
                {
                    view.GetLocationOnScreen(twoIntArray);
                }
                catch
                {
                    // System.ObjectDisposedException: Cannot access a disposed object.
                    continue;
                }

                Rectangle viewRect = new Rectangle(twoIntArray[0], twoIntArray[1], view.Width, view.Height);

                if (viewRect.Contains(pointerLocation))
                {
                    touchEffectHit = viewDictionary.SafeValue(view)?.DroidEffect;
                }
            }

            if (touchEffectHit != idToEffectDictionary[id])
            {
                if (idToEffectDictionary[id] != null)
                {
                    FireEvent(idToEffectDictionary[id], id, TouchActionType.Exited, pointerLocation, true);
                }

                if (touchEffectHit != null)
                {
                    FireEvent(touchEffectHit, id, TouchActionType.Entered, pointerLocation, true);
                }

                idToEffectDictionary[id] = touchEffectHit;
            }
        }

        private void FireEvent(TouchEffect touchEffect, int id, TouchActionType actionType, Point pointerLocation, bool isInContact)
        {
            var current = touchEffect.currentView;
            if (current == null)
            {
                return;
            }

            // Get the method to call for firing events
            Action<Element, TouchActionEventArgs> onTouchAction = current.Effect.OnTouchAction;
            if (onTouchAction == null)
            {
                return;
            }

            // Get the location of the pointer within the view
            current.View.GetLocationOnScreen(twoIntArray);
            double x = pointerLocation.X - twoIntArray[0];
            double y = pointerLocation.Y - twoIntArray[1];
            Point point = new Point(current.FromPixel(x), current.FromPixel(y));

            // Call the method
            onTouchAction(
                current.Element,
                new TouchActionEventArgs(id, actionType, point, isInContact));
        }
    }
}