using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware;
using Android.Hardware.Camera2;
using Android.Hardware.Camera2.Params;
using Android.Media;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using ApxLabs.FastAndroidCamera;
using BuildIt.Forms.Controls;
using BuildIt.Forms.Controls.Platforms.Android;
using Java.Lang;
using Java.Util;
using Java.Util.Concurrent;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using static Android.App.Application;
using Context = Android.Content.Context;

[assembly: ExportRenderer(typeof(CameraPreviewControl), typeof(CameraPreviewControlRenderer))]
namespace BuildIt.Forms.Controls.Platforms.Android
{
    /// <summary>
    /// Custom Renderer for <see cref="CameraPreviewControl"/>
    /// </summary>
    public class CameraPreviewControlRenderer : FrameRenderer, TextureView.ISurfaceTextureListener, INonMarshalingPreviewCallback, IActivityLifecycleCallbacks
    {
        // Camera state: Showing camera preview.
        internal const int StatePreview = 0;

        // Camera state: Waiting for the focus to be locked.
        internal const int StateWaitingLock = 1;

        // Camera state: Waiting for the exposure to be precapture state.
        internal const int StateWaitingPrecapture = 2;

        // Camera state: Waiting for the exposure state to be something other than precapture.
        internal const int StateWaitingNonPrecapture = 3;

        // Camera state: Picture was taken.
        internal const int StatePictureTaken = 4;

        private const string ImageCapture = "ImageCapture";

        // Max preview height that is guaranteed by Camera2 API
        private static readonly int MaxPreviewHeight = 1080;

        // Max preview width that is guaranteed by Camera2 API
        private static readonly int MaxPreviewWidth = 1920;

        private static readonly SparseIntArray Orientations = new SparseIntArray();

        // An additional thread for running tasks that shouldn't block the UI.
        private HandlerThread backgroundThread;

        private global::Android.Hardware.Camera camera;

        private string cameraId;
        private CameraPreviewControl cameraPreviewControl;
        private CameraFacing cameraType;
        private CameraFocusMode defaultFocusMode;
        private bool flashSupported;
        private ImageAvailableListener imageAvailableListener;
        private ImageReader imageReader;
        private LensFacing lensFacing;

        // The size of the camera preview
        private global::Android.Util.Size previewSize;
        private int sensorOrientation;

        // CameraDevice.StateListener is called when a CameraDevice changes its state
        private CameraStateListener stateCallback;
        private CaptureRequest.Builder stillCaptureBuilder;
        private SurfaceTexture surfaceTexture;
        private Camera2BasicSurfaceTextureListener surfaceTextureListener;
        private AutoFitTextureView textureView;
        private bool useCamera2Api;
        private global::Android.Views.View view;
        int textureWidth;
        int textureHeight;

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraPreviewControlRenderer"/> class.
        /// </summary>
        /// <param name="context"><see cref="Context"/></param>
        public CameraPreviewControlRenderer(Context context)
            : base(context)
        {
        }

        // A handler for running tasks in the background.
        public Handler BackgroundHandler { get; set; }

        // A reference to the opened CameraDevice
        public CameraDevice CameraDevice { get; set; }

        public Semaphore CameraOpenCloseLock { get; } = new Semaphore(1);

        public CameraCaptureListener CaptureCallback { get; private set; }

        // A capture session for camera preview.
        public CameraCaptureSession CaptureSession { get; set; }

        public CaptureRequest PreviewRequest { get; internal set; }

        public CaptureRequest.Builder PreviewRequestBuilder { get; private set; }

        public int State { get; set; } = StatePreview;

        internal Activity Activity => Context as Activity;

