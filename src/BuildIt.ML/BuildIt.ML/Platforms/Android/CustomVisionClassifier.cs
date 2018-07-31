using Android.Graphics;
using BuildIt.ML.Models;
using Org.Tensorflow.Contrib.Android;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BuildIt.ML.Platforms.Android
{
    public class CustomVisionClassifier
    {
        private const string OutputName = "loss";
        private const string InputName = "Placeholder";
        private TensorFlowInferenceInterface inferenceInterface;
        private string[] labels;

        public async Task InitAsync(string modelName, string[] labels)
        {
            this.labels = labels;
            var assets = global::Android.App.Application.Context.Assets;

            inferenceInterface = new TensorFlowInferenceInterface(assets, modelName);
        }

        public async Task<IReadOnlyList<ImageClassification>> ClassifyAsync(Stream imageStream)
        {
            using (var bitmap = await BitmapFactory.DecodeStreamAsync(imageStream))
            {
                var outputs = new float[labels.Length];
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

                bitmap.Recycle();
                inferenceInterface.Feed(InputName, floatValues.ToArray(), 1, 227, 227, 3);
                inferenceInterface.Run(new string[] { OutputName });
                inferenceInterface.Fetch(OutputName, outputs);
                var imageClassifications = new List<ImageClassification>();
                for (var i = 0; i < outputs.Length; i++)
                {
                    imageClassifications.Add(new ImageClassification(labels[i], outputs[i]));
                }

                return imageClassifications.AsReadOnly();
            }
        }
    }
}