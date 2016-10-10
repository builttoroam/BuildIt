using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BuildIt.AR
{
    public class ScreenWorld
    {
        public static IDictionary<WorldConfiguration, Vector3> CameraUpConfigurations = new Dictionary<WorldConfiguration, Vector3>
        {
            {WorldConfiguration.Android, Vector3.Up},
            {WorldConfiguration.iOS, Vector3.Down},
            {WorldConfiguration.WindowsMobile, Vector3.Down}
        };

        public static IDictionary<WorldConfiguration, IDictionary<Rotation, Matrix>> OrientationMatrixConfigurations = new Dictionary<WorldConfiguration, IDictionary<Rotation, Matrix>>
        {
            {
                WorldConfiguration.Android, new Dictionary<Rotation, Matrix>()
                {
                    {Rotation.Rotation0, Matrix.CreateRotationY(MathHelper.ToRadians(-90)) * Matrix.CreateRotationZ(MathHelper.ToRadians(90)) * Matrix.CreateRotationX(MathHelper.ToRadians(-90))},
                    {Rotation.Rotation180, Matrix.CreateRotationY(MathHelper.ToRadians(90)) * Matrix.CreateRotationZ(MathHelper.ToRadians(-90)) * Matrix.CreateRotationX(MathHelper.ToRadians(90))},
                    {Rotation.Rotation90, Matrix.CreateRotationY(MathHelper.ToRadians(90))*Matrix.CreateRotationZ(MathHelper.ToRadians(90))},
                    {Rotation.Rotation270, Matrix.CreateRotationY(MathHelper.ToRadians(-90))*Matrix.CreateRotationZ(MathHelper.ToRadians(-90))}
                }
            },
            {
                WorldConfiguration.iOS, new Dictionary<Rotation, Matrix>()
                {
					{Rotation.Rotation0, Matrix.CreateRotationZ(MathHelper.ToRadians(90)) * Matrix.CreateRotationX(MathHelper.ToRadians(90))},
                    {Rotation.Rotation180, Matrix.CreateRotationZ(MathHelper.ToRadians(90)) * Matrix.CreateRotationX(MathHelper.ToRadians(90))},
					{Rotation.Rotation90, Matrix.CreateRotationZ(MathHelper.ToRadians(90)) * Matrix.CreateRotationX(MathHelper.ToRadians(180))},
					{Rotation.Rotation270, Matrix.CreateRotationZ(MathHelper.ToRadians(-90))}
                }
            }
        };

        public List<ILocationBasedMarker> Markers { get; private set; } = new List<ILocationBasedMarker>();

        public ScreenWorld(WorldConfiguration config, Rotation rotation)
        {
            Configuration = config;
            var conf = CameraUpConfigurations[config];
            CameraUp = conf;
            if (OrientationMatrixConfigurations.ContainsKey(config))
            {
                var platformSpecificRotationMatrices = OrientationMatrixConfigurations[config];
                if (platformSpecificRotationMatrices != null)
                {
                    if (platformSpecificRotationMatrices.ContainsKey(rotation))
                    {
                        Debug.WriteLine($"Current Orientation from constructor: {rotation}");
                        WorldAdjustment = platformSpecificRotationMatrices[rotation];
                    }
                }
            }
            Initialize();
        }

        private Viewport Viewport { get; set; }
        private Matrix Projection { get; set; }
        private Matrix View { get; set; }

        public double MinScale { get; set; } = 0.1;

        public double MaxScale { get; set; } = 2.0;

        public double WorldVisualRange { get; set; } = 50.0;

        public double VisualRangeKm { get; private set; }

        public Location CentreOfWorld { get; private set; } = new Location { Latitude = -33.865143, Longitude = 151.209900 };

        private IList<IGenericWorldElement> WorldElements { get; } = new List<IGenericWorldElement>();

        private WorldConfiguration Configuration { get; }

        private Vector3 CameraUp { get; }
        private Matrix WorldAdjustment { get; set; }


        public IEnumerable<IWorldElement<TElement>> ElementsInWorld<TElement>() where TElement : ILocationBasedMarker
        {
            return WorldElements.OfType<IWorldElement<TElement>>().ToArray();
        }

        public bool Initialize(double screenWidth = 500, double screenHeight = 500)
        {
            if (screenWidth <= 0 || screenHeight <= 0) return false;

            // Initialize the viewport and matrixes for 3d projection.
            Viewport = new Viewport(0, 0, (int)screenWidth, (int)screenHeight);
            var aspect = Viewport.AspectRatio;
            Projection = Matrix.CreatePerspectiveFieldOfView(1, aspect, 1, 100);
            View = Matrix.CreateLookAt(new Vector3(0, 0, 1), Vector3.Zero, CameraUp);
            return true;
        }

        public void UpdateWorldAdjustment(Rotation rotation)
        {
			Debug.WriteLine($"Current Orientation from update: {rotation}");
            if (!OrientationMatrixConfigurations.ContainsKey(Configuration)) return;
            var platformSpecificMatrices = OrientationMatrixConfigurations[Configuration];
            if (platformSpecificMatrices == null)
            {
                return;
            }

            if (platformSpecificMatrices.ContainsKey(rotation))
            {
                WorldAdjustment = platformSpecificMatrices[rotation];
            }
        }

        public ScreenOffset Offset<TElement>(IWorldElement<TElement> point, Rectangle bounds, double roll, double pitch, double yaw) where TElement : ILocationBasedMarker
        {
            #region HACK based on bearing

            //var offset = new ScreenOffset();
            //var bearing = CentreOfWorld.Bearing(point.Element.GeoLocation);
            //var diff = yaw - bearing;

            //// let vof be 90 degrees (45 each side of straight ahead
            //var vof = (90.0).ToRad();
            //offset.TranslateX = (Viewport.Width/2)*(1 + (diff/vof)); 
            //offset.TranslateY = Viewport.Height/2;

            //var distance = CentreOfWorld.DistanceInKilometres(point.Element.GeoLocation);
            //offset.Scale = 1; // Do something here with distance and range etc
            //return offset;

            #endregion HACK

            var mat = Matrix.CreateFromYawPitchRoll((float)yaw, (float)pitch, (float)roll);
            var currentAttitude = WorldAdjustment * mat;

            var offset = WorldHelpers.Offset(point.PositionInWorld, bounds, Viewport, Projection, View, currentAttitude);
            return offset;
        }

        public double CalculateScale(double distance)
        {
            var percentage = (VisualRangeKm * 1000.00 - distance) / (VisualRangeKm * 1000.00);
            return percentage * MaxScale + MinScale;
        }

        public void UpdateRangeOfWorld(double rangeInKilometres)
        {
            VisualRangeKm = rangeInKilometres;
            // need to multiply by 10 so calculations can correctly determine what is outside of the visual range
            WorldVisualRange = VisualRangeKm * 10;
            RepositionElements();
        }


        public void UpdateCentre(Location newCentreOfWorld)
        {
            CentreOfWorld = newCentreOfWorld;
            RepositionElements();
        }

        public void AddElementToWorld<TElement>(TElement element) where TElement : ILocationBasedMarker
        {
            var wrapper = new ElementWrapper<TElement> { Element = element };
            WorldElements.Add(wrapper);
            wrapper.PositionInWorld = DeterminePositionInWorld(element);
            //wrapper.PositionInWorld;
        }

        private void RepositionElements()
        {
            foreach (var worldElement in WorldElements)
            {
                worldElement.PositionInWorld = DeterminePositionInWorld(worldElement.ElementWithLocation);
            }
        }

        private Vector3 DeterminePositionInWorld(ILocationBasedMarker element)
        {

            var eventItem = element.GeoLocation;
            var eastWestLocation = new Location { Latitude = CentreOfWorld.Latitude, Longitude = eventItem.Longitude };
            var eastWestDistance = eastWestLocation.DistanceInMetres(CentreOfWorld);
            if (eastWestLocation.Longitude < CentreOfWorld.Longitude)
            {
                eastWestDistance *= -1;
            }

            var northSouthDistance = eventItem.DistanceInMetres(eastWestLocation);

            //Debug.WriteLine("distance " + northSouthDistance);
            if (eventItem.Latitude > eastWestLocation.Latitude)
            {
                northSouthDistance *= -1;
            }

            // Make sure there's a valid range
            if (WorldVisualRange <= 0)
            {
                WorldVisualRange = 1.0;
            }

            // AddDirectionPoints((int)(-eastWestDistance/ 1000.0), 0, (int)(-northSouthDistance / 1000.0), eventItem.Type);
            return new Vector3((float)(eastWestDistance / WorldVisualRange), 0, (float)(northSouthDistance / (WorldVisualRange)));

        }

        private interface IGenericWorldElement
        {
            ILocationBasedMarker ElementWithLocation { get; }
            Vector3 PositionInWorld { get; set; }
        }

        private class ElementWrapper<TElement> : IGenericWorldElement, IWorldElement<TElement> where TElement : ILocationBasedMarker
        {
            public ILocationBasedMarker ElementWithLocation => Element;
            public Vector3 PositionInWorld { get; set; }
            public TElement Element { get; set; }
        }
    }

    public interface ILocationBasedMarker
    {
        int Id { get; set; }

        Location GeoLocation { get; set; }

        double Distance { get; set; }
    }

    public interface IWorldElement<TElement> where TElement : ILocationBasedMarker
    {
        TElement Element { get; }
        Vector3 PositionInWorld { get; }
    }
}
