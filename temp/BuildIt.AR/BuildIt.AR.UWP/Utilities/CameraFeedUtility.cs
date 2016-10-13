using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Panel = Windows.Devices.Enumeration.Panel;

namespace BuildIt.AR.UWP.Utilities
{
    public class CameraFeedUtility
    {
        private MediaCapture mediaCapture;
        private bool isPreviewing;
        DisplayRequest displayRequest = new DisplayRequest();
        private readonly CaptureElement captureElement;
        private readonly CoreDispatcher dispatcher;

        public CameraFeedUtility(CaptureElement captureElement, CoreDispatcher dispatcher)
        {
            this.captureElement = captureElement;
            this.dispatcher = dispatcher;
        }

        private async Task StartPreviewAsync(VideoRotation videoRotation)
        {
            try
            {
                if (mediaCapture == null)
                {
                    var cameraDevice = await FindCameraDeviceByPanelAsync(Panel.Back);
                    mediaCapture = new MediaCapture();
                    var settings = new MediaCaptureInitializationSettings { VideoDeviceId = cameraDevice.Id };
                    await mediaCapture.InitializeAsync(settings);
                    captureElement.Source = mediaCapture;
                    await mediaCapture.StartPreviewAsync();
                    isPreviewing = true;
                    mediaCapture.SetPreviewRotation(videoRotation);
                    displayRequest.RequestActive();
                }
                //DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
            }
            catch (UnauthorizedAccessException)
            {
                // This will be thrown if the user denied access to the camera in privacy settings
                Debug.WriteLine("The app was denied access to the camera");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MediaCapture initialization failed. {0}", ex.Message);
            }
        }

        /// <summary>
        /// Queries the available video capture devices to try and find one mounted on the desired panel
        /// </summary>
        /// <param name="desiredPanel">The panel on the device that the desired camera is mounted on</param>
        /// <returns>A DeviceInformation instance with a reference to the camera mounted on the desired panel if available,
        ///          any other camera if not, or null if no camera is available.</returns>
        private static async Task<DeviceInformation> FindCameraDeviceByPanelAsync(Panel desiredPanel)
        {
            // Get available devices for capturing pictures
            var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            // Get the desired camera by panel
            DeviceInformation desiredDevice = allVideoDevices.FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == desiredPanel);

            // If there is no device mounted on the desired panel, return the first device found
            return desiredDevice ?? allVideoDevices.FirstOrDefault();
        }

        private async Task CleanupCameraAsync()
        {
            if (mediaCapture != null)
            {
                isPreviewing = false;
                await mediaCapture.StopPreviewAsync();

                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    captureElement.Source = null;
                    displayRequest?.RequestRelease();
                });

                mediaCapture.Dispose();
                mediaCapture = null;
            }
        }
    }
}
