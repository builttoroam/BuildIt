using CoreML;
using CoreVideo;

namespace BuildIt.ML
{
    public class ImageFeatureValue : FeatureValue<CVPixelBuffer>
    {
        public override MLFeatureType Type => MLFeatureType.Image;
    }
}