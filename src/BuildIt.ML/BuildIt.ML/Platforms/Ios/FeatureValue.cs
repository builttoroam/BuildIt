using CoreML;

namespace BuildIt.ML
{
    public abstract class FeatureValue<T> : Feature
    {
        public T Value { get; set; }
    }
}