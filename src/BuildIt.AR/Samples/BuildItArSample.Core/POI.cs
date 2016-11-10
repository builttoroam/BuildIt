using BuildIt.AR;

namespace BuildItArSample.Core
{
    public class POI : ILocationBasedMarker
    {
        public int Id { get; set; }

        public Location GeoLocation { get; set; }

        public double DistanceMetres { get; set; }

        public double DistanceKm => DistanceMetres/1000.0;


        public string DistanceAway => DistanceMetres < 1000 ? $"{(int)DistanceMetres} m" : $"{(int)DistanceKm} km";

        public string Name { get; set; }
    }
}
