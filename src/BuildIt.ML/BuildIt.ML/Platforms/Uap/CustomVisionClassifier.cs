using BuildIt.ML.Models;
using BuildIt.ML.Platforms.Uap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.AI.MachineLearning.Preview;
using Windows.Foundation.Metadata;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Storage;

namespace BuildIt.ML.Platforms.Android
{
    public class CustomVisionClassifier
    {
        private const string WindowsMLContract = "Windows.AI.MachineLearning.Preview.MachineLearningPreviewContract";
        private string[] labels;
        private LearningModelPreview learningModel;

        public async Task InitAsync(string modelName, string[] labels)
        {
            if (IsWindowsMLSupported())
            {
                this.labels = labels;
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/{modelName}.onnx"));
                learningModel = await LearningModelPreview.LoadModelFromStorageFileAsync(file);
            }

            // TODO: return an error
        }

        public async Task<IReadOnlyList<ImageClassification>> ClassifyAsync(Stream imageStream)
        {
            var decoder = await BitmapDecoder.CreateAsync(imageStream.AsRandomAccessStream());
            using (var softwareBitmap = await decoder.GetSoftwareBitmapAsync())
            {
                using (var videoFrame = VideoFrame.CreateWithSoftwareBitmap(softwareBitmap))
                {
                    var customVisionOutput = new CustomVisionOutput(labels);
                    var binding = new LearningModelBindingPreview(learningModel);
                    binding.Bind("data", videoFrame);
                    binding.Bind("classLabel", customVisionOutput.ClassLabel);
                    binding.Bind("loss", customVisionOutput.Loss);
                    var result = await learningModel.EvaluateAsync(binding, string.Empty);
                    return customVisionOutput.Loss.Select(l => new ImageClassification(l.Key,  l.Value)).ToList().AsReadOnly();
                }
            }
        }

        private bool IsWindowsMLSupported()
        {
            return ApiInformation.IsApiContractPresent(WindowsMLContract, 1, 0);
        }
    }
}