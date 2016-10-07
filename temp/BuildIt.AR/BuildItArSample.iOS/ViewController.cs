using System;
using System.Collections.Generic;
using System.Threading;
using AVFoundation;
using BuildIt.AR;
using BuildItArSample.Core;
using CoreGraphics;
using CoreLocation;
using CoreMotion;
using Foundation;
using UIKit;

namespace BuildItArSample.iOS
{
    public partial class ViewController : UIViewController
    {
        private readonly IDictionary<UIInterfaceOrientation, AVCaptureVideoOrientation> configDicByRotationChanged = new Dictionary<UIInterfaceOrientation, AVCaptureVideoOrientation>
        {
            {UIInterfaceOrientation.LandscapeLeft, AVCaptureVideoOrientation.LandscapeLeft},
            {UIInterfaceOrientation.LandscapeRight, AVCaptureVideoOrientation.LandscapeRight},
            {UIInterfaceOrientation.Portrait, AVCaptureVideoOrientation.Portrait},
            {UIInterfaceOrientation.PortraitUpsideDown, AVCaptureVideoOrientation.PortraitUpsideDown},
            {UIInterfaceOrientation.Unknown, AVCaptureVideoOrientation.Portrait}
        };

        private AVCaptureSession session;
        private AVCaptureVideoPreviewLayer previewLayer;
        private CMMotionManager motion;
        CGRect cameraBounds;
        private ScreenWorld world;
        private int updating;
        private IDictionary<UIView, IWorldElement<POI>> events = new Dictionary<UIView, IWorldElement<POI>>();

        private CLLocationManager locationManager = new CLLocationManager { PausesLocationUpdatesAutomatically = false };

        public ViewController(IntPtr handle) : base(handle)
        {
        }



        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            InitCamera();
            InitializeWorld();
            motion = new CMMotionManager();
            motion.StartDeviceMotionUpdates(CMAttitudeReferenceFrame.XMagneticNorthZVertical, NSOperationQueue.CurrentQueue, MotionHandler);
            
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                locationManager.RequestAlwaysAuthorization(); // works in background
                                                     //locMgr.RequestWhenInUseAuthorization (); // only in foreground
            }

            if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
            {
                locationManager.AllowsBackgroundLocationUpdates = true;
            }
            locationManager.LocationsUpdated += LocationManager_LocationsUpdated;
            locationManager.StartUpdatingLocation();


            PopulateWorld();
            world.UpdateRangeOfWorld(50);
        }

        private void PopulateWorld()
        {
            foreach (var evt in world.ElementsInWorld<POI>())
            {
                
            }
        }

        private void LocationManager_LocationsUpdated(object sender, CLLocationsUpdatedEventArgs e)
        {
          
        }

        private void InitializeWorld()
        {
            var rotation = UIApplication.SharedApplication.StatusBarOrientation.ToRotationEnum();
            world = new ScreenWorld(WorldConfiguration.iOS, rotation);
            world.Initialize(View.Bounds.Width, View.Bounds.Height);
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

        private void MotionHandler(CMDeviceMotion data, NSError error)
        {
            try
            {
                if (Interlocked.CompareExchange(ref updating, 1, 0) == 1) return;
                InvokeOnMainThread(() =>
                {
                    if (data != null)
                    {
                        UpdateElementsOnScreen((float)data.Attitude.Roll, (float)data.Attitude.Pitch, (float)data.Attitude.Yaw);
                        Interlocked.Exchange(ref updating, 0);
                    }
                });
            }
            catch (Exception ex)
            {
            }
        }

        private void UpdateElementsOnScreen(float roll, float pitch, float yaw)
        {
            var currentOrientation = UIApplication.SharedApplication.StatusBarOrientation;

            foreach (var evt in events)
            {
                var fe = evt.Key;
                if (fe == null || fe.Bounds.Height == 0 || fe.Bounds.Width == 0) continue;

                var element = events[fe];
                if (element == null) continue;

                //Swap pitch & roll value in Portrait
                var offset = currentOrientation.IsLandscape() ?
                           world.Offset(element, new Rectangle(0,0, (int)fe.Bounds.Width, (int)fe.Bounds.Height), roll, pitch, yaw)
                         : world.Offset(element, new Rectangle(0, 0, (int)fe.Bounds.Width, (int)fe.Bounds.Height), pitch, roll, yaw);

                fe.Hidden = true;

                if (offset == null || element.Element == null) continue;
                var offsetScale = world.CalculateScale(element.Element.Distance);

                var tf =
                    CGAffineTransform.MakeTranslation((float)offset.TranslateX, 0);
                if (offsetScale > 0)
                {
                    tf.Scale((float)offsetScale, (float)offsetScale);
                    fe.Center = new CGPoint(0, View.Bounds.Height / 2);
                    fe.Transform = tf;
                    fe.Hidden = false;
                }
            }
        }

        private void InitCamera()
        {
            session = new AVCaptureSession
            {
                SessionPreset = AVCaptureSession.PresetMedium
            };
            var captureDevice = AVCaptureDevice.DefaultDeviceWithMediaType(AVMediaType.Video);

            NSError error;
            var videoInput = AVCaptureDeviceInput.FromDevice(captureDevice, out error);

            if (videoInput == null || !session.CanAddInput(videoInput)) return;
            session.AddInput(videoInput);
            previewLayer = new AVCaptureVideoPreviewLayer(session) {Frame = cameraBounds};

            previewLayer.Connection.VideoOrientation = configDicByRotationChanged[UIApplication.SharedApplication.StatusBarOrientation];//AVCaptureVideoOrientation.LandscapeRight;
            previewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
            CameraView.Layer.AddSublayer(previewLayer);
            session.StartRunning();
        }

        public override void ViewDidDisappear(bool animated)
        {
            session?.StopRunning();
            motion?.StopDeviceMotionUpdates();
            locationManager.LocationsUpdated -= LocationManager_LocationsUpdated;
            locationManager.StopUpdatingLocation();
            base.ViewDidDisappear(animated);
        }
    }
}