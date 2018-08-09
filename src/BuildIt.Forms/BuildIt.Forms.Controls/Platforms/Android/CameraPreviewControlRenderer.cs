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
        private global::Android.Hardware.Camera camera;
        private global::Android.Views.View view;
        public CaptureRequest.Builder mPreviewRequestBuilder;
        public CaptureRequest mPreviewRequest;
        public CameraCaptureListener mCaptureCallback;
        private bool mFlashSupported;
        public int mState = STATE_PREVIEW;
        private bool useCamera2Api;

        // Camera state: Showing camera preview.
        public const int STATE_PREVIEW = 0;

        // Camera state: Waiting for the focus to be locked.
        public const int STATE_WAITING_LOCK = 1;

        // Camera state: Waiting for the exposure to be precapture state.
        public const int STATE_WAITING_PRECAPTURE = 2;

        //Camera state: Waiting for the exposure state to be something other than precapture.
        public const int STATE_WAITING_NON_PRECAPTURE = 3;

        // Camera state: Picture was taken.
        public const int STATE_PICTURE_TAKEN = 4;
        public Activity Activity => Context as Activity;
        private CameraFacing cameraType;
        private LensFacing lensFacing;
        private TextureView textureView;
        private SurfaceTexture surfaceTexture;
        private int mSensorOrientation;
        private ImageAvailableListener mOnImageAvailableListener;

        private CameraPreviewControl cameraPreviewControl;
        private string defaultFocusMode;
        private ImageReader mImageReader;

        private Camera2BasicSurfaceTextureListener mSurfaceTextureListener;

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
            if (mFlashSupported)
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
            useCamera2Api = Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop;
            if (useCamera2Api)
            {
                mStateCallback = new CameraStateListener(this);
                mSurfaceTextureListener = new Camera2BasicSurfaceTextureListener(this);

                // fill ORIENTATIONS list
                ORIENTATIONS.Append((int)SurfaceOrientation.Rotation0, 90);
                ORIENTATIONS.Append((int)SurfaceOrientation.Rotation90, 0);
                ORIENTATIONS.Append((int)SurfaceOrientation.Rotation180, 270);
                ORIENTATIONS.Append((int)SurfaceOrientation.Rotation270, 180);
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
                    Activity.SendBroadcast(intent);
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
            if (useCamera2Api)
            {
                Activity.Application.RegisterActivityLifecycleCallbacks(this);

                view = Activity.LayoutInflater.Inflate(Resource.Layout.Camera2Layout, this, false);
                lensFacing = ToLensFacing(cameraPreviewControl.PreferredCamera);

                mTextureView = view.FindViewById<AutoFitTextureView>(Resource.Id.autoFitTextureView);
                mTextureView.SurfaceTextureListener = this;
            }
            else
            {
                view = Activity.LayoutInflater.Inflate(Resource.Layout.CameraLayout, this, false);
                cameraType = ToCameraFacing(cameraPreviewControl.PreferredCamera);

                textureView = view.FindViewById<TextureView>(Resource.Id.textureView);
                textureView.SurfaceTextureListener = this;
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

        private void PrepareAndStartCamera2()
        {
        }

        // ID of the current {@link CameraDevice}.
        private string mCameraId;

        // An AutoFitTextureView for camera preview
        private AutoFitTextureView mTextureView;

        // A {@link CameraCaptureSession } for camera preview.
        public CameraCaptureSession mCaptureSession;

        // A reference to the opened CameraDevice
        public CameraDevice mCameraDevice;

        // The size of the camera preview
        private global::Android.Util.Size mPreviewSize;

        // CameraDevice.StateListener is called when a CameraDevice changes its state
        private CameraStateListener mStateCallback;

        // An additional thread for running tasks that shouldn't block the UI.
        private HandlerThread mBackgroundThread;

        // A {@link Handler} for running tasks in the background.
        public Handler mBackgroundHandler;

        public void OpenCamera(int width, int height)
        {
            System.Diagnostics.Debug.WriteLine("bui: open camera");
            SetUpCameraOutputs(width, height);
            ConfigureTransform(width, height);
            var manager = (CameraManager)Context.GetSystemService(Context.CameraService);
            try
            {
                if (!mCameraOpenCloseLock.TryAcquire(2500, TimeUnit.Milliseconds))
                {
                    throw new RuntimeException("Time out waiting to lock camera opening.");
                }

                manager.OpenCamera(mCameraId, mStateCallback, mBackgroundHandler);
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

        public void ConfigureTransform(int viewWidth, int viewHeight)
        {
            if (null == mTextureView || null == mPreviewSize || null == Activity)
            {
                return;
            }

            var rotation = (int)Activity.WindowManager.DefaultDisplay.Rotation;
            Matrix matrix = new Matrix();
            RectF viewRect = new RectF(0, 0, viewWidth, viewHeight);
            RectF bufferRect = new RectF(0, 0, mPreviewSize.Height, mPreviewSize.Width);
            float centerX = viewRect.CenterX();
            float centerY = viewRect.CenterY();
            if ((int)SurfaceOrientation.Rotation90 == rotation || (int)SurfaceOrientation.Rotation270 == rotation)
            {
                bufferRect.Offset(centerX - bufferRect.CenterX(), centerY - bufferRect.CenterY());
                matrix.SetRectToRect(viewRect, bufferRect, Matrix.ScaleToFit.Fill);
                float scale = System.Math.Max((float)viewHeight / mPreviewSize.Height, (float)viewWidth / mPreviewSize.Width);
                matrix.PostScale(scale, scale, centerX, centerY);
                matrix.PostRotate(90 * (rotation - 2), centerX, centerY);
            }
            else if ((int)SurfaceOrientation.Rotation180 == rotation)
            {
                matrix.PostRotate(180, centerX, centerY);
            }
            mTextureView.SetTransform(matrix);
        }

        // Max preview width that is guaranteed by Camera2 API
        private static readonly int MAX_PREVIEW_WIDTH = 1920;

        // Max preview height that is guaranteed by Camera2 API
        private static readonly int MAX_PREVIEW_HEIGHT = 1080;

        private void SetUpCameraOutputs(int width, int height)
        {
            System.Diagnostics.Debug.WriteLine("Bui: setup camera outputs");
            var manager = (CameraManager)Activity.GetSystemService(Context.CameraService);
            try
            {
                for (var i = 0; i < manager.GetCameraIdList().Length; i++)
                {
                    var cameraId = manager.GetCameraIdList()[i];
                    CameraCharacteristics characteristics = manager.GetCameraCharacteristics(cameraId);

                    // We don't use a front facing camera in this sample.
                    var facing = (Integer)characteristics.Get(CameraCharacteristics.LensFacing);
                    if (facing == null || facing != Integer.ValueOf((int)lensFacing))
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
                    mImageReader = ImageReader.NewInstance(largest.Width, largest.Height, ImageFormatType.Jpeg, /*maxImages*/2);
                    mImageReader.SetOnImageAvailableListener(mOnImageAvailableListener, mBackgroundHandler);

                    // Find out if we need to swap dimension to get the preview size relative to sensor
                    // coordinate.
                    var displayRotation = Activity.WindowManager.DefaultDisplay.Rotation;

                    //noinspection ConstantConditions
                    mSensorOrientation = (int)characteristics.Get(CameraCharacteristics.SensorOrientation);
                    bool swappedDimensions = false;
                    switch (displayRotation)
                    {
                        case SurfaceOrientation.Rotation0:
                        case SurfaceOrientation.Rotation180:
                            if (mSensorOrientation == 90 || mSensorOrientation == 270)
                            {
                                swappedDimensions = true;
                            }
                            break;

                        case SurfaceOrientation.Rotation90:
                        case SurfaceOrientation.Rotation270:
                            if (mSensorOrientation == 0 || mSensorOrientation == 180)
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
                    mPreviewSize = ChooseOptimalSize(map.GetOutputSizes(Class.FromType(typeof(SurfaceTexture))),
                        rotatedPreviewWidth, rotatedPreviewHeight, maxPreviewWidth,
                        maxPreviewHeight, largest);

                    // We fit the aspect ratio of TextureView to the size of preview we picked.
                    var orientation = Resources.Configuration.Orientation;
                    if (orientation == global::Android.Content.Res.Orientation.Landscape)
                    {
                        mTextureView.SetAspectRatio(mPreviewSize.Width, mPreviewSize.Height);
                    }
                    else
                    {
                        mTextureView.SetAspectRatio(mPreviewSize.Height, mPreviewSize.Width);
                    }

                    // Check if the flash is supported.
                    var available = (Java.Lang.Boolean)characteristics.Get(CameraCharacteristics.FlashInfoAvailable);
                    if (available == null)
                    {
                        mFlashSupported = false;
                    }
                    else
                    {
                        mFlashSupported = (bool)available;
                    }

                    mCameraId = cameraId;
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

        private static global::Android.Util.Size ChooseOptimalSize(global::Android.Util.Size[] choices, int textureViewWidth,
            int textureViewHeight, int maxWidth, int maxHeight, global::Android.Util.Size aspectRatio)
        {
            System.Diagnostics.Debug.WriteLine("Bui: choose optimal size");

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

        private CaptureRequest.Builder stillCaptureBuilder;

        public Semaphore mCameraOpenCloseLock = new Semaphore(1);

        private void CloseCamera()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Bui: close camera");

                mCameraOpenCloseLock.Acquire();
                if (null != mCaptureSession)
                {
                    mCaptureSession.Close();
                    mCaptureSession = null;
                }
                if (null != mCameraDevice)
                {
                    mCameraDevice.Close();
                    mCameraDevice = null;
                }
                if (null != mImageReader)
                {
                    mImageReader.Close();
                    mImageReader = null;
                }
            }
            catch (InterruptedException e)
            {
                throw new RuntimeException("Interrupted while trying to lock camera closing.", e);
            }
            finally
            {
                mCameraOpenCloseLock.Release();
            }
        }

        public void CreateCameraPreviewSession()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Bui: create camera preview session");

                SurfaceTexture texture = mTextureView.SurfaceTexture;
                if (texture == null)
                {
                    throw new IllegalStateException("texture is null");
                }

                // We configure the size of default buffer to be the size of camera preview we want.
                texture.SetDefaultBufferSize(mPreviewSize.Width, mPreviewSize.Height);

                // This is the output Surface we need to start preview.
                Surface surface = new Surface(texture);

                // We set up a CaptureRequest.Builder with the output Surface.
                mPreviewRequestBuilder = mCameraDevice.CreateCaptureRequest(CameraTemplate.Preview);
                mPreviewRequestBuilder.AddTarget(surface);

                // Here, we create a CameraCaptureSession for camera preview.
                List<Surface> surfaces = new List<Surface>();
                surfaces.Add(surface);
                surfaces.Add(mImageReader.Surface);
                mCameraDevice.CreateCaptureSession(surfaces, new CameraCaptureSessionCallback(this), null);
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
                System.Diagnostics.Debug.WriteLine("Bui: capture still pic");

                if (Activity == null || null == mCameraDevice)
                {
                    return;
                }

                // This is the CaptureRequest.Builder that we use to take a picture.
                if (stillCaptureBuilder == null)
                    stillCaptureBuilder = mCameraDevice.CreateCaptureRequest(CameraTemplate.StillCapture);

                stillCaptureBuilder.AddTarget(mImageReader.Surface);

                // Use the same AE and AF modes as the preview.
                stillCaptureBuilder.Set(CaptureRequest.ControlAfMode, (int)ControlAFMode.ContinuousPicture);
                SetAutoFlash(stillCaptureBuilder);

                // Orientation
                int rotation = (int)Activity.WindowManager.DefaultDisplay.Rotation;
                stillCaptureBuilder.Set(CaptureRequest.JpegOrientation, GetOrientation(rotation));

                mCaptureSession.StopRepeating();
                mCaptureSession.Capture(stillCaptureBuilder.Build(), new CameraCaptureStillPictureSessionCallback(this), null);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        private static readonly SparseIntArray ORIENTATIONS = new SparseIntArray();

        private int GetOrientation(int rotation)
        {
            // Sensor orientation is 90 for most devices, or 270 for some devices (eg. Nexus 5X)
            // We have to take that into account and rotate JPEG properly.
            // For devices with orientation of 90, we simply return our mapping from ORIENTATIONS.
            // For devices with orientation of 270, we need to rotate the JPEG 180 degrees.
            return (ORIENTATIONS.Get(rotation) + mSensorOrientation + 270) % 360;
        }

        // Run the precapture sequence for capturing a still image. This method should be called when
        // we get a response in {@link #mCaptureCallback} from {@link #lockFocus()}.
        public void RunPrecaptureSequence()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Bui: run precapture");

                // This is how to tell the camera to trigger.
                mPreviewRequestBuilder.Set(CaptureRequest.ControlAePrecaptureTrigger, (int)ControlAEPrecaptureTrigger.Start);

                // Tell #mCaptureCallback to wait for the precapture sequence to be set.
                mState = STATE_WAITING_PRECAPTURE;
                mCaptureSession.Capture(mPreviewRequestBuilder.Build(), mCaptureCallback, mBackgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        // Starts a background thread and its {@link Handler}.
        private void StartBackgroundThread()
        {
            mBackgroundThread = new HandlerThread("CameraBackground");
            mBackgroundThread.Start();
            mBackgroundHandler = new Handler(mBackgroundThread.Looper);
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
        }

        public void OnActivityDestroyed(Activity activity)
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
            if (mTextureView == null)
            {
                return;
            }

            // When the screen is turned off and turned back on, the SurfaceTexture is already
            // available, and "onSurfaceTextureAvailable" will not be called. In that case, we can open
            // a camera and start preview from here (otherwise, we wait until the surface is ready in
            // the SurfaceTextureListener).
            if (mTextureView.IsAvailable)
            {
                OpenCamera(mTextureView.Width, mTextureView.Height);
            }
            else
            {
                mTextureView.SurfaceTextureListener = mSurfaceTextureListener;
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

        // Stops the background thread and its {@link Handler}.
        private void StopBackgroundThread()
        {
            mBackgroundThread.QuitSafely();
            try
            {
                mBackgroundThread.Join();
                mBackgroundThread = null;
                mBackgroundHandler = null;
            }
            catch (InterruptedException e)
            {
                e.PrintStackTrace();
            }
        }
    }
}