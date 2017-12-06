using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;
using Xamarin.Forms;

#pragma warning disable SA1300 // Element must begin with upper-case letter - iOS platform

namespace BuildIt.Forms.Controls.iOS
#pragma warning restore SA1300 // Element must begin with upper-case letter
{
    /// <summary>
    /// Touch recognizer for intercepting touch behaviour
    /// </summary>
    public class TouchRecognizer : UIGestureRecognizer
    {
        private static Dictionary<UIView, TouchRecognizer> viewDictionary =
            new Dictionary<UIView, TouchRecognizer>();

        private static Dictionary<long, TouchRecognizer> idToTouchDictionary =
            new Dictionary<long, TouchRecognizer>();

        private Element element;        // Forms element for firing events
        private UIView view;            // iOS UIView
        private Forms.TouchEffect touchEffect;
        private bool capture;

        /// <summary>
        /// Initializes a new instance of the <see cref="TouchRecognizer"/> class.
        /// </summary>
        /// <param name="element">The element to add the recognizer to</param>
        /// <param name="view">The view to connect to</param>
        /// <param name="touchEffect">The touch events to monitor for</param>
        public TouchRecognizer(Element element, UIView view, Forms.TouchEffect touchEffect)
        {
            this.element = element;
            this.view = view;
            this.touchEffect = touchEffect;

            viewDictionary.Add(view, this);
        }

        /// <summary>
        /// Detach the effect
        /// </summary>
        public void Detach()
        {
            viewDictionary.Remove(view);
        }

        /// <summary>
        /// Touches starts
        /// </summary>
        /// <param name="touches">Touches of interest</param>
        /// <param name="evt">All touches of type UITouch</param>
        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            try
            {
                base.TouchesBegan(touches, evt);

                foreach (UITouch touch in touches.Cast<UITouch>())
                {
                    long id = touch.Handle.ToInt64();
                    FireEvent(this, id, TouchActionType.Pressed, touch, true);

                    idToTouchDictionary[id] = this;
                }

                // Save the setting of the Capture property
                capture = touchEffect.Capture;
            }
            catch (Exception ex)
            {
                ex.LogError();
            }
        }

        /// <summary>
        /// Touch moved
        /// </summary>
        /// <param name="touches">The touches of interest</param>
        /// <param name="evt">All the touches</param>
        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            try
            {
                base.TouchesMoved(touches, evt);

                foreach (UITouch touch in touches.Cast<UITouch>())
                {
                    long id = touch.Handle.ToInt64();

                    if (capture)
                    {
                        FireEvent(this, id, TouchActionType.Moved, touch, true);
                    }
                    else
                    {
                        CheckForBoundaryHop(touch);

                        if (idToTouchDictionary[id] != null)
                        {
                            FireEvent(idToTouchDictionary[id], id, TouchActionType.Moved, touch, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogError();
            }
        }

        /// <summary>
        /// Handle when a touch ends
        /// </summary>
        /// <param name="touches">The touches ending</param>
        /// <param name="evt">The touch event</param>
        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            try
            {
                base.TouchesEnded(touches, evt);

                foreach (UITouch touch in touches.Cast<UITouch>())
                {
                    long id = touch.Handle.ToInt64();

                    if (capture)
                    {
                        FireEvent(this, id, TouchActionType.Released, touch, false);
                    }
                    else
                    {
                        CheckForBoundaryHop(touch);

                        if (idToTouchDictionary.TryGetValue(id, out var idElement) && idElement != null)
                        {
                            FireEvent(idElement, id, TouchActionType.Released, touch, false);
                        }
                    }

                    idToTouchDictionary.Remove(id);
                }
            }
            catch (Exception ex)
            {
                ex.LogError();
            }
        }

        /// <summary>
        /// Touch cancelled
        /// </summary>
        /// <param name="touches">Touches of interest</param>
        /// <param name="evt">All the touches</param>
        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            try
            {
                base.TouchesCancelled(touches, evt);

                foreach (UITouch touch in touches.Cast<UITouch>())
                {
                    long id = touch.Handle.ToInt64();

                    if (capture)
                    {
                        FireEvent(this, id, TouchActionType.Cancelled, touch, false);
                    }
                    else if (idToTouchDictionary[id] != null)
                    {
                        FireEvent(idToTouchDictionary[id], id, TouchActionType.Cancelled, touch, false);
                    }

                    idToTouchDictionary.Remove(id);
                }
            }
            catch (Exception ex)
            {
                ex.LogError();
            }
        }

        private void CheckForBoundaryHop(UITouch touch)
        {
            long id = touch.Handle.ToInt64();

            // TODO: Might require converting to a List for multiple hits
            TouchRecognizer recognizerHit = null;

            foreach (UIView view in viewDictionary.Keys)
            {
                CGPoint location = touch.LocationInView(view);

                if (new CGRect(new CGPoint(0.0, 0.0), view.Frame.Size).Contains(location))
                {
                    recognizerHit = viewDictionary[view];
                }
            }

            if (recognizerHit != idToTouchDictionary[id])
            {
                if (idToTouchDictionary[id] != null)
                {
                    FireEvent(idToTouchDictionary[id], id, TouchActionType.Exited, touch, true);
                }

                if (recognizerHit != null)
                {
                    FireEvent(recognizerHit, id, TouchActionType.Entered, touch, true);
                }

                idToTouchDictionary[id] = recognizerHit;
            }
        }

        private void FireEvent(TouchRecognizer recognizer, long id, TouchActionType actionType, UITouch touch, bool isInContact)
        {
            try
            {
                // Convert touch location to Xamarin.Forms Point value
                var touchPoint = touch.LocationInView(recognizer.View);
                var formsPoint = new Point(touchPoint.X, touchPoint.Y);

                // Get the method to call for firing events
                Action<Element, TouchActionEventArgs> onTouchAction = recognizer.touchEffect.OnTouchAction;

                // Call that method
                onTouchAction(
                    recognizer.element,
                    new TouchActionEventArgs(id, actionType, formsPoint, isInContact));
            }
            catch (Exception ex)
            {
                ex.LogError();
            }
        }
    }
}