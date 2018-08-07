using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware;
using Android.Views;
using Android.Widget;
using ApxLabs.FastAndroidCamera;
using BuildIt.Forms.Controls;
using BuildIt.Forms.Controls.Platforms.Android;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Context = Android.Content.Context;

[assembly: ExportRenderer(typeof(CameraPreviewControl), typeof(CameraPreviewControlRenderer))]
namespace BuildIt.Forms.Controls.Platforms.Android
{
    /// <summary>
    /// Custom Renderer for <see cref="CameraPreviewControl"/>
    /// </summary>
    public class CameraPreviewControlRenderer : FrameRenderer, TextureView.ISurfaceTextureListener, INonMarshalingPreviewCallback
    {
        private global::Android.Hardware.Camera camera;
        private global::Android.Views.View view;

        private Activity activity;
        private CameraFacing cameraType;
        private TextureView textureView;
        private SurfaceTexture surfaceTexture;

        private CameraPreviewControl cameraPreviewControl;
        private string defaultFocusMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraPreviewControlRenderer"/> class.
        /// </summary>
        /// <param name="context"><see cref="Context"/></param>
        public CameraPreviewControlRenderer(Context context)
            : base(context)
        {
        }

        /// <inheritdoc />
        public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
        {
            camera = global::Android.Hardware.Camera.Open((int)cameraType);
            textureView.LayoutParameters = new FrameLayout.LayoutParams(width, height);
            surfaceTexture = surface;

            camera.SetPreviewTexture(surface);
            PrepareAndStartCamera();
        }

        /// <inheritdoc />
        public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
        {
            camera.StopPreview();
            camera.Release();
            return true;
        }

        /// <inheritdoc />
        public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
        {
            PrepareAndStartCamera();
        }

        /// <inheritdoc />
        public void OnSurfaceTextureUpdated(SurfaceTexture surface)
        {
        }

        /// <inheritdoc />
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            var cpc = Element as CameraPreviewControl;
            if (cpc == null)
            {
                return;
            }

            cameraPreviewControl = cpc;
            cameraPreviewControl.CaptureNativeFrameToFileFunc = CapturePhotoToFile;
            cameraPreviewControl.RetrieveSupportedFocusModesFunc = RetrieveSupportedFocusModes;
            cameraPreviewControl.RetrieveCamerasFunc = RetrieveCamerasAsync;

            try
            {
                SetupUserInterface();
                AddView(view);
            }
            catch (Exception ex)
            {
                ex.LogError();
            }
        }

