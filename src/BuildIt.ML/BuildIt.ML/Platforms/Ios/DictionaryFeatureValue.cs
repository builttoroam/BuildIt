using CoreML;
using Foundation;

namespace BuildIt.ML
{
    public class DictionaryFeatureValue : FeatureValue<NSDictionary<NSObject, NSNumber>>
    {
        public override MLFeatureType Type => MLFeatureType.Dictionary;
    }
}