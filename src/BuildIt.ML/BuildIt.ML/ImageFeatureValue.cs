using System.IO;

namespace BuildIt.ML
{
    public class ImageFeatureValue : FeatureValue<Stream>
    {
        public override FeatureType FeatureType => FeatureType.Image;
    }
}