        /// <summary>
        /// Captures the most current video frame to a photo and saves it to local storage. Requires 'android.permission.WRITE_EXTERNAL_STORAGE' in manifest
        /// </summary>
        /// <param name="saveToPhotosLibrary">Whether or not to add the file to the device's photo library</param>
        /// <returns>The path to the saved photo file</returns>
        protected virtual async Task<string> CapturePhotoToFile(bool saveToPhotosLibrary)
        {
            camera.StopPreview();
            var image = textureView.Bitmap;

            try
            {
                var absolutePath = global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryDcim).AbsolutePath;
                var folderPath = System.IO.Path.Combine(absolutePath, "VideoCapture", DateTime.Now.ToString("yyyy-MM-dd"));
                var fileCount = 0;
                if (Directory.Exists(folderPath))
                {
                    fileCount = Directory.GetFiles(folderPath).Length;
                }
                else
                {
                    var directory = new Java.IO.File(folderPath);
                    directory.Mkdirs();
                }

                var filePath = System.IO.Path.Combine(folderPath, $"{fileCount}.jpg");
                var fileStream = new FileStream(filePath, FileMode.Create);
                await image.CompressAsync(Bitmap.CompressFormat.Jpeg, 50, fileStream);
                fileStream.Close();
                image.Recycle();

                if (saveToPhotosLibrary)
                {
                    // Broadcasting the the file's Uri in the following intent will allow any Photo Gallery apps to incorporate
                    // the photo file into their collection -RR
                    var intent = new Intent(Intent.ActionMediaScannerScanFile);
                    var file = new Java.IO.File(filePath);
                    var uri = global::Android.Net.Uri.FromFile(file);
                    intent.SetData(uri);
                    activity.SendBroadcast(intent);
                }

                return filePath;
            }
            catch (Exception ex)
            {
                ex.LogError();
                return ex.ToString();
            }
            finally
            {
                camera.StartPreview();
            }
        }

        /// <inheritdoc />
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == CameraPreviewControl.PreferredCameraProperty.PropertyName)
            {
                SwitchCameraIfNecessary();
            }
            else if (e.PropertyName == CameraPreviewControl.EnableContinuousAutoFocusProperty.PropertyName)
            {
                EnableContinuousAutoFocus(cameraPreviewControl.EnableContinuousAutoFocus);
            }
        }

        /// <inheritdoc />
        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            var msw = MeasureSpec.MakeMeasureSpec(r - l, MeasureSpecMode.Exactly);
            var msh = MeasureSpec.MakeMeasureSpec(b - t, MeasureSpecMode.Exactly);

            view.Measure(msw, msh);
            view.Layout(0, 0, r - l, b - t);
        }

        private static CameraFacing ToCameraFacing(CameraPreviewControl.CameraFacing cameraFacing)
        {
            return cameraFacing == CameraPreviewControl.CameraFacing.Back
                ? CameraFacing.Back
                : CameraFacing.Front;
        }

        private static CameraPreviewControl.CameraFacing ToCameraFacing(CameraFacing cameraFacing)
        {
            return cameraFacing == CameraFacing.Back ? CameraPreviewControl.CameraFacing.Back : CameraPreviewControl.CameraFacing.Front;
        }

        private void SwitchCameraIfNecessary()
        {
            var newCameraType = ToCameraFacing(cameraPreviewControl.PreferredCamera);
            if (newCameraType == cameraType)
            {
                return;
            }

            cameraType = newCameraType;

            camera.StopPreview();
            camera.Release();
            camera = global::Android.Hardware.Camera.Open((int)cameraType);
            camera.SetPreviewTexture(surfaceTexture);
            PrepareAndStartCamera();
        }

        private void SetupUserInterface()
        {
            activity = Context as Activity;
            if (activity == null)
            {
                return;
            }

            view = activity.LayoutInflater.Inflate(Resource.Layout.CameraLayout, this, false);
            cameraType = ToCameraFacing(cameraPreviewControl.PreferredCamera);

            textureView = view.FindViewById<TextureView>(Resource.Id.textureView);
            textureView.SurfaceTextureListener = this;
        }

        private void PrepareAndStartCamera()
        {
            camera.StopPreview();

            var display = activity.WindowManager.DefaultDisplay;
            if (display.Rotation == SurfaceOrientation.Rotation0)
            {
                camera.SetDisplayOrientation(90);
            }

            if (display.Rotation == SurfaceOrientation.Rotation270)
            {
                camera.SetDisplayOrientation(180);
            }

            var cameraParameters = camera.GetParameters();
            int numBytes = (cameraParameters.PreviewSize.Width * cameraParameters.PreviewSize.Height * ImageFormat.GetBitsPerPixel(cameraParameters.PreviewFormat)) / 8;

            using (FastJavaByteArray buffer = new FastJavaByteArray(numBytes))
            {
                // allocate new Java byte arrays for Android to use for preview frames
                camera.AddCallbackBuffer(new FastJavaByteArray(numBytes));
            }

            defaultFocusMode = cameraParameters.FocusMode;
            camera.SetNonMarshalingPreviewCallback(this);
            camera.StartPreview();

            // non-marshaling version of the preview callback
        }

        private void EnableContinuousAutoFocus(bool enable)
        {
            var cameraParameters = camera.GetParameters();
            if (enable)
            {
                if (cameraParameters.SupportedFocusModes != null && cameraParameters.SupportedFocusModes.Contains(global::Android.Hardware.Camera.Parameters.FocusModeContinuousPicture))
                {
                    cameraParameters.FocusMode = global::Android.Hardware.Camera.Parameters.FocusModeContinuousPicture;
                    camera.SetParameters(cameraParameters);
                }
            }
            else
            {
                cameraParameters.FocusMode = defaultFocusMode;
                camera.SetParameters(cameraParameters);
            }
        }

        private IReadOnlyList<CameraFocusMode> RetrieveSupportedFocusModes()
        {
            var supportedFocusModes = new List<CameraFocusMode>();
            var cameraParameters = camera.GetParameters();
            if (cameraParameters != null)
            {
                foreach (var supportedFocusMode in cameraParameters.SupportedFocusModes)
                {
                    switch (supportedFocusMode)
                    {
                        case global::Android.Hardware.Camera.Parameters.FocusModeAuto:
                            supportedFocusModes.Add(CameraFocusMode.Auto);
                            break;

                        case global::Android.Hardware.Camera.Parameters.FocusModeContinuousPicture:
                            supportedFocusModes.Add(CameraFocusMode.Continuous);
                            break;
                    }
                }
            }

            // manual focus isn't a mode exposed by Android with the old Camera API but achieved through
            // various method calls so should work
            supportedFocusModes.Add(CameraFocusMode.Manual);
            return supportedFocusModes;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<IReadOnlyList<ICamera>> RetrieveCamerasAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var cameras = new List<ICamera>();
            for (var i = 0; i < global::Android.Hardware.Camera.NumberOfCameras; i++)
            {
                var cameraInfo = new global::Android.Hardware.Camera.CameraInfo();
                global::Android.Hardware.Camera.GetCameraInfo(i, cameraInfo);
                var camera = new Camera()
                {
                    Id = i.ToString(),
                    CameraFacing = ToCameraFacing(cameraInfo.Facing)
                };
                cameras.Add(camera);
            }

            return cameras.AsReadOnly();
        }

        private byte[] ConvertYuvToJpeg(IList<byte> yuvData, global::Android.Hardware.Camera camera)
        {
            // conversion code may be needed to work with the ML library
            var cameraParameters = camera.GetParameters();
            var width = cameraParameters.PreviewSize.Width;
            var height = cameraParameters.PreviewSize.Height;
            using (var yuv = new YuvImage(yuvData.ToArray(), cameraParameters.PreviewFormat, width, height, null))
            {
                var ms = new MemoryStream();
                var quality = 100;   // adjust this as needed
                yuv.CompressToJpeg(new Rect(0, 0, width, height), quality, ms);
                var jpegData = ms.ToArray();
                return jpegData;
            }
        }

        public async void OnPreviewFrame(IntPtr data, global::Android.Hardware.Camera camera)
        {
            using (var buffer = new FastJavaByteArray(data))
            {
                // TODO: see if this can be optimised
                var jpeg = ConvertYuvToJpeg(buffer, camera);
                await cameraPreviewControl.OnMediaFrameArrived(new MediaFrame(jpeg));

                // finished with this frame, process the next one
                camera.AddCallbackBuffer(buffer);
            }
        }
    }
}