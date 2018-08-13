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
        public int state = STATE_PREVIEW;
        public CaptureRequest.Builder previewRequestBuilder;
        public CaptureRequest previewRequest;
        public CameraCaptureListener captureCallback;

        private static readonly SparseIntArray ORIENTATIONS = new SparseIntArray();

        private global::Android.Hardware.Camera camera;
        private global::Android.Views.View view;
        private bool flashSupported;
        private bool useCamera2Api;
        private TaskCompletionSource<string> savePhotoTaskCompletionSource = new TaskCompletionSource<string>();

        // Camera state: Showing camera preview.
        internal const int STATE_PREVIEW = 0;

        // Camera state: Waiting for the focus to be locked.
        internal const int STATE_WAITING_LOCK = 1;

        // Camera state: Waiting for the exposure to be precapture state.
        internal const int STATE_WAITING_PRECAPTURE = 2;

        // Camera state: Waiting for the exposure state to be something other than precapture.
        internal const int STATE_WAITING_NON_PRECAPTURE = 3;

        // Camera state: Picture was taken.
        internal const int STATE_PICTURE_TAKEN = 4;

        // Max preview width that is guaranteed by Camera2 API
        private static readonly int MAX_PREVIEW_WIDTH = 1920;

        // Max preview height that is guaranteed by Camera2 API
        private static readonly int MAX_PREVIEW_HEIGHT = 1080;

        private CaptureRequest.Builder stillCaptureBuilder;
        private CameraFacing cameraType;
        private LensFacing lensFacing;
        private AutoFitTextureView textureView;
        private SurfaceTexture surfaceTexture;
        private int sensorOrientation;
        private ImageAvailableListener imageAvailableListener;

        private CameraPreviewControl cameraPreviewControl;
        private CameraFocusMode defaultFocusMode;
        private ImageReader imageReader;

        private Camera2BasicSurfaceTextureListener surfaceTextureListener;

        // ID of the current {@link CameraDevice}.
        private string cameraId;

        // A {@link CameraCaptureSession } for camera preview.
        public CameraCaptureSession captureSession;

        // A reference to the opened CameraDevice
        public CameraDevice cameraDevice;

        // The size of the camera preview
        private global::Android.Util.Size previewSize;

        // CameraDevice.StateListener is called when a CameraDevice changes its state
        private CameraStateListener stateCallback;

        // An additional thread for running tasks that shouldn't block the UI.
        private HandlerThread backgroundThread;

        // A {@link Handler} for running tasks in the background.
        public Handler backgroundHandler;

        internal Activity Activity => Context as Activity;

        public Semaphore CameraOpenCloseLock { get; } = new Semaphore(1);

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraPreviewControlRenderer"/> class.
        /// </summary>
        /// <param name="context"><see cref="Context"/></param>
        public CameraPreviewControlRenderer(Context context)
            : base(context)
        {
        }

        internal void OpenCamera(int width, int height)
        {
            cameraDevice?.Close();
            cameraDevice = null;
            SetUpCameraOutputs(width, height);
            ConfigureTransform(width, height);
            var manager = (CameraManager)Context.GetSystemService(Context.CameraService);
            try
            {
                if (!CameraOpenCloseLock.TryAcquire(2500, TimeUnit.Milliseconds))
                {
                    throw new RuntimeException("Time out waiting to lock camera opening.");
                }

                manager.OpenCamera(cameraId, stateCallback, backgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
            catch (InterruptedException e)
            {
                throw new RuntimeException("Interrupted while trying to lock camera opening.", e);
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

        /// <inheritdoc />
        public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
        {
            if (useCamera2Api)
            {
                //mOnImageAvailableListener = new ImageAvailableListener(this);
                OpenCamera(width, height);
                return;
            }

            camera = global::Android.Hardware.Camera.Open((int)cameraType);
            textureView.LayoutParameters = new FrameLayout.LayoutParams(width, height);
            surfaceTexture = surface;

            camera.SetPreviewTexture(surface);
            PrepareAndStartCamera();
        }

        public void SetAutoFlash(CaptureRequest.Builder requestBuilder)
        {
            if (flashSupported)
            {
                requestBuilder.Set(CaptureRequest.ControlAeMode, (int)ControlAEMode.OnAutoFlash);
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

        public void OnActivityPaused(Activity activity)
        {
            CloseCamera();
            StopBackgroundThread();
        }

        public void OnActivityResumed(Activity activity)
        {
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
            cameraPreviewControl.RetrieveCamerasFunc = RetrieveCamerasAsync;

            useCamera2Api = Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop;
            if (useCamera2Api)
            {
                cameraPreviewControl.RetrieveSupportedFocusModesFunc = RetrieveCamera2SupportedFocusModes;
                cameraPreviewControl.CaptureNativeFrameToFileFunc = CaptureCamera2PhotoToFile;
                stateCallback = new CameraStateListener(this);
                captureCallback = new CameraCaptureListener(this);
                imageAvailableListener = new ImageAvailableListener(this, savePhotoTaskCompletionSource);

                surfaceTextureListener = new Camera2BasicSurfaceTextureListener(this);
                StartBackgroundThread();

                // fill ORIENTATIONS list
                ORIENTATIONS.Append((int)SurfaceOrientation.Rotation0, 90);
                ORIENTATIONS.Append((int)SurfaceOrientation.Rotation90, 0);
                ORIENTATIONS.Append((int)SurfaceOrientation.Rotation180, 270);
                ORIENTATIONS.Append((int)SurfaceOrientation.Rotation270, 180);
            }
            else
            {
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

        internal void SaveToPhotosLibrary(Java.IO.File file)
        {
            var intent = new Intent(Intent.ActionMediaScannerScanFile);
            var uri = global::Android.Net.Uri.FromFile(file);
            intent.SetData(uri);
            Activity.SendBroadcast(intent);
        }

        private static string BuildFilePath()
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
            return filePath;
        }

        protected async Task<string> CaptureCamera2PhotoToFile(bool saveToPhotosLibrary)
        {
            try
            {
                imageAvailableListener.FilePath = BuildFilePath();
                imageAvailableListener.SaveToPhotosLibrary = saveToPhotosLibrary;
                LockFocus();
                var finalFilePath = await savePhotoTaskCompletionSource.Task;
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

        private static LensFacing ToLensFacing(CameraPreviewControl.CameraFacing cameraFacing)
        {
            return cameraFacing == CameraPreviewControl.CameraFacing.Back ? LensFacing.Back :
                LensFacing.Front;
        }

        private static CameraPreviewControl.CameraFacing ToCameraFacing(CameraFacing cameraFacing)
        {
            return cameraFacing == CameraFacing.Back ? CameraPreviewControl.CameraFacing.Back : CameraPreviewControl.CameraFacing.Front;
        }

        private void SwitchCameraIfNecessary()
        {
            if (useCamera2Api)
            {
                lensFacing = ToLensFacing(cameraPreviewControl.PreferredCamera);
                OpenCamera(textureView.Width, textureView.Height);
                return;
            }

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
            view = Activity.LayoutInflater.Inflate(Resource.Layout.CameraLayout, this, false);
            lensFacing = ToLensFacing(cameraPreviewControl.PreferredCamera);

            textureView = view.FindViewById<AutoFitTextureView>(Resource.Id.autoFitTextureView);
            textureView.SurfaceTextureListener = this;
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

        private void EnableContinuousAutoFocus(bool enable)
        {
            if (useCamera2Api)
            {
                captureSession.StopRepeating();
                var current = (int)previewRequest.Get(CaptureRequest.ControlAfMode);

                previewRequestBuilder.Set(CaptureRequest.ControlAfMode, enable ? (int)ControlAFMode.ContinuousPicture : (int)ToControlAFMode(defaultFocusMode));
                captureSession.SetRepeatingRequest(previewRequestBuilder.Build(), captureCallback, backgroundHandler);
                return;
            }

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

        private IReadOnlyList<CameraFocusMode> RetrieveCamera2SupportedFocusModes()
        {
            var supportedFocusModes = new List<CameraFocusMode>();
            var manager = (CameraManager)Activity.GetSystemService(Context.CameraService);
            var cameraCharacteristics = manager.GetCameraCharacteristics(cameraDevice.Id);
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

            return supportedFocusModes.AsReadOnly();
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
                    global::Android.Util.Size largest = (global::Android.Util.Size)Collections.Max(Arrays.AsList(map.GetOutputSizes((int)ImageFormatType.Jpeg)),
                        new CompareSizesByArea());
                    imageReader = ImageReader.NewInstance(largest.Width, largest.Height, ImageFormatType.Jpeg, /*maxImages*/2);
                    imageReader.SetOnImageAvailableListener(imageAvailableListener, backgroundHandler);

                    // Find out if we need to swap dimension to get the preview size relative to sensor
                    // coordinate.
                    var displayRotation = Activity.WindowManager.DefaultDisplay.Rotation;

                    //noinspection ConstantConditions
                    sensorOrientation = (int)characteristics.Get(CameraCharacteristics.SensorOrientation);
                    bool swappedDimensions = false;
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

                            //Log.Error(TAG, "Display rotation is invalid: " + displayRotation);
                            break;
                    }

                    global::Android.Graphics.Point displaySize = new global::Android.Graphics.Point();
                    Activity.WindowManager.DefaultDisplay.GetSize(displaySize);
                    var rotatedPreviewWidth = width;
                    var rotatedPreviewHeight = height;
                    var maxPreviewWidth = displaySize.X;
                    var maxPreviewHeight = displaySize.Y;

                    if (swappedDimensions)
                    {
                        rotatedPreviewWidth = height;
                        rotatedPreviewHeight = width;
                        maxPreviewWidth = displaySize.Y;
                        maxPreviewHeight = displaySize.X;
                    }

                    if (maxPreviewWidth > MAX_PREVIEW_WIDTH)
                    {
                        maxPreviewWidth = MAX_PREVIEW_WIDTH;
                    }

                    if (maxPreviewHeight > MAX_PREVIEW_HEIGHT)
                    {
                        maxPreviewHeight = MAX_PREVIEW_HEIGHT;
                    }

                    // Danger, W.R.! Attempting to use too large a preview size could  exceed the camera
                    // bus' bandwidth limitation, resulting in gorgeous previews but the storage of
                    // garbage capture data.
                    previewSize = ChooseOptimalSize(map.GetOutputSizes(Class.FromType(typeof(SurfaceTexture))), rotatedPreviewWidth, rotatedPreviewHeight, maxPreviewWidth, maxPreviewHeight, largest);

                    // We fit the aspect ratio of TextureView to the size of preview we picked.
                    var orientation = Resources.Configuration.Orientation;
                    if (orientation == global::Android.Content.Res.Orientation.Landscape)
                    {
                        textureView.SetAspectRatio(previewSize.Width, previewSize.Height);
                    }
                    else
                    {
                        textureView.SetAspectRatio(previewSize.Height, previewSize.Width);
                    }

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
                // Currently an NPE is thrown when the Camera2API is used but not supported on the
                // device this code runs.
                //ErrorDialog.NewInstance(GetString(Resource.String.camera_error)).Show(ChildFragmentManager, FRAGMENT_DIALOG);
            }
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

        private void CloseCamera()
        {
            try
            {
                CameraOpenCloseLock.Acquire();
                if (captureSession != null)
                {
                    captureSession.Close();
                    captureSession = null;
                }

                if (cameraDevice != null)
                {
                    cameraDevice.Close();
                    cameraDevice = null;
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
                previewRequestBuilder = cameraDevice.CreateCaptureRequest(CameraTemplate.Preview);
                previewRequestBuilder.AddTarget(surface);

                // Here, we create a CameraCaptureSession for camera preview.
                var surfaces = new List<Surface>();
                surfaces.Add(surface);
                surfaces.Add(imageReader.Surface);
                cameraDevice.CreateCaptureSession(surfaces, new CameraCaptureSessionCallback(this), null);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        public void CaptureStillPicture()
        {
            try
            {
                if (Activity == null || cameraDevice == null)
                {
                    return;
                }

                // This is the CaptureRequest.Builder that we use to take a picture.
                if (stillCaptureBuilder == null)
                {
                    stillCaptureBuilder = cameraDevice.CreateCaptureRequest(CameraTemplate.StillCapture);
                }

                stillCaptureBuilder.AddTarget(imageReader.Surface);

                // Use the same AE and AF modes as the preview.
                stillCaptureBuilder.Set(CaptureRequest.ControlAfMode, (int)ControlAFMode.ContinuousPicture);
                SetAutoFlash(stillCaptureBuilder);

                // Orientation
                int rotation = (int)Activity.WindowManager.DefaultDisplay.Rotation;
                stillCaptureBuilder.Set(CaptureRequest.JpegOrientation, GetOrientation(rotation));

                captureSession.StopRepeating();
                captureSession.Capture(stillCaptureBuilder.Build(), new CameraCaptureStillPictureSessionCallback(this), null);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        private int GetOrientation(int rotation)
        {
            // Sensor orientation is 90 for most devices, or 270 for some devices (eg. Nexus 5X)
            // We have to take that into account and rotate JPEG properly.
            // For devices with orientation of 90, we simply return our mapping from ORIENTATIONS.
            // For devices with orientation of 270, we need to rotate the JPEG 180 degrees.
            return (ORIENTATIONS.Get(rotation) + sensorOrientation + 270) % 360;
        }

        // Run the precapture sequence for capturing a still image. This method should be called when
        // we get a response in {@link #mCaptureCallback} from {@link #lockFocus()}.
        public void RunPrecaptureSequence()
        {
            try
            {
                // This is how to tell the camera to trigger.
                previewRequestBuilder.Set(CaptureRequest.ControlAePrecaptureTrigger, (int)ControlAEPrecaptureTrigger.Start);

                // Tell #mCaptureCallback to wait for the precapture sequence to be set.
                state = STATE_WAITING_PRECAPTURE;
                captureSession.Capture(previewRequestBuilder.Build(), captureCallback, backgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        // Starts a background thread and its {@link Handler}.
        private void StartBackgroundThread()
        {
            backgroundThread = new HandlerThread("CameraBackground");
            backgroundThread.Start();
            backgroundHandler = new Handler(backgroundThread.Looper);
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
        }

        public void OnActivityDestroyed(Activity activity)
        {
        }

        // Stops the background thread and its {@link Handler}.
        private void StopBackgroundThread()
        {
            backgroundThread.QuitSafely();
            try
            {
                backgroundThread.Join();
                backgroundThread = null;
                backgroundHandler = null;
            }
            catch (InterruptedException e)
            {
                e.PrintStackTrace();
            }
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

        public void UnlockFocus()
        {
            try
            {
                // Reset the auto-focus trigger
                previewRequestBuilder.Set(CaptureRequest.ControlAfTrigger, (int)ControlAFTrigger.Cancel);
                SetAutoFlash(previewRequestBuilder);
                captureSession.Capture(previewRequestBuilder.Build(), captureCallback, backgroundHandler);

                // After this, the camera will go back to the normal state of preview.
                state = STATE_PREVIEW;
                captureSession.SetRepeatingRequest(previewRequest, captureCallback, backgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        private void LockFocus()
        {
            try
            {
                // This is how to tell the camera to lock focus.
                previewRequestBuilder.Set(CaptureRequest.ControlAfTrigger, (int)ControlAFTrigger.Start);

                // Tell #captureCallback to wait for the lock.
                state = STATE_WAITING_LOCK;
                captureSession.Capture(previewRequestBuilder.Build(), captureCallback, backgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }
    }
}