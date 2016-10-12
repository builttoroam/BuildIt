using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Hardware;
using Android.Locations;
using Android.OS;
using Android.Views;
using Android.Widget;
using BuildIt.AR.Helpers;

namespace BuildIt.AR.Android
{
    public class ARWorld<T> : Java.Lang.Object, ISensorEventListener where T : ILocationBasedMarker
    {
        private readonly Activity activity;
        private readonly float lowPassFilterThreshold;
        private SensorManager sensorManager;
        private Sensor accelerometer;
        private Sensor magnetometer;
        private float[] gravity;
        private float[] geomagnetic;
        private int updating;
        private Rotation rotation;
        private ScreenWorld world;
        private RelativeLayout markerLayout;
        private LocationManager locationManager;



        public ARWorld(Activity activity, RelativeLayout markerLayout, float lowPassFilterThreshold)
        {
            this.activity = activity;
            this.lowPassFilterThreshold = lowPassFilterThreshold;
            this.markerLayout = markerLayout;
        }

        public void Initialize(List<T> elements)
        {
            CalculateRotation();
            world = new ScreenWorld(WorldConfiguration.Android, rotation);
            world.Initialize(activity.Resources.DisplayMetrics.WidthPixels, activity.Resources.DisplayMetrics.HeightPixels);
            if (elements != null)
            {
                foreach (var element in elements)
                {
                    world.AddElementToWorld(element);
                }
            }
        }

        public void StartSensors()
        {
            sensorManager = activity.GetSystemService(Context.SensorService) as SensorManager;
            accelerometer = sensorManager?.GetDefaultSensor(SensorType.Accelerometer);
            magnetometer = sensorManager?.GetDefaultSensor(SensorType.MagneticField);
            if (accelerometer != null)
            {
                sensorManager?.RegisterListener(this, accelerometer, SensorDelay.Ui);
            }
            if (magnetometer != null)
            {
                sensorManager?.RegisterListener(this, magnetometer, SensorDelay.Ui);
            }
        }

        public void StopSensors()
        {
            if (magnetometer != null)
            {
                sensorManager?.UnregisterListener(this, magnetometer);
            }
            if (accelerometer != null)
            {
                sensorManager?.UnregisterListener(this, accelerometer);
            }
        }

        public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
        {

        }

        public void OnSensorChanged(SensorEvent evt)
        {
            try
            {
                if (evt.Sensor.Type == SensorType.Accelerometer)
                    gravity = FilterHelper.LowPassFilter(lowPassFilterThreshold, evt.Values.ToArray(), gravity);
                if (evt.Sensor.Type == SensorType.MagneticField)
                    geomagnetic = FilterHelper.LowPassFilter(lowPassFilterThreshold, evt.Values.ToArray(), geomagnetic);
                if (gravity == null || geomagnetic == null) return;
                var rotationMatrix = new float[16];
                var success = SensorManager.GetRotationMatrix(rotationMatrix, null, gravity, geomagnetic);
                if (!success) return;
                var orientation = new float[3];
                SensorManager.GetOrientation(rotationMatrix, orientation);

                if (Interlocked.CompareExchange(ref updating, 1, 0) == 1) return;
                activity.RunOnUiThread(() =>
                {
                    //Debug.WriteLine($"roll {orientation[0]}, pitch {orientation[1]}, yaw {orientation[2]}");
                    if (rotation == Rotation.Rotation90 || rotation == Rotation.Rotation270)
                    {
                        //UpdateElementsOnScreen(orientation[2], orientation[1], orientation[0]);
                    }
                    else
                    {
                        //UpdateElementsOnScreen(orientation[1], orientation[2], orientation[0]);
                    }
                    Interlocked.Exchange(ref updating, 0);
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public void UpdateRotation()
        {
            CalculateRotation();
            world.UpdateWorldAdjustment(rotation);
            world.Initialize(activity.Resources.DisplayMetrics.WidthPixels, activity.Resources.DisplayMetrics.HeightPixels);
        }

        public void UpdateLocation(Location location)
        {
            world.UpdateCentre(location);
            foreach (var worldElement in world.ElementsInWorld<T>())
            {
                worldElement.Element.Distance = worldElement.Element.GeoLocation.DistanceInMetres(location);
            }
        }

        /*private void UpdateElementsOnScreen(float roll, float pitch, float yaw)
        {
            try
            {


                var cnt = markerLayout.ChildCount;
                for (int i = 0; i < cnt; i++)
                {
                    var fe = markerLayout.GetChildAt(i) as T;
                    if (fe == null)
                        continue;
                    if (!events.ContainsKey(fe))
                    {
                        continue;
                    }
                    var element = events[fe];
                    if (element == null)
                    {
                        continue;
                    }
                    if (fe.POI.Distance > world.VisualRangeKm * 1000)
                    {
                        continue;
                    }

                    var offset = world.Offset(element, new Rectangle(0, 0, fe.Width, fe.Height), roll, pitch, yaw);
                    if (offset == null)
                    {
                        continue;
                    }
                    var finalX = (int)offset.TranslateX + fe.Left;
                    var finalY = (int)offset.TranslateY + fe.Top;
                    if (finalX >= 0 && finalX < arMarkerLayout.Width && finalY >= 0 && finalY < arMarkerLayout.Height)
                    {

                        fe.DistanceText.Text = element.Element.Distance.ToString();
                        fe.TitleText.Text = element.Element.Name;
                        fe.TranslationX = (float)offset.TranslateX;
                        fe.SetY(activity.Resources.DisplayMetrics.HeightPixels / 2);
                        fe.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        fe.Visibility = ViewStates.Gone;
                    }

                    var scale = world.CalculateScale(element.Element.Distance);
                    fe.ScaleX = (float)scale;
                    fe.ScaleY = (float)scale;
                }
            }
            catch (System.Exception ex)
            {
            }
        }*/

        private void CalculateRotation()
        {
            rotation = Rotation.Rotation0;

            switch (activity.WindowManager.DefaultDisplay.Rotation)
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
    }
}