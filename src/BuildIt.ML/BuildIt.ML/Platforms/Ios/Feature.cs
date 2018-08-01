using CoreML;

namespace BuildIt.ML
{
    public class Feature
    {
        public string Name { get; set; }

        public virtual MLFeatureType Type { get; set; }
    }
}