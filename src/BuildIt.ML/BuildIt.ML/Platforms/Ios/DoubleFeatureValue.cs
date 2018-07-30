using CoreML;

namespace BuildIt.ML.Platforms.Ios
{
    public class DoubleFeatureValue : FeatureValue<double>
    {
        public override MLFeatureType Type => MLFeatureType.Double;
    }
}