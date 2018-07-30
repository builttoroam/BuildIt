using CoreML;

namespace BuildIt.ML.Platforms.Ios
{
    public abstract class FeatureValue<T> : Feature
    {
        public T Value { get; set; }
    }
}