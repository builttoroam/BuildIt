using CoreML;

namespace BuildIt.ML.Platforms.Ios
{
    public class LongFeatureValue : FeatureValue<long>
    {
        public override MLFeatureType Type => MLFeatureType.Int64;
    }
}