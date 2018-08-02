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

namespace BuildIt.ML
{
    public class CustomVisionClassifier : ICustomVisionClassifier
    {
        private const string WindowsMLContract = "Windows.AI.MachineLearning.Preview.MachineLearningPreviewContract";
        private const string Data = "data";
        private const string ClassLabel = "classLabel";
        private const string Loss = "loss";
        private string[] labels;
        private LearningModelPreview learningModel;
        CustomVisionOutput customVisionOutput;

        public async Task InitAsync(string modelName, string[] labels)
        {
            if (IsWindowsMLSupported())
            {
                this.labels = labels;
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/{modelName}.onnx"));
                learningModel = await LearningModelPreview.LoadModelFromStorageFileAsync(file);
                customVisionOutput = new CustomVisionOutput(labels);
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
                    binding.Bind(Data, videoFrame);
                    binding.Bind(ClassLabel, customVisionOutput.ClassLabel);
                    binding.Bind(Loss, customVisionOutput.Loss);
                    var result = await learningModel.EvaluateAsync(binding, string.Empty);
                    return customVisionOutput.Loss.Select(l => new ImageClassification(l.Key, l.Value)).ToList().AsReadOnly();
                }
            }
        }

        private bool IsWindowsMLSupported()
        {
            return ApiInformation.IsApiContractPresent(WindowsMLContract, 1, 0);
        }

        public async Task<IReadOnlyList<ImageClassification>> ClassifyNativeFrameAsync(object obj)
        {
            var videoFrame = obj as VideoFrame;
            if (videoFrame?.Direct3DSurface == null)
            {
                return new List<ImageClassification>();
            }

            var binding = new LearningModelBindingPreview(learningModel);
            binding.Bind(Data, videoFrame);
            binding.Bind(ClassLabel, customVisionOutput.ClassLabel);
            binding.Bind(Loss, customVisionOutput.Loss);
            var result = await learningModel.EvaluateAsync(binding, string.Empty);
            return customVisionOutput.Loss.Select(l => new ImageClassification(l.Key, l.Value)).ToList().AsReadOnly();
        }
    }
}