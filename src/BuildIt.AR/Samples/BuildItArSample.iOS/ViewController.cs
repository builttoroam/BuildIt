using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AVFoundation;
using BuildIt.AR;
using BuildIt.AR.iOS;
using BuildIt.AR.iOS.Utilities;
using BuildItArSample.Core;
using CoreGraphics;
using CoreLocation;
using Foundation;
using UIKit;

namespace BuildItArSample.iOS
{
    public partial class ViewController : UIViewController
    {
        private ARWorld<POI> world;
        private readonly IDictionary<UIView, IWorldElement<POI>> events = new Dictionary<UIView, IWorldElement<POI>>();
        private CameraFeedUtility cameraFeedUtility;

        private readonly List<POI> pois = new List<POI>()
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

        private readonly CLLocationManager locationManager = new CLLocationManager
        {
            PausesLocationUpdatesAutomatically = false
        };

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            cameraFeedUtility = new CameraFeedUtility(View, CameraView);
            NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidChangeStatusBarOrientationNotification, RotationChanged);
        }

        public override void ViewDidUnload()
        {
            base.ViewDidUnload();

            NSNotificationCenter.DefaultCenter.RemoveObserver(UIApplication.DidChangeStatusBarOrientationNotification);
        }

        private void RotationChanged(NSNotification notification)
        {
            //Reset the world
            var currentOrientation = UIApplication.SharedApplication.StatusBarOrientation;
            world.Initialize(pois);
            cameraFeedUtility?.UpdatePreviewRotation(currentOrientation);
        }

        public override void ViewDidAppear(bool animated)
        {
            try
            {
                base.ViewDidAppear(animated);
                cameraFeedUtility.InitAndStartCamera();
                if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
                {
                    locationManager.RequestWhenInUseAuthorization();
                }

                world = new ARWorld<POI>(View, 50, UpdateElementsOnScreen);
                world.Initialize(pois);
                PopulateWorld();
                world.StartSensors();
                locationManager.LocationsUpdated += LocationManager_LocationsUpdated;
                locationManager.StartUpdatingLocation();

                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }


        private void PopulateWorld()
        {
            try
            {
                foreach (var view in events.Keys)
                {
                    view.RemoveFromSuperview();
                }
                foreach (var evt in world.Elements)
                {
                    if (evt?.Element == null) continue;

                    var poiView = new UIView
                    {
                        Tag = evt.Element.Id,
                        Bounds = new CGRect(0, 0, 50, 50),
                        Center = new CGPoint(0, View.Bounds.Height/2)
                    };

                    var distanceLabel = new UILabel
                    {
                        Text = evt.Element.DistanceAway,
                        TextColor = UIColor.White,
                        BackgroundColor = UIColor.Black,
                        TextAlignment = UITextAlignment.Center,
                        Alpha = 0.8f
                    };

                    var frameWidth = distanceLabel.IntrinsicContentSize.Width;
                    distanceLabel.Frame = new CGRect(0,0, frameWidth, 15);
                    poiView.Bounds = new CGRect(0,0, frameWidth, 50);
                    poiView.AddSubview(distanceLabel);
                    CameraView.AddSubview(poiView);
                    events[poiView] = evt;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void LocationManager_LocationsUpdated(object sender, CLLocationsUpdatedEventArgs e)
        {
            try
            {
                var position = e.Locations?.FirstOrDefault();
                if (position == null)
                {
                    return;
                }
                var currentLocation = new Location {Latitude = position.Coordinate.Latitude, Longitude = position.Coordinate.Longitude};
                world.UpdateLocation(currentLocation);
                foreach (var view in events.Keys)
                {
                    var poi = events[view];
                    poi.Element.DistanceMetres = currentLocation.DistanceInMetres(poi.Element.GeoLocation);
                    var distanceLabel = view.Subviews?.FirstOrDefault(v => v is UILabel) as UILabel;
                    if (distanceLabel == null) continue;
                    var distance = events[view].Element.DistanceAway;
                    distanceLabel.Text = distance;
                    var frameWidth = distanceLabel.IntrinsicContentSize.Width;
                    distanceLabel.Frame = new CGRect(0, 0, frameWidth, 15);
                    view.Bounds = new CGRect(0, 0, frameWidth, 50);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void UpdateElementsOnScreen(float roll, float pitch, float yaw)
        {
            foreach (var evt in events)
            {
                var fe = evt.Key;
                if (fe == null || fe.Bounds.Height == 0 || fe.Bounds.Width == 0) continue;

                var element = events[fe];
                if (element == null) continue;

                //Swap pitch & roll value in Portrait

                var offset = world.CalculateOffset(element, (int) fe.Bounds.Width, (int) fe.Bounds.Height, roll, pitch, yaw);

                fe.Hidden = true;

                if (offset == null || element.Element == null) continue;

                var tf = CGAffineTransform.MakeTranslation((float)offset.TranslateX, 0);
                if (offset.Scale > 0)
                {
                    tf.Scale((float)offset.Scale, (float)offset.Scale);
                    fe.Center = new CGPoint(0, View.Bounds.Height / 2);
                    fe.Transform = tf;
                    fe.Hidden = false;
                }
            }
        }

        public override void ViewDidDisappear(bool animated)
        {
            cameraFeedUtility?.StopCamera();
            world?.StopSensors();
            locationManager.LocationsUpdated -= LocationManager_LocationsUpdated;
            locationManager.StopUpdatingLocation();
            base.ViewDidDisappear(animated);
        }
    }
}