        public void CaptureStillPicture()
        {
            try
            {
                if (Activity == null || CameraDevice == null)
                {
                    return;
                }

                // This is the CaptureRequest.Builder that we use to take a picture.
                if (stillCaptureBuilder == null)
                {
                    stillCaptureBuilder = CameraDevice.CreateCaptureRequest(CameraTemplate.StillCapture);
                }

                stillCaptureBuilder.AddTarget(imageReader.Surface);

                // Use the same AE and AF modes as the preview.
                stillCaptureBuilder.Set(CaptureRequest.ControlAfMode, (int)ControlAFMode.ContinuousPicture);
                SetAutoFlash(stillCaptureBuilder);

                // Orientation
                int rotation = (int)Activity.WindowManager.DefaultDisplay.Rotation;
                stillCaptureBuilder.Set(CaptureRequest.JpegOrientation, GetOrientation(rotation));

                CaptureSession.StopRepeating();
                CaptureSession.Capture(stillCaptureBuilder.Build(), new CameraCaptureStillPictureSessionCallback(this), null);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        public void CreateCameraPreviewSession()
        {
            try
            {
                SurfaceTexture texture = textureView.SurfaceTexture;
                if (texture == null)
                {
                    throw new IllegalStateException("texture is null");
                }

                // We configure the size of default buffer to be the size of camera preview we want.
                texture.SetDefaultBufferSize(previewSize.Width, previewSize.Height);

                // This is the output Surface we need to start preview.
                Surface surface = new Surface(texture);

                // We set up a CaptureRequest.Builder with the output Surface.
                PreviewRequestBuilder = CameraDevice.CreateCaptureRequest(CameraTemplate.Preview);
                PreviewRequestBuilder.AddTarget(surface);
                PreviewRequestBuilder.AddTarget(imageReader.Surface);

                // Need this here as add the image reader surface would affect the orientation of jpeg capture
                // TODO: handle orientation changes
                int rotation = (int)Activity.WindowManager.DefaultDisplay.Rotation;
                PreviewRequestBuilder.Set(CaptureRequest.JpegOrientation, GetOrientation(rotation));

                // Here, we create a CameraCaptureSession for camera preview.
                var surfaces = new List<Surface>();
                surfaces.Add(surface);
                surfaces.Add(imageReader.Surface);
                CameraDevice.CreateCaptureSession(surfaces, new CameraCaptureSessionCallback(this), null);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
        }

        public void OnActivityDestroyed(Activity activity)
        {
        }

        public void OnActivityPaused(Activity activity)
        {
            if (useCamera2Api)
            {
                CloseCamera();
                StopBackgroundThread();
            }
        }

        public void OnActivityResumed(Activity activity)
        {
            if (!useCamera2Api)
            {
                return;
            }

            StartBackgroundThread();
            if (textureView == null)
            {
                return;
            }

            // When the screen is turned off and turned back on, the SurfaceTexture is already
            // available, and "onSurfaceTextureAvailable" will not be called. In that case, we can open
            // a camera and start preview from here (otherwise, we wait until the surface is ready in
            // the SurfaceTextureListener).
            if (textureView.IsAvailable)
            {
                OpenCamera(textureView.Width, textureView.Height);
            }
            else
            {
                textureView.SurfaceTextureListener = surfaceTextureListener;
            }
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
        }

        public void OnActivityStarted(Activity activity)
        {
        }

        public void OnActivityStopped(Activity activity)
        {
        }

        public async void OnPreviewFrame(IntPtr data, global::Android.Hardware.Camera camera)
        {
            using (var buffer = new FastJavaByteArray(data))
            {
                // TODO: see if this can be optimised
                var jpegBytes = ConvertYuvToJpeg(buffer, camera);
                await cameraPreviewControl.OnMediaFrameArrived(new MediaFrame(jpegBytes));

                // finished with this frame, process the next one
                camera.AddCallbackBuffer(buffer);
            }
        }

        /// <inheritdoc />
        public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
        {
            textureWidth = width;
            textureHeight = height;
            if (useCamera2Api)
            {
                OpenCamera(width, height);
                return;
            }

            camera = global::Android.Hardware.Camera.Open((int)cameraType);
            ApplyAspect();

            surfaceTexture = surface;

            camera.SetPreviewTexture(surface);
            PrepareAndStartCamera();
        }

        private void ApplyAspect()
        {
            var cameraParameters = camera.GetParameters();
            var previewWidth = 0;
            var previewHeight = 0;
            var orientation = Resources.Configuration.Orientation;
            if (orientation == global::Android.Content.Res.Orientation.Landscape)
            {
                previewWidth = cameraParameters.PreviewSize.Width;
                previewHeight = cameraParameters.PreviewSize.Height;
            }
            else
            {
                previewWidth = cameraParameters.PreviewSize.Height;
                previewHeight = cameraParameters.PreviewSize.Width;
            }

            switch (cameraPreviewControl.Aspect)
            {
                case Aspect.AspectFit:
                    if (textureWidth < (float)textureHeight * previewWidth / (float)previewHeight)
                    {
                        textureView.LayoutParameters = new FrameLayout.LayoutParams(textureWidth, textureWidth * previewHeight / previewWidth);
                    }
                    else
                    {
                        textureView.LayoutParameters = new FrameLayout.LayoutParams(textureHeight * previewWidth / previewHeight, textureHeight);
                    }

                    break;

                case Aspect.AspectFill:
                    var previewAspectRatio = (double)previewWidth / previewHeight;
                    if (previewAspectRatio > 1)
                    {
                        // width > height, so keep the height but scale the width to meet aspect ratio
                        textureView.LayoutParameters = new FrameLayout.LayoutParams((int)(textureWidth * previewAspectRatio), textureHeight);
                    }
                    else if (previewAspectRatio < 1 && previewAspectRatio != 0)
                    {
                        // width < height
                        textureView.LayoutParameters = new FrameLayout.LayoutParams(textureWidth, (int)(textureHeight / previewAspectRatio));
                    }
                    else
                    {
                        // width == height
                        textureView.LayoutParameters = new FrameLayout.LayoutParams(textureWidth, textureHeight);
                    }

                    break;

                case Aspect.Fill:
                    textureView.LayoutParameters = new FrameLayout.LayoutParams(textureWidth, textureHeight);
                    break;
            }
        }

        /// <inheritdoc />
        public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
        {
            if (!useCamera2Api)
            {
                camera.StopPreview();
                camera.Release();
            }

            return true;
        }

        /// <inheritdoc />
        public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
        {
            if (!useCamera2Api)
            {
                PrepareAndStartCamera();
            }
        }

        /// <inheritdoc />
        public void OnSurfaceTextureUpdated(SurfaceTexture surface)
        {
        }

        // Run the precapture sequence for capturing a still image. This method should be called when
        // we get a response in {@link #mCaptureCallback} from {@link #lockFocus()}.
        public void RunPrecaptureSequence()
        {
            try
            {
                // This is how to tell the camera to trigger.
                PreviewRequestBuilder.Set(CaptureRequest.ControlAePrecaptureTrigger, (int)ControlAEPrecaptureTrigger.Start);

                // Tell #mCaptureCallback to wait for the precapture sequence to be set.
                State = StateWaitingPrecapture;
                CaptureSession.Capture(PreviewRequestBuilder.Build(), CaptureCallback, BackgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        public void SetAutoFlash(CaptureRequest.Builder requestBuilder)
        {
            if (flashSupported)
            {
                requestBuilder.Set(CaptureRequest.ControlAeMode, (int)ControlAEMode.OnAutoFlash);
            }
        }

        public void UnlockFocus()
        {
            try
            {
                // Reset the auto-focus trigger
                PreviewRequestBuilder.Set(CaptureRequest.ControlAfTrigger, (int)ControlAFTrigger.Cancel);
                SetAutoFlash(PreviewRequestBuilder);
                CaptureSession.Capture(PreviewRequestBuilder.Build(), CaptureCallback, BackgroundHandler);

                // After this, the camera will go back to the normal state of preview.
                State = StatePreview;
                CaptureSession.SetRepeatingRequest(PreviewRequest, CaptureCallback, BackgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        internal void ConfigureTransform(int viewWidth, int viewHeight)
        {
            if (textureView == null || previewSize == null || Activity == null)
            {
                return;
            }

            var rotation = (int)Activity.WindowManager.DefaultDisplay.Rotation;
            Matrix matrix = new Matrix();
            RectF viewRect = new RectF(0, 0, viewWidth, viewHeight);
            RectF bufferRect = new RectF(0, 0, previewSize.Height, previewSize.Width);
            float centerX = viewRect.CenterX();
            float centerY = viewRect.CenterY();
            if (rotation == (int)SurfaceOrientation.Rotation90 || rotation == (int)SurfaceOrientation.Rotation270)
            {
                bufferRect.Offset(centerX - bufferRect.CenterX(), centerY - bufferRect.CenterY());
                matrix.SetRectToRect(viewRect, bufferRect, Matrix.ScaleToFit.Fill);
                float scale = System.Math.Max((float)viewHeight / previewSize.Height, (float)viewWidth / previewSize.Width);
                matrix.PostScale(scale, scale, centerX, centerY);
                matrix.PostRotate(90 * (rotation - 2), centerX, centerY);
            }
            else if (rotation == (int)SurfaceOrientation.Rotation180)
            {
                matrix.PostRotate(180, centerX, centerY);
            }

            textureView.SetTransform(matrix);
        }

        internal async Task OnMediaFrameArrived(byte[] jpegBytes)
        {
            await cameraPreviewControl.OnMediaFrameArrived(new MediaFrame(jpegBytes));
        }

        internal void OpenCamera(int width, int height)
        {
            CameraDevice?.Close();
            CameraDevice = null;
            SetUpCameraOutputs(width, height);
            ConfigureTransform(width, height);
            var errorOccurred = false;
            var manager = (CameraManager)Context.GetSystemService(Context.CameraService);
            try
            {
                if (!CameraOpenCloseLock.TryAcquire(2500, TimeUnit.Milliseconds))
                {
                    cameraPreviewControl.RaiseErrorOpeningCamera();
                    return;
                }

                manager.OpenCamera(cameraId, stateCallback, BackgroundHandler);
            }
            catch (CameraAccessException e)
            {
                // in case camera is disabled, disconnected or already in use
                errorOccurred = true;
                e.PrintStackTrace();
            }
            catch (InterruptedException e)
            {
                // in case an interruption occurred while trying to lock the camera
                e.PrintStackTrace();
                errorOccurred = true;
            }
            catch (Java.Lang.Exception e)
            {
                // in case any other error occurred, which may be because permissions have been granted yet
                e.PrintStackTrace();
                errorOccurred = true;
            }

            if (errorOccurred)
            {
                cameraPreviewControl.RaiseErrorOpeningCamera();
            }
        }

        internal void SaveToPhotosLibrary(Java.IO.File file)
        {
            var intent = new Intent(Intent.ActionMediaScannerScanFile);
            var uri = global::Android.Net.Uri.FromFile(file);
            intent.SetData(uri);
            Activity.SendBroadcast(intent);
        }

        internal void RaiseErrorOpening()
        {
            cameraPreviewControl.RaiseErrorOpeningCamera();
        }

        protected async Task<string> CaptureCamera2PhotoToFile(bool saveToPhotosLibrary)
        {
            try
            {
                imageAvailableListener.FilePath = BuildFilePath();
                imageAvailableListener.SaveToPhotosLibrary = saveToPhotosLibrary;
                imageAvailableListener.SavePhotoTaskCompletionSource = new TaskCompletionSource<string>();
                LockFocus();
                var finalFilePath = await imageAvailableListener.SavePhotoTaskCompletionSource.Task;
                if (saveToPhotosLibrary)
                {
                    using (var file = new Java.IO.File(finalFilePath))
                    {
                        SaveToPhotosLibrary(file);
                    }
                }

                return imageAvailableListener.FilePath;
            }
            finally
            {
                imageAvailableListener.FilePath = null;
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
                string filePath = BuildFilePath();
                var fileStream = new FileStream(filePath, FileMode.Create);
                await image.CompressAsync(Bitmap.CompressFormat.Jpeg, 50, fileStream);
                fileStream.Close();
                image.Recycle();

                if (saveToPhotosLibrary)
                {
                    // Broadcasting the the file's Uri in the following intent will allow any Photo Gallery apps to incorporate
                    // the photo file into their collection -RR
                    using (var file = new Java.IO.File(filePath))
                    {
                        SaveToPhotosLibrary(file);
                    }
                }

                return filePath;
            }
            catch (System.Exception ex)
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
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            var cpc = Element as CameraPreviewControl;
            if (cpc == null)
            {
                return;
            }

            cameraPreviewControl = cpc;

            useCamera2Api = Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop;
            if (useCamera2Api)
            {
                cameraPreviewControl.RetrieveCamerasFunc = RetrieveCameras2;
                cameraPreviewControl.RetrieveSupportedFocusModesFunc = RetrieveCamera2SupportedFocusModes;
                cameraPreviewControl.CaptureNativeFrameToFileFunc = CaptureCamera2PhotoToFile;
                stateCallback = new CameraStateListener(this);
                CaptureCallback = new CameraCaptureListener(this);
                imageAvailableListener = new ImageAvailableListener(this);
                surfaceTextureListener = new Camera2BasicSurfaceTextureListener(this);
                StartBackgroundThread();

                // fill Orientations list
                Orientations.Append((int)SurfaceOrientation.Rotation0, 90);
                Orientations.Append((int)SurfaceOrientation.Rotation90, 0);
                Orientations.Append((int)SurfaceOrientation.Rotation180, 270);
                Orientations.Append((int)SurfaceOrientation.Rotation270, 180);
            }
            else
            {
                cameraPreviewControl.RetrieveCamerasFunc = RetrieveCameras;
                cameraPreviewControl.CaptureNativeFrameToFileFunc = CapturePhotoToFile;
                cameraPreviewControl.RetrieveSupportedFocusModesFunc = RetrieveSupportedFocusModes;
            }

            try
            {
                SetupUserInterface();
                AddView(view);
            }
            catch (System.Exception ex)
            {
                ex.LogError();
            }
        }

        /// <inheritdoc />
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == CameraPreviewControl.PreferredCameraProperty.PropertyName)
            {
                if (useCamera2Api)
                {
                    SwitchCamera2IfNecessary();
                    return;
                }

                SwitchCameraIfNecessary();
            }
            else if (e.PropertyName == CameraPreviewControl.EnableContinuousAutoFocusProperty.PropertyName)
            {
                if (useCamera2Api)
                {
                    EnableContinuousAutoFocusCamera2(cameraPreviewControl.EnableContinuousAutoFocus);
                    return;
                }

                EnableContinuousAutoFocus(cameraPreviewControl.EnableContinuousAutoFocus);
            }
            else if (e.PropertyName == CameraPreviewControl.AspectProperty.PropertyName)
            {
                if (useCamera2Api)
                {
                    ApplyAspectForCameraApi2();
                }
                else
                {
                    ApplyAspect();
                }
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

        private static string BuildFilePath()
        {
            var absolutePath = global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryDcim).AbsolutePath;
            var folderPath = System.IO.Path.Combine(absolutePath, ImageCapture, DateTime.Now.ToString("yyyy-MM-dd"));
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
            return filePath;
        }

        private static global::Android.Util.Size ChooseOptimalSize(global::Android.Util.Size[] choices, int textureViewWidth, int textureViewHeight, int maxWidth, int maxHeight, global::Android.Util.Size aspectRatio)
        {
            // Collect the supported resolutions that are at least as big as the preview Surface
            var bigEnough = new List<global::Android.Util.Size>();

            // Collect the supported resolutions that are smaller than the preview Surface
            var notBigEnough = new List<global::Android.Util.Size>();
            int w = aspectRatio.Width;
            int h = aspectRatio.Height;

            for (var i = 0; i < choices.Length; i++)
            {
                global::Android.Util.Size option = choices[i];
                if ((option.Width <= maxWidth) && (option.Height <= maxHeight) &&
                       option.Height == option.Width * h / w)
                {
                    if (option.Width >= textureViewWidth &&
                        option.Height >= textureViewHeight)
                    {
                        bigEnough.Add(option);
                    }
                    else
                    {
                        notBigEnough.Add(option);
                    }
                }
            }

            // Pick the smallest of those big enough. If there is no one big enough, pick the
            // largest of those not big enough.
            if (bigEnough.Count > 0)
            {
                return (global::Android.Util.Size)Collections.Min(bigEnough, new CompareSizesByArea());
            }
            else if (notBigEnough.Count > 0)
            {
                return (global::Android.Util.Size)Collections.Max(notBigEnough, new CompareSizesByArea());
            }
            else
            {
                return choices[0];
            }
        }

        private static CameraPreviewControl.CameraFacing ToCameraFacing(CameraFacing cameraFacing)
        {
            return cameraFacing == CameraFacing.Back ? CameraPreviewControl.CameraFacing.Back : CameraPreviewControl.CameraFacing.Front;
        }

        private static CameraPreviewControl.CameraFacing ToCameraFacing(LensFacing lensFacing)
        {
            switch (lensFacing)
            {
                case LensFacing.Back:
                    return CameraPreviewControl.CameraFacing.Back;

                case LensFacing.Front:
                    return CameraPreviewControl.CameraFacing.Front;

                default:
                    return CameraPreviewControl.CameraFacing.Unspecified;
            }
        }

        private static CameraFacing ToCameraFacing(CameraPreviewControl.CameraFacing cameraFacing)
        {
            return cameraFacing == CameraPreviewControl.CameraFacing.Back
                ? CameraFacing.Back
                : CameraFacing.Front;
        }

        private static ControlAFMode ToControlAFMode(CameraFocusMode cameraFocusMode)
        {
            switch (cameraFocusMode)
            {
                case CameraFocusMode.Auto:
                    return ControlAFMode.Auto;

                case CameraFocusMode.Continuous:
                    return ControlAFMode.ContinuousPicture;

                default:
                    return ControlAFMode.Off;
            }
        }

        private static LensFacing ToLensFacing(CameraPreviewControl.CameraFacing cameraFacing)
        {
            return cameraFacing == CameraPreviewControl.CameraFacing.Back ? LensFacing.Back :
                LensFacing.Front;
        }

        private void CloseCamera()
        {
            try
            {
                CameraOpenCloseLock.Acquire();
                if (CaptureSession != null)
                {
                    CaptureSession.Close();
                    CaptureSession = null;
                }

                if (CameraDevice != null)
                {
                    CameraDevice.Close();
                    CameraDevice = null;
                }

                if (imageReader != null)
                {
                    imageReader.Close();
                    imageReader = null;
                }
            }
            catch (InterruptedException e)
            {
                throw new RuntimeException("Interrupted while trying to lock camera closing.", e);
            }
            finally
            {
                CameraOpenCloseLock.Release();
            }
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
                switch (defaultFocusMode)
                {
                    case CameraFocusMode.Continuous:
                        cameraParameters.FocusMode = global::Android.Hardware.Camera.Parameters.FocusModeContinuousPicture;
                        break;

                    default:

                        // fallback to autofocus
                        cameraParameters.FocusMode = global::Android.Hardware.Camera.Parameters.FocusModeAuto;
                        break;
                }

                camera.SetParameters(cameraParameters);
            }
        }

        private void EnableContinuousAutoFocusCamera2(bool enable)
        {
            CaptureSession.StopRepeating();
            var current = (int)PreviewRequest.Get(CaptureRequest.ControlAfMode);

            PreviewRequestBuilder.Set(CaptureRequest.ControlAfMode, enable ? (int)ControlAFMode.ContinuousPicture : (int)ToControlAFMode(defaultFocusMode));
            CaptureSession.SetRepeatingRequest(PreviewRequestBuilder.Build(), CaptureCallback, BackgroundHandler);
        }

        private int GetOrientation(int rotation)
        {
            // Sensor orientation is 90 for most devices, or 270 for some devices (eg. Nexus 5X)
            // We have to take that into account and rotate JPEG properly.
            // For devices with orientation of 90, we simply return our mapping from ORIENTATIONS.
            // For devices with orientation of 270, we need to rotate the JPEG 180 degrees.
            return (Orientations.Get(rotation) + sensorOrientation + 270) % 360;
        }

        private void LockFocus()
        {
            try
            {
                // This is how to tell the camera to lock focus.
                PreviewRequestBuilder.Set(CaptureRequest.ControlAfTrigger, (int)ControlAFTrigger.Start);

                // Tell #captureCallback to wait for the lock.
                State = StateWaitingLock;
                CaptureSession.Capture(PreviewRequestBuilder.Build(), CaptureCallback, BackgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        private void PrepareAndStartCamera()
        {
            camera.StopPreview();

            var display = Activity.WindowManager.DefaultDisplay;
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

            switch (cameraParameters.FocusMode)
            {
                case global::Android.Hardware.Camera.Parameters.FocusModeContinuousPicture:
                    defaultFocusMode = CameraFocusMode.Continuous;
                    break;

                case global::Android.Hardware.Camera.Parameters.FocusModeAuto:
                    defaultFocusMode = CameraFocusMode.Auto;
                    break;
            }

            camera.SetNonMarshalingPreviewCallback(this);
            camera.StartPreview();
        }

        private IReadOnlyList<CameraFocusMode> RetrieveCamera2SupportedFocusModes()
        {
            var supportedFocusModes = new List<CameraFocusMode>();
            var manager = (CameraManager)Activity.GetSystemService(Context.CameraService);
            var cameraCharacteristics = manager.GetCameraCharacteristics(CameraDevice.Id);
            var focusModes = cameraCharacteristics.Get(CameraCharacteristics.ControlAfAvailableModes).ToArray<int>();
            foreach (var focusMode in focusModes)
            {
                if (focusMode == (int)ControlAFMode.Auto)
                {
                    supportedFocusModes.Add(CameraFocusMode.Auto);
                }
                else if (focusMode == (int)ControlAFMode.ContinuousPicture)
                {
                    supportedFocusModes.Add(CameraFocusMode.Continuous);
                }
            }

            var capabilities = cameraCharacteristics.Get(CameraCharacteristics.RequestAvailableCapabilities).ToArray<int>();
            if (capabilities.Any(c => c == (int)RequestAvailableCapabilities.ManualSensor))
            {
                supportedFocusModes.Add(CameraFocusMode.Manual);
            }

            return supportedFocusModes.AsReadOnly();
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<IReadOnlyList<ICamera>> RetrieveCameras2()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var cameras = new List<ICamera>();
            var manager = (CameraManager)Activity.GetSystemService(Context.CameraService);
            var cameraIdList = manager.GetCameraIdList();
            for (var i = 0; i < cameraIdList.Length; i++)
            {
                var cameraId = cameraIdList[i];
                CameraCharacteristics characteristics = manager.GetCameraCharacteristics(cameraId);
                var facing = (LensFacing)(int)characteristics.Get(CameraCharacteristics.LensFacing);
                var camera = new Camera()
                {
                    Id = cameraId,
                    CameraFacing = ToCameraFacing(facing)
                };
                cameras.Add(camera);
            }

            return cameras.AsReadOnly();
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<IReadOnlyList<ICamera>> RetrieveCameras()
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

        private void SetUpCameraOutputs(int width, int height)
        {
            var manager = (CameraManager)Activity.GetSystemService(Context.CameraService);
            try
            {
                var cameraIdList = manager.GetCameraIdList();
                for (var i = 0; i < cameraIdList.Length; i++)
                {
                    var cameraId = cameraIdList[i];
                    CameraCharacteristics characteristics = manager.GetCameraCharacteristics(cameraId);
                    var facing = (int)characteristics.Get(CameraCharacteristics.LensFacing);
                    if (facing != (int)lensFacing)
                    {
                        continue;
                    }

                    var map = (StreamConfigurationMap)characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap);
                    if (map == null)
                    {
                        continue;
                    }

                    // For still image captures, we use the largest available size.
                    global::Android.Util.Size largest = (global::Android.Util.Size)Collections.Max(
                        Arrays.AsList(map.GetOutputSizes((int)ImageFormatType.Jpeg)),
                        new CompareSizesByArea());
                    imageReader = ImageReader.NewInstance(largest.Width, largest.Height, ImageFormatType.Jpeg, 2);
                    imageReader.SetOnImageAvailableListener(imageAvailableListener, BackgroundHandler);

                    sensorOrientation = (int)characteristics.Get(CameraCharacteristics.SensorOrientation);

                    global::Android.Graphics.Point displaySize = new global::Android.Graphics.Point();
                    Activity.WindowManager.DefaultDisplay.GetSize(displaySize);
                    var rotatedPreviewWidth = width;
                    var rotatedPreviewHeight = height;
                    var maxPreviewWidth = displaySize.X;
                    var maxPreviewHeight = displaySize.Y;

                    bool swappedDimensions = SwappedDimensions();
                    if (swappedDimensions)
                    {
                        rotatedPreviewWidth = height;
                        rotatedPreviewHeight = width;
                        maxPreviewWidth = displaySize.Y;
                        maxPreviewHeight = displaySize.X;
                    }

                    if (maxPreviewWidth > MaxPreviewWidth)
                    {
                        maxPreviewWidth = MaxPreviewWidth;
                    }

                    if (maxPreviewHeight > MaxPreviewHeight)
                    {
                        maxPreviewHeight = MaxPreviewHeight;
                    }

                    // Danger, W.R.! Attempting to use too large a preview size could  exceed the camera
                    // bus' bandwidth limitation, resulting in gorgeous previews but the storage of
                    // garbage capture data.
                    previewSize = ChooseOptimalSize(map.GetOutputSizes(Class.FromType(typeof(SurfaceTexture))), rotatedPreviewWidth, rotatedPreviewHeight, maxPreviewWidth, maxPreviewHeight, largest);
                    ApplyAspectForCameraApi2();

                    // Check if the flash is supported.
                    var available = (Java.Lang.Boolean)characteristics.Get(CameraCharacteristics.FlashInfoAvailable);
                    if (available == null)
                    {
                        flashSupported = false;
                    }
                    else
                    {
                        flashSupported = (bool)available;
                    }

                    this.cameraId = cameraId;
                    return;
                }
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
            catch (NullPointerException e)
            {
                e.PrintStackTrace();
            }
        }

        private void ApplyAspectForCameraApi2()
        {
            // We fit the aspect ratio of TextureView to the size of preview we picked.
            var orientation = Resources.Configuration.Orientation;
            if (orientation == global::Android.Content.Res.Orientation.Landscape)
            {
                textureView.SetAspectRatio(cameraPreviewControl.Aspect, previewSize.Width, previewSize.Height);
            }
            else
            {
                textureView.SetAspectRatio(cameraPreviewControl.Aspect, previewSize.Height, previewSize.Width);
            }
        }

        private bool SwappedDimensions()
        {
            var swappedDimensions = false;

            // Find out if we need to swap dimension to get the preview size relative to sensor
            // coordinate.
            var displayRotation = Activity.WindowManager.DefaultDisplay.Rotation;
            switch (displayRotation)
            {
                case SurfaceOrientation.Rotation0:
                case SurfaceOrientation.Rotation180:
                    if (sensorOrientation == 90 || sensorOrientation == 270)
                    {
                        swappedDimensions = true;
                    }

                    break;

                case SurfaceOrientation.Rotation90:
                case SurfaceOrientation.Rotation270:
                    if (sensorOrientation == 0 || sensorOrientation == 180)
                    {
                        swappedDimensions = true;
                    }

                    break;

                default:
                    break;
            }

            return swappedDimensions;
        }

        private void SetupUserInterface()
        {
            view = Activity.LayoutInflater.Inflate(Resource.Layout.CameraLayout, this, false);
            lensFacing = ToLensFacing(cameraPreviewControl.PreferredCamera);

            textureView = view.FindViewById<AutoFitTextureView>(Resource.Id.autoFitTextureView);
            textureView.SurfaceTextureListener = this;
        }

        // Starts a background thread and its {@link Handler}.
        private void StartBackgroundThread()
        {
            backgroundThread = new HandlerThread("CameraBackground");
            backgroundThread.Start();
            BackgroundHandler = new Handler(backgroundThread.Looper);
        }

        // Stops the background thread and its {@link Handler}.
        private void StopBackgroundThread()
        {
            backgroundThread.QuitSafely();
            try
            {
                backgroundThread.Join();
                backgroundThread = null;
                BackgroundHandler = null;
            }
            catch (InterruptedException e)
            {
                e.PrintStackTrace();
            }
        }

        private void SwitchCamera2IfNecessary()
        {
            lensFacing = ToLensFacing(cameraPreviewControl.PreferredCamera);
            OpenCamera(textureView.Width, textureView.Height);
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
    }
}