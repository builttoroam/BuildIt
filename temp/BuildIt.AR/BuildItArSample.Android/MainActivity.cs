using System.Linq;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Hardware;
using Android.Hardware.Camera2;
using Android.Widget;
using Android.OS;
using Android.Views;
using BuildIt.AR;
using BuildIt.AR.Helpers;
using BuildItArSample.Android.Controls;
using Java.Lang;

namespace BuildItArSample.Android
{
    [Activity(Label = "BuildItArSample.Android", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, ISensorEventListener
    {
        private int numberOfCameras;
        private RelativeLayout arMarkerLayout;
        private Camera camera;
        private CameraPreview cameraPreview;
        private Rotation rotation;
        private ScreenWorld world;
        private SensorManager sensorManager;
        private Sensor accelerometer;
        private Sensor magnetometer;
        private float[] gravity;
        private float[] geomagnetic;
        private int updating;
        private float filterThreshold = 0.1f;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            arMarkerLayout = FindViewById<RelativeLayout>(Resource.Id.arMarkerOverlay);
            CalculateRotation();
            InitializeWorld();
            PopulateWorld();
        }

        protected override void OnResume()
        {
            base.OnResume();
            cameraPreview = FindViewById<CameraPreview>(Resource.Id.texture);
            cameraPreview?.InitPreview(this);
            OpenCamera();
            InitSensors();
        }

        private void InitSensors()
        {
            sensorManager = GetSystemService(Context.SensorService) as SensorManager;
            accelerometer = sensorManager?.GetDefaultSensor(SensorType.Accelerometer);
            magnetometer = sensorManager?.GetDefaultSensor(SensorType.MagneticField);
            var hasRequiredSensors = true;
            if (accelerometer != null)
            {
                sensorManager?.RegisterListener(this, accelerometer, SensorDelay.Ui);
            }
            else
            {
                hasRequiredSensors = false;
            }
            if (magnetometer != null)
            {
                sensorManager?.RegisterListener(this, magnetometer, SensorDelay.Ui);
            }
            else
            {
                hasRequiredSensors = false;
            }
            if (!hasRequiredSensors)
            {
            }
        }

        private void InitializeWorld()
        {
            world = new ScreenWorld(WorldConfiguration.Android, rotation);
            world.Initialize(Resources.DisplayMetrics.WidthPixels, Resources.DisplayMetrics.HeightPixels);
            world.AddElementToWorld(new POI() {GeoLocation = new Location() {Latitude = -33.832855, Longitude = 151.211989}});
            world.AddElementToWorld(new POI() {GeoLocation = new Location() {Latitude = -33.848870, Longitude = 151.212342}});
            world.AddElementToWorld(new POI()
            {
                GeoLocation = new Location()
                {
                    Latitude = -33.861499,
                    Longitude = 150.858273
                }
            });
            world.AddElementToWorld(new POI()
            {
                GeoLocation = new Location()
                {
                    Latitude = -33.863479,
                    Longitude = 150.923031
                }
            });
        }

        private void PopulateWorld()
        {
            foreach (var evt in world.ElementsInWorld<POI>())
            {
                var arMarker = new ArMarker(this)
                {
                    Visibility = ViewStates.Gone
                };
                arMarkerLayout.AddView(arMarker);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            if (camera != null)
            {
                camera.StopPreview();
                if (cameraPreview != null)
                {
                    cameraPreview.PreviewCamera = null;
                }
                camera.Release();
                camera = null;
            }
            if (magnetometer != null)
            {
                sensorManager?.UnregisterListener(this, magnetometer);
            }
            if (accelerometer != null)
            {
                sensorManager?.UnregisterListener(this, accelerometer);
            }
        }

        private void OpenCamera()
        {
            try
            {
                numberOfCameras = Camera.NumberOfCameras;
                int? rearFacingCameraId = null;
                // Find the ID of the default camera
                Camera.CameraInfo cameraInfo = new Camera.CameraInfo();
                for (int i = 0; i < numberOfCameras; i++)
                {
                    Camera.GetCameraInfo(i, cameraInfo);
                    if (cameraInfo.Facing == CameraFacing.Back)
                    {
                        rearFacingCameraId = i;
                    }
                }
                if (rearFacingCameraId.HasValue)
                {
                    camera = Camera.Open(rearFacingCameraId.Value);
                    UpdateCameraDisplayOrientation();
                    if (cameraPreview != null)
                    {
                        cameraPreview.PreviewCamera = camera;
                    }
                }
            }
            catch (CameraAccessException ex)
            {
            }
            catch (NullPointerException)
            {
            }
            catch (System.Exception ex)
            {
            }
        }

        private void CalculateRotation()
        {
            rotation = Rotation.Rotation0;

            switch (WindowManager.DefaultDisplay.Rotation)
            {
                case SurfaceOrientation.Rotation0:
                    rotation = Rotation.Rotation0;
                    break;
                case SurfaceOrientation.Rotation90:
                    rotation = Rotation.Rotation90;
                    break;
                case SurfaceOrientation.Rotation180:
                    rotation = Rotation.Rotation180;
                    break;
                case SurfaceOrientation.Rotation270:
                    rotation = Rotation.Rotation270;
                    break;
            }
        }


        private void UpdateCameraDisplayOrientation()
        {
            CalculateRotation();
            var angle = 0;
            switch (rotation)
            {
                case Rotation.Rotation0:
                    angle = 90;
                    break;
                case Rotation.Rotation90:
                    break;
                case Rotation.Rotation180:
                    angle = 270;
                    break;
                case Rotation.Rotation270:
                    angle = 180;
                    break;
            }

            camera?.SetDisplayOrientation(angle);
        }

        public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
        {

        }

        public void OnSensorChanged(SensorEvent evt)
        {
            try
            {
                if (evt.Sensor.Type == SensorType.Accelerometer)
                    gravity = FilterHelper.LowPassFilter(filterThreshold, evt.Values.ToArray(), gravity);
                if (evt.Sensor.Type == SensorType.MagneticField)
                    geomagnetic = FilterHelper.LowPassFilter(filterThreshold, evt.Values.ToArray(), geomagnetic);
                if (gravity != null && geomagnetic != null && camera != null)
                {
                    var rotationMatrix = new float[16];
                    var success = SensorManager.GetRotationMatrix(rotationMatrix, null, gravity, geomagnetic);
                    if (success)
                    {
                        var orientation = new float[3];
                        SensorManager.GetOrientation(rotationMatrix, orientation);

                        if (Interlocked.CompareExchange(ref updating, 1, 0) == 1) return;
                        RunOnUiThread(() =>
                        {

                            if (rotation == Rotation.Rotation90 || rotation == Rotation.Rotation270)
                            {
                                UpdateElementsOnScreen(arMarkerLayout, orientation[2], orientation[1], orientation[0]);
                            }
                            else
                            {
                                UpdateElementsOnScreen(arMarkerLayout, orientation[1], orientation[2], orientation[0]);
                            }
                            Interlocked.Exchange(ref updating, 0);
                        });
                    }
                }
            }
            catch (System.Exception ex)
            {

            }
        }

        private void UpdateElementsOnScreen(RelativeLayout arMarkerLayout, float roll, float pitch, float yaw)
        {

        }
    }

    public class POI : ILocationBasedMarker
    {
        public Location GeoLocation { get; set; }
        public double Distance { get; set; }
    }
}

