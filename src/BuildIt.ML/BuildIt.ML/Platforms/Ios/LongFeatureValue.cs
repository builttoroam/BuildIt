using CoreML;

namespace BuildIt.ML
{
    public class LongFeatureValue : FeatureValue<long>
    {
        public override MLFeatureType Type => MLFeatureType.Int64;
    }
}