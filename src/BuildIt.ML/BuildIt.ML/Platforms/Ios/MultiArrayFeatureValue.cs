using CoreML;

namespace BuildIt.ML.Platforms.Ios
{
    public class MultiArrayInputFeature : FeatureValue<MLMultiArray>
    {
        public override MLFeatureType Type => MLFeatureType.MultiArray;
    }
}