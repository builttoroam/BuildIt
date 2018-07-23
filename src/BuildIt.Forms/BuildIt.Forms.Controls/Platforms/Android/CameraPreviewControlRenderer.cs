using Android.App;
using Android.Graphics;
using Android.Hardware;
using Android.Views;
using Android.Widget;
using BuildIt.Forms.Controls;
using BuildIt.Forms.Controls.Platforms.Android;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Context = Android.Content.Context;

[assembly: ExportRenderer(typeof(CameraPreviewControl), typeof(CameraPreviewControlRenderer))]
namespace BuildIt.Forms.Controls.Platforms.Android
{
    /// <summary>
    /// Custom Renderer for <see cref="CameraPreviewControl"/>
    /// </summary>
    public class CameraPreviewControlRenderer : FrameRenderer, TextureView.ISurfaceTextureListener
    {
        private global::Android.Hardware.Camera camera;
        private global::Android.Views.View view;

        private Activity activity;
        private CameraFacing cameraType;
        private TextureView textureView;
        private SurfaceTexture surfaceTexture;

        private CameraPreviewControl cameraPreviewControl;
        private String defaultFocusMode;

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

            if (cameraPreviewControl != null)
            {
                cameraPreviewControl.CaptureNativeFrameToFileDelegate = null;
            }

            cameraPreviewControl = cpc;
            cameraPreviewControl.CaptureNativeFrameToFileDelegate = CapturePhotoToFile;

            try
            {
                SetupUserInterface();
                SetupEventHandlers();
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
        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);


            if (e.PropertyName == CameraPreviewControl.PreferredCameraProperty.PropertyName)
            {
                SwitchCameraIfNecessary();
            }
            else if (e.PropertyName == CameraPreviewControl.EnableContinuousAutoFocusProperty.PropertyName)
            {
                await EnableContinuousAutoFocusAsync(cameraPreviewControl.EnableContinuousAutoFocus);
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

        private static CameraFacing ToCameraFacing(CameraPreviewControl.CameraPreference cameraPreference)
        {
            return cameraPreference == CameraPreviewControl.CameraPreference.Back
                ? CameraFacing.Back
                : CameraFacing.Front;
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

        private void SetupEventHandlers()
        {
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
            defaultFocusMode = cameraParameters.FocusMode;
            camera.StartPreview();
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task EnableContinuousAutoFocusAsync(bool enable)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
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
    }
}