using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Media.MediaProperties;
using Windows.UI.Core;
#if WINDOWS_UWP
using Windows.UI.Xaml.Media.Imaging;
#else
using Microsoft.UI.Xaml.Media.Imaging;
#endif

namespace BuildIt.Media.Uno.WinUI
{
    public partial class CameraPreview
    {

        partial void PlatformInitCameraPreview()
        {
            imageElement.Source = new SoftwareBitmapSource();

            var cameraPreviewControl = this;
            cameraPreviewControl.StartPreviewFunc = PlatformStartPreviewFunc;
            cameraPreviewControl.StopPreviewFunc = PlatformStopPreviewFunc;
            cameraPreviewControl.SetFocusModeFunc = PlatformSetFocusModeFunc;
            cameraPreviewControl.TryFocusingFunc = PlatformTryFocusingFunc;
            cameraPreviewControl.CaptureNativeFrameToFileFunc = PlatformCapturePhotoToFile;
            cameraPreviewControl.RetrieveSupportedFocusModesFunc = PlatformRetrieveSupportedFocusModes;
            cameraPreviewControl.RetrieveCamerasFunc = PlatformRetrieveCamerasAsync;
        }

        private Task<IReadOnlyList<ICamera>> PlatformRetrieveCamerasAsync()
        {
            throw new NotImplementedException();
        }

        private IReadOnlyList<CameraFocusMode> PlatformRetrieveSupportedFocusModes()
        {
            throw new NotImplementedException();
        }

        private Task<string> PlatformCapturePhotoToFile(bool arg)
        {
            throw new NotImplementedException();
        }

        private Task<bool> PlatformTryFocusingFunc()
        {
            throw new NotImplementedException();
        }

        private Task PlatformSetFocusModeFunc()
        {
            throw new NotImplementedException();
        }

        private Task PlatformStopPreviewFunc(ICameraPreviewStopParameters arg)
        {
            throw new NotImplementedException();
        }

        private async Task PlatformStartPreviewFunc()
        {
            var frameSourceGroups = await MediaFrameSourceGroup.FindAllAsync();

            var selectedGroupObjects = frameSourceGroups.Select(group =>
   new
   {
       sourceGroup = group,
       colorSourceInfo = group.SourceInfos.FirstOrDefault((sourceInfo) =>
       {
           // On Xbox/Kinect, omit the MediaStreamType and EnclosureLocation tests
           return sourceInfo.MediaStreamType == MediaStreamType.VideoPreview
           && sourceInfo.SourceKind == MediaFrameSourceKind.Color
           && sourceInfo.DeviceInformation?.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front;
       })

   }).Where(t => t.colorSourceInfo != null)
   .FirstOrDefault();

            MediaFrameSourceGroup selectedGroup = selectedGroupObjects?.sourceGroup;
            MediaFrameSourceInfo colorSourceInfo = selectedGroupObjects?.colorSourceInfo;

            if (selectedGroup == null)
            {
                return;
            }


            mediaCapture = new MediaCapture();

            var settings = new MediaCaptureInitializationSettings()
            {
                SourceGroup = selectedGroup,
                SharingMode = MediaCaptureSharingMode.ExclusiveControl,
                MemoryPreference = MediaCaptureMemoryPreference.Cpu,
                StreamingCaptureMode = StreamingCaptureMode.Video
            };
            try
            {
                await mediaCapture.InitializeAsync(settings);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("MediaCapture initialization failed: " + ex.Message);
                return;
            }

            var colorFrameSource = mediaCapture.FrameSources[colorSourceInfo.Id];
            var preferredFormat = colorFrameSource.SupportedFormats.Where(format =>
            {
                return format.VideoFormat.Width >= 1080
                && format.Subtype == MediaEncodingSubtypes.Argb32;

            }).FirstOrDefault();

            preferredFormat = preferredFormat ?? colorFrameSource.SupportedFormats.FirstOrDefault();
            if (preferredFormat == null)
            {
                // Our desired format is not supported
                return;
            }

            await colorFrameSource.SetFormatAsync(preferredFormat);

            mediaFrameReader = await mediaCapture.CreateFrameReaderAsync(colorFrameSource, MediaEncodingSubtypes.Argb32);
            mediaFrameReader.FrameArrived += ColorFrameReader_FrameArrived;
            await mediaFrameReader.StartAsync();
        }

        private void ColorFrameReader_FrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            var mediaFrameReference = sender.TryAcquireLatestFrame();
            var videoMediaFrame = mediaFrameReference?.VideoMediaFrame;
            var softwareBitmap = videoMediaFrame?.SoftwareBitmap;

            if (softwareBitmap != null)
            {
                if (softwareBitmap.BitmapPixelFormat != Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8 ||
                    softwareBitmap.BitmapAlphaMode != Windows.Graphics.Imaging.BitmapAlphaMode.Premultiplied)
                {
                    softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                }

                // Swap the processed frame to _backBuffer and dispose of the unused image.
                softwareBitmap = Interlocked.Exchange(ref backBuffer, softwareBitmap);
                softwareBitmap?.Dispose();

                // Changes to XAML ImageElement must happen on UI thread through Dispatcher
                var task = imageElement
#if WINDOWS_UWP
                    .Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
#else
                    .DispatcherQueue.TryEnqueue(
#endif
                async () =>
                    {
                        try
                        {
                            // Don't let two copies of this task run at the same time.
                            if (taskRunning)
                            {
                                return;
                            }
                            taskRunning = true;

                            // Keep draining frames from the backbuffer until the backbuffer is empty.
                            SoftwareBitmap latestBitmap;
                            while ((latestBitmap = Interlocked.Exchange(ref backBuffer, null)) != null)
                            {
                                var imageSource = (SoftwareBitmapSource)imageElement.Source;
                                await imageSource.SetBitmapAsync(latestBitmap);
                                latestBitmap.Dispose();
                            }

                            taskRunning = false;
                        }
                        finally
                        {
                            mediaFrameReference.Dispose();
                        }
                    });
            }

            
        }

        MediaCapture mediaCapture;
        MediaFrameReader mediaFrameReader;
        private SoftwareBitmap backBuffer;
        private bool taskRunning = false;
    }
}
