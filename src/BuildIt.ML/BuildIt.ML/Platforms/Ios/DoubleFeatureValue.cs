using CoreML;

namespace BuildIt.ML
{
    public class DoubleFeatureValue : FeatureValue<double>
    {
        public override MLFeatureType Type => MLFeatureType.Double;
    }
}