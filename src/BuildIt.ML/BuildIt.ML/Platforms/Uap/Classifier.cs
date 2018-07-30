using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.AI.MachineLearning.Preview;
using Windows.Foundation.Metadata;
using Windows.Storage;

namespace BuildIt.ML.Platforms.Android
{
    public class Classifier
    {
        private const string WindowsMLContract = "Windows.AI.MachineLearning.Preview.MachineLearningPreviewContract";
        private List<string> labels;
        private LearningModelPreview learningModel;

        public async Task InitAsync(string modelName, List<string> labels)
        {
            if (IsWindowsMLSupported())
            {
                this.labels = labels;
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/{modelName}.onnx"));
                learningModel = await LearningModelPreview.LoadModelFromStorageFileAsync(file);
            }

            // TODO: return an error
        }

        private bool IsWindowsMLSupported()
        {
            return ApiInformation.IsApiContractPresent(WindowsMLContract, 1, 0);
        }
    }
}