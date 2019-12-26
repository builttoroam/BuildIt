using Android.App;
using Android.Content;
using Android.Hardware;
using Android.Locations;
using Android.Views;
using BuildIt.AR.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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

        private ScreenWorld world;
        private readonly Action<float, float, float> updateElementsOnScreen;

        public Rotation Rotation { get; private set; }
        public double VisualRangeKm { get; }

        public IEnumerable<IWorldElement<T>> Elements => world?.ElementsInWorld<T>();

        public ARWorld(Activity activity, float lowPassFilterThreshold, double visualRangeKm, Action<float, float, float> updateElementsOnScreen)
        {
            this.activity = activity;
            this.lowPassFilterThreshold = lowPassFilterThreshold;
            VisualRangeKm = visualRangeKm;
            this.updateElementsOnScreen = updateElementsOnScreen;
        }

        public void Initialize(List<T> elements)
        {
            CalculateRotation();
            world = new ScreenWorld(WorldConfiguration.Android, Rotation);
            world.Initialize(activity.Resources.DisplayMetrics.WidthPixels, activity.Resources.DisplayMetrics.HeightPixels);
            if (elements != null)
            {
                foreach (var element in elements)
                {
                    world.AddElementToWorld(element);
                }
            }

            world.UpdateRangeOfWorld(VisualRangeKm);
        }

        public ScreenOffset CalculateOffset(IWorldElement<T> element, int markerWidth, int markerHeight, double roll, double pitch, double yaw)
        {
            var offset = world.Offset(element, new Rectangle(0, 0, markerWidth, markerHeight), roll, pitch, yaw);
            if (offset != null)
            {
                offset.Scale = world.CalculateScale(element.Element.DistanceMetres);
            }

            return offset;
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
                {
                    gravity = FilterHelper.LowPassFilter(lowPassFilterThreshold, evt.Values.ToArray(), gravity);
                }

                if (evt.Sensor.Type == SensorType.MagneticField)
                {
                    geomagnetic = FilterHelper.LowPassFilter(lowPassFilterThreshold, evt.Values.ToArray(), geomagnetic);
                }

                if (gravity == null || geomagnetic == null)
                {
                    return;
                }

                var rotationMatrix = new float[16];
                var success = SensorManager.GetRotationMatrix(rotationMatrix, null, gravity, geomagnetic);
                if (!success)
                {
                    return;
                }

                var orientation = new float[3];
                SensorManager.GetOrientation(rotationMatrix, orientation);

                if (Interlocked.CompareExchange(ref updating, 1, 0) == 1)
                {
                    return;
                }

                activity.RunOnUiThread(() =>
                {
                    if (Rotation == Rotation.Rotation90 || Rotation == Rotation.Rotation270)
                    {
                        updateElementsOnScreen?.Invoke(orientation[2], orientation[1], orientation[0]);
                    }
                    else
                    {
                        updateElementsOnScreen?.Invoke(orientation[1], orientation[2], orientation[0]);
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
            world.UpdateWorldAdjustment(Rotation);
            world.Initialize(activity.Resources.DisplayMetrics.WidthPixels, activity.Resources.DisplayMetrics.HeightPixels);
        }

        public void UpdateLocation(Location location)
        {
            world.UpdateCentre(location);
            foreach (var worldElement in world.ElementsInWorld<T>())
            {
                worldElement.Element.DistanceMetres = worldElement.Element.GeoLocation.DistanceInMetres(location);
            }
        }

        private void CalculateRotation()
        {
            Rotation = Rotation.Rotation0;

            switch (activity.WindowManager.DefaultDisplay.Rotation)
            {
                case SurfaceOrientation.Rotation0:
                    Rotation = Rotation.Rotation0;
                    break;

                case SurfaceOrientation.Rotation90:
                    Rotation = Rotation.Rotation90;
                    break;

                case SurfaceOrientation.Rotation180:
                    Rotation = Rotation.Rotation180;
                    break;

                case SurfaceOrientation.Rotation270:
                    Rotation = Rotation.Rotation270;
                    break;
            }
        }
    }
}