using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Hardware;
using Android.Hardware.Camera2;
using Android.Locations;
using Android.Widget;
using Android.OS;
using Android.Views;
using BuildIt.AR;
using BuildIt.AR.Android;
using BuildIt.AR.Android.Controls;
using BuildIt.AR.Helpers;
using BuildItArSample.Android.Controls;
using Java.Lang;
using Debug = System.Diagnostics.Debug;
using Location = BuildIt.AR.Location;
using BuildItArSample.Core;

namespace BuildItArSample.Android
{
    [Activity(Label = "BuildItArSample.Android", MainLauncher = true, Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class MainActivity : Activity, ILocationListener
    {
        private int numberOfCameras;
        private RelativeLayout arMarkerLayout;
        private Camera camera;
        private CameraPreview cameraPreview;
        private Rotation rotation;
        //private ScreenWorld world;
        private ARWorld<POI> world;
        private SensorManager sensorManager;
        private Sensor accelerometer;
        private Sensor magnetometer;
        private float[] gravity;
        private float[] geomagnetic;
        private int updating;
        private float filterThreshold = 0.1f;
        private LocationManager locationManager;
        //private readonly IDictionary<ArMarker, POI> events = new Dictionary<ArMarker, POI>();
        private readonly IDictionary<POI, ArMarker> poiMarkers = new Dictionary<POI, ArMarker>();
        private List<POI> pois;

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            UpdateCameraDisplayOrientation();
            world.UpdateRotation();
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.Main);


            arMarkerLayout = FindViewById<RelativeLayout>(Resource.Id.arMarkerOverlay);
            world = new ARWorld<POI>(this, 0.1f, 50, UpdateElementsOnScreen);
            pois = new List<POI>()
            {
                new POI
                {
                    GeoLocation = new Location
                    {
                        Latitude = -33.832855,
                        Longitude = 151.211989
                    },
                    Id = 1,
                    Name = "North"
                },
                new POI
                {
                    GeoLocation = new Location
                    {
                        Latitude = -33.839878,
                        Longitude = 151.220633
                    },
                    Id = 2,
                    Name = "East"
                },
                new POI
                {
                    GeoLocation = new Location
                    {
                        Latitude = -33.839309,
                        Longitude = 151.195384
                    },
                    Id = 3,
                    Name = "West"
                },
                new POI
                {
                    GeoLocation = new Location
                    {
                        Latitude = -33.848870,
                        Longitude = 151.212342
                    },
                    Name = "South",
                    Id = 4
                }
            };
            world.Initialize(pois);
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
            locationManager = GetSystemService(LocationService) as LocationManager;
            
            if (locationManager != null)
            {
                var provider = locationManager.GetBestProvider(new Criteria(), true);
                locationManager.RequestLocationUpdates(provider, 50, 0, this);
            }

            world.StartSensors();
        }


        private void PopulateWorld()
        {
            foreach (var poi in pois)
            {
                var arMarker = new ArMarker(this, poi)
                {
                    Visibility = ViewStates.Gone
                };
                poiMarkers[poi] = arMarker;
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
            world?.StopSensors();
            locationManager?.RemoveUpdates(this);
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
            Debug.WriteLine($"rotation {rotation}");
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


        private void UpdateElementsOnScreen(float roll, float pitch, float yaw)
        {
            try
            {
                if (world?.Elements == null)
                {
                    return;
                }

                foreach (var element in world.Elements)
                {
                    var poiMarker = poiMarkers[element.Element];
                    if (element.Element.DistanceKm > world.VisualRangeKm)
                    {
                        poiMarker.Visibility = ViewStates.Gone;
                        continue;
                    }

                    var offset = world.CalculateOffset(element, poiMarker.Width, poiMarker.Height, roll, pitch, yaw);
                    if (offset == null)
                    {
                        continue;
                    }
                    var finalX = (int)offset.TranslateX + poiMarker.Left;
                    var finalY = (int)offset.TranslateY + poiMarker.Top;
                    if (finalX >= 0 && finalX < arMarkerLayout.Width && finalY >= 0 && finalY < arMarkerLayout.Height)
                    {

                        poiMarker.DistanceText.Text = element.Element.DistanceMetres.ToString();
                        poiMarker.TitleText.Text = element.Element.Name;
                        poiMarker.TranslationX = (float)offset.TranslateX;
                        poiMarker.SetY(Resources.DisplayMetrics.HeightPixels / 2);
                        poiMarker.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        poiMarker.Visibility = ViewStates.Gone;
                    }


                    poiMarker.ScaleX = (float)offset.Scale;
                    poiMarker.ScaleY = (float)offset.Scale;
                }

            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void OnLocationChanged(global::Android.Locations.Location location)
        {
            var position = new Location {Latitude = location.Latitude, Longitude = location.Longitude};
            world?.UpdateLocation(position);
        }

        public void OnProviderDisabled(string provider)
        {

        }

        public void OnProviderEnabled(string provider)
        {
        
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
  
        }
    }
}

