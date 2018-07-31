using Android.Graphics;
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

        public async Task ClassifyAsync(IEnumerable<Feature> inputFeatures)
        {
            foreach (var inputFeature in inputFeatures)
            {
                switch (inputFeature)
                {
                    case ImageFeatureValue imageFeatureValue:
                        using (var bitmap = await BitmapFactory.DecodeStreamAsync(imageFeatureValue.Value))
                        {
                            var floatValues = new float[227 * 227 * 3];
                            using (var scaledBitmap = Bitmap.CreateScaledBitmap(bitmap, 227, 227, false))
                            {
                                using (var resizedBitmap = scaledBitmap.Copy(Bitmap.Config.Argb8888, false))
                                {
                                    var intValues = new int[227 * 227];
                                    resizedBitmap.GetPixels(intValues, 0, 227, 0, 0, 227, 227);
                                    for (int i = 0; i < intValues.Length; ++i)
                                    {
                                        var val = intValues[i];
                                        floatValues[i * 3 + 0] = ((val & 0xFF) - 104);
                                        floatValues[i * 3 + 1] = (((val >> 8) & 0xFF) - 117);
                                        floatValues[i * 3 + 2] = (((val >> 16) & 0xFF) - 123);
                                    }

                                    resizedBitmap.Recycle();
                                }

                                scaledBitmap.Recycle();
                            }

                            inferenceInterface.Feed(imageFeatureValue.Name, floatValues.ToArray(), 227, 227, 3);
                        }

                        break;
                }
            }

        }
    }
}