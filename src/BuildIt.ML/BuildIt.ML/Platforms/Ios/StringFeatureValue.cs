using CoreML;

namespace BuildIt.ML
{
    public class StringFeatureValue : FeatureValue<string>
    {
        public override MLFeatureType Type => MLFeatureType.String;
    }
}