using System;
using Windows.Devices.Enumeration;
using Windows.Devices.Sensors;
using Windows.Graphics.Display;
using Windows.Media.Capture;
using Windows.Storage.FileProperties;

namespace BuildIt.Forms.Controls.Platforms.Uap.Helpers
{
    /// <summary>
    /// A helper class for roatating camera feed previews.
    /// </summary>
    public class CameraRotationHelper
    {
        private readonly EnclosureLocation cameraEnclosureLocation;
        private readonly DisplayInformation displayInformation = DisplayInformation.GetForCurrentView();
        private readonly SimpleOrientationSensor orientationSensor = SimpleOrientationSensor.GetDefault();

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraRotationHelper"/> class.
        /// </summary>
        /// <param name="cameraEnclosureLocation">The location on the device where the camera is.</param>
        public CameraRotationHelper(EnclosureLocation cameraEnclosureLocation)
        {
            this.cameraEnclosureLocation = cameraEnclosureLocation;
            if (!IsEnclosureLocationExternal(this.cameraEnclosureLocation) && orientationSensor != null)
            {
                orientationSensor.OrientationChanged += SimpleOrientationSensor_OrientationChanged;
            }

            displayInformation.OrientationChanged += DisplayInformation_OrientationChanged;
        }

        /// <summary>
        /// Occurs each time the simple orientation sensor reports a new sensor reading or when the display's current or native orientation changes
        /// </summary>
        public event EventHandler<bool> OrientationChanged;

        /// <summary>
        /// Detects whether or not the camera is external to the device.
        /// </summary>
        /// <param name="enclosureLocation">The location on the device where the camera is.</param>
        /// <returns><see cref="bool"/>.</returns>
        public static bool IsEnclosureLocationExternal(EnclosureLocation enclosureLocation)
        {
            return enclosureLocation == null || enclosureLocation.Panel == Panel.Unknown;
        }

        /// <summary>
        /// Converters an instance of <see cref="SimpleOrientation"/> to <see cref="PhotoOrientation"/>.
        /// </summary>
        /// <param name="orientation">The incoming <see cref="SimpleOrientation"/>.</param>
        /// <returns>Converter <see cref="PhotoOrientation"/>.</returns>
        public static PhotoOrientation ConvertSimpleOrientationToPhotoOrientation(SimpleOrientation orientation)
        {
            switch (orientation)
            {
                case SimpleOrientation.Rotated90DegreesCounterclockwise:
                    return PhotoOrientation.Rotate90;

                case SimpleOrientation.Rotated180DegreesCounterclockwise:
                    return PhotoOrientation.Rotate180;

                case SimpleOrientation.Rotated270DegreesCounterclockwise:
                    return PhotoOrientation.Rotate270;

                default:
                    return PhotoOrientation.Normal;
            }
        }

