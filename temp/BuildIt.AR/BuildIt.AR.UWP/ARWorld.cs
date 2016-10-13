using System;
using System.Collections.Generic;
using System.Threading;
using Windows.Devices.Sensors;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace BuildIt.AR.UWP
{
    public class ARWorld<T> where T : ILocationBasedMarker
    {
        private readonly DisplayInformation displayInformation = DisplayInformation.GetForCurrentView();
        private int updating;

        private ScreenWorld world;
        private Inclinometer inclinometer;
        private readonly Action<double, double, double> updateElementsOnScreen;
        private readonly Page page;

        public Rotation Rotation { get; private set; }

        public double VisualRangeKm { get; }

        public IEnumerable<IWorldElement<T>> Elements => world?.ElementsInWorld<T>();

        public ARWorld(Page page, double visualRangeKm, Action<double, double, double> updateElementsOnScreen)
        {
            this.page = page;
            VisualRangeKm = visualRangeKm;
            this.updateElementsOnScreen = updateElementsOnScreen;
        }

        public void Initialize(List<T> elements)
        {
            CalculateRotation();
            world = new ScreenWorld(WorldConfiguration.UWP, Rotation);
            world.Initialize(page.ActualWidth, page.ActualHeight);
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
            page.SizeChanged += Page_SizeChanged;
            inclinometer = Inclinometer.GetDefault();
            if (inclinometer != null)
            {
                inclinometer.ReadingChanged += Inclinometer_ReadingChanged;
                inclinometer.ReportInterval = 1;
            }
        }

        private void Page_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            world?.Initialize(page.ActualWidth, page.ActualHeight);
        }

        private void Inclinometer_ReadingChanged(Inclinometer sender, InclinometerReadingChangedEventArgs args)
        {
            if (Interlocked.CompareExchange(ref updating, 1, 0) == 1) return;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            page.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var roll = args.Reading.RollDegrees * Math.PI / 180.0;
                var pitch = args.Reading.PitchDegrees * Math.PI / 180.0;
                var yaw = args.Reading.YawDegrees * Math.PI / 180.0;
                if (Rotation == Rotation.Rotation90 || Rotation == Rotation.Rotation270)
                {
                    updateElementsOnScreen?.Invoke(roll, pitch, yaw);
                }
                else
                {
                    updateElementsOnScreen?.Invoke(pitch, roll, yaw);
                }
                Interlocked.Exchange(ref updating, 0);
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        public void StopSensors()
        {
            page.SizeChanged -= Page_SizeChanged;
            if (inclinometer != null)
            {
                inclinometer.ReadingChanged -= Inclinometer_ReadingChanged;
            }
        }

        public void UpdateRotation()
        {
            CalculateRotation();
            world.UpdateWorldAdjustment(Rotation);
            world.Initialize(page.ActualWidth, page.ActualHeight);
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

            switch (displayInformation.CurrentOrientation)
            {
                case DisplayOrientations.None:
                case DisplayOrientations.Portrait:
                    break;
                case DisplayOrientations.Landscape:
                    Rotation = Rotation.Rotation90;
                    break;
                case DisplayOrientations.LandscapeFlipped:
                    Rotation = Rotation.Rotation270;
                    break;
                case DisplayOrientations.PortraitFlipped:
                    Rotation = Rotation.Rotation180;
                    break;

            }
        }
    }
}
