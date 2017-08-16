using System;
using System.Linq;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ResolutionGroupName("BuildIt")]
[assembly: ExportEffect(typeof(BuildIt.Forms.Controls.UWP.TouchEffect), "TouchEffect")]

namespace BuildIt.Forms.Controls.UWP
{
    /// <summary>
    /// Effect for detecting touch input
    /// </summary>
    public class TouchEffect : PlatformEffect
    {
        private FrameworkElement frameworkElement;
        private BuildIt.Forms.Controls.TouchEffect effect;
        private Action<Element, TouchActionEventArgs> onTouchAction;

        /// <summary>
        /// Attach the effect
        /// </summary>
        protected override void OnAttached()
        {
            // Get the Windows FrameworkElement corresponding to the Element that the effect is attached to
            frameworkElement = Control ?? Container;

            // Get access to the TouchEffect class in the PCL
            effect = (BuildIt.Forms.Controls.TouchEffect)Element.Effects.
                        FirstOrDefault(e => e is BuildIt.Forms.Controls.TouchEffect);

            if (effect != null && frameworkElement != null)
            {
                // Save the method to call on touch events
                onTouchAction = effect.OnTouchAction;

                // Set event handlers on FrameworkElement
                frameworkElement.PointerEntered += OnPointerEntered;
                frameworkElement.PointerPressed += OnPointerPressed;
                frameworkElement.PointerMoved += OnPointerMoved;
                frameworkElement.PointerReleased += OnPointerReleased;
                frameworkElement.PointerExited += OnPointerExited;
                frameworkElement.PointerCanceled += OnPointerCancelled;
            }
        }

        /// <summary>
        /// Detach the effect
        /// </summary>
        protected override void OnDetached()
        {
            if (onTouchAction == null)
            {
                return;
            }

            // Release event handlers on FrameworkElement
            frameworkElement.PointerEntered -= OnPointerEntered;
            frameworkElement.PointerPressed -= OnPointerPressed;
            frameworkElement.PointerMoved -= OnPointerMoved;
            frameworkElement.PointerReleased -= OnPointerReleased;
            frameworkElement.PointerExited -= OnPointerEntered;
            frameworkElement.PointerCanceled -= OnPointerCancelled;
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs args)
        {
            CommonHandler(sender, TouchActionType.Entered, args);
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs args)
        {
            CommonHandler(sender, TouchActionType.Pressed, args);

            // Check setting of Capture property
            if (effect.Capture)
            {
                (sender as FrameworkElement).CapturePointer(args.Pointer);
            }
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs args)
        {
            CommonHandler(sender, TouchActionType.Moved, args);
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs args)
        {
            CommonHandler(sender, TouchActionType.Released, args);
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs args)
        {
            CommonHandler(sender, TouchActionType.Exited, args);
        }

        private void OnPointerCancelled(object sender, PointerRoutedEventArgs args)
        {
            CommonHandler(sender, TouchActionType.Cancelled, args);
        }

        private void CommonHandler(object sender, TouchActionType touchActionType, PointerRoutedEventArgs args)
        {
            PointerPoint pointerPoint = args.GetCurrentPoint(sender as UIElement);
            Windows.Foundation.Point windowsPoint = pointerPoint.Position;

            onTouchAction(Element, new TouchActionEventArgs(args.Pointer.PointerId, touchActionType, new Point(windowsPoint.X, windowsPoint.Y), args.Pointer.IsInContact));
        }
    }
}
