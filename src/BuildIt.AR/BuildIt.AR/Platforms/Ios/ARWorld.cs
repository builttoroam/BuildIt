using CoreMotion;
using Foundation;
using System;
using System.Collections.Generic;
using System.Threading;
using UIKit;

namespace BuildIt.AR.iOS
{
    public class ARWorld<T> where T : ILocationBasedMarker
    {
        private CMMotionManager motion;
        private ScreenWorld world;

        private readonly Action<float, float, float> updateElementsOnScreen;
        private UIView view;

        public Rotation Rotation { get; private set; }
        public double VisualRangeKm { get; }

        private int updating;

        public IEnumerable<IWorldElement<T>> Elements => world?.ElementsInWorld<T>();

        public ARWorld(UIView view, double visualRangeKm, Action<float, float, float> updateElementsOnScreen)
        {
            this.view = view;
            VisualRangeKm = visualRangeKm;
            this.updateElementsOnScreen = updateElementsOnScreen;
        }

        public void Initialize(List<T> elements)
        {
            CalculateRotation();
            world = new ScreenWorld(WorldConfiguration.iOS, Rotation);
            world.Initialize(view.Bounds.Width, view.Bounds.Height);
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
            var currentOrientation = UIApplication.SharedApplication.StatusBarOrientation;
            var offset = currentOrientation.IsLandscape() ? world.Offset(element, new Rectangle(0, 0, markerWidth, markerHeight), roll, pitch, yaw) : world.Offset(element, new Rectangle(0, 0, markerWidth, markerHeight), pitch, roll, yaw);
            return offset;
        }

        public void StartSensors()
        {
            motion = new CMMotionManager();
            motion.StartDeviceMotionUpdates(CMAttitudeReferenceFrame.XMagneticNorthZVertical, NSOperationQueue.CurrentQueue, MotionHandler);
        }

        private void MotionHandler(CMDeviceMotion data, NSError error)
        {
            try
            {
                if (Interlocked.CompareExchange(ref updating, 1, 0) == 1)
                {
                    return;
                }

                view.InvokeOnMainThread(() =>
                {
                    if (data != null)
                    {
                        updateElementsOnScreen?.Invoke((float)data.Attitude.Roll, (float)data.Attitude.Pitch, (float)data.Attitude.Yaw);
                        Interlocked.Exchange(ref updating, 0);
                    }
                });
            }
            catch (Exception)
            {
            }
        }

        public void StopSensors()
        {
            motion?.StopDeviceMotionUpdates();
        }

        public void UpdateRotation()
        {
            CalculateRotation();
            world.UpdateWorldAdjustment(Rotation);
            world.Initialize(view.Bounds.Width, view.Bounds.Height);
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
            Rotation = UIApplication.SharedApplication.StatusBarOrientation.ToRotationEnum();
        }
    }
}