using Android.Views;
using System;
using System.Collections.Generic;
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
        private static Dictionary<Android.Views.View, TouchEffect> viewDictionary =
            new Dictionary<Android.Views.View, TouchEffect>();

        private static Dictionary<int, TouchEffect> idToEffectDictionary =
            new Dictionary<int, TouchEffect>();

        private Android.Views.View view;
        private Element formsElement;
        private Forms.TouchEffect pclTouchEffect;
        private bool capture;
        private Func<double, double> fromPixels;
        private int[] twoIntArray = new int[2];

        /// <summary>
        /// Attach the effect
        /// </summary>
        protected override void OnAttached()
        {
            // Get the Android View corresponding to the Element that the effect is attached to
            view = Control ?? Container;

            // Get access to the TouchEffect class in the PCL
            Forms.TouchEffect touchEffect =
                (Forms.TouchEffect)Element.Effects.
                    FirstOrDefault(e => e is Forms.TouchEffect);

            if (touchEffect == null || view == null)
            {
                return;
            }

            viewDictionary.Add(view, this);

            formsElement = Element;

            if (formsElement is Xamarin.Forms.View fview)
            {
                ElementHelper.ApplyToAllNested<Xamarin.Forms.View>(fview, e => e.InputTransparent = true, false);
            }

            pclTouchEffect = touchEffect;

            // Save fromPixels function
            fromPixels = view.Context.FromPixels;

            // Set event handler on View
            view.Touch += OnTouch;
        }

        /// <summary>
        /// Detach effect
        /// </summary>
        protected override void OnDetached()
        {
            if (!viewDictionary.ContainsKey(view))
            {
                return;
            }

            viewDictionary.Remove(view);
            view.Touch -= OnTouch;
        }

        private void OnTouch(object sender, Android.Views.View.TouchEventArgs args)
        {
            try
            {
                // Two object common to all the events
                Android.Views.View senderView = sender as Android.Views.View;
                MotionEvent motionEvent = args.Event;

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

                        capture = pclTouchEffect.Capture;
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
                    touchEffectHit = viewDictionary[view];
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
            // Get the method to call for firing events
            Action<Element, TouchActionEventArgs> onTouchAction = touchEffect.pclTouchEffect.OnTouchAction;

            // Get the location of the pointer within the view
            touchEffect.view.GetLocationOnScreen(twoIntArray);
            double x = pointerLocation.X - twoIntArray[0];
            double y = pointerLocation.Y - twoIntArray[1];
            Point point = new Point(fromPixels(x), fromPixels(y));

            // Call the method
            onTouchAction(
                touchEffect.formsElement,
                new TouchActionEventArgs(id, actionType, point, isInContact));
        }
    }
}