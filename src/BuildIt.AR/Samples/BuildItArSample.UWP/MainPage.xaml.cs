using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.Devices.Geolocation;
using Windows.Devices.Sensors;
using Windows.Graphics.Display;
using Windows.Media.Capture;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using BuildIt.AR;
using BuildIt.AR.UWP;
using BuildIt.AR.UWP.Extensions;
using BuildIt.AR.UWP.Utilities;
using BuildItArSample.Core;


namespace BuildItArSample.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        bool isPreviewing;
        private Geolocator geolocator;
        private Inclinometer inclinometer;
        private ARWorld<POI> world;
        private readonly DisplayInformation displayInformation = DisplayInformation.GetForCurrentView();
        private IDictionary<POI, TextBlock> poiMarkers = new Dictionary<POI, TextBlock>();
        private int updating;
        private CameraFeedUtility cameraFeedUtility;
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


        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                base.OnNavigatedTo(e);
                displayInformation.OrientationChanged += DisplayInformation_OrientationChanged;
                if (world == null)
                {
                    InitializeWorld();
                    PopulateWorld();
                }
                
                if (cameraFeedUtility == null)
                {
                    cameraFeedUtility = new CameraFeedUtility(CameraPreview, Dispatcher);
                }
                if (world != null)
                {
                    world.StartSensors();
                    
                    await cameraFeedUtility.StartPreviewAsync(world.Rotation.ToVideoRotation());
                    world.UpdateRotation();
                    cameraFeedUtility.UpdatePreviewRotation(world.Rotation);
                }
                geolocator = new Geolocator();
                geolocator.PositionChanged += Geolocator_PositionChanged;
                var position = await geolocator.GetGeopositionAsync();
                UpdateLocation(position);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void DisplayInformation_OrientationChanged(DisplayInformation sender, object args)
        {
            if (world == null)
            {
                return;
            }

            world.UpdateRotation();
            cameraFeedUtility?.UpdatePreviewRotation(world.Rotation);
        }

        private void PopulateWorld()
        {
            foreach (var evt in world.Elements)
            {
                var tb = new TextBlock
                {
                    Text = evt.Element.Name
                };
                poiMarkers[evt.Element] = tb;
                LayoutRoot.Children.Add(tb);
            }
        }

        private void InitializeWorld()
        {
            world = new ARWorld<POI>(this, 50, UpdateElementsOnScreen);
            world.Initialize(pois);
        }

        private void UpdateElementsOnScreen(double roll, double pitch, double yaw)
        {
            try
            {
                foreach (var element in world.Elements)
                {
                    var poiMarker = poiMarkers[element.Element];
                    if (poiMarker.ActualHeight == 0 || poiMarker.ActualWidth == 0) continue;
                    var offset = world.CalculateOffset(element, (int)poiMarker.ActualWidth, (int)poiMarker.ActualHeight, roll, pitch, yaw);
                    if (offset == null)
                    {
                        continue;
                    }
                    poiMarker.Text = element.Element.DistanceAway;
                    if (offset.TranslateX < -ActualWidth)
                    {
                        offset.TranslateX = -ActualWidth;
                    }
                    if (offset.TranslateX > ActualWidth * 2)
                    {
                        offset.TranslateX = ActualWidth * 2;
                    }
                    offset.TranslateY = ActualHeight / 2;
                    
                    poiMarker.RenderTransform = new CompositeTransform
                    {
                        TranslateX = offset.TranslateX,
                        TranslateY = offset.TranslateY,
                        ScaleY = offset.Scale,
                        ScaleX = offset.Scale
                    };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        

        private void UpdateLocation(Geoposition position)
        {
            if (world == null)
            {
                return;
            }

            var currentLocation = new Location {Latitude = position.Coordinate.Point.Position.Latitude, Longitude = position.Coordinate.Point.Position.Longitude};
            world.UpdateLocation(currentLocation);
        }

        private void Geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => UpdateLocation(args.Position));
        }


        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            displayInformation.OrientationChanged -= DisplayInformation_OrientationChanged;
            world.StopSensors();
            await cameraFeedUtility.CleanupCameraAsync();
            if (geolocator != null)
            {
                geolocator.PositionChanged -= Geolocator_PositionChanged;
            }
        }
    }
}
