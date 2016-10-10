using BuildIt.AR;

namespace BuildItArSample.Core
{
    public class POI : ILocationBasedMarker
    {
        public int Id { get; set; }

        public Location GeoLocation { get; set; }

        public double Distance { get; set; }

        public string DistanceAway => Distance < 1000 ? $"{(int)Distance} m" : $"{(int)(Distance / 1000.0)} km";
    }
}
