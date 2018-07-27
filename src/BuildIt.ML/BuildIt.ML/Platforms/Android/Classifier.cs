using Org.Tensorflow.Contrib.Android;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BuildIt.ML.Platforms.Android
{
    public class Classifier
    {
        private TensorFlowInferenceInterface inferenceInterface;
        private List<string> labels;

        public async Task InitAsync(string modelName, string labelsFileName)
        {
            var assets = global::Android.App.Application.Context.Assets;

            // TODO: investigate if list of labels can just be passed in without reading from the file
            using (var sr = new StreamReader(assets.Open(labelsFileName)))
            {
                var content = sr.ReadToEnd();
                labels = content.Split('\n').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();
            }

            inferenceInterface = new TensorFlowInferenceInterface(assets, modelName);
        }
    }
}