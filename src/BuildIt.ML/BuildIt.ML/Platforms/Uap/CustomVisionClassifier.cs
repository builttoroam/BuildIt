using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.AI.MachineLearning.Preview;
using Windows.Graphics.DirectX.Direct3D11;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Storage;

namespace BuildIt.ML
{
    public class CustomVisionClassifier : ICustomVisionClassifier
    {
        private const string Data = "data";
        private const string ClassLabel = "classLabel";
        private const string Loss = "loss";
        private string[] labels;
        private LearningModelPreview learningModel;
        private CustomVisionOutput customVisionOutput;

        public async Task InitAsync(string modelName, string[] labels)
        {
            this.labels = labels;
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/{modelName}.onnx"));
            learningModel = await LearningModelPreview.LoadModelFromStorageFileAsync(file);
            customVisionOutput = new CustomVisionOutput(labels);
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

        public async Task<IReadOnlyList<ImageClassification>> ClassifyNativeFrameAsync(object obj)
        {
            try
            {
                using (var surface = obj as IDirect3DSurface)
                {
                    if (surface != null)
                    {
                        using (var videoFrame = VideoFrame.CreateWithDirect3D11Surface(surface))
                        {
                            var binding = new LearningModelBindingPreview(learningModel);
                            binding.Bind(Data, videoFrame);
                            binding.Bind(ClassLabel, customVisionOutput.ClassLabel);
                            binding.Bind(Loss, customVisionOutput.Loss);
                            var result = await learningModel.EvaluateAsync(binding, string.Empty);
                            return customVisionOutput.Loss.Select(l => new ImageClassification(l.Key, l.Value)).ToList().AsReadOnly();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return new List<ImageClassification>().AsReadOnly();
        }
    }
}