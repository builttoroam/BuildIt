using System;
using System.Collections.Generic;
using System.IO;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Hardware;
using Android.Hardware.Camera2;
using Android.Hardware.Camera2.Params;
using Android.Media;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using BuildIt.AR.FormsSamples.Android;
using BuildIt.AR.FormsSamples.UI.Views;
using Java.Lang;
using Java.Util;
using Java.Util.Concurrent;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using BuildIt.AR.FormsSamples.Android.Listeners;
using Boolean = System.Boolean;
using Exception = System.Exception;
using Orientation = Android.Content.Res.Orientation;
using Point = Android.Graphics.Point;
using Size = Android.Util.Size;
using View = Android.Views.View;
//using FragmentManager = Android.Support.V4.App.FragmentManager;
using FragmentManager = Android.App.FragmentManager;

[assembly:ExportRenderer (typeof(CameraPage), typeof(CameraPageRenderer))]
namespace BuildIt.AR.FormsSamples.Android
{
	public class CameraPageRenderer : PageRenderer, TextureView.ISurfaceTextureListener, ActivityCompat.IOnRequestPermissionsResultCallback, IManageFragments, View.IOnClickListener
    {
        global::Android.Hardware.Camera camera;
        global::Android.Widget.Button takePhotoButton;
        global::Android.Widget.Button toggleFlashButton;
        global::Android.Widget.Button switchCameraButton;
        global::Android.Views.View view;

        Activity activity;
        CameraFacing cameraType;
        TextureView textureView;
        SurfaceTexture surfaceTexture;
        // A {@link Semaphore} to prevent the app from exiting before closing the camera.
        public Semaphore mCameraOpenCloseLock = new Semaphore(1);
        // ID of the current {@link CameraDevice}.
        private string mCameraId;        
        // CameraDevice.StateListener is called when a CameraDevice changes its state
        private CameraStateListener mStateCallback;
        // A {@link Handler} for running tasks in the background.
        public Handler mBackgroundHandler;
        // An {@link ImageReader} that handles still image capture.
        private ImageReader mImageReader;
        // This a callback object for the {@link ImageReader}. "onImageAvailable" will be called when a
        // still image is ready to be saved.
        private ImageAvailableListener mOnImageAvailableListener;
        // Orientation of the camera sensor
        private int mSensorOrientation;
        // Max preview width that is guaranteed by Camera2 API
        private static readonly int MAX_PREVIEW_WIDTH = 1920;
        // Max preview height that is guaranteed by Camera2 API
        private static readonly int MAX_PREVIEW_HEIGHT = 1080;
        // The size of the camera preview
        private Size mPreviewSize;
        // An AutoFitTextureView for camera preview
        private AutoFitTextureView mTextureView;
        // Whether the current camera device supports Flash or not.
        private bool mFlashSupported;
        // An additional thread for running tasks that shouldn't block the UI.
        private HandlerThread mBackgroundThread;

        FragmentManager fragmentManager;

        bool flashOn;
        byte[] imageBytes;

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
            {
                return;
            }