        /// <summary>
        /// Converters an instance of <see cref="SimpleOrientation"/> to the number of degrees clockwise of orientation.
        /// </summary>
        /// <param name="orientation">The incoming <see cref="SimpleOrientation"/>.</param>
        /// <returns>Degrees rotation clockwise.</returns>
        public static int ConvertSimpleOrientationToClockwiseDegrees(SimpleOrientation orientation)
        {
            switch (orientation)
            {
                case SimpleOrientation.Rotated90DegreesCounterclockwise:
                    return 270;

                case SimpleOrientation.Rotated180DegreesCounterclockwise:
                    return 180;

                case SimpleOrientation.Rotated270DegreesCounterclockwise:
                    return 90;

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Gets the rotation to rotate ui elements.
        /// </summary>
        /// <returns><see cref="SimpleOrientation"/>.</returns>
        public SimpleOrientation GetUiOrientation()
        {
            if (IsEnclosureLocationExternal(cameraEnclosureLocation))
            {
                // Cameras that are not attached to the device do not rotate along with it, so apply no rotation
                return SimpleOrientation.NotRotated;
            }

            // Return the difference between the orientation of the device and the orientation of the app display
            var deviceOrientation = orientationSensor?.GetCurrentOrientation() ?? SimpleOrientation.NotRotated;
            var displayOrientation = ConvertDisplayOrientationToSimpleOrientation(displayInformation.CurrentOrientation);
            return SubtractOrientations(displayOrientation, deviceOrientation);
        }

        /// <summary>
        /// Gets the rotation of the camera to rotate pictures/videos when saving to file.
        /// </summary>
        /// <returns><see cref="SimpleOrientation"/>.</returns>
        public SimpleOrientation GetCameraCaptureOrientation()
        {
            if (IsEnclosureLocationExternal(cameraEnclosureLocation))
            {
                // Cameras that are not attached to the device do not rotate along with it, so apply no rotation
                return SimpleOrientation.NotRotated;
            }

            // Get the device orientation offset by the camera hardware offset
            var deviceOrientation = orientationSensor?.GetCurrentOrientation() ?? SimpleOrientation.NotRotated;
            var result = SubtractOrientations(deviceOrientation, GetCameraOrientationRelativeToNativeOrientation());

            // If the preview is being mirrored for a front-facing camera, then the rotation should be inverted
            if (ShouldMirrorPreview())
            {
                result = MirrorOrientation(result);
            }

            return result;
        }

        /// <summary>
        /// Gets the rotation of the camera to display the camera preview.
        /// </summary>
        /// <returns><see cref="SimpleOrientation"/>.</returns>
        public SimpleOrientation GetCameraPreviewOrientation()
        {
            if (IsEnclosureLocationExternal(cameraEnclosureLocation))
            {
                // Cameras that are not attached to the device do not rotate along with it, so apply no rotation
                return SimpleOrientation.NotRotated;
            }

            // Get the app display rotation offset by the camera hardware offset
            var result = ConvertDisplayOrientationToSimpleOrientation(displayInformation.CurrentOrientation);
            result = SubtractOrientations(result, GetCameraOrientationRelativeToNativeOrientation());

            // If the preview is being mirrored for a front-facing camera, then the rotation should be inverted
            if (ShouldMirrorPreview())
            {
                result = MirrorOrientation(result);
            }

            return result;
        }

        private static SimpleOrientation MirrorOrientation(SimpleOrientation orientation)
        {
            // This only affects the 90 and 270 degree cases, because rotating 0 and 180 degrees is the same clockwise and counter-clockwise
            switch (orientation)
            {
                case SimpleOrientation.Rotated90DegreesCounterclockwise:
                    return SimpleOrientation.Rotated270DegreesCounterclockwise;

                case SimpleOrientation.Rotated270DegreesCounterclockwise:
                    return SimpleOrientation.Rotated90DegreesCounterclockwise;
            }

            return orientation;
        }

        private static SimpleOrientation AddOrientations(SimpleOrientation a, SimpleOrientation b)
        {
            var aRot = ConvertSimpleOrientationToClockwiseDegrees(a);
            var bRot = ConvertSimpleOrientationToClockwiseDegrees(b);
            var result = (aRot + bRot) % 360;
            return ConvertClockwiseDegreesToSimpleOrientation(result);
        }

        private static SimpleOrientation SubtractOrientations(SimpleOrientation a, SimpleOrientation b)
        {
            var aRot = ConvertSimpleOrientationToClockwiseDegrees(a);
            var bRot = ConvertSimpleOrientationToClockwiseDegrees(b);

            // Add 360 to ensure the modulus operator does not operate on a negative
            var result = (360 + (aRot - bRot)) % 360;
            return ConvertClockwiseDegreesToSimpleOrientation(result);
        }

        private static VideoRotation ConvertSimpleOrientationToVideoRotation(SimpleOrientation orientation)
        {
            switch (orientation)
            {
                case SimpleOrientation.Rotated90DegreesCounterclockwise:
                    return VideoRotation.Clockwise270Degrees;

                case SimpleOrientation.Rotated180DegreesCounterclockwise:
                    return VideoRotation.Clockwise180Degrees;

                case SimpleOrientation.Rotated270DegreesCounterclockwise:
                    return VideoRotation.Clockwise90Degrees;

                default:
                    return VideoRotation.None;
            }
        }

        private static SimpleOrientation ConvertClockwiseDegreesToSimpleOrientation(int orientation)
        {
            switch (orientation)
            {
                case 270:
                    return SimpleOrientation.Rotated90DegreesCounterclockwise;

                case 180:
                    return SimpleOrientation.Rotated180DegreesCounterclockwise;

                case 90:
                    return SimpleOrientation.Rotated270DegreesCounterclockwise;

                default:
                    return SimpleOrientation.NotRotated;
            }
        }

        private SimpleOrientation ConvertDisplayOrientationToSimpleOrientation(DisplayOrientations orientation)
        {
            SimpleOrientation result;
            switch (orientation)
            {
                case DisplayOrientations.Landscape:
                    result = SimpleOrientation.NotRotated;
                    break;

                case DisplayOrientations.PortraitFlipped:
                    result = SimpleOrientation.Rotated90DegreesCounterclockwise;
                    break;

                case DisplayOrientations.LandscapeFlipped:
                    result = SimpleOrientation.Rotated180DegreesCounterclockwise;
                    break;

                default:
                    result = SimpleOrientation.Rotated270DegreesCounterclockwise;
                    break;
            }

            // Above assumes landscape; offset is needed if native orientation is portrait
            if (displayInformation.NativeOrientation == DisplayOrientations.Portrait)
            {
                result = AddOrientations(result, SimpleOrientation.Rotated90DegreesCounterclockwise);
            }

            return result;
        }

        private void SimpleOrientationSensor_OrientationChanged(SimpleOrientationSensor sender, SimpleOrientationSensorOrientationChangedEventArgs args)
        {
            if (args.Orientation != SimpleOrientation.Faceup && args.Orientation != SimpleOrientation.Facedown)
            {
                // Only raise the OrientationChanged event if the device is not parallel to the ground. This allows users to take pictures of documents (FaceUp)
                // or the ceiling (FaceDown) in portrait or landscape, by first holding the device in the desired orientation, and then pointing the camera
                // either up or down, at the desired subject.
                // Note: This assumes that the camera is either facing the same way as the screen, or the opposite way. For devices with cameras mounted
                //      on other panels, this logic should be adjusted.
                OrientationChanged?.Invoke(this, false);
            }
        }

        private void DisplayInformation_OrientationChanged(DisplayInformation sender, object args)
        {
            OrientationChanged?.Invoke(this, true);
        }

        private bool ShouldMirrorPreview()
        {
            // It is recommended that applications mirror the preview for front-facing cameras, as it gives users a more natural experience, since it behaves more like a mirror
            return cameraEnclosureLocation.Panel == Panel.Front;
        }

        private SimpleOrientation GetCameraOrientationRelativeToNativeOrientation()
        {
            // Get the rotation angle of the camera enclosure as it is mounted in the device hardware
            var enclosureAngle = ConvertClockwiseDegreesToSimpleOrientation((int)cameraEnclosureLocation.RotationAngleInDegreesClockwise);

            // Account for the fact that, on portrait-first devices, the built in camera sensor is read at a 90 degree offset to the native orientation
            if (displayInformation.NativeOrientation == DisplayOrientations.Portrait && !IsEnclosureLocationExternal(cameraEnclosureLocation))
            {
                enclosureAngle = AddOrientations(SimpleOrientation.Rotated90DegreesCounterclockwise, enclosureAngle);
            }

            return enclosureAngle;
        }
    }
}