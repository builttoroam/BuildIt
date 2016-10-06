using BuildIt.AR;

namespace BuildItArSample.Core
{
    public class POI : ILocationBasedMarker
    {
        public Location GeoLocation { get; set; }
        public double Distance { get; set; }
    }
}
