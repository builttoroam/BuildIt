using CoreML;

namespace BuildIt.ML.Platforms.Ios
{
    public class StringFeatureValue : FeatureValue<string>
    {
        public override MLFeatureType Type => MLFeatureType.String;
    }
}