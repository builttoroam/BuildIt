using CoreML;
using Foundation;
using System.Threading.Tasks;

namespace BuildIt.ML.Platforms.Ios
{
    public class Classifier
    {
        private const string CoreMLModelExtension = "mlmodelc";
        private MLModel model;

        public async Task InitAsync(string modelName)
        {
            var assetPath = NSBundle.MainBundle.GetUrlForResource(modelName, CoreMLModelExtension);
            model = MLModel.Create(assetPath, out NSError error);
            // TODO: handle when there's an error
        }
    }
}