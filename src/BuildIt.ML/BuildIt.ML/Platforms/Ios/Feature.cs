using CoreML;

namespace BuildIt.ML.Platforms.Ios
{
    public class Feature
    {
        public string Name { get; set; }

        public virtual MLFeatureType Type { get; set; }
    }
}