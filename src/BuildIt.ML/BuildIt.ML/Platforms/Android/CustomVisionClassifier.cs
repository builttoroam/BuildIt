using Android.Graphics;
using Org.Tensorflow;
using Org.Tensorflow.Contrib.Android;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BuildIt.ML
{
    /// <inheritdoc />
    public class CustomVisionClassifier : ICustomVisionClassifier
    {
        private const string OutputName = "loss";
        private const string InputName = "Placeholder";
        private const string DataNormLayerPrefix = "data_bn";
        private const string ModelFileExtension = ".pb";
        private const int ImageWidthHeight = 227;
        private TensorFlowInferenceInterface inferenceInterface;
        private string[] labels;
        private bool hasNormalizationLayer;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        /// <inheritdoc />
        public async Task InitAsync(string modelName, string[] labels)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            this.labels = labels;
            var assets = Android.App.Application.Context.Assets;
            inferenceInterface = new TensorFlowInferenceInterface(assets, string.Format("{0}{1}", modelName, ModelFileExtension));
            var iter = inferenceInterface.Graph().Operations();
            while (iter.HasNext && !hasNormalizationLayer)
            {
                var op = iter.Next() as Operation;
                if (op.Name().Contains(DataNormLayerPrefix))
                {
                    hasNormalizationLayer = true;
                }
            }
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<ImageClassification>> ClassifyAsync(Stream imageStream)
        {
            using (var bitmap = await BitmapFactory.DecodeStreamAsync(imageStream))
            {
                var outputs = new float[labels.Length];
                var floatValues = GetBitmapPixels(bitmap, ImageWidthHeight, ImageWidthHeight, hasNormalizationLayer);

                bitmap.Recycle();
                inferenceInterface.Feed(InputName, floatValues.ToArray(), 1, ImageWidthHeight, ImageWidthHeight, 3);
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

        /// <inheritdoc />
        public async Task<IReadOnlyList<ImageClassification>> ClassifyNativeFrameAsync(object obj)
        {
            var bytes = obj as byte[];
            using (var bitmap = await BitmapFactory.DecodeByteArrayAsync(bytes, 0, bytes.Length))
            {
                var outputs = new float[labels.Length];
                var floatValues = GetBitmapPixels(bitmap, ImageWidthHeight, ImageWidthHeight, hasNormalizationLayer);
                inferenceInterface.Feed(InputName, floatValues.ToArray(), 1, ImageWidthHeight, ImageWidthHeight, 3);
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

        private static float[] GetBitmapPixels(Bitmap bitmap, int width, int height, bool hasNormalizationLayer)
        {
            var floatValues = new float[width * height * 3];
            var imageMeanR = hasNormalizationLayer ? 0f : 123f;
            var imageMeanG = hasNormalizationLayer ? 0f : 117f;
            var imageMeanB = hasNormalizationLayer ? 0f : 104f;

            using (var scaledBitmap = Bitmap.CreateScaledBitmap(bitmap, width, height, false))
            {
                using (var resizedBitmap = scaledBitmap.Copy(Bitmap.Config.Argb8888, false))
                {
                    var intValues = new int[width * height];
                    resizedBitmap.GetPixels(intValues, 0, resizedBitmap.Width, 0, 0, resizedBitmap.Width, resizedBitmap.Height);

                    for (int i = 0; i < intValues.Length; ++i)
                    {
                        var val = intValues[i];

                        floatValues[(i * 3) + 0] = ((val & 0xFF) - imageMeanB) / 1f;
                        floatValues[(i * 3) + 1] = (((val >> 8) & 0xFF) - imageMeanG) / 1f;
                        floatValues[(i * 3) + 2] = (((val >> 16) & 0xFF) - imageMeanR) / 1f;
                    }

                    resizedBitmap.Recycle();
                }

                scaledBitmap.Recycle();
            }

            return floatValues;
        }
    }
}