            try
            {
                SetupUserInterface();
                //SetupEventHandlers();
                AddView(view);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@"			ERROR: ", ex.Message);
            }
        }

        void SetupUserInterface()
        {
            activity = this.Context as Activity;
            view = activity.LayoutInflater.Inflate(Resource.Layout.activity_camera, this, false);
            cameraType = CameraFacing.Back;
            FragmentManager.BeginTransaction().Replace(Resource.Id.container, Camera2BasicFragment.NewInstance()).Commit();

            //textureView = view.FindViewById<TextureView>(Resource.Id.textureView);
            //textureView.SurfaceTextureListener = this;
        }

        void SetupEventHandlers()
        {
            takePhotoButton = view.FindViewById<global::Android.Widget.Button>(Resource.Id.takePhotoButton);
            takePhotoButton.Click += TakePhotoButtonTapped;

            switchCameraButton = view.FindViewById<global::Android.Widget.Button>(Resource.Id.switchCameraButton);
            switchCameraButton.Click += SwitchCameraButtonTapped;

            toggleFlashButton = view.FindViewById<global::Android.Widget.Button>(Resource.Id.toggleFlashButton);
            toggleFlashButton.Click += ToggleFlashButtonTapped;
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            var msw = MeasureSpec.MakeMeasureSpec(r - l, MeasureSpecMode.Exactly);
            var msh = MeasureSpec.MakeMeasureSpec(b - t, MeasureSpecMode.Exactly);

            view.Measure(msw, msh);
            view.Layout(0, 0, r - l, b - t);
        }

        public void OnSurfaceTextureUpdated(SurfaceTexture surface)
        {

        }

        public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
        {
            camera = global::Android.Hardware.Camera.Open((int)cameraType);

            var manager = (CameraManager)activity.GetSystemService(Context.CameraService);
            try
            {
                for (var i = 0; i < manager.GetCameraIdList().Length; i++)
                {
                    var cameraId = manager.GetCameraIdList()[i];
                    CameraCharacteristics characteristics = manager.GetCameraCharacteristics(cameraId);

                    // We don't use a front facing camera in this sample.
                    var facing = (Integer)characteristics.Get(CameraCharacteristics.LensFacing);
                    if (facing != null && facing == (Integer.ValueOf((int)LensFacing.Front)))
                    {
                        continue;
                    }

                    var map = (StreamConfigurationMap)characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap);
                    if (map == null)
                    {
                        continue;
                    }

                    // For still image captures, we use the largest available size.
                    Size largest = (Size)Collections.Max(Arrays.AsList(map.GetOutputSizes((int)ImageFormatType.Jpeg)),
                        new CompareSizesByArea());
                    mImageReader = ImageReader.NewInstance(largest.Width, largest.Height, ImageFormatType.Jpeg, /*maxImages*/2);
                    mImageReader.SetOnImageAvailableListener(mOnImageAvailableListener, mBackgroundHandler);

                    // Find out if we need to swap dimension to get the preview size relative to sensor
                    // coordinate.
                    var displayRotation = activity.WindowManager.DefaultDisplay.Rotation;
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
                            Console.WriteLine("Display rotation is invalid: " + displayRotation);
                            break;
                    }

                    Point displaySize = new Point();
                    activity.WindowManager.DefaultDisplay.GetSize(displaySize);
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
                    mTextureView = (AutoFitTextureView)view.FindViewById(Resource.Id.texture);
                    if (orientation == Orientation.Landscape)
                    {
                        mTextureView.SetAspectRatio(mPreviewSize.Width, mPreviewSize.Height);
                    }
                    else
                    {
                        mTextureView.SetAspectRatio(mPreviewSize.Height, mPreviewSize.Width);
                    }

                    // Check if the flash is supported.
                    var available = (Boolean)characteristics.Get(CameraCharacteristics.FlashInfoAvailable);
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

                //mStateCallback = new CameraStateListener() { owner = Camera2BasicFragment };

                mBackgroundThread = new HandlerThread("CameraBackground");
                mBackgroundThread.Start();
                mBackgroundHandler = new Handler(mBackgroundThread.Looper);

                if (!mCameraOpenCloseLock.TryAcquire(2500, TimeUnit.Microseconds))
                {
                    throw new RuntimeException("Time out waiting to lock camera opening.");
                }
                manager.OpenCamera(mCameraId, mStateCallback, mBackgroundHandler);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
            textureView.LayoutParameters = new FrameLayout.LayoutParams(width, height);
            surfaceTexture = surface;

            camera.SetPreviewTexture(surface);
            PrepareAndStartCamera();
        }

        private static Size ChooseOptimalSize(Size[] choices, int textureViewWidth,
            int textureViewHeight, int maxWidth, int maxHeight, Size aspectRatio)
        {
            // Collect the supported resolutions that are at least as big as the preview Surface
            var bigEnough = new List<Size>();
            // Collect the supported resolutions that are smaller than the preview Surface
            var notBigEnough = new List<Size>();
            int w = aspectRatio.Width;
            int h = aspectRatio.Height;

            for (var i = 0; i < choices.Length; i++)
            {
                Size option = choices[i];
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
                return (Size)Collections.Min(bigEnough, new CompareSizesByArea());
            }
            else if (notBigEnough.Count > 0)
            {
                return (Size)Collections.Max(notBigEnough, new CompareSizesByArea());
            }
            else
            {
                Console.WriteLine("Couldn't find any suitable preview size");
                return choices[0];
            }
        }

        public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
        {
            camera.StopPreview();
            camera.Release();
            return true;
        }

        public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
        {
            PrepareAndStartCamera();
        }

        void PrepareAndStartCamera()
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

            camera.StartPreview();
        }

        void ToggleFlashButtonTapped(object sender, EventArgs e)
        {
            flashOn = !flashOn;
            if (flashOn)
            {
                if (cameraType == CameraFacing.Back)
                {
                    toggleFlashButton.SetBackgroundResource(Resource.Drawable.FlashButton);
                    cameraType = CameraFacing.Back;

                    camera.StopPreview();
                    camera.Release();
                    camera = global::Android.Hardware.Camera.Open((int)cameraType);
                    var parameters = camera.GetParameters();
                    parameters.FlashMode = global::Android.Hardware.Camera.Parameters.FlashModeTorch;
                    camera.SetParameters(parameters);
                    camera.SetPreviewTexture(surfaceTexture);
                    PrepareAndStartCamera();
                }
            }
            else
            {
                toggleFlashButton.SetBackgroundResource(Resource.Drawable.NoFlashButton);
                camera.StopPreview();
                camera.Release();

                camera = global::Android.Hardware.Camera.Open((int)cameraType);
                var parameters = camera.GetParameters();
                parameters.FlashMode = global::Android.Hardware.Camera.Parameters.FlashModeOff;
                camera.SetParameters(parameters);
                camera.SetPreviewTexture(surfaceTexture);
                PrepareAndStartCamera();
            }
        }

        void SwitchCameraButtonTapped(object sender, EventArgs e)
        {
            if (cameraType == CameraFacing.Front)
            {
                cameraType = CameraFacing.Back;

                camera.StopPreview();
                camera.Release();
                camera = global::Android.Hardware.Camera.Open((int)cameraType);
                camera.SetPreviewTexture(surfaceTexture);
                PrepareAndStartCamera();
            }
            else
            {
                cameraType = CameraFacing.Front;

                camera.StopPreview();
                camera.Release();
                camera = global::Android.Hardware.Camera.Open((int)cameraType);
                camera.SetPreviewTexture(surfaceTexture);
                PrepareAndStartCamera();
            }
        }

        async void TakePhotoButtonTapped(object sender, EventArgs e)
        {
            camera.StopPreview();

            var image = textureView.Bitmap;

            try
            {
                var absolutePath = global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryDcim).AbsolutePath;
                var folderPath = absolutePath + "/Camera";
                var filePath = System.IO.Path.Combine(folderPath, string.Format("photo_{0}.jpg", Guid.NewGuid()));

                var fileStream = new FileStream(filePath, FileMode.Create);
                await image.CompressAsync(Bitmap.CompressFormat.Jpeg, 50, fileStream);
                fileStream.Close();
                image.Recycle();

                var intent = new global::Android.Content.Intent(global::Android.Content.Intent.ActionMediaScannerScanFile);
                var file = new Java.IO.File(filePath);
                var uri = global::Android.Net.Uri.FromFile(file);
                intent.SetData(uri);
                Forms.Context.SendBroadcast(intent);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@"				", ex.Message);
            }

            camera.StartPreview();
        }

        public void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            throw new NotImplementedException();
        }

        FragmentManager FragmentManager => fragmentManager ?? (fragmentManager = ((FormsAppCompatActivity)Context).FragmentManager);

        public void SetFragmentManager(FragmentManager childFragmentManager)
        {
            if (fragmentManager == null)
            {
                this.fragmentManager = childFragmentManager;
            }
        }
    }
}

