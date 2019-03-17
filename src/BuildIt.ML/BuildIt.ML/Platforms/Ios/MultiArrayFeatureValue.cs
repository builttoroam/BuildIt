using CoreML;

namespace BuildIt.ML
{
    public class MultiArrayInputFeature : FeatureValue<MLMultiArray>
    {
        public override MLFeatureType Type => MLFeatureType.MultiArray;
    }
}