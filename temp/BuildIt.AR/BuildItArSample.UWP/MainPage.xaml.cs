using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Enumeration;
using Windows.Devices.Geolocation;
using Windows.Devices.Sensors;
using Windows.Graphics.Display;
using Windows.Media.Capture;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using BuildIt.AR;
using BuildIt.AR.UWP;
using BuildIt.AR.UWP.Extensions;
using BuildItArSample.Core;
using Panel = Windows.Devices.Enumeration.Panel;


namespace BuildItArSample.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        MediaCapture mediaCapture;
        bool isPreviewing;
        DisplayRequest displayRequest = new DisplayRequest();
        private Geolocator geolocator;
        private Inclinometer inclinometer;
        //private ScreenWorld world;
        private ARWorld<POI> world;
        private IDictionary<POI, TextBlock> markers = new Dictionary<POI, TextBlock>();
        private int updating;
        List<POI> pois = new List<POI>
            {
                new POI {GeoLocation = new Location {Latitude = -33.832855,
                    Longitude = 151.211989}, Id = 1, Name = "North"}, new POI {GeoLocation = new Location {Latitude = -33.839878,
                    Longitude = 151.220633}, Id = 2, Name = "East"}, new POI
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

        private Rotation displayRotation;


        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            Application.Current.Suspending += Application_Suspending;
            Application.Current.Resuming += Application_Resuming;
        }

        private void Application_Resuming(object sender, object e)
        {
            world?.StartSensors();
        }

        private void Application_Suspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            try
            {
                world?.StopSensors();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                deferral.Complete();
            }
        }

        

        /// <summary>
        /// Queries the available video capture devices to try and find one mounted on the desired panel
        /// </summary>
        /// <param name="desiredPanel">The panel on the device that the desired camera is mounted on</param>
        /// <returns>A DeviceInformation instance with a reference to the camera mounted on the desired panel if available,
        ///          any other camera if not, or null if no camera is available.</returns>
        private static async Task<DeviceInformation> FindCameraDeviceByPanelAsync(Panel desiredPanel)
        {
            // Get available devices for capturing pictures
            var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            // Get the desired camera by panel
            DeviceInformation desiredDevice = allVideoDevices.FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == desiredPanel);

            // If there is no device mounted on the desired panel, return the first device found
            return desiredDevice ?? allVideoDevices.FirstOrDefault();
        }


        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                base.OnNavigatedTo(e);

                if (world == null)
                {
                    InitializeWorld();
                    PopulateWorld();
                }
                world?.StartSensors();

                var position = await geolocator.GetGeopositionAsync();
                UpdateLocation(position);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }


        private void PopulateWorld()
        {
            foreach (var evt in world.Elements)
            {
                var tb = new TextBlock
                {
                    Text = evt.Element.Name, DataContext = evt
                };
                markers[evt.Element] = tb;
                LayoutRoot.Children.Add(tb);
            }
        }

        private void InitializeWorld()
        {
            world = new ARWorld<POI>(this, 0.1f, 50, null);
            world.Initialize(pois);
        }


        /*private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            world?.Initialize(ActualWidth, ActualHeight);
        }*/

        /*private void Inclinometer_ReadingChanged(Inclinometer sender, InclinometerReadingChangedEventArgs args)
        {
            if (Interlocked.CompareExchange(ref updating, 1, 0) == 1) return;
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                UpdateElementsOnScreen(args.Reading);
                Interlocked.Exchange(ref updating, 0);
            });
        }*/

        /*private void UpdateElementsOnScreen(InclinometerReading reading)
        {
            if (world == null)
            {
                return;
            }

            var roll = reading.RollDegrees*Math.PI/180.0;
            var pitch = reading.PitchDegrees*Math.PI/180.0;
            var yaw = reading.YawDegrees*Math.PI/180.0;

            foreach (var child in LayoutRoot.Children)
            {
                var fe = (child as FrameworkElement);
                if (fe == null || fe.ActualHeight == 0 || fe.ActualWidth == 0) continue;
                var element = fe.DataContext as IWorldElement<POI>;
                if (element == null) continue;

                var offset = (displayRotation == Rotation.Rotation90 || displayRotation == Rotation.Rotation270) ? world.Offset(element, new Rectangle(0, 0, (int) fe.ActualWidth, (int) fe.ActualHeight), roll, pitch, yaw) : world.Offset(element, new Rectangle(0, 0, (int)fe.ActualWidth, (int)fe.ActualHeight), pitch, roll, yaw);
                if (offset == null)
                {
                    continue;
                }
                if (offset.TranslateX < -ActualWidth)
                {
                    offset.TranslateX = -ActualWidth;
                }
                if (offset.TranslateX > ActualWidth*2)
                {
                    offset.TranslateX = ActualWidth*2;
                }
                offset.TranslateY = ActualHeight/2;
                var scale = world.CalculateScale(element.Element.DistanceMetres);
                fe.RenderTransform = new CompositeTransform
                {
                    TranslateX = offset.TranslateX, TranslateY = offset.TranslateY, ScaleY = scale, ScaleX = scale
                };
            }
        }*/

        private void UpdateElementsOnScreen(double roll, double pitch, double yaw)
        {
        }

        private void UpdateLocation(Geoposition position)
        {
            if (world == null)
            {
                return;
            }

            var currentLocation = new Location {Latitude = position.Coordinate.Point.Position.Latitude, Longitude = position.Coordinate.Point.Position.Longitude};
            /*world.UpdateCentre(currentLocation);
            foreach (var poi in world.ElementsInWorld<POI>())
            {
                var distance = poi.Element.GeoLocation.DistanceInMetres(currentLocation);
                Debug.WriteLine($"distance {distance}");
                poi.Element.DistanceMetres = poi.Element.GeoLocation.DistanceInMetres(currentLocation);
                if (markers.ContainsKey(poi.Element))
                {
                    Debug.WriteLine($"distance away {poi.Element.DistanceAway}");
                    markers[poi.Element].Text = poi.Element.Name;
                }
            }*/
            world.UpdateLocation(currentLocation);
        }

        private void Geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => UpdateLocation(args.Position));
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            world.StopSensors();
        }

        /*private async Task DeactivateSensors()
        {
            displayInformation.OrientationChanged -= MainPage_OrientationChanged;
            await CleanupCameraAsync();
            isPreviewing = false;
            geolocator.PositionChanged -= Geolocator_PositionChanged;
            if (inclinometer != null)
            {
                inclinometer.ReadingChanged -= Inclinometer_ReadingChanged;
            }
        }*/
    }
}
