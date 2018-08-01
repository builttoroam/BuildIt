using BuildIt.ML.Interfaces;
using BuildIt.ML.Models;
using CoreGraphics;
using CoreML;
using Foundation;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vision;

namespace BuildIt.ML.Platforms.Ios
{
    public class CustomVisionClassifier : ICustomVisionClassifier
    {
        private const string CoreMLModelExtension = "mlmodelc";
        private VNCoreMLModel model;

        public Task InitAsync(string modelName, string[] labels)
        {
            var assetPath = NSBundle.MainBundle.GetUrlForResource(modelName, CoreMLModelExtension) ?? CompileModel(modelName);

            // TODO: handle when there's an error
            var mlModel = MLModel.Create(assetPath, out NSError error);

            model = VNCoreMLModel.FromMLModel(mlModel, out error);

            return Task.CompletedTask;
        }

        public async Task<IReadOnlyList<ImageClassification>> ClassifyAsync(Stream imageStream)
        {
            var tcs = new TaskCompletionSource<IEnumerable<ImageClassification>>();
            var request = new VNCoreMLRequest(model, (r, e) =>
            {
                if (e != null)
                {
                    // TODO: handle errors
                }
                else
                {
                    var classifications = r.GetResults<VNClassificationObservation>();
                    tcs.SetResult(classifications.Select(c => new ImageClassification(c.Identifier, c.Confidence)));
                }
            });
            using (var image = await imageStream.ToUIImageAsync())
            {
                var buffer = image.ToCVPixelBuffer(new CGSize(227, 227));
                var requestHandler = new VNImageRequestHandler(buffer, new NSDictionary());
                requestHandler.Perform(new[] { request }, out NSError error);
                if (error != null)
                {
                    // TODO: handle error
                }

                var imageClassifications = await tcs.Task;
                return imageClassifications.ToList().AsReadOnly();
            }
        }

        private NSUrl CompileModel(string modelName)
        {
            var uncompiled = NSBundle.MainBundle.GetUrlForResource(modelName, "mlmodel");
            var modelPath = MLModel.CompileModel(uncompiled, out NSError err);

            if (err != null)
                throw new NSErrorException(err);

            return modelPath;
        }
    }
}