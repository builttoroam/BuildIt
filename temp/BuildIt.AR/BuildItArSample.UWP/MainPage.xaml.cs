using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Geolocation;
using Windows.Devices.Sensors;
using Windows.Graphics.Display;
using Windows.Media.Capture;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using BuildIt.AR;
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
        private ScreenWorld world;
        private IDictionary<POI, TextBlock> markers = new Dictionary<POI, TextBlock>();
        private int updating;
        List<POI> pois = new List<POI>
            {
                new POI {GeoLocation = new Location {Latitude = -33.832855,
                    Longitude = 151.211989}, Id = 1}, new POI {GeoLocation = new Location {Latitude = -33.839878,
                    Longitude = 151.220633}, Id = 2}, new POI
                {
                    GeoLocation = new Location
                    {
                        Latitude = -33.839309,
                    Longitude = 151.195384
                    },
                    Id = 3
                },
                new POI
                {
                    GeoLocation = new Location
                    {
                        Latitude = -33.848870,
                    Longitude = 151.212342
                    },
                    Id = 4
                }
            };



        public MainPage()
        {
            InitializeComponent();
        }

        private async Task StartPreviewAsync()
        {
            try
            {
                if (mediaCapture == null)
                {
                    var cameraDevice = await FindCameraDeviceByPanelAsync(Panel.Back);
                    mediaCapture = new MediaCapture();
                    var settings = new MediaCaptureInitializationSettings {VideoDeviceId = cameraDevice.Id};
                    await mediaCapture.InitializeAsync(settings);
                    CameraPreview.Source = mediaCapture;
                    await mediaCapture.StartPreviewAsync();
                    isPreviewing = true;
                    var videoRotation = CalculateVideoRotation();
                    mediaCapture.SetPreviewRotation(videoRotation);
                    displayRequest.RequestActive();
                }
                //DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
            }
            catch (UnauthorizedAccessException)
            {
                // This will be thrown if the user denied access to the camera in privacy settings
                Debug.WriteLine("The app was denied access to the camera");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MediaCapture initialization failed. {0}", ex.Message);
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

        private static VideoRotation CalculateVideoRotation()
        {
            var orientation = DisplayInformation.GetForCurrentView().CurrentOrientation;
            var videoRotation = VideoRotation.None;
            switch (orientation)
            {
                case DisplayOrientations.Portrait:
                    videoRotation = VideoRotation.Clockwise90Degrees;
                    break;
                case DisplayOrientations.LandscapeFlipped:
                    videoRotation = VideoRotation.Clockwise180Degrees;
                    break;
                case DisplayOrientations.PortraitFlipped:
                    videoRotation = VideoRotation.Clockwise270Degrees;
                    break;
            }
            return videoRotation;
        }

        private async Task CleanupCameraAsync()
        {
            if (mediaCapture != null)
            {
                if (isPreviewing)
                {
                    await mediaCapture.StopPreviewAsync();
                }

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    CameraPreview.Source = null;
                    displayRequest?.RequestRelease();
                });

                mediaCapture.Dispose();
                mediaCapture = null;
            }

        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                base.OnNavigatedTo(e);
                SizeChanged += MainPage_SizeChanged;
                DisplayInformation.GetForCurrentView().OrientationChanged += MainPage_OrientationChanged;
                await StartPreviewAsync();
                var accessStatus = await Geolocator.RequestAccessAsync();
                geolocator = new Geolocator();
                var position = await geolocator.GetGeopositionAsync();
                UpdateLocation(position);
                geolocator.PositionChanged += Geolocator_PositionChanged;
                inclinometer = Inclinometer.GetDefault();
                if (inclinometer != null)
                {
                    inclinometer.ReadingChanged += Inclinometer_ReadingChanged;
                    inclinometer.ReportInterval = 1;
                }
                if (world == null)
                {
                    InitializeWorld();
                    PopulateWorld();
                }
                // 
                //UpdateElementsOnScreen(inclination);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void PopulateWorld()
        {
            foreach (var evt in world.ElementsInWorld<POI>())
            {
                var tb = new TextBlock
                {
                    Text = evt.Element.DistanceAway,
                    DataContext = evt
                };
                markers[evt.Element] = tb;
                LayoutRoot.Children.Add(tb);
            }

        }

        private void InitializeWorld()
        {
            world = new ScreenWorld(WorldConfiguration.WindowsMobile, Rotation.Rotation0);
            world.Initialize(ActualWidth, ActualHeight);
            foreach (var poi in pois)
            {
                world.AddElementToWorld(poi);
            }
            world.UpdateRangeOfWorld(50);
        }
        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            world?.Initialize(ActualWidth, ActualHeight);
        }

        private void Inclinometer_ReadingChanged(Inclinometer sender, InclinometerReadingChangedEventArgs args)
        {
            if (Interlocked.CompareExchange(ref updating, 1, 0) == 1) return;
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                UpdateElementsOnScreen(args.Reading);
                Interlocked.Exchange(ref updating, 0);
            });
        }

        private void UpdateElementsOnScreen(InclinometerReading reading)
        {
            if (world == null)
            {
                return;
            }

            var roll = reading.RollDegrees * Math.PI / 180.0;
            var pitch = reading.PitchDegrees * Math.PI / 180.0;
            var yaw = reading.YawDegrees * Math.PI / 180.0;

            foreach (var child in LayoutRoot.Children)
            {
                var fe = (child as FrameworkElement);
                if (fe == null || fe.ActualHeight == 0 || fe.ActualWidth == 0) continue;
                var element = fe.DataContext as IWorldElement<POI>;
                if (element == null) continue;

                var offset = world.Offset(element, new Rectangle(0, 0, (int)fe.ActualWidth, (int)fe.ActualHeight), roll, pitch, yaw);
                if (offset == null)
                {
                    continue;
                }
                if (offset.TranslateX < -ActualWidth)
                {
                    offset.TranslateX = -ActualWidth;
                }
                if (offset.TranslateX > ActualWidth * 2)
                {
                    offset.TranslateX = ActualWidth * 2;
                }
                if (offset.TranslateY < -ActualHeight)
                {
                    offset.TranslateY = -ActualHeight;
                }
                if (offset.TranslateY > ActualHeight * 2)
                {
                    offset.TranslateY = ActualHeight * 2;
                }
                var scale = world.CalculateScale(element.Element.Distance);
                fe.RenderTransform = new CompositeTransform
                {
                    TranslateX = offset.TranslateX,
                    TranslateY = offset.TranslateY,
                    ScaleY = scale,
                    ScaleX = scale
                };
            }
        }

        private void UpdateLocation(Geoposition position)
        {
            if (world == null)
            {
                return;
            }

            var currentLocation = new Location {Latitude = position.Coordinate.Point.Position.Latitude, Longitude = position.Coordinate.Point.Position.Longitude};
            world.UpdateCentre(currentLocation);
            foreach (var poi in world.ElementsInWorld<POI>())
            {
                var distance = poi.Element.GeoLocation.DistanceInMetres(currentLocation);
                Debug.WriteLine($"distance {distance}");
                poi.Element.Distance = poi.Element.GeoLocation.DistanceInMetres(currentLocation);
                if (markers.ContainsKey(poi.Element))
                {
                    Debug.WriteLine($"distance away {poi.Element.DistanceAway}");
                    markers[poi.Element].Text = poi.Element.DistanceAway;
                }
            }
        }

        private void Geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => UpdateLocation(args.Position));
        }

        private void MainPage_OrientationChanged(DisplayInformation sender, object args)
        {
            if (isPreviewing)
            {
                var videoRotation = CalculateVideoRotation();
                mediaCapture?.SetPreviewRotation(videoRotation);
            }
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            DisplayInformation.GetForCurrentView().OrientationChanged -= MainPage_OrientationChanged;
            await CleanupCameraAsync();
            geolocator.PositionChanged -= Geolocator_PositionChanged;
            SizeChanged -= MainPage_SizeChanged;
            if (inclinometer != null)
            {
                inclinometer.ReadingChanged -= Inclinometer_ReadingChanged;
            }
        }
    }
}
