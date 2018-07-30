using CoreML;
using CoreVideo;

namespace BuildIt.ML.Platforms.Ios
{
    public class ImageFeatureValue : FeatureValue<CVPixelBuffer>
    {
        public override MLFeatureType Type => MLFeatureType.Image;
    }
}