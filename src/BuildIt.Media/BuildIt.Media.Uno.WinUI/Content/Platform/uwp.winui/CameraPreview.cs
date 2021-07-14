using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Media.Devices;
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
            nativePreviewElement.Source = new SoftwareBitmapSource();

            var cameraPreviewControl = this;
            cameraPreviewControl.StartPreviewFunc = PlatformStartPreviewFunc;
            cameraPreviewControl.StopPreviewFunc = PlatformStopPreviewFunc;
            cameraPreviewControl.SetFocusModeFunc = PlatformSetFocusModeFunc;
            cameraPreviewControl.TryFocusingFunc = PlatformTryFocusingFunc;
            cameraPreviewControl.CaptureNativeFrameToFileFunc = PlatformCapturePhotoToFile;
            cameraPreviewControl.RetrieveSupportedFocusModesFunc = PlatformRetrieveSupportedFocusModes;
            cameraPreviewControl.RetrieveCamerasFunc = PlatformRetrieveCamerasAsync;
            cameraPreviewControl.SetStretch = PlatformSetStretchFunc;
        }

        private Task<IReadOnlyList<ICamera>> PlatformRetrieveCamerasAsync()
        {
            throw new NotImplementedException();
        }

        private IReadOnlyList<FocusMode> PlatformRetrieveSupportedFocusModes()
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
        private async Task PlatformSetFocusModeFunc()
        {
            var videoController = mediaCapture?.VideoDeviceController;
            var focus = videoController?.FocusControl;
            if (!(focus?.Supported ?? false)) return;

            var modes = focus.SupportedFocusModes.ToArray();
            var dists = focus.SupportedFocusDistances.ToArray();
            var ranges = focus.SupportedFocusRanges.ToArray();
            focus.Configure(new FocusSettings { Mode = FocusMode, Value = 100, DisableDriverFallback = true });
            await focus.FocusAsync();
        }

        private async Task PlatformSetStretchFunc()
        {
            nativePreviewElement.Stretch = this.Stretch;
        }

        private async Task PlatformStopPreviewFunc(ICameraPreviewStopParameters arg)
        {
            await StopReaderAsync();
        }

        private SemaphoreSlim PreviewLock { get; } = new SemaphoreSlim(1);
        private async Task<CameraResult> PlatformStartPreviewFunc()
        {
            await PreviewLock.WaitAsync();
            var cleanUp = true;
            try
            {
                var frameSourceGroups = await GetFrameSourceGroupsAsync();

                var selectedGroupObjects = frameSourceGroups.FirstOrDefault();

                mediaCapture = new MediaCapture();

                var settings = new MediaCaptureInitializationSettings()
                {
                    SourceGroup = selectedGroupObjects,
                    MemoryPreference = MediaCaptureMemoryPreference.Cpu,
                    StreamingCaptureMode = StreamingCaptureMode.Video,
                };

                await mediaCapture.InitializeAsync(settings);

                // Find the first video preview or record stream available
                var PreviewFrameSource = mediaCapture.FrameSources.FirstOrDefault(source => source.Value.Info.MediaStreamType == MediaStreamType.VideoPreview
                                                                                      && source.Value.Info.SourceKind == MediaFrameSourceKind.Color).Value;
                if (PreviewFrameSource == null)
                {
                    PreviewFrameSource = mediaCapture.FrameSources.FirstOrDefault(source => source.Value.Info.MediaStreamType == MediaStreamType.VideoRecord
                                                                                          && source.Value.Info.SourceKind == MediaFrameSourceKind.Color).Value;
                }

                if (PreviewFrameSource == null)
                {
                    return CameraResult.NoFrameSourceAvailable;
                }

                // Get only formats of a certain frame-rate and compatible subtype for previewing, order them by resolution
                var FrameFormatsAvailable = PreviewFrameSource.SupportedFormats.Where(format =>
                    format.FrameRate.Numerator / format.FrameRate.Denominator >= 15 // fps
                    && (string.Compare(format.Subtype, MediaEncodingSubtypes.Nv12, true) == 0
                        || string.Compare(format.Subtype, MediaEncodingSubtypes.Bgra8, true) == 0
                        || string.Compare(format.Subtype, MediaEncodingSubtypes.Yuy2, true) == 0
                        || string.Compare(format.Subtype, MediaEncodingSubtypes.Rgb32, true) == 0))?.OrderBy(format => format.VideoFormat.Width * format.VideoFormat.Height).ToList();

                if (FrameFormatsAvailable == null || !FrameFormatsAvailable.Any())
                {
                    return CameraResult.NoCompatibleFrameFormatAvailable;
                }

                // Set the format with the highest resolution available by default
                var defaultFormat = FrameFormatsAvailable.Last();
                await PreviewFrameSource.SetFormatAsync(defaultFormat);

                mediaCapture.Failed += MediaCapture_Failed;


                if (PreviewFrameSource != null)
                {
                    _frameReader = await mediaCapture.CreateFrameReaderAsync(PreviewFrameSource);
                    _frameReader.AcquisitionMode = MediaFrameReaderAcquisitionMode.Realtime;

                    _frameReader.FrameArrived += ColorFrameReader_FrameArrived;

                    if (_frameReader == null)
                    {
                        return CameraResult.CreateFrameReaderFailed;
                    }
                    else
                    {
                        MediaFrameReaderStartStatus statusResult = await _frameReader.StartAsync();
                        if (statusResult != MediaFrameReaderStartStatus.Success)
                        {
                            return CameraResult.StartFrameReaderFailed;
                        }
                    }
                }

                // If we get to here then everything is working, so we don't need to clean up
                cleanUp = false;
            }
            catch (UnauthorizedAccessException)
            {
                return CameraResult.CameraAccessDenied;
            }
            catch (Exception)
            {
                return CameraResult.InitializationFailed_UnknownError;
            }
            finally
            {
                PreviewLock.Release();
                if (cleanUp)
                {
                    await StopReaderAsync();
                }
            }

            return CameraResult.Success;
        }

        private async Task StopReaderAsync()
        {
            await PreviewLock.WaitAsync();
            try
            {
                if (_frameReader != null)
                {
                    _frameReader.FrameArrived -= ColorFrameReader_FrameArrived;
                    await _frameReader.StopAsync();
                    _frameReader.Dispose();
                    _frameReader = null;
                }

                if (mediaCapture != null)
                {
                    mediaCapture.Dispose();
                    mediaCapture = null;
                }
            }
            finally
            {
                PreviewLock.Release();
            }
        }

        private void MediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
        }

        public static async Task<IReadOnlyList<MediaFrameSourceGroup>> GetFrameSourceGroupsAsync()
        {
            var videoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            var groups = await MediaFrameSourceGroup.FindAllAsync();

            // Filter out color video preview and video record type sources and remove duplicates video devices.
            return groups.Where(g => g.SourceInfos.Any(s => s.SourceKind == MediaFrameSourceKind.Color &&
                                                                        (s.MediaStreamType == MediaStreamType.VideoPreview || s.MediaStreamType == MediaStreamType.VideoRecord))
                                                                        && g.SourceInfos.All(sourceInfo => videoDevices.Any(vd => vd.Id == sourceInfo.DeviceInformation.Id))).ToList();
        }

        private void ColorFrameReader_FrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {

            ProcessFrame(sender);
            ProcessBuffer();
        }

        private int bufferLock = 0;
        private void ProcessBuffer()
        {
            if (Interlocked.CompareExchange(ref bufferLock, 1, 0) != 0)
            {
                return;
            }

            // Changes to XAML nativePreviewElement must happen on UI thread through Dispatcher
            var task = nativePreviewElement
#if WINDOWS_UWP
                    .Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
#else
                    .DispatcherQueue.TryEnqueue(
#endif
                            async () =>
                            {
                                try
                                {

                                    // Keep draining frames from the backbuffer until the backbuffer is empty.
                                    FrameBitmap latestBuffer;
                                    while ((latestBuffer = Interlocked.Exchange(ref backBuffer, null)) != null)
                                    {
                                        var imageSource = (SoftwareBitmapSource)nativePreviewElement.Source;
                                        await imageSource.SetBitmapAsync(latestBuffer.Bitmap);
                                        latestBuffer.Dispose();
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                                finally
                                {
                                    Interlocked.Exchange(ref bufferLock, 0);
                                }
                            });
#if !WINDOWS_UWP
            if (!task)
            {
                Interlocked.Exchange(ref bufferLock, 0);
            }
#endif
        }

        private int frameLock = 0;
        private void ProcessFrame(MediaFrameReader sender)
        {
            // Make sure we're only processing one frame at a time
            // If we're mid processing, we're just going to dump the frame
            if (Interlocked.CompareExchange(ref frameLock, 1, 0) != 0)
            {
                return;
            }

            // Try block is here to make sure we release the frameLock
            try
            {
                // Try to acquire the latest frame - if no frame, just return
                var mediaFrameReference = sender.TryAcquireLatestFrame();
                if (mediaFrameReference is null)
                {
                    // Return before we go into the try block, since we don't want
                    // to try to dispose null frame reference
                    return;
                }

                var softwareBitmap = mediaFrameReference.VideoMediaFrame?.SoftwareBitmap;

                if (softwareBitmap != null)
                {
                    if (softwareBitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 ||
                        softwareBitmap.BitmapAlphaMode != BitmapAlphaMode.Premultiplied)
                    {
                        var oldBmp = softwareBitmap;
                        softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                        // Make sure we dispose of the old software bitmap to free up resources immediately
                        oldBmp.Dispose();
                    }

                    // Push the new frame/bitmap pair onto the back buffer
                    // Any frame still in the buffer will be returned so it can be disposed
                    var buffer = new FrameBitmap(mediaFrameReference, softwareBitmap);
                    var oldBitmap = Interlocked.Exchange(ref backBuffer, buffer);
                    oldBitmap?.Dispose();
                }
                else
                {
                    // No software bitmap to process, so just dispose the frame
                    mediaFrameReference.Dispose();
                }
            }
            finally
            {
                Interlocked.Exchange(ref frameLock, 0);
            }
        }

        //        private void ColorFrameReader_FrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        //        {

        //            var isProcessing = false;
        //            if (Interlocked.CompareExchange(ref frameLock, 1, 0) != 0)
        //            {
        //                return;
        //            }
        //            try
        //            {

        //                var mediaFrameReference = sender.TryAcquireLatestFrame();

        //                try
        //                {

        //                    var videoMediaFrame = mediaFrameReference?.VideoMediaFrame;
        //                    var softwareBitmap = videoMediaFrame?.SoftwareBitmap;


        //                    if (softwareBitmap != null)
        //                    {
        //                        if (softwareBitmap.BitmapPixelFormat != Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8 ||
        //                            softwareBitmap.BitmapAlphaMode != Windows.Graphics.Imaging.BitmapAlphaMode.Premultiplied)
        //                        {
        //                            var oldBmp = softwareBitmap;
        //                            softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
        //                            oldBmp.Dispose();
        //                        }

        //                        //// Swap the processed frame to _backBuffer and dispose of the unused image.
        //                        //softwareBitmap = Interlocked.Exchange(ref backBuffer, softwareBitmap);
        //                        //softwareBitmap?.Dispose();

        //                        isProcessing = true;
        //                        // Changes to XAML nativePreviewElement must happen on UI thread through Dispatcher
        //                        var task = nativePreviewElement
        //#if WINDOWS_UWP
        //                    .Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
        //#else
        //                    .DispatcherQueue.TryEnqueue(
        //#endif
        //                            async () =>
        //                            {
        //                                try
        //                                {

        //                                    // Keep draining frames from the backbuffer until the backbuffer is empty.
        //                                    SoftwareBitmap latestBitmap = softwareBitmap;
        //                                    //while ((latestBitmap = Interlocked.Exchange(ref backBuffer, null)) != null)
        //                                    //{
        //                                        var imageSource = (SoftwareBitmapSource)nativePreviewElement.Source;
        //                                        await imageSource.SetBitmapAsync(latestBitmap);
        //                                        latestBitmap.Dispose();
        //                                    //}
        //                                }
        //                                catch (Exception ex)
        //                                {
        //                                }
        //                                finally
        //                                {
        //                                    mediaFrameReference?.Dispose();
        //                                    Interlocked.Exchange(ref frameLock, 0);
        //                                }
        //                            });
        //                    }
        //                }
        //                finally
        //                {

        //                    if (!isProcessing)
        //                    {
        //                        mediaFrameReference?.Dispose();

        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //            }
        //            finally
        //            {

        //                if (!isProcessing)
        //                {
        //                    Interlocked.Exchange(ref frameLock, 0);
        //                }
        //            }

        //        }

        MediaCapture mediaCapture;
        MediaFrameReader mediaFrameReader;
        private FrameBitmap backBuffer;
        private bool taskRunning = false;
        private MediaFrameReader _frameReader;

        private record FrameBitmap(MediaFrameReference Frame, SoftwareBitmap Bitmap) : IDisposable
        {
            public void Dispose()
            {
                Frame?.Dispose();
                Bitmap?.Dispose();
            }
        }
    }
}

namespace System.Runtime.CompilerServices
{
    public class IsExternalInit { }